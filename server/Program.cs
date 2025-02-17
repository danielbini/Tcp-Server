using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class TcpServer
{
    private const int Port = 3000;
    private static readonly object _lock = new object();

    static void Main(string[] args)
    {
        TcpListener server = new TcpListener(IPAddress.Any, Port);
        server.Start();
        Console.WriteLine("Server started on port " + Port);

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("New client connected.");

            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(client);
        }
    }

    private static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        try
        {
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received: " + request);

                string response = ProcessRequest(request);
                byte[] responseBytes = Encoding.ASCII.GetBytes(response);

                lock (_lock)
                {
                    stream.Write(responseBytes, 0, responseBytes.Length);
                    Console.WriteLine("Sent: " + response);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            client.Close();
            Console.WriteLine("Client disconnected.");
        }
    }

    private static string ProcessRequest(string request)
    {
        switch (request.Trim().ToUpper())
        {
            case "GET_TEMP":
                return "Temperature: 24°C";
            case "GET_STATUS":
                return "Status: Active";
            default:
                return "Error: Unknown command";
        }
    }
}