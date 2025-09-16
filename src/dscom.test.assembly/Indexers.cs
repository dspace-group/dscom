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
public interface IIndexers1
{
    int this[int index]
    {
        get;
    }
}

[ComVisible(true)]
public interface IIndexers2
{
    int this[int index]
    {
        get; set;
    }
}

[ComVisible(true)]
public interface IIndexers3
{
    string this[int index]
    {
        get;
    }
}

[ComVisible(true)]
public interface IIndexers4
{
    string this[int index]
    {
        get; set;
    }
}

