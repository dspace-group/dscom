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

using System.Reflection;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices;

internal sealed class TypeProvider
{
    public TypeProvider(WriterContext context, ICustomAttributeProvider customAttributeProvider, bool isMethod = true)
    {
        Context = context;
        IsMethod = isMethod;
        if (customAttributeProvider != null)
        {
            MarshalAsAttribute = customAttributeProvider.GetCustomAttributes(typeof(MarshalAsAttribute), true).FirstOrDefault() as MarshalAsAttribute;
        }
    }

    public bool HasMarshalAsAttribute => MarshalAsAttribute != null;

    public MarshalAsAttribute? MarshalAsAttribute { get; }

    public WriterContext Context { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the type should be used for a property or method or, in the if the value is false, for a field in a struct.
    /// </summary>
    public bool IsMethod { get; } = true;

    public VarEnum GetVariantType(Type type, out VarEnum? parentLevel, bool isSafeArraySubType = false)
    {
        var marshalAsAttribute = MarshalAsAttribute;
        UnmanagedType? marshalTo = null;
        parentLevel = null;
        if (isSafeArraySubType && type.IsArray)
        {
            return VarEnum.VT_EMPTY;
        }

        if (marshalAsAttribute != null)
        {
            if (isSafeArraySubType)
            {
                switch (marshalAsAttribute.SafeArraySubType)
                {
                    case VarEnum.VT_FILETIME:
                    case VarEnum.VT_BLOB:
                    case VarEnum.VT_STREAM:
                    case VarEnum.VT_STORAGE:
                    case VarEnum.VT_STREAMED_OBJECT:
                    case VarEnum.VT_STORED_OBJECT:
                    case VarEnum.VT_BLOB_OBJECT:
                    case VarEnum.VT_CF:
                    case VarEnum.VT_CLSID:
                    case VarEnum.VT_VECTOR:
                    case VarEnum.VT_ARRAY:
                    case VarEnum.VT_BYREF:
                    case VarEnum.VT_PTR:
                    case VarEnum.VT_CARRAY:
                    case VarEnum.VT_RECORD:
                    case VarEnum.VT_SAFEARRAY:
                        return VarEnum.VT_EMPTY;
                }

                if (marshalAsAttribute.SafeArraySubType != VarEnum.VT_EMPTY)
                {
                    if (marshalAsAttribute.SafeArraySubType == VarEnum.VT_USERDEFINED)
                    {
                        marshalTo = null;
                    }
                    else
                    {
                        return marshalAsAttribute.SafeArraySubType;
                    }
                }
            }
            else
            {
                marshalTo = marshalAsAttribute.Value;
            }
        }

        switch (type.FullName)
        {
            case "System.Void":
                return VarEnum.VT_VOID;
            case "System.Boolean":
                if (marshalTo != null)
                {
                    switch (marshalTo)
                    {
                        case 0:
                            return IsMethod ? VarEnum.VT_BOOL : VarEnum.VT_I4;
                        case UnmanagedType.VariantBool:
                        case UnmanagedType.AsAny:
                            return VarEnum.VT_BOOL;
                        case UnmanagedType.Bool:
                            return VarEnum.VT_I4;
                        case UnmanagedType.U1:
                        case UnmanagedType.I1:
                            return VarEnum.VT_UI1;
                        default:
                            return VarEnum.VT_EMPTY;
                    }
                }
                return IsMethod ? VarEnum.VT_BOOL : VarEnum.VT_I4;
            case "System.Decimal":
                if (marshalTo != null)
                {
                    switch (marshalTo)
                    {
                        case UnmanagedType.AsAny:
                            return VarEnum.VT_DECIMAL;
                        case UnmanagedType.Currency:
                            return VarEnum.VT_CY;
                        default:
                            return VarEnum.VT_EMPTY;
                    }
                }
                return VarEnum.VT_DECIMAL;
            case "System.Single":
                return VarEnum.VT_R4;
            case "System.Double":
                return VarEnum.VT_R8;
            case "System.Object":
                if (marshalTo != null)
                {
                    switch (marshalTo)
                    {
                        case UnmanagedType.AsAny:
                            return VarEnum.VT_VARIANT;
                        case UnmanagedType.IUnknown:
                            return VarEnum.VT_UNKNOWN;
                        case UnmanagedType.IDispatch:
                            return VarEnum.VT_DISPATCH;
                        case UnmanagedType.Struct:
                            return VarEnum.VT_VARIANT;
                        case UnmanagedType.Interface:
                            return VarEnum.VT_UNKNOWN;
                        default:
                            return VarEnum.VT_EMPTY;
                    }
                }
                return IsMethod ? VarEnum.VT_VARIANT : VarEnum.VT_UNKNOWN;
            case "System.String":
                if (marshalTo != null)
                {
                    switch (marshalTo)
                    {
                        case UnmanagedType.BStr:
                        case UnmanagedType.AsAny:
                            return VarEnum.VT_BSTR;
                        case UnmanagedType.LPStr:
                            return VarEnum.VT_LPSTR;
                        case UnmanagedType.LPWStr:
                            return VarEnum.VT_LPWSTR;
                        default:
                            return VarEnum.VT_EMPTY;
                    }
                }
                return IsMethod ? VarEnum.VT_BSTR : VarEnum.VT_LPSTR;
            case "System.Byte":
                return VarEnum.VT_UI1;
            case "System.Char":
                if (marshalTo != null)
                {
                    switch (marshalTo)
                    {
                        case 0:
                        case UnmanagedType.AsAny:
                        case UnmanagedType.U2:
                        case UnmanagedType.I2:
                            return VarEnum.VT_UI2;
                        case UnmanagedType.U1:
                        case UnmanagedType.I1:
                            return VarEnum.VT_UI1;
                        default:
                            return VarEnum.VT_EMPTY;
                    }
                }
                return IsMethod ? VarEnum.VT_UI2 : VarEnum.VT_UI1;
            case "System.SByte":
                return VarEnum.VT_I1;
            case "System.Int16":
                return VarEnum.VT_I2;
            case "System.Int64":
                return VarEnum.VT_I8;
            case "System.UInt16":
                return VarEnum.VT_UI2;
            case "System.Int32":
                if (marshalTo != null)
                {
                    switch (marshalTo)
                    {
                        case 0:
                        case UnmanagedType.I4:
                        case UnmanagedType.AsAny:
                        case UnmanagedType.U4:
                        case UnmanagedType.Interface:
                            return VarEnum.VT_I4;
                        case UnmanagedType.Error:
                            return VarEnum.VT_HRESULT;
                        default:
                            return VarEnum.VT_EMPTY;
                    }
                }
                return VarEnum.VT_I4;
            case "System.UInt32":
                if (marshalTo != null)
                {
                    switch (marshalTo)
                    {
                        case 0:
                        case UnmanagedType.AsAny:
                        case UnmanagedType.U4:
                            return VarEnum.VT_UI4;
                        case UnmanagedType.Error:
                            return VarEnum.VT_HRESULT;
                        default:
                            return VarEnum.VT_EMPTY;
                    }
                }
                return VarEnum.VT_UI4;
            case "System.UInt64":
                return VarEnum.VT_UI8;
            case "System.Object[]":
                if (marshalTo != null)
                {
                    switch (marshalTo)
                    {
                        case UnmanagedType.SafeArray:
                        case UnmanagedType.AsAny:
                            return VarEnum.VT_SAFEARRAY;
                        case UnmanagedType.LPArray:
                            return VarEnum.VT_PTR;
                        default:
                            return VarEnum.VT_EMPTY;
                    }
                }
                return VarEnum.VT_SAFEARRAY;
            case "System.Collections.IEnumerator":
                parentLevel = VarEnum.VT_PTR;
                return VarEnum.VT_USERDEFINED;
            case "System.DateTime":
                return VarEnum.VT_DATE;
            case "System.Delegate":
                return VarEnum.VT_UNKNOWN;
            case "System.IntPtr":
                return Environment.Is64BitProcess ? VarEnum.VT_I8 : VarEnum.VT_I4;
            case "System.Drawing.Color":
                return VarEnum.VT_USERDEFINED;
            case "System.Guid":
                if (marshalTo != null)
                {
                    switch (marshalTo)
                    {
                        case UnmanagedType.Struct:
                        case UnmanagedType.AsAny:
                            return VarEnum.VT_USERDEFINED;
                        case UnmanagedType.LPStruct:
                            parentLevel = VarEnum.VT_PTR;
                            return VarEnum.VT_USERDEFINED;
                        default:
                            return VarEnum.VT_EMPTY;
                    }
                }
                return VarEnum.VT_USERDEFINED;

            default:
                if (type.ToString() == typeof(IDispatch).FullName)
                {
                    return VarEnum.VT_DISPATCH;
                }
                else if (type.ToString() == typeof(IUnknown).FullName)
                {
                    return VarEnum.VT_UNKNOWN;
                }
                if (type.IsInterface && !type.IsGenericType)
                {
                    if (Context.TypeInfoResolver.ResolveTypeInfo(type) != null)
                    {
                        parentLevel = VarEnum.VT_PTR;
                        return VarEnum.VT_USERDEFINED;
                    }
                    else
                    {
                        return VarEnum.VT_UNKNOWN;
                    }
                }
                else if (type.IsEnum)
                {
                    if (marshalTo != null)
                    {
                        return GetVariantType(type.GetEnumUnderlyingType(), out parentLevel, isSafeArraySubType);
                    }

                    return VarEnum.VT_USERDEFINED;
                }
                else if (type.IsArray)
                {
                    if (marshalTo != null)
                    {
                        return marshalTo.Value switch
                        {
                            UnmanagedType.LPArray => VarEnum.VT_PTR,
                            _ => VarEnum.VT_SAFEARRAY,
                        };
                    }
                    return VarEnum.VT_SAFEARRAY;
                }
                else if (type.IsValueType && !type.IsPrimitive && !type.IsEnum)
                {
                    // struct
                    return VarEnum.VT_USERDEFINED;
                }

                if (Context.TypeInfoResolver.ResolveDefaultCoClassInterface(type) != null)
                {
                    parentLevel = VarEnum.VT_PTR;
                    return VarEnum.VT_USERDEFINED;
                }

                // Don't log a warning! See behavior of tlbexp.exe.
                return VarEnum.VT_UNKNOWN;
        }
    }
}
