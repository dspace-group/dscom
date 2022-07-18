@echo off

SET PWD=%~dp0

dotnet watch test -f net6.0 -c Release --project %PWD%..\src\dscom.test\dscom.test.csproj