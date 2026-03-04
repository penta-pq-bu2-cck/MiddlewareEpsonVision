using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace MqttClientApp
{
    /// <summary>
    /// Simple reusable MQTT client wrapper.
    /// Usage:
    ///   var mqtt = new MqttService("broker.hivemq.com", 1883);
    ///   await mqtt.ConnectAsync();
    ///   await mqtt.PublishAsync("my/topic", "hello");
    ///   await mqtt.SubscribeAsync("my/topic");
    ///   mqtt.OnMessageReceived += (topic, payload) => Console.WriteLine($"{topic}: {payload}");
    /// </summary>
    public class MqttService : IAsyncDisposable
    {
        // ── Config ────────────────────────────────────────────────────────────
        private readonly string _broker;
        private readonly int    _port;
        private readonly string _clientId;
        private readonly string? _username;
        private readonly string? _password;
        private readonly bool   _useTls;

        // ── Internals ─────────────────────────────────────────────────────────
        private IMqttClient? _client;
        private readonly MqttFactory _factory = new();

        // ── Events ────────────────────────────────────────────────────────────
        /// <summary>Fired when a subscribed message arrives. (topic, payload)</summary>
        public event Action<string, string>? OnMessageReceived;

        /// <summary>Fired when the connection state changes. (isConnected)</summary>
        public event Action<bool>? OnConnectionChanged;

        // ── State ─────────────────────────────────────────────────────────────
        public bool IsConnected => _client?.IsConnected ?? false;

        // ── Constructor ───────────────────────────────────────────────────────
        public MqttService(
            string broker,
            int    port      = 1883,
            string clientId  = "",
            string? username = null,
            string? password = null,
            bool   useTls    = false)
        {
            _broker   = broker;
            _port     = port;
            _clientId = string.IsNullOrEmpty(clientId)
                            ? "client-" + Guid.NewGuid().ToString()[..8]
                            : clientId;
            _username = username;
            _password = password;
            _useTls   = useTls;
        }

        // ── Connect ───────────────────────────────────────────────────────────
        public async Task ConnectAsync()
        {
            _client = _factory.CreateMqttClient();

            _client.ConnectedAsync    += _ => { OnConnectionChanged?.Invoke(true);  return Task.CompletedTask; };
            _client.DisconnectedAsync += _ => { OnConnectionChanged?.Invoke(false); return Task.CompletedTask; };
            _client.ApplicationMessageReceivedAsync += OnRawMessageReceived;

            var builder = new MqttClientOptionsBuilder()
                .WithClientId(_clientId)
                .WithTcpServer(_broker, _port)
                .WithCleanSession();

            if (!string.IsNullOrEmpty(_username))
                builder.WithCredentials(_username, _password);

            if (_useTls)
                builder.WithTlsOptions(o => o.UseTls());

            await _client.ConnectAsync(builder.Build());
        }

        // ── Disconnect ────────────────────────────────────────────────────────
        public async Task DisconnectAsync()
        {
            if (_client is not null && _client.IsConnected)
                await _client.DisconnectAsync();
        }

        // ── Publish ───────────────────────────────────────────────────────────
        /// <summary>Publish a string payload to a topic.</summary>
        public async Task PublishAsync(
            string topic,
            string payload,
            int    qos    = 0,
            bool   retain = false)
        {
            EnsureConnected();

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qos)
                .WithRetainFlag(retain)
                .Build();

            await _client!.PublishAsync(message);
        }

        /// <summary>Publish a raw byte[] payload to a topic.</summary>
        public async Task PublishAsync(
            string topic,
            byte[] payload,
            int    qos    = 0,
            bool   retain = false)
        {
            EnsureConnected();

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qos)
                .WithRetainFlag(retain)
                .Build();

            await _client!.PublishAsync(message);
        }

        // ── Subscribe / Unsubscribe ───────────────────────────────────────────
        public async Task SubscribeAsync(string topic, int qos = 0)
        {
            EnsureConnected();

            var options = _factory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => f
                    .WithTopic(topic)
                    .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qos))
                .Build();

            await _client!.SubscribeAsync(options);
        }

        public async Task UnsubscribeAsync(string topic)
        {
            EnsureConnected();

            var options = _factory.CreateUnsubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build();

            await _client!.UnsubscribeAsync(options);
        }

        // ── Internal message handler ──────────────────────────────────────────
        private Task OnRawMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            string topic   = e.ApplicationMessage.Topic;
            string payload = e.ApplicationMessage.ConvertPayloadToString() ?? string.Empty;
            OnMessageReceived?.Invoke(topic, payload);
            return Task.CompletedTask;
        }

        private void EnsureConnected()
        {
            if (_client is null || !_client.IsConnected)
                throw new InvalidOperationException("MQTT client is not connected. Call ConnectAsync() first.");
        }

        // ── Dispose ───────────────────────────────────────────────────────────
        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync();
            _client?.Dispose();
        }
    }
}
