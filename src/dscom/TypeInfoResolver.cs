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
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices;

internal sealed class TypeInfoResolver : ITypeLibCache
{
    private readonly IDictionary<TypeLibIdentifier, ITypeLib> _typeLibs = new Dictionary<TypeLibIdentifier, ITypeLib>();

    private readonly List<string> _additionalLibs = new();

    private readonly Dictionary<Guid, ITypeInfo> _types = new();

    private readonly Dictionary<Type, ITypeInfo?> _resolvedTypeInfos = new();

    public WriterContext WriterContext { get; }

    public TypeInfoResolver(WriterContext writerContext)
    {
        WriterContext = writerContext;

        if (WriterContext.NotifySink is ITypeLibCacheProvider typeLibCacheProvider)
        {
            typeLibCacheProvider.TypeLibCache = this;
        }

        AddTypeLib("stdole", Constants.LCID_NEUTRAL, new Guid(Guids.TLBID_Ole), 2, 0);

        _additionalLibs.AddRange(WriterContext.Options.TLBReference);

        foreach (var directoriesWithTypeLibraries in WriterContext.Options.TLBRefpath)
        {
            if (Directory.Exists(directoriesWithTypeLibraries))
            {
                _additionalLibs.AddRange(Directory.GetFiles(directoriesWithTypeLibraries, "*.tlb"));
            }
        }

        var fullPath = !string.IsNullOrEmpty(WriterContext.Options.Out) ? Path.GetFullPath(WriterContext.Options.Out) : string.Empty;

        var pathToRemove = _additionalLibs.FirstOrDefault(p => string.Equals(p, fullPath, StringComparison.OrdinalIgnoreCase));
        if (pathToRemove != null)
        {
            _additionalLibs.Remove(pathToRemove);
        }

        _additionalLibs = _additionalLibs.Distinct().ToList();
    }

    public ITypeInfo? ResolveTypeInfo(Guid guid)
    {
        _types.TryGetValue(guid, out var typeInfo);
        return typeInfo;
    }

    /// <summary>
    /// Resolve the <see cref="ITypeInfo"/> by a <see cref="Guid"/>. If the <see cref="ITypeInfo"/> is not
    /// present yet, it will try to add the type library for the corresponding assembly.
    /// </summary>
    private ITypeInfo? ResolveTypeInfo(Type type, Guid guid)
    {
        // If the given type is not present in _types
        // this means, that the typelib is not loaded yet
        if (_types.TryGetValue(guid, out var typeInfo))
        {
            return typeInfo;
        }

        var assembly = type.Assembly;

        // check if the type library is already present
        var identifier = GetTypeLibFromIdentifier(assembly.GetLibIdentifier(WriterContext.Options.OverrideTlbId));
        if (identifier is null)
        {
            var name = assembly.GetName().Name ?? string.Empty;

            var additionalLibsWithMatchingName = _additionalLibs
                .Where(additionalLib => Path.GetFileNameWithoutExtension(additionalLib).Equals(name, StringComparison.OrdinalIgnoreCase));

            // At first we try to find a type library that matches the assembly name.
            // We do this to limit the number of type libraries to load.
            // See https://github.com/dspace-group/dscom/issues/310
            foreach (var additionalLib in additionalLibsWithMatchingName)
            {
                AddTypeLib(additionalLib);
                typeInfo = ResolveTypeInfo(type.GUID);
                break;
            }

            // If no type was found in a type library with matching name we search in the remaining type libraries.
            if (typeInfo == null)
            {
                var additionalLibsWithoutMatchingName = _additionalLibs.Except(additionalLibsWithMatchingName);
                foreach (var additionalLib in additionalLibsWithoutMatchingName)
                {
                    AddTypeLib(additionalLib);
                    typeInfo = ResolveTypeInfo(type.GUID);
                    if (typeInfo != null)
                    {
                        break;
                    }
                }
            }

            var notifySink = WriterContext.NotifySink;
            if (notifySink != null)
            {
                if (notifySink.ResolveRef(assembly) is ITypeLib refTypeLib)
                {
                    AddTypeLib(refTypeLib);
                    typeInfo = ResolveTypeInfo(type.GUID);
                }
            }
        }

        // The dictionary will be updated in 'AddTypeLib'.
        // Therefore it should be contain the typeinfo now.
        _types.TryGetValue(guid, out typeInfo);
        return typeInfo;
    }

    public ITypeInfo? ResolveTypeInfo(Type type)
    {
#pragma warning disable IDE0045 // Convert to conditional expression
        if (_resolvedTypeInfos.TryGetValue(type, out var typeInfo))
        {
            return typeInfo;
        }

        ITypeInfo? retval;
        if (type.FullName == "System.Collections.IEnumerator")
        {
            retval = ResolveTypeInfo(new Guid(Guids.IID_IEnumVARIANT));
        }
        else if (type.FullName == "System.Drawing.Color")
        {
            retval = ResolveTypeInfo(new Guid(Guids.TDID_OLECOLOR));
        }
        else if (type.FullName == "System.Guid")
        {
            var identifierOle = new TypeLibIdentifier()
            {
                Name = "System.Guid",
                LanguageIdentifier = Constants.LCID_NEUTRAL,
                LibID = new Guid(Guids.TLBID_Ole),
                MajorVersion = 2,
                MinorVersion = 0
            };
            var typeLibOle = GetTypeLibFromIdentifier(identifierOle) ?? throw new COMException("System.Guid not found in any type library");
            typeLibOle.GetTypeInfo(0, out retval);
        }
        else if (type.GUID == new Guid(Guids.IID_IDispatch))
        {
            retval = ResolveTypeInfo(new Guid(Guids.IID_IDispatch));
        }
        else if (type.IsClass)
        {
            retval = ResolveTypeInfo(type, MarshalExtension.GetClassInterfaceGuidForType(type));
        }
        else
        {
            retval = ResolveTypeInfo(type, type.GUID);
        }
#pragma warning restore IDE0045 // Convert to conditional expression

        _resolvedTypeInfos[type] = retval;
        return retval;
    }

    private static ITypeInfo? GetDefaultInterface(ITypeInfo? typeInfo)
    {
        if (typeInfo != null)
        {
            var ppTypeAttr = new IntPtr();
            try
            {
                typeInfo.GetTypeAttr(out ppTypeAttr);
                var typeAttr = Marshal.PtrToStructure<TYPEATTR>(ppTypeAttr);

                for (var i = 0; i < typeAttr.cImplTypes; i++)
                {
                    typeInfo.GetRefTypeOfImplType(i, out var href);
                    typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
                    typeInfo.GetImplTypeFlags(i, out var pImplTypeFlags);

                    refTypeInfo.GetDocumentation(-1, out var m, out _, out _, out _);

                    if (pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT))
                    {
                        return refTypeInfo;
                    }
                }
            }
            finally
            {
                typeInfo.ReleaseTypeAttr(ppTypeAttr);
            }
        }

        return null;
    }

    internal ITypeInfo? ResolveDefaultCoClassInterface(Type type)
    {
        if (type.IsClass)
        {
            var typeInfo = ResolveTypeInfo(type.GUID);
            return GetDefaultInterface(typeInfo);
        }

        return null;
    }

    public ITypeLib? GetTypeLibFromIdentifier(TypeLibIdentifier identifier)
    {
        _typeLibs.TryGetValue(identifier, out var value);
        return value;
    }

    public void AddTypeLib(string name, int lcid, Guid registeredTypeLib, ushort majorVersion, ushort minorVersion)
    {
        OleAut32.LoadRegTypeLib(registeredTypeLib, majorVersion, minorVersion, Constants.LCID_NEUTRAL, out var typeLib)
            .ThrowIfFailed($"Failed to load type library {registeredTypeLib}.");

        var identifier = new TypeLibIdentifier
        {
            Name = name,
            LanguageIdentifier = lcid,
            LibID = registeredTypeLib,
            MajorVersion = majorVersion,
            MinorVersion = minorVersion
        };

        _typeLibs.Add(identifier, typeLib);
        UpdateTypeLibCache(typeLib);
    }

    public bool AddTypeLib(ITypeLib typeLib)
    {
        var retValue = false;
        typeLib.GetLibAttr(out var libattrPtr);
        try
        {
            var libattr = Marshal.PtrToStructure<TYPELIBATTR>(libattrPtr);
            typeLib.GetDocumentation(-1, out var name, out var strDocString, out var dwHelpContext, out var strHelpFile);
            var identifier = new TypeLibIdentifier
            {
                Name = name,
                LanguageIdentifier = libattr.lcid,
                LibID = libattr.guid,
                MajorVersion = (ushort)libattr.wMajorVerNum,
                MinorVersion = (ushort)libattr.wMinorVerNum
            };

            if (!_typeLibs.ContainsKey(identifier))
            {
                _typeLibs.Add(identifier, typeLib);
                UpdateTypeLibCache(typeLib);
                retValue = true;
            }
        }
        finally
        {
            typeLib.ReleaseTLibAttr(libattrPtr);
        }
        return retValue;
    }

    private void UpdateTypeLibCache(ITypeLib typelib)
    {
        var count = typelib.GetTypeInfoCount();
        for (var i = 0; i < count; i++)
        {
            typelib.GetTypeInfo(i, out var typeInfo);
            AddTypeToCache(typeInfo);
        }
    }

    public void AddTypeToCache(ITypeInfo? typeInfo)
    {
        if (typeInfo != null)
        {
            typeInfo.GetTypeAttr(out var ppTypAttr);
            try
            {
                var attr = Marshal.PtrToStructure<TYPEATTR>(ppTypAttr);
                if (!_types.ContainsKey(attr.guid))
                {
                    _types[attr.guid] = typeInfo;
                }
            }
            finally
            {
                typeInfo.ReleaseTypeAttr(ppTypAttr);
            }
        }
    }

    [ExcludeFromCodeCoverage] // UnitTest with dependent type libraries is not supported
    public bool AddTypeLib(string typeLibPath)
    {
        OleAut32.LoadTypeLibEx(typeLibPath, REGKIND.NONE, out var typeLib).ThrowIfFailed($"Failed to load type library {typeLibPath}.");
        return AddTypeLib(typeLib);
    }
}
