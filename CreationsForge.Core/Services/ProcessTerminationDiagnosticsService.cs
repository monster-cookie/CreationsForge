using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Services.Interfaces;
using Serilog;

namespace CreationsForge.Core.Services;

public class ProcessTerminationDiagnosticsService : IProcessTerminationDiagnosticsService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly object SyncRoot = new();
    private readonly IApplicationConfigurationStore ConfigurationStore;
    private readonly ILogger Logger = Log.ForContext<ProcessTerminationDiagnosticsService>();
    private readonly CancellationTokenSource TerminationCancellationTokenSource = new();
    private readonly List<PosixSignalRegistration> PosixSignalRegistrations = new();
    private string? SessionPath;
    private SessionState? CurrentSession;
    private bool Started;
    private bool Disposed;

    public ProcessTerminationDiagnosticsService(IApplicationConfigurationStore configurationStore)
    {
        ConfigurationStore = configurationStore;
    }

    public CancellationToken TerminationToken => TerminationCancellationTokenSource.Token;

    public void StartSession(string surfaceName, string? logPath)
    {
        lock (SyncRoot)
        {
            if (Started)
            {
                return;
            }

            Started = true;
            SessionPath = Path.Combine(ConfigurationStore.Current.ApplicationDataDirectory, "CreationsForge.Session.json");
            Directory.CreateDirectory(ConfigurationStore.Current.ApplicationDataDirectory);
            LogPreviousUncleanSession();
            CurrentSession = new SessionState
            {
                ProcessId = Environment.ProcessId,
                SurfaceName = surfaceName,
                StartedAtUTC = DateTime.UtcNow,
                LastHeartbeatUTC = DateTime.UtcNow,
                LogPath = logPath,
                CleanShutdown = false
            };
            UpdateProcessSnapshot(CurrentSession);
            WriteSession();
            RegisterHandlers();
            Logger.Information(
                "Started process termination diagnostics for {SurfaceName}; pid: {ProcessId}; session path: {SessionPath}",
                surfaceName,
                CurrentSession.ProcessId,
                SessionPath);
        }
    }

    public void UpdateHeartbeat(string phaseName, GameImportProgressDTO? progress = null)
    {
        lock (SyncRoot)
        {
            if (CurrentSession == null)
            {
                return;
            }

            CurrentSession.LastHeartbeatUTC = DateTime.UtcNow;
            CurrentSession.LastPhase = phaseName;
            CurrentSession.LastStatusText = progress?.StatusText;
            CurrentSession.LastDetailText = progress?.DetailText;
            CurrentSession.LastGame = progress?.CurrentPluginName;
            UpdateProcessSnapshot(CurrentSession);
            WriteSession();
        }
    }

    public void MarkCleanShutdown(string reason)
    {
        lock (SyncRoot)
        {
            if (CurrentSession == null)
            {
                return;
            }

            CurrentSession.CleanShutdown = true;
            CurrentSession.ShutdownReason = reason;
            CurrentSession.CompletedAtUTC = DateTime.UtcNow;
            CurrentSession.LastHeartbeatUTC = DateTime.UtcNow;
            UpdateProcessSnapshot(CurrentSession);
            WriteSession();
            Logger.Information("Marked CreationsForge session as clean shutdown; reason: {ShutdownReason}", reason);
        }
    }

    public void Dispose()
    {
        lock (SyncRoot)
        {
            if (Disposed)
            {
                return;
            }

            Disposed = true;
            Console.CancelKeyPress -= OnConsoleCancelKeyPress;
            AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
            foreach (var registration in PosixSignalRegistrations)
            {
                registration.Dispose();
            }

            PosixSignalRegistrations.Clear();
            TerminationCancellationTokenSource.Dispose();
        }
    }

    private void RegisterHandlers()
    {
        Console.CancelKeyPress += OnConsoleCancelKeyPress;
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            PosixSignalRegistrations.Add(PosixSignalRegistration.Create(PosixSignal.SIGTERM, OnPosixSignal));
            PosixSignalRegistrations.Add(PosixSignalRegistration.Create(PosixSignal.SIGINT, OnPosixSignal));
            PosixSignalRegistrations.Add(PosixSignalRegistration.Create(PosixSignal.SIGHUP, OnPosixSignal));
        }
    }

    private void OnConsoleCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        RequestTermination($"console {e.SpecialKey}", cancelDefaultTermination: true);
        e.Cancel = true;
    }

    private void OnPosixSignal(PosixSignalContext context)
    {
        RequestTermination(context.Signal.ToString(), cancelDefaultTermination: true);
        context.Cancel = true;
    }

    private void OnProcessExit(object? sender, EventArgs e)
    {
        lock (SyncRoot)
        {
            if (CurrentSession?.CleanShutdown == false)
            {
                CurrentSession.LastHeartbeatUTC = DateTime.UtcNow;
                CurrentSession.ShutdownReason = "process exit";
                UpdateProcessSnapshot(CurrentSession);
                WriteSession();
            }
        }
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var message = e.ExceptionObject is Exception exception ? exception.ToString() : e.ExceptionObject?.ToString();
        lock (SyncRoot)
        {
            if (CurrentSession != null)
            {
                CurrentSession.LastUnhandledException = message;
                CurrentSession.LastHeartbeatUTC = DateTime.UtcNow;
                UpdateProcessSnapshot(CurrentSession);
                WriteSession();
            }
        }
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        lock (SyncRoot)
        {
            if (CurrentSession != null)
            {
                CurrentSession.LastUnhandledException = e.Exception.ToString();
                CurrentSession.LastHeartbeatUTC = DateTime.UtcNow;
                UpdateProcessSnapshot(CurrentSession);
                WriteSession();
            }
        }
    }

    private void RequestTermination(string reason, bool cancelDefaultTermination)
    {
        lock (SyncRoot)
        {
            if (CurrentSession != null)
            {
                CurrentSession.TerminationRequested = true;
                CurrentSession.TerminationReason = reason;
                CurrentSession.LastHeartbeatUTC = DateTime.UtcNow;
                UpdateProcessSnapshot(CurrentSession);
                WriteSession();
            }
        }

        Logger.Warning(
            "Process termination requested by {TerminationReason}; cancel default termination: {CancelDefaultTermination}",
            reason,
            cancelDefaultTermination);
        if (!TerminationCancellationTokenSource.IsCancellationRequested)
        {
            TerminationCancellationTokenSource.Cancel();
        }
    }

    private void LogPreviousUncleanSession()
    {
        if (SessionPath == null || !File.Exists(SessionPath))
        {
            return;
        }

        try
        {
            var json = File.ReadAllText(SessionPath);
            var previous = JsonSerializer.Deserialize<SessionState>(json, SerializerOptions);
            if (previous is not null && !previous.CleanShutdown)
            {
                Logger.Warning(
                    "Previous CreationsForge session ended unexpectedly; pid: {PreviousProcessId}; surface: {SurfaceName}; started: {StartedAtUTC}; last heartbeat: {LastHeartbeatUTC}; phase: {LastPhase}; status: {LastStatusText}; detail: {LastDetailText}; managed bytes: {ManagedBytes}; working set bytes: {WorkingSetBytes}; private bytes: {PrivateBytes}; handle count: {HandleCount}; thread count: {ThreadCount}; log path: {LogPath}; termination requested: {TerminationRequested}; termination reason: {TerminationReason}; unhandled exception: {LastUnhandledException}",
                    previous.ProcessId,
                    previous.SurfaceName,
                    previous.StartedAtUTC,
                    previous.LastHeartbeatUTC,
                    previous.LastPhase,
                    previous.LastStatusText,
                    previous.LastDetailText,
                    previous.ManagedBytes,
                    previous.WorkingSetBytes,
                    previous.PrivateBytes,
                    previous.HandleCount,
                    previous.ThreadCount,
                    previous.LogPath,
                    previous.TerminationRequested,
                    previous.TerminationReason,
                    previous.LastUnhandledException);
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException)
        {
            Logger.Warning(exception, "Unable to read previous CreationsForge session diagnostics from {SessionPath}", SessionPath);
        }
    }

    private static void UpdateProcessSnapshot(SessionState session)
    {
        session.ManagedBytes = GC.GetTotalMemory(forceFullCollection: false);
        using var process = Process.GetCurrentProcess();
        session.ProcessName = process.ProcessName;
        session.WorkingSetBytes = process.WorkingSet64;
        session.PrivateBytes = process.PrivateMemorySize64;
        session.HandleCount = process.HandleCount;
        session.ThreadCount = process.Threads.Count;
    }

    private void WriteSession()
    {
        if (SessionPath == null || CurrentSession == null)
        {
            return;
        }

        try
        {
            File.WriteAllText(SessionPath, JsonSerializer.Serialize(CurrentSession, SerializerOptions));
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            Logger.Warning(exception, "Unable to write CreationsForge session diagnostics to {SessionPath}", SessionPath);
        }
    }

    private sealed class SessionState
    {
        public int ProcessId { get; set; }

        public string? ProcessName { get; set; }

        public string? SurfaceName { get; set; }

        public DateTime StartedAtUTC { get; set; }

        public DateTime LastHeartbeatUTC { get; set; }

        public DateTime? CompletedAtUTC { get; set; }

        public bool CleanShutdown { get; set; }

        public string? ShutdownReason { get; set; }

        public string? LogPath { get; set; }

        public string? LastPhase { get; set; }

        public string? LastStatusText { get; set; }

        public string? LastDetailText { get; set; }

        public string? LastGame { get; set; }

        public long ManagedBytes { get; set; }

        public long WorkingSetBytes { get; set; }

        public long PrivateBytes { get; set; }

        public int HandleCount { get; set; }

        public int ThreadCount { get; set; }

        public bool TerminationRequested { get; set; }

        public string? TerminationReason { get; set; }

        public string? LastUnhandledException { get; set; }
    }
}
