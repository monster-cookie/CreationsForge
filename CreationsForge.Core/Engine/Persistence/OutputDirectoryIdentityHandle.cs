using CreationsForge.Core.Engine.Contracts;
using Microsoft.Win32.SafeHandles;

namespace CreationsForge.Core.Engine.Persistence;

/// <summary>Retains a native directory handle and the stable identity captured from it.</summary>
internal sealed class OutputDirectoryIdentityHandle : IDisposable
{
    /// <summary>Initializes a retained native directory identity.</summary>
    /// <param name="handle">The owned native directory handle.</param>
    /// <param name="identity">The stable identity captured from the handle.</param>
    internal OutputDirectoryIdentityHandle(SafeFileHandle handle, NativeFileIdentity identity)
    {
        Handle = handle;
        Identity = identity;
    }

    /// <summary>Gets the owned native directory handle.</summary>
    internal SafeFileHandle Handle { get; }

    /// <summary>Gets the stable identity captured from the directory handle.</summary>
    internal NativeFileIdentity Identity { get; }

    /// <summary>Closes the retained native directory handle.</summary>
    public void Dispose()
    {
        Handle.Dispose();
    }
}
