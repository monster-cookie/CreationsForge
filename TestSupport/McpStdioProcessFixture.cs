using System.Diagnostics;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace CreationsForge.TestSupport;

/// <summary>
/// Owns a real MCP child process while the SDK communicates over its physical standard streams.
/// </summary>
internal sealed class McpStdioProcessFixture : IAsyncDisposable
{
    /// <summary>The maximum time allowed for normal host exit after standard input closes.</summary>
    private static readonly TimeSpan GracefulExitTimeout = TimeSpan.FromSeconds(15);

    /// <summary>The maximum time allowed for each failure-cleanup step.</summary>
    private static readonly TimeSpan CleanupTimeout = TimeSpan.FromSeconds(5);

    /// <summary>The owned MCP child process.</summary>
    private readonly Process ChildProcess;

    /// <summary>The asynchronous complete standard-error capture.</summary>
    private readonly Task<string> StandardErrorTask;

    /// <summary>Tracks whether normal completion has already started.</summary>
    private int CompletionStarted;

    /// <summary>Tracks whether the SDK client has already been disposed.</summary>
    private int ClientDisposalStarted;

    /// <summary>Initializes ownership after the SDK has completed protocol initialization.</summary>
    /// <param name="childProcess">The running MCP child process.</param>
    /// <param name="client">The initialized SDK client connected to the child streams.</param>
    /// <param name="standardErrorTask">The complete asynchronous standard-error capture.</param>
    private McpStdioProcessFixture(
        Process childProcess,
        McpClient client,
        Task<string> standardErrorTask)
    {
        ChildProcess = childProcess;
        Client = client;
        StandardErrorTask = standardErrorTask;
    }

    /// <summary>Gets the initialized SDK client connected to the real child process.</summary>
    public McpClient Client { get; }

    /// <summary>Starts the production MCP host and initializes an SDK client over its physical stdio streams.</summary>
    /// <param name="mcpAssemblyPath">The absolute built MCP assembly path.</param>
    /// <param name="cancellationToken">A token that bounds process and protocol initialization.</param>
    /// <returns>An owned initialized process fixture.</returns>
    public static async Task<McpStdioProcessFixture> StartAsync(
        string mcpAssemblyPath,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mcpAssemblyPath);
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        startInfo.ArgumentList.Add(mcpAssemblyPath);

        var process = new Process
        {
            StartInfo = startInfo,
        };
        if (!process.Start())
        {
            process.Dispose();
            throw new InvalidOperationException("The CreationsForge MCP child process did not start.");
        }

        var standardErrorTask = process.StandardError.ReadToEndAsync();
        try
        {
            var transport = new StreamClientTransport(
                process.StandardInput.BaseStream,
                process.StandardOutput.BaseStream);
            var client = await McpClient.CreateAsync(
                transport,
                cancellationToken: cancellationToken);
            return new McpStdioProcessFixture(process, client, standardErrorTask);
        }
        catch
        {
            await StopProcessAsync(process, standardErrorTask);
            process.Dispose();
            throw;
        }
    }

    /// <summary>Closes child input, requires normal bounded process exit, captures diagnostics, and then disposes the SDK client.</summary>
    /// <param name="cancellationToken">A token that bounds normal completion in addition to the fixed exit limit.</param>
    /// <returns>The actual child exit code and complete standard-error diagnostics.</returns>
    public async Task<(int ExitCode, string StandardError)> CompleteAsync(CancellationToken cancellationToken)
    {
        if (Interlocked.Exchange(ref CompletionStarted, 1) != 0)
        {
            throw new InvalidOperationException("The MCP child process completion has already started.");
        }

        CloseStandardInput(ChildProcess);
        using var exitSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        exitSource.CancelAfter(GracefulExitTimeout);
        try
        {
            await ChildProcess.WaitForExitAsync(exitSource.Token);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"The CreationsForge MCP child process did not exit within {GracefulExitTimeout.TotalSeconds} seconds after standard input closed.");
        }

        var standardError = await StandardErrorTask.WaitAsync(CleanupTimeout, cancellationToken);
        var exitCode = ChildProcess.ExitCode;
        await DisposeClientAsync();
        return (exitCode, standardError);
    }

    /// <summary>Closes or terminates the owned child with finite waits and releases the SDK client and process.</summary>
    /// <returns>A task that completes after bounded cleanup.</returns>
    public async ValueTask DisposeAsync()
    {
        try
        {
            await StopProcessAsync(ChildProcess, StandardErrorTask);
        }
        finally
        {
            try
            {
                await DisposeClientAsync();
            }
            finally
            {
                ChildProcess.Dispose();
            }
        }
    }

    /// <summary>Disposes the SDK client once with a finite wait.</summary>
    /// <returns>A task that completes after client disposal or its cleanup limit.</returns>
    private async Task DisposeClientAsync()
    {
        if (Interlocked.Exchange(ref ClientDisposalStarted, 1) != 0)
        {
            return;
        }

        await Client.DisposeAsync().AsTask().WaitAsync(CleanupTimeout);
    }

    /// <summary>Closes process input, waits briefly, and force-terminates only when graceful cleanup fails.</summary>
    /// <param name="process">The task-owned child process.</param>
    /// <param name="standardErrorTask">The active standard-error drain.</param>
    /// <returns>A task that completes after the child and error reader finish.</returns>
    private static async Task StopProcessAsync(Process process, Task<string> standardErrorTask)
    {
        CloseStandardInput(process);
        if (!process.HasExited)
        {
            using var gracefulSource = new CancellationTokenSource(CleanupTimeout);
            try
            {
                await process.WaitForExitAsync(gracefulSource.Token);
            }
            catch (OperationCanceledException)
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }

                using var forcedSource = new CancellationTokenSource(CleanupTimeout);
                await process.WaitForExitAsync(forcedSource.Token);
            }
        }

        await standardErrorTask.WaitAsync(CleanupTimeout);
    }

    /// <summary>Closes the task-owned child input once while tolerating an already-exited process.</summary>
    /// <param name="process">The task-owned child process.</param>
    private static void CloseStandardInput(Process process)
    {
        try
        {
            process.StandardInput.Close();
        }
        catch (InvalidOperationException) when (process.HasExited)
        {
        }
        catch (ObjectDisposedException)
        {
        }
    }
}
