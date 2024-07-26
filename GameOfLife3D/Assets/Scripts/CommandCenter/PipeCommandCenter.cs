using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using UnityEngine;

public class PipeCommandCenter : MonoBehaviour
{
    private Thread pipeThread;
    private bool isRunning = true;

    void Start()
    {
        pipeThread = new Thread(StartNamedPipeServer);
        pipeThread.IsBackground = true;
        pipeThread.Start();
    }

    void StartNamedPipeServer()
    {
        while (isRunning)
        {
            using (var pipeServer = new NamedPipeServerStream("GOLPipeCommandCenter", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            {
                Debug.Log("Named pipe server started, waiting for connection...");
                pipeServer.WaitForConnection();

                using (var reader = new StreamReader(pipeServer))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Debug.Log("Received command: " + line);
                        ProcessCommand(line);
                    }
                }
            }
        }
    }

    void ProcessCommand(string command)
    {
        switch (command.ToUpper())
        {
            case "BEACON":
                StartGameOfLife("BEACON");
                break;
            case "BLINKER":
                StartGameOfLife("BLINKER");
                break;
            case "TOAD":
                StartGameOfLife("TOAD");
                break;
            case "RANDOM":
            default:
                StartGameOfLife("RANDOM");
                break;
        }
    }

    void StartGameOfLife(string pattern)
    {
        // TODO: actual coding challenge here!
        Debug.Log($"Starting Game of Life with pattern: {pattern}");
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        pipeThread.Abort();
    }
}
