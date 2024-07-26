# UltimateGameOfLife
Coolest Game of Life EVER

This project utilizes PIPES to send commands, and reload the following specified patterns:

BEACON
BLINKER
RANDOM
TOAD

Example command:

powershell -Command "$command = 'BEACON'; $pipe = new-object System.IO.Pipes.NamedPipeClientStream('.', 'GOLPipeCommandCenter', [System.IO.Pipes.PipeDirection]::Out); $pipe.Connect(2000); $writer = new-object System.IO.StreamWriter($pipe); $writer.WriteLine($command); $writer.Flush(); $pipe.Close(); Write-Host 'Command sent: ' $command"