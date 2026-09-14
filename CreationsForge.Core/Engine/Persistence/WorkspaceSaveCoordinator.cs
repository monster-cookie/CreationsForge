using System.Security.Cryptography;
using System.Text;
using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.Persistence;

/// <summary>Coordinates recoverable, lease-guarded publication of complete plugin output artifact sets.</summary>
public sealed partial class WorkspaceSaveCoordinator : IWorkspaceSaveCoordinator
{
    /// <summary>The injected process-wide and cooperating cross-process directory lease provider.</summary>
    private readonly IOutputDirectoryLeaseProvider LeaseProvider;

    /// <summary>The bounded durable journal store.</summary>
    private readonly SaveTransactionStore TransactionStore;

    /// <summary>The narrow per-file mutation seam used by publication and repair.</summary>
    private readonly SaveTransactionFileOperations FileOperations;

    /// <summary>Initializes a guarded workspace save coordinator.</summary>
    /// <param name="leaseProvider">The provider for exclusive canonical output-directory leases.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="leaseProvider"/> is <see langword="null"/>.</exception>
    public WorkspaceSaveCoordinator(IOutputDirectoryLeaseProvider leaseProvider)
        : this(leaseProvider, new SaveTransactionStore(), new SaveTransactionFileOperations())
    { }

    /// <summary>Initializes a coordinator with explicit persistence seams for focused fault testing.</summary>
    /// <param name="leaseProvider">The directory lease provider.</param>
    /// <param name="transactionStore">The durable journal store.</param>
    /// <param name="fileOperations">The per-file publication operations.</param>
    internal WorkspaceSaveCoordinator(
        IOutputDirectoryLeaseProvider leaseProvider,
        SaveTransactionStore transactionStore,
        SaveTransactionFileOperations fileOperations)
    {
        ArgumentNullException.ThrowIfNull(leaseProvider);
        ArgumentNullException.ThrowIfNull(transactionStore);
        ArgumentNullException.ThrowIfNull(fileOperations);
        LeaseProvider = leaseProvider;
        TransactionStore = transactionStore;
        FileOperations = fileOperations;
    }

    /// <summary>Gets and validates the canonical existing output directory for an association.</summary>
    /// <param name="output">The output association.</param>
    /// <returns>The canonical existing output directory.</returns>
    /// <exception cref="InvalidDataException">Thrown when the plugin path is not canonical or its directory is absent.</exception>
    private static string GetOutputDirectory(OutputAssociation output)
    {
        var pluginPath = Path.GetFullPath(output.PluginPath);
        if (!PluginSaveArtifactUtilities.PathComparer.Equals(pluginPath, output.PluginPath))
        {
            throw new InvalidDataException("The output plugin path must be canonical before save coordination.");
        }

        var directory = Path.GetDirectoryName(pluginPath);
        if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
        {
            throw new InvalidDataException($"The output plugin directory does not exist: '{directory}'.");
        }

        return directory;
    }

    /// <summary>Validates that a supplied lease protects the exact association directory.</summary>
    /// <param name="lease">The supplied caller-owned lease.</param>
    /// <param name="output">The exact output association.</param>
    /// <returns>The canonical leased output directory.</returns>
    /// <exception cref="InvalidDataException">Thrown when the lease and association directories differ.</exception>
    private static string ValidateLease(IOutputDirectoryLease lease, OutputAssociation output)
    {
        ArgumentNullException.ThrowIfNull(lease);
        var outputDirectory = GetOutputDirectory(output);
        var leaseDirectory = Path.GetFullPath(lease.OutputDirectoryPath);
        if (!PluginSaveArtifactUtilities.PathComparer.Equals(outputDirectory, leaseDirectory))
        {
            throw new InvalidDataException("The supplied output-directory lease does not protect the requested output association.");
        }

        return outputDirectory;
    }

    /// <summary>Creates the canonical request fingerprint for original save idempotency.</summary>
    /// <param name="context">The borrowed exact workspace save context.</param>
    /// <param name="request">The guarded save request.</param>
    /// <returns>An uppercase SHA-256 digest.</returns>
    private static string CreateSaveFingerprint(WorkspaceSaveContext context, SaveRequest request)
    {
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        AppendToken(hash, "CreationsForge.WorkspaceSave/v1");
        AppendToken(hash, context.WorkspaceId.ToString("N"));
        AppendToken(hash, request.OperationId.ToString("N"));
        AppendToken(hash, request.ExpectedRevision.ToString());
        AppendToken(hash, ((int)context.Game).ToString(System.Globalization.CultureInfo.InvariantCulture));
        AppendToken(hash, ((int)context.Release).ToString(System.Globalization.CultureInfo.InvariantCulture));
        AppendOutput(hash, context.OutputAssociation);
        AppendArtifacts(hash, context.Sources.Baseline.Artifacts);
        AppendArtifacts(hash, request.ExpectedOutputBaseline.Artifacts);
        return Convert.ToHexString(hash.GetHashAndReset());
    }

    /// <summary>Creates the canonical request fingerprint for repair idempotency.</summary>
    /// <param name="request">The explicit repair request.</param>
    /// <returns>An uppercase SHA-256 digest.</returns>
    private static string CreateRepairFingerprint(RepairSaveRequest request)
    {
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        AppendToken(hash, "CreationsForge.WorkspaceSaveRepair/v1");
        AppendToken(hash, request.WorkspaceId.ToString("N"));
        AppendToken(hash, request.SaveOperationId.ToString("N"));
        AppendToken(hash, request.RepairOperationId.ToString("N"));
        AppendToken(hash, request.ExpectedSaveRevision.ToString());
        AppendToken(hash, ((int)request.Direction).ToString(System.Globalization.CultureInfo.InvariantCulture));
        AppendToken(hash, request.EvidenceToken.Value);
        AppendOutput(hash, request.Output);
        return Convert.ToHexString(hash.GetHashAndReset());
    }

    /// <summary>Appends one complete output association to a canonical digest.</summary>
    /// <param name="hash">The digest receiving the association.</param>
    /// <param name="output">The association to append.</param>
    private static void AppendOutput(IncrementalHash hash, OutputAssociation output)
    {
        AppendToken(hash, Path.GetFullPath(output.PluginPath));
        AppendToken(hash, output.ModKey.FileName);
        AppendToken(hash, ((int)output.LocalizedOutputMode).ToString(System.Globalization.CultureInfo.InvariantCulture));
        AppendToken(hash, ((int)output.MasterStyle).ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    /// <summary>Appends complete ordered artifact metadata to a canonical digest.</summary>
    /// <param name="hash">The digest receiving the artifacts.</param>
    /// <param name="artifacts">The artifacts to append.</param>
    private static void AppendArtifacts(IncrementalHash hash, IReadOnlyList<PluginArtifactAssociation> artifacts)
    {
        foreach (var artifact in artifacts)
        {
            AppendToken(hash, artifact.Path);
            AppendToken(hash, ((int)artifact.Role).ToString(System.Globalization.CultureInfo.InvariantCulture));
            AppendToken(hash, artifact.Language ?? string.Empty);
            AppendToken(hash, artifact.Fingerprint.Exists ? "1" : "0");
            AppendToken(hash, artifact.Fingerprint.Length.ToString(System.Globalization.CultureInfo.InvariantCulture));
            AppendToken(hash, artifact.Fingerprint.Sha256 ?? string.Empty);
            AppendToken(hash, artifact.FileIdentity?.Provider ?? string.Empty);
            AppendToken(hash, artifact.FileIdentity?.VolumeId ?? string.Empty);
            AppendToken(hash, artifact.FileIdentity?.FileId ?? string.Empty);
            AppendToken(hash, artifact.FileIdentity?.LinkCount?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty);
        }
    }

    /// <summary>Appends one length-delimited UTF-8 token to a canonical digest.</summary>
    /// <param name="hash">The digest receiving the token.</param>
    /// <param name="value">The token value.</param>
    private static void AppendToken(IncrementalHash hash, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        hash.AppendData(BitConverter.GetBytes(bytes.Length));
        hash.AppendData(bytes);
    }

    /// <summary>Creates a stable engine error for an unexpected coordinator exception.</summary>
    /// <param name="operation">The operation description.</param>
    /// <param name="exception">The unexpected exception.</param>
    /// <returns>The typed error without plugin payload data.</returns>
    private static EngineError Unexpected(string operation, Exception exception)
    {
        return new EngineError(EngineErrorCode.UnexpectedFailure, $"{operation} failed: {exception.Message}");
    }
}
