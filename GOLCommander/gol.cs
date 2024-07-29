using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace gol
{
    class gol
    {
        static void Main(string[] args)
        {
            string command;
            if (args.Length == 0)
            {
                command = "RANDOM";
            }
            else if(args.Length == 1) 
            {
                command = args[0];
            }
            else
            {
                Console.WriteLine("Alas, you cannot fool me, RANDOM TIME!");
                command = "RANDOM";
            }

            string unityAppPath = Path.Combine(Directory.GetCurrentDirectory(), "GameOfLife3D.exe");
            string unityProcessName = "GameOfLife3D"; // Without .exe

            try
            {
                // Check if the Unity application is already running
                Process[] runningUnityProcesses = Process.GetProcessesByName(unityProcessName);
                if (runningUnityProcesses.Length == 0)
                {
                    // Start the Unity application if not running
                    Process unityProcess = new Process();
                    unityProcess.StartInfo.FileName = unityAppPath;
                    unityProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(unityAppPath);
                    unityProcess.Start();

                    // Give the Unity application some time to initialize
                    Thread.Sleep(10000); // Wait for 10 seconds, adjust as needed
                }

                // Connect to the named pipe server
                using (var pipeClient = new NamedPipeClientStream(".", "GOLPipeCommandCenter", PipeDirection.InOut))
                {
                    pipeClient.Connect(2000);

                    using (var writer = new StreamWriter(pipeClient))
                    using (var reader = new StreamReader(pipeClient))
                    {
                        writer.AutoFlush = true;

                        // Send command
                        writer.WriteLine(command);

                        // Read response
                        string response = string.Empty;
                        string line;
                        while ((line = reader.ReadLine()) != null && line != "<EOF>")
                        {
                            response += line + Environment.NewLine;
                        }

                        Console.WriteLine(response);
                    }
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
