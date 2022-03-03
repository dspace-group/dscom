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
/// <para>
/// Provides the tools for creating and administering the type information defined through the type description. Derives from
/// ICreateTypeInfo, and adds methods for deleting items that have been added through ICreateTypeInfo.
/// </para>
/// <para>
/// The ICreateTypeInfo::LayOut method provides a way for the creator of the type information to check for any errors. A call to
/// QueryInterface can be made to the ICreateTypeInfo instance at any time for its ITypeInfo interface. Calling any of the methods
/// in the ITypeInfointerface that require layout information lays out the type information automatically.
/// </para>
/// </summary>
// https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nn-oaidl-icreatetypeinfo2
[ComImport, Guid("0002040E-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICreateTypeInfo2 : ICreateTypeInfo
{
    /// <summary>Sets the globally unique identifier (GUID) associated with the type description.</summary>
    /// <param name="guid">The globally unique ID to be associated with the type description.</param>
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
    /// <term>E_OUTOFMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// For an interface, this is an interface ID (IID); for a coclass, it is a class ID (CLSID). For information on GUIDs, see Type
    /// Libraries and the Object Description Language.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-setguid HRESULT SetGuid(REFGUID guid);
    [PreserveSig]
    new HRESULT SetGuid(in Guid guid);

    /// <summary>Sets type flags of the type description being created.</summary>
    /// <param name="uTypeFlags">The settings for the type flags. For details, see TYPEFLAGS.</param>
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
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_WRONGTYPEKIND</term>
    /// <term>Type mismatch.</term>
    /// </item>
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-settypeflags HRESULT SetTypeFlags(UINT uTypeFlags);
    [PreserveSig]
    new HRESULT SetTypeFlags(uint uTypeFlags);

    /// <summary>Sets the documentation string displayed by type browsers.</summary>
    /// <param name="pStrDoc">A brief description of the type description.</param>
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
    /// <term>E_OUTOFMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
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
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-setdocstring HRESULT SetDocString(LPOLESTR pStrDoc);
    [PreserveSig]
    new HRESULT SetDocString([MarshalAs(UnmanagedType.LPWStr)] string pStrDoc);

    /// <summary>Sets the Help context ID of the type information.</summary>
    /// <param name="dwHelpContext">A handle to the Help context.</param>
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
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-sethelpcontext HRESULT SetHelpContext(//
    // DWORD dwHelpContext);
    [PreserveSig]
    new HRESULT SetHelpContext(uint dwHelpContext);

    /// <summary>Sets the major and minor version number of the type information.</summary>
    /// <param name="wMajorVerNum">The major version number.</param>
    /// <param name="wMinorVerNum">The minor version number.</param>
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
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_INVALIDSTATE</term>
    /// <term>The state of the type library is not valid for this operation.</term>
    /// </item>
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-setversion HRESULT SetVersion(WORD
    // wMajorVerNum, WORD wMinorVerNum);
    [PreserveSig]
    new HRESULT SetVersion(ushort wMajorVerNum, ushort wMinorVerNum);

    /// <summary>Adds a type description to those referenced by the type description being created.</summary>
    /// <param name="pTInfo">The type description to be referenced.</param>
    /// <param name="phRefType">The handle that this type description associates with the referenced type information.</param>
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
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_WRONGTYPEKIND</term>
    /// <term>Type mismatch.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// The second parameter returns a pointer to the handle of the added type information. If <c>AddRefTypeInfo</c> has been called
    /// previously for the same type information, the index that was returned by the previous call is returned in phRefType. If the
    /// referenced type description is in the type library being created, its type information can be obtained by calling
    /// IUnknown::QueryInterface(IID_ITypeInfo, ...) on the ICreateTypeInfo interface of that type description.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-addreftypeinfo HRESULT AddRefTypeInfo(//
    // ITypeInfo *pTInfo, HREFTYPE *phRefType);
    [PreserveSig]
    new HRESULT AddRefTypeInfo([In] ITypeInfo? pTInfo, out uint phRefType);

    /// <summary>Adds a function description to the type description.</summary>
    /// <param name="index">The index of the new FUNCDESC in the type information.</param>
    /// <param name="pFuncDesc">
    /// A FUNCDESC structure that describes the function. The <c>bstrIDLInfo</c> field in the FUNCDESC should be null.
    /// </param>
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
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_WRONGTYPEKIND</term>
    /// <term>Type mismatch.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <para>
    /// The index specifies the order of the functions within the type information. The first function has an index of zero. If an
    /// index is specified that exceeds one less than the number of functions in the type information, an error is returned. Calling
    /// this function does not pass ownership of the FUNCDESC structure to ICreateTypeInfo. Therefore, the caller must still
    /// de-allocate the FUNCDESC structure.
    /// </para>
    /// <para>
    /// The passed-in virtual function table (VTBL) field (oVft) of the FUNCDESC is ignored if the TYPEKIND is TKIND_MODULE or if
    /// oVft is -1 or 0. This attribute is set when ICreateTypeInfo::LayOut is called. The oVft value is used if the TYPEKIND is
    /// TKIND_DISPATCH and a dual interface or if the TYPEKIND is TKIND_INTERFACE. If the oVft is used, it must be a multiple of the
    /// sizeof(VOID *) on the machine, otherwise the function fails and E_INVALIDARG is returned as the HRESULT.
    /// </para>
    /// <para>
    /// The function <c>AddFuncDesc</c> uses the passed-in member identifier (memid) fields within each FUNCDESC for classes with
    /// TYPEKIND = TKIND_DISPATCH or TKIND_INTERFACE. If the member IDs are set to MEMBERID_NIL, <c>AddFuncDesc</c> assigns member
    /// IDs to the functions. Otherwise, the member ID fields within each FUNCDESC are ignored.
    /// </para>
    /// <para>
    /// Any HREFTYPE fields in the FUNCDESC structure must have been produced by the same instance of ITypeInfo for which
    /// <c>AddFuncDesc</c> is called.
    /// </para>
    /// <para>The get and put accessor functions for the same property must have the same dispatch identifier (DISPID).</para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-addfuncdesc HRESULT AddFuncDesc(UINT index,
    // FUNCDESC *pFuncDesc);
    [PreserveSig]
    new HRESULT AddFuncDesc(uint index, in FUNCDESC pFuncDesc);

    /// <summary>Specifies an inherited interface, or an interface implemented by a component object class (coclass).</summary>
    /// <param name="index">
    /// The index of the implementation class to be added. Specifies the order of the type relative to the other type.
    /// </param>
    /// <param name="hRefType">A handle to the referenced type description obtained from the AddRefType description.</param>
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
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_WRONGTYPEKIND</term>
    /// <term>Type mismatch.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// To specify an inherited interface, use index = 0. For a dispinterface with Syntax 2, call
    /// <c>ICreateTypeInfo::AddImplType</c> twice, once with index = 0 for the inherited IDispatch and once with index = 1 for the
    /// interface that is being wrapped. For a dual interface, call <c>ICreateTypeInfo::AddImplType</c> with index = -1 for the
    /// TKIND_INTERFACE type information component of the dual interface.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-addimpltype HRESULT AddImplType(UINT index,
    // HREFTYPE hRefType);
    [PreserveSig]
    new HRESULT AddImplType(uint index, uint hRefType);

    /// <summary>Sets the attributes for an implemented or inherited interface of a type.</summary>
    /// <param name="index">The index of the interface for which to set type flags.</param>
    /// <param name="implTypeFlags">IMPLTYPE flags to be set.</param>
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
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-setimpltypeflags HRESULT
    // SetImplTypeFlags(// UINT index, INT implTypeFlags);
    [PreserveSig]
    new HRESULT SetImplTypeFlags(uint index, IMPLTYPEFLAGS implTypeFlags);

    /// <summary>Specifies the data alignment for an item of TYPEKIND=TKIND_RECORD.</summary>
    /// <param name="cbAlignment">
    /// Alignment method for the type. A value of 0 indicates alignment on the 64K boundary; 1 indicates no special alignment. For
    /// other values, n indicates alignment on byte n.
    /// </param>
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
    /// <term>E_OUTOFMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
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
    /// The alignment is the minimum of the natural alignment (for example, byte data on byte boundaries, word data on word
    /// boundaries, and so on), and the alignment denoted by cbAlignment.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-setalignment HRESULT SetAlignment(WORD cbAlignment);
    [PreserveSig]
    new HRESULT SetAlignment(ushort cbAlignment);

    /// <summary>Reserved for future use.</summary>
    /// <param name="pStrSchema">The schema.</param>
    [PreserveSig]
    new HRESULT SetSchema([MarshalAs(UnmanagedType.LPWStr)] string pStrSchema);

    /// <summary>Adds a variable or data member description to the type description.</summary>
    /// <param name="index">The index of the variable or data member to be added to the type description.</param>
    /// <param name="pVarDesc">A pointer to the variable or data member description to be added.</param>
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
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_WRONGTYPEKIND</term>
    /// <term>Type mismatch.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <para>
    /// The index specifies the order of the variables. The first variable has an index of zero. <c>ICreateTypeInfo::AddVarDesc</c>
    /// returns an error if the specified index is greater than the number of variables currently in the type information. Calling
    /// this function does not pass ownership of the VARDESC structure to ICreateTypeInfo. The instance field (oInst) of the VARDESC
    /// structure is ignored. This attribute is set only when ICreateTypeInfo::LayOut is called. Also, the member ID fields within
    /// the VARDESCs are ignored unless the TYPEKIND of the class is TKIND_DISPATCH.
    /// </para>
    /// <para>
    /// Any HREFTYPE fields in the VARDESC structure must have been produced by the same instance of ITypeInfo for which
    /// <c>AddVarDesc</c> is called.
    /// </para>
    /// <para><c>AddVarDesc</c> ignores the contents of the idldesc field of the ELEMDESC.</para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-addvardesc HRESULT AddVarDesc(UINT index,
    // VARDESC *pVarDesc);
    [PreserveSig]
    new HRESULT AddVarDesc(uint index, in VARDESC pVarDesc);

    /// <summary>Sets the name of a function and the names of its parameters to the specified names.</summary>
    /// <param name="index">The index of the function whose function name and parameter names are to be set.</param>
    /// <param name="rgszNames">
    /// An array of pointers to names. The first element is the function name. Subsequent elements are names of parameters.
    /// </param>
    /// <param name="cNames">The number of elements in the rgszNames array.</param>
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
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_ELEMENTNOTFOUND</term>
    /// <term>The element cannot be found.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// This method must be used once for each property. The last parameter for put and putref accessor functions is unnamed.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-setfuncandparamnames HRESULT
    // SetFuncAndParamNames(UINT index, LPOLESTR *rgszNames, UINT cNames);
    [PreserveSig]
    new HRESULT SetFuncAndParamNames(uint index, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2, ArraySubType = UnmanagedType.LPWStr)] string[] rgszNames, uint cNames);

    /// <summary>Sets the name of a variable.</summary>
    /// <param name="index">The index of the variable.</param>
    /// <param name="szName">The name.</param>
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
    /// <term>E_OUTOFMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_ELEMENTNOTFOUND</term>
    /// <term>The element cannot be found.</term>
    /// </item>
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-setvarname HRESULT SetVarName(UINT index,
    // LPOLESTR szName);
    [PreserveSig]
    new HRESULT SetVarName(uint index, [MarshalAs(UnmanagedType.LPWStr)] string szName);

    /// <summary>Sets the type description for which this type description is an alias, if TYPEKIND=TKIND_ALIAS.</summary>
    /// <param name="pTDescAlias">The type description.</param>
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
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_WRONGTYPEKIND</term>
    /// <term>Type mismatch.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>To set the type for an alias, call <c>SetTypeDescAlias</c> for a type description whose TYPEKIND is TKIND_ALIAS.</remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-settypedescalias HRESULT
    // SetTypeDescAlias(// TYPEDESC *pTDescAlias);
    [PreserveSig]
    new HRESULT SetTypeDescAlias(in TYPEDESC pTDescAlias);

    /// <summary>Associates a DLL entry point with the function that has the specified index.</summary>
    /// <param name="index">The index of the function.</param>
    /// <param name="szDllName">The name of the DLL that contains the entry point.</param>
    /// <param name="szProcName">The name of the entry point or an ordinal (if the high word is zero).</param>
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
    /// <term>TYPE_E_ELEMENTNOTFOUND</term>
    /// <term>The element cannot be found.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_WRONGTYPEKIND</term>
    /// <term>Type mismatch.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// If the high word of szProcName is zero, then the low word must contain the ordinal of the entry point; otherwise, szProcName
    /// points to the zero-terminated name of the entry point.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-definefuncasdllentry HRESULT
    // DefineFuncAsDllEntry(UINT index, LPOLESTR szDllName, LPOLESTR szProcName);
    [PreserveSig]
    new HRESULT DefineFuncAsDllEntry(uint index, [MarshalAs(UnmanagedType.LPWStr)] string szDllName, [MarshalAs(UnmanagedType.LPWStr)] string szProcName);

    /// <summary>Sets the documentation string for the function with the specified index.</summary>
    /// <param name="index">The index of the function.</param>
    /// <param name="szDocString">The documentation string.</param>
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
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_ELEMENTNOTFOUND</term>
    /// <term>The element cannot be found.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// The documentation string is a brief description of the function intended for use by tools such as type browsers.
    /// <c>SetFuncDocString</c> only needs to be used once for each property, because all property accessor functions are identified
    /// by one name.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-setfuncdocstring HRESULT
    // SetFuncDocString(// UINT index, LPOLESTR szDocString);
    [PreserveSig]
    new HRESULT SetFuncDocString(uint index, [MarshalAs(UnmanagedType.LPWStr)] string szDocString);

    /// <summary>Sets the documentation string for the variable with the specified index.</summary>
    /// <param name="index">The index of the variable.</param>
    /// <param name="szDocString">The documentation string.</param>
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
    /// <term>E_OUTOFMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_ELEMENTNOTFOUND</term>
    /// <term>The element cannot be found.</term>
    /// </item>
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-setvardocstring HRESULT SetVarDocString(//
    // UINT index, LPOLESTR szDocString);
    [PreserveSig]
    new HRESULT SetVarDocString(uint index, [MarshalAs(UnmanagedType.LPWStr)] string szDocString);

    /// <summary>Sets the Help context ID for the function with the specified index.</summary>
    /// <param name="index">The index of the function.</param>
    /// <param name="dwHelpContext">The Help context ID for the Help topic.</param>
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
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_ELEMENTNOTFOUND</term>
    /// <term>The element cannot be found.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <c>SetFuncHelpContext</c> only needs to be set once for each property, because all property accessor functions are
    /// identified by one name.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-setfunchelpcontext HRESULT
    // SetFuncHelpContext(UINT index, DWORD dwHelpContext);
    [PreserveSig]
    new HRESULT SetFuncHelpContext(uint index, uint dwHelpContext);

    /// <summary>Sets the Help context ID for the variable with the specified index.</summary>
    /// <param name="index">The index of the variable.</param>
    /// <param name="dwHelpContext">The handle to the Help context ID for the Help topic on the variable.</param>
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
    /// <term>E_OUTOFMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_ELEMENTNOTFOUND</term>
    /// <term>The element cannot be found.</term>
    /// </item>
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-setvarhelpcontext HRESULT
    // SetVarHelpContext(UINT index, DWORD dwHelpContext);
    [PreserveSig]
    new HRESULT SetVarHelpContext(uint index, uint dwHelpContext);

    /// <summary>Sets the marshaling opcode string associated with the type description or the function.</summary>
    /// <param name="index">
    /// The index of the member for which to set the opcode string. If index is –1, sets the opcode string for the type description.
    /// </param>
    /// <param name="bstrMops">The marshaling opcode string.</param>
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
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-setmops HRESULT SetMops(UINT index, BSTR bstrMops);
    [PreserveSig]
    new HRESULT SetMops(uint index, [MarshalAs(UnmanagedType.BStr)] string bstrMops);

    /// <summary>Reserved for future use.</summary>
    /// <param name="pIdlDesc">The IDLDESC.</param>
    [PreserveSig]
    new HRESULT SetTypeIdldesc(in IDLDESC pIdlDesc);

    /// <summary>
    /// Assigns VTBL offsets for virtual functions and instance offsets for per-instance data members, and creates the two type
    /// descriptions for dual interfaces.
    /// </summary>
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
    /// <term>E_OUTOFMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>E_ACCESSDENIED</term>
    /// <term>Cannot write to the destination.</term>
    /// </item>
    /// <item>
    /// <term>STG_E_INSUFFICIENTMEMORY</term>
    /// <term>Insufficient memory to complete the operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_UNDEFINEDTYPE</term>
    /// <term>Bound to unrecognized type.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_INVALIDSTATE</term>
    /// <term>The state of the type library is not valid for this operation.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_WRONGTYPEKIND</term>
    /// <term>Type mismatch.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_ELEMENTNOTFOUND</term>
    /// <term>The element cannot be found.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_AMBIGUOUSNAME</term>
    /// <term>More than one item exists with this name.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_SIZETOOBIG</term>
    /// <term>The type information is too long.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_TYPEMISMATCH</term>
    /// <term>Type mismatch.</term>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <para>
    /// <c>LayOut</c> also assigns member ID numbers to the functions and variables, unless the TYPEKIND of the class is
    /// TKIND_DISPATCH. Call <c>LayOut</c> after all members of the type information are defined, and before the type library is saved.
    /// </para>
    /// <para>
    /// Use ICreateTypeLib::SaveAllChanges to save the type information after calling <c>LayOut</c>. Other members of the
    /// ICreateTypeInfo interface should not be called after calling <c>LayOut</c>.
    /// </para>
    /// <para>
    /// <c>Note</c> Different implementations of ICreateTypeLib::SaveAllChanges or other interfaces that create type information are
    /// free to assign any member ID numbers, provided that all members (including inherited members), have unique IDs. For
    /// examples, see ICreateTypeInfo2.
    /// </para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo-layout HRESULT LayOut();
    [PreserveSig]
    new HRESULT LayOut();

    /// <summary>Deletes a function description specified by the index number.</summary>
    /// <param name="index">
    /// The index of the function whose description is to be deleted. The index should be in the range of 0 to 1 less than the
    /// number of functions in this type.
    /// </param>
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
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo2-deletefuncdesc HRESULT DeleteFuncDesc(//
    // UINT index);
    [PreserveSig]
    HRESULT DeleteFuncDesc(uint index);

    /// <summary>Deletes the specified function description (FUNCDESC).</summary>
    /// <param name="memid">The member identifier of the FUNCDESC to delete.</param>
    /// <param name="invKind">The type of the invocation.</param>
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
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo2-deletefuncdescbymemid HRESULT
    // DeleteFuncDescByMemId(MEMBERID memid, INVOKEKIND invKind);
    [PreserveSig]
    HRESULT DeleteFuncDescByMemId(int memid, INVOKEKIND invKind);

    /// <summary>Deletes the specified VARDESC structure.</summary>
    /// <param name="index">The index number of the VARDESC structure.</param>
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
    /// <term>TYPE_E_IOERROR</term>
    /// <term>The function cannot read from the file.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_INVDATAREAD</term>
    /// <term>The function cannot read from the file.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_UNSUPFORMAT</term>
    /// <term>The type library has an old format.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_INVALIDSTATE</term>
    /// <term>The type library cannot be opened.</term>
    /// </item>
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo2-deletevardesc HRESULT DeleteVarDesc(UINT index);
    [PreserveSig]
    HRESULT DeleteVarDesc(uint index);

    /// <summary>Deletes the specified VARDESC structure.</summary>
    /// <param name="memid">The member identifier of the VARDESC to be deleted.</param>
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
    /// <term>TYPE_E_IOERROR</term>
    /// <term>The function cannot read from the file.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_INVDATAREAD</term>
    /// <term>The function cannot read from the file.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_UNSUPFORMAT</term>
    /// <term>The type library has an old format.</term>
    /// </item>
    /// <item>
    /// <term>TYPE_E_INVALIDSTATE</term>
    /// <term>The type library cannot be opened.</term>
    /// </item>
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo2-deletevardescbymemid HRESULT
    // DeleteVarDescByMemId(MEMBERID memid);
    [PreserveSig]
    HRESULT DeleteVarDescByMemId(int memid);

    /// <summary>Deletes the IMPLTYPE flags for the indexed interface.</summary>
    /// <param name="index">The index of the interface for which to delete the type flags.</param>
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
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo2-deleteimpltype HRESULT DeleteImplType(//
    // UINT index);
    [PreserveSig]
    HRESULT DeleteImplType(uint index);

    /// <summary>Sets a value for custom data.</summary>
    /// <param name="guid">The unique identifier that can be used to identify the data.</param>
    /// <param name="pVarVal">The data to store (any variant except an object).</param>
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
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo2-setcustdata HRESULT SetCustData(REFGUID
    // guid, VARIANT *pVarVal);
    [PreserveSig]
    HRESULT SetCustData(in Guid guid, [In, MarshalAs(UnmanagedType.Struct)] object pVarVal);

    /// <summary>Sets a value for custom data for the specified function.</summary>
    /// <param name="index">The index of the function for which to set the custom data.</param>
    /// <param name="guid">The unique identifier used to identify the data.</param>
    /// <param name="pVarVal">The data to store (any variant except an object).</param>
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
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo2-setfunccustdata HRESULT SetFuncCustData(//
    // UINT index, REFGUID guid, VARIANT *pVarVal);
    [PreserveSig]
    HRESULT SetFuncCustData(uint index, in Guid guid, [In, MarshalAs(UnmanagedType.Struct)] object pVarVal);

    /// <summary>Sets a value for the custom data for the specified parameter.</summary>
    /// <param name="indexFunc">The index of the function for which to set the custom data.</param>
    /// <param name="indexParam">The index of the parameter of the function for which to set the custom data.</param>
    /// <param name="guid">The globally unique identifier (GUID) used to identify the data.</param>
    /// <param name="pVarVal">The data to store (any variant except an object).</param>
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
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo2-setparamcustdata HRESULT
    // SetParamCustData(// UINT indexFunc, UINT indexParam, REFGUID guid, VARIANT *pVarVal);
    [PreserveSig]
    HRESULT SetParamCustData(uint indexFunc, uint indexParam, in Guid guid, [In, MarshalAs(UnmanagedType.Struct)] object pVarVal);

    /// <summary>Sets a value for custom data for the specified variable.</summary>
    /// <param name="index">The index of the variable for which to set the custom data.</param>
    /// <param name="guid">The globally unique ID (GUID) used to identify the data.</param>
    /// <param name="pVarVal">The data to store (any variant except an object).</param>
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
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo2-setvarcustdata HRESULT SetVarCustData(//
    // UINT index, REFGUID guid, VARIANT *pVarVal);
    [PreserveSig]
    HRESULT SetVarCustData(uint index, in Guid guid, [In, MarshalAs(UnmanagedType.Struct)] object pVarVal);

    /// <summary>Sets a value for custom data for the specified implementation type.</summary>
    /// <param name="index">The index of the variable for which to set the custom data.</param>
    /// <param name="guid">The unique identifier used to identify the data.</param>
    /// <param name="pVarVal">The data to store (any variant except an object).</param>
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
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo2-setimpltypecustdata HRESULT
    // SetImplTypeCustData(UINT index, REFGUID guid, VARIANT *pVarVal);
    [PreserveSig]
    HRESULT SetImplTypeCustData(uint index, in Guid guid, [In, MarshalAs(UnmanagedType.Struct)] object pVarVal);

    /// <summary>Sets the context number for the specified Help string.</summary>
    /// <param name="dwHelpStringContext">The Help string context number.</param>
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
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo2-sethelpstringcontext HRESULT
    // SetHelpStringContext(ULONG dwHelpStringContext);
    [PreserveSig]
    HRESULT SetHelpStringContext(uint dwHelpStringContext);

    /// <summary>Sets a Help context value for a specified function.</summary>
    /// <param name="index">The index of the function for which to set the help string context.</param>
    /// <param name="dwHelpStringContext">The Help string context for a localized string.</param>
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
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo2-setfunchelpstringcontext HRESULT
    // SetFuncHelpStringContext(UINT index, ULONG dwHelpStringContext);
    [PreserveSig]
    HRESULT SetFuncHelpStringContext(uint index, uint dwHelpStringContext);

    /// <summary>Sets a Help context value for a specified variable.</summary>
    /// <param name="index">The index of the variable.</param>
    /// <param name="dwHelpStringContext">The Help string context for a localized string.</param>
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
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo2-setvarhelpstringcontext HRESULT
    // SetVarHelpStringContext(UINT index, ULONG dwHelpStringContext);
    [PreserveSig]
    HRESULT SetVarHelpStringContext(uint index, uint dwHelpStringContext);

    /// <summary>Reserved for future use.</summary>
    [PreserveSig]
    HRESULT Invalidate();

    /// <summary>Sets the name of the typeinfo.</summary>
    /// <param name="szName">The name to be assigned.</param>
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
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-icreatetypeinfo2-setname HRESULT SetName(LPOLESTR szName);
    [PreserveSig]
    HRESULT SetName([In, MarshalAs(UnmanagedType.LPWStr)] string szName);
}
