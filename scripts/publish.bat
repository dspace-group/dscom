@echo off

FOR %%A IN ("%~dp0.") DO set workspace=%%~dpA

IF NOT EXIST "%workspace%.publish" (
    mkdir "%workspace%.publish"
)

dotnet publish "%workspace%src\dscom.client\dscom.client.csproj" --no-self-contained -c Release -r win-x86 -f net6.0 /p:PublishSingleFile=true
copy "%workspace%src\dscom.client\bin\Release\net6.0\win-x86\publish\dscom.exe" "%workspace%.publish\dscom32.exe"

dotnet publish "%workspace%src\dscom.client\dscom.client.csproj" --no-self-contained -c Release -r win-x64 -f net6.0 /p:PublishSingleFile=true
copy "%workspace%src\dscom.client\bin\Release\net6.0\win-x64\publish\dscom.exe" "%workspace%.publish\dscom.exe"

echo.
echo.
echo "%workspace%.publish\dscom32.exe"
echo "%workspace%.publish\dscom.exe"
echo.
