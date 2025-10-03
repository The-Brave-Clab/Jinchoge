using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

internal static class NativeResolver
{
    static NativeResolver()
    {
        NativeLibrary.SetDllImportResolver(typeof(NativeResolver).Assembly, Resolve);
    }

    public static void EnsureRegistered() { /* triggers static ctor */ }

    private static IntPtr Resolve(string libraryName, Assembly assembly, DllImportSearchPath? paths)
    {
        // Compute current RID
        var rid = GetCurrentRid(); // e.g., "win-x64", "osx-arm64", "linux-x64"
        var baseDir = AppContext.BaseDirectory;
        var dir = Path.Combine(baseDir, "runtimes", rid, "native");
        var fileName = GetPlatformFileName(libraryName);

        // Try the RID-native folder first
        var full = Path.Combine(dir, fileName);
        if (NativeLibrary.TryLoad(full, out var handle))
            return handle;

        // Fallback: app base (if you sometimes drop natives flat)
        var flat = Path.Combine(baseDir, fileName);
        if (NativeLibrary.TryLoad(flat, out handle))
            return handle;

        return IntPtr.Zero; // let default probing continue (will throw if not found)
    }

    private static string GetPlatformFileName(string name)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return name + ".dll";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))     return "lib" + name + ".dylib";
        return "lib" + name + ".so"; // Linux and others
    }

    private static string GetCurrentRid()
    {
        var arch = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64   => "x64",
            Architecture.Arm64 => "arm64",
            _ => throw new PlatformNotSupportedException("Only x64/arm64 expected here.")
        };

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return $"win-{arch}";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))     return $"osx-{arch}";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))   return $"linux-{arch}";
        throw new PlatformNotSupportedException("Unsupported OS.");
    }
}
