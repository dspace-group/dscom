@echo off

SET PWD=%~dp0
SET TEST_RESULTS=%PWD%\..\test.results

IF NOT EXIST %TEST_RESULTS% (
    mkdir %TEST_RESULTS%
)

dotnet test -f net6.0 -c Release /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=%TEST_RESULTS%\lcov.info --logger "trx;LogFileName=%TEST_RESULTS%\results.xml" %PWD%..\dscom.sln 
COPY %TEST_RESULTS%\lcov.net6.0.info %TEST_RESULTS%\lcov.info