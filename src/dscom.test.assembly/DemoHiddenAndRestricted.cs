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
public interface IDemoInterfaceVisible
{
    void DoSomething(IDemoInterfaceVisible param);
}

[
    ComVisible(true),
    Hidden
]
public interface IDemoInterfaceHiddenImplicit
{
    void DoSomething(IDemoInterfaceHiddenImplicit param);
}

[
    ComVisible(true),
    Hidden(true)
]
public interface IDemoInterfaceHidden
{
    void DoSomething(IDemoInterfaceHidden param);
}

[
    ComVisible(true),
    Hidden(true)
]
public interface IDemoInterfaceVisibleExplicit
{
    void DoSomething(IDemoInterfaceVisibleExplicit param);
}

[
    ComVisible(true),
    Restricted
]
public interface IDemoInterfaceRestrictedImplicit
{
    void DoSomething(IDemoInterfaceRestrictedImplicit param);
}

[
    ComVisible(true),
    Restricted(true)
]
public interface IDemoInterfaceRestricted
{
    void DoSomething(IDemoInterfaceRestricted param);
}

[
    ComVisible(true),
    Restricted(true)
]
public interface IDemoInterfaceusableExplicit
{
    void DoSomething(IDemoInterfaceusableExplicit param);
}

[
    ComVisible(true),
    Hidden,
    Restricted
]
public interface IDemoInterfaceHiddeAndRestricted
{
    void DoSomething(IDemoInterfaceHiddeAndRestricted param);
}
