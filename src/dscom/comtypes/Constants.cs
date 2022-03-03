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

namespace dSPACE.Runtime.InteropServices;

public static class Constants
{
    public const string Kernel32 = "kernel32.dll";

    public const string OleAut32 = "oleaut32.dll";

    public const int LCID_NEUTRAL = 0;

    public const uint BASE_OLEAUT_DISPID = 0x60020000;

    public const uint BASE_OLEAUT_IUNKNOWN = 0x60010000;

    // [Windows SDK]\um\oaidl.h" 
    // DISPID reserved for the "value" property
    public const uint DISPIP_VALUE = 0x0;

    // DISPID reserved for the standard "NewEnum" method
    public const uint DISPID_NEWENUM = 0xfffffffc;

    // DISPID reserved to indicate an "unknown" name
    // only reserved for data members (properties); reused as a method dispid below
    public const uint DISPID_UNKNWN = 0xffffffff;
}
