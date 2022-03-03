// Copyright 2022 dSPACE GmbH, Mark Lechtermann, Matthias Nissen and Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace dSPACE.Runtime.InteropServices;

internal static class MarshalExtension
{
    [SuppressMessage("Microsoft.Style", "IDE1006", Justification = "")]
    private static readonly Guid COMPLUS_RUNTIME_GUID_FULLFRAMEWORK = new("c9cbf969-05da-d111-9408-0000f8083460");

    [SuppressMessage("Microsoft.Style", "IDE1006", Justification = "")]
    private static readonly Guid COMPLUS_RUNTIME_GUID_CORE = new("69f9cbc9-da05-11d1-9408-0000f8083460");

    internal static Guid GetClassInterfaceGuidForType(Type type)
    {
        var bytes = Encoding.Default.GetBytes($"{type.Namespace}._{type.Name}");
        var bytes2 = Marshal.GenerateGuidForType(type).ToByteArray();

        var rv = new byte[bytes.Length + bytes2.Length];

        Buffer.BlockCopy(bytes, 0, rv, 0, bytes.Length);
        Buffer.BlockCopy(bytes2, 0, rv, bytes.Length, bytes2.Length);

        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(rv);
        return new Guid(hash);
    }

    internal static Guid GetTypeLibGuidForAssembly(Assembly assembly)
    {
        Guid guid;
        GuidAttribute? guidAttribute = null;
        try
        {
            guidAttribute = assembly.GetCustomAttribute<GuidAttribute>();
        }
        catch (FileNotFoundException)
        {
        }

        if (guidAttribute != null)
        {
            guid = new Guid(guidAttribute.Value);
        }
        else
        {
            var stringGuid = GetStringizedTypeLibGuidForAssembly(assembly);
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(stringGuid);
            guid = new Guid(hash);
        }
        return guid;
    }

    private static byte[] StructureToByteArray(object obj)
    {
        var len = Marshal.SizeOf(obj);
        var arr = new byte[len];
        var ptr = Marshal.AllocHGlobal(len);
        Marshal.StructureToPtr(obj, ptr, true);
        Marshal.Copy(ptr, arr, 0, len);
        Marshal.FreeHGlobal(ptr);
        return arr;
    }

    private static byte[] GetStringizedTypeLibGuidForAssembly(Assembly assembly)
    {
        const string typelibKeyName = "Typelib";
        var assemblyName = assembly.GetName().Name;
        var publicKeyBytes = assembly.GetName().GetPublicKey();
        var majorVersion = 0;
        var minorVersion = 0;
        var buildNumber = 0;
        var revisionNumber = 0;

        if (assemblyName == null)
        {
            assemblyName = "";
        }

        ComCompatibleVersionAttribute? versionAttr = null;
        try
        {
            versionAttr = assembly.GetCustomAttribute<ComCompatibleVersionAttribute>();
        }
        catch (FileNotFoundException)
        {

        }

        if (versionAttr != null)
        {
            majorVersion = versionAttr.MajorVersion;
            minorVersion = versionAttr.MinorVersion;
            buildNumber = versionAttr.BuildNumber;
            revisionNumber = versionAttr.RevisionNumber;
        }
        else
        {
            var version = assembly.GetName().Version;

            if (version != null)
            {
                majorVersion = version.Major;
                minorVersion = version.Minor;
                buildNumber = version.Build;
                revisionNumber = version.Revision;
            }
        }

        var versionInfo = new Versioninfo() { MajorVersion = (short)majorVersion, MinorVersion = (short)majorVersion, BuildNumber = (short)buildNumber, RevisionNumber = (short)revisionNumber };

        var guidBytes = COMPLUS_RUNTIME_GUID_CORE.ToByteArray();
        var targetFrameworkAttribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();
        if (targetFrameworkAttribute != null && targetFrameworkAttribute.FrameworkName.StartsWith(".NETFramework", StringComparison.Ordinal))
        {
            guidBytes = COMPLUS_RUNTIME_GUID_FULLFRAMEWORK.ToByteArray();
        }

        var nameBytes = Encoding.Default.GetBytes(assemblyName);
        var typelibBytes = Encoding.ASCII.GetBytes(typelibKeyName);
        var versionBytes = StructureToByteArray(versionInfo);
        var returnLength = guidBytes.Length + nameBytes.Length + typelibBytes.Length + versionBytes.Length;
        byte[]? minorBytes = null;

        if (minorVersion != 0)
        {
            minorBytes = BitConverter.GetBytes((short)minorVersion);
            returnLength += minorBytes.Length;
        }

        if (publicKeyBytes != null)
        {
            returnLength += publicKeyBytes.Length;
        }

        var rv = new byte[returnLength];

        var currentStart = 0;
        Buffer.BlockCopy(guidBytes, 0, rv, currentStart, guidBytes.Length);
        currentStart = guidBytes.Length;

        Buffer.BlockCopy(nameBytes, 0, rv, currentStart, nameBytes.Length);
        currentStart += nameBytes.Length;
        Buffer.BlockCopy(typelibBytes, 0, rv, currentStart, typelibBytes.Length);
        currentStart += typelibBytes.Length;
        Buffer.BlockCopy(versionBytes, 0, rv, currentStart, versionBytes.Length);
        currentStart += versionBytes.Length;
        if (minorBytes != null)
        {
            Buffer.BlockCopy(minorBytes, 0, rv, currentStart, minorBytes.Length);
            currentStart += minorBytes.Length;
        }
        if (publicKeyBytes != null)
        {
            Buffer.BlockCopy(publicKeyBytes, 0, rv, currentStart, publicKeyBytes.Length);
        }

        return rv;
    }
}
