# UltimateGameOfLife
Coolest Game of Life EVER

This project utilizes PIPES to send commands, and reload the following specified patterns:

BEACON
BLINKER
RANDOM
TOAD

Example command:

powershell -Command "$command = 'RaNd0m'; $pipe = new-object System.IO.Pipes.NamedPipeClientStream('.', 'GOLPipeCommandCenter', [System.IO.Pipes.PipeDirection]::InOut); $pipe.Connect(2000); $writer = new-object System.IO.StreamWriter($pipe); $reader = new-object System.IO.StreamReader($pipe); $writer.WriteLine($command); $writer.Flush(); $response = ''; while (($line = $reader.ReadLine()) -ne '<EOF>') { $response += $line + [Environment]::NewLine }; $pipe.Close(); Write-Host 'Command sent: ' $command; Write-Host 'Response received: '; Write-Host $response"

powershell -Command "$command = 'toad'; $pipe = new-object System.IO.Pipes.NamedPipeClientStream('.', 'GOLPipeCommandCenter', [System.IO.Pipes.PipeDirection]::InOut); $pipe.Connect(2000); $writer = new-object System.IO.StreamWriter($pipe); $reader = new-object System.IO.StreamReader($pipe); $writer.WriteLine($command); $writer.Flush(); $response = ''; while (($line = $reader.ReadLine()) -ne '<EOF>') { $response += $line + [Environment]::NewLine }; $pipe.Close(); Write-Host 'Command sent: ' $command; Write-Host 'Response received: '; Write-Host $response"

powershell -Command "$command = 'blinker'; $pipe = new-object System.IO.Pipes.NamedPipeClientStream('.', 'GOLPipeCommandCenter', [System.IO.Pipes.PipeDirection]::InOut); $pipe.Connect(2000); $writer = new-object System.IO.StreamWriter($pipe); $reader = new-object System.IO.StreamReader($pipe); $writer.WriteLine($command); $writer.Flush(); $response = ''; while (($line = $reader.ReadLine()) -ne '<EOF>') { $response += $line + [Environment]::NewLine }; $pipe.Close(); Write-Host 'Command sent: ' $command; Write-Host 'Response received: '; Write-Host $response"

powershell -Command "$command = 'BEACON'; $pipe = new-object System.IO.Pipes.NamedPipeClientStream('.', 'GOLPipeCommandCenter', [System.IO.Pipes.PipeDirection]::InOut); $pipe.Connect(2000); $writer = new-object System.IO.StreamWriter($pipe); $reader = new-object System.IO.StreamReader($pipe); $writer.WriteLine($command); $writer.Flush(); $response = ''; while (($line = $reader.ReadLine()) -ne '<EOF>') { $response += $line + [Environment]::NewLine }; $pipe.Close(); Write-Host 'Command sent: ' $command; Write-Host 'Response received: '; Write-Host $response"
