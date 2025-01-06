# COM OutProc Server Demo

## Introduction

This is a simple demo of an out-of-process COM server. The server is implemented in C# and the client is implemented in Python.  
The `OutProcServer` class shows how to register a .NET type for use by COM clients as a local server.  
The local server is implemented as a console application that listens for incoming connections from COM clients.  
The new instance of the .NET type is registered as `MultipleUse`, which means that a client uses the same process to call methods on the .NET type.

## Pre-requisites

- .NET >= 8.0 SDK
- Python => 3.9

## Run

```powershell
dotnet publish
```

Open a new PowerShell window **as an administrator** and run the following command:  

```powershell
.\server\bin\Release\net8.0\publish\server.exe /regserver
```

Now the server is registered and ready to be used by COM clients.

## Client

Open a new PowerShell window as a regular user and run the following commands:  

```powershell
python -m venv .venv
.\.venv\Scripts\Activate.ps1
pip install -r .\client\requirements.txt
```

Now you can run the client:  

```cmd
python .\client\client.py
```
