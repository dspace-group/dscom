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

namespace dSPACE.Runtime.InteropServices.Writer;

internal sealed class PropertySetMethodWriter : PropertyMethodWriter
{
    public PropertySetMethodWriter(InterfaceWriter interfaceWriter, MethodInfo methodInfo, WriterContext context, string methodName) : base(interfaceWriter, methodInfo, context, methodName)
    {
        InvokeKind = INVOKEKIND.INVOKE_PROPERTYPUT;
    }

    public override void Create()
    {
        try
        {
            if (MethodInfo.GetParameters().Any(p =>
             {
                 var type = p.ParameterType;

                 // Object is not 'IsClass'
                 if (type == typeof(object))
                 {
                     return true;
                 }

                 // every class except special ones (e.g. String) or SAFEARRAY
                 // are handles as references
                 if (type.IsClass && !type.IsSpecialHandledClass() && !type.IsArray)
                 {
                     return true;
                 }

                 // interfaces are same as classes
                 if (type.IsInterface)
                 {
                     return true;
                 }

                 return false;
             }))
            {
                InvokeKind = INVOKEKIND.INVOKE_PROPERTYPUTREF;
            }
        }
        catch (FileNotFoundException)
        {
            //FileNotFoundException can occur on GetParameters() method, disable method writer
        }

        base.Create();
    }

    protected override List<string> GetNamesForParameters()
    {
        var names = base.GetNamesForParameters();

        if (names.Count > 1 && string.Equals(names[names.Count - 1], "value", StringComparison.OrdinalIgnoreCase))
        {
            names = names.GetRange(0, names.Count - 1);
        }

        return names;
    }

    protected override short GetParametersCount()
    {
        return (short)MethodInfo.GetParameters().Length;
    }
}
