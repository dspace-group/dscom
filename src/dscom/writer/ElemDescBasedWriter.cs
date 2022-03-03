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
using dSPACE.Runtime.InteropServices.ComTypes;

namespace dSPACE.Runtime.InteropServices.Writer;

internal class ElemDescBasedWriter : BaseWriter
{
    public ElemDescBasedWriter(Type type, ICustomAttributeProvider attributeProvider, Type parentType, ICreateTypeInfo2 typeInfo, WriterContext context) : base(context)
    {
        Type = type;
        ParentType = parentType;
        TypeProvider = new TypeProvider(context, attributeProvider, false);
        TypeInfo = typeInfo;
    }

    private ICreateTypeInfo2 TypeInfo { get; set; }

    protected bool _isValid = true;

    protected Type Type { get; set; }

    protected Type ParentType { get; set; }

    public ELEMDESC ElementDescription => _elementDescription;

    protected ELEMDESC _elementDescription;

    protected TypeProvider TypeProvider { get; set; }

    public override void Create()
    {
        //calculate effective type
        var lowLevel = TypeProvider.GetVariantType(Type.IsByRef ? Type.GetElementType()! : Type, out var highLevel, false);
        if (lowLevel != VarEnum.VT_EMPTY)
        {
            VarEnum? midLevel = null;
            //safe array or lparray
            if (((lowLevel is VarEnum.VT_SAFEARRAY) || (lowLevel is VarEnum.VT_PTR)) && (Type.IsArray || (Type.IsByRef && Type.GetElementType()!.IsArray)))
            {
                var arrayType = Type.IsByRef ? Type.GetElementType()!.GetElementType()! : Type.GetElementType()!;
                highLevel = lowLevel;
                lowLevel = TypeProvider.GetVariantType(arrayType, out midLevel, true);
            }
            if (lowLevel != VarEnum.VT_EMPTY)
            {
                if (midLevel != null)
                {
                    _elementDescription.tdesc = GetTypeDescription(lowLevel, midLevel);
                    _elementDescription.tdesc = new TYPEDESC()
                    {
                        vt = (short)highLevel!,
                        lpValue = StructureToPtr(_elementDescription.tdesc)
                    };
                }
                else
                {
                    _elementDescription.tdesc = GetTypeDescription(lowLevel, highLevel);
                }
            }
            else
            {
                _isValid = false;
            }
        }
        else
        {
            _isValid = false;
        }
    }

    protected VarEnum GetVariantType(Type type, bool isSafeArraySubType = false)
    {
        var retval = TypeProvider.GetVariantType(type, out _, isSafeArraySubType);
        if (retval == VarEnum.VT_EMPTY)
        {
            _isValid = false;
        }

        return retval;
    }

    protected TYPEDESC GetTypeDescription(VarEnum elementVarDesc, VarEnum? parentVarDesc)
    {
        var typeDesc = GetTypeDescription(elementVarDesc);
        if (parentVarDesc != null)
        {
            typeDesc = new TYPEDESC()
            {
                vt = (short)parentVarDesc,
                lpValue = StructureToPtr(typeDesc)
            };
        }
        return typeDesc;
    }

    protected TYPEDESC GetTypeDescription(VarEnum elementVarDesc)
    {
        Type? effectiveType = null;
        ITypeInfo? effectiveTypeInfo = null;
        switch (elementVarDesc)
        {
            case VarEnum.VT_UNKNOWN:
                effectiveType = typeof(IUnknown);
                break;
            case VarEnum.VT_DISPATCH:
                effectiveType = typeof(IDispatch);
                break;
            case VarEnum.VT_USERDEFINED:
                effectiveType = Type.GetUnderlayingType();
                effectiveTypeInfo = Context.TypeInfoResolver.ResolveTypeInfo(effectiveType);

                if ((Type.IsInterface || Type.IsClass) && effectiveTypeInfo == null)
                {
                    // Try to find the default interface
                    var defaultInterface = Context.TypeInfoResolver.ResolveDefaultCoClassInterface(effectiveType);
                    if (defaultInterface == null)
                    {
                        //IUnknown substitution
                        elementVarDesc = VarEnum.VT_UNKNOWN;
                        effectiveType = typeof(IUnknown);
                    }
                    effectiveTypeInfo = defaultInterface;
                }
                break;
        }

        var typeDesc = new TYPEDESC()
        {
            vt = (short)elementVarDesc
        };


        if (effectiveTypeInfo != null)
        {
            var typeInfo = effectiveTypeInfo;
            TypeInfo.AddRefTypeInfo(typeInfo, out var refTypeInfo)
                .ThrowIfFailed($"Failed to add reference type for {effectiveType}");
            typeDesc.lpValue = new IntPtr(refTypeInfo);
        }

        return typeDesc;
    }
}
