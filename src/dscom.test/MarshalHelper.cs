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

namespace dSPACE.Runtime.InteropServices.Tests;

internal static class MarshalHelper
{
    internal record struct UnmanagedTypeMap(Type Type, UnmanagedType UnmanagedType);

    internal static Dictionary<UnmanagedTypeMap, VarEnum?> UnmanagedTypeMapDict { get; } = new();

    internal static VarEnum TypeToVarEnum(Type type, bool IsMethod = true)
    {
        switch (type.FullName)
        {
            case "System.Void":
                return VarEnum.VT_VOID;
            case "System.Boolean":
                return IsMethod ? VarEnum.VT_BOOL : VarEnum.VT_I4;
            case "System.Decimal":
                return VarEnum.VT_DECIMAL;
            case "System.Single":
                return VarEnum.VT_R4;
            case "System.Double":
                return VarEnum.VT_R8;
            case "System.Object":
                return IsMethod ? VarEnum.VT_VARIANT : VarEnum.VT_UNKNOWN;
            case "System.String":
                return IsMethod ? VarEnum.VT_BSTR : VarEnum.VT_LPSTR;
            case "System.Byte":
                return VarEnum.VT_UI1;
            case "System.Char":
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
                return VarEnum.VT_I4;
            case "System.UInt32":
                return VarEnum.VT_UI4;
            case "System.UInt64":
                return VarEnum.VT_UI8;
            case "System.Object[]":
                return VarEnum.VT_SAFEARRAY;
            case "System.Collections.IEnumerator":
                return VarEnum.VT_USERDEFINED;
            case "System.DateTime":
                return VarEnum.VT_DATE;
            case "System.Delegate":
                return VarEnum.VT_UNKNOWN;
            case "System.IntPtr":
                return VarEnum.VT_I8;
            case "System.Drawing.Color":
                return VarEnum.VT_USERDEFINED;
            case "System.Guid":
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
                    return VarEnum.VT_USERDEFINED;
                }
                else if (type.IsEnum)
                {
                    return VarEnum.VT_USERDEFINED;
                }
                else if (type.IsArray)
                {
                    return VarEnum.VT_SAFEARRAY;
                }
                else if (!type.IsGenericType && type.BaseType == typeof(MulticastDelegate))
                {
                    return VarEnum.VT_UNKNOWN;
                }
                else if (type.IsValueType && !type.IsPrimitive && !type.IsEnum)
                {
                    //struct
                    return VarEnum.VT_USERDEFINED;
                }
                return VarEnum.VT_UNKNOWN;
        }
    }

    static MarshalHelper()
    {
        // Only for fields
        // UnmanagedType.ByValTStr
        // UnmanagedType.ByValArray

        // Attribute parameter 'MarshalType' or 'MarshalTypeRef' must be specified.
        // UnmanagedType.CustomMarshaler

        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.Bool), /*             */ VarEnum.VT_I4);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.VariantBool), /*      */ VarEnum.VT_BOOL);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.I1), /*               */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.U1), /*               */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.I2), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.U2), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.I4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.U4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.I8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.U8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.R4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.R8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.Currency), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.BStr), /*             */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.LPStr), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.LPWStr), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.LPTStr), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.IUnknown), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.IDispatch), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.Struct), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.Interface), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.SafeArray), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.SysInt), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.SysUInt), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.VBByRefStr), /*       */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.AnsiBStr), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.TBStr), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.FunctionPtr), /*      */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.AsAny), /*            */ VarEnum.VT_BOOL);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.LPArray), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.LPStruct), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.Error), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.IInspectable), /*     */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.HString), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(bool), UnmanagedType.LPUTF8Str), /*        */ null);

        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.Bool), /*             */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.VariantBool), /*      */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.I1), /*               */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.U1), /*               */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.I2), /*               */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.U2), /*               */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.I4), /*               */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.U4), /*               */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.I8), /*               */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.U8), /*               */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.R4), /*               */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.R8), /*               */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.Currency), /*         */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.BStr), /*             */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.LPStr), /*            */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.LPWStr), /*           */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.LPTStr), /*           */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.IUnknown), /*         */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.IDispatch), /*        */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.Struct), /*           */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.Interface), /*        */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.SafeArray), /*        */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.SysInt), /*           */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.SysUInt), /*          */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.VBByRefStr), /*       */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.AnsiBStr), /*         */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.TBStr), /*            */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.FunctionPtr), /*      */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.AsAny), /*            */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.LPArray), /*          */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.LPStruct), /*         */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.Error), /*            */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.IInspectable), /*     */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.HString), /*          */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(byte), UnmanagedType.LPUTF8Str), /*        */ VarEnum.VT_UI1);

        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.Bool), /*             */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.VariantBool), /*      */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.I1), /*               */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.U1), /*               */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.I2), /*               */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.U2), /*               */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.I4), /*               */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.U4), /*               */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.I8), /*               */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.U8), /*               */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.R4), /*               */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.R8), /*               */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.Currency), /*         */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.BStr), /*             */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.LPStr), /*            */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.LPWStr), /*           */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.LPTStr), /*           */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.IUnknown), /*         */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.IDispatch), /*        */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.Struct), /*           */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.Interface), /*        */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.SafeArray), /*        */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.SysInt), /*           */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.SysUInt), /*          */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.VBByRefStr), /*       */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.AnsiBStr), /*         */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.TBStr), /*            */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.FunctionPtr), /*      */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.AsAny), /*            */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.LPArray), /*          */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.LPStruct), /*         */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.Error), /*            */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.IInspectable), /*     */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.HString), /*          */ VarEnum.VT_I1);
        UnmanagedTypeMapDict.Add(new(typeof(sbyte), UnmanagedType.LPUTF8Str), /*        */ VarEnum.VT_I1);

        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.Bool), /*             */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.VariantBool), /*      */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.I1), /*               */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.U1), /*               */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.I2), /*               */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.U2), /*               */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.I4), /*               */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.U4), /*               */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.I8), /*               */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.U8), /*               */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.R4), /*               */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.R8), /*               */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.Currency), /*         */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.BStr), /*             */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.LPStr), /*            */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.LPWStr), /*           */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.LPTStr), /*           */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.IUnknown), /*         */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.IDispatch), /*        */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.Struct), /*           */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.Interface), /*        */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.SafeArray), /*        */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.SysInt), /*           */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.SysUInt), /*          */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.VBByRefStr), /*       */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.AnsiBStr), /*         */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.TBStr), /*            */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.FunctionPtr), /*      */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.AsAny), /*            */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.LPArray), /*          */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.LPStruct), /*         */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.Error), /*            */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.IInspectable), /*     */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.HString), /*          */ VarEnum.VT_I2);
        UnmanagedTypeMapDict.Add(new(typeof(short), UnmanagedType.LPUTF8Str), /*        */ VarEnum.VT_I2);

        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.Bool), /*             */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.VariantBool), /*      */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.I1), /*               */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.U1), /*               */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.I2), /*               */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.U2), /*               */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.I4), /*               */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.U4), /*               */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.I8), /*               */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.U8), /*               */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.R4), /*               */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.R8), /*               */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.Currency), /*         */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.BStr), /*             */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.LPStr), /*            */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.LPWStr), /*           */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.LPTStr), /*           */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.IUnknown), /*         */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.IDispatch), /*        */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.Struct), /*           */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.Interface), /*        */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.SafeArray), /*        */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.SysInt), /*           */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.SysUInt), /*          */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.VBByRefStr), /*       */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.AnsiBStr), /*         */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.TBStr), /*            */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.FunctionPtr), /*      */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.AsAny), /*            */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.LPArray), /*          */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.LPStruct), /*         */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.Error), /*            */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.IInspectable), /*     */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.HString), /*          */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(ushort), UnmanagedType.LPUTF8Str), /*        */ VarEnum.VT_UI2);

        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.Bool), /*             */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.VariantBool), /*      */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.I1), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.U1), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.I2), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.U2), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.I4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.U4), /*               */ VarEnum.VT_UI4);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.I8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.U8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.R4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.R8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.Currency), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.BStr), /*             */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.LPStr), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.LPWStr), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.LPTStr), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.IUnknown), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.IDispatch), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.Struct), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.Interface), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.SafeArray), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.SysInt), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.SysUInt), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.VBByRefStr), /*       */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.AnsiBStr), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.TBStr), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.FunctionPtr), /*      */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.AsAny), /*            */ VarEnum.VT_UI4);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.LPArray), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.LPStruct), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.Error), /*            */ VarEnum.VT_HRESULT);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.IInspectable), /*     */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.HString), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(uint), UnmanagedType.LPUTF8Str), /*        */ null);

        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.Bool), /*             */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.VariantBool), /*      */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.I1), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.U1), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.I2), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.U2), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.I4), /*               */ VarEnum.VT_I4);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.U4), /*               */ VarEnum.VT_I4);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.I8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.U8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.R4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.R8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.Currency), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.BStr), /*             */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.LPStr), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.LPWStr), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.LPTStr), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.IUnknown), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.IDispatch), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.Struct), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.Interface), /*        */ VarEnum.VT_I4);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.SafeArray), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.SysInt), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.SysUInt), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.VBByRefStr), /*       */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.AnsiBStr), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.TBStr), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.FunctionPtr), /*      */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.AsAny), /*            */ VarEnum.VT_I4);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.LPArray), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.LPStruct), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.Error), /*            */ VarEnum.VT_HRESULT);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.IInspectable), /*     */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.HString), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(int), UnmanagedType.LPUTF8Str), /*        */ null);

        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.Bool), /*             */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.VariantBool), /*      */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.I1), /*               */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.U1), /*               */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.I2), /*               */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.U2), /*               */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.I4), /*               */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.U4), /*               */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.I8), /*               */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.U8), /*               */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.R4), /*               */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.R8), /*               */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.Currency), /*         */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.BStr), /*             */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.LPStr), /*            */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.LPWStr), /*           */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.LPTStr), /*           */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.IUnknown), /*         */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.IDispatch), /*        */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.Struct), /*           */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.Interface), /*        */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.SafeArray), /*        */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.SysInt), /*           */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.SysUInt), /*          */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.VBByRefStr), /*       */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.AnsiBStr), /*         */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.TBStr), /*            */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.FunctionPtr), /*      */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.AsAny), /*            */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.LPArray), /*          */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.LPStruct), /*         */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.Error), /*            */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.IInspectable), /*     */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.HString), /*          */ VarEnum.VT_UI8);
        UnmanagedTypeMapDict.Add(new(typeof(ulong), UnmanagedType.LPUTF8Str), /*        */ VarEnum.VT_UI8);

        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.Bool), /*             */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.VariantBool), /*      */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.I1), /*               */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.U1), /*               */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.I2), /*               */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.U2), /*               */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.I4), /*               */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.U4), /*               */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.I8), /*               */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.U8), /*               */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.R4), /*               */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.R8), /*               */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.Currency), /*         */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.BStr), /*             */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.LPStr), /*            */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.LPWStr), /*           */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.LPTStr), /*           */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.IUnknown), /*         */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.IDispatch), /*        */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.Struct), /*           */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.Interface), /*        */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.SafeArray), /*        */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.SysInt), /*           */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.SysUInt), /*          */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.VBByRefStr), /*       */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.AnsiBStr), /*         */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.TBStr), /*            */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.FunctionPtr), /*      */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.AsAny), /*            */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.LPArray), /*          */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.LPStruct), /*         */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.Error), /*            */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.IInspectable), /*     */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.HString), /*          */ VarEnum.VT_I8);
        UnmanagedTypeMapDict.Add(new(typeof(long), UnmanagedType.LPUTF8Str), /*        */ VarEnum.VT_I8);

        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.Bool), /*             */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.VariantBool), /*      */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.I1), /*               */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.U1), /*               */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.I2), /*               */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.U2), /*               */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.I4), /*               */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.U4), /*               */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.I8), /*               */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.U8), /*               */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.R4), /*               */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.R8), /*               */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.Currency), /*         */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.BStr), /*             */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.LPStr), /*            */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.LPWStr), /*           */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.LPTStr), /*           */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.IUnknown), /*         */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.IDispatch), /*        */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.Struct), /*           */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.Interface), /*        */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.SafeArray), /*        */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.SysInt), /*           */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.SysUInt), /*          */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.VBByRefStr), /*       */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.AnsiBStr), /*         */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.TBStr), /*            */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.FunctionPtr), /*      */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.AsAny), /*            */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.LPArray), /*          */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.LPStruct), /*         */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.Error), /*            */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.IInspectable), /*     */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.HString), /*          */ VarEnum.VT_R4);
        UnmanagedTypeMapDict.Add(new(typeof(float), UnmanagedType.LPUTF8Str), /*        */ VarEnum.VT_R4);

        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.Bool), /*             */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.VariantBool), /*      */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.I1), /*               */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.U1), /*               */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.I2), /*               */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.U2), /*               */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.I4), /*               */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.U4), /*               */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.I8), /*               */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.U8), /*               */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.R4), /*               */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.R8), /*               */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.Currency), /*         */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.BStr), /*             */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.LPStr), /*            */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.LPWStr), /*           */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.LPTStr), /*           */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.IUnknown), /*         */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.IDispatch), /*        */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.Struct), /*           */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.Interface), /*        */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.SafeArray), /*        */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.SysInt), /*           */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.SysUInt), /*          */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.VBByRefStr), /*       */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.AnsiBStr), /*         */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.TBStr), /*            */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.FunctionPtr), /*      */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.AsAny), /*            */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.LPArray), /*          */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.LPStruct), /*         */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.Error), /*            */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.IInspectable), /*     */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.HString), /*          */ VarEnum.VT_R8);
        UnmanagedTypeMapDict.Add(new(typeof(double), UnmanagedType.LPUTF8Str), /*        */ VarEnum.VT_R8);

        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.Bool), /*             */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.VariantBool), /*      */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.I1), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.U1), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.I2), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.U2), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.I4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.U4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.I8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.U8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.R4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.R8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.Currency), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.BStr), /*             */ VarEnum.VT_BSTR);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.LPStr), /*            */ VarEnum.VT_LPSTR);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.LPWStr), /*           */ VarEnum.VT_LPWSTR);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.LPTStr), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.IUnknown), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.IDispatch), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.Struct), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.Interface), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.SafeArray), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.SysInt), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.SysUInt), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.VBByRefStr), /*       */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.AnsiBStr), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.TBStr), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.FunctionPtr), /*      */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.AsAny), /*            */ VarEnum.VT_BSTR);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.LPArray), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.LPStruct), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.Error), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.IInspectable), /*     */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.HString), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(string), UnmanagedType.LPUTF8Str), /*        */ null);

        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.Bool), /*             */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.VariantBool), /*      */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.I1), /*               */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.U1), /*               */ VarEnum.VT_UI1);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.I2), /*               */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.U2), /*               */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.I4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.U4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.I8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.U8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.R4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.R8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.Currency), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.BStr), /*             */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.LPStr), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.LPWStr), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.LPTStr), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.IUnknown), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.IDispatch), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.Struct), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.Interface), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.SafeArray), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.SysInt), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.SysUInt), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.VBByRefStr), /*       */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.AnsiBStr), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.TBStr), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.FunctionPtr), /*      */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.AsAny), /*            */ VarEnum.VT_UI2);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.LPArray), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.LPStruct), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.Error), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.IInspectable), /*     */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.HString), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(char), UnmanagedType.LPUTF8Str), /*        */ null);

        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.Bool), /*             */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.VariantBool), /*      */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.I1), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.U1), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.I2), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.U2), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.I4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.U4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.I8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.U8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.R4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.R8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.Currency), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.BStr), /*             */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.LPStr), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.LPWStr), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.LPTStr), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.IUnknown), /*         */ VarEnum.VT_UNKNOWN);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.IDispatch), /*        */ VarEnum.VT_DISPATCH);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.Struct), /*           */ VarEnum.VT_VARIANT);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.Interface), /*        */ VarEnum.VT_UNKNOWN);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.SafeArray), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.SysInt), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.SysUInt), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.VBByRefStr), /*       */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.AnsiBStr), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.TBStr), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.FunctionPtr), /*      */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.AsAny), /*            */ VarEnum.VT_VARIANT);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.LPArray), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.LPStruct), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.Error), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.IInspectable), /*     */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.HString), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object), UnmanagedType.LPUTF8Str), /*        */ null);

        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.Bool), /*             */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.VariantBool), /*      */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.I1), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.U1), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.I2), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.U2), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.I4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.U4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.I8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.U8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.R4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.R8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.Currency), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.BStr), /*             */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.LPStr), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.LPWStr), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.LPTStr), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.IUnknown), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.IDispatch), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.Struct), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.Interface), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.SafeArray), /*        */ VarEnum.VT_SAFEARRAY);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.SysInt), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.SysUInt), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.VBByRefStr), /*       */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.AnsiBStr), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.TBStr), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.FunctionPtr), /*      */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.AsAny), /*            */ VarEnum.VT_SAFEARRAY);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.LPArray), /*          */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.LPStruct), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.Error), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.IInspectable), /*     */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.HString), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(object[]), UnmanagedType.LPUTF8Str), /*        */ null);

        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.Bool), /*             */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.VariantBool), /*      */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.I1), /*               */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.U1), /*               */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.I2), /*               */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.U2), /*               */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.I4), /*               */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.U4), /*               */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.I8), /*               */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.U8), /*               */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.R4), /*               */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.R8), /*               */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.Currency), /*         */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.BStr), /*             */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.LPStr), /*            */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.LPWStr), /*           */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.LPTStr), /*           */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.IUnknown), /*         */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.IDispatch), /*        */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.Struct), /*           */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.Interface), /*        */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.SafeArray), /*        */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.SysInt), /*           */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.SysUInt), /*          */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.VBByRefStr), /*       */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.AnsiBStr), /*         */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.TBStr), /*            */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.FunctionPtr), /*      */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.AsAny), /*            */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.LPArray), /*          */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.LPStruct), /*         */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.Error), /*            */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.IInspectable), /*     */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.HString), /*          */ VarEnum.VT_PTR);
        UnmanagedTypeMapDict.Add(new(typeof(System.Collections.IEnumerator), UnmanagedType.LPUTF8Str), /*        */ VarEnum.VT_PTR);

        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.Bool), /*             */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.VariantBool), /*      */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.I1), /*               */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.U1), /*               */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.I2), /*               */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.U2), /*               */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.I4), /*               */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.U4), /*               */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.I8), /*               */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.U8), /*               */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.R4), /*               */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.R8), /*               */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.Currency), /*         */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.BStr), /*             */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.LPStr), /*            */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.LPWStr), /*           */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.LPTStr), /*           */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.IUnknown), /*         */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.IDispatch), /*        */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.Struct), /*           */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.Interface), /*        */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.SafeArray), /*        */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.SysInt), /*           */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.SysUInt), /*          */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.VBByRefStr), /*       */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.AnsiBStr), /*         */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.TBStr), /*            */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.FunctionPtr), /*      */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.AsAny), /*            */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.LPArray), /*          */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.LPStruct), /*         */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.Error), /*            */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.IInspectable), /*     */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.HString), /*          */ VarEnum.VT_DATE);
        UnmanagedTypeMapDict.Add(new(typeof(DateTime), UnmanagedType.LPUTF8Str), /*        */ VarEnum.VT_DATE);

        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.Bool), /*             */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.VariantBool), /*      */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.I1), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.U1), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.I2), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.U2), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.I4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.U4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.I8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.U8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.R4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.R8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.Currency), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.BStr), /*             */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.LPStr), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.LPWStr), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.LPTStr), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.IUnknown), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.IDispatch), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.Struct), /*           */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.Interface), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.SafeArray), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.SysInt), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.SysUInt), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.VBByRefStr), /*       */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.AnsiBStr), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.TBStr), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.FunctionPtr), /*      */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.AsAny), /*            */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.LPArray), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.LPStruct), /*         */ VarEnum.VT_PTR); // VarEnum.VT_PTR -> VarEnum.VT_USERDEFINED
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.Error), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.IInspectable), /*     */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.HString), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(Guid), UnmanagedType.LPUTF8Str), /*        */ null);

        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.Bool), /*             */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.VariantBool), /*      */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.I1), /*               */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.U1), /*               */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.I2), /*               */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.U2), /*               */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.I4), /*               */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.U4), /*               */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.I8), /*               */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.U8), /*               */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.R4), /*               */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.R8), /*               */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.Currency), /*         */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.BStr), /*             */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.LPStr), /*            */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.LPWStr), /*           */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.LPTStr), /*           */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.IUnknown), /*         */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.IDispatch), /*        */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.Struct), /*           */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.Interface), /*        */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.SafeArray), /*        */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.SysInt), /*           */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.SysUInt), /*          */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.VBByRefStr), /*       */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.AnsiBStr), /*         */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.TBStr), /*            */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.FunctionPtr), /*      */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.AsAny), /*            */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.LPArray), /*          */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.LPStruct), /*         */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.Error), /*            */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.IInspectable), /*     */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.HString), /*          */ VarEnum.VT_USERDEFINED);
        UnmanagedTypeMapDict.Add(new(typeof(System.Drawing.Color), UnmanagedType.LPUTF8Str), /*        */ VarEnum.VT_USERDEFINED);

        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.Bool), /*             */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.VariantBool), /*      */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.I1), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.U1), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.I2), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.U2), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.I4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.U4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.I8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.U8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.R4), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.R8), /*               */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.Currency), /*         */ VarEnum.VT_CY);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.BStr), /*             */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.LPStr), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.LPWStr), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.LPTStr), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.IUnknown), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.IDispatch), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.Struct), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.Interface), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.SafeArray), /*        */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.SysInt), /*           */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.SysUInt), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.VBByRefStr), /*       */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.AnsiBStr), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.TBStr), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.FunctionPtr), /*      */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.AsAny), /*            */ VarEnum.VT_DECIMAL);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.LPArray), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.LPStruct), /*         */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.Error), /*            */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.IInspectable), /*     */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.HString), /*          */ null);
        UnmanagedTypeMapDict.Add(new(typeof(decimal), UnmanagedType.LPUTF8Str), /*        */ null);
    }
}
