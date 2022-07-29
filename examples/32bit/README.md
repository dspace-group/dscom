# 32bit COM example application

This example shows how to build a 32bit application with a .NET COM server and a C++ client.

## 1
Create a `bin` folder.

## 2
Download the `dscom32.exe` and the `dscom.exe` <https://github.com/dspace-group/dscom/releases> to `bin`.

## 3
```bash
dotnet build -r win-x86  --no-self-contained .\comtestdotnet\comtestdotnet.csproj
``` 

## 4
Register the TLB as **Adminstrator** :  
 ```bash
 .\bin\dscom32.exe tlbregister <YOUR-REPO-PATH>\comtestdotnet\comtestdotnet.tlb
 ```

## 5
Register the COM server as **Adminstrator** :  
```bash
 regsvr32.exe <YOUR-REPO-PATH>\comtestdotnet\bin\Debug\net6.0\win-x86\comtestdotnet.comhost.dll
```

## 6
Open Visual Studio 2022.  
Compile and run the application as **x86**.



