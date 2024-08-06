using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using UnityEngine;

// This is our command center class that allows us to talk and listen to other programs using pipes
// The pipe name "GOLPipeCommandCenter" must match
// https://www.youtube.com/watch?v=nFnomZDaCC8&ab_channel=CodeMonkey

public class PipeCommandCenter : MonoBehaviour
{
    // Life Generator converts raw data sent through the pipe into animated 3D
    [SerializeField]
    private LifeGenerator lifeGenerator;

    [SerializeField]
    private string pipeServerName = "GOLPipeCommandCenter";

    private Thread pipeThread;
    private bool isRunning = true;
    private ConcurrentQueue<string> commandQueue = new ConcurrentQueue<string>();
    private NamedPipeServerStream pipeServer;
    private StreamWriter writer;

    // Run the pipe server as a background thread. This allows us to take multiple commands without waiting
    // on the main thread.
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
            // InOut pipeserver to send and receive messages
            using (pipeServer = new NamedPipeServerStream(pipeServerName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
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

    // Process all commands every frame.
    // Could opt to process one command per frame if 'ProcessCommand' became more expensive or running on low end hardware
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

    // Sends the grid state back through the pipe (to our console app or command line)
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
            // TODO: Handle the broken pipe case here
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

    [ContextMenu("GenerateToad")]
    public void GenerateToad()
    {
        lifeGenerator.GeneratePattern("TOAD");
    }

    [ContextMenu("GenerateRandom")]
    public void GenerateRandom()
    {
        lifeGenerator.GeneratePattern("RANDOM");
    }
}
