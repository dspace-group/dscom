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

using System.Globalization;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.Writer;

internal abstract class InterfaceWriter : TypeWriter
{
    public InterfaceWriter(Type sourceType, LibraryWriter libraryWriter, WriterContext context) : base(sourceType, libraryWriter, context)
    {
    }

    public DispatchIdCreator? DispatchIdCreator { get; protected set; }

    public int VTableOffsetUserMethodStart { get; set; } = 56;

    public ComInterfaceType ComInterfaceType { get; set; }

    public FUNCKIND FuncKind { get; protected set; }

    public abstract Guid BaseInterfaceGuid { get; }

    protected List<MethodWriter> MethodWriter { get; } = new();

    private ITypeInfo? BaseTypeInfo { get; set; }

    /// <summary>
    /// Gets or Sets a value indicating whether all method should return HRESULT as return value.
    /// </summary>
    internal bool UseHResultAsReturnValue { get; set; }

    public override void CreateTypeInheritance()
    {
        BaseTypeInfo = Context.TypeInfoResolver.ResolveTypeInfo(BaseInterfaceGuid);
        if (BaseTypeInfo != null)
        {
            TypeInfo.AddRefTypeInfo(BaseTypeInfo, out var phRefType)
                .ThrowIfFailed($"Failed to add IDispatch reference to {SourceType}.");
            TypeInfo.AddImplType(0, phRefType)
                .ThrowIfFailed($"Failed to add IDispatch implementation type to {SourceType}.");
        }
    }

    public override void Create()
    {
        DispatchIdCreator = new DispatchIdCreator(this, ComInterfaceType == ComInterfaceType.InterfaceIsIUnknown);
        Context.LogVerbose($"Creating {TypeKind} {TypeFlags} {FuncKind} {Name}");

        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(InterfaceWriter));
        }

        CreateMethodWriters();

        // Check all DispIDs
        // Handle special IDs like 0 or -4, and try to fix duplicate DispIds if possible.
        DispatchIdCreator.NormalizeIds();

        // This index is necessary to generate the correct offset of the VTable.
        // Every method must be considered, even those that cannot be generated.
        // var index = 0;

        // // The index of the function inside the type library
        // var functionIndex = 0;
        foreach (var item in MethodWriter)
        {
            // item.FunctionIndex = functionIndex;
            // item.VTableOffset = VTableOffsetUserMethodStart + (index * 8);
            item.Create();
            // functionIndex += item.IsValid ? 1 : 0;
            // index++;
        }

        TypeInfo.LayOut().ThrowIfFailed($"Failed to layout type {SourceType}.");
    }

    private void CreateMethodWriters()
    {
        var methods = SourceType.GetMethods().ToList();
        methods.Sort((a, b) => a.MetadataToken - b.MetadataToken);

        foreach (var method in methods)
        {
            var numIdenticalNames = MethodWriter.Count(z => z.IsValid && (z.MemberInfo.Name == method.Name || z.MethodName.StartsWith(method.Name + "_", StringComparison.Ordinal)));

            numIdenticalNames += GetMethodNamesOfBaseTypeInfo(BaseTypeInfo).Count(z => z == method.Name || z.StartsWith(method.Name + "_", StringComparison.Ordinal));

            var alternateName = numIdenticalNames == 0 ? method.Name : method.Name + "_" + (numIdenticalNames + 1).ToString(CultureInfo.InvariantCulture);
            MethodWriter? methodWriter = null;
            if ((method.Name.StartsWith("get_", StringComparison.Ordinal) || method.Name.StartsWith("set_", StringComparison.Ordinal)) && method.IsSpecialName)
            {
                alternateName = alternateName.Substring(4);
                if (method.Name.StartsWith("get_", StringComparison.Ordinal))
                {
                    methodWriter = new PropertyGetMethodWriter(this, method, Context, alternateName);
                }
                else
                if (method.Name.StartsWith("set_", StringComparison.Ordinal))
                {
                    methodWriter = new PropertySetMethodWriter(this, method, Context, alternateName);
                }
            }
            else
            {
                methodWriter = new(this, method, Context, alternateName);
            }
            if (methodWriter != null)
            {
                MethodWriter.Add(methodWriter);
            }
        }

        var index = 0;
        var functionIndex = 0;
        foreach (var methodWriter in MethodWriter)
        {
            methodWriter.FunctionIndex = functionIndex;
            methodWriter.VTableOffset = VTableOffsetUserMethodStart + (index * 8);
            DispatchIdCreator!.RegisterMember(methodWriter);
            functionIndex += methodWriter.IsValid ? 1 : 0;
            index++;
        }
    }

    private static IEnumerable<string> GetMethodNamesOfBaseTypeInfo(ITypeInfo? typeInfo)
    {
        var names = new List<string>();
        if (typeInfo != null)
        {
            var ppTypeAttr = new IntPtr();
            try
            {
                typeInfo.GetTypeAttr(out ppTypeAttr);
                var typeAttr = Marshal.PtrToStructure<TYPEATTR>(ppTypeAttr);

                for (var i = 0; i < typeAttr.cFuncs; i++)
                {
                    typeInfo.GetFuncDesc(i, out var ppFuncDesc);
                    var funcDesc = Marshal.PtrToStructure<FUNCDESC>(ppFuncDesc);
                    typeInfo.GetDocumentation(funcDesc.memid, out var name, out _, out _, out _);
                    names.Add(name);
                }

                for (var i = 0; i < typeAttr.cImplTypes; i++)
                {
                    typeInfo.GetRefTypeOfImplType(i, out var href);
                    typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
                    names.AddRange(GetMethodNamesOfBaseTypeInfo(refTypeInfo));
                }
            }
            finally
            {
                typeInfo.ReleaseTypeAttr(ppTypeAttr);
            }
        }

        return names;
    }


    protected override void Dispose(bool disposing)
    {
        if (MethodWriter != null)
        {
            MethodWriter.ForEach(t => t.Dispose());
            MethodWriter.Clear();
        }

        base.Dispose(disposing);
    }
}
