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

using System.Reflection;
using System.Runtime.InteropServices;
using dSPACE.Runtime.InteropServices.ComTypes;

namespace dSPACE.Runtime.InteropServices.Writer;

internal class MethodWriter : BaseWriter
{
    public MethodWriter(InterfaceWriter interfaceWriter, MethodInfo methodInfo, WriterContext context, string methodName) : base(context)
    {
        InterfaceWriter = interfaceWriter;
        MethodInfo = methodInfo;
        MethodName = methodName;

        //switch off HResult transformation on method level
        var preserveSigAttribute = methodInfo.GetCustomAttribute<PreserveSigAttribute>();
        UseHResultAsReturnValue = preserveSigAttribute == null && interfaceWriter.UseHResultAsReturnValue;

        // In case of a Property
        MemberInfo ??= methodInfo;

        CreateParameterWriters();
    }

    public bool UseHResultAsReturnValue { get; private set; }

    public InterfaceWriter InterfaceWriter { get; }

    public MemberInfo MemberInfo { get; protected set; }

    internal MethodInfo MethodInfo { get; }

    internal string MethodName { get; private set; }

    protected INVOKEKIND InvokeKind { get; set; } = INVOKEKIND.INVOKE_FUNC;

    private List<ParameterWriter> ParameterWriters { get; } = new();

    private ParameterWriter? ReturnParamWriter { get; set; }

    protected virtual short GetParametersCount()
    {
        var retVal = (short)(UseHResultAsReturnValue && MethodInfo.ReturnType != typeof(void) ?
                MethodInfo.GetParameters().Length + 1 :
                MethodInfo.GetParameters().Length);
        return retVal;
    }

    protected virtual bool IsComVisible
    {
        get
        {
            var methodAttribute = MethodInfo.GetCustomAttribute<ComVisibleAttribute>();
            return methodAttribute == null || methodAttribute.Value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance of MethodWriter is valid to generate a FuncDesc.
    /// </summary>
    /// <value>true if valid; otherwise false</value>
    public bool IsValid => !MethodInfo.IsGenericMethod && IsComVisible && ParameterWriters.All(z => z.IsValid) && ReturnParamWriter != null && ReturnParamWriter.IsValid;

    public int FunctionIndex { get; set; } = -1;

    public int VTableOffset { get; set; }

    public void CreateParameterWriters()
    {
        //check for disabled HRESULT transformation
        var preserveSigAttribute = MethodInfo.GetCustomAttribute<PreserveSigAttribute>();
        var useNetMethodSignature = (preserveSigAttribute == null) && !UseHResultAsReturnValue;
        foreach (var paramInfo in MethodInfo.GetParameters())
        {
            ParameterWriters.Add(new ParameterWriter(this, paramInfo, Context, false));
        }
        if (useNetMethodSignature || preserveSigAttribute != null)
        {
            ReturnParamWriter = new ParameterWriter(this, MethodInfo.ReturnParameter, Context, false);
        }
        else
        {
            ParameterWriters.Add(new ParameterWriter(this, MethodInfo.ReturnParameter, Context, true));
            ReturnParamWriter = new ParameterWriter(this, new HResultParamInfo(), Context, false);
        }

        ReturnParamWriter!.Create();

        foreach (var paramWriter in ParameterWriters)
        {
            paramWriter.Create();
        }
    }

    public override void Create()
    {
        var vTableOffset = VTableOffset;
        var funcIndex = FunctionIndex;
        if (funcIndex == -1)
        {
            throw new InvalidOperationException("Function index is -1");
        }

        var typeInfo = InterfaceWriter.TypeInfo;

        if (!IsComVisible)
        {
            return;
        }

        if (ReturnParamWriter != null)
        {
            foreach (var writer in ParameterWriters.Append(ReturnParamWriter))
            {
                writer.ReportEvent();
            }
        }
        if (!IsValid)
        {
            return;
        }

        if (typeInfo == null)
        {
            throw new ArgumentException("ICreateTypeInfo2 is null.");
        }

        var memidCreated = (int)InterfaceWriter.DispatchIdCreator!.GetDispatchId(this);
        string[] names;
        try
        {
            // ReturnParamWriter!.Create();

            // foreach (var paramWriter in ParameterWriters)
            // {
            //     paramWriter.Create();
            // }

            FUNCDESC? funcDesc = new FUNCDESC
            {
                callconv = CALLCONV.CC_STDCALL,
                cParams = GetParametersCount(),
                cParamsOpt = 0,
                cScodes = 0,
                elemdescFunc = ReturnParamWriter!.ElementDescription,
                funckind = InterfaceWriter.FuncKind,
                invkind = (memidCreated == 0 && InvokeKind == INVOKEKIND.INVOKE_FUNC) ? INVOKEKIND.INVOKE_PROPERTYGET : InvokeKind,
                lprgelemdescParam = GetElementDescriptionsPtrForParameters(),
                lprgscode = IntPtr.Zero,
                memid = memidCreated,
                oVft = (short)vTableOffset,
                wFuncFlags = 0
            };

            //Check if method still enabled. If a parameter is not enabled, the method should not be created.
            if (!IsValid)
            {
                return;
            }

            names = GetNamesForParameters().ToArray();

            typeInfo.AddFuncDesc((uint)funcIndex, funcDesc.Value)
                .ThrowIfFailed($"Failed to add function description for {MethodInfo.Name}.");

            typeInfo.SetFuncAndParamNames((uint)funcIndex, names, (uint)names.Length)
                .ThrowIfFailed($"Failed to set function and parameter names for {MethodInfo.Name}.");

            var description = MethodInfo.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
            if (description != null)
            {
                typeInfo.SetFuncDocString((uint)funcIndex, description.Description)
                    .ThrowIfFailed($"Failed to set function documentation string for {MethodInfo.Name}.");
            }

            if (memidCreated == 0 && InvokeKind == INVOKEKIND.INVOKE_FUNC)
            {
                //Function has been forced as property (default member handling)
                typeInfo.SetFuncCustData((uint)funcIndex, new Guid(Guids.GUID_Function2Getter), 1)
                    .ThrowIfFailed($"Failed to set function custom data for {MethodInfo.Name}.");
            }

        }
        catch (Exception ex) when (ex is TypeLoadException or COMException)
        {
            Context.LogWarning(ex.Message, HRESULT.E_INVALIDARG);
        }
    }

    protected virtual List<string> GetNamesForParameters()
    {
        var names = new List<string>();
        var methodName = Context.NameResolver.GetMappedName(MethodName);
        names.Add(methodName);

        MethodInfo.GetParameters().ToList().ForEach(p => names.Add(Context.NameResolver.GetMappedName(p.Name ?? string.Empty)));

        if (UseHResultAsReturnValue && MethodInfo.ReturnType != typeof(void))
        {
            names.Add("pRetVal");
        }

        return names;
    }

    private IntPtr GetElementDescriptionsPtrForParameters()
    {
        var parameters = new ELEMDESC[ParameterWriters.Count];

        foreach (var paramWriter in ParameterWriters)
        {
            parameters[ParameterWriters.IndexOf(paramWriter)] = paramWriter.ElementDescription;
        }

        var intPtr = StructuresToPtr(parameters);

        return intPtr;
    }
}
