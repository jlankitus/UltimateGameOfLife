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
    private NamedPipeServerStream pipeServer;
    private StreamWriter writer;

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
            using (pipeServer = new NamedPipeServerStream("GOLPipeCommandCenter", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            {
                Debug.Log("Named pipe server started, waiting for connection...");
                pipeServer.WaitForConnection();

                using (var reader = new StreamReader(pipeServer))
                {
                    writer = new StreamWriter(pipeServer) { AutoFlush = true };

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
            Debug.Log("Processing command: " + command);
            string gridState = ProcessCommand(command);
            SendGridState(gridState);
        }
    }

    string ProcessCommand(string command)
    {
        return lifeGenerator.GeneratePattern(command);
    }

    void SendGridState(string gridState)
    {
        try
        {
            if (pipeServer.IsConnected && writer != null)
            {
                Debug.Log("Sending grid state:\n" + gridState);
                writer.WriteLine(gridState);
                writer.Flush();
            }
        }
        catch (IOException ex)
        {
            Debug.LogError("IOException during SendGridState: " + ex.Message);
            // Handle the broken pipe case here (e.g., close the pipe, notify the client, etc.)
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        if (pipeServer != null)
        {
            pipeServer.Close();
            pipeServer = null;
        }
        if (writer != null)
        {
            writer.Close();
            writer = null;
        }
        pipeThread.Abort();
    }
}
