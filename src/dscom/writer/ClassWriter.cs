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

namespace dSPACE.Runtime.InteropServices.Writer;

internal sealed class ClassWriter : TypeWriter
{
    public ClassWriter(Type sourceType, LibraryWriter libraryWriter, WriterContext context) : base(sourceType, libraryWriter, context)
    {
        TypeKind = TYPEKIND.TKIND_COCLASS;
    }

    public override void CreateTypeInfo()
    {
        var constructorsInfo = SourceType.GetConstructors();

        //only one default constructor allowed
        var constructorInfo = constructorsInfo.Where(x => !x.IsGenericMethod && x.GetParameters().Length == 0).FirstOrDefault();
        if (constructorInfo != null)
        {
            TypeFlags = TYPEFLAGS.TYPEFLAG_FCANCREATE;
        }

        base.CreateTypeInfo();
    }

    public override void Create()
    {
        Context.LogTypeExported($"Class '{Name}' exported.");
    }

    public TypeWriter? ClassInterfaceWriter
    {
        get; set;
    }

    public override void CreateTypeInheritance()
    {
        uint index = 0;

        var interfaces = SourceType.GetInterfaces().Where(x => x.IsComVisible());

        Type? defaultInterfaceType = null;
        var useClassInterfaceAsDefault = false;
        if (SourceType.GetCustomAttribute<ComDefaultInterfaceAttribute>() != null)
        {
            defaultInterfaceType = SourceType.GetCustomAttribute<ComDefaultInterfaceAttribute>()!.Value;
        }
        else
        {
            //no default attribute
            if (ClassInterfaceWriter != null)
            {
                useClassInterfaceAsDefault = true;
            }
            else
            {
                var result = SourceType.GetComInterfacesRecursive();
                var orderedInterfaces = new List<Type>();
                result.ForEach(t => orderedInterfaces.AddRange(t));
                defaultInterfaceType = orderedInterfaces.FirstOrDefault();
            }
        }

        if (ClassInterfaceWriter != null)
        {
            var classInterfaceTypeInfo = (ITypeInfo)ClassInterfaceWriter.TypeInfo;
            TypeInfo.AddRefTypeInfo(classInterfaceTypeInfo, out var phRefType)
                .ThrowIfFailed($"Failed to add class interface reference to {SourceType}.");
            TypeInfo.AddImplType(index, phRefType)
                .ThrowIfFailed($"Failed to add class interface implementation to {SourceType}.");
            if (useClassInterfaceAsDefault)
            {
                TypeInfo.SetImplTypeFlags(index, IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT)
                        .ThrowIfFailed($"Failed to set IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT for class interface.");
            }
            index++;
        }

        foreach (var currentInterface in interfaces)
        {
            var referencedTypeInfo = Context.TypeInfoResolver.ResolveTypeInfo(currentInterface);
            if (referencedTypeInfo != null)
            {
                TypeInfo.AddRefTypeInfo(referencedTypeInfo, out var phRefType)
                    .ThrowIfFailed($"Failed to add reference type {SourceType}.");
                TypeInfo.AddImplType(index, phRefType)
                    .ThrowIfFailed($"Failed to add interface {currentInterface.Name} to {SourceType}.");

                //check for attributation
                if (defaultInterfaceType != null && defaultInterfaceType == currentInterface)
                {
                    TypeInfo.SetImplTypeFlags(index, IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT)
                        .ThrowIfFailed($"Failed to set IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT for {currentInterface.Name}.");
                }
                index++;
            }
            else
            {
                Context.NotifySink!.ReportEvent(ExporterEventKind.NOTIF_CONVERTWARNING, 0, $"ComVisible interface {currentInterface} could not be added to source type {SourceType}.");
            }

        }

        //check for ComSourceInterfaces
        var defaultSourceInterfaceSet = false;
        foreach (var sourceInterfaceAttribute in SourceType.GetCustomAttributesRecursive<ComSourceInterfacesAttribute>())
        {
            foreach (var interfaceTypeValue in sourceInterfaceAttribute.Value.Split('\0').Distinct())
            {
                var interfaceType = SourceType.Assembly.GetType(interfaceTypeValue) ?? AppDomain.CurrentDomain.GetAssemblies().Select(z => z.GetType(interfaceTypeValue)).FirstOrDefault(x => x != null);
                if (interfaceType != null)
                {
                    var interfaceTypeInfo = Context.TypeInfoResolver.ResolveTypeInfo(interfaceType);
                    if (interfaceTypeInfo != null)
                    {
                        TypeInfo.AddRefTypeInfo(interfaceTypeInfo, out var phRefType)
                            .ThrowIfFailed($"Failed to add reference type {interfaceTypeValue}.");
                        TypeInfo.AddImplType(index, phRefType)
                            .ThrowIfFailed($"Failed to add interface {interfaceTypeValue} to {SourceType}.");
                        TypeInfo.SetImplTypeFlags(index, defaultSourceInterfaceSet ? IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE : IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT | IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE)
                            .ThrowIfFailed($"Failed to set IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT | IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE for {SourceType}.");
                        defaultSourceInterfaceSet = true;
                        index++;
                    }
                }
            }
        }

        base.CreateTypeInheritance();
    }
}
