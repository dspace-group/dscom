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

namespace dSPACE.Runtime.InteropServices.Test;

public class DemoClass
{
    private string _value = "test";

    public void MyvoidMethod2Param(int param1, int param2) { Console.WriteLine($"{_value} {param1} {param2}"); }
    public byte MybyteMethod2Param(byte param1, byte param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public sbyte MysbyteMethod2Param(sbyte param1, sbyte param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public short MyshortMethod2Param(short param1, short param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public ushort MyushortMethod2Param(ushort param1, ushort param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public uint MyuintMethod2Param(uint param1, uint param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public int MyintMethod2Param(int param1, int param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public ulong MyulongMethod2Param(ulong param1, ulong param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public long MylongMethod2Param(long param1, long param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public float MySingleMethod2Param(float param1, float param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public double MydoubleMethod2Param(double param1, double param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public string? MystringMethod2Param(string param1, string param2) { _value = param1; Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public bool MyboolMethod2Param(bool param1, bool param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public char MycharMethod2Param(char param1, char param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public object? MyobjectMethod2Param(object param1, object param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public object[]? MyobjectArrayMethod2Param(object[] param1, object[] param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public System.Collections.IEnumerator? MySystemCollectionsIEnumeratorMethod2Param(System.Collections.IEnumerator param1, System.Collections.IEnumerator param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public DateTime MySystemDateTimeMethod2Param(DateTime param1, DateTime param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public Guid MySystemGuidMethod2Param(Guid param1, Guid param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public System.Drawing.Color MySystemDrawingColorMethod2Param(System.Drawing.Color param1, System.Drawing.Color param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
    public decimal MySystemDecimalMethod2Param(decimal param1, decimal param2) { Console.WriteLine($"{_value} {param1} {param2}"); return default; }
}
