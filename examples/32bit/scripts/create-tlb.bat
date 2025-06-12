@echo off

SET dscom=%~dp0\..\bin\dscom.exe
SET dscom32=%~dp0\..\bin\dscom32.exe

if not exist %dscom% (
    echo dscom.exe not found 
    echo Please download the dscom.exe and dscom32.exe from https://github.com/dspace-group/dscom/releases to the script folder
    exit 1
)

if not exist %dscom32% (
    echo dscom32.exe not found 
    echo Please download the dscom.exe and dscom32.exe from https://github.com/dspace-group/dscom/releases to the script folder
    exit 1
)

dotnet build --framework net48 -c Release %~dp0\..\comtestdotnet\comtestdotnet.csproj
dotnet build --framework net8.0 -c Release %~dp0\..\comtestdotnet\comtestdotnet.csproj

SET dll60=%~dp0\..\comtestdotnet\bin\Release\net8.0\comtestdotnet.dll
SET dll48=%~dp0\..\comtestdotnet\bin\Release\net48\comtestdotnet.dll

%dscom% tlbexport %dll60% --out %~dp0\tlb-comtestdotnet-dscom-64.tlb
%dscom32% tlbexport %dll60%  --out %~dp0\tlb-comtestdotnet-dscom-32.tlb
tlbexp.exe /win32 %dll48% /out:%~dp0\tlb-comtestdotnet-tlbexp-32.tlb
tlbexp.exe /win64 %dll48% /out:%~dp0\tlb-comtestdotnet-tlbexp-64.tlb

%dscom% tlbdump %~dp0\tlb-comtestdotnet-dscom-64.tlb --out %~dp0\tlb-comtestdotnet-dscom64.tlb.yaml
%dscom32% tlbdump %~dp0\tlb-comtestdotnet-dscom-32.tlb  --out %~dp0\tlb-comtestdotnet-dscom-32.tlb.yaml
%dscom% tlbdump %~dp0\tlb-comtestdotnet-tlbexp-64.tlb --out %~dp0\tlb-comtestdotnet-tlbexp-64.tlb.yaml
%dscom32% tlbdump %~dp0\tlb-comtestdotnet-tlbexp-32.tlb  --out %~dp0\tlb-comtestdotnet-tlbexp-32.tlb.yaml

pause