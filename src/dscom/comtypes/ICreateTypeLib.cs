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

namespace dSPACE.Runtime.InteropServices.ComTypes;

/// <summary>
/// Provides the methods for creating and managing the component or file that contains type information. Type libraries are created
/// from type descriptions using the MIDL compiler. These type libraries are accessed through the ITypeLib interface.
/// </summary>
// https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nn-oaidl-icreatetypelib
[ComImport, Guid("00020406-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICreateTypeLib
{
    /// <summary>Creates a new type description instance within the type library.</summary>
    /// <param name="szName">The name of the new type.</param>
    /// <param name="tkind">TYPEKIND of the type description to be created.</param>
    /// <param name="ppCTInfo">The type description.</param>
    /// <returns>
    /// <para>This method can return one of these values.</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Return code</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <term>S_OK</term>
    /// <term>Success.</term>
    /// </item>
    /// <item>
    /// <term>E_INVALIDARG</term>
    /// <term>One or more of the arguments is not valid.</term>
    /// </item>
    /// <item>
    /// <term>E_OUTOFMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_INVALIDSTATE</term>
    /// <term>The state of the type library is not valid for this operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_NAMECONFLICT</term>
    /// <term>The provided name is not unique.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_WRONGTYPEKIND</term>
    /// <term>Type mismatch.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// Use ICreateTypeLib to create a new type description instance within the library. An error is returned if the specified name
    /// already appears in the library. Valid tkind values are described in TYPEKIND. To get the type information of the type
    /// description that is being created, call on the returned <c>ICreateTypeLib</c>. This type information can be used by other
    /// type descriptions that reference it by using ICreateTypeInfo::AddRefTypeInfo.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypelib-createtypeinfo HRESULT
    // CreateTypeInfo(LPOLESTR szName, TYPEKIND tkind, ICreateTypeInfo **ppCTInfo);
    [PreserveSig]
    HRESULT CreateTypeInfo([In, MarshalAs(UnmanagedType.LPWStr)] string szName, TYPEKIND tkind, out ICreateTypeInfo ppCTInfo);

    /// <summary>Sets the name of the type library.</summary>
    /// <param name="szName">The name to be assigned to the library.</param>
    /// <returns>
    /// <para>This method can return one of these values.</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Return code</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <term>S_OK</term>
    /// <term>Success.</term>
    /// </item>
    /// <item>
    /// <term>E_INVALIDARG</term>
    /// <term>One or more of the arguments is not valid.</term>
    /// </item>
    /// <item>
    /// <term>E_OUTOFMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_INVALIDSTATE</term>
    /// <term>The state of the type library is not valid for this operation.</term>
    /// </item>
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypelib-setname HRESULT SetName(LPOLESTR szName);
    [PreserveSig]
    HRESULT SetName([In, MarshalAs(UnmanagedType.LPWStr)] string szName);

    /// <summary>Sets the major and minor version numbers of the type library.</summary>
    /// <param name="wMajorVerNum">The major version number for the library.</param>
    /// <param name="wMinorVerNum">The minor version number for the library.</param>
    /// <returns>
    /// <para>This method can return one of these values.</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Return code</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <term>S_OK</term>
    /// <term>Success.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_INVALIDSTATE</term>
    /// <term>The state of the type library is not valid for this operation.</term>
    /// </item>
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypelib-setversion HRESULT SetVersion(WORD
    // wMajorVerNum, WORD wMinorVerNum);
    [PreserveSig]
    HRESULT SetVersion(ushort wMajorVerNum, ushort wMinorVerNum);

    /// <summary>
    /// Sets the universal unique identifier (UUID) associated with the type library (Also known as the globally unique identifier (GUID)).
    /// </summary>
    /// <param name="guid">The globally unique identifier to be assigned to the library.</param>
    /// <returns>
    /// <para>This method can return one of these values.</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Return code</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <term>S_OK</term>
    /// <term>Success.</term>
    /// </item>
    /// <item>
    /// <term>E_INVALIDARG</term>
    /// <term>One or more of the arguments is not valid.</term>
    /// </item>
    /// <item>
    /// <term>E_OUTOFMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_INVALIDSTATE</term>
    /// <term>The state of the type library is not valid for this operation.</term>
    /// </item>
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypelib-setguid HRESULT SetGuid(REFGUID guid);
    [PreserveSig]
    HRESULT SetGuid(in Guid guid);

    /// <summary>Sets the documentation string associated with the library.</summary>
    /// <param name="szDoc">A brief description of the type library.</param>
    /// <returns>
    /// <para>This method can return one of these values.</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Return code</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <term>S_OK</term>
    /// <term>Success.</term>
    /// </item>
    /// <item>
    /// <term>E_INVALIDARG</term>
    /// <term>One or more of the arguments is not valid.</term>
    /// </item>
    /// <item>
    /// <term>E_OUTOFMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// The documentation string is a brief description of the library intended for use by type information browsing tools.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypelib-setdocstring HRESULT SetDocString(LPOLESTR szDoc);
    [PreserveSig]
    HRESULT SetDocString([MarshalAs(UnmanagedType.LPWStr)] string szDoc);

    /// <summary>Sets the name of the Help file.</summary>
    /// <param name="szHelpFileName">The name of the Help file for the library.</param>
    /// <returns>
    /// <para>This method can return one of these values.</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Return code</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <term>S_OK</term>
    /// <term>Success.</term>
    /// </item>
    /// <item>
    /// <term>E_INVALIDARG</term>
    /// <term>One or more of the arguments is not valid.</term>
    /// </item>
    /// <item>
    /// <term>E_OUTOFMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_INVALIDSTATE</term>
    /// <term>The state of the type library is not valid for this operation.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <para>Each type library can reference a single Help file.</para>
    /// <para>
    /// The GetDocumentation method of the created ITypeLib returns a fully qualified path for the Help file, which is formed by
    /// appending the name passed into szHelpFileName to the registered Help directory for the type library. The Help directory is
    /// registered under:
    /// </para>
    /// <para>\TYPELIB&amp;lt;guid of library&gt;&amp;lt;Major.Minor version &gt;\HELPDIR</para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypelib-sethelpfilename HRESULT
    // SetHelpFileName(LPOLESTR szHelpFileName);
    [PreserveSig]
    HRESULT SetHelpFileName([MarshalAs(UnmanagedType.LPWStr)] string szHelpFileName);

    /// <summary>Sets the Help context ID for retrieving general Help information for the type library.</summary>
    /// <param name="dwHelpContext">The Help context ID.</param>
    /// <returns>
    /// <para>This method can return one of these values.</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Return code</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <term>S_OK</term>
    /// <term>Success.</term>
    /// </item>
    /// <item>
    /// <term>E_INVALIDARG</term>
    /// <term>One or more of the arguments is not valid.</term>
    /// </item>
    /// <item>
    /// <term>E_OUTOFMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_INVALIDSTATE</term>
    /// <term>The state of the type library is not valid for this operation.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// Calling <c>SetHelpContext</c> with a Help context of zero is equivalent to not calling it at all, because zero indicates a
    /// null Help context.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypelib-sethelpcontext HRESULT SetHelpContext(DWORD dwHelpContext);
    [PreserveSig]
    HRESULT SetHelpContext(uint dwHelpContext);

    /// <summary>Sets the binary Microsoft national language ID associated with the library.</summary>
    /// <param name="lcid">The locale ID for the type library.</param>
    /// <returns>
    /// <para>This method can return one of these values.</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Return code</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <term>S_OK</term>
    /// <term>Success.</term>
    /// </item>
    /// <item>
    /// <term>E_INVALIDARG</term>
    /// <term>One or more of the arguments is not valid.</term>
    /// </item>
    /// <item>
    /// <term>E_OUTOFMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_INVALIDSTATE</term>
    /// <term>The state of the type library is not valid for this operation.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// For more information on national language IDs, see Supporting Multiple National Languages and the National Language Support
    /// (NLS) API.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypelib-setlcid HRESULT SetLcid(LCID lcid);
    [PreserveSig]
    HRESULT SetLcid(int lcid);

    /// <summary>Sets library flags.</summary>
    /// <param name="uLibFlags">The flags to set.</param>
    /// <returns>
    /// <para>This method can return one of these values.</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Return code</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <term>S_OK</term>
    /// <term>Success.</term>
    /// </item>
    /// <item>
    /// <term>E_INVALIDARG</term>
    /// <term>One or more of the arguments is not valid.</term>
    /// </item>
    /// <item>
    /// <term>E_OUTOFMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_INVALIDSTATE</term>
    /// <term>The state of the type library is not valid for this operation.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>Valid uLibFlags values are listed in LIBFLAGS.</remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypelib-setlibflags HRESULT SetLibFlags(UINT uLibFlags);
    [PreserveSig]
    HRESULT SetLibFlags(uint uLibFlags);

    /// <summary>Saves the ICreateTypeLib instance following the layout of type information.</summary>
    /// <returns>
    /// <para>This method can return one of these values.</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Return code</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <term>S_OK</term>
    /// <term>Success.</term>
    /// </item>
    /// <item>
    /// <term>E_INVALIDARG</term>
    /// <term>One or more of the arguments is not valid.</term>
    /// </item>
    /// <item>
    /// <term>E_OUTOFMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_IOERROR</term>
    /// <term>The function cannot write to the file.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_INVALIDSTATE</term>
    /// <term>The state of the type library is not valid for this operation.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>You should not call any other ICreateTypeLib methods after calling <c>SaveAllChanges</c>.</remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypelib-saveallchanges HRESULT SaveAllChanges();
    [PreserveSig]
    HRESULT SaveAllChanges();
}
