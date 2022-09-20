import win32com.client

application = win32com.client.Dispatch("Greeter.Dotnet6")

print(application.SayHello("from Python"))
