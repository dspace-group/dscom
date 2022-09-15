import win32com.client

application = win32com.client.Dispatch("Greeter.Application")

print(application.SayHello("from Python"))
