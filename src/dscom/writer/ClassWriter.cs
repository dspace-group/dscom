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

internal class ClassWriter : TypeWriter
{
    public ClassWriter(Type sourceType, LibraryWriter libraryWriter, WriterContext context) : base(sourceType, libraryWriter, context)
    {
        TypeKind = TYPEKIND.TKIND_COCLASS;
    }

    public override void CreateTypeInfo()
    {
        var constructorsInfo = SourceType.GetConstructors();
        //only one default constructor allowed
        var constructorInfo = constructorsInfo.Where(x => x.GetParameters().Length == 0 && !x.IsGenericMethod).FirstOrDefault();
        if (constructorInfo != null)
        {
            TypeFlags = TYPEFLAGS.TYPEFLAG_FCANCREATE;
        }

        base.CreateTypeInfo();

        Context.LogVerbose($"Creating coclass {Name}");
    }

    public override void Create()
    {
    }

    public TypeWriter? ClassInterfaceWriter
    {
        get; set;
    }

    public override void CreateTypeInheritance()
    {
        uint index = 0;
        var defaultInterfaceSet = false;

        if (ClassInterfaceWriter != null)
        {
            var classInterfaceTypeInfo = (ITypeInfo)ClassInterfaceWriter.TypeInfo;
            TypeInfo.AddRefTypeInfo(classInterfaceTypeInfo, out var phRefType)
                .ThrowIfFailed($"Failed to add class interface reference to {SourceType}.");
            TypeInfo.AddImplType(index, phRefType)
                .ThrowIfFailed($"Failed to add class interface implementation to {SourceType}.");
            index++;
        }

        var interfaces = SourceType.GetInterfaces();
        foreach (var currentInterface in interfaces)
        {
            if (!currentInterface.IsComVisible())
            {
                continue;
            }

            var referencedTypeInfo = Context.TypeInfoResolver.ResolveTypeInfo(currentInterface);

            TypeInfo.AddRefTypeInfo(referencedTypeInfo, out var phRefType)
                .ThrowIfFailed($"Failed to add reference type {SourceType}.");
            TypeInfo.AddImplType(index, phRefType)
                .ThrowIfFailed($"Failed to add interface {currentInterface.Name} to {SourceType}.");

            //check for attributation
            if (SourceType.GetCustomAttributes<ComDefaultInterfaceAttribute>().Any(y => y.Value == currentInterface))
            {
                TypeInfo.SetImplTypeFlags(index, IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT)
                    .ThrowIfFailed($"Failed to set IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT for {currentInterface.Name}.");
                defaultInterfaceSet = true;
            }

            if (currentInterface.GetCustomAttributes<ComEventInterfaceAttribute>().Any())
            {
                if (SourceType.GetCustomAttributes<ComSourceInterfacesAttribute>().Any(y => y.Value == currentInterface.GetCustomAttributes<ComEventInterfaceAttribute>().First().SourceInterface.FullName))
                {
                    TypeInfo.SetImplTypeFlags(index, IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT | IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE)
                        .ThrowIfFailed($"Failed to set IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT | IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE for {currentInterface.Name}.");
                    defaultInterfaceSet = true;
                }
            }
            index++;
        }

        //check for ComSourceInterfaces
        foreach (var sourceInterfaceAttribute in SourceType.GetCustomAttributes<ComSourceInterfacesAttribute>())
        {
            var interfaceType = SourceType.Assembly.GetType(sourceInterfaceAttribute.Value);
            if (interfaceType == null)
            {
                interfaceType = AppDomain.CurrentDomain.GetAssemblies().Select(z => z.GetType(sourceInterfaceAttribute.Value)).FirstOrDefault(x => x != null);
            }
            if (interfaceType != null)
            {
                var interfaceTypeInfo = Context.TypeInfoResolver.ResolveTypeInfo(interfaceType);
                if (interfaceTypeInfo != null)
                {
                    TypeInfo.AddRefTypeInfo(interfaceTypeInfo, out var phRefType)
                        .ThrowIfFailed($"Failed to add reference type {sourceInterfaceAttribute.Value}.");
                    TypeInfo.AddImplType(index, phRefType)
                        .ThrowIfFailed($"Failed to add interface {sourceInterfaceAttribute.Value} to {SourceType}.");
                    TypeInfo.SetImplTypeFlags(index, IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT | IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE)
                        .ThrowIfFailed($"Failed to set IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT | IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE for {SourceType}.");
                    index++;
                }
            }
        }

        base.CreateTypeInheritance();

        // The default interface is the class interface or the first implemented interface.
        if ((ClassInterfaceWriter != null || interfaces.Length > 0) && !defaultInterfaceSet)
        {
            TypeInfo.SetImplTypeFlags(0, IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT)
                .ThrowIfFailed($"Failed to set IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT to {SourceType}.");
        }
    }
}
