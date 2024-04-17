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
using System.Reflection;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.Writer;

internal abstract class InterfaceWriter : TypeWriter
{
    public InterfaceWriter(Type sourceType, LibraryWriter libraryWriter, WriterContext context) : base(sourceType, libraryWriter, context)
    {
        VTableOffsetUserMethodStart = 7 * IntPtr.Size;
    }

    public DispatchIdCreator? DispatchIdCreator { get; protected set; }

    public int VTableOffsetUserMethodStart { get; set; }

    public ComInterfaceType ComInterfaceType { get; set; }

    public FUNCKIND FuncKind { get; protected set; }

    public abstract Guid BaseInterfaceGuid { get; }

    protected List<MethodWriter?> MethodWriters { get; } = new();

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
        DispatchIdCreator = new DispatchIdCreator(this);
        Context.LogTypeExported($"Interface '{Name}' exported.");

        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(InterfaceWriter));
        }

        CreateMethodWriters();

        // Check all DispIDs
        // Handle special IDs like 0 or -4, and try to fix duplicate DispIds if possible.
        DispatchIdCreator.NormalizeIds();

        // Create all writer.
        MethodWriters.ForEach(writer => writer?.Create());

        TypeInfo.LayOut().ThrowIfFailed($"Failed to layout type {SourceType}.");
    }

    private void CreateMethodWriters()
    {
        var methods = SourceType.GetMethods().ToList();
        methods.Sort((a, b) => a.MetadataToken - b.MetadataToken);

        foreach (var method in methods)
        {
            var numIdenticalNames = MethodWriters.Count(z => z is not null && z.IsVisibleMethod && (z.MemberInfo.Name == method.Name || z.MethodName.StartsWith(method.Name + "_", StringComparison.Ordinal)));

            numIdenticalNames += GetMethodNamesOfBaseTypeInfo(BaseTypeInfo).Count(z => z == method.Name || z.StartsWith(method.Name + "_", StringComparison.Ordinal));

            var alternateName = numIdenticalNames == 0 ? method.Name : method.Name + "_" + (numIdenticalNames + 1).ToString(CultureInfo.InvariantCulture);
            MethodWriter? methodWriter = null;
            if ((method.Name.StartsWith("get_", StringComparison.Ordinal) || method.Name.StartsWith("set_", StringComparison.Ordinal)) && method.IsSpecialName)
            {
                var propertyInfo = method.DeclaringType!.GetProperties().First(p => p.GetGetMethod() == method || p.GetSetMethod() == method);
                var comVisibleAttribute = propertyInfo.GetCustomAttribute<ComVisibleAttribute>();

                if (comVisibleAttribute is null || comVisibleAttribute.Value)
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
            }
            else
            {
                var comVisibleAttribute = method.GetCustomAttribute<ComVisibleAttribute>();
                if (comVisibleAttribute is null || comVisibleAttribute.Value)
                {
                    methodWriter = new(this, method, Context, alternateName);
                }
            }

            MethodWriters.Add(methodWriter);
        }

        var index = 0;
        var functionIndex = 0;
        foreach (var methodWriter in MethodWriters)
        {
            // A methodWriter can be null if ComVisible is false
            if (methodWriter is not null)
            {
                methodWriter.FunctionIndex = functionIndex;
                methodWriter.VTableOffset = VTableOffsetUserMethodStart + (index * IntPtr.Size);
                DispatchIdCreator!.RegisterMember(methodWriter);
                functionIndex += methodWriter.IsValid ? 1 : 0;
            }
            else
            {
                // In case of ComVisible false, we need to increment the the next free DispId
                // This offset is needed to keep the DispId in sync with the VTable and the behavior of tlbexp.
                DispatchIdCreator!.GetNextFreeDispId();
            }

            // Increment the index for the VTableOffset
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
        if (MethodWriters != null)
        {
            MethodWriters.ForEach(t => t?.Dispose());
            MethodWriters.Clear();
        }

        base.Dispose(disposing);
    }
}
