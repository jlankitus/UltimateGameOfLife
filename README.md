# UltimateGameOfLife
Coolest Game of Life EVER

This project utilizes PIPES to send commands, and reload the following specified patterns:

BEACON
BLINKER
RANDOM
TOAD

It is separated into two projects:
1. GOLCommander - Simple C# console application that builds to gol.exe
2. GameOfLife3D - Unity 3D project that is launched from GOLCommander

Usage
"gol (pattern)"

<br/>

## Unity Breakdown:

![1 CommandCenter](https://github.com/user-attachments/assets/06655976-7af6-4654-a068-4cfea04734b5)

The command center does the fancy pipework and listens for the GOLCommander to regenerate the Game.
TODO: if you run commands while 3D is animating it gets....interesting....fortunately it retains console output, so sanity 
can be upheld. I may have left this for fun...or to be...pragmatic.

![2  LifeGenerator](https://github.com/user-attachments/assets/71bc6623-7dc5-4e2b-a581-1548b4c433a9)

This is the core script that manages the formatting / printing, as well as the 3D generation.

![3  LifePattern](https://github.com/user-attachments/assets/18d09427-7db4-44e1-8a83-75099b4c43ae)

This is a fun use of scriptable objects with a fancy editor so we can "draw" initial states in rather than ugly arrays.
This also allows us to attach a 'dead' and 'alive' prefab, which gives us our fun 3D states. 
This also allows us to perfectly match the initial 'Random' test case, while also actually being random in subsequent runs.

![4 ScriptableArchitecture](https://github.com/user-attachments/assets/3be3807f-d384-4df2-8d16-d9bd58e40f29)

'Toad' makes frogs, beacon creates torches, blinkers make flashlights, and random creates...CTV's!
Using scriptable objects allows us to quickly generate more and more patterns as we please without much tech skill.

![GridManager](https://github.com/user-attachments/assets/6f1da73b-04ac-4bb0-83ba-b096e197feb3)

This has all the fun core GOL algorithms, searching our neighbors for who is alive and dead!

<br />
<br />

### Powershell Testing Commands [devs only]

powershell -Command "$command = 'BEACON'; $pipe = new-object System.IO.Pipes.NamedPipeClientStream('.', 'GOLPipeCommandCenter', [System.IO.Pipes.PipeDirection]::InOut); $pipe.Connect(2000); $writer = new-object System.IO.StreamWriter($pipe); $reader = new-object System.IO.StreamReader($pipe); $writer.WriteLine($command); $writer.Flush(); $response = ''; while (($line = $reader.ReadLine()) -ne '<EOF>') { $response += $line + [Environment]::NewLine }; $pipe.Close(); Write-Host 'Command sent: ' $command; Write-Host 'Response received: '; Write-Host $response"

powershell -Command "$command = 'BLINKER'; $pipe = new-object System.IO.Pipes.NamedPipeClientStream('.', 'GOLPipeCommandCenter', [System.IO.Pipes.PipeDirection]::InOut); $pipe.Connect(2000); $writer = new-object System.IO.StreamWriter($pipe); $reader = new-object System.IO.StreamReader($pipe); $writer.WriteLine($command); $writer.Flush(); $response = ''; while (($line = $reader.ReadLine()) -ne '<EOF>') { $response += $line + [Environment]::NewLine }; $pipe.Close(); Write-Host 'Command sent: ' $command; Write-Host 'Response received: '; Write-Host $response"

powershell -Command "$command = 'RANDOM'; $pipe = new-object System.IO.Pipes.NamedPipeClientStream('.', 'GOLPipeCommandCenter', [System.IO.Pipes.PipeDirection]::InOut); $pipe.Connect(2000); $writer = new-object System.IO.StreamWriter($pipe); $reader = new-object System.IO.StreamReader($pipe); $writer.WriteLine($command); $writer.Flush(); $response = ''; while (($line = $reader.ReadLine()) -ne '<EOF>') { $response += $line + [Environment]::NewLine }; $pipe.Close(); Write-Host 'Command sent: ' $command; Write-Host 'Response received: '; Write-Host $response"

powershell -Command "$command = 'TOAD'; $pipe = new-object System.IO.Pipes.NamedPipeClientStream('.', 'GOLPipeCommandCenter', [System.IO.Pipes.PipeDirection]::InOut); $pipe.Connect(2000); $writer = new-object System.IO.StreamWriter($pipe); $reader = new-object System.IO.StreamReader($pipe); $writer.WriteLine($command); $writer.Flush(); $response = ''; while (($line = $reader.ReadLine()) -ne '<EOF>') { $response += $line + [Environment]::NewLine }; $pipe.Close(); Write-Host 'Command sent: ' $command; Write-Host 'Response received: '; Write-Host $response"


