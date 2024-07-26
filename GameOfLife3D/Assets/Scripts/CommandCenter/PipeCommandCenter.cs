using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using UnityEngine;

public class PipeCommandCenter : MonoBehaviour
{
    [SerializeField]
    private LifeGenerator lifeGenerator;

    private Thread pipeThread;
    private bool isRunning = true;
    private ConcurrentQueue<string> commandQueue = new ConcurrentQueue<string>();

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
                        commandQueue.Enqueue(line);
                    }
                }
            }
        }
    }

    void Update()
    {
        while (commandQueue.TryDequeue(out string command))
        {
            ProcessCommand(command);
        }
    }

    void ProcessCommand(string command)
    {
        lifeGenerator.GeneratePattern(command);
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        pipeThread.Abort();
    }
}
