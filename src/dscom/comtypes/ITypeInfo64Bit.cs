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

namespace dSPACE.Runtime.InteropServices.Exporter;

/// <summary>
/// The original ITypeInfo interface in the CLR has incorrect definitions for GetRefTypeOfImplType and GetRefTypeInfo.
/// It uses ints for marshalling handles which will result in a crash on 64 bit systems. This is a temporary interface
/// for use until the one in the CLR is fixed. When it is we can go back to using ITypeInfo.
/// </summary>
[Guid("00020401-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[ComImport]
public interface ITypeInfo64Bit
{
    void GetTypeAttr(out IntPtr ppTypeAttr);
    void GetTypeComp(out ITypeComp ppTComp);
    void GetFuncDesc(int index, out IntPtr ppFuncDesc);
    void GetVarDesc(int index, out IntPtr ppVarDesc);
    void GetNames(int memid, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2), Out] string[] rgBstrNames, int cMaxNames, out int pcNames);
    void GetRefTypeOfImplType(int index, out IntPtr href);
    void GetImplTypeFlags(int index, out IMPLTYPEFLAGS pImplTypeFlags);
    void GetIDsOfNames([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1), In] string[] rgszNames, int cNames, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1), Out] int[] pMemId);
    void Invoke([MarshalAs(UnmanagedType.IUnknown)] object pvInstance, int memid, short wFlags, ref DISPPARAMS pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, out int puArgErr);
    void GetDocumentation(int index, out string strName, out string strDocString, out int dwHelpContext, out string strHelpFile);
    void GetDllEntry(int memid, INVOKEKIND invKind, IntPtr pBstrDllName, IntPtr pBstrName, IntPtr pwOrdinal);
    void GetRefTypeInfo(IntPtr hRef, out ITypeInfo64Bit ppTI);
    void AddressOfMember(int memid, INVOKEKIND invKind, out IntPtr ppv);
    void CreateInstance([MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter, [In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown), Out] out object ppvObj);
    void GetMops(int memid, out string pBstrMops);
    void GetContainingTypeLib(out ITypeLib ppTLB, out int pIndex);
    [PreserveSig]
    void ReleaseTypeAttr(IntPtr pTypeAttr);
    [PreserveSig]
    void ReleaseFuncDesc(IntPtr pFuncDesc);
    [PreserveSig]
    void ReleaseVarDesc(IntPtr pVarDesc);
}
