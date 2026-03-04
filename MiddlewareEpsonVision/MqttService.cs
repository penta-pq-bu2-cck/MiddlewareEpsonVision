using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Client.Subscribing;
using MQTTnet.Client.Unsubscribing;
using MQTTnet.Protocol;
using System;
using System.Threading.Tasks;

namespace MqttClientApp
{
    /// <summary>
    /// Reusable MQTT client wrapper — compatible with MQTTnet 3.1.2
    ///
    /// Usage:
    ///   var mqtt = new MqttService("broker.hivemq.com", 1883);
    ///   await mqtt.ConnectAsync();
    ///   await mqtt.PublishAsync("my/topic", "hello");
    ///   await mqtt.SubscribeAsync("my/topic");
    ///   mqtt.OnMessageReceived += (topic, payload) => Console.WriteLine($"{topic}: {payload}");
    /// </summary>
    public class MqttService : IDisposable
    {
        // ── Config ────────────────────────────────────────────────────────────
        private readonly string _broker;
        private readonly int _port;
        private readonly string _clientId;
        private readonly string _username;
        private readonly string _password;
        private readonly bool _useTls;

        // ── Internals ─────────────────────────────────────────────────────────
        private IMqttClient _client;

        // ── Events ────────────────────────────────────────────────────────────
        /// <summary>Fired when a subscribed message arrives. (topic, payload)</summary>
        public event Action<string, string> OnMessageReceived;

        /// <summary>Fired when connection state changes. (isConnected)</summary>
        public event Action<bool> OnConnectionChanged;

        // ── State ─────────────────────────────────────────────────────────────
        public bool IsConnected => _client?.IsConnected ?? false;

        // ── Constructor ───────────────────────────────────────────────────────
        public MqttService(
            string broker,
            int port = 1883,
            string clientId = "",
            string username = null,
            string password = null,
            bool useTls = false)
        {
            _broker = broker;
            _port = port;
            _clientId = string.IsNullOrEmpty(clientId)
                            ? "client-" + Guid.NewGuid().ToString().Substring(0, 8)
                            : clientId;
            _username = username;
            _password = password;
            _useTls = useTls;
        }

        // ── Connect ───────────────────────────────────────────────────────────
        public async Task ConnectAsync()
        {
            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            // v3 uses handler methods instead of async events
            _client.UseConnectedHandler(e =>
            {
                OnConnectionChanged?.Invoke(true);
            });

            _client.UseDisconnectedHandler(e =>
            {
                OnConnectionChanged?.Invoke(false);
            });

            _client.UseApplicationMessageReceivedHandler(e =>
            {
                string topic = e.ApplicationMessage.Topic;
                string payload = e.ApplicationMessage.ConvertPayloadToString() ?? string.Empty;
                OnMessageReceived?.Invoke(topic, payload);
            });

            // Build options
            var builder = new MqttClientOptionsBuilder()
                .WithClientId(_clientId)
                .WithTcpServer(_broker, _port)
                .WithCleanSession();

            if (!string.IsNullOrEmpty(_username))
                builder = builder.WithCredentials(_username, _password);

            if (_useTls)
                builder = builder.WithTls();

            await _client.ConnectAsync(builder.Build());
        }

        // ── Disconnect ────────────────────────────────────────────────────────
        public async Task DisconnectAsync()
        {
            if (_client != null && _client.IsConnected)
                await _client.DisconnectAsync();
        }

        // ── Publish (string) ──────────────────────────────────────────────────
        public async Task PublishAsync(
            string topic,
            string payload,
            int qos = 0,
            bool retain = false)
        {
            EnsureConnected();

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qos)
                .WithRetainFlag(retain)
                .Build();

            await _client.PublishAsync(message);
        }

        // ── Publish (bytes) ───────────────────────────────────────────────────
        public async Task PublishAsync(
            string topic,
            byte[] payload,
            int qos = 0,
            bool retain = false)
        {
            EnsureConnected();

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qos)
                .WithRetainFlag(retain)
                .Build();

            await _client.PublishAsync(message);
        }

        // ── Subscribe ─────────────────────────────────────────────────────────
        public async Task SubscribeAsync(string topic, int qos = 0)
        {
            EnsureConnected();

            await _client.SubscribeAsync(
                new TopicFilterBuilder()
                    .WithTopic(topic)
                    .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qos)
                    .Build());
        }

        // ── Unsubscribe ───────────────────────────────────────────────────────
        public async Task UnsubscribeAsync(string topic)
        {
            EnsureConnected();
            await _client.UnsubscribeAsync(topic);
        }

        // ── Publish then wait for reply ───────────────────────────────────────
        /// <summary>
        /// Publish to a topic, then BLOCK and wait until a message arrives
        /// on replyTopic. Returns the payload string, or null if timed out.
        /// </summary>
        public async Task<string> PublishAndWaitReplyAsync(
            string publishTopic,
            string payload,
            string replyTopic,
            int timeoutMs = 5000,
            int qos = 0)
        {
            EnsureConnected();

            var tcs = new TaskCompletionSource<string>();

            // Temporary handler — fires only for the reply topic
            Action<string, string> handler = null;
            handler = (topic, msg) =>
            {
                if (topic == replyTopic)
                {
                    OnMessageReceived -= handler;   // unregister immediately
                    tcs.TrySetResult(msg);
                }
            };

            OnMessageReceived += handler;

            // MUST subscribe to reply topic first so broker sends us the message
            await SubscribeAsync(replyTopic, qos);

            // Publish the request
            await PublishAsync(publishTopic, payload, qos);

            // Wait for reply or timeout
            var timeoutTask = Task.Delay(timeoutMs);
            var completed = await Task.WhenAny(tcs.Task, timeoutTask);

            // Always unsubscribe reply topic after done
            try { await UnsubscribeAsync(replyTopic); } catch { }

            if (completed == timeoutTask)
            {
                OnMessageReceived -= handler;       // clean up on timeout
                return null;                        // caller checks for null
            }

            return tcs.Task.Result;
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private void EnsureConnected()
        {
            if (_client == null || !_client.IsConnected)
                throw new InvalidOperationException(
                    "MQTT client is not connected. Call ConnectAsync() first.");
        }

        // ── Dispose ───────────────────────────────────────────────────────────
        public void Dispose()
        {
            if (_client != null && _client.IsConnected)
                _client.DisconnectAsync().GetAwaiter().GetResult();
            _client?.Dispose();
        }
    }
}