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

using System.Diagnostics.CodeAnalysis;

namespace dSPACE.Runtime.InteropServices;

public struct TypeLibIdentifier
{
    public string Name { get; set; }

    public ushort MajorVersion { get; set; }

    public ushort MinorVersion { get; set; }

    public Guid LibID { get; set; }

    public int LanguageIdentifier { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is TypeLibIdentifier o && Equals(o);
    }

    public bool Equals(TypeLibIdentifier other)
    {
        return MajorVersion == other.MajorVersion
                && MinorVersion == other.MinorVersion
                && LibID == other.LibID
                && LanguageIdentifier == other.LanguageIdentifier;
    }

    public override int GetHashCode()
    {
        return MajorVersion.GetHashCode() ^ MinorVersion.GetHashCode() ^ LibID.GetHashCode() ^ LanguageIdentifier.GetHashCode();
    }

    [ExcludeFromCodeCoverage] // Implement the equality operators and make their behavior identical to that of the Equals method
    public static bool operator ==(TypeLibIdentifier left, TypeLibIdentifier right)
    {
        return left.Equals(right);
    }


    [ExcludeFromCodeCoverage] // Implement the equality operators and make their behavior identical to that of the Equals method
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(TypeLibIdentifier left, TypeLibIdentifier right)
    {
        return !(left == right);
    }
}
