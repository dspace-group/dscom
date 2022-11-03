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

using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using dSPACE.Runtime.InteropServices.DemoAssembly2;

namespace dSPACE.Runtime.InteropServices.DemoAssembly1;

[ComVisible(Constants.DEFAULT_VISIBILITY)]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
public interface IDemoInterfaceWithMarshalAs
{
    #region boolean
    void TestMethod_System_Boolean_Bool([MarshalAs(UnmanagedType.Bool)] bool Param1);
    void TestMethod_System_Boolean_U8([MarshalAs(UnmanagedType.U8)] bool Param1);
    void TestMethod_System_Boolean_VariantBool([MarshalAs(UnmanagedType.VariantBool)] bool Param1);
    void TestMethod_System_Boolean_I1([MarshalAs(UnmanagedType.I1)] bool Param1);
    void TestMethod_System_Boolean_U1([MarshalAs(UnmanagedType.U1)] bool Param1);
    void TestMethod_System_Boolean_I2([MarshalAs(UnmanagedType.I2)] bool Param1);
    void TestMethod_System_Boolean_U2([MarshalAs(UnmanagedType.U2)] bool Param1);
    void TestMethod_System_Boolean_I4([MarshalAs(UnmanagedType.I4)] bool Param1);
    void TestMethod_System_Boolean_U4([MarshalAs(UnmanagedType.U4)] bool Param1);
    void TestMethod_System_Boolean_I8([MarshalAs(UnmanagedType.I8)] bool Param1);
    void TestMethod_System_Boolean_R4([MarshalAs(UnmanagedType.R4)] bool Param1);
    void TestMethod_System_Boolean_R8([MarshalAs(UnmanagedType.R8)] bool Param1);
    void TestMethod_System_Boolean_Currency([MarshalAs(UnmanagedType.Currency)] bool Param1);
    void TestMethod_System_Boolean_BStr([MarshalAs(UnmanagedType.BStr)] bool Param1);
    void TestMethod_System_Boolean_LPStr([MarshalAs(UnmanagedType.LPStr)] bool Param1);
    void TestMethod_System_Boolean_LPWStr([MarshalAs(UnmanagedType.LPWStr)] bool Param1);
    void TestMethod_System_Boolean_LPTStr([MarshalAs(UnmanagedType.LPTStr)] bool Param1);
    void TestMethod_System_Boolean_IUnknown([MarshalAs(UnmanagedType.IUnknown)] bool Param1);
    void TestMethod_System_Boolean_IDispatch([MarshalAs(UnmanagedType.IDispatch)] bool Param1);
    void TestMethod_System_Boolean_Struct([MarshalAs(UnmanagedType.Struct)] bool Param1);
    void TestMethod_System_Boolean_Interface([MarshalAs(UnmanagedType.Interface)] bool Param1);
    void TestMethod_System_Boolean_SafeArray([MarshalAs(UnmanagedType.SafeArray)] bool Param1);
    void TestMethod_System_Boolean_SysInt([MarshalAs(UnmanagedType.SysInt)] bool Param1);
    void TestMethod_System_Boolean_SysUInt([MarshalAs(UnmanagedType.SysUInt)] bool Param1);
    void TestMethod_System_Boolean_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] bool Param1);
    void TestMethod_System_Boolean_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] bool Param1);
    void TestMethod_System_Boolean_TBStr([MarshalAs(UnmanagedType.TBStr)] bool Param1);
    void TestMethod_System_Boolean_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] bool Param1);
    void TestMethod_System_Boolean_AsAny([MarshalAs(UnmanagedType.AsAny)] bool Param1);
    void TestMethod_System_Boolean_LPArray([MarshalAs(UnmanagedType.LPArray)] bool Param1);
    void TestMethod_System_Boolean_LPStruct([MarshalAs(UnmanagedType.LPStruct)] bool Param1);
    void TestMethod_System_Boolean_Error([MarshalAs(UnmanagedType.Error)] bool Param1);
    void TestMethod_System_Boolean_IInspectable([MarshalAs(UnmanagedType.IInspectable)] bool Param1);
    void TestMethod_System_Boolean_HString([MarshalAs(UnmanagedType.HString)] bool Param1);
    void TestMethod_System_Boolean_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] bool Param1);
    #endregion

    #region byte
    void TestMethod_System_Byte_Bool([MarshalAs(UnmanagedType.Bool)] byte Param1);
    void TestMethod_System_Byte_VariantBool([MarshalAs(UnmanagedType.VariantBool)] byte Param1);
    void TestMethod_System_Byte_I1([MarshalAs(UnmanagedType.I1)] byte Param1);
    void TestMethod_System_Byte_U1([MarshalAs(UnmanagedType.U1)] byte Param1);
    void TestMethod_System_Byte_I2([MarshalAs(UnmanagedType.I2)] byte Param1);
    void TestMethod_System_Byte_U2([MarshalAs(UnmanagedType.U2)] byte Param1);
    void TestMethod_System_Byte_I4([MarshalAs(UnmanagedType.I4)] byte Param1);
    void TestMethod_System_Byte_U4([MarshalAs(UnmanagedType.U4)] byte Param1);
    void TestMethod_System_Byte_I8([MarshalAs(UnmanagedType.I8)] byte Param1);
    void TestMethod_System_Byte_U8([MarshalAs(UnmanagedType.U8)] byte Param1);
    void TestMethod_System_Byte_R4([MarshalAs(UnmanagedType.R4)] byte Param1);
    void TestMethod_System_Byte_R8([MarshalAs(UnmanagedType.R8)] byte Param1);
    void TestMethod_System_Byte_Currency([MarshalAs(UnmanagedType.Currency)] byte Param1);
    void TestMethod_System_Byte_BStr([MarshalAs(UnmanagedType.BStr)] byte Param1);
    void TestMethod_System_Byte_LPStr([MarshalAs(UnmanagedType.LPStr)] byte Param1);
    void TestMethod_System_Byte_LPWStr([MarshalAs(UnmanagedType.LPWStr)] byte Param1);
    void TestMethod_System_Byte_LPTStr([MarshalAs(UnmanagedType.LPTStr)] byte Param1);
    void TestMethod_System_Byte_IUnknown([MarshalAs(UnmanagedType.IUnknown)] byte Param1);
    void TestMethod_System_Byte_IDispatch([MarshalAs(UnmanagedType.IDispatch)] byte Param1);
    void TestMethod_System_Byte_Struct([MarshalAs(UnmanagedType.Struct)] byte Param1);
    void TestMethod_System_Byte_Interface([MarshalAs(UnmanagedType.Interface)] byte Param1);
    void TestMethod_System_Byte_SafeArray([MarshalAs(UnmanagedType.SafeArray)] byte Param1);
    void TestMethod_System_Byte_SysInt([MarshalAs(UnmanagedType.SysInt)] byte Param1);
    void TestMethod_System_Byte_SysUInt([MarshalAs(UnmanagedType.SysUInt)] byte Param1);
    void TestMethod_System_Byte_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] byte Param1);
    void TestMethod_System_Byte_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] byte Param1);
    void TestMethod_System_Byte_TBStr([MarshalAs(UnmanagedType.TBStr)] byte Param1);
    void TestMethod_System_Byte_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] byte Param1);
    void TestMethod_System_Byte_AsAny([MarshalAs(UnmanagedType.AsAny)] byte Param1);
    void TestMethod_System_Byte_LPArray([MarshalAs(UnmanagedType.LPArray)] byte Param1);
    void TestMethod_System_Byte_LPStruct([MarshalAs(UnmanagedType.LPStruct)] byte Param1);
    void TestMethod_System_Byte_Error([MarshalAs(UnmanagedType.Error)] byte Param1);
    void TestMethod_System_Byte_IInspectable([MarshalAs(UnmanagedType.IInspectable)] byte Param1);
    void TestMethod_System_Byte_HString([MarshalAs(UnmanagedType.HString)] byte Param1);
    void TestMethod_System_Byte_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] byte Param1);
    #endregion

    #region sbyte
    void TestMethod_System_SByte_Bool([MarshalAs(UnmanagedType.Bool)] sbyte Param1);
    void TestMethod_System_SByte_VariantBool([MarshalAs(UnmanagedType.VariantBool)] sbyte Param1);
    void TestMethod_System_SByte_I1([MarshalAs(UnmanagedType.I1)] sbyte Param1);
    void TestMethod_System_SByte_U1([MarshalAs(UnmanagedType.U1)] sbyte Param1);
    void TestMethod_System_SByte_I2([MarshalAs(UnmanagedType.I2)] sbyte Param1);
    void TestMethod_System_SByte_U2([MarshalAs(UnmanagedType.U2)] sbyte Param1);
    void TestMethod_System_SByte_I4([MarshalAs(UnmanagedType.I4)] sbyte Param1);
    void TestMethod_System_SByte_U4([MarshalAs(UnmanagedType.U4)] sbyte Param1);
    void TestMethod_System_SByte_I8([MarshalAs(UnmanagedType.I8)] sbyte Param1);
    void TestMethod_System_SByte_U8([MarshalAs(UnmanagedType.U8)] sbyte Param1);
    void TestMethod_System_SByte_R4([MarshalAs(UnmanagedType.R4)] sbyte Param1);
    void TestMethod_System_SByte_R8([MarshalAs(UnmanagedType.R8)] sbyte Param1);
    void TestMethod_System_SByte_Currency([MarshalAs(UnmanagedType.Currency)] sbyte Param1);
    void TestMethod_System_SByte_BStr([MarshalAs(UnmanagedType.BStr)] sbyte Param1);
    void TestMethod_System_SByte_LPStr([MarshalAs(UnmanagedType.LPStr)] sbyte Param1);
    void TestMethod_System_SByte_LPWStr([MarshalAs(UnmanagedType.LPWStr)] sbyte Param1);
    void TestMethod_System_SByte_LPTStr([MarshalAs(UnmanagedType.LPTStr)] sbyte Param1);
    void TestMethod_System_SByte_IUnknown([MarshalAs(UnmanagedType.IUnknown)] sbyte Param1);
    void TestMethod_System_SByte_IDispatch([MarshalAs(UnmanagedType.IDispatch)] sbyte Param1);
    void TestMethod_System_SByte_Struct([MarshalAs(UnmanagedType.Struct)] sbyte Param1);
    void TestMethod_System_SByte_Interface([MarshalAs(UnmanagedType.Interface)] sbyte Param1);
    void TestMethod_System_SByte_SafeArray([MarshalAs(UnmanagedType.SafeArray)] sbyte Param1);
    void TestMethod_System_SByte_SysInt([MarshalAs(UnmanagedType.SysInt)] sbyte Param1);
    void TestMethod_System_SByte_SysUInt([MarshalAs(UnmanagedType.SysUInt)] sbyte Param1);
    void TestMethod_System_SByte_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] sbyte Param1);
    void TestMethod_System_SByte_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] sbyte Param1);
    void TestMethod_System_SByte_TBStr([MarshalAs(UnmanagedType.TBStr)] sbyte Param1);
    void TestMethod_System_SByte_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] sbyte Param1);
    void TestMethod_System_SByte_AsAny([MarshalAs(UnmanagedType.AsAny)] sbyte Param1);
    void TestMethod_System_SByte_LPArray([MarshalAs(UnmanagedType.LPArray)] sbyte Param1);
    void TestMethod_System_SByte_LPStruct([MarshalAs(UnmanagedType.LPStruct)] sbyte Param1);
    void TestMethod_System_SByte_Error([MarshalAs(UnmanagedType.Error)] sbyte Param1);
    void TestMethod_System_SByte_IInspectable([MarshalAs(UnmanagedType.IInspectable)] sbyte Param1);
    void TestMethod_System_SByte_HString([MarshalAs(UnmanagedType.HString)] sbyte Param1);
    void TestMethod_System_SByte_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] sbyte Param1);
    #endregion

    #region short
    void TestMethod_System_Int16_Bool([MarshalAs(UnmanagedType.Bool)] short Param1);
    void TestMethod_System_Int16_VariantBool([MarshalAs(UnmanagedType.VariantBool)] short Param1);
    void TestMethod_System_Int16_I1([MarshalAs(UnmanagedType.I1)] short Param1);
    void TestMethod_System_Int16_U1([MarshalAs(UnmanagedType.U1)] short Param1);
    void TestMethod_System_Int16_I2([MarshalAs(UnmanagedType.I2)] short Param1);
    void TestMethod_System_Int16_U2([MarshalAs(UnmanagedType.U2)] short Param1);
    void TestMethod_System_Int16_I4([MarshalAs(UnmanagedType.I4)] short Param1);
    void TestMethod_System_Int16_U4([MarshalAs(UnmanagedType.U4)] short Param1);
    void TestMethod_System_Int16_I8([MarshalAs(UnmanagedType.I8)] short Param1);
    void TestMethod_System_Int16_U8([MarshalAs(UnmanagedType.U8)] short Param1);
    void TestMethod_System_Int16_R4([MarshalAs(UnmanagedType.R4)] short Param1);
    void TestMethod_System_Int16_R8([MarshalAs(UnmanagedType.R8)] short Param1);
    void TestMethod_System_Int16_Currency([MarshalAs(UnmanagedType.Currency)] short Param1);
    void TestMethod_System_Int16_BStr([MarshalAs(UnmanagedType.BStr)] short Param1);
    void TestMethod_System_Int16_LPStr([MarshalAs(UnmanagedType.LPStr)] short Param1);
    void TestMethod_System_Int16_LPWStr([MarshalAs(UnmanagedType.LPWStr)] short Param1);
    void TestMethod_System_Int16_LPTStr([MarshalAs(UnmanagedType.LPTStr)] short Param1);
    void TestMethod_System_Int16_IUnknown([MarshalAs(UnmanagedType.IUnknown)] short Param1);
    void TestMethod_System_Int16_IDispatch([MarshalAs(UnmanagedType.IDispatch)] short Param1);
    void TestMethod_System_Int16_Struct([MarshalAs(UnmanagedType.Struct)] short Param1);
    void TestMethod_System_Int16_Interface([MarshalAs(UnmanagedType.Interface)] short Param1);
    void TestMethod_System_Int16_SafeArray([MarshalAs(UnmanagedType.SafeArray)] short Param1);
    void TestMethod_System_Int16_SysInt([MarshalAs(UnmanagedType.SysInt)] short Param1);
    void TestMethod_System_Int16_SysUInt([MarshalAs(UnmanagedType.SysUInt)] short Param1);
    void TestMethod_System_Int16_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] short Param1);
    void TestMethod_System_Int16_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] short Param1);
    void TestMethod_System_Int16_TBStr([MarshalAs(UnmanagedType.TBStr)] short Param1);
    void TestMethod_System_Int16_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] short Param1);
    void TestMethod_System_Int16_AsAny([MarshalAs(UnmanagedType.AsAny)] short Param1);
    void TestMethod_System_Int16_LPArray([MarshalAs(UnmanagedType.LPArray)] short Param1);
    void TestMethod_System_Int16_LPStruct([MarshalAs(UnmanagedType.LPStruct)] short Param1);
    void TestMethod_System_Int16_Error([MarshalAs(UnmanagedType.Error)] short Param1);
    void TestMethod_System_Int16_IInspectable([MarshalAs(UnmanagedType.IInspectable)] short Param1);
    void TestMethod_System_Int16_HString([MarshalAs(UnmanagedType.HString)] short Param1);
    void TestMethod_System_Int16_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] short Param1);
    #endregion

    #region ushort
    void TestMethod_System_UInt16_Bool([MarshalAs(UnmanagedType.Bool)] ushort Param1);
    void TestMethod_System_UInt16_VariantBool([MarshalAs(UnmanagedType.VariantBool)] ushort Param1);
    void TestMethod_System_UInt16_I1([MarshalAs(UnmanagedType.I1)] ushort Param1);
    void TestMethod_System_UInt16_U1([MarshalAs(UnmanagedType.U1)] ushort Param1);
    void TestMethod_System_UInt16_I2([MarshalAs(UnmanagedType.I2)] ushort Param1);
    void TestMethod_System_UInt16_U2([MarshalAs(UnmanagedType.U2)] ushort Param1);
    void TestMethod_System_UInt16_I4([MarshalAs(UnmanagedType.I4)] ushort Param1);
    void TestMethod_System_UInt16_U4([MarshalAs(UnmanagedType.U4)] ushort Param1);
    void TestMethod_System_UInt16_I8([MarshalAs(UnmanagedType.I8)] ushort Param1);
    void TestMethod_System_UInt16_U8([MarshalAs(UnmanagedType.U8)] ushort Param1);
    void TestMethod_System_UInt16_R4([MarshalAs(UnmanagedType.R4)] ushort Param1);
    void TestMethod_System_UInt16_R8([MarshalAs(UnmanagedType.R8)] ushort Param1);
    void TestMethod_System_UInt16_Currency([MarshalAs(UnmanagedType.Currency)] ushort Param1);
    void TestMethod_System_UInt16_BStr([MarshalAs(UnmanagedType.BStr)] ushort Param1);
    void TestMethod_System_UInt16_LPStr([MarshalAs(UnmanagedType.LPStr)] ushort Param1);
    void TestMethod_System_UInt16_LPWStr([MarshalAs(UnmanagedType.LPWStr)] ushort Param1);
    void TestMethod_System_UInt16_LPTStr([MarshalAs(UnmanagedType.LPTStr)] ushort Param1);
    void TestMethod_System_UInt16_IUnknown([MarshalAs(UnmanagedType.IUnknown)] ushort Param1);
    void TestMethod_System_UInt16_IDispatch([MarshalAs(UnmanagedType.IDispatch)] ushort Param1);
    void TestMethod_System_UInt16_Struct([MarshalAs(UnmanagedType.Struct)] ushort Param1);
    void TestMethod_System_UInt16_Interface([MarshalAs(UnmanagedType.Interface)] ushort Param1);
    void TestMethod_System_UInt16_SafeArray([MarshalAs(UnmanagedType.SafeArray)] ushort Param1);
    void TestMethod_System_UInt16_SysInt([MarshalAs(UnmanagedType.SysInt)] ushort Param1);
    void TestMethod_System_UInt16_SysUInt([MarshalAs(UnmanagedType.SysUInt)] ushort Param1);
    void TestMethod_System_UInt16_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] ushort Param1);
    void TestMethod_System_UInt16_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] ushort Param1);
    void TestMethod_System_UInt16_TBStr([MarshalAs(UnmanagedType.TBStr)] ushort Param1);
    void TestMethod_System_UInt16_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] ushort Param1);
    void TestMethod_System_UInt16_AsAny([MarshalAs(UnmanagedType.AsAny)] ushort Param1);
    void TestMethod_System_UInt16_LPArray([MarshalAs(UnmanagedType.LPArray)] ushort Param1);
    void TestMethod_System_UInt16_LPStruct([MarshalAs(UnmanagedType.LPStruct)] ushort Param1);
    void TestMethod_System_UInt16_Error([MarshalAs(UnmanagedType.Error)] ushort Param1);
    void TestMethod_System_UInt16_IInspectable([MarshalAs(UnmanagedType.IInspectable)] ushort Param1);
    void TestMethod_System_UInt16_HString([MarshalAs(UnmanagedType.HString)] ushort Param1);
    void TestMethod_System_UInt16_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] ushort Param1);
    #endregion

    #region uint
    void TestMethod_System_UInt32_Bool([MarshalAs(UnmanagedType.Bool)] uint Param1);
    void TestMethod_System_UInt32_VariantBool([MarshalAs(UnmanagedType.VariantBool)] uint Param1);
    void TestMethod_System_UInt32_I1([MarshalAs(UnmanagedType.I1)] uint Param1);
    void TestMethod_System_UInt32_U1([MarshalAs(UnmanagedType.U1)] uint Param1);
    void TestMethod_System_UInt32_I2([MarshalAs(UnmanagedType.I2)] uint Param1);
    void TestMethod_System_UInt32_U2([MarshalAs(UnmanagedType.U2)] uint Param1);
    void TestMethod_System_UInt32_I4([MarshalAs(UnmanagedType.I4)] uint Param1);
    void TestMethod_System_UInt32_U4([MarshalAs(UnmanagedType.U4)] uint Param1);
    void TestMethod_System_UInt32_I8([MarshalAs(UnmanagedType.I8)] uint Param1);
    void TestMethod_System_UInt32_U8([MarshalAs(UnmanagedType.U8)] uint Param1);
    void TestMethod_System_UInt32_R4([MarshalAs(UnmanagedType.R4)] uint Param1);
    void TestMethod_System_UInt32_R8([MarshalAs(UnmanagedType.R8)] uint Param1);
    void TestMethod_System_UInt32_Currency([MarshalAs(UnmanagedType.Currency)] uint Param1);
    void TestMethod_System_UInt32_BStr([MarshalAs(UnmanagedType.BStr)] uint Param1);
    void TestMethod_System_UInt32_LPStr([MarshalAs(UnmanagedType.LPStr)] uint Param1);
    void TestMethod_System_UInt32_LPWStr([MarshalAs(UnmanagedType.LPWStr)] uint Param1);
    void TestMethod_System_UInt32_LPTStr([MarshalAs(UnmanagedType.LPTStr)] uint Param1);
    void TestMethod_System_UInt32_IUnknown([MarshalAs(UnmanagedType.IUnknown)] uint Param1);
    void TestMethod_System_UInt32_IDispatch([MarshalAs(UnmanagedType.IDispatch)] uint Param1);
    void TestMethod_System_UInt32_Struct([MarshalAs(UnmanagedType.Struct)] uint Param1);
    void TestMethod_System_UInt32_Interface([MarshalAs(UnmanagedType.Interface)] uint Param1);
    void TestMethod_System_UInt32_SafeArray([MarshalAs(UnmanagedType.SafeArray)] uint Param1);
    void TestMethod_System_UInt32_SysInt([MarshalAs(UnmanagedType.SysInt)] uint Param1);
    void TestMethod_System_UInt32_SysUInt([MarshalAs(UnmanagedType.SysUInt)] uint Param1);
    void TestMethod_System_UInt32_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] uint Param1);
    void TestMethod_System_UInt32_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] uint Param1);
    void TestMethod_System_UInt32_TBStr([MarshalAs(UnmanagedType.TBStr)] uint Param1);
    void TestMethod_System_UInt32_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] uint Param1);
    void TestMethod_System_UInt32_AsAny([MarshalAs(UnmanagedType.AsAny)] uint Param1);
    void TestMethod_System_UInt32_LPArray([MarshalAs(UnmanagedType.LPArray)] uint Param1);
    void TestMethod_System_UInt32_LPStruct([MarshalAs(UnmanagedType.LPStruct)] uint Param1);
    void TestMethod_System_UInt32_Error([MarshalAs(UnmanagedType.Error)] uint Param1);
    void TestMethod_System_UInt32_IInspectable([MarshalAs(UnmanagedType.IInspectable)] uint Param1);
    void TestMethod_System_UInt32_HString([MarshalAs(UnmanagedType.HString)] uint Param1);
    void TestMethod_System_UInt32_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] uint Param1);
    #endregion

    #region int
    void TestMethod_System_Int32_Bool([MarshalAs(UnmanagedType.Bool)] int Param1);
    void TestMethod_System_Int32_VariantBool([MarshalAs(UnmanagedType.VariantBool)] int Param1);
    void TestMethod_System_Int32_I1([MarshalAs(UnmanagedType.I1)] int Param1);
    void TestMethod_System_Int32_U1([MarshalAs(UnmanagedType.U1)] int Param1);
    void TestMethod_System_Int32_I2([MarshalAs(UnmanagedType.I2)] int Param1);
    void TestMethod_System_Int32_U2([MarshalAs(UnmanagedType.U2)] int Param1);
    void TestMethod_System_Int32_I4([MarshalAs(UnmanagedType.I4)] int Param1);
    void TestMethod_System_Int32_U4([MarshalAs(UnmanagedType.U4)] int Param1);
    void TestMethod_System_Int32_I8([MarshalAs(UnmanagedType.I8)] int Param1);
    void TestMethod_System_Int32_U8([MarshalAs(UnmanagedType.U8)] int Param1);
    void TestMethod_System_Int32_R4([MarshalAs(UnmanagedType.R4)] int Param1);
    void TestMethod_System_Int32_R8([MarshalAs(UnmanagedType.R8)] int Param1);
    void TestMethod_System_Int32_Currency([MarshalAs(UnmanagedType.Currency)] int Param1);
    void TestMethod_System_Int32_BStr([MarshalAs(UnmanagedType.BStr)] int Param1);
    void TestMethod_System_Int32_LPStr([MarshalAs(UnmanagedType.LPStr)] int Param1);
    void TestMethod_System_Int32_LPWStr([MarshalAs(UnmanagedType.LPWStr)] int Param1);
    void TestMethod_System_Int32_LPTStr([MarshalAs(UnmanagedType.LPTStr)] int Param1);
    void TestMethod_System_Int32_IUnknown([MarshalAs(UnmanagedType.IUnknown)] int Param1);
    void TestMethod_System_Int32_IDispatch([MarshalAs(UnmanagedType.IDispatch)] int Param1);
    void TestMethod_System_Int32_Struct([MarshalAs(UnmanagedType.Struct)] int Param1);
    void TestMethod_System_Int32_Interface([MarshalAs(UnmanagedType.Interface)] int Param1);
    void TestMethod_System_Int32_SafeArray([MarshalAs(UnmanagedType.SafeArray)] int Param1);
    void TestMethod_System_Int32_SysInt([MarshalAs(UnmanagedType.SysInt)] int Param1);
    void TestMethod_System_Int32_SysUInt([MarshalAs(UnmanagedType.SysUInt)] int Param1);
    void TestMethod_System_Int32_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] int Param1);
    void TestMethod_System_Int32_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] int Param1);
    void TestMethod_System_Int32_TBStr([MarshalAs(UnmanagedType.TBStr)] int Param1);
    void TestMethod_System_Int32_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] int Param1);
    void TestMethod_System_Int32_AsAny([MarshalAs(UnmanagedType.AsAny)] int Param1);
    void TestMethod_System_Int32_LPArray([MarshalAs(UnmanagedType.LPArray)] int Param1);
    void TestMethod_System_Int32_LPStruct([MarshalAs(UnmanagedType.LPStruct)] int Param1);
    void TestMethod_System_Int32_Error([MarshalAs(UnmanagedType.Error)] int Param1);
    void TestMethod_System_Int32_IInspectable([MarshalAs(UnmanagedType.IInspectable)] int Param1);
    void TestMethod_System_Int32_HString([MarshalAs(UnmanagedType.HString)] int Param1);
    void TestMethod_System_Int32_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] int Param1);
    #endregion

    #region ulong
    void TestMethod_System_UInt64_Bool([MarshalAs(UnmanagedType.Bool)] ulong Param1);
    void TestMethod_System_UInt64_VariantBool([MarshalAs(UnmanagedType.VariantBool)] ulong Param1);
    void TestMethod_System_UInt64_I1([MarshalAs(UnmanagedType.I1)] ulong Param1);
    void TestMethod_System_UInt64_U1([MarshalAs(UnmanagedType.U1)] ulong Param1);
    void TestMethod_System_UInt64_I2([MarshalAs(UnmanagedType.I2)] ulong Param1);
    void TestMethod_System_UInt64_U2([MarshalAs(UnmanagedType.U2)] ulong Param1);
    void TestMethod_System_UInt64_I4([MarshalAs(UnmanagedType.I4)] ulong Param1);
    void TestMethod_System_UInt64_U4([MarshalAs(UnmanagedType.U4)] ulong Param1);
    void TestMethod_System_UInt64_I8([MarshalAs(UnmanagedType.I8)] ulong Param1);
    void TestMethod_System_UInt64_U8([MarshalAs(UnmanagedType.U8)] ulong Param1);
    void TestMethod_System_UInt64_R4([MarshalAs(UnmanagedType.R4)] ulong Param1);
    void TestMethod_System_UInt64_R8([MarshalAs(UnmanagedType.R8)] ulong Param1);
    void TestMethod_System_UInt64_Currency([MarshalAs(UnmanagedType.Currency)] ulong Param1);
    void TestMethod_System_UInt64_BStr([MarshalAs(UnmanagedType.BStr)] ulong Param1);
    void TestMethod_System_UInt64_LPStr([MarshalAs(UnmanagedType.LPStr)] ulong Param1);
    void TestMethod_System_UInt64_LPWStr([MarshalAs(UnmanagedType.LPWStr)] ulong Param1);
    void TestMethod_System_UInt64_LPTStr([MarshalAs(UnmanagedType.LPTStr)] ulong Param1);
    void TestMethod_System_UInt64_IUnknown([MarshalAs(UnmanagedType.IUnknown)] ulong Param1);
    void TestMethod_System_UInt64_IDispatch([MarshalAs(UnmanagedType.IDispatch)] ulong Param1);
    void TestMethod_System_UInt64_Struct([MarshalAs(UnmanagedType.Struct)] ulong Param1);
    void TestMethod_System_UInt64_Interface([MarshalAs(UnmanagedType.Interface)] ulong Param1);
    void TestMethod_System_UInt64_SafeArray([MarshalAs(UnmanagedType.SafeArray)] ulong Param1);
    void TestMethod_System_UInt64_SysInt([MarshalAs(UnmanagedType.SysInt)] ulong Param1);
    void TestMethod_System_UInt64_SysUInt([MarshalAs(UnmanagedType.SysUInt)] ulong Param1);
    void TestMethod_System_UInt64_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] ulong Param1);
    void TestMethod_System_UInt64_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] ulong Param1);
    void TestMethod_System_UInt64_TBStr([MarshalAs(UnmanagedType.TBStr)] ulong Param1);
    void TestMethod_System_UInt64_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] ulong Param1);
    void TestMethod_System_UInt64_AsAny([MarshalAs(UnmanagedType.AsAny)] ulong Param1);
    void TestMethod_System_UInt64_LPArray([MarshalAs(UnmanagedType.LPArray)] ulong Param1);
    void TestMethod_System_UInt64_LPStruct([MarshalAs(UnmanagedType.LPStruct)] ulong Param1);
    void TestMethod_System_UInt64_Error([MarshalAs(UnmanagedType.Error)] ulong Param1);
    void TestMethod_System_UInt64_IInspectable([MarshalAs(UnmanagedType.IInspectable)] ulong Param1);
    void TestMethod_System_UInt64_HString([MarshalAs(UnmanagedType.HString)] ulong Param1);
    void TestMethod_System_UInt64_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] ulong Param1);
    #endregion

    #region long
    void TestMethod_System_Int64_Bool([MarshalAs(UnmanagedType.Bool)] long Param1);
    void TestMethod_System_Int64_VariantBool([MarshalAs(UnmanagedType.VariantBool)] long Param1);
    void TestMethod_System_Int64_I1([MarshalAs(UnmanagedType.I1)] long Param1);
    void TestMethod_System_Int64_U1([MarshalAs(UnmanagedType.U1)] long Param1);
    void TestMethod_System_Int64_I2([MarshalAs(UnmanagedType.I2)] long Param1);
    void TestMethod_System_Int64_U2([MarshalAs(UnmanagedType.U2)] long Param1);
    void TestMethod_System_Int64_I4([MarshalAs(UnmanagedType.I4)] long Param1);
    void TestMethod_System_Int64_U4([MarshalAs(UnmanagedType.U4)] long Param1);
    void TestMethod_System_Int64_I8([MarshalAs(UnmanagedType.I8)] long Param1);
    void TestMethod_System_Int64_U8([MarshalAs(UnmanagedType.U8)] long Param1);
    void TestMethod_System_Int64_R4([MarshalAs(UnmanagedType.R4)] long Param1);
    void TestMethod_System_Int64_R8([MarshalAs(UnmanagedType.R8)] long Param1);
    void TestMethod_System_Int64_Currency([MarshalAs(UnmanagedType.Currency)] long Param1);
    void TestMethod_System_Int64_BStr([MarshalAs(UnmanagedType.BStr)] long Param1);
    void TestMethod_System_Int64_LPStr([MarshalAs(UnmanagedType.LPStr)] long Param1);
    void TestMethod_System_Int64_LPWStr([MarshalAs(UnmanagedType.LPWStr)] long Param1);
    void TestMethod_System_Int64_LPTStr([MarshalAs(UnmanagedType.LPTStr)] long Param1);
    void TestMethod_System_Int64_IUnknown([MarshalAs(UnmanagedType.IUnknown)] long Param1);
    void TestMethod_System_Int64_IDispatch([MarshalAs(UnmanagedType.IDispatch)] long Param1);
    void TestMethod_System_Int64_Struct([MarshalAs(UnmanagedType.Struct)] long Param1);
    void TestMethod_System_Int64_Interface([MarshalAs(UnmanagedType.Interface)] long Param1);
    void TestMethod_System_Int64_SafeArray([MarshalAs(UnmanagedType.SafeArray)] long Param1);
    void TestMethod_System_Int64_SysInt([MarshalAs(UnmanagedType.SysInt)] long Param1);
    void TestMethod_System_Int64_SysUInt([MarshalAs(UnmanagedType.SysUInt)] long Param1);
    void TestMethod_System_Int64_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] long Param1);
    void TestMethod_System_Int64_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] long Param1);
    void TestMethod_System_Int64_TBStr([MarshalAs(UnmanagedType.TBStr)] long Param1);
    void TestMethod_System_Int64_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] long Param1);
    void TestMethod_System_Int64_AsAny([MarshalAs(UnmanagedType.AsAny)] long Param1);
    void TestMethod_System_Int64_LPArray([MarshalAs(UnmanagedType.LPArray)] long Param1);
    void TestMethod_System_Int64_LPStruct([MarshalAs(UnmanagedType.LPStruct)] long Param1);
    void TestMethod_System_Int64_Error([MarshalAs(UnmanagedType.Error)] long Param1);
    void TestMethod_System_Int64_IInspectable([MarshalAs(UnmanagedType.IInspectable)] long Param1);
    void TestMethod_System_Int64_HString([MarshalAs(UnmanagedType.HString)] long Param1);
    void TestMethod_System_Int64_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] long Param1);
    #endregion

    #region float
    void TestMethod_System_Single_Bool([MarshalAs(UnmanagedType.Bool)] float Param1);
    void TestMethod_System_Single_VariantBool([MarshalAs(UnmanagedType.VariantBool)] float Param1);
    void TestMethod_System_Single_I1([MarshalAs(UnmanagedType.I1)] float Param1);
    void TestMethod_System_Single_U1([MarshalAs(UnmanagedType.U1)] float Param1);
    void TestMethod_System_Single_I2([MarshalAs(UnmanagedType.I2)] float Param1);
    void TestMethod_System_Single_U2([MarshalAs(UnmanagedType.U2)] float Param1);
    void TestMethod_System_Single_I4([MarshalAs(UnmanagedType.I4)] float Param1);
    void TestMethod_System_Single_U4([MarshalAs(UnmanagedType.U4)] float Param1);
    void TestMethod_System_Single_I8([MarshalAs(UnmanagedType.I8)] float Param1);
    void TestMethod_System_Single_U8([MarshalAs(UnmanagedType.U8)] float Param1);
    void TestMethod_System_Single_R4([MarshalAs(UnmanagedType.R4)] float Param1);
    void TestMethod_System_Single_R8([MarshalAs(UnmanagedType.R8)] float Param1);
    void TestMethod_System_Single_Currency([MarshalAs(UnmanagedType.Currency)] float Param1);
    void TestMethod_System_Single_BStr([MarshalAs(UnmanagedType.BStr)] float Param1);
    void TestMethod_System_Single_LPStr([MarshalAs(UnmanagedType.LPStr)] float Param1);
    void TestMethod_System_Single_LPWStr([MarshalAs(UnmanagedType.LPWStr)] float Param1);
    void TestMethod_System_Single_LPTStr([MarshalAs(UnmanagedType.LPTStr)] float Param1);
    void TestMethod_System_Single_IUnknown([MarshalAs(UnmanagedType.IUnknown)] float Param1);
    void TestMethod_System_Single_IDispatch([MarshalAs(UnmanagedType.IDispatch)] float Param1);
    void TestMethod_System_Single_Struct([MarshalAs(UnmanagedType.Struct)] float Param1);
    void TestMethod_System_Single_Interface([MarshalAs(UnmanagedType.Interface)] float Param1);
    void TestMethod_System_Single_SafeArray([MarshalAs(UnmanagedType.SafeArray)] float Param1);
    void TestMethod_System_Single_SysInt([MarshalAs(UnmanagedType.SysInt)] float Param1);
    void TestMethod_System_Single_SysUInt([MarshalAs(UnmanagedType.SysUInt)] float Param1);
    void TestMethod_System_Single_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] float Param1);
    void TestMethod_System_Single_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] float Param1);
    void TestMethod_System_Single_TBStr([MarshalAs(UnmanagedType.TBStr)] float Param1);
    void TestMethod_System_Single_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] float Param1);
    void TestMethod_System_Single_AsAny([MarshalAs(UnmanagedType.AsAny)] float Param1);
    void TestMethod_System_Single_LPArray([MarshalAs(UnmanagedType.LPArray)] float Param1);
    void TestMethod_System_Single_LPStruct([MarshalAs(UnmanagedType.LPStruct)] float Param1);
    void TestMethod_System_Single_Error([MarshalAs(UnmanagedType.Error)] float Param1);
    void TestMethod_System_Single_IInspectable([MarshalAs(UnmanagedType.IInspectable)] float Param1);
    void TestMethod_System_Single_HString([MarshalAs(UnmanagedType.HString)] float Param1);
    void TestMethod_System_Single_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] float Param1);
    #endregion

    #region double
    void TestMethod_System_Double_Bool([MarshalAs(UnmanagedType.Bool)] double Param1);
    void TestMethod_System_Double_VariantBool([MarshalAs(UnmanagedType.VariantBool)] double Param1);
    void TestMethod_System_Double_I1([MarshalAs(UnmanagedType.I1)] double Param1);
    void TestMethod_System_Double_U1([MarshalAs(UnmanagedType.U1)] double Param1);
    void TestMethod_System_Double_I2([MarshalAs(UnmanagedType.I2)] double Param1);
    void TestMethod_System_Double_U2([MarshalAs(UnmanagedType.U2)] double Param1);
    void TestMethod_System_Double_I4([MarshalAs(UnmanagedType.I4)] double Param1);
    void TestMethod_System_Double_U4([MarshalAs(UnmanagedType.U4)] double Param1);
    void TestMethod_System_Double_I8([MarshalAs(UnmanagedType.I8)] double Param1);
    void TestMethod_System_Double_U8([MarshalAs(UnmanagedType.U8)] double Param1);
    void TestMethod_System_Double_R4([MarshalAs(UnmanagedType.R4)] double Param1);
    void TestMethod_System_Double_R8([MarshalAs(UnmanagedType.R8)] double Param1);
    void TestMethod_System_Double_Currency([MarshalAs(UnmanagedType.Currency)] double Param1);
    void TestMethod_System_Double_BStr([MarshalAs(UnmanagedType.BStr)] double Param1);
    void TestMethod_System_Double_LPStr([MarshalAs(UnmanagedType.LPStr)] double Param1);
    void TestMethod_System_Double_LPWStr([MarshalAs(UnmanagedType.LPWStr)] double Param1);
    void TestMethod_System_Double_LPTStr([MarshalAs(UnmanagedType.LPTStr)] double Param1);
    void TestMethod_System_Double_IUnknown([MarshalAs(UnmanagedType.IUnknown)] double Param1);
    void TestMethod_System_Double_IDispatch([MarshalAs(UnmanagedType.IDispatch)] double Param1);
    void TestMethod_System_Double_Struct([MarshalAs(UnmanagedType.Struct)] double Param1);
    void TestMethod_System_Double_Interface([MarshalAs(UnmanagedType.Interface)] double Param1);
    void TestMethod_System_Double_SafeArray([MarshalAs(UnmanagedType.SafeArray)] double Param1);
    void TestMethod_System_Double_SysInt([MarshalAs(UnmanagedType.SysInt)] double Param1);
    void TestMethod_System_Double_SysUInt([MarshalAs(UnmanagedType.SysUInt)] double Param1);
    void TestMethod_System_Double_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] double Param1);
    void TestMethod_System_Double_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] double Param1);
    void TestMethod_System_Double_TBStr([MarshalAs(UnmanagedType.TBStr)] double Param1);
    void TestMethod_System_Double_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] double Param1);
    void TestMethod_System_Double_AsAny([MarshalAs(UnmanagedType.AsAny)] double Param1);
    void TestMethod_System_Double_LPArray([MarshalAs(UnmanagedType.LPArray)] double Param1);
    void TestMethod_System_Double_LPStruct([MarshalAs(UnmanagedType.LPStruct)] double Param1);
    void TestMethod_System_Double_Error([MarshalAs(UnmanagedType.Error)] double Param1);
    void TestMethod_System_Double_IInspectable([MarshalAs(UnmanagedType.IInspectable)] double Param1);
    void TestMethod_System_Double_HString([MarshalAs(UnmanagedType.HString)] double Param1);
    void TestMethod_System_Double_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] double Param1);
    #endregion

    #region string
    void TestMethod_System_String_Bool([MarshalAs(UnmanagedType.Bool)] string Param1);
    void TestMethod_System_String_VariantBool([MarshalAs(UnmanagedType.VariantBool)] string Param1);
    void TestMethod_System_String_I1([MarshalAs(UnmanagedType.I1)] string Param1);
    void TestMethod_System_String_U1([MarshalAs(UnmanagedType.U1)] string Param1);
    void TestMethod_System_String_I2([MarshalAs(UnmanagedType.I2)] string Param1);
    void TestMethod_System_String_U2([MarshalAs(UnmanagedType.U2)] string Param1);
    void TestMethod_System_String_I4([MarshalAs(UnmanagedType.I4)] string Param1);
    void TestMethod_System_String_U4([MarshalAs(UnmanagedType.U4)] string Param1);
    void TestMethod_System_String_I8([MarshalAs(UnmanagedType.I8)] string Param1);
    void TestMethod_System_String_U8([MarshalAs(UnmanagedType.U8)] string Param1);
    void TestMethod_System_String_R4([MarshalAs(UnmanagedType.R4)] string Param1);
    void TestMethod_System_String_R8([MarshalAs(UnmanagedType.R8)] string Param1);
    void TestMethod_System_String_Currency([MarshalAs(UnmanagedType.Currency)] string Param1);
    void TestMethod_System_String_BStr([MarshalAs(UnmanagedType.BStr)] string Param1);
    void TestMethod_System_String_LPStr([MarshalAs(UnmanagedType.LPStr)] string Param1);
    void TestMethod_System_String_LPWStr([MarshalAs(UnmanagedType.LPWStr)] string Param1);
    void TestMethod_System_String_LPTStr([MarshalAs(UnmanagedType.LPTStr)] string Param1);
    void TestMethod_System_String_IUnknown([MarshalAs(UnmanagedType.IUnknown)] string Param1);
    void TestMethod_System_String_IDispatch([MarshalAs(UnmanagedType.IDispatch)] string Param1);
    void TestMethod_System_String_Struct([MarshalAs(UnmanagedType.Struct)] string Param1);
    void TestMethod_System_String_Interface([MarshalAs(UnmanagedType.Interface)] string Param1);
    void TestMethod_System_String_SafeArray([MarshalAs(UnmanagedType.SafeArray)] string Param1);
    void TestMethod_System_String_SysInt([MarshalAs(UnmanagedType.SysInt)] string Param1);
    void TestMethod_System_String_SysUInt([MarshalAs(UnmanagedType.SysUInt)] string Param1);
    void TestMethod_System_String_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] string Param1);
    void TestMethod_System_String_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] string Param1);
    void TestMethod_System_String_TBStr([MarshalAs(UnmanagedType.TBStr)] string Param1);
    void TestMethod_System_String_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] string Param1);
    void TestMethod_System_String_AsAny([MarshalAs(UnmanagedType.AsAny)] string Param1);
    void TestMethod_System_String_LPArray([MarshalAs(UnmanagedType.LPArray)] string Param1);
    void TestMethod_System_String_LPStruct([MarshalAs(UnmanagedType.LPStruct)] string Param1);
    void TestMethod_System_String_Error([MarshalAs(UnmanagedType.Error)] string Param1);
    void TestMethod_System_String_IInspectable([MarshalAs(UnmanagedType.IInspectable)] string Param1);
    void TestMethod_System_String_HString([MarshalAs(UnmanagedType.HString)] string Param1);
    void TestMethod_System_String_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] string Param1);
    #endregion

    #region char
    void TestMethod_System_Char_Bool([MarshalAs(UnmanagedType.Bool)] char Param1);
    void TestMethod_System_Char_VariantBool([MarshalAs(UnmanagedType.VariantBool)] char Param1);
    void TestMethod_System_Char_I1([MarshalAs(UnmanagedType.I1)] char Param1);
    void TestMethod_System_Char_U1([MarshalAs(UnmanagedType.U1)] char Param1);
    void TestMethod_System_Char_I2([MarshalAs(UnmanagedType.I2)] char Param1);
    void TestMethod_System_Char_U2([MarshalAs(UnmanagedType.U2)] char Param1);
    void TestMethod_System_Char_I4([MarshalAs(UnmanagedType.I4)] char Param1);
    void TestMethod_System_Char_U4([MarshalAs(UnmanagedType.U4)] char Param1);
    void TestMethod_System_Char_I8([MarshalAs(UnmanagedType.I8)] char Param1);
    void TestMethod_System_Char_U8([MarshalAs(UnmanagedType.U8)] char Param1);
    void TestMethod_System_Char_R4([MarshalAs(UnmanagedType.R4)] char Param1);
    void TestMethod_System_Char_R8([MarshalAs(UnmanagedType.R8)] char Param1);
    void TestMethod_System_Char_Currency([MarshalAs(UnmanagedType.Currency)] char Param1);
    void TestMethod_System_Char_BStr([MarshalAs(UnmanagedType.BStr)] char Param1);
    void TestMethod_System_Char_LPStr([MarshalAs(UnmanagedType.LPStr)] char Param1);
    void TestMethod_System_Char_LPWStr([MarshalAs(UnmanagedType.LPWStr)] char Param1);
    void TestMethod_System_Char_LPTStr([MarshalAs(UnmanagedType.LPTStr)] char Param1);
    void TestMethod_System_Char_IUnknown([MarshalAs(UnmanagedType.IUnknown)] char Param1);
    void TestMethod_System_Char_IDispatch([MarshalAs(UnmanagedType.IDispatch)] char Param1);
    void TestMethod_System_Char_Struct([MarshalAs(UnmanagedType.Struct)] char Param1);
    void TestMethod_System_Char_Interface([MarshalAs(UnmanagedType.Interface)] char Param1);
    void TestMethod_System_Char_SafeArray([MarshalAs(UnmanagedType.SafeArray)] char Param1);
    void TestMethod_System_Char_SysInt([MarshalAs(UnmanagedType.SysInt)] char Param1);
    void TestMethod_System_Char_SysUInt([MarshalAs(UnmanagedType.SysUInt)] char Param1);
    void TestMethod_System_Char_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] char Param1);
    void TestMethod_System_Char_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] char Param1);
    void TestMethod_System_Char_TBStr([MarshalAs(UnmanagedType.TBStr)] char Param1);
    void TestMethod_System_Char_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] char Param1);
    void TestMethod_System_Char_AsAny([MarshalAs(UnmanagedType.AsAny)] char Param1);
    void TestMethod_System_Char_LPArray([MarshalAs(UnmanagedType.LPArray)] char Param1);
    void TestMethod_System_Char_LPStruct([MarshalAs(UnmanagedType.LPStruct)] char Param1);
    void TestMethod_System_Char_Error([MarshalAs(UnmanagedType.Error)] char Param1);
    void TestMethod_System_Char_IInspectable([MarshalAs(UnmanagedType.IInspectable)] char Param1);
    void TestMethod_System_Char_HString([MarshalAs(UnmanagedType.HString)] char Param1);
    void TestMethod_System_Char_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] char Param1);
    #endregion

    #region object
    void TestMethod_System_Object_Bool([MarshalAs(UnmanagedType.Bool)] object Param1);
    void TestMethod_System_Object_VariantBool([MarshalAs(UnmanagedType.VariantBool)] object Param1);
    void TestMethod_System_Object_I1([MarshalAs(UnmanagedType.I1)] object Param1);
    void TestMethod_System_Object_U1([MarshalAs(UnmanagedType.U1)] object Param1);
    void TestMethod_System_Object_I2([MarshalAs(UnmanagedType.I2)] object Param1);
    void TestMethod_System_Object_U2([MarshalAs(UnmanagedType.U2)] object Param1);
    void TestMethod_System_Object_I4([MarshalAs(UnmanagedType.I4)] object Param1);
    void TestMethod_System_Object_U4([MarshalAs(UnmanagedType.U4)] object Param1);
    void TestMethod_System_Object_I8([MarshalAs(UnmanagedType.I8)] object Param1);
    void TestMethod_System_Object_U8([MarshalAs(UnmanagedType.U8)] object Param1);
    void TestMethod_System_Object_R4([MarshalAs(UnmanagedType.R4)] object Param1);
    void TestMethod_System_Object_R8([MarshalAs(UnmanagedType.R8)] object Param1);
    void TestMethod_System_Object_Currency([MarshalAs(UnmanagedType.Currency)] object Param1);
    void TestMethod_System_Object_BStr([MarshalAs(UnmanagedType.BStr)] object Param1);
    void TestMethod_System_Object_LPStr([MarshalAs(UnmanagedType.LPStr)] object Param1);
    void TestMethod_System_Object_LPWStr([MarshalAs(UnmanagedType.LPWStr)] object Param1);
    void TestMethod_System_Object_LPTStr([MarshalAs(UnmanagedType.LPTStr)] object Param1);
    void TestMethod_System_Object_IUnknown([MarshalAs(UnmanagedType.IUnknown)] object Param1);
    void TestMethod_System_Object_IDispatch([MarshalAs(UnmanagedType.IDispatch)] object Param1);
    void TestMethod_System_Object_Struct([MarshalAs(UnmanagedType.Struct)] object Param1);
    void TestMethod_System_Object_Interface([MarshalAs(UnmanagedType.Interface)] object Param1);
    void TestMethod_System_Object_SafeArray([MarshalAs(UnmanagedType.SafeArray)] object Param1);
    void TestMethod_System_Object_SysInt([MarshalAs(UnmanagedType.SysInt)] object Param1);
    void TestMethod_System_Object_SysUInt([MarshalAs(UnmanagedType.SysUInt)] object Param1);
    void TestMethod_System_Object_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] object Param1);
    void TestMethod_System_Object_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] object Param1);
    void TestMethod_System_Object_TBStr([MarshalAs(UnmanagedType.TBStr)] object Param1);
    void TestMethod_System_Object_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] object Param1);
    void TestMethod_System_Object_AsAny([MarshalAs(UnmanagedType.AsAny)] object Param1);
    void TestMethod_System_Object_LPArray([MarshalAs(UnmanagedType.LPArray)] object Param1);
    void TestMethod_System_Object_LPStruct([MarshalAs(UnmanagedType.LPStruct)] object Param1);
    void TestMethod_System_Object_Error([MarshalAs(UnmanagedType.Error)] object Param1);
    void TestMethod_System_Object_IInspectable([MarshalAs(UnmanagedType.IInspectable)] object Param1);
    void TestMethod_System_Object_HString([MarshalAs(UnmanagedType.HString)] object Param1);
    void TestMethod_System_Object_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] object Param1);
    #endregion

    #region objectArray
    void TestMethod_System_Object___Bool([MarshalAs(UnmanagedType.Bool)] object[] Param1);
    void TestMethod_System_Object___VariantBool([MarshalAs(UnmanagedType.VariantBool)] object[] Param1);
    void TestMethod_System_Object___I1([MarshalAs(UnmanagedType.I1)] object[] Param1);
    void TestMethod_System_Object___U1([MarshalAs(UnmanagedType.U1)] object[] Param1);
    void TestMethod_System_Object___I2([MarshalAs(UnmanagedType.I2)] object[] Param1);
    void TestMethod_System_Object___U2([MarshalAs(UnmanagedType.U2)] object[] Param1);
    void TestMethod_System_Object___I4([MarshalAs(UnmanagedType.I4)] object[] Param1);
    void TestMethod_System_Object___U4([MarshalAs(UnmanagedType.U4)] object[] Param1);
    void TestMethod_System_Object___I8([MarshalAs(UnmanagedType.I8)] object[] Param1);
    void TestMethod_System_Object___U8([MarshalAs(UnmanagedType.U8)] object[] Param1);
    void TestMethod_System_Object___R4([MarshalAs(UnmanagedType.R4)] object[] Param1);
    void TestMethod_System_Object___R8([MarshalAs(UnmanagedType.R8)] object[] Param1);
    void TestMethod_System_Object___Currency([MarshalAs(UnmanagedType.Currency)] object[] Param1);
    void TestMethod_System_Object___BStr([MarshalAs(UnmanagedType.BStr)] object[] Param1);
    void TestMethod_System_Object___LPStr([MarshalAs(UnmanagedType.LPStr)] object[] Param1);
    void TestMethod_System_Object___LPWStr([MarshalAs(UnmanagedType.LPWStr)] object[] Param1);
    void TestMethod_System_Object___LPTStr([MarshalAs(UnmanagedType.LPTStr)] object[] Param1);
    void TestMethod_System_Object___IUnknown([MarshalAs(UnmanagedType.IUnknown)] object[] Param1);
    void TestMethod_System_Object___IDispatch([MarshalAs(UnmanagedType.IDispatch)] object[] Param1);
    void TestMethod_System_Object___Struct([MarshalAs(UnmanagedType.Struct)] object[] Param1);
    void TestMethod_System_Object___Interface([MarshalAs(UnmanagedType.Interface)] object[] Param1);
    void TestMethod_System_Object___SafeArray([MarshalAs(UnmanagedType.SafeArray)] object[] Param1);
    void TestMethod_System_Object___SysInt([MarshalAs(UnmanagedType.SysInt)] object[] Param1);
    void TestMethod_System_Object___SysUInt([MarshalAs(UnmanagedType.SysUInt)] object[] Param1);
    void TestMethod_System_Object___VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] object[] Param1);
    void TestMethod_System_Object___AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] object[] Param1);
    void TestMethod_System_Object___TBStr([MarshalAs(UnmanagedType.TBStr)] object[] Param1);
    void TestMethod_System_Object___FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] object[] Param1);
    void TestMethod_System_Object___AsAny([MarshalAs(UnmanagedType.AsAny)] object[] Param1);
    void TestMethod_System_Object___LPArray([MarshalAs(UnmanagedType.LPArray)] object[] Param1);
    void TestMethod_System_Object___LPStruct([MarshalAs(UnmanagedType.LPStruct)] object[] Param1);
    void TestMethod_System_Object___Error([MarshalAs(UnmanagedType.Error)] object[] Param1);
    void TestMethod_System_Object___IInspectable([MarshalAs(UnmanagedType.IInspectable)] object[] Param1);
    void TestMethod_System_Object___HString([MarshalAs(UnmanagedType.HString)] object[] Param1);
    void TestMethod_System_Object___LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] object[] Param1);
    #endregion

    #region IEnumerator
    void TestMethod_System_Collections_IEnumerator_Bool([MarshalAs(UnmanagedType.Bool)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_VariantBool([MarshalAs(UnmanagedType.VariantBool)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_I1([MarshalAs(UnmanagedType.I1)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_U1([MarshalAs(UnmanagedType.U1)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_I2([MarshalAs(UnmanagedType.I2)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_U2([MarshalAs(UnmanagedType.U2)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_I4([MarshalAs(UnmanagedType.I4)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_U4([MarshalAs(UnmanagedType.U4)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_I8([MarshalAs(UnmanagedType.I8)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_U8([MarshalAs(UnmanagedType.U8)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_R4([MarshalAs(UnmanagedType.R4)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_R8([MarshalAs(UnmanagedType.R8)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_Currency([MarshalAs(UnmanagedType.Currency)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_BStr([MarshalAs(UnmanagedType.BStr)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_LPStr([MarshalAs(UnmanagedType.LPStr)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_LPWStr([MarshalAs(UnmanagedType.LPWStr)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_LPTStr([MarshalAs(UnmanagedType.LPTStr)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_IUnknown([MarshalAs(UnmanagedType.IUnknown)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_IDispatch([MarshalAs(UnmanagedType.IDispatch)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_Struct([MarshalAs(UnmanagedType.Struct)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_Interface([MarshalAs(UnmanagedType.Interface)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_SafeArray([MarshalAs(UnmanagedType.SafeArray)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_SysInt([MarshalAs(UnmanagedType.SysInt)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_SysUInt([MarshalAs(UnmanagedType.SysUInt)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_TBStr([MarshalAs(UnmanagedType.TBStr)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_AsAny([MarshalAs(UnmanagedType.AsAny)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_LPArray([MarshalAs(UnmanagedType.LPArray)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_LPStruct([MarshalAs(UnmanagedType.LPStruct)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_Error([MarshalAs(UnmanagedType.Error)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_IInspectable([MarshalAs(UnmanagedType.IInspectable)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_HString([MarshalAs(UnmanagedType.HString)] IEnumerator Param1);
    void TestMethod_System_Collections_IEnumerator_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] IEnumerator Param1);
    #endregion

    #region DateTime
    void TestMethod_System_DateTime_Bool([MarshalAs(UnmanagedType.Bool)] DateTime Param1);
    void TestMethod_System_DateTime_VariantBool([MarshalAs(UnmanagedType.VariantBool)] DateTime Param1);
    void TestMethod_System_DateTime_I1([MarshalAs(UnmanagedType.I1)] DateTime Param1);
    void TestMethod_System_DateTime_U1([MarshalAs(UnmanagedType.U1)] DateTime Param1);
    void TestMethod_System_DateTime_I2([MarshalAs(UnmanagedType.I2)] DateTime Param1);
    void TestMethod_System_DateTime_U2([MarshalAs(UnmanagedType.U2)] DateTime Param1);
    void TestMethod_System_DateTime_I4([MarshalAs(UnmanagedType.I4)] DateTime Param1);
    void TestMethod_System_DateTime_U4([MarshalAs(UnmanagedType.U4)] DateTime Param1);
    void TestMethod_System_DateTime_I8([MarshalAs(UnmanagedType.I8)] DateTime Param1);
    void TestMethod_System_DateTime_U8([MarshalAs(UnmanagedType.U8)] DateTime Param1);
    void TestMethod_System_DateTime_R4([MarshalAs(UnmanagedType.R4)] DateTime Param1);
    void TestMethod_System_DateTime_R8([MarshalAs(UnmanagedType.R8)] DateTime Param1);
    void TestMethod_System_DateTime_Currency([MarshalAs(UnmanagedType.Currency)] DateTime Param1);
    void TestMethod_System_DateTime_BStr([MarshalAs(UnmanagedType.BStr)] DateTime Param1);
    void TestMethod_System_DateTime_LPStr([MarshalAs(UnmanagedType.LPStr)] DateTime Param1);
    void TestMethod_System_DateTime_LPWStr([MarshalAs(UnmanagedType.LPWStr)] DateTime Param1);
    void TestMethod_System_DateTime_LPTStr([MarshalAs(UnmanagedType.LPTStr)] DateTime Param1);
    void TestMethod_System_DateTime_IUnknown([MarshalAs(UnmanagedType.IUnknown)] DateTime Param1);
    void TestMethod_System_DateTime_IDispatch([MarshalAs(UnmanagedType.IDispatch)] DateTime Param1);
    void TestMethod_System_DateTime_Struct([MarshalAs(UnmanagedType.Struct)] DateTime Param1);
    void TestMethod_System_DateTime_Interface([MarshalAs(UnmanagedType.Interface)] DateTime Param1);
    void TestMethod_System_DateTime_SafeArray([MarshalAs(UnmanagedType.SafeArray)] DateTime Param1);
    void TestMethod_System_DateTime_SysInt([MarshalAs(UnmanagedType.SysInt)] DateTime Param1);
    void TestMethod_System_DateTime_SysUInt([MarshalAs(UnmanagedType.SysUInt)] DateTime Param1);
    void TestMethod_System_DateTime_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] DateTime Param1);
    void TestMethod_System_DateTime_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] DateTime Param1);
    void TestMethod_System_DateTime_TBStr([MarshalAs(UnmanagedType.TBStr)] DateTime Param1);
    void TestMethod_System_DateTime_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] DateTime Param1);
    void TestMethod_System_DateTime_AsAny([MarshalAs(UnmanagedType.AsAny)] DateTime Param1);
    void TestMethod_System_DateTime_LPArray([MarshalAs(UnmanagedType.LPArray)] DateTime Param1);
    void TestMethod_System_DateTime_LPStruct([MarshalAs(UnmanagedType.LPStruct)] DateTime Param1);
    void TestMethod_System_DateTime_Error([MarshalAs(UnmanagedType.Error)] DateTime Param1);
    void TestMethod_System_DateTime_IInspectable([MarshalAs(UnmanagedType.IInspectable)] DateTime Param1);
    void TestMethod_System_DateTime_HString([MarshalAs(UnmanagedType.HString)] DateTime Param1);
    void TestMethod_System_DateTime_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] DateTime Param1);
    #endregion

    #region Guid
    void TestMethod_System_Guid_Bool([MarshalAs(UnmanagedType.Bool)] Guid Param1);
    void TestMethod_System_Guid_VariantBool([MarshalAs(UnmanagedType.VariantBool)] Guid Param1);
    void TestMethod_System_Guid_I1([MarshalAs(UnmanagedType.I1)] Guid Param1);
    void TestMethod_System_Guid_U1([MarshalAs(UnmanagedType.U1)] Guid Param1);
    void TestMethod_System_Guid_I2([MarshalAs(UnmanagedType.I2)] Guid Param1);
    void TestMethod_System_Guid_U2([MarshalAs(UnmanagedType.U2)] Guid Param1);
    void TestMethod_System_Guid_I4([MarshalAs(UnmanagedType.I4)] Guid Param1);
    void TestMethod_System_Guid_U4([MarshalAs(UnmanagedType.U4)] Guid Param1);
    void TestMethod_System_Guid_I8([MarshalAs(UnmanagedType.I8)] Guid Param1);
    void TestMethod_System_Guid_U8([MarshalAs(UnmanagedType.U8)] Guid Param1);
    void TestMethod_System_Guid_R4([MarshalAs(UnmanagedType.R4)] Guid Param1);
    void TestMethod_System_Guid_R8([MarshalAs(UnmanagedType.R8)] Guid Param1);
    void TestMethod_System_Guid_Currency([MarshalAs(UnmanagedType.Currency)] Guid Param1);
    void TestMethod_System_Guid_BStr([MarshalAs(UnmanagedType.BStr)] Guid Param1);
    void TestMethod_System_Guid_LPStr([MarshalAs(UnmanagedType.LPStr)] Guid Param1);
    void TestMethod_System_Guid_LPWStr([MarshalAs(UnmanagedType.LPWStr)] Guid Param1);
    void TestMethod_System_Guid_LPTStr([MarshalAs(UnmanagedType.LPTStr)] Guid Param1);
    void TestMethod_System_Guid_IUnknown([MarshalAs(UnmanagedType.IUnknown)] Guid Param1);
    void TestMethod_System_Guid_IDispatch([MarshalAs(UnmanagedType.IDispatch)] Guid Param1);
    void TestMethod_System_Guid_Struct([MarshalAs(UnmanagedType.Struct)] Guid Param1);
    void TestMethod_System_Guid_Interface([MarshalAs(UnmanagedType.Interface)] Guid Param1);
    void TestMethod_System_Guid_SafeArray([MarshalAs(UnmanagedType.SafeArray)] Guid Param1);
    void TestMethod_System_Guid_SysInt([MarshalAs(UnmanagedType.SysInt)] Guid Param1);
    void TestMethod_System_Guid_SysUInt([MarshalAs(UnmanagedType.SysUInt)] Guid Param1);
    void TestMethod_System_Guid_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] Guid Param1);
    void TestMethod_System_Guid_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] Guid Param1);
    void TestMethod_System_Guid_TBStr([MarshalAs(UnmanagedType.TBStr)] Guid Param1);
    void TestMethod_System_Guid_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] Guid Param1);
    void TestMethod_System_Guid_AsAny([MarshalAs(UnmanagedType.AsAny)] Guid Param1);
    void TestMethod_System_Guid_LPArray([MarshalAs(UnmanagedType.LPArray)] Guid Param1);
    void TestMethod_System_Guid_LPStruct([MarshalAs(UnmanagedType.LPStruct)] Guid Param1);
    void TestMethod_System_Guid_Error([MarshalAs(UnmanagedType.Error)] Guid Param1);
    void TestMethod_System_Guid_IInspectable([MarshalAs(UnmanagedType.IInspectable)] Guid Param1);
    void TestMethod_System_Guid_HString([MarshalAs(UnmanagedType.HString)] Guid Param1);
    void TestMethod_System_Guid_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] Guid Param1);
    #endregion

    #region Color
    void TestMethod_System_Drawing_Color_Bool([MarshalAs(UnmanagedType.Bool)] Color Param1);
    void TestMethod_System_Drawing_Color_VariantBool([MarshalAs(UnmanagedType.VariantBool)] Color Param1);
    void TestMethod_System_Drawing_Color_I1([MarshalAs(UnmanagedType.I1)] Color Param1);
    void TestMethod_System_Drawing_Color_U1([MarshalAs(UnmanagedType.U1)] Color Param1);
    void TestMethod_System_Drawing_Color_I2([MarshalAs(UnmanagedType.I2)] Color Param1);
    void TestMethod_System_Drawing_Color_U2([MarshalAs(UnmanagedType.U2)] Color Param1);
    void TestMethod_System_Drawing_Color_I4([MarshalAs(UnmanagedType.I4)] Color Param1);
    void TestMethod_System_Drawing_Color_U4([MarshalAs(UnmanagedType.U4)] Color Param1);
    void TestMethod_System_Drawing_Color_I8([MarshalAs(UnmanagedType.I8)] Color Param1);
    void TestMethod_System_Drawing_Color_U8([MarshalAs(UnmanagedType.U8)] Color Param1);
    void TestMethod_System_Drawing_Color_R4([MarshalAs(UnmanagedType.R4)] Color Param1);
    void TestMethod_System_Drawing_Color_R8([MarshalAs(UnmanagedType.R8)] Color Param1);
    void TestMethod_System_Drawing_Color_Currency([MarshalAs(UnmanagedType.Currency)] Color Param1);
    void TestMethod_System_Drawing_Color_BStr([MarshalAs(UnmanagedType.BStr)] Color Param1);
    void TestMethod_System_Drawing_Color_LPStr([MarshalAs(UnmanagedType.LPStr)] Color Param1);
    void TestMethod_System_Drawing_Color_LPWStr([MarshalAs(UnmanagedType.LPWStr)] Color Param1);
    void TestMethod_System_Drawing_Color_LPTStr([MarshalAs(UnmanagedType.LPTStr)] Color Param1);
    void TestMethod_System_Drawing_Color_IUnknown([MarshalAs(UnmanagedType.IUnknown)] Color Param1);
    void TestMethod_System_Drawing_Color_IDispatch([MarshalAs(UnmanagedType.IDispatch)] Color Param1);
    void TestMethod_System_Drawing_Color_Struct([MarshalAs(UnmanagedType.Struct)] Color Param1);
    void TestMethod_System_Drawing_Color_Interface([MarshalAs(UnmanagedType.Interface)] Color Param1);
    void TestMethod_System_Drawing_Color_SafeArray([MarshalAs(UnmanagedType.SafeArray)] Color Param1);
    void TestMethod_System_Drawing_Color_SysInt([MarshalAs(UnmanagedType.SysInt)] Color Param1);
    void TestMethod_System_Drawing_Color_SysUInt([MarshalAs(UnmanagedType.SysUInt)] Color Param1);
    void TestMethod_System_Drawing_Color_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] Color Param1);
    void TestMethod_System_Drawing_Color_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] Color Param1);
    void TestMethod_System_Drawing_Color_TBStr([MarshalAs(UnmanagedType.TBStr)] Color Param1);
    void TestMethod_System_Drawing_Color_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] Color Param1);
    void TestMethod_System_Drawing_Color_AsAny([MarshalAs(UnmanagedType.AsAny)] Color Param1);
    void TestMethod_System_Drawing_Color_LPArray([MarshalAs(UnmanagedType.LPArray)] Color Param1);
    void TestMethod_System_Drawing_Color_LPStruct([MarshalAs(UnmanagedType.LPStruct)] Color Param1);
    void TestMethod_System_Drawing_Color_Error([MarshalAs(UnmanagedType.Error)] Color Param1);
    void TestMethod_System_Drawing_Color_IInspectable([MarshalAs(UnmanagedType.IInspectable)] Color Param1);
    void TestMethod_System_Drawing_Color_HString([MarshalAs(UnmanagedType.HString)] Color Param1);
    void TestMethod_System_Drawing_Color_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] Color Param1);
    #endregion

    #region decimal
    void TestMethod_System_Decimal_Bool([MarshalAs(UnmanagedType.Bool)] decimal Param1);
    void TestMethod_System_Decimal_VariantBool([MarshalAs(UnmanagedType.VariantBool)] decimal Param1);
    void TestMethod_System_Decimal_I1([MarshalAs(UnmanagedType.I1)] decimal Param1);
    void TestMethod_System_Decimal_U1([MarshalAs(UnmanagedType.U1)] decimal Param1);
    void TestMethod_System_Decimal_I2([MarshalAs(UnmanagedType.I2)] decimal Param1);
    void TestMethod_System_Decimal_U2([MarshalAs(UnmanagedType.U2)] decimal Param1);
    void TestMethod_System_Decimal_I4([MarshalAs(UnmanagedType.I4)] decimal Param1);
    void TestMethod_System_Decimal_U4([MarshalAs(UnmanagedType.U4)] decimal Param1);
    void TestMethod_System_Decimal_I8([MarshalAs(UnmanagedType.I8)] decimal Param1);
    void TestMethod_System_Decimal_U8([MarshalAs(UnmanagedType.U8)] decimal Param1);
    void TestMethod_System_Decimal_R4([MarshalAs(UnmanagedType.R4)] decimal Param1);
    void TestMethod_System_Decimal_R8([MarshalAs(UnmanagedType.R8)] decimal Param1);
    void TestMethod_System_Decimal_Currency([MarshalAs(UnmanagedType.Currency)] decimal Param1);
    void TestMethod_System_Decimal_BStr([MarshalAs(UnmanagedType.BStr)] decimal Param1);
    void TestMethod_System_Decimal_LPStr([MarshalAs(UnmanagedType.LPStr)] decimal Param1);
    void TestMethod_System_Decimal_LPWStr([MarshalAs(UnmanagedType.LPWStr)] decimal Param1);
    void TestMethod_System_Decimal_LPTStr([MarshalAs(UnmanagedType.LPTStr)] decimal Param1);
    void TestMethod_System_Decimal_IUnknown([MarshalAs(UnmanagedType.IUnknown)] decimal Param1);
    void TestMethod_System_Decimal_IDispatch([MarshalAs(UnmanagedType.IDispatch)] decimal Param1);
    void TestMethod_System_Decimal_Struct([MarshalAs(UnmanagedType.Struct)] decimal Param1);
    void TestMethod_System_Decimal_Interface([MarshalAs(UnmanagedType.Interface)] decimal Param1);
    void TestMethod_System_Decimal_SafeArray([MarshalAs(UnmanagedType.SafeArray)] decimal Param1);
    void TestMethod_System_Decimal_SysInt([MarshalAs(UnmanagedType.SysInt)] decimal Param1);
    void TestMethod_System_Decimal_SysUInt([MarshalAs(UnmanagedType.SysUInt)] decimal Param1);
    void TestMethod_System_Decimal_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] decimal Param1);
    void TestMethod_System_Decimal_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] decimal Param1);
    void TestMethod_System_Decimal_TBStr([MarshalAs(UnmanagedType.TBStr)] decimal Param1);
    void TestMethod_System_Decimal_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] decimal Param1);
    void TestMethod_System_Decimal_AsAny([MarshalAs(UnmanagedType.AsAny)] decimal Param1);
    void TestMethod_System_Decimal_LPArray([MarshalAs(UnmanagedType.LPArray)] decimal Param1);
    void TestMethod_System_Decimal_LPStruct([MarshalAs(UnmanagedType.LPStruct)] decimal Param1);
    void TestMethod_System_Decimal_Error([MarshalAs(UnmanagedType.Error)] decimal Param1);
    void TestMethod_System_Decimal_IInspectable([MarshalAs(UnmanagedType.IInspectable)] decimal Param1);
    void TestMethod_System_Decimal_HString([MarshalAs(UnmanagedType.HString)] decimal Param1);
    void TestMethod_System_Decimal_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] decimal Param1);
    #endregion

    #region Marshal the IList in return values 
    [return: MarshalAs(UnmanagedType.IUnknown)]
    IList TestMethod_ReturnValue_System_IList_IUnknown();

    #endregion

    #region IList parameter
    void TestMethod_System_IList_Bool([MarshalAs(UnmanagedType.Bool)] IList Param1);
    void TestMethod_System_IList_VariantBool([MarshalAs(UnmanagedType.VariantBool)] IList Param1);
    void TestMethod_System_IList_I1([MarshalAs(UnmanagedType.I1)] IList Param1);
    void TestMethod_System_IList_U1([MarshalAs(UnmanagedType.U1)] IList Param1);
    void TestMethod_System_IList_I2([MarshalAs(UnmanagedType.I2)] IList Param1);
    void TestMethod_System_IList_U2([MarshalAs(UnmanagedType.U2)] IList Param1);
    void TestMethod_System_IList_I4([MarshalAs(UnmanagedType.I4)] IList Param1);
    void TestMethod_System_IList_U4([MarshalAs(UnmanagedType.U4)] IList Param1);
    void TestMethod_System_IList_I8([MarshalAs(UnmanagedType.I8)] IList Param1);
    void TestMethod_System_IList_U8([MarshalAs(UnmanagedType.U8)] IList Param1);
    void TestMethod_System_IList_R4([MarshalAs(UnmanagedType.R4)] IList Param1);
    void TestMethod_System_IList_R8([MarshalAs(UnmanagedType.R8)] IList Param1);
    void TestMethod_System_IList_Currency([MarshalAs(UnmanagedType.Currency)] IList Param1);
    void TestMethod_System_IList_BStr([MarshalAs(UnmanagedType.BStr)] IList Param1);
    void TestMethod_System_IList_LPStr([MarshalAs(UnmanagedType.LPStr)] IList Param1);
    void TestMethod_System_IList_LPWStr([MarshalAs(UnmanagedType.LPWStr)] IList Param1);
    void TestMethod_System_IList_LPTStr([MarshalAs(UnmanagedType.LPTStr)] IList Param1);
    void TestMethod_System_IList_IUnknown([MarshalAs(UnmanagedType.IUnknown)] IList Param1);
    void TestMethod_System_IList_IDispatch([MarshalAs(UnmanagedType.IDispatch)] IList Param1);
    void TestMethod_System_IList_Struct([MarshalAs(UnmanagedType.Struct)] IList Param1);
    void TestMethod_System_IList_Interface([MarshalAs(UnmanagedType.Interface)] IList Param1);
    void TestMethod_System_IList_SafeArray([MarshalAs(UnmanagedType.SafeArray)] IList Param1);
    void TestMethod_System_IList_SysInt([MarshalAs(UnmanagedType.SysInt)] IList Param1);
    void TestMethod_System_IList_SysUInt([MarshalAs(UnmanagedType.SysUInt)] IList Param1);
    void TestMethod_System_IList_VBByRefStr([MarshalAs(UnmanagedType.VBByRefStr)] IList Param1);
    void TestMethod_System_IList_AnsiBStr([MarshalAs(UnmanagedType.AnsiBStr)] IList Param1);
    void TestMethod_System_IList_TBStr([MarshalAs(UnmanagedType.TBStr)] IList Param1);
    void TestMethod_System_IList_FunctionPtr([MarshalAs(UnmanagedType.FunctionPtr)] IList Param1);
    void TestMethod_System_IList_AsAny([MarshalAs(UnmanagedType.AsAny)] IList Param1);
    void TestMethod_System_IList_LPArray([MarshalAs(UnmanagedType.LPArray)] IList Param1);
    void TestMethod_System_IList_LPStruct([MarshalAs(UnmanagedType.LPStruct)] IList Param1);
    void TestMethod_System_IList_Error([MarshalAs(UnmanagedType.Error)] IList Param1);
    void TestMethod_System_IList_IInspectable([MarshalAs(UnmanagedType.IInspectable)] IList Param1);
    void TestMethod_System_IList_HString([MarshalAs(UnmanagedType.HString)] IList Param1);
    void TestMethod_System_IList_LPUTF8Str([MarshalAs(UnmanagedType.LPUTF8Str)] IList Param1);

    #endregion
}
