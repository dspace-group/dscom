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

namespace dSPACE.Runtime.InteropServices;

public static class Extensions
{
    /// <summary>
    /// Returns a assembly indentifier.
    /// </summary>
    /// <param name="assembly">An assembly that is used to create an identifer.</param>
    /// <param name="overrideGuid">A guid that should be used</param>
    public static TypeLibIdentifier GetLibIdentifier(this Assembly assembly, Guid overrideGuid)
    {
        var version = assembly.GetTLBVersionForAssembly();

        // From Major, Minor, Language, Guid
        return new TypeLibIdentifier()
        {
            Name = assembly.GetName().Name ?? string.Empty,
            MajorVersion = (ushort)version.Major,
            MinorVersion = (ushort)version.Minor,
            LibID = overrideGuid == Guid.Empty ? assembly.GetTLBGuidForAssembly() : overrideGuid,
            LanguageIdentifier = assembly.GetTLBLanguageIdentifierForAssembly()
        };
    }

    /// <summary>
    /// Returns a assembly indentifier.
    /// </summary>
    /// <param name="assembly">An assembly that is used to create an identifer.</param>
    public static TypeLibIdentifier GetLibIdentifier(this Assembly assembly)
    {
        return assembly.GetLibIdentifier(Guid.Empty);
    }

    [SuppressMessage("Microsoft.Style", "IDE0060", Justification = "For future use")]
    internal static int GetTLBLanguageIdentifierForAssembly(this Assembly assembly)
    {
        // This is currently a limitation.
        // Only LCID_NEUTRAL is supported.
        return Constants.LCID_NEUTRAL;
    }

    internal static Version GetTLBVersionForAssembly(this Assembly assembly)
    {
        TypeLibVersionAttribute? typeLibVersionAttribute = null;
        try
        {
            typeLibVersionAttribute = assembly.GetCustomAttribute<TypeLibVersionAttribute>();
        }
        catch (FileNotFoundException)
        {
        }

        if (typeLibVersionAttribute != null)
        {
            return new Version(typeLibVersionAttribute.MajorVersion, typeLibVersionAttribute.MinorVersion);
        }
        if (assembly.GetName().Version != null)
        {
            return assembly.GetName().Version!;
        }

        throw new InvalidOperationException();
    }

    internal static Guid GetTLBGuidForAssembly(this Assembly assembly)
    {
        return MarshalExtension.GetTypeLibGuidForAssembly(assembly);
    }

    internal static bool IsSpecialHandledClass(this Type type)
    {
        var typeAsString = type.ToString();
        return typeAsString switch
        {
            "System.Object" => true,
            "System.String" => true,
            _ => false
        };
    }

    internal static bool IsSpecialHandledValueType(this Type type)
    {
        var typeAsString = type.ToString();
        return typeAsString switch
        {
            "System.Void" => true,
            "System.Drawing.Color" => true,
            "System.DateTime" => true,
            "System.Decimal" => true,
            "System.Guid" => true,
            _ => false
        };
    }
    internal static Type GetCLRType(this VarEnum varenum)
    {
        return varenum switch
        {
            VarEnum.VT_VOID => typeof(void),
            VarEnum.VT_DECIMAL => typeof(decimal),
            VarEnum.VT_R4 => typeof(float),
            VarEnum.VT_R8 => typeof(double),
            VarEnum.VT_BOOL => typeof(bool),
            VarEnum.VT_VARIANT => typeof(object),
            VarEnum.VT_BSTR => typeof(string),
            VarEnum.VT_I4 => typeof(int),
            VarEnum.VT_UI1 => typeof(byte),
            VarEnum.VT_I1 => typeof(sbyte),
            VarEnum.VT_I2 => typeof(short),
            VarEnum.VT_I8 => typeof(long),
            VarEnum.VT_UI2 => typeof(ushort),
            VarEnum.VT_UI4 => typeof(uint),
            VarEnum.VT_UI8 => typeof(ulong),
            VarEnum.VT_SAFEARRAY => typeof(object[]),
            VarEnum.VT_DATE => typeof(DateTime),
            VarEnum.VT_UNKNOWN => typeof(ComTypes.IUnknown),
            VarEnum.VT_DISPATCH => typeof(ComTypes.IDispatch),
            VarEnum.VT_PTR => typeof(IntPtr),
            _ => throw new NotSupportedException(),
        };
    }

    internal static UnmanagedType? ToUnmanagedType(this VarEnum varenum)
    {
        return varenum switch
        {
            VarEnum.VT_EMPTY => null,
            VarEnum.VT_NULL => null,
            VarEnum.VT_I2 => UnmanagedType.I2,
            VarEnum.VT_I4 => UnmanagedType.I4,
            VarEnum.VT_R4 => UnmanagedType.R4,
            VarEnum.VT_R8 => UnmanagedType.R8,
            VarEnum.VT_CY => UnmanagedType.Bool,
            VarEnum.VT_DATE => null,
            VarEnum.VT_BSTR => UnmanagedType.BStr,
            VarEnum.VT_DISPATCH => UnmanagedType.IDispatch,
            VarEnum.VT_ERROR => UnmanagedType.Error,
            VarEnum.VT_BOOL => UnmanagedType.Bool,
            VarEnum.VT_VARIANT => UnmanagedType.Struct,
            VarEnum.VT_UNKNOWN => UnmanagedType.IUnknown,
            VarEnum.VT_DECIMAL => UnmanagedType.Currency,
            VarEnum.VT_I1 => UnmanagedType.I1,
            VarEnum.VT_UI1 => UnmanagedType.U1,
            VarEnum.VT_UI2 => UnmanagedType.U2,
            VarEnum.VT_UI4 => UnmanagedType.U4,
            VarEnum.VT_I8 => UnmanagedType.I8,
            VarEnum.VT_UI8 => UnmanagedType.U8,
            VarEnum.VT_INT => UnmanagedType.I4,
            VarEnum.VT_UINT => UnmanagedType.U4,
            VarEnum.VT_VOID => null,
            VarEnum.VT_HRESULT => UnmanagedType.Error,
            VarEnum.VT_PTR => null,
            VarEnum.VT_SAFEARRAY => UnmanagedType.SafeArray,
            VarEnum.VT_CARRAY => null,
            VarEnum.VT_USERDEFINED => null,
            VarEnum.VT_LPSTR => UnmanagedType.LPStr,
            VarEnum.VT_LPWSTR => UnmanagedType.LPWStr,
            VarEnum.VT_RECORD => null,
            VarEnum.VT_FILETIME => null,
            VarEnum.VT_BLOB => null,
            VarEnum.VT_STREAM => null,
            VarEnum.VT_STORAGE => null,
            VarEnum.VT_STREAMED_OBJECT => null,
            VarEnum.VT_STORED_OBJECT => null,
            VarEnum.VT_BLOB_OBJECT => null,
            VarEnum.VT_CF => null,
            VarEnum.VT_CLSID => null,
            VarEnum.VT_VECTOR => null,
            VarEnum.VT_ARRAY => null,
            VarEnum.VT_BYREF => null,
            _ => throw new NotSupportedException(),
        };
    }

    internal static bool IsComVisible(this Type type)
    {
        type = type.GetUnderlayingType();
        var AssemblyIsComVisible = type.Assembly.GetCustomAttribute<ComVisibleAttribute>() == null
                                   || type.Assembly.GetCustomAttribute<ComVisibleAttribute>()!.Value;

        if (AssemblyIsComVisible)
        {
            //a type of a com visible assembly is com visible unless it is explicitly com invisible
            if (type.GetCustomAttribute<ComVisibleAttribute>() != null
                && !type.GetCustomAttribute<ComVisibleAttribute>()!.Value)
            {
                return false;
            }
        }
        else
        {
            //a type of a com invisible assembly is com invisible unless it is explicitly com visible
            if (type.GetCustomAttribute<ComVisibleAttribute>() == null
                || (type.GetCustomAttribute<ComVisibleAttribute>() != null
                && !type.GetCustomAttribute<ComVisibleAttribute>()!.Value))
            {
                return false;
            }
        }
        return true;
    }

    internal static Type GetUnderlayingType(this Type type)
    {
        var returnType = type;
        while (returnType!.IsByRef || returnType!.IsArray)
        {
            returnType = returnType.GetElementType();
        }
        return returnType;
    }
}
