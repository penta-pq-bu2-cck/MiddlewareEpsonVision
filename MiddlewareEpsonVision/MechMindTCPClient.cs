using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class MechMindTcpClient
{
    private TcpClient _client;
    private NetworkStream _stream;

    // Connect to Mech-Mind vision
    public async Task<bool> ConnectAsync(string ip, int port, int timeoutMs = 3000)
    {
        _client = new TcpClient();
        _client.ReceiveTimeout = timeoutMs;
        _client.SendTimeout = timeoutMs;

        await _client.ConnectAsync(ip, port);
        _stream = _client.GetStream();

        return _client.Connected;
    }

    // Send command to vision system
    public async Task SendAsync(string command)
    {
        if (_stream == null)
            throw new Exception("Not connected");

        byte[] data = Encoding.ASCII.GetBytes(command);
        await _stream.WriteAsync(data, 0, data.Length);
        await _stream.FlushAsync();
    }

    // Receive all data in one read (assume < 64KB)
    public async Task<string> ReceiveAsync()
    {
        if (_stream == null)
            throw new Exception("Not connected");

        byte[] buffer = new byte[64 * 1024]; // 64 KB buffer
        int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);

        if (bytesRead == 0)
            throw new Exception("Connection closed or no data received");

        string rawData = Encoding.ASCII.GetString(buffer, 0, bytesRead);

        return rawData;
    }

    // Close connection
    public void Close()
    {
        _stream?.Close();
        _client?.Close();
    }
}
