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
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ITestInterfaceIUnknown
{
    byte MybyteProperty { get; set; }
    sbyte MysbyteProperty { get; set; }
    short MyshortProperty { get; set; }
    ushort MyushortProperty { get; set; }
    uint MyuintProperty { get; set; }
    int MyintProperty { get; set; }
    ulong MyulongProperty { get; set; }
    long MylongProperty { get; set; }
    float MySingleProperty { get; set; }
    double MydoubleProperty { get; set; }
    string MystringProperty { get; set; }
    bool MyboolProperty { get; set; }
    char MycharProperty { get; set; }
    object MyobjectProperty { get; set; }
    object[] MyobjectArrayProperty { get; set; }
    System.Collections.IEnumerator MySystemCollectionsIEnumeratorProperty { get; set; }
    DateTime MySystemDateTimeProperty { get; set; }
    Guid MySystemGuidProperty { get; set; }
    System.Drawing.Color MySystemDrawingColorProperty { get; set; }
    decimal MySystemDecimalProperty { get; set; }

    byte MybyteGetterProperty { get; }
    sbyte MysbyteGetterProperty { get; }
    short MyshortGetterProperty { get; }
    ushort MyushortGetterProperty { get; }
    uint MyuintGetterProperty { get; }
    int MyintGetterProperty { get; }
    ulong MyulongGetterProperty { get; }
    long MylongGetterProperty { get; }
    float MySingleGetterProperty { get; }
    double MydoubleGetterProperty { get; }
    string MystringGetterProperty { get; }
    bool MyboolGetterProperty { get; }
    char MycharGetterProperty { get; }
    object MyobjectGetterProperty { get; }
    object[] MyobjectArrayGetterProperty { get; }
    System.Collections.IEnumerator MySystemCollectionsIEnumeratorGetterProperty { get; }
    DateTime MySystemDateTimeGetterProperty { get; }
    Guid MySystemGuidGetterProperty { get; }
    System.Drawing.Color MySystemDrawingColorGetterProperty { get; }
    decimal MySystemDecimalGetterProperty { get; }

    byte MybyteSetterProperty { set; }
    sbyte MysbyteSetterProperty { set; }
    short MyshortSetterProperty { set; }
    ushort MyushortSetterProperty { set; }
    uint MyuintSetterProperty { set; }
    int MyintSetterProperty { set; }
    ulong MyulongSetterProperty { set; }
    long MylongSetterProperty { set; }
    float MySingleSetterProperty { set; }
    double MydoubleSetterProperty { set; }
    string MystringSetterProperty { set; }
    bool MyboolSetterProperty { set; }
    char MycharSetterProperty { set; }
    object MyobjectSetterProperty { set; }
    object[] MyobjectArraySetterProperty { set; }
    System.Collections.IEnumerator MySystemCollectionsIEnumeratorSetterProperty { set; }
    DateTime MySystemDateTimeSetterProperty { set; }
    Guid MySystemGuidSetterProperty { set; }
    System.Drawing.Color MySystemDrawingColorSetterProperty { set; }
    decimal MySystemDecimalSetterProperty { set; }

    void MyvoidMethod();
    byte MybyteMethod();
    sbyte MysbyteMethod();
    short MyshortMethod();
    ushort MyushortMethod();
    uint MyuintMethod();
    int MyintMethod();
    ulong MyulongMethod();
    long MylongMethod();
    float MySingleMethod();
    double MydoubleMethod();
    string MystringMethod();
    bool MyboolMethod();
    char MycharMethod();
    object MyobjectMethod();
    object[] MyobjectArrayMethod();
    System.Collections.IEnumerator MySystemCollectionsIEnumeratorMethod();
    DateTime MySystemDateTimeMethod();
    Guid MySystemGuidMethod();
    System.Drawing.Color MySystemDrawingColorMethod();
    decimal MySystemDecimalMethod();

    void MyvoidMethod1Param(int param1);
    byte MybyteMethod1Param(byte param1);
    sbyte MysbyteMethod1Param(sbyte param1);
    short MyshortMethod1Param(short param1);
    ushort MyushortMethod1Param(ushort param1);
    uint MyuintMethod1Param(uint param1);
    int MyintMethod1Param(int param1);
    ulong MyulongMethod1Param(ulong param1);
    long MylongMethod1Param(long param1);
    float MySingleMethod1Param(float param1);
    double MydoubleMethod1Param(double param1);
    string MystringMethod1Param(string param1);
    bool MyboolMethod1Param(bool param1);
    char MycharMethod1Param(char param1);
    object MyobjectMethod1Param(object param1);
    object[] MyobjectArrayMethod1Param(object[] param1);
    System.Collections.IEnumerator MySystemCollectionsIEnumeratorMethod1Param(System.Collections.IEnumerator param1);
    DateTime MySystemDateTimeMethod1Param(DateTime param1);
    Guid MySystemGuidMethod1Param(Guid param1);
    System.Drawing.Color MySystemDrawingColorMethod1Param(System.Drawing.Color param1);
    decimal MySystemDecimalMethod1Param(decimal param1);

    void MyvoidMethod2Param(int param1, int param2);
    byte MybyteMethod2Param(byte param1, byte param2);
    sbyte MysbyteMethod2Param(sbyte param1, sbyte param2);
    short MyshortMethod2Param(short param1, short param2);
    ushort MyushortMethod2Param(ushort param1, ushort param2);
    uint MyuintMethod2Param(uint param1, uint param2);
    int MyintMethod2Param(int param1, int param2);
    ulong MyulongMethod2Param(ulong param1, ulong param2);
    long MylongMethod2Param(long param1, long param2);
    float MySingleMethod2Param(float param1, float param2);
    double MydoubleMethod2Param(double param1, double param2);
    string MystringMethod2Param(string param1, string param2);
    bool MyboolMethod2Param(bool param1, bool param2);
    char MycharMethod2Param(char param1, char param2);
    object MyobjectMethod2Param(object param1, object param2);
    object[] MyobjectArrayMethod2Param(object[] param1, object[] param2);
    System.Collections.IEnumerator MySystemCollectionsIEnumeratorMethod2Param(System.Collections.IEnumerator param1, System.Collections.IEnumerator param2);
    DateTime MySystemDateTimeMethod2Param(DateTime param1, DateTime param2);
    Guid MySystemGuidMethod2Param(Guid param1, Guid param2);
    System.Drawing.Color MySystemDrawingColorMethod2Param(System.Drawing.Color param1, System.Drawing.Color param2);
    decimal MySystemDecimalMethod2Param(decimal param1, decimal param2);
    IDependentTestInterface MyIDependentTestInterfaceMethod();
    unsafe void MyMethodWithUnsafeBytePointer(byte* pointer);
    unsafe void MyMethodWithUnsafeCharPointer(char* pointer);
    unsafe void MyMethodWithUnsafeSBytePointer(sbyte* pointer);
    unsafe void MyMethodWithUnsafeShortPointer(short* pointer);
    unsafe void MyMethodWithUnsafeIntPointer(int* pointer);
    unsafe void MyMethodWithUnsafeUIntPointer(uint* pointer);
    unsafe void MyMethodWithUnsafeUShortPointer(ushort* pointer);
    unsafe void MyMethodWithUnsafeLongPointer(long* pointer);
    unsafe void MyMethodWithUnsafeULongPointer(ulong* pointer);
    unsafe void MyMethodWithUnsafeFloatPointer(float* pointer);
    unsafe void MyMethodWithUnsafeDoublePointer(double* pointer);
}
