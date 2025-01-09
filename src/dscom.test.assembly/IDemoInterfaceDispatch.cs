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

namespace dSPACE.Runtime.InteropServices.Test;

[ComVisible(true)]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
public interface IDemoInterfaceDispatch
{
    public byte MybyteProperty { get; set; }
    public sbyte MysbyteProperty { get; set; }
    public short MyshortProperty { get; set; }
    public ushort MyushortProperty { get; set; }
    public uint MyuintProperty { get; set; }
    public int MyintProperty { get; set; }
    public ulong MyulongProperty { get; set; }
    public long MylongProperty { get; set; }
    public float MySingleProperty { get; set; }
    public double MydoubleProperty { get; set; }
    public string MystringProperty { get; set; }
    public bool MyboolProperty { get; set; }
    public char MycharProperty { get; set; }
    public object MyobjectProperty { get; set; }
    public object[] MyobjectArrayProperty { get; set; }
    public System.Collections.IEnumerator MySystemCollectionsIEnumeratorProperty { get; set; }
    public DateTime MySystemDateTimeProperty { get; set; }
    public Guid MySystemGuidProperty { get; set; }
    public System.Drawing.Color MySystemDrawingColorProperty { get; set; }
    public decimal MySystemDecimalProperty { get; set; }

    public byte MybyteGetterProperty { get; }
    public sbyte MysbyteGetterProperty { get; }
    public short MyshortGetterProperty { get; }
    public ushort MyushortGetterProperty { get; }
    public uint MyuintGetterProperty { get; }
    public int MyintGetterProperty { get; }
    public ulong MyulongGetterProperty { get; }
    public long MylongGetterProperty { get; }
    public float MySingleGetterProperty { get; }
    public double MydoubleGetterProperty { get; }
    public string MystringGetterProperty { get; }
    public bool MyboolGetterProperty { get; }
    public char MycharGetterProperty { get; }
    public object MyobjectGetterProperty { get; }
    public object[] MyobjectArrayGetterProperty { get; }
    public System.Collections.IEnumerator MySystemCollectionsIEnumeratorGetterProperty { get; }
    public DateTime MySystemDateTimeGetterProperty { get; }
    public Guid MySystemGuidGetterProperty { get; }
    public System.Drawing.Color MySystemDrawingColorGetterProperty { get; }
    public decimal MySystemDecimalGetterProperty { get; }

    public byte MybyteSetterProperty { set; }
    public sbyte MysbyteSetterProperty { set; }
    public short MyshortSetterProperty { set; }
    public ushort MyushortSetterProperty { set; }
    public uint MyuintSetterProperty { set; }
    public int MyintSetterProperty { set; }
    public ulong MyulongSetterProperty { set; }
    public long MylongSetterProperty { set; }
    public float MySingleSetterProperty { set; }
    public double MydoubleSetterProperty { set; }
    public string MystringSetterProperty { set; }
    public bool MyboolSetterProperty { set; }
    public char MycharSetterProperty { set; }
    public object MyobjectSetterProperty { set; }
    public object[] MyobjectArraySetterProperty { set; }
    public System.Collections.IEnumerator MySystemCollectionsIEnumeratorSetterProperty { set; }
    public DateTime MySystemDateTimeSetterProperty { set; }
    public Guid MySystemGuidSetterProperty { set; }
    public System.Drawing.Color MySystemDrawingColorSetterProperty { set; }
    public decimal MySystemDecimalSetterProperty { set; }

    public void MyvoidMethod();
    public byte MybyteMethod();
    public sbyte MysbyteMethod();
    public short MyshortMethod();
    public ushort MyushortMethod();
    public uint MyuintMethod();
    public int MyintMethod();
    public ulong MyulongMethod();
    public long MylongMethod();
    public float MySingleMethod();
    public double MydoubleMethod();
    public string MystringMethod();
    public bool MyboolMethod();
    public char MycharMethod();
    public object MyobjectMethod();
    public object[] MyobjectArrayMethod();
    public System.Collections.IEnumerator MySystemCollectionsIEnumeratorMethod();
    public DateTime MySystemDateTimeMethod();
    public Guid MySystemGuidMethod();
    public System.Drawing.Color MySystemDrawingColorMethod();
    public decimal MySystemDecimalMethod();

    public void MyvoidMethod1Param(int param1);
    public byte MybyteMethod1Param(byte param1);
    public sbyte MysbyteMethod1Param(sbyte param1);
    public short MyshortMethod1Param(short param1);
    public ushort MyushortMethod1Param(ushort param1);
    public uint MyuintMethod1Param(uint param1);
    public int MyintMethod1Param(int param1);
    public ulong MyulongMethod1Param(ulong param1);
    public long MylongMethod1Param(long param1);
    public float MySingleMethod1Param(float param1);
    public double MydoubleMethod1Param(double param1);
    public string MystringMethod1Param(string param1);
    public bool MyboolMethod1Param(bool param1);
    public char MycharMethod1Param(char param1);
    public object MyobjectMethod1Param(object param1);
    public object[] MyobjectArrayMethod1Param(object[] param1);
    public System.Collections.IEnumerator MySystemCollectionsIEnumeratorMethod1Param(System.Collections.IEnumerator param1);
    public DateTime MySystemDateTimeMethod1Param(DateTime param1);
    public Guid MySystemGuidMethod1Param(Guid param1);
    public System.Drawing.Color MySystemDrawingColorMethod1Param(System.Drawing.Color param1);
    public decimal MySystemDecimalMethod1Param(decimal param1);

    public void MyvoidMethod2Param(int param1, int param2);
    public byte MybyteMethod2Param(byte param1, byte param2);
    public sbyte MysbyteMethod2Param(sbyte param1, sbyte param2);
    public short MyshortMethod2Param(short param1, short param2);
    public ushort MyushortMethod2Param(ushort param1, ushort param2);
    public uint MyuintMethod2Param(uint param1, uint param2);
    public int MyintMethod2Param(int param1, int param2);
    public ulong MyulongMethod2Param(ulong param1, ulong param2);
    public long MylongMethod2Param(long param1, long param2);
    public float MySingleMethod2Param(float param1, float param2);
    public double MydoubleMethod2Param(double param1, double param2);
    public string MystringMethod2Param(string param1, string param2);
    public bool MyboolMethod2Param(bool param1, bool param2);
    public char MycharMethod2Param(char param1, char param2);
    public object MyobjectMethod2Param(object param1, object param2);
    public object[] MyobjectArrayMethod2Param(object[] param1, object[] param2);
    public System.Collections.IEnumerator MySystemCollectionsIEnumeratorMethod2Param(System.Collections.IEnumerator param1, System.Collections.IEnumerator param2);
    public DateTime MySystemDateTimeMethod2Param(DateTime param1, DateTime param2);
    public Guid MySystemGuidMethod2Param(Guid param1, Guid param2);
    public System.Drawing.Color MySystemDrawingColorMethod2Param(System.Drawing.Color param1, System.Drawing.Color param2);
    public decimal MySystemDecimalMethod2Param(decimal param1, decimal param2);
}
