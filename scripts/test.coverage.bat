@echo off

SET PWD=%~dp0
SET TEST_RESULTS=%PWD%\..\test.results

IF NOT EXIST %TEST_RESULTS% (
    mkdir %TEST_RESULTS%
)

dotnet tool install -g dotnet-reportgenerator-globaltool

dotnet test -f net6.0 -c Release /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=%TEST_RESULTS%\lcov.info --logger "trx;LogFileName=%TEST_RESULTS%\results.xml" %PWD%..\dscom.sln 
COPY %TEST_RESULTS%\lcov.net6.0.info %TEST_RESULTS%\lcov.info

dotnet test  -f net6.0 -c Release /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput="%TEST_RESULTS%\\"  %PWD%..\dscom.sln 
reportgenerator -reports:"%TEST_RESULTS%\coverage.net6.0.cobertura.xml" -targetdir:"%TEST_RESULTS%\report" -reporttypes:Html