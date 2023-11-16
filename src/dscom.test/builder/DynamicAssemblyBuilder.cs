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

namespace dSPACE.Runtime.InteropServices.Tests;

internal sealed class DynamicAssemblyBuilder : DynamicBuilder<DynamicAssemblyBuilder>
{
    public DynamicAssemblyBuilder(string name, AssemblyBuilder assemblyBuilder, string path) : base(name)
    {
        TypeLibPath = path;
        AssemblyBuilder = assemblyBuilder;
        ModuleBuilder = CreateModuleBuilder();
        AppDomain.CurrentDomain.AssemblyResolve += ResolveEventHandler;
    }

    private List<DynamicTypeBuilder> DynamicTypeBuilder { get; } = new List<DynamicTypeBuilder>();

    private string TypeLibPath { get; }

    public ModuleBuilder ModuleBuilder { get; set; }

    public AssemblyBuilder AssemblyBuilder { get; set; }

    public Assembly Assembly => ModuleBuilder.Assembly;

    protected override AttributeTargets AttributeTarget => AttributeTargets.Assembly;

    private Assembly? ResolveEventHandler(object? sender, ResolveEventArgs args)
    {
        if (args!.RequestingAssembly!.FullName == AssemblyBuilder.FullName)
        {
            return AssemblyBuilder;
        }

        return null;
    }

    public DynamicAssemblyBuilderResult Build(bool storeOnDisk = true, bool skipTlbExeCall = false, bool useComAlias = false)
    {
        foreach (var customAttributeBuilder in CustomAttributeBuilder)
        {
            AssemblyBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        DynamicTypeBuilder.ForEach(dynamicTypeBuilder => dynamicTypeBuilder.CreateType());

        // Only supported for .NET Framework
        if (!skipTlbExeCall)
        {
            CreateDLLAndCreateTLBWithTlbExe();
        }

        var typeLibConverter = new TypeLibConverter();
        var assembly = ModuleBuilder.Assembly;
        var tlbFilePath = storeOnDisk ? TypeLibPath : string.Empty;
        var typeLibExporterNotifySink = new TypeLibExporterNotifySink(useComAlias ? assembly : null);
        if (typeLibConverter.ConvertAssemblyToTypeLib(assembly, tlbFilePath, typeLibExporterNotifySink) is not ITypeLib2 typelib)
        {
            throw new COMException("Cannot create type library for this dynamic assembly");
        }

        if (storeOnDisk && typelib is ICreateTypeLib2 createTypeLib2)
        {
            createTypeLib2.SaveAllChanges().ThrowIfFailed($"Failed to save type library {tlbFilePath}.");
        }

        if (storeOnDisk)
        {
            var options = new TypeLibTextConverterSettings()
            {
                Out = $"{TypeLibPath}.yaml",
                TypeLibrary = TypeLibPath,
                FilterRegex = new string[] { "file" }
            };

            var typeLibConvert = new TypeLibConverter();
            typeLibConvert.ConvertTypeLibToText(options);
        }

        AppDomain.CurrentDomain.AssemblyResolve -= ResolveEventHandler;

        return new DynamicAssemblyBuilderResult(typelib, ModuleBuilder.Assembly, typeLibExporterNotifySink);
    }

    internal DynamicTypeBuilder WithInterface(string interfaceName)
    {
        var dynamicTypeBuilder = new DynamicTypeBuilder(this, interfaceName, TypeAttributes.Interface | TypeAttributes.Public | TypeAttributes.Abstract);
        DynamicTypeBuilder.Add(dynamicTypeBuilder);
        return dynamicTypeBuilder;
    }

    internal DynamicEnumBuilder WithEnum(string enumName, Type enumType)
    {
        return new DynamicEnumBuilder(this, enumName, enumType);
    }

    internal DynamicEnumBuilder WithEnum<T>(string enumName)
    {
        return WithEnum(enumName, typeof(T));
    }

    internal DynamicTypeBuilder WithClass(string className)
    {
        return WithClass(className, Array.Empty<string>());
    }

    internal DynamicTypeBuilder WithClass(string className, string[] interfaceNames, Type? parentType = null)
    {
        var dynamicTypeBuilder = new DynamicTypeBuilder(this, className, TypeAttributes.Class | TypeAttributes.Public, interfaceNames, parentType);
        DynamicTypeBuilder.Add(dynamicTypeBuilder);
        return dynamicTypeBuilder;
    }

    internal DynamicTypeBuilder WithStruct(string structName)
    {
#pragma warning disable SYSLIB0050 // Type or member is obsolete
        var dynamicTypeBuilder = new DynamicTypeBuilder(this, structName, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.SequentialLayout | TypeAttributes.Serializable, null, typeof(ValueType));
#pragma warning restore SYSLIB0050 // Type or member is obsolete
        DynamicTypeBuilder.Add(dynamicTypeBuilder);
        return dynamicTypeBuilder;
    }

    internal void ForceTypeCreation(DynamicTypeBuilder toCreate, out Type? createdType)
    {
        createdType = toCreate.CreateType();
        DynamicTypeBuilder.Remove(toCreate);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Compatibility")]
    private void CreateDLLAndCreateTLBWithTlbExe()
    {
#if NETFRAMEWORK && DEBUG

        var dllPath = $"{TypeLibPath}.dll";

        AssemblyBuilder.Save(Path.GetFileName($"{TypeLibPath}.dll"));

        var tlbexpexePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\TlbExp.exe");

        if (File.Exists(tlbexpexePath))
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = tlbexpexePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.RedirectStandardError = false;
            process.StartInfo.Arguments = $"{dllPath} /win64 /out:{dllPath}.tlbexp.tlb";
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;

            process.Start();

            process.WaitForExit();
        }

        var options = new TypeLibTextConverterSettings()
        {
            Out = $"{dllPath}.tlbexp.tlb.yaml",
            TypeLibrary = $"{dllPath}.tlbexp.tlb"
        };

        if (File.Exists(options.TypeLibrary))
        {
            var typeLibConvert = new TypeLibConverter();
            typeLibConvert.ConvertTypeLibToText(options);
        }

#endif
    }

#if NETFRAMEWORK
    private ModuleBuilder CreateModuleBuilder()
    {
        return AssemblyBuilder.DefineDynamicModule("DynamicTestModule", $"{Path.GetFileName($"{TypeLibPath}.dll")}");
    }
#else
    private ModuleBuilder CreateModuleBuilder()
    {
        return AssemblyBuilder.DefineDynamicModule("DynamicTestModule"); ;
    }
#endif
}
