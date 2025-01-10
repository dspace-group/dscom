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
global using System.Linq;
global using System.Reflection;
global using System.Reflection.Emit;
global using System.Runtime.CompilerServices;
global using System.Text.RegularExpressions;
global using FluentAssertions;
global using Xunit;

#pragma warning disable CS8019

global using ITypeComp = System.Runtime.InteropServices.ComTypes.ITypeComp;
global using ITypeInfo = System.Runtime.InteropServices.ComTypes.ITypeInfo;
global using ITypeInfo2 = System.Runtime.InteropServices.ComTypes.ITypeInfo2;
global using ITypeLib = System.Runtime.InteropServices.ComTypes.ITypeLib;
global using ITypeLib2 = System.Runtime.InteropServices.ComTypes.ITypeLib2;
global using ITypeInfo64Bit = dSPACE.Runtime.InteropServices.ComTypes.Internal.ITypeInfo64Bit;
global using ICreateTypeInfo2 = dSPACE.Runtime.InteropServices.ComTypes.Internal.ICreateTypeInfo2;
global using ICreateTypeLib2 = dSPACE.Runtime.InteropServices.ComTypes.Internal.ICreateTypeLib2;
global using ICreateTypeLib = dSPACE.Runtime.InteropServices.ComTypes.Internal.ICreateTypeLib;
global using IClassFactory = dSPACE.Runtime.InteropServices.ComTypes.Internal.IClassFactory;
global using IDispatch = dSPACE.Runtime.InteropServices.ComTypes.Internal.IDispatch;
global using IUnknown = dSPACE.Runtime.InteropServices.ComTypes.Internal.IUnknown;

global using Constants = dSPACE.Runtime.InteropServices.Internal.Constants;
global using Ole32 = dSPACE.Runtime.InteropServices.ComTypes.Internal.Ole32;
global using Guids = dSPACE.Runtime.InteropServices.Internal.Guids;

global using CUSTDATA = dSPACE.Runtime.InteropServices.ComTypes.Internal.CUSTDATA;
global using CUSTDATAITEM = dSPACE.Runtime.InteropServices.ComTypes.Internal.CUSTDATAITEM;
global using HRESULT = dSPACE.Runtime.InteropServices.ComTypes.Internal.HRESULT;
global using IDLDESC = dSPACE.Runtime.InteropServices.ComTypes.Internal.IDLDESC;
global using PARAMDESCEX = dSPACE.Runtime.InteropServices.ComTypes.Internal.PARAMDESCEX;
global using OleAut32 = dSPACE.Runtime.InteropServices.ComTypes.Internal.OleAut32;

global using DISPPARAMS = System.Runtime.InteropServices.ComTypes.DISPPARAMS;
global using ELEMDESC = System.Runtime.InteropServices.ComTypes.ELEMDESC;
global using FUNCDESC = System.Runtime.InteropServices.ComTypes.FUNCDESC;
global using PARAMDESC = System.Runtime.InteropServices.ComTypes.PARAMDESC;
global using TYPEATTR = System.Runtime.InteropServices.ComTypes.TYPEATTR;
global using TYPEDESC = System.Runtime.InteropServices.ComTypes.TYPEDESC;

global using REGKIND = dSPACE.Runtime.InteropServices.ComTypes.Internal.REGKIND;
global using VARIANT = dSPACE.Runtime.InteropServices.ComTypes.Internal.VARIANT;

global using CALLCONV = System.Runtime.InteropServices.ComTypes.CALLCONV;
global using IDLFLAG = System.Runtime.InteropServices.ComTypes.IDLFLAG;
global using FUNCKIND = System.Runtime.InteropServices.ComTypes.FUNCKIND;
global using IMPLTYPEFLAGS = System.Runtime.InteropServices.ComTypes.IMPLTYPEFLAGS;
global using LIBFLAGS = System.Runtime.InteropServices.ComTypes.LIBFLAGS;
global using PARAMFLAG = System.Runtime.InteropServices.ComTypes.PARAMFLAG;
global using SYSKIND = System.Runtime.InteropServices.ComTypes.SYSKIND;
global using INVOKEKIND = System.Runtime.InteropServices.ComTypes.INVOKEKIND;
global using TYPEFLAGS = System.Runtime.InteropServices.ComTypes.TYPEFLAGS;
global using TYPEKIND = System.Runtime.InteropServices.ComTypes.TYPEKIND;
global using TYPELIBATTR = System.Runtime.InteropServices.ComTypes.TYPELIBATTR;
global using VARDESC = System.Runtime.InteropServices.ComTypes.VARDESC;
global using VARKIND = System.Runtime.InteropServices.ComTypes.VARKIND;

#pragma warning restore CS8019
