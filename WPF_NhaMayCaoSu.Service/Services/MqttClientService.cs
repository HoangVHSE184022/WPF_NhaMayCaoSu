﻿using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet;
using System.Text;
using WPF_NhaMayCaoSu.Service.Interfaces;
using System.Diagnostics;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class MqttClientService : IMqttClientService
    {
        private readonly IMqttClient _client;
        private readonly MqttClientOptions _options;

        public event EventHandler<string> MessageReceived;
        public bool IsConnected => _client?.IsConnected ?? false;

        public MqttClientService()
        {
            MqttFactory factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            MqttClientOptionsBuilder optionsBuilder = new MqttClientOptionsBuilder();
            optionsBuilder.WithClientId("e_scale")
                           .WithTcpServer("127.0.0.1", 1883)
                           .WithCredentials("admin", "admin")
                           .WithCleanSession();

            _options = optionsBuilder.Build();

            // Register event handlers
            _client.ConnectedAsync += OnConnectedAsync;
            _client.DisconnectedAsync += OnDisconnectedAsync;
            _client.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;
        }

        private Task OnConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Debug.WriteLine("Connected to MQTT broker.");
            return Task.CompletedTask;
        }

        private Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            Debug.WriteLine("Disconnected from MQTT broker.");
            return Task.CompletedTask;
        }

        private Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            string message = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
            Console.WriteLine($"Message received on topic {arg.ApplicationMessage.Topic}: {message}");

            // Raise the MessageReceived event
            MessageReceived?.Invoke(this, message);

            return Task.CompletedTask;
        }

        public async Task ConnectAsync()
        {
            if (_client.IsConnected != true)
            {
                await _client.ConnectAsync(_options);
            }
        }

        public async Task SubscribeAsync(string topic)
        {
            MqttTopicFilterBuilder topicFilterBuilder = new MqttTopicFilterBuilder();
            topicFilterBuilder.WithTopic(topic);

            await _client.SubscribeAsync(topicFilterBuilder.Build());
        }

        public async Task PublishAsync(string topic, string payload)
        {
            MqttApplicationMessageBuilder messageBuilder = new MqttApplicationMessageBuilder();
            messageBuilder.WithTopic(topic)
                          .WithPayload(payload)
                          .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                          .WithRetainFlag();

            MqttApplicationMessage message = messageBuilder.Build();

            await _client.PublishAsync(message);
        }

        public async Task CloseConnectionAsync()
        {
            if (_client.IsConnected)
            {
                await _client.DisconnectAsync();
                Console.WriteLine("Disconnected from MQTT broker.");
            }
        }
    }
}