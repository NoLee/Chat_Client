using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Chat_Client.Models;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Chat_Client
{
    internal class Service : IHostedService, IDisposable

    {
        private readonly string exchangeName = "chat_fnt"; // TODO move this to appsetting.json

        public Task StartAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("HELLO");
            Testing();

            var factory = new ConnectionFactory() { HostName = "localhost" }; //TODO pick this from appsettings
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var queueName = DeclareQueue(channel);
                    AddConsumer(channel, queueName);
                    string keyboardInput = Console.ReadLine();

                    while (keyboardInput != "/quit")
                    {
                        PublishMessage();
                        keyboardInput = Console.ReadLine();
                    }
                    //TODO must stop execution when quitting
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("BYE");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        private void Testing()
        {
            var c = new MemberMessageModel { Message = "test", Nickname = "NoLee" };
            Console.WriteLine(JsonSerializer.Serialize<MemberMessageModel>(c));
            Console.WriteLine(new DateTime(1575466048).ToString("yy-MM-dd"));
        }

        private string DeclareQueue(IModel channel)
        {
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout); //TODO optional

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                exchange: exchangeName,
                routingKey: "");

            return queueName;
        }

        private void AddConsumer(IModel channel, string queueName)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                MessageHandler.Handle(message);
            };
            channel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: consumer);
        }

        private void PublishMessage() //IModel channel, string queueName
        {
        }
    }
}