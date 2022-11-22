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

internal sealed class TypelLibInfo : BaseInfo
{
    public TypelLibInfo(string file) : base(null, string.Empty)
    {
        if (!System.IO.File.Exists(file))
        {
            throw new FileNotFoundException($"File {file} not found.");
        }

        File = file;

        var hresult = OleAut32.LoadTypeLibEx(file, REGKIND.NONE, out var typelib);
        hresult.ThrowIfFailed();

        if (typelib is not ITypeLib2 typelib2)
        {
            throw new COMException("Unable to cast to ITypeLib2 interface.");
        }

        Attributes = new TypeLibAttributesInfo(typelib2, this, nameof(Attributes));

        UpdateDocumentation(typelib2);
        UpdateCustomData(typelib2);
        UpdatesTypes(typelib);
    }

    public string Name { get; private set; } = string.Empty;

    public string DocString { get; set; } = string.Empty;

    public int HelpContext { get; set; }

    public string HelpFile { get; set; } = string.Empty;

    public TypeLibAttributesInfo? Attributes { get; set; }

    public string File { get; set; }

    public List<CustomDataItemInfo> CustomData { get; } = new List<CustomDataItemInfo>();

    public List<TypeInfo> Types { get; } = new List<TypeInfo>();

    private void UpdatesTypes(ITypeLib typelib)
    {
        var count = typelib.GetTypeInfoCount();

        for (var i = 0; i < count; i++)
        {
            typelib.GetTypeInfo(i, out var typeInfo);
            var typeInfo2 = (ITypeInfo2)typeInfo;
            Types.Add(new TypeInfo(typeInfo2, this, nameof(Types)) { OwningCollection = Types });
        }
    }

    private void UpdateDocumentation(ITypeLib2 typelib2)
    {
        typelib2.GetDocumentation(-1, out var typeLibName, out var docString, out var helpContext, out var helpFile);

        Name = typeLibName;
        DocString = docString;
        HelpContext = helpContext;
        HelpFile = helpFile;
    }

    private void UpdateCustomData(ITypeLib2 typelib2)
    {
        var customDataPtr = Marshal.AllocHGlobal(Marshal.SizeOf<CUSTDATA>());
        try
        {
            typelib2.GetAllCustData(customDataPtr);
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
