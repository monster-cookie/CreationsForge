using CreationsForge.Core.Engine.Contracts;
using Microsoft.Win32.SafeHandles;

namespace CreationsForge.Core.Engine.Persistence;

/// <summary>Retains an operating-system directory handle and the stable identity captured from it.</summary>
internal sealed class OutputDirectoryIdentityHandle : IDisposable
{
    /// <summary>Initializes a retained operating-system directory identity.</summary>
    /// <param name="handle">The owned operating-system directory handle.</param>
    /// <param name="identity">The stable identity captured from the handle.</param>
    internal OutputDirectoryIdentityHandle(SafeFileHandle handle, ArtifactFileIdentity identity)
    {
        Handle = handle;
        Identity = identity;
    }

    /// <summary>Gets the owned operating-system directory handle.</summary>
    internal SafeFileHandle Handle { get; }

    /// <summary>Gets the stable identity captured from the directory handle.</summary>
    internal ArtifactFileIdentity Identity { get; }

    /// <summary>Closes the retained operating-system directory handle.</summary>
    public void Dispose()
    {
        Handle.Dispose();
    }
}
