// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppression either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance", Justification = "Not available in .NET < 8")]
[assembly: SuppressMessage("Performance", "CA1850:Prefer static 'HashData' method over 'ComputeHash'", Justification = "Not available in .NET < 8")]

[assembly: SuppressMessage("Maintainability", "CA1513:Use ObjectDisposedException throw helper", Justification = "Not available in .NET < 8")]
[assembly: SuppressMessage("Maintainability", "CA1510:Use ArgumentNullException throw helper", Justification = "Not available in .NET < 8")]

[assembly: SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", Justification = "Not available in .NET < 8")]
[assembly: SuppressMessage("Interoperability", "SYSLIB1096:Convert to 'GeneratedComInterface'", Justification = "Not available in .NET < 8")]

[assembly: SuppressMessage("Style", "IDE0251:Make member 'readonly'", Justification = "Not available in .NET < 8")]
