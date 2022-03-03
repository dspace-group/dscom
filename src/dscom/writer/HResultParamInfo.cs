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

internal class HResultParamInfo : ParameterInfo
{
    public HResultParamInfo()
    {
        ClassImpl = typeof(int);
        AttrsImpl = ParameterAttributes.Out | ParameterAttributes.Retval;
    }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
        List<object> returnAttributes = new();
        if (attributeType == typeof(MarshalAsAttribute))
        {
            returnAttributes.Add(new MarshalAsAttribute(UnmanagedType.Error));
        }
        if (inherit)
        {
            returnAttributes.AddRange(base.GetCustomAttributes(attributeType, inherit));
        }
        return returnAttributes.ToArray();
    }

    public override object[] GetCustomAttributes(bool inherit)
    {
        List<object> returnAttributes = new();
        returnAttributes.AddRange(GetCustomAttributes(typeof(MarshalAsAttribute), false));
        if (inherit)
        {
            returnAttributes.AddRange(base.GetCustomAttributes(inherit));
        }
        return returnAttributes.ToArray();
    }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
        if (attributeType == typeof(MarshalAsAttribute))
        {
            return true;
        }
        if (inherit)
        {
            return base.IsDefined(attributeType, inherit);
        }
        return false;
    }

    public override bool HasDefaultValue => false;
}
