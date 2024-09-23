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

using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices;

/// <summary>
/// Embeds a type library into an assembly.
/// </summary>
public static class TypeLibEmbedder
{
    // HANDLE BeginUpdateResourceW(
    //   [in] LPCWSTR pFileName,
    //   [in] BOOL    bDeleteExistingResources
    // );
    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    private static extern IntPtr BeginUpdateResourceW(
        [In, MarshalAs(UnmanagedType.LPWStr)] string pFileName,
        [In] bool bDeleteExistingResources);

    // BOOL UpdateResourceW(
    //   [in]           HANDLE  hUpdate,
    //   [in]           LPCWSTR lpType,
    //   [in]           LPCWSTR lpName,
    //   [in]           WORD    wLanguage,
    //   [in, optional] LPVOID  lpData,
    //   [in]           DWORD   cb
    // );
    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    private static extern bool UpdateResourceW(
        [In] IntPtr hUpdate,
        IntPtr lpType,
        IntPtr lpName,
        [In] short wLanguage,
        [In, Optional] byte[] lpData,
        [In] long cb);

    // BOOL EndUpdateResourceW(
    //   [in] HANDLE hUpdate,
    //   [in] BOOL   fDiscard
    // );
    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    private static extern bool EndUpdateResourceW(
        [In] IntPtr hUpdate,
        [In] bool fDiscard);

    public static bool EmbedTypeLib(TypeLibEmbedderSettings settings)
    {
        int win32ErrorCode;

        var assemblyFileHandle = BeginUpdateResourceW(settings.TargetAssembly, false);
        win32ErrorCode = Marshal.GetLastWin32Error();
        if (assemblyFileHandle == IntPtr.Zero)
        {
            throw new ApplicationException($"Unable to lock the assembly file {settings.TargetAssembly} for resource updating; error code {win32ErrorCode}.");
        }

        var strPtr = IntPtr.Zero;
        try
        {
            strPtr = Marshal.StringToHGlobalUni("TYPELIB");
            var bytes = File.ReadAllBytes(settings.SourceTlbPath);
            if (!UpdateResourceW(assemblyFileHandle, strPtr, new IntPtr(1), 0, bytes, bytes.Length))
            {
                win32ErrorCode = Marshal.GetLastWin32Error();
                throw new ApplicationException($"Error: Unable to update assembly file '{settings.TargetAssembly}' using TLB file '{settings.SourceTlbPath}'; error code {win32ErrorCode}.");
            }
        }
        finally
        {
            if (strPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(strPtr);
            }
        }

        if (!EndUpdateResourceW(assemblyFileHandle, false))
        {
            win32ErrorCode = Marshal.GetLastWin32Error();
            throw new ApplicationException($"Error: Unable to write changes to assembly file '{settings.TargetAssembly}' using TLB file '{settings.SourceTlbPath}'; error code {win32ErrorCode}.");
        };

        return true;
    }
}
