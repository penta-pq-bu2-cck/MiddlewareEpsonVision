using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TcpServer : IDisposable
{
    private TcpListener _listener;
    private TcpClient _client;
    private NetworkStream _stream;

    public void Start(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();

        UiLogger.Log($"TCP server listening on port {port}");

        // Accept one client (blocking)
        _client = _listener.AcceptTcpClient();
        _stream = _client.GetStream();

        UiLogger.Log("Client connected");
    }

    // BLOCKS until client sends data
    public string Listen()
    {
        var buffer = new byte[4096];
        int bytesRead = _stream.Read(buffer, 0, buffer.Length);

        if (bytesRead == 0)
            throw new IOException("Client disconnected");

        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
    }

    // WRITE to client (no coupling to Listen)
    public void Send(string message)
    {
        if (_stream == null)
            throw new InvalidOperationException("No client connected");

        byte[] data = Encoding.UTF8.GetBytes(message+"\r");
        _stream.Write(data, 0, data.Length);
    }

    public void Stop()
    {
        _stream?.Close();
        _client?.Close();
        _listener?.Stop();
    }

    public void Dispose()
    {
        Stop();
    }
}
