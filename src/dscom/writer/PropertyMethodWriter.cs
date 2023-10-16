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

internal class PropertyMethodWriter : MethodWriter
{
    private readonly PropertyInfo? _propertyInfo;
    public PropertyMethodWriter(InterfaceWriter interfaceWriter, MethodInfo methodInfo, WriterContext context, string methodName) : base(interfaceWriter, methodInfo, context, methodName)
    {
        _propertyInfo = methodInfo.DeclaringType!.GetProperties().First(p => p.GetGetMethod() == methodInfo || p.GetSetMethod() == methodInfo);
        MemberInfo = _propertyInfo!;
    }

    private bool? _isComVisible;

    protected override bool IsComVisible
    {
        get
        {
            if (_isComVisible != null)
            {
                return _isComVisible.Value;
            }

            var propertyAttribute = MemberInfo.GetCustomAttribute<ComVisibleAttribute>();
            _isComVisible = (propertyAttribute == null || propertyAttribute.Value) && base.IsComVisible;
            return _isComVisible.Value;
        }
    }

    protected override List<string> GetNamesForParameters()
    {
        var names = new List<string>();
        var propertyName = GetPropertyName();
        propertyName = _propertyInfo != null ? Context.NameResolver.GetMappedName(_propertyInfo, propertyName) : null;
        names.Add(propertyName!);

        if (InvokeKind == INVOKEKIND.INVOKE_PROPERTYGET)
        {
            MethodInfo.GetParameters().ToList().ForEach(p => names.Add(Context.NameResolver.GetMappedName(p, p.Name ?? string.Empty) ?? string.Empty));
        }

        if (InvokeKind == INVOKEKIND.INVOKE_PROPERTYPUTREF)
        {
            MethodInfo.GetParameters().Where(p => !string.IsNullOrEmpty(p.Name)).ToList().ForEach(p => names.Add(Context.NameResolver.GetMappedName(p, p.Name ?? string.Empty) ?? string.Empty));
        }

        if (UseHResultAsReturnValue && InvokeKind == INVOKEKIND.INVOKE_PROPERTYGET)
        {
            names.Add("pRetVal");
        }

        return names;
    }

    private string GetPropertyName()
    {
        return MethodInfo.Name.Substring(MethodInfo.Name.IndexOf('_') + 1);
    }
}
