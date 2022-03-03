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
/// Exposes objects, methods and properties to programming tools and other applications that support Automation. COM components
/// implement the <c>IDispatch</c> interface to enable access by Automation clients, such as Visual Basic.
/// </summary>
// https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nn-oaidl-idispatch
[System.Security.SuppressUnmanagedCodeSecurity, ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00020400-0000-0000-C000-000000000046")]
public interface IDispatch
{
    /// <summary>Retrieves the number of type information interfaces that an object provides (either 0 or 1).</summary>
    /// <param name="pctinfo">
    /// The number of type information interfaces provided by the object. If the object provides type information, this number is 1;
    /// otherwise the number is 0.
    /// </param>
    /// <remarks>
    /// The method may return zero, which indicates that the object does not provide any type information. In this case, the object
    /// may still be programmable through <c>IDispatch</c> or a VTBL, but does not provide run-time type information for browsers,
    /// compilers, or other programming tools that access type information. This can be useful for hiding an object from browsers.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-idispatch-gettypeinfocount HRESULT GetTypeInfoCount(UINT
    // *pctinfo);
    [System.Security.SecurityCritical]
    void GetTypeInfoCount(out uint pctinfo);

    /// <summary>Retrieves the type information for an object, which can then be used to get the type information for an interface.</summary>
    /// <param name="iTInfo">The type information to return. Pass 0 to retrieve type information for the IDispatch implementation.</param>
    /// <param name="lcid">
    /// The locale identifier for the type information. An object may be able to return different type information for different
    /// languages. This is important for classes that support localized member names. For classes that do not support localized
    /// member names, this parameter can be ignored.
    /// </param>
    /// <param name="ppTInfo">The requested type information object.</param>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-idispatch-gettypeinfo HRESULT GetTypeInfo(UINT iTInfo, LCID
    // lcid, ITypeInfo **ppTInfo);
    [System.Security.SecurityCritical]
    void GetTypeInfo(uint iTInfo, int lcid, out ITypeInfo ppTInfo);

    /// <summary>
    /// Maps a single member and an optional set of argument names to a corresponding set of integer DISPIDs, which can be used on
    /// subsequent calls to Invoke. The dispatch function DispGetIDsOfNames provides a standard implementation of <c>GetIDsOfNames</c>.
    /// </summary>
    /// <param name="riid">Reserved for future use. Must be IID_NULL.</param>
    /// <param name="rgszNames">The array of names to be mapped.</param>
    /// <param name="cNames">The count of the names to be mapped.</param>
    /// <param name="lcid">The locale context in which to interpret the names.</param>
    /// <param name="rgDispId">
    /// Caller-allocated array, each element of which contains an identifier (ID) corresponding to one of the names passed in the
    /// rgszNames array. The first element represents the member name. The subsequent elements represent each of the member's parameters.
    /// </param>
    /// <remarks>
    /// <para>
    /// An IDispatch implementation can associate any positive integer ID value with a given name. Zero is reserved for the default,
    /// or <c>Value</c> property; –1 is reserved to indicate an unknown name; and other negative values are defined for other
    /// purposes. For example, if <c>GetIDsOfNames</c> is called, and the implementation does not recognize one or more of the
    /// names, it returns DISP_E_UNKNOWNNAME, and the rgDispId array contains DISPID_UNKNOWN for the entries that correspond to the
    /// unknown names.
    /// </para>
    /// <para>
    /// The member and parameter DISPIDs must remain constant for the lifetime of the object. This allows a client to obtain the
    /// DISPIDs once, and cache them for later use.
    /// </para>
    /// <para>
    /// When <c>GetIDsOfNames</c> is called with more than one name, the first name (rgszNames[0]) corresponds to the member name,
    /// and subsequent names correspond to the names of the member's parameters.
    /// </para>
    /// <para>
    /// The same name may map to different DISPIDs, depending on context. For example, a name may have a DISPID when it is used as a
    /// member name with a particular interface, a different ID as a member of a different interface, and different mapping for each
    /// time it appears as a parameter.
    /// </para>
    /// <para>
    /// <c>GetIDsOfNames</c> is used when an IDispatch client binds to names at run time. To bind at compile time instead, an
    /// <c>IDispatch</c> client can map names to DISPIDs by using the type information interfaces described in Type Description
    /// Interfaces. This allows a client to bind to members at compile time and avoid calling <c>GetIDsOfNames</c> at run time. For
    /// a description of binding at compile time, see Type Description Interfaces.
    /// </para>
    /// <para>
    /// The implementation of <c>GetIDsOfNames</c> is case insensitive. Users that need case-sensitive name mapping should use type
    /// information interfaces to map names to DISPIDs, rather than call <c>GetIDsOfNames</c>.
    /// </para>
    /// <para>
    /// <c>Caution</c> You cannot use this method to access values that have been added dynamically, such as values added through
    /// JavaScript. Instead, use the GetDispID of the IDispatchEx interface. For more information, see the IDispatchEx interface.
    /// </para>
    /// <para>Examples</para>
    /// <para>
    /// The following code from the Lines sample file Lines.cpp implements the <c>GetIDsOfNames</c> member function for the CLine
    /// class. The ActiveX or OLE object uses the standard implementation, DispGetIDsOfNames. This implementation relies on
    /// <c>DispGetIdsOfNames</c> to validate input arguments. To help minimize security risks, include code that performs more
    /// robust validation of the input arguments.
    /// </para>
    /// <para>
    /// The following code might appear in an ActiveX client that calls <c>GetIDsOfNames</c> to get the DISPID of the
    /// <c>CLine</c><c>Color</c> property.
    /// </para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-idispatch-getidsofnames HRESULT GetIDsOfNames(REFIID riid,
    // LPOLESTR *rgszNames, UINT cNames, LCID lcid, DISPID *rgDispId);
    [System.Security.SecurityCritical]
    void GetIDsOfNames([Optional] in Guid riid, [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 2)] string[] rgszNames,
        uint cNames, int lcid, [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4, SizeParamIndex = 2)] int[] rgDispId);

    /// <summary>
    /// Provides access to properties and methods exposed by an object. The dispatch function DispInvoke provides a standard
    /// implementation of <c>Invoke</c>.
    /// </summary>
    /// <param name="dispIdMember">
    /// Identifies the member. Use GetIDsOfNames or the object's documentation to obtain the dispatch identifier.
    /// </param>
    /// <param name="riid">Reserved for future use. Must be IID_NULL.</param>
    /// <param name="lcid">
    /// <para>
    /// The locale context in which to interpret arguments. The <paramref name="lcid"/> is used by the GetIDsOfNames function, and
    /// is also passed to <c>Invoke</c> to allow the object to interpret its arguments specific to a locale.
    /// </para>
    /// <para>
    /// Applications that do not support multiple national languages can ignore this parameter. For more information, refer to
    /// Supporting Multiple National Languages and Exposing ActiveX Objects.
    /// </para>
    /// </param>
    /// <param name="wFlags">
    /// <para>Flags describing the context of the <c>Invoke</c> call.</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Value</term>
    /// <term>Meaning</term>
    /// </listheader>
    /// <item>
    /// <term>DISPATCH_METHOD</term>
    /// <term>
    /// The member is invoked as a method. If a property has the same name, both this and the DISPATCH_PROPERTYGET flag can be set.
    /// </term>
    /// </item>
    /// <item>
    /// <term>DISPATCH_PROPERTYGET</term>
    /// <term>The member is retrieved as a property or data member.</term>
    /// </item>
    /// <item>
    /// <term>DISPATCH_PROPERTYPUT</term>
    /// <term>The member is changed as a property or data member.</term>
    /// </item>
    /// <item>
    /// <term>DISPATCH_PROPERTYPUTREF</term>
    /// <term>
    /// The member is changed by a reference assignment, rather than a value assignment. This flag is valid only when the property
    /// accepts a reference to an object.
    /// </term>
    /// </item>
    /// </list>
    /// </param>
    /// <param name="pDispParams">
    /// Pointer to a DISPPARAMS structure containing an array of arguments, an array of argument DISPIDs for named arguments, and
    /// counts for the number of elements in the arrays.
    /// </param>
    /// <param name="pVarResult">
    /// Pointer to the location where the result is to be stored, or NULL if the caller expects no result. This argument is ignored
    /// if DISPATCH_PROPERTYPUT or DISPATCH_PROPERTYPUTREF is specified.
    /// </param>
    /// <param name="pExcepInfo">
    /// Pointer to a structure that contains exception information. This structure should be filled in if DISP_E_EXCEPTION is
    /// returned. Can be NULL.
    /// </param>
    /// <param name="puArgErr">
    /// The index within rgvarg of the first argument that has an error. Arguments are stored in pDispParams-&gt;rgvarg in reverse
    /// order, so the first argument is the one with the highest index in the array. This parameter is returned only when the
    /// resulting return value is DISP_E_TYPEMISMATCH or DISP_E_PARAMNOTFOUND. This argument can be set to null. For details, see
    /// Returning Errors.
    /// </param>
    /// <remarks>
    /// <para>
    /// Generally, you should not implement <c>Invoke</c> directly. Instead, use the dispatch interface to create functions
    /// CreateStdDispatch and DispInvoke. For details, refer to <c>CreateStdDispatch</c>, <c>DispInvoke</c>, Creating the IDispatch
    /// Interface and Exposing ActiveX Objects.
    /// </para>
    /// <para>
    /// If some application-specific processing needs to be performed before calling a member, the code should perform the necessary
    /// actions, and then call ITypeInfo::Invoke to invoke the member. <c>ITypeInfo::Invoke</c> acts exactly like <c>Invoke</c>. The
    /// standard implementations of <c>Invoke</c> created by <c>CreateStdDispatch</c> and <c>DispInvoke</c> defer to <c>ITypeInfo::Invoke</c>.
    /// </para>
    /// <para>
    /// In an ActiveX client, <c>Invoke</c> should be used to get and set the values of properties, or to call a method of an
    /// ActiveX object. The dispIdMember argument identifies the member to invoke. The DISPIDs that identify members are defined by
    /// the implementor of the object and can be determined by using the object's documentation, the IDispatch::GetIDsOfNames
    /// function, or the ITypeInfo interface.
    /// </para>
    /// <para>
    /// When you use <c>IDispatch::Invoke()</c> with DISPATCH_PROPERTYPUT or DISPATCH_PROPERTYPUTREF, you have to specially
    /// initialize the <c>cNamedArgs</c> and <c>rgdispidNamedArgs</c> elements of your DISPPARAMS structure with the following:
    /// </para>
    /// <para>
    /// The information that follows addresses developers of ActiveX clients and others who use code to expose ActiveX objects. It
    /// describes the behavior that users of exposed objects should expect.
    /// </para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-idispatch-invoke HRESULT Invoke(DISPID dispIdMember, REFIID
    // riid, LCID lcid, WORD wFlags, DISPPARAMS *pDispParams, VARIANT *pVarResult, EXCEPINFO *pExcepInfo, UINT *puArgErr);
    [System.Security.SecurityCritical]
    void Invoke(int dispIdMember, [Optional] in Guid riid, int lcid, INVOKEKIND wFlags, ref DISPPARAMS pDispParams, [Optional] IntPtr pVarResult, [Optional] IntPtr pExcepInfo, [Optional] IntPtr puArgErr);
}
