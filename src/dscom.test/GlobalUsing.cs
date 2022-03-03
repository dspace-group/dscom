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

// Namespaces
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Reflection;
global using System.Reflection.Emit;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Text.RegularExpressions;
global using FluentAssertions;
global using Xunit;
global using ELEMDESC = System.Runtime.InteropServices.ComTypes.ELEMDESC;
// struct
global using FUNCDESC = System.Runtime.InteropServices.ComTypes.FUNCDESC;
global using IDLFLAG = System.Runtime.InteropServices.ComTypes.IDLFLAG;
global using IMPLTYPEFLAGS = System.Runtime.InteropServices.ComTypes.IMPLTYPEFLAGS;
global using INVOKEKIND = System.Runtime.InteropServices.ComTypes.INVOKEKIND;
// Interface
global using ITypeInfo = System.Runtime.InteropServices.ComTypes.ITypeInfo;
global using ITypeInfo2 = System.Runtime.InteropServices.ComTypes.ITypeInfo2;
global using ITypeLib2 = System.Runtime.InteropServices.ComTypes.ITypeLib2;
global using PARAMFLAG = System.Runtime.InteropServices.ComTypes.PARAMFLAG;
global using SYSKIND = System.Runtime.InteropServices.ComTypes.SYSKIND;
global using TYPEATTR = System.Runtime.InteropServices.ComTypes.TYPEATTR;
global using TYPEDESC = System.Runtime.InteropServices.ComTypes.TYPEDESC;
// enum
global using TYPEFLAGS = System.Runtime.InteropServices.ComTypes.TYPEFLAGS;
global using TYPEKIND = System.Runtime.InteropServices.ComTypes.TYPEKIND;
global using TYPELIBATTR = System.Runtime.InteropServices.ComTypes.TYPELIBATTR;
global using VARDESC = System.Runtime.InteropServices.ComTypes.VARDESC;
global using VARKIND = System.Runtime.InteropServices.ComTypes.VARKIND;
