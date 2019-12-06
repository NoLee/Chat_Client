using System;
using System.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Chat_Client.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Chat_Client
{
    internal class Service : IHostedService

    {
        private readonly string exchangeName = "chat_fnt"; // TODO move this to appsetting.json

        private string _name;

        private int _currentMessageCursorY;

        private readonly IConfiguration _configuration;

        public Service(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            //string configvalue1 = _configuration.GetSection("RabbitMQ").Value;
            //Console.WriteLine(configvalue1);

            Console.Write("Username:");
            _name = Console.ReadLine();

            var factory = new ConnectionFactory() { HostName = "localhost" }; //TODO pick this from appsettings
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var queueName = DeclareQueue(channel);
                    AddConsumer(channel, queueName);
                    PublishMessage(channel, "join");

                    string keyboardInput = Console.ReadLine();
                    ClearTypedLine(keyboardInput);
                    while (keyboardInput != "/quit" && keyboardInput != null)
                    {
                        if (keyboardInput == "/clear")
                        {
                            Console.Clear();
                            _currentMessageCursorY = Console.WindowTop - Console.WindowHeight;
                            if (_currentMessageCursorY < 0)
                            {
                                _currentMessageCursorY = 0;
                            }
                            ClearTypedLine(keyboardInput);
                        }
                        else
                        {
                            PublishMessage(channel, "publish", keyboardInput);
                        }

                        keyboardInput = Console.ReadLine();
                        ClearTypedLine(keyboardInput);
                    }

                    PublishMessage(channel, "leave");
                    Thread.Sleep(1000); // wait a bit the receive message from RabbitMQ
                }
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
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

            _currentMessageCursorY = Console.CursorTop;
            consumer.Received += (model, ea) =>
            {
                int cursorLastX = Console.CursorLeft;
                int cursosrLastY = Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;

                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                Console.SetCursorPosition(0, _currentMessageCursorY);
                MessageHandler.Handle(message);
                _currentMessageCursorY++;
                Console.SetCursorPosition(cursorLastX, cursosrLastY);
            };
            channel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: consumer);
        }

        private void PublishMessage(IModel channel, string type, string input = "")
        {
            var message = new MemberMessageModel
            {
                Type = type,
                Timestamp = DateTime.Now.Ticks,
                Nickname = _name,
                Message = input
            };
            var json = JsonSerializer.Serialize<MemberMessageModel>(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: exchangeName,
                    routingKey: "",
                    basicProperties: null,
                    body: body);
        }

        private void ClearTypedLine(string input)
        {
            if (input == null)
            {
                input = " ";
            }
            int y = Console.WindowTop + Console.WindowHeight - 2;
            Console.SetCursorPosition(0, y);
            Console.Write(new String(' ', input.Length));
            Console.SetCursorPosition(0, y);
        }
    }
}