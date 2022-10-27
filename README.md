# Example use

In the project we can se 3 folder 

- Demo → Contains a test console app to send message **through** named pipe.
- RS_SDK → Contains the logic of Client / Server pipe comunication.
- RootSystemService → Contains a WindowsService that start a pipe server.

## How to use :

0) Start VisualStudio with Administrator grant

1) Build the project in Release mode

2) Install windows Service

```json
cd .\bin\Release\
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe RootSystemService.exe

(Adjust the Framework64 version)

To unistall :
C:\Windows\Microsoft.NET\Framework64\_version_\InstallUtil.exe -u RootSystemService.exe

```

3)Start RootSystemService using “Services” windows app .

![Untitled](Example%20use%20a5780be3ae7c46e2986e92e84f19cd07/Untitled.png)

4)Start demo program to send message 

```json
IPipeClient _client = new NPClient("elis_pipe");

_client.Start();

PipeMessage pipe = new PipeMessage("request", "client message");

_client.SendMessage(pipe);

```

The server will answer like “echo” object, so it return the same message with topic : echo_reply