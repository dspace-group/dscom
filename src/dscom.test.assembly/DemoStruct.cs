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
using System.Drawing;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.Test;

[ComVisible(true)]
[StructLayout(LayoutKind.Sequential)]
public struct DemoStruct
{
    public byte System_Byte;
    public sbyte System_SByte;
    public short System_Int16;
    public ushort System_UInt16;
    public uint System_UInt32;
    public int System_Int32;
    public ulong System_UInt64;
    public long System_Int64;
    public IntPtr System_IntPtr;
    public float System_Single;
    public double System_Double;
    public string System_String;
    public bool System_Boolean;
    public char System_Char;
    public object System_Object;
    public object[] System_Object__;
    public IEnumerator System_Collections_IEnumerator;
    public DateTime System_DateTime;
    public Guid System_Guid;
    public Color System_Drawing_Color;
    public decimal System_Decimal;
}
