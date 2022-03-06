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

#pragma warning disable 612, 618

using System.Reflection;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.Writer;

internal class LibraryWriter : BaseWriter
{
    public LibraryWriter(Assembly assembly, WriterContext context) : base(context)
    {
        Assembly = assembly;
    }

    private Assembly Assembly { get; }

    private List<TypeWriter> TypeWriters { get; } = new();

    private Dictionary<Type, string> UniqueNames { get; } = new();

    public override void Create()
    {
        var name = Assembly.GetName().Name!.Replace('.', '_');

        var versionFromAssembly = Assembly.GetTLBVersionForAssembly();
        var guid = Context.Options.OverrideTlbId == Guid.Empty ? Assembly.GetTLBGuidForAssembly() : Context.Options.OverrideTlbId;

        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(LibraryWriter));
        }

        var typeLib = Context.TargetTypeLib;
        if (typeLib != null)
        {
            typeLib.SetGuid(guid);
            typeLib.SetVersion(versionFromAssembly.Major == 0 ? (ushort)1 : (ushort)versionFromAssembly.Major, (ushort)versionFromAssembly.Minor);
            typeLib.SetLcid(Constants.LCID_NEUTRAL);
            typeLib.SetCustData(new Guid(Guids.GUID_ExportedFromComPlus), Assembly.FullName);
            typeLib.SetName(name);
            var description = Assembly.GetCustomAttributes<AssemblyDescriptionAttribute>().FirstOrDefault()?.Description;
            if (description != null)
            {
                typeLib.SetDocString(description);
            }

            Context.TypeInfoResolver.AddTypeLib((ITypeLib)typeLib);
            Context.TypeInfoResolver.AddAdditionalTypeLibs();

            if (!string.IsNullOrEmpty(Context.Options.Out))
            {
                typeLib.SetLibFlags((uint)LIBFLAGS.LIBFLAG_FHASDISKIMAGE).ThrowIfFailed("Failed to set LIBFLAGS LIBFLAG_FHASDISKIMAGE.");
            }
        }

        CollectAllTypes();

        // Create and prepare the CreateTypeInfo instance.
        TypeWriters.ForEach(e => e.CreateTypeInfo());

        // Create the type inheritance
        TypeWriters.ForEach(e => e.CreateTypeInheritance());

        // Fill the content of methods and enums.
        TypeWriters.ForEach(e => e.Create());
    }

    private void CollectAllTypes()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(LibraryWriter));
        }

        var comVisibleAttributeAssembly = Assembly.GetCustomAttribute<ComVisibleAttribute>();
        var typesAreVisibleForComByDefault = comVisibleAttributeAssembly == null || comVisibleAttributeAssembly.Value;

        Type?[] types;
        try
        {
            types = Assembly.GetTypes().ToArray();
        }
        catch (ReflectionTypeLoadException e)
        {
            Context.LogWarning($"Type library exporter encountered an error while processing '{Assembly.GetName().Name}'. Error: {e.LoaderExceptions.First()!.Message}");

            if (!e.Types.Any(t => t != null))
            {
                throw e.LoaderExceptions.First()!;
            }
            types = e.Types;
        }

        List<TypeWriter> classInterfaceWriters = new();

        foreach (var type in types)
        {
            if (type == null)
            {
                continue;
            }

            var comVisibleAttribute = type.GetCustomAttribute<ComVisibleAttribute>();

            if (typesAreVisibleForComByDefault && comVisibleAttribute != null && !comVisibleAttribute.Value)
            {
                continue;
            }

            if (!typesAreVisibleForComByDefault && (comVisibleAttribute == null || !comVisibleAttribute.Value))
            {
                continue;
            }

            if (!type.IsPublic)
            {
                continue;
            }

            if (type.IsGenericType)
            {
                continue;
            }
            // Add this type to the unique names collection.
            UpdateUniqueNames(type);

            TypeWriter? typeWriter = null;
            if (type.IsInterface)
            {
                var interfaceTypeAttribute = type.GetCustomAttribute<InterfaceTypeAttribute>();

                typeWriter = interfaceTypeAttribute != null
                    ? interfaceTypeAttribute.Value switch
                    {
                        ComInterfaceType.InterfaceIsDual => new DualInterfaceWriter(type, this, Context),
                        ComInterfaceType.InterfaceIsIDispatch => new DispInterfaceWriter(type, this, Context),
                        ComInterfaceType.InterfaceIsIUnknown => new IUnknownInterfaceWriter(type, this, Context),
                        _ => throw new NotSupportedException($"{interfaceTypeAttribute.Value} not supported"),
                    }
                    : new DualInterfaceWriter(type, this, Context);
            }
            else if (type.IsEnum)
            {
                typeWriter = new EnumWriter(type, this, Context);
            }
            else if (type.IsClass)
            {
                typeWriter = new ClassWriter(type, this, Context);
            }
            else if (type.IsValueType && !type.IsPrimitive)
            {
                typeWriter = new StructWriter(type, this, Context);
            }
            if (typeWriter != null)
            {

                TypeWriters.Add(typeWriter);
            }

            if (type.IsClass)
            {
                var createClassInterface = true;
                //check for class interfaces to generate:
                if (type?.GetCustomAttribute<ClassInterfaceAttribute>() != null)
                {
                    var classInterfaceType = type.GetCustomAttribute<ClassInterfaceAttribute>()!.Value;
                    switch (classInterfaceType)
                    {
                        case ClassInterfaceType.AutoDispatch:
                            createClassInterface = true;
                            break;
                        case ClassInterfaceType.AutoDual:
                            //CA1408: Do not use AutoDual ClassInterfaceType
                            //https://docs.microsoft.com/en-us/visualstudio/code-quality/ca1408?view=vs-2022
                            throw new NotSupportedException("Dual class interfaces not supported!");
                        case ClassInterfaceType.None:
                            createClassInterface = false;
                            break;
                    }
                }

                //check for generic base types
                var baseType = type!.BaseType;
                while (baseType != null)
                {
                    if (baseType.IsGenericType)
                    {
                        createClassInterface = false;
                        break;
                    }
                    baseType = baseType.BaseType;
                }

                if (createClassInterface)
                {
                    if (typeWriter is ClassWriter classWriter)
                    {
                        var classInterfaceWriter = new ClassInterfaceWriter(type!, this, Context);
                        classInterfaceWriters.Add(classInterfaceWriter);
                        classWriter.ClassInterfaceWriter = classInterfaceWriter;
                    }
                }
            }
        }
        TypeWriters.AddRange(classInterfaceWriters);
    }

    private void UpdateUniqueNames(Type type)
    {
        var searchExistingType = UniqueNames.FirstOrDefault(t => t.Key.Name == type.Name);
        if (searchExistingType.Key != null)
        {
            var namesp = searchExistingType.Key.Namespace!;
            namesp = namesp.Replace(".", "_");
            UniqueNames[searchExistingType.Key] = $"{namesp}_{searchExistingType.Key.Name}";

            namesp = type.Namespace!;
            namesp = namesp.Replace(".", "_");
            UniqueNames.Add(type, $"{namesp}_{type.Name}");
        }
        else
        {
            UniqueNames.Add(type, type.Name);
        }
    }

    internal string GetUniqueTypeName(Type type)
    {
        var searchExistingType = UniqueNames.FirstOrDefault(t => t.Key.Name == type.Name && t.Key.Namespace == type.Namespace);
        return searchExistingType.Value;
    }

    protected override void Dispose(bool disposing)
    {
        if (TypeWriters != null)
        {
            TypeWriters.ForEach(t => t.Dispose());
            TypeWriters.Clear();
        }

        base.Dispose(disposing);
    }
}
