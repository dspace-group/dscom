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

public class ComVisibilityTest : BaseTest
{
    [Theory]
    [InlineData(null, null, true)]
    [InlineData(null, true, true)]
    [InlineData(null, false, false)]
    [InlineData(true, null, true)]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, null, false)]
    [InlineData(false, true, true)]
    [InlineData(false, false, false)]
    public void ComVisibleAttributes_Considered(bool? assemblyComVisibleAttribute, bool? interfacesComVisibleAttribute, bool interfaceCreated)
    {
        var builder = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{assemblyComVisibleAttribute}{interfacesComVisibleAttribute}{interfaceCreated}"));
        if (assemblyComVisibleAttribute != null)
        {
            builder.WithCustomAttribute(typeof(ComVisibleAttribute), assemblyComVisibleAttribute.Value);
        }

        var typebuilder = builder.WithInterface("TestInterface");
        if (interfacesComVisibleAttribute != null)
        {
            typebuilder.WithCustomAttribute(typeof(ComVisibleAttribute), interfacesComVisibleAttribute.Value);
        }

        var result = typebuilder.Build().Build();
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");

        (typeInfo != null).Should().Be(interfaceCreated);
    }

    [Fact]
    public void PropertyWithComVisibleAttributesFalse_PropertyNotAvailable()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithProperty("TestProperty", typeof(string))
                                .WithCustomAttribute<ComVisibleAttribute>(false)
                            .Build()
                        .Build()
                    .Build();

        using var property = result.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("TestProperty");
        property.Should().BeNull("Property is ComVisible=false and should not be visible");
    }

    [Fact]
    public void MethodWithComVisibleAttributesFalse_MethodNotAvailable()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithMethod("TestMethod")
                                .WithReturnType<string>()
                                .WithCustomAttribute<ComVisibleAttribute>(false)
                            .Build()
                        .Build()
                    .Build();

        using var property = result.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("TestMethod");
        property.Should().BeNull("Method is ComVisible=false and should not be visible");
    }
}
