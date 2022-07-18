@echo off 

SET workspace=%~dp0..\
@REM set filterregex=--filterregex \.file\= --filterregex \.attributes\.guid\= --filterregex numberOfImplementedInterfaces --filterregex implementedInterfaces
set filterregex=

SET net60dll=%workspace%src\dscom.demo\assembly1\bin\Release\net6.0-windows\dSPACE.Runtime.InteropServices.DemoAssembly1.dll
SET net48dll=%workspace%src\dscom.demo\assembly1\bin\Release\net48-windows\dSPACE.Runtime.InteropServices.DemoAssembly1.dll

del %workspace%src\dscom.demo\assembly1\bin\Release\net48-windows\*.tlb 
del %workspace%src\dscom.demo\assembly1\bin\Release\net6.0-windows\*.tlb 
del %workspace%src\dscom.demo\assembly1\bin\Release\net6.0-windows\*.yaml
del %workspace%src\dscom.demo\assembly1\bin\Release\net48-windows\*.yaml

dotnet build -c Release "%workspace%dscom.sln"
IF ERRORLEVEL 1 goto error

@REM dscom
echo ############## dscom.exe tlbexport
%workspace%src\dscom.client\bin\Release\net6.0-windows\dscom.exe tlbexport /verbose "%net60dll%" "/out:%net60dll%.tlb"
IF ERRORLEVEL 1 goto error

echo ############## dscom.exe tlbdump
%workspace%src\dscom.client\bin\Release\net6.0-windows\dscom.exe tlbdump %filterregex% "/tlbrefpath:%net60dll%.tlb/.." "%net60dll%.tlb" "/out:%net60dll%.yaml"
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
tlbexp /win64 /verbose "%net48dll%" "/out:%net48dll%.tlb"
IF ERRORLEVEL 1 goto error

echo ############## dscom.exe tlbdump
%workspace%src\dscom.client\bin\Release\net6.0-windows\dscom.exe tlbdump %filterregex% "/tlbrefpath:%net48dll%.tlb/.." "%net48dll%.tlb" "/out:%net48dll%.yaml"
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
    ECHO %net60dll%.yaml
    ECHO.
    ECHO ######################################################
    ECHO.
    goto error
)

code -d "%net48dll%.yaml" "%net60dll%.yaml"

:error
    pause