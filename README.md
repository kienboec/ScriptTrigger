This project is intended to automate developer's testing effort by automation. 
        
Feel free to contribute. 

## State of work
This project is still "work in progress".

## Installation
1) 	You need .net core 3.1 installed on your machine \
	see: https://dotnet.microsoft.com/download/dotnet-core/3.1
2) 	download the latest version from the releases section \
	see: https://github.com/kienboec/ScriptTrigger/releases
3)	unzip and run
4)	read the docs \
	see: https://kienboec.github.io/ScriptTrigger/

## Command line
```
-v=[VALUE      ] sets the value (string)
-s=[SCRIPTNAME ] sets the script name (use absolute paths)
-t=[TRIGGERNAME] sets the trigger-value (see below)
-l=[True|False ] sets whether to listen on startup or not
```
                
TriggerValues:
- OpenPort  ... checks if the application can connect to a port using tcp

