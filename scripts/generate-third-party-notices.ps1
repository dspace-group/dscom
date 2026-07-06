<#
.SYNOPSIS
    Generates the THIRD-PARTY-NOTICES.txt file at the repository root by listing all
    NuGet package licenses used by the published dscom projects (dscom, dscom.client,
    dscom.build), including transitive dependencies, across all target frameworks.

.DESCRIPTION
    Uses the "nuget-license" dotnet tool (https://github.com/sensslen/nuget-license)
    to analyze dscom.sln, excluding test-only projects (dscom.test*), since those are
    not part of the published NuGet packages.

    Requires:
      - dotnet restore to have been run against dscom.sln beforehand (nuget-license
        reads the project.assets.json lock file, it does not restore itself).
      - The "nuget-license" global dotnet tool to be installed:
          dotnet tool install --global nuget-license

.PARAMETER OutputPath
    Path of the THIRD-PARTY-NOTICES.txt file to write. Defaults to the repository root.

.EXAMPLE
    dotnet restore dscom.sln
    dotnet tool install --global nuget-license
    .\scripts\generate-third-party-notices.ps1
#>

param(
    [string]$OutputPath = (Join-Path $PSScriptRoot ".." "THIRD-PARTY-NOTICES.txt")
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$solutionPath = Join-Path $repoRoot "dscom.sln"
$tempJsonPath = [System.IO.Path]::GetTempFileName()

# Published projects whose restore graph (project.assets.json) is inspected to determine
# which packages actually ship a runtime asset (as opposed to build-time-only tooling
# like MinVer or SourceLink, which only contribute MSBuild targets/props).
$publishedProjectAssetsFiles = @(
    "src/dscom/obj/project.assets.json",
    "src/dscom.client/obj/project.assets.json",
    "src/dscom.build/obj/project.assets.json"
) | ForEach-Object { Join-Path $repoRoot $_ }

function Get-ShippedPackageIds {
    <#
    .SYNOPSIS
        Determines the set of NuGet package IDs that actually ship a runtime/native asset
        in at least one target framework of at least one of the given project.assets.json
        restore lock files.

    .DESCRIPTION
        Packages that are development/build-time-only dependencies (e.g. MinVer,
        Microsoft.SourceLink.*, Microsoft.Build.Tasks.Git) only contribute "build",
        "buildTransitive", "buildMultiTargeting" or "analyzers" assets in the restore
        graph - never "runtime" or "native" assets - so they are never copied to the
        build/publish output and don't need to be included in third-party notices.
        This is determined dynamically from the actual restore graph so it stays correct
        even as dependencies change, without needing a manually maintained ignore list.
    #>
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$AssetsFilePaths
    )

    $shippedPackageIds = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)

    foreach ($assetsFilePath in $AssetsFilePaths) {
        if (-not (Test-Path $assetsFilePath)) {
            throw "Project assets file not found: $assetsFilePath. Run 'dotnet restore dscom.sln' first."
        }

        $assets = Get-Content -Path $assetsFilePath -Raw -Encoding UTF8 | ConvertFrom-Json

        foreach ($targetProperty in $assets.targets.PSObject.Properties) {
            foreach ($packageProperty in $targetProperty.Value.PSObject.Properties) {
                $packageInfo = $packageProperty.Value
                if ($packageInfo.type -ne "package") {
                    continue
                }

                $shipsRuntimeAsset = @("runtime", "native") | Where-Object {
                    $assetGroup = $packageInfo.$_
                    $null -ne $assetGroup -and @($assetGroup.PSObject.Properties).Count -gt 0
                }

                if ($shipsRuntimeAsset) {
                    $packageId = $packageProperty.Name.Split('/')[0]
                    [void]$shippedPackageIds.Add($packageId)
                }
            }
        }
    }

    return $shippedPackageIds
}

function Format-NoticesTable {
    <#
    .SYNOPSIS
        Renders a fixed-width ASCII table (matching the style produced by the
        nuget-license tool's own "Table" output) from a list of PSCustomObjects.
    #>
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Columns,

        [Parameter(Mandatory = $true)]
        [object[]]$Rows
    )

    $widths = @{}
    foreach ($column in $Columns) {
        $widths[$column] = $column.Length
        foreach ($row in $Rows) {
            $value = [string]$row.$column
            if ($value.Length -gt $widths[$column]) {
                $widths[$column] = $value.Length
            }
        }
    }

    $separator = "+" + (($Columns | ForEach-Object { "-" * ($widths[$_] + 2) }) -join "+") + "+"
    $headerLine = "|" + (($Columns | ForEach-Object { " " + $_.PadRight($widths[$_]) + " " }) -join "|") + "|"

    $lines = [System.Collections.Generic.List[string]]::new()
    $lines.Add($separator)
    $lines.Add($headerLine)
    $lines.Add($separator)
    foreach ($row in $Rows) {
        $line = "|" + (($Columns | ForEach-Object { " " + ([string]$row.$_).PadRight($widths[$_]) + " " }) -join "|") + "|"
        $lines.Add($line)
    }
    $lines.Add($separator)

    return ($lines -join "`n") + "`n"
}

try {
    & nuget-license `
        -i $solutionPath `
        -t `
        -exclude-projects "dscom.test*" `
        -o Json `
        -fo $tempJsonPath

    if ($LASTEXITCODE -ne 0) {
        throw "nuget-license failed with exit code $LASTEXITCODE"
    }

    $originNames = @("Expression", "Url", "Unknown", "Ignored", "Overwrite", "File")

    $packages = Get-Content -Path $tempJsonPath -Raw -Encoding UTF8 | ConvertFrom-Json

    # Exclude build-time-only packages (e.g. MinVer, Microsoft.SourceLink.*,
    # Microsoft.Build.Tasks.Git) that never ship a runtime asset and therefore never
    # end up in the published output - see Get-ShippedPackageIds for details.
    $shippedPackageIds = Get-ShippedPackageIds -AssetsFilePaths $publishedProjectAssetsFiles
    $packages = $packages | Where-Object { $shippedPackageIds.Contains($_.PackageId) }

    # The package version is intentionally omitted: it is not required for MIT/Apache-2.0
    # attribution (only the copyright notice and license text are) and including it would
    # force this file to change on every dependency bump. Entries that only differ by
    # version across target frameworks (net8.0/net48) are de-duplicated below.
    $rows = $packages |
        ForEach-Object {
            [PSCustomObject]@{
                Package                    = $_.PackageId
                "License Information Origin" = $originNames[[int]$_.LicenseInformationOrigin]
                "License Expression"       = $_.License
                "License Url"              = $_.LicenseUrl
                Copyright                  = $_.Copyright
                Authors                    = $_.Authors
                "Package Project Url"      = $_.PackageProjectUrl
            }
        } |
        Sort-Object Package, "License Expression", Copyright, Authors, "Package Project Url" -Unique

    $columns = @("Package", "License Information Origin", "License Expression", "License Url", "Copyright", "Authors", "Package Project Url")
    $table = Format-NoticesTable -Columns $columns -Rows $rows

    $header = @"
THIRD-PARTY NOTICES
===================

dscom (dSPACE.Runtime.InteropServices, dscom, dSPACE.Runtime.InteropServices.BuildTasks)
uses third-party NuGet packages that are subject to the following license terms.
The list below covers all direct and transitive package dependencies of the published
projects (src/dscom, src/dscom.client, src/dscom.build) across all target frameworks
(net8.0, net48). Test-only projects are not included, as they are not part of the
published NuGet packages. Build-time-only tooling (e.g. MinVer, Microsoft.SourceLink.*,
Microsoft.Build.Tasks.Git) is also excluded, since those packages never ship a runtime
asset and are therefore never part of the published output.

The full text of each license referenced below (by "License Expression") is
reproduced verbatim in the "FULL LICENSE TEXTS" section at the end of this file.
Per-package copyright notices are listed in the "Copyright" column of the table.
Package versions are intentionally not listed, as they are not required for
attribution and would otherwise change on every dependency update.

This file is auto-generated. Do not edit it manually.

Generation:
  dotnet tool install --global nuget-license
  dotnet restore dscom.sln
  .\scripts\generate-third-party-notices.ps1

Tool: https://github.com/sensslen/nuget-license

"@

    $mitLicenseText = @"
MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
"@

    $apache2LicenseText = @"
Apache License
Version 2.0, January 2004
http://www.apache.org/licenses/

TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION

1. Definitions.

"License" shall mean the terms and conditions for use, reproduction, and
distribution as defined by Sections 1 through 9 of this document.

"Licensor" shall mean the copyright owner or entity authorized by the
copyright owner that is granting the License.

"Legal Entity" shall mean the union of the acting entity and all other
entities that control, are controlled by, or are under common control with
that entity. For the purposes of this definition, "control" means (i) the
power, direct or indirect, to cause the direction or management of such
entity, whether by contract or otherwise, or (ii) ownership of fifty percent
(50%) or more of the outstanding shares, or (iii) beneficial ownership of
such entity.

"You" (or "Your") shall mean an individual or Legal Entity exercising
permissions granted by this License.

"Source" form shall mean the preferred form for making modifications,
including but not limited to software source code, documentation source, and
configuration files.

"Object" form shall mean any form resulting from mechanical transformation or
translation of a Source form, including but not limited to compiled object
code, generated documentation, and conversions to other media types.

"Work" shall mean the work of authorship, whether in Source or Object form,
made available under the License, as indicated by a copyright notice that is
included in or attached to the work (an example is provided in the Appendix
below).

"Derivative Works" shall mean any work, whether in Source or Object form,
that is based on (or derived from) the Work and for which the editorial
revisions, annotations, elaborations, or other modifications represent, as a
whole, an original work of authorship. For the purposes of this License,
Derivative Works shall not include works that remain separable from, or
merely link (or bind by name) to the interfaces of, the Work and Derivative
Works thereof.

"Contribution" shall mean any work of authorship, including the original
version of the Work and any modifications or additions to that Work or
Derivative Works thereof, that is intentionally submitted to Licensor for
inclusion in the Work by the copyright owner or by an individual or Legal
Entity authorized to submit on behalf of the copyright owner. For the
purposes of this definition, "submitted" means any form of electronic,
verbal, or written communication sent to the Licensor or its representatives,
including but not limited to communication on electronic mailing lists,
source code control systems, and issue tracking systems that are managed by,
or on behalf of, the Licensor for the purpose of discussing and improving the
Work, but excluding communication that is conspicuously marked or otherwise
designated in writing by the copyright owner as "Not a Contribution."

"Contributor" shall mean Licensor and any individual or Legal Entity on
behalf of whom a Contribution has been received by Licensor and subsequently
incorporated within the Work.

2. Grant of Copyright License. Subject to the terms and conditions of this
License, each Contributor hereby grants to You a perpetual, worldwide,
non-exclusive, no-charge, royalty-free, irrevocable copyright license to
reproduce, prepare Derivative Works of, publicly display, publicly perform,
sublicense, and distribute the Work and such Derivative Works in Source or
Object form.

3. Grant of Patent License. Subject to the terms and conditions of this
License, each Contributor hereby grants to You a perpetual, worldwide,
non-exclusive, no-charge, royalty-free, irrevocable (except as stated in this
section) patent license to make, have made, use, offer to sell, sell,
import, and otherwise transfer the Work, where such license applies only to
those patent claims licensable by such Contributor that are necessarily
infringed by their Contribution(s) alone or by combination of their
Contribution(s) with the Work to which such Contribution(s) was submitted. If
You institute patent litigation against any entity (including a cross-claim
or counterclaim in a lawsuit) alleging that the Work or a Contribution
incorporated within the Work constitutes direct or contributory patent
infringement, then any patent licenses granted to You under this License for
that Work shall terminate as of the date such litigation is filed.

4. Redistribution. You may reproduce and distribute copies of the Work or
Derivative Works thereof in any medium, with or without modifications, and in
Source or Object form, provided that You meet the following conditions:

(a) You must give any other recipients of the Work or Derivative Works a
    copy of this License; and

(b) You must cause any modified files to carry prominent notices stating
    that You changed the files; and

(c) You must retain, in the Source form of any Derivative Works that You
    distribute, all copyright, patent, trademark, and attribution notices
    from the Source form of the Work, excluding those notices that do not
    pertain to any part of the Derivative Works; and

(d) If the Work includes a "NOTICE" text file as part of its distribution,
    then any Derivative Works that You distribute must include a readable
    copy of the attribution notices contained within such NOTICE file,
    excluding those notices that do not pertain to any part of the
    Derivative Works, in at least one of the following places: within a
    NOTICE text file distributed as part of the Derivative Works; within
    the Source form or documentation, if provided along with the Derivative
    Works; or, within a display generated by the Derivative Works, if and
    wherever such third-party notices normally appear. The contents of the
    NOTICE file are for informational purposes only and do not modify the
    License. You may add Your own attribution notices within Derivative
    Works that You distribute, alongside or as an addendum to the NOTICE
    text from the Work, provided that such additional attribution notices
    cannot be construed as modifying the License.

You may add Your own copyright statement to Your modifications and may
provide additional or different license terms and conditions for use,
reproduction, or distribution of Your modifications, or for any such
Derivative Works as a whole, provided Your use, reproduction, and
distribution of the Work otherwise complies with the conditions stated in
this License.

5. Submission of Contributions. Unless You explicitly state otherwise, any
Contribution intentionally submitted for inclusion in the Work by You to the
Licensor shall be under the terms and conditions of this License, without
any additional terms or conditions. Notwithstanding the above, nothing
herein shall supersede or modify the terms of any separate license agreement
you may have executed with Licensor regarding such Contributions.

6. Trademarks. This License does not grant permission to use the trade
names, trademarks, service marks, or product names of the Licensor, except
as required for reasonable and customary use in describing the origin of
the Work and reproducing the content of the NOTICE file.

7. Disclaimer of Warranty. Unless required by applicable law or agreed to in
writing, Licensor provides the Work (and each Contributor provides its
Contributions) on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, either express or implied, including, without limitation, any
warranties or conditions of TITLE, NON-INFRINGEMENT, MERCHANTABILITY, or
FITNESS FOR A PARTICULAR PURPOSE. You are solely responsible for determining
the appropriateness of using or redistributing the Work and assume any risks
associated with Your exercise of permissions under this License.

8. Limitation of Liability. In no event and under no legal theory, whether
in tort (including negligence), contract, or otherwise, unless required by
applicable law (such as deliberate and grossly negligent acts) or agreed to
in writing, shall any Contributor be liable to You for damages, including
any direct, indirect, special, incidental, or consequential damages of any
character arising as a result of this License or out of the use or
inability to use the Work (including but not limited to damages for loss of
goodwill, work stoppage, computer failure or malfunction, or any and all
other commercial damages or losses), even if such Contributor has been
advised of the possibility of such damages.

9. Accepting Warranty or Additional Liability. While redistributing the Work
or Derivative Works thereof, You may choose to offer, and charge a fee for,
acceptance of support, warranty, indemnity, or other liability obligations
and/or rights consistent with this License. However, in accepting such
obligations, You may act only on Your own behalf and on Your sole
responsibility, not on behalf of any other Contributor, and only if You
agree to indemnify, defend, and hold each Contributor harmless for any
liability incurred by, or claims asserted against, such Contributor by
reason of your accepting any such warranty or additional liability.

END OF TERMS AND CONDITIONS
"@

    # Only the license texts actually referenced by a package that ships in the published
    # output are included below, so this section shrinks/grows automatically as the
    # licenses in use change (e.g. removing the last Apache-2.0-licensed dependency drops
    # the Apache-2.0 text too).
    $licenseTextsByExpression = [ordered]@{
        "MIT"         = $mitLicenseText
        "Apache-2.0"  = $apache2LicenseText
    }

    $usedLicenseExpressions = $rows | ForEach-Object { $_."License Expression" } | Where-Object { $_ } | Sort-Object -Unique

    $licenseTextsSectionBuilder = [System.Text.StringBuilder]::new()
    [void]$licenseTextsSectionBuilder.Append("`n`nFULL LICENSE TEXTS`n===================`n")
    foreach ($licenseKey in $licenseTextsByExpression.Keys) {
        $isReferenced = $usedLicenseExpressions | Where-Object { $_ -match [regex]::Escape($licenseKey) }
        if ($isReferenced) {
            [void]$licenseTextsSectionBuilder.Append("`n--- $licenseKey ---`n`n")
            [void]$licenseTextsSectionBuilder.Append($licenseTextsByExpression[$licenseKey])
            [void]$licenseTextsSectionBuilder.Append("`n")
        }
    }
    $licenseTextsSection = $licenseTextsSectionBuilder.ToString()

    $content = $header + $table + $licenseTextsSection

    # Normalize to LF line endings regardless of the OS/runner this script executes on,
    # or the line endings embedded in this script's own here-strings. This keeps the
    # generated file byte-for-byte reproducible so the CI "is it up to date" diff check
    # (which does a plain git diff, unaffected by core.autocrlf since .gitattributes pins
    # this file to LF) never fails due to line-ending differences alone.
    $content = $content -replace "`r`n", "`n" -replace "`r", "`n"

    [System.IO.File]::WriteAllText($OutputPath, $content, (New-Object System.Text.UTF8Encoding($false)))

    Write-Host "Wrote $OutputPath"
}
finally {
    Remove-Item -Path $tempJsonPath -ErrorAction SilentlyContinue
}
