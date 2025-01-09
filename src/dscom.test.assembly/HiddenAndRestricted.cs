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
using dSPACE.Runtime.InteropServices.Attributes;

namespace dSPACE.Runtime.InteropServices.Test;

[ComVisible(true)]
public interface ITestInterfaceVisible
{
    void DoSomething(ITestInterfaceVisible param);
}

[
    ComVisible(true),
    Hidden
]
public interface ITestInterfaceHiddenImplicit
{
    void DoSomething(ITestInterfaceHiddenImplicit param);
}

[
    ComVisible(true),
    Hidden(true)
]
public interface ITestInterfaceHidden
{
    void DoSomething(ITestInterfaceHidden param);
}

[
    ComVisible(true),
    Hidden(true)
]
public interface ITestInterfaceVisibleExplicit
{
    void DoSomething(ITestInterfaceVisibleExplicit param);
}

[
    ComVisible(true),
    Restricted
]
public interface ITestInterfaceRestrictedImplicit
{
    void DoSomething(ITestInterfaceRestrictedImplicit param);
}

[
    ComVisible(true),
    Restricted(true)
]
public interface ITestInterfaceRestricted
{
    void DoSomething(ITestInterfaceRestricted param);
}

[
    ComVisible(true),
    Restricted(true)
]
public interface ITestInterfaceusableExplicit
{
    void DoSomething(ITestInterfaceusableExplicit param);
}

[
    ComVisible(true),
    Hidden,
    Restricted
]
public interface ITestInterfaceHiddeAndRestricted
{
    void DoSomething(ITestInterfaceHiddeAndRestricted param);
}
