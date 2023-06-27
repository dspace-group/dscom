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

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace dSPACE.Runtime.InteropServices;

/// <summary>
/// Identifies a typlibrary.
/// </summary>
[Browsable(false)]
public struct TypeLibIdentifier
{
    /// <summary>
    /// Gets or sets the Name of the type library.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the major version.
    /// </summary>
    public ushort MajorVersion { get; set; }

    /// <summary>
    /// Gets or sets the minor version.
    /// </summary>
    public ushort MinorVersion { get; set; }

    /// <summary>
    /// Gets or sets the library id.
    /// </summary>
    public Guid LibID { get; set; }

    /// <summary>
    /// Gets or set sets the language indentifier.
    /// </summary>
    public int LanguageIdentifier { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override readonly bool Equals(object? obj)
    {
        return obj is TypeLibIdentifier o && Equals(o);
    }

    /// <summary>
    /// Determines whether two object instances are equal.
    /// </summary>
    /// <param name="other">The <see cref="TypeLibIdentifier"/> to compare</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public readonly bool Equals(TypeLibIdentifier other)
    {
        return MajorVersion == other.MajorVersion
                && MinorVersion == other.MinorVersion
                && LibID == other.LibID
                && LanguageIdentifier == other.LanguageIdentifier;
    }

    /// <summary>
    /// A hash code for the current object.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override readonly int GetHashCode()
    {
        return MajorVersion.GetHashCode() ^ MinorVersion.GetHashCode() ^ LibID.GetHashCode() ^ LanguageIdentifier.GetHashCode();
    }

    /// <summary>
    /// The == (equality) and != (inequality) operators check if their operands are equal or not.
    /// </summary>
    /// <param name="left">Left object.</param>
    /// <param name="right">Right object</param>
    /// <returns>true if equal; otherwise, false.</returns>
    [ExcludeFromCodeCoverage]
    public static bool operator ==(TypeLibIdentifier left, TypeLibIdentifier right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// The == (equality) and != (inequality) operators check if their operands are equal or not.
    /// </summary>
    /// <param name="left">The left opject</param>
    /// <param name="right">The right object.</param>
    /// <returns>true if equal; otherwise, false.</returns>
    [ExcludeFromCodeCoverage]
    public static bool operator !=(TypeLibIdentifier left, TypeLibIdentifier right)
    {
        return !(left == right);
    }
}
