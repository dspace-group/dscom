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

using System.Collections;
using System.Reflection;
using dSPACE.Runtime.InteropServices.Writer;

namespace dSPACE.Runtime.InteropServices;

internal sealed class DispatchIdCreator
{
    private readonly List<IDInfo> _dispIds = new();

    private uint _currentDispId = Constants.BASE_OLEAUT_DISPID;

    public DispatchIdCreator(InterfaceWriter interfaceWriter)
    {
        InterfaceWriter = interfaceWriter;

        // In case of IUnknown we start with Constants.BASE_OLEAUT_IUNKNOWN for the first DispId
        if (interfaceWriter.BaseInterfaceGuid == new Guid(Guids.IID_IUnknown))
        {
            _currentDispId = Constants.BASE_OLEAUT_IUNKNOWN;
        }
    }

    public WriterContext Context => InterfaceWriter.Context;

    public InterfaceWriter InterfaceWriter { get; }

    public uint GetDispatchId(MethodWriter methodWriter)
    {
        return _dispIds.First(m => m.MemberInfo.Equals(methodWriter.MemberInfo)).Id;
    }

    public void RegisterMember(MethodWriter methodWriter)
    {
        var memberInfo = methodWriter.MemberInfo;
        var name = methodWriter.MemberInfo.Name;

        var dispId = GetNextFreeDispId();
        while (_dispIds.Any(x => x.ExplicitId == dispId || x.Id == dispId))
        {
            dispId = GetNextFreeDispId();
        }

        var dispIdAttribute = memberInfo.GetCustomAttribute<System.Runtime.InteropServices.DispIdAttribute>();
        var defaultMemberAttribute = memberInfo.DeclaringType!.GetCustomAttribute<DefaultMemberAttribute>();

        if (dispIdAttribute != null)
        {
            AddDispatchId(dispId, (uint)dispIdAttribute!.Value, memberInfo);
        }
        else if (defaultMemberAttribute != null && string.Equals(defaultMemberAttribute.MemberName, methodWriter.MemberInfo.Name, StringComparison.Ordinal))
        {
            AddDispatchId(dispId, Constants.DISPIP_VALUE, memberInfo);
        }
        else
        {
            AddDispatchId(dispId, null, memberInfo);
        }
    }

    private void AddDispatchId(uint idGenerated, uint? idFromAttribute, MemberInfo memberInfo)
    {
        if (!_dispIds.Any(z => z.MemberInfo == memberInfo))
        {
            _dispIds.Add(new(idGenerated, idFromAttribute, memberInfo));
        }
    }

    private uint GetNextFreeDispId()
    {
        var dispId = _currentDispId;
        _currentDispId++;
        return dispId;
    }

    internal void NormalizeIds()
    {
        var getEnumeratorItem = _dispIds.FirstOrDefault(z => z.MemberInfo.Name.Equals("GetEnumerator", StringComparison.Ordinal));
        if (getEnumeratorItem != null && getEnumeratorItem.MemberInfo is MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType == typeof(IEnumerator) && !getEnumeratorItem.ExplicitId.HasValue)
            {
                getEnumeratorItem.ExplicitId = Constants.DISPID_NEWENUM;
            }
        }

        // Remove all duplicate explicit DispIds and send a notification
        foreach (var kv in _dispIds.Where(i => i.ExplicitId.HasValue))
        {
            var elements = _dispIds.Where(i => i.ExplicitId == kv.ExplicitId);
            if (elements.Count() > 1)
            {
                elements.ToList().ForEach(i => i.ExplicitId = null);
                Context.LogWarning($"Type library exporter warning processing '{kv.MemberInfo.DeclaringType!.Name}'. Warning: The type had one or more duplicate DISPIDs specified. The duplicate DISPIDs were ignored.", HRESULT.TYPE_E_DUPLICATEID);
            }
        }

        // If DispId == 0 not used by any member, Value should take the 0 (if the DispId for the Value property is not explicit defined).
        var valueItem = _dispIds.FirstOrDefault(z => z.MemberInfo.Name.Equals("Value", StringComparison.Ordinal));
        if (valueItem != null)
        {
            if (!_dispIds.Any(i => i.Id == 0) && !valueItem.ExplicitId.HasValue)
            {
                valueItem.ExplicitId = Constants.DISPIP_VALUE;
            }
        }
    }

    public sealed class IDInfo
    {
        public IDInfo(uint generatedId, uint? explicitId, MemberInfo memberInfo)
        {
            GeneratedId = generatedId;
            ExplicitId = explicitId;
            MemberInfo = memberInfo;
        }

        public uint GeneratedId { get; set; }

        public uint? ExplicitId { get; set; }

        public MemberInfo MemberInfo { get; set; }

        public uint Id => ExplicitId ?? GeneratedId;
    }
}
