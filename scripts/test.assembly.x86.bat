@echo off 

SET workspace=%~dp0..\
@REM set filterregex=--filterregex \.file\= --filterregex \.attributes\.guid\= --filterregex numberOfImplementedInterfaces --filterregex implementedInterfaces
set filterregex=

SET net80dll=%workspace%src\dscom.test.assembly\bin\Release\net8.0\dSPACE.Runtime.InteropServices.Test.Assembly.dll
SET net48dll=%workspace%src\dscom.test.assembly\bin\Release\net48\dSPACE.Runtime.InteropServices.Test.Assembly.dll

del %workspace%src\dscom.test.assembly\bin\Release\net48\*.tlb 
del %workspace%src\dscom.test.assembly\bin\Release\net8.0\*.tlb 
del %workspace%src\dscom.test.assembly\bin\Release\net8.0\*.yaml
del %workspace%src\dscom.test.assembly\bin\Release\net48\*.yaml

dotnet build -c Release "%workspace%dscom.sln"
IF ERRORLEVEL 1 goto error

@REM dscom
echo ############## dscom.exe tlbexport
dotnet run --project %workspace%src\dscom.client\dscom.client.csproj -r win-x86 -f net8.0 --no-self-contained -- tlbexport --silent "%net80dll%" "--out" "%net80dll%.tlb"

IF ERRORLEVEL 1 goto error

echo ############## dscom.exe tlbdump
dotnet run --project %workspace%src\dscom.client\dscom.client.csproj -r win-x86 -f net8.0 --no-self-contained -- tlbdump %filterregex% "/tlbrefpath:%net80dll%.tlb/.." "%net80dll%.tlb" "/out:%net80dll%.yaml"
IF ERRORLEVEL 1 goto error

WHERE tlbexp
IF %ERRORLEVEL% NEQ 0 (
    ECHO.
    ECHO ######################################################
    ECHO.
    ECHO tlbexp.exe not found
    ECHO Please add the path to tlbexp.exe to your PATH variable.
    ECHO.
    ECHO ######################################################
    ECHO.
    goto error
)

@REM tlbexp
echo ############## tlbexp.exe
tlbexp /win32 /silent "%net48dll%" "/out:%net48dll%.tlb"
IF ERRORLEVEL 1 goto error

echo ############## dscom.exe tlbdump
dotnet run --project %workspace%src\dscom.client\dscom.client.csproj -r win-x86 -f net8.0 --no-self-contained tlbdump %filterregex% "/tlbrefpath:%net48dll%.tlb/.." "%net48dll%.tlb" "/out:%net48dll%.yaml"
IF ERRORLEVEL 1 goto error

WHERE code
IF %ERRORLEVEL% NEQ 0 (
    ECHO.
    ECHO ######################################################
    ECHO.
    ECHO You don't have Visual Studio Code installed, too bad.
    ECHO https://code.visualstudio.com/download
    ECHO.
    ECHO Please compare the following files:
    ECHO %net48dll%.yaml 
    ECHO %net80dll%.yaml
    ECHO.
    ECHO ######################################################
    ECHO.
    goto error
)

code -d "%net48dll%.yaml" "%net80dll%.yaml"

:error
    pause