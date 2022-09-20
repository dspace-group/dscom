import win32com.client

application = win32com.client.Dispatch("Greeter.Net48")

print(application.SayHello("from Python"))
