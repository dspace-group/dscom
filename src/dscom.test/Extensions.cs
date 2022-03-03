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

namespace dSPACE.Runtime.InteropServices.Tests;

public static class Extensions
{
    public static DisposabelStruct<TYPELIBATTR>? GetTypeLibAttributes(this ITypeLib2 typelib)
    {
        typelib.GetLibAttr(out var ppTLIBAttr);
        if (ppTLIBAttr != IntPtr.Zero)
        {
            return new DisposabelStruct<TYPELIBATTR>(Marshal.PtrToStructure<TYPELIBATTR>(ppTLIBAttr), () =>
            {
                typelib.ReleaseTLibAttr(ppTLIBAttr);
            });
        }
        return null;
    }

    public static DisposabelStruct<TYPEATTR>? GetTypeInfoAttributes(this ITypeInfo2 typeInfo)
    {
        typeInfo.GetTypeAttr(out var ppTypAttr);
        if (ppTypAttr != IntPtr.Zero)
        {
            return new DisposabelStruct<TYPEATTR>(Marshal.PtrToStructure<TYPEATTR>(ppTypAttr), () =>
            {
                typeInfo.ReleaseTypeAttr(ppTypAttr);
            });
        }
        return null;
    }

    /// <summary>
    /// Return the parameter at the specified index.
    /// </summary>
    /// <param name="funcDesc">The FUNCDESC.</param>
    /// <param name="index">The index.</param>
    /// <returns>A parameter at the specified index, or null.</returns>
    public static ELEMDESC? GetParameter(this FUNCDESC funcDesc, int index)
    {
        var ptr = funcDesc.lprgelemdescParam;
        var size = Marshal.SizeOf<ELEMDESC>();
        for (var i = 0; i < funcDesc.cParams; i++)
        {
            var parameter = Marshal.PtrToStructure<ELEMDESC>(ptr);
            if (index == i)
            {
                return parameter;
            }
            ptr += size;
        }

        return null;
    }

    /// <summary>
    /// Returns disposable FUNCDESC, if method is found. Otherwise null.
    /// </summary>
    /// <param name="typeInfo">A <paramref name="typeInfo"/></param>
    /// <param name="name">The method name</param>
    /// <returns></returns>
    public static DisposabelStruct<VARDESC>? GetVarDescByName(this ITypeInfo2 typeInfo, string name)
    {
        using var attributes = typeInfo.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();

        int numberOfVars = attributes!.Value.cVars;
        for (var i = 0; i < numberOfVars; i++)
        {
            typeInfo.GetVarDesc(i, out var ppVarDesc);

            var varDesc = Marshal.PtrToStructure<VARDESC>(ppVarDesc);

            typeInfo.GetDocumentation(varDesc.memid, out var varName, out var docString, out var helpContext, out var helpFile);

            if (string.Equals(varName, name, StringComparison.Ordinal))
            {
                return new DisposabelStruct<VARDESC>(varDesc, () =>
                {
                    typeInfo.ReleaseVarDesc(ppVarDesc);
                });
            }
            else
            {
                typeInfo.ReleaseVarDesc(ppVarDesc);
            }
        }

        return null;
    }

    /// <summary>
    /// Returns disposable FUNCDESC, if method is found. Otherwise null.
    /// </summary>
    /// <param name="typeInfo">A <paramref name="typeInfo"/></param>
    /// <param name="name">The method name</param>
    /// <returns></returns>
    public static DisposabelStruct<FUNCDESC>? GetFuncDescByName(this ITypeInfo2 typeInfo, string name, INVOKEKIND? invokeKind = null)
    {
        typeInfo.GetTypeAttr(out var ppTypAttr);

        using var attribute = typeInfo.GetTypeInfoAttributes();
        attribute.Should().NotBeNull();
        int numberOfFunction = attribute!.Value.cFuncs;
        for (var i = 0; i < numberOfFunction; i++)
        {
            typeInfo.GetFuncDesc(i, out var ppFuncDesc);

            var funcDesc = Marshal.PtrToStructure<FUNCDESC>(ppFuncDesc);

            typeInfo.GetDocumentation(funcDesc.memid, out var methodName, out var docString, out var helpContext, out var helpFile);

            if (string.Equals(methodName, name, StringComparison.Ordinal))
            {
                if (invokeKind.HasValue && !invokeKind.Value.HasFlag(funcDesc.invkind))
                {
                    continue;
                }

                return new DisposabelStruct<FUNCDESC>(funcDesc, () =>
                {
                    typeInfo.ReleaseFuncDesc(ppFuncDesc);
                });
            }
            else
            {
                typeInfo.ReleaseFuncDesc(ppFuncDesc);
            }
        }

        return null;
    }

    public static bool ContainsFuncDescByName(this ITypeInfo2 typeInfo, string name)
    {
        using var funcDesc = typeInfo.GetFuncDescByName(name);
        return funcDesc != null;
    }

    public static KeyValuePair<string, object>[] GetAllEnumValues(this ITypeInfo2 typeInfo)
    {
        var retVal = new List<KeyValuePair<string, object>>();
        using var attribute = typeInfo.GetTypeInfoAttributes();
        attribute.Should().NotBeNull();
        var count = attribute!.Value.cVars;
        for (short i = 0; i < count; i++)
        {
            typeInfo.GetVarDesc(i, out var ppVarDesc);

            try
            {
                var varDesc = Marshal.PtrToStructure<VARDESC>(ppVarDesc);

                if (varDesc.varkind == VARKIND.VAR_CONST)
                {
                    typeInfo!.GetDocumentation(varDesc.memid, out var strTypeLibName, out var strDocString, out var dwHelpContext, out var strHelpFile);
                    var value = Marshal.GetObjectForNativeVariant(varDesc.desc.lpvarValue);
                    retVal.Add(new KeyValuePair<string, object>(strTypeLibName, value!));
                }
            }
            finally
            {
                typeInfo.ReleaseVarDesc(ppVarDesc);
            }
        }

        return retVal.ToArray();
    }

    public static ITypeInfo2? GetTypeInfoByName(this ITypeLib2 typeLib, string name)
    {
        var count = typeLib.GetTypeInfoCount();
        for (var i = 0; i < count; i++)
        {
            typeLib.GetTypeInfo(i, out var typeInfo);

            if (string.Equals(typeInfo.GetName(), name, StringComparison.Ordinal))
            {
                return typeInfo as ITypeInfo2;
            }
        }

        return null;
    }

    public static string GetName(this ITypeInfo typeInfo)
    {
        typeInfo.GetDocumentation(-1, out var strTypeLibName, out _, out _, out _);
        return strTypeLibName;
    }

    public static VarEnum GetVarEnum(this TYPEDESC desc)
    {
        return (VarEnum)desc.vt;
    }

    public static short GetShortVarEnum(this Type varEnum)
    {
        return (short)GetVarEnum(varEnum);
    }

    public static VarEnum GetVarEnum(this Type varEnum)
    {
        switch (varEnum.ToString())
        {
            case "System.Boolean":
                return VarEnum.VT_BOOL;
            case "System.String":
                return VarEnum.VT_BSTR;
            case "System.DateTime":
                return VarEnum.VT_DATE;
            case "System.Decimal":
                return VarEnum.VT_DECIMAL;
            case "System.SByte":
                return VarEnum.VT_I1;
            case "System.Int16":
                return VarEnum.VT_I2;
            case "System.Int32":
                return VarEnum.VT_I4;
            case "System.Int64":
                return VarEnum.VT_I8;
            case "System.Collections.IEnumerator":
                return VarEnum.VT_PTR;
            case "System.Single":
                return VarEnum.VT_R4;
            case "System.Double":
                return VarEnum.VT_R8;
            case "System.Object[]":
                return VarEnum.VT_SAFEARRAY;
            case "System.Byte":
                return VarEnum.VT_UI1;
            case "System.Char":
            case "System.UInt16":
                return VarEnum.VT_UI2;
            case "System.UInt32":
                return VarEnum.VT_UI4;
            case "System.UInt64":
                return VarEnum.VT_UI8;
            case "System.Object":
                return VarEnum.VT_VARIANT;
            case "System.Void":
                return VarEnum.VT_VOID;
            case "System.Drawing.Color":
            case "System.Guid":
            case "System.Enum":
            default:
                if (varEnum.IsInterface)
                {
                    return VarEnum.VT_PTR;
                }
                else if (varEnum.IsArray)
                {
                    return VarEnum.VT_SAFEARRAY;
                }
                else if (varEnum.IsClass)
                {
                    return VarEnum.VT_UNKNOWN;
                }
                return VarEnum.VT_USERDEFINED;
        }
    }
}
