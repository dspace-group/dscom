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

#pragma warning disable IL3000

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32;

namespace dSPACE.Runtime.InteropServices;

/// <summary>
/// Provides a set of services for registering and unregistering managed assemblies for use from COM.
/// </summary>
[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Compatibility to the mscorelib TypeLibConverter class")]
[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Compatibility to the mscorelib TypeLibConverter class")]
public class RegistrationServices
{
    /// <summary>
    /// When registering a managed type to the classes using the
    /// <see cref="RegisterAssembly(Assembly, bool, ManagedCategoryAction)"/> method,
    /// the managed registration method can check whether the registry key
    /// HKEY_CLASSES_ROOT\Component Categories\{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}\
    /// is present and has at least one key representing a locale code having the 
    /// value ".NET Category".
    /// </summary>
    public enum ManagedCategoryAction
    {
        /// <summary>
        /// No check or action is performed.
        /// </summary>
        None,

        /// <summary>
        /// A check is performed. If at least one numeric key exist below 
        /// HKEY_CLASSES_ROOT\Component Categories\{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}\
        /// and all numeric keys have the correct value.
        /// </summary>
        FailIfNotPresent,

        /// <summary>
        /// A check is performed. This is performed like <see cref="FailIfNotPresent"/>, but
        /// only for a neutral language, i.e. language code 0.
        /// </summary>
        FailIfNotPresentOnlyForNeutralLocale,

        /// <summary>
        /// This will actually create the key and value below 
        /// HKEY_CLASSES_ROOT\Component Categories\{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}\
        /// concerning only neutral languages, i.e. language code 0.
        /// This is the original implementation as of 
        /// <code>System.Runtime.InteropServices.RegisterAssembly(Assembly, RegistrationFlags)</code>.
        /// If the operation fails, e.g. due to missing privileges or elevation,
        /// any exception will be transmitted.
        /// </summary>
        Create,

        /// <summary>
        /// This will actually create the key and value below 
        /// HKEY_CLASSES_ROOT\Component Categories\{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}\
        /// concerning only neutral languages, i.e. language code 0.
        /// This is the original implementation as of 
        /// <code>System.Runtime.InteropServices.RegisterAssembly(Assembly, RegistrationFlags)</code>.
        /// If the operation fails, e.g. due to missing privileges or elevation,
        /// any exception will be swallowed.
        /// </summary>
        CreateSilently
    }

    private static class RegistryKeys
    {
        private const string Implemented = nameof(Implemented);
        private const string Component = nameof(Component);
        private const string Categories = nameof(Categories);

        public const string Record = nameof(Record);
        public const string Class = nameof(Class);
        public const string Assembly = nameof(Assembly);
        public const string RuntimeVersion = nameof(RuntimeVersion);
        public const string CodeBase = nameof(CodeBase);
        public const string CLSID = nameof(CLSID);
        public const string ThreadingModel = nameof(ThreadingModel);
        public const string InprocServer32 = nameof(InprocServer32);
        public const string ProgId = nameof(ProgId);

        public const string Software = nameof(Software);

        public const string Classes = nameof(Classes);

        public const string ManagedCategoryGuid = "{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}"; // Found in mscorelib

        public const string ImplementedCategories = $"{Implemented} {Categories}";
        public const string ComponentCategories = $"{Component} {Categories}";
    }

    private static class RegistryValues
    {
        private const string Both = nameof(Both);

        public const string ThreadingModel = Both;
        public const string ManagedCategoryDescription = ".NET Category";
    }

    /// <summary>Registers the specified type with COM using the specified GUID.</summary>
    /// <param name="type">The <see cref="T:System.Type" /> to be registered for use from COM.</param>
    /// <param name="g">The <see cref="T:System.Guid" /> used to register the specified type.</param>
    /// <exception cref="T:System.ArgumentException">The <paramref name="type" /> parameter is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="type" /> parameter cannot be created.</exception>
    public void RegisterTypeForComClients(Type type, ref Guid g)
    {
        var genericClassFactory = Thread.CurrentThread.GetApartmentState() == ApartmentState.STA ? typeof(STAClassFactory<>) : typeof(ClassFactory<>);
        Type[] typeArgs = { type };
        var constructedClassFactory = genericClassFactory.MakeGenericType(typeArgs);

        var createdClassFactory = Activator.CreateInstance(constructedClassFactory);

        var hr = Ole32.CoRegisterClassObject(g, createdClassFactory!, (uint)(ComTypes.RegistrationClassContext.InProcessServer | ComTypes.RegistrationClassContext.LocalServer), (uint)ComTypes.RegistrationConnectionType.MultipleUse, out _);
        if (hr < 0)
        {
            Marshal.ThrowExceptionForHR(hr);
        }
    }

    /// <summary>Registers the specified type with COM using the specified execution context and connection type.</summary>
    /// <param name="type">The <see cref="T:System.Type" /> object to register for use from COM.</param>
    /// <param name="classContext">One of the <see cref="T:dSPACE.Runtime.InteropServices.ComTypes.RegistrationClassContext" /> values that indicates the context in which the executable code will be run.</param>
    /// <param name="flags">One of the <see cref="T:dSPACE.Runtime.InteropServices.ComTypes.RegistrationConnectionType" /> values that specifies how connections are made to the class object.</param>
    /// <returns>An integer that represents a cookie value.</returns>
    /// <exception cref="T:System.ArgumentException">The <paramref name="type" /> parameter is <see langword="null" /> or not a valid type.</exception>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="type" /> parameter cannot be created.</exception>
    public int RegisterTypeForComClients(Type type, ComTypes.RegistrationClassContext classContext, ComTypes.RegistrationConnectionType flags)
    {
        var value = (type.GetCustomAttributes<GuidAttribute>().FirstOrDefault()?.Value
                    ?? type.Assembly.GetCustomAttributes<GuidAttribute>().FirstOrDefault()?.Value) ?? throw new ArgumentException($"The given type {type} does not have a valid GUID attribute.");
        var guid = new Guid(value);

        var genericClassFactory = Thread.CurrentThread.GetApartmentState() == ApartmentState.STA ? typeof(STAClassFactory<>) : typeof(ClassFactory<>);
        Type[] typeArgs = { type };
        var constructedClassFactory = genericClassFactory.MakeGenericType(typeArgs);

        var createdClassFactory = Activator.CreateInstance(constructedClassFactory);

        var hr = Ole32.CoRegisterClassObject(guid, createdClassFactory!, (uint)classContext, (uint)flags, out var cookie);
        if (hr < 0)
        {
            Marshal.ThrowExceptionForHR(hr);
        }
        return cookie;
    }

    /// <summary>Unregisters the specified type referenced by the cookie.</summary>
    /// <param name="cookie">The cookie to unregister for use from COM.</param>
    public void UnregisterTypeForComClients(int cookie)
    {
        var hr = Ole32.CoRevokeClassObject(cookie);
        if (hr < 0)
        {
            Marshal.ThrowExceptionForHR(hr);
        }
    }

    /// <summary>
    /// Registers the classes in a managed assembly to enable creation from COM.
    /// </summary>
    /// <param name="assembly">The assembly to register.</param>
    /// <param name="registerCodeBase">If set to <c>true</c>, the code base will be added to the registry; otherwise not.</param>
    /// <param name="preferredAction">The managed category action for a global registration of HKEY_CLASSES_ROOT\Component Categories\62C8FE65-4EBB-45e7-B440-6E39B2CDBF29</param>
    /// <returns><c>true</c>, if at least one type from the registry has been registered.</returns>
    public bool RegisterAssembly(Assembly assembly, bool registerCodeBase, ManagedCategoryAction preferredAction = ManagedCategoryAction.None)
    {
        if (assembly is null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        if (assembly.ReflectionOnly || assembly.IsDynamic)
        {
            throw new ArgumentException("Cannot register a ReflectionOnly or dynamic assembly");
        }

        var fullName = assembly.FullName ?? throw new ArgumentException("Cannot register an assembly without a full name");
        string? codeBase = null;
        if (registerCodeBase && assembly.Location is null)
        {
            // GetCodeBase/CodeBase is obsolete. Use Location instead
            throw new ArgumentException("Cannot set code base on an assembly not providing a code base location.");
        }
        else if (registerCodeBase)
        {
            codeBase = assembly.Location;
        }

        var typesToRegister = GetComRegistratableTypes(assembly);

        // Should be the same as RuntimeAssembly.GetVersion()
        var assemblyVersion = assembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version ?? new Version().ToString();

        var runtimeVersion = assembly.ImageRuntimeVersion;

        foreach (var type in typesToRegister)
        {
            if (IsComRegistratableValueType(type))
            {
                RegisterValueType(type, fullName, assemblyVersion, codeBase, runtimeVersion);
            }
            else if (IsComType(type))
            {
                RegisterImportedComType(type, fullName, assemblyVersion, codeBase, runtimeVersion);
            }
            else
            {
                RegisterManagedType(type, fullName, assemblyVersion, codeBase, runtimeVersion, preferredAction);
            }

            // Skip: CustomRegistrationFunction
        }

        // Skip: PIA Regitration

        return typesToRegister.Count > 0;
    }

    /// <summary>
    /// Unregisters the classes in a managed assembly to enable creation from COM.
    /// </summary>
    /// <param name="assembly">The assembly to unregister.</param>
    /// <returns><c>true</c>, if all types from the registry have been unregistered.</returns>
    public bool UnregisterAssembly(Assembly assembly)
    {
        if (assembly is null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        if (assembly.ReflectionOnly || assembly.IsDynamic)
        {
            throw new ArgumentException("Cannot unregister a ReflectionOnly or dynamic assembly");
        }

        var typesNotRemoved = new List<Type>();

        var typesToUnregister = GetComRegistratableTypes(assembly);

        // Should be the same as RuntimeAssembly.GetVersion()
        var assemblyVersion = assembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version ?? new Version().ToString();

        foreach (var type in typesToUnregister)
        {
            // Skip: Custom unregister function
            if (IsComRegistratableValueType(type) && !UnregisterValueType(type, assemblyVersion))
            {
                typesNotRemoved.Add(type);
            }
            else if (IsComType(type) && !UnregisterImportedComType(type, assemblyVersion))
            {
                typesNotRemoved.Add(type);
            }
            else if (!UnregisterManagedType(type, assemblyVersion))
            {
                typesNotRemoved.Add(type);
            }
        }

        // Skip: PIA

        return typesNotRemoved.Count == 0;
    }

    private static IReadOnlyCollection<Type> GetComRegistratableTypes(Assembly assembly)
    {
        static bool TypeMustBeRegistered(Type type)
        {
            if (type.IsGenericType)
            {
                return false;
            }

            if (type.IsAbstract)
            {
                return false;
            }

            var publicParameterlessCtor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, Array.Empty<Type>(), null);
            if (!type.IsValueType && publicParameterlessCtor is null)
            {
                return false;
            }

            if (type.IsClass || type.IsValueType)
            {
                return true;
            }

            return Marshal.IsTypeVisibleFromCom(type);
        }

        if (assembly is null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        return assembly.GetExportedTypes().Where(TypeMustBeRegistered).ToArray();
    }

    private static bool IsComRegistratableValueType(Type type)
    {
        return type.IsValueType;
    }

    private static bool IsComType(Type type)
    {
        static Type? GetBaseComImportType(Type? currentType)
        {
            while (currentType != null && !currentType.IsImport)
            {
                currentType = currentType.BaseType;
            }

            return currentType;
        }

        if (type.IsCOMObject)
        {
            return false;
        }

        if (type.IsImport)
        {
            return true;
        }

        var parentComType = GetBaseComImportType(type);
        return parentComType != null && MarshalExtension.GetClassInterfaceGuidForType(parentComType!) == MarshalExtension.GetClassInterfaceGuidForType(type);
    }

    private static void RegisterValueType(Type type, string assemblyName, string assemblyVersion, string? codeBase, string runtimeVersion)
    {
        if (type.FullName is null)
        {
            throw new ArgumentException("Cannot register a type without a full name");
        }

        var recordId = $"{{{MarshalExtension.GetClassInterfaceGuidForType(type).ToString().ToUpperInvariant()}}}";

        using var rootKey = GetTargetRootKey();
        using var recordRootKey = rootKey.CreateSubKey(RegistryKeys.Record);
        using var recordKey = recordRootKey.CreateSubKey(recordId);
        using var recordVersionKey = recordKey.CreateSubKey(assemblyVersion);

        recordVersionKey.SetValue(RegistryKeys.Class, type.FullName);

        recordVersionKey.SetValue(RegistryKeys.Assembly, assemblyName);

        recordVersionKey.SetValue(RegistryKeys.RuntimeVersion, runtimeVersion);

        if (codeBase is not null)
        {
            recordVersionKey.SetValue(RegistryKeys.CodeBase, codeBase);
        }
    }

    private static bool UnregisterValueType(Type type, string assemblyVersion)
    {
        var recordId = $"{{{MarshalExtension.GetClassInterfaceGuidForType(type).ToString().ToUpperInvariant()}}}";

        using var rootKey = GetTargetRootKey();
        using var recordRootKey = rootKey.OpenSubKey(RegistryKeys.Record, true);
        using var recordKey = recordRootKey?.OpenSubKey(recordId, true);
        using var recordVersionKey = recordKey?.OpenSubKey(assemblyVersion, true);

        recordVersionKey?.DeleteValue(RegistryKeys.Class, false);

        recordVersionKey?.DeleteValue(RegistryKeys.Assembly, false);

        recordVersionKey?.DeleteValue(RegistryKeys.RuntimeVersion, false);

        recordVersionKey?.DeleteValue(RegistryKeys.CodeBase, false);

        if (IsEmptyRegistryKey(recordVersionKey))
        {
            recordKey?.DeleteSubKey(assemblyVersion, false);
        }

        var allVersionsGone = (recordKey?.SubKeyCount ?? 0) == 0;

        if (IsEmptyRegistryKey(recordKey))
        {
            recordRootKey?.DeleteSubKey(recordId, false);
        }

        if (IsEmptyRegistryKey(recordRootKey))
        {
            Registry.ClassesRoot.DeleteSubKey(RegistryKeys.Record, false);
        }

        return allVersionsGone;
    }

    private static void RegisterManagedType(Type type, string assemblyName, string assemblyVersion, string? codeBase, string runtimeVersion, ManagedCategoryAction preferredAction)
    {
        if (type.FullName is null)
        {
            throw new ArgumentException("Cannot register a type without a full name");
        }

        var docString = type.FullName!;
        var clsId = $"{{{Marshal.GenerateGuidForType(type).ToString().ToUpperInvariant()}}}";
        var progId = Marshal.GenerateProgIdForType(type);

        using var rootKey = GetTargetRootKey();
        if (!string.IsNullOrWhiteSpace(progId))
        {
            using var typeNameKey = rootKey.CreateSubKey(progId!);

            typeNameKey.SetValue(string.Empty, docString);

            using var progIdClsIdKey = typeNameKey.CreateSubKey(RegistryKeys.CLSID);

            progIdClsIdKey.SetValue(string.Empty, clsId);
        }

        using var clsIdRootKey = rootKey.CreateSubKey(RegistryKeys.CLSID);

        using var clsIdKey = clsIdRootKey.CreateSubKey(clsId);

        clsIdKey.SetValue(string.Empty, docString);

        using var inProcServerKey = clsIdKey.CreateSubKey(RegistryKeys.InprocServer32);

        // This should be the entry point for COM CoCreateInstance()
        // Currently, there is no entry point for modern .NET 6 assemblies.
        // This must be implemented in .NET 6 afterwards, if required.
        // For .NET FX this would be mscoree.dll.
        // According to https://learn.microsoft.com/en-us/dotnet/core/native-interop/expose-components-to-com,
        // this might be the XYZ.comhost.dll
        var comHostFile = GetComHost(codeBase);
        if (null != comHostFile)
        {
            inProcServerKey.SetValue(string.Empty, string.Empty);
            inProcServerKey.SetValue(RegistryKeys.ThreadingModel, RegistryValues.ThreadingModel);
            inProcServerKey.SetValue(RegistryKeys.Class, type.FullName!);
            inProcServerKey.SetValue(RegistryKeys.Assembly, assemblyName);
            inProcServerKey.SetValue(RegistryKeys.RuntimeVersion, runtimeVersion);
            if (codeBase is not null)
            {
                inProcServerKey.SetValue(RegistryKeys.CodeBase, codeBase!);
            }

            using var versionSubKey = inProcServerKey.CreateSubKey(assemblyVersion);
            versionSubKey.SetValue(RegistryKeys.Class, type.FullName!);
            versionSubKey.SetValue(RegistryKeys.Assembly, assemblyName);
            versionSubKey.SetValue(RegistryKeys.RuntimeVersion, runtimeVersion);
            if (codeBase is not null)
            {
                versionSubKey.SetValue(RegistryKeys.CodeBase, codeBase!);
            }
        }

        if (!string.IsNullOrWhiteSpace(progId))
        {
            using var progIdKey = clsIdKey.CreateSubKey(RegistryKeys.ProgId);

            progIdKey.SetValue(string.Empty, progId!);

        }

        using var implementedCategoryKey = clsIdKey.CreateSubKey(RegistryKeys.ImplementedCategories);

        using var managedCategoryKeyForImplemented = implementedCategoryKey.CreateSubKey(RegistryKeys.ManagedCategoryGuid);

        SetupGlobalManagedCategoryAction(preferredAction);
    }

    private static void SetupGlobalManagedCategoryAction(ManagedCategoryAction preferredAction)
    {
        switch (preferredAction)
        {
            case ManagedCategoryAction.Create:
            case ManagedCategoryAction.CreateSilently:
                {
                    try
                    {
                        EnsureManageCategoryIsPresent();
                    }
                    catch (Exception e) when ((preferredAction is ManagedCategoryAction.CreateSilently)
                        && (e is UnauthorizedAccessException or SecurityException))
                    {
                        Debug.WriteLine(e);
                    }
                }
                break;
            case ManagedCategoryAction.FailIfNotPresent:
            case ManagedCategoryAction.FailIfNotPresentOnlyForNeutralLocale:
                {
                    if (!HasManagedCategory(preferredAction == ManagedCategoryAction.FailIfNotPresentOnlyForNeutralLocale))
                    {
                        var message = $"HKEY_CLASSES_ROOT\\{RegistryKeys.ComponentCategories}\\{RegistryKeys.ManagedCategoryGuid} does not provide a localized or neutral definition value containing '{RegistryValues.ManagedCategoryDescription}'";

                        throw new InvalidOperationException(message);
                    }
                }
                break;
        }
    }

    private static bool HasManagedCategory(bool checkForNeutralLocale = true)
    {
        using var componentCategoryKey = Registry.ClassesRoot.OpenSubKey(RegistryKeys.ComponentCategories, false);
        if (componentCategoryKey is null)
        {
            return false;
        }

        using var managedCategoryKeyCheck = componentCategoryKey.OpenSubKey(RegistryKeys.ManagedCategoryGuid, false);
        if (managedCategoryKeyCheck is null)
        {
            return false;
        }

        if (checkForNeutralLocale)
        {
            var key0 = Convert.ToString(0, CultureInfo.InvariantCulture);
            var value = managedCategoryKeyCheck.GetValue(key0);
            if (value is null || value.GetType() != typeof(string))
            {
                return false;
            }

            var exactValue = (string)value;
            return StringComparer.InvariantCulture.Equals(exactValue, RegistryValues.ManagedCategoryDescription);
        }
        else
        {
            var valueNames = managedCategoryKeyCheck.GetValueNames().Where(item => int.TryParse(item, out _)).ToArray();
            return valueNames.LongLength > 0 && valueNames.All(key =>
            {
                var value = managedCategoryKeyCheck.GetValue(key);
                if (value is not null and string exactValue)
                {
                    return StringComparer.InvariantCulture.Equals(exactValue, RegistryValues.ManagedCategoryDescription);
                }

                return false;
            });
        }
    }

    private static void EnsureManageCategoryIsPresent()
    {
        if (HasManagedCategory())
        {
            return;
        }

        using var componentCategoryKey = Registry.ClassesRoot.CreateSubKey(RegistryKeys.ComponentCategories);
        using var managedCategoryKey = componentCategoryKey.CreateSubKey(RegistryKeys.ManagedCategoryGuid);

        var key0 = Convert.ToString(0, CultureInfo.InvariantCulture);
        var value = managedCategoryKey.GetValue(key0);
        if (value is not null && value.GetType() != typeof(string))
        {
            managedCategoryKey.DeleteValue(key0, false);
            managedCategoryKey.SetValue(key0, RegistryValues.ManagedCategoryDescription);
        }
        else if (value is not null)
        {
            var keyValue = (string)value;
            if (!StringComparer.InvariantCulture.Equals(keyValue, RegistryValues.ManagedCategoryDescription))
            {
                managedCategoryKey.SetValue(key0, RegistryValues.ManagedCategoryDescription);
            }
        }
        else
        {
            managedCategoryKey.SetValue(key0, RegistryValues.ManagedCategoryDescription);
        }
    }

    private static string? GetComHost(string? codeBase)
    {
        if (null == codeBase)
        {
            return null;
        }

        string comhost = nameof(comhost);

        var extension = Path.GetExtension(codeBase);
        var fileName = Path.GetFileNameWithoutExtension(codeBase);
        var fileLocation = Path.GetDirectoryName(codeBase);

        var comHostFileName = $"{fileName}.{comhost}{extension}";
        var comHostFileLocation = comHostFileName;
        if (!string.IsNullOrWhiteSpace(fileLocation))
        {
            comHostFileLocation = Path.Combine(fileLocation, comHostFileName);
        }

        if (File.Exists(comHostFileLocation))
        {
            return comHostFileLocation;
        }

        return null;
    }

    private static bool UnregisterManagedType(Type type, string assemblyVersion)
    {
        var clsId = $"{{{Marshal.GenerateGuidForType(type).ToString().ToUpperInvariant()}}}";
        var progId = Marshal.GenerateProgIdForType(type);

        using var clsIdRootKey = Registry.ClassesRoot.OpenSubKey(RegistryKeys.CLSID, true);

        using var clsIdKey = clsIdRootKey?.OpenSubKey(clsId, true);

        clsIdKey?.DeleteValue(string.Empty, false);

        using var inProcServerKey = clsIdKey?.OpenSubKey(RegistryKeys.InprocServer32, true);

        using var versionSubKey = inProcServerKey?.OpenSubKey(assemblyVersion, true);

        versionSubKey?.DeleteValue(RegistryKeys.Class, false);
        versionSubKey?.DeleteValue(RegistryKeys.Assembly, false);
        versionSubKey?.DeleteValue(RegistryKeys.RuntimeVersion, false);
        versionSubKey?.DeleteValue(RegistryKeys.CodeBase, false);

        if (IsEmptyRegistryKey(versionSubKey))
        {
            inProcServerKey?.DeleteSubKey(assemblyVersion, false);
        }

        var allVersionsGone = (inProcServerKey?.SubKeyCount ?? 0) == 0;

        if (allVersionsGone)
        {
            inProcServerKey?.DeleteValue(string.Empty, false);
            inProcServerKey?.DeleteValue(RegistryKeys.ThreadingModel, false);
        }

        inProcServerKey?.DeleteValue(RegistryKeys.Class, false);
        inProcServerKey?.DeleteValue(RegistryKeys.Assembly, false);
        inProcServerKey?.DeleteValue(RegistryKeys.RuntimeVersion, false);
        inProcServerKey?.DeleteValue(RegistryKeys.CodeBase, false);

        if (IsEmptyRegistryKey(inProcServerKey))
        {
            clsIdKey?.DeleteSubKey(RegistryKeys.InprocServer32, false);
        }

        if (allVersionsGone && !string.IsNullOrWhiteSpace(progId))
        {
            using var progIdKey = clsIdKey?.OpenSubKey(RegistryKeys.ProgId, true);

            progIdKey?.DeleteValue(string.Empty, false);

            if (IsEmptyRegistryKey(progIdKey))
            {
                clsIdKey?.DeleteSubKey(RegistryKeys.ProgId, false);
            }
        }

        using var implementedCategoryKey = clsIdKey?.OpenSubKey(RegistryKeys.ImplementedCategories, true);

        using var managedCategoryKey = implementedCategoryKey?.OpenSubKey(RegistryKeys.ManagedCategoryGuid, true);

        if (IsEmptyRegistryKey(managedCategoryKey))
        {
            implementedCategoryKey?.DeleteSubKey(RegistryKeys.ManagedCategoryGuid, false);
        }

        if (IsEmptyRegistryKey(implementedCategoryKey))
        {
            clsIdKey?.DeleteSubKey(RegistryKeys.ImplementedCategories, false);
        }

        if (IsEmptyRegistryKey(clsIdKey))
        {
            clsIdRootKey?.DeleteSubKey(clsId, false);
        }

        if (IsEmptyRegistryKey(clsIdRootKey))
        {
            Registry.ClassesRoot.DeleteSubKey(RegistryKeys.CLSID, false);
        }

        if (allVersionsGone && !string.IsNullOrWhiteSpace(progId))
        {
            using var typeNameKey = Registry.ClassesRoot.OpenSubKey(progId!, true);

            typeNameKey?.DeleteValue(string.Empty, false);

            using var progIdClsIdKey = typeNameKey?.OpenSubKey(RegistryKeys.CLSID, true);

            progIdClsIdKey?.DeleteValue(string.Empty, false);

            if (IsEmptyRegistryKey(progIdClsIdKey))
            {
                typeNameKey?.DeleteSubKey(RegistryKeys.CLSID, false);
            }

            if (IsEmptyRegistryKey(typeNameKey))
            {
                Registry.ClassesRoot.DeleteSubKey(progId!, false);
            }
        }

        return allVersionsGone;
    }

    private static void RegisterImportedComType(Type type, string assemblyName, string assemblyVersion, string? codeBase, string runtimeVersion)
    {
        if (type.FullName is null)
        {
            throw new ArgumentException("Cannot register a type without a full name");
        }

        var clsId = $"{{{Marshal.GenerateGuidForType(type).ToString().ToUpperInvariant()}}}";

        using var clsIdRootKey = Registry.ClassesRoot.CreateSubKey(RegistryKeys.CLSID);

        using var clsIdKey = clsIdRootKey.CreateSubKey(clsId);

        using var inProcServerKey = clsIdKey.CreateSubKey(RegistryKeys.InprocServer32);

        inProcServerKey.SetValue(RegistryKeys.Class, type.FullName!);

        inProcServerKey.SetValue(RegistryKeys.Assembly, assemblyName);

        inProcServerKey.SetValue(RegistryKeys.RuntimeVersion, runtimeVersion);

        if (codeBase is not null)
        {
            inProcServerKey.SetValue(RegistryKeys.CodeBase, codeBase!);
        }

        using var versionSubKey = inProcServerKey.CreateSubKey(assemblyVersion);

        versionSubKey.SetValue(RegistryKeys.Class, type.FullName!);
        versionSubKey.SetValue(RegistryKeys.Assembly, assemblyName);
        versionSubKey.SetValue(RegistryKeys.RuntimeVersion, runtimeVersion);
        if (codeBase is not null)
        {
            versionSubKey.SetValue(RegistryKeys.CodeBase, codeBase!);
        }
    }

    private static bool UnregisterImportedComType(Type type, string assemblyVersion)
    {
        var clsId = $"{{{Marshal.GenerateGuidForType(type).ToString().ToUpperInvariant()}}}";

        using var clsIdRootKey = Registry.ClassesRoot.OpenSubKey(RegistryKeys.CLSID, true);

        using var clsIdKey = clsIdRootKey?.OpenSubKey(clsId, true);

        using var inProcServerKey = clsIdKey?.OpenSubKey(RegistryKeys.InprocServer32, true);

        inProcServerKey?.DeleteValue(RegistryKeys.Class, false);

        inProcServerKey?.DeleteValue(RegistryKeys.Assembly, false);

        inProcServerKey?.DeleteValue(RegistryKeys.RuntimeVersion, false);

        inProcServerKey?.DeleteValue(RegistryKeys.CodeBase, false);

        using var versionSubKey = inProcServerKey?.OpenSubKey(assemblyVersion, true);

        versionSubKey?.DeleteValue(RegistryKeys.Class, false);
        versionSubKey?.DeleteValue(RegistryKeys.Assembly, false);
        versionSubKey?.DeleteValue(RegistryKeys.RuntimeVersion, false);
        versionSubKey?.DeleteValue(RegistryKeys.CodeBase, false);

        if (IsEmptyRegistryKey(versionSubKey))
        {
            inProcServerKey?.DeleteSubKey(assemblyVersion, false);
        }

        var allVersionsGone = (inProcServerKey?.SubKeyCount ?? 0) == 0;

        if (IsEmptyRegistryKey(inProcServerKey))
        {
            clsIdKey?.DeleteSubKey(RegistryKeys.InprocServer32, false);
        }

        if (IsEmptyRegistryKey(clsIdRootKey))
        {
            Registry.ClassesRoot.DeleteSubKey(RegistryKeys.CLSID, false);
        }

        return allVersionsGone;
    }

    private static bool IsEmptyRegistryKey(RegistryKey? key)
    {
        if (key is null)
        {
            return true;
        }

        return key!.SubKeyCount == 0 && key!.ValueCount == 0;
    }

    private static RegistryKey GetTargetRootKey()
    {
        // According to
        // https://learn.microsoft.com/en-us/windows/win32/sysinfo/merged-view-of-hkey-classes-root
        // COM registration can take place per user without elevated permissions.
        if (CanWriteGlobalRegistry())
        {
            return Registry.ClassesRoot;
        }

        var root = Registry.CurrentUser;
        using var software = root.CreateSubKey(RegistryKeys.Software, true);
        var classes = software.CreateSubKey(RegistryKeys.Software, true);
        return classes;
    }

    private static bool CanWriteGlobalRegistry()
    {
        try
        {
            _ = Registry.ClassesRoot.OpenSubKey(RegistryKeys.Software.ToUpperInvariant());
            // This must be done using exceptions, since RegistryPermissions from CAS
            // Will no longer work in .NET 6 and above and will return true always.

            return true;
        }
        catch (Exception e) when (e is UnauthorizedAccessException or SecurityException)
        {
            return false;
        }
    }

}
