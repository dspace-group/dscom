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

namespace dSPACE.Runtime.InteropServices.Tests;

public class RegistrationServices : BaseTest
{


    // var h = new Guid("00000001-0000-0000-C000-000000000046");
    // var f = CoGetClassObject(guid, CLSCTX.CLSCTX_LOCAL_SERVER, IntPtr.Zero, h) as IClassFactory;
    // f.CreateInstance(null, interfaceGuid, out IntPtr ptr);

    // var h3 = Marshal.GetObjectForIUnknown(ptr) as IGreeter;

    // var h4 = h3.SayHello("lkl");

    // hr = CoResumeClassObjects();
    // if (hr < 0)
    // {
    //     Marshal.ThrowExceptionForHR(hr);
    // }


    [Fact]
    public void ITestClassRegister()
    {
    }
}
