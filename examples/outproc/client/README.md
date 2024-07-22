# Python COM Client

## Prepare

The COM server must be started first.  
Either the dotnet 6 or the dotnet 4.8 server.  

Create a virtual Python environment:  

```bash
python -m venv .venv
```

Activate the environment:  

```bash
.venv\Scripts\activate.bat
```

Install a requirements:  

```bash
pip install -r requirements.txt
```

## Run

```bash
# For a dotnet 6 server
python app-dotnet6.py 

# For a dotnet 4.8 server
python app-dotnet48.py
```
