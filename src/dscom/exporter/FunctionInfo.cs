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

using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.Exporter;

internal sealed class FunctionInfo : BaseInfo
{
    public FunctionInfo(ITypeInfo2 typeInfo, FUNCDESC funcDesc, BaseInfo? parent, string itemName) : base(parent, itemName)
    {
        typeInfo.GetDocumentation(funcDesc.memid, out _, out _, out _, out _);
        var names = new string[funcDesc.cParams + 1];
        typeInfo.GetNames(funcDesc.memid, names, funcDesc.cParams + 1, out _);
        Name = names[0];

        MemberId = funcDesc.memid;
        CallConv = funcDesc.callconv;
        Kind = funcDesc.funckind.ToString();
        InvokeKind = funcDesc.invkind.ToString();
        FunctionFlags = funcDesc.wFuncFlags;
        NumberOfParameters = funcDesc.cParams;
        VTableOffset = funcDesc.oVft;

        var ptr = funcDesc.lprgelemdescParam;

        var size = Marshal.SizeOf<ELEMDESC>();
        for (var i = 0; i < NumberOfParameters; i++)
        {
            var parameter = Marshal.PtrToStructure<ELEMDESC>(ptr);
            Parameters.Add(new ElementDescriptionInfo(typeInfo, parameter, names[i + 1], this, nameof(Parameters)) { OwningCollection = Parameters });
            ptr += size;
        }

        if (VarEnum.VT_EMPTY != (VarEnum)funcDesc.elemdescFunc.tdesc.vt)
        {
            ReturnType = new ElementDescriptionInfo(typeInfo, funcDesc.elemdescFunc, string.Empty, this, nameof(ReturnType));
        }

        UpdateCustomData(typeInfo);
    }

    public string Name { get; private set; }

    public int MemberId { get; }

    public CALLCONV CallConv { get; private set; }

    public string Kind { get; private set; }

    public string InvokeKind { get; private set; }

    public short FunctionFlags { get; private set; }

    public short NumberOfParameters { get; private set; }

    public short VTableOffset { get; private set; }

    public ElementDescriptionInfo? ReturnType { get; private set; }

    public List<ElementDescriptionInfo> Parameters { get; } = new List<ElementDescriptionInfo>();

    public List<CustomDataItemInfo> CustomData { get; } = new List<CustomDataItemInfo>();

    private void UpdateCustomData(ITypeInfo2 typeInfo)
    {
        var customDataPtr = Marshal.AllocHGlobal(Marshal.SizeOf<CUSTDATA>());
        try
        {
            typeInfo.GetAllCustData(customDataPtr);

            var customData = Marshal.PtrToStructure<CUSTDATA>(customDataPtr);

            for (var i = 0; i < customData.cCustData; i++)
            {
                CustomData.Add(new CustomDataItemInfo(customData.Items[i], this, nameof(CustomData)) { OwningCollection = CustomData });
            }
        }
        finally
        {
            Marshal.FreeHGlobal(customDataPtr);
        }
    }
}
