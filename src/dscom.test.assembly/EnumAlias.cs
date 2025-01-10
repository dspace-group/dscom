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

[
    ComVisible(true),
    Guid("A54B6C8C-852B-4F18-A5FD-BEDADFC8F92C"),
    ComAlias("FrooFroo")
]
public enum Fruits
{
    [ComAlias("frApple")]
    Apple,
    [ComAlias("frBanana")]
    Banana,
    [ComAlias("frCherry")]
    Cherry
}

[
    ComVisible(true),
    Guid("C3207301-F1CB-41E8-9582-8B387C3F2ABF"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)
]
public interface IEnumAlias
{
    Fruits GetFruit(Fruits preferredFruit = Fruits.Cherry);
}

[
    ComVisible(true),
    Guid("4FF2FE65-E157-4EED-A42D-D10B6B891C22"),
    ClassInterface(ClassInterfaceType.None),
    ProgId("dSPACE.Test.EnumAlias")
]
public class EnumAlias : IEnumAlias
{
    public Fruits GetFruit(Fruits preferredFruit = Fruits.Cherry)
    {
        switch (preferredFruit)
        {
            case Fruits.Apple:
                return Fruits.Cherry;
            case Fruits.Banana:
                return Fruits.Apple;
            case Fruits.Cherry:
                return Fruits.Banana;
            default:
                return Fruits.Banana;
        }
    }
}
