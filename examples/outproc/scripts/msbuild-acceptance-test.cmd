@ECHO off

SET root=%~dp0\..\..\..\

PUSHD %~dp0\..

dotnet build

POPD

SetLocal EnableDelayedExpansion

SET EXITCODE=0

IF NOT "%ERRORLEVEL%" == "0" (
  SET EXITCODE=1
  ECHO "acceptance test failed."
)

EXIT /B %EXITCODE%
