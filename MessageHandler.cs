using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Chat_Client.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chat_Client
{
    internal class MessageHandler
    {
        private static readonly string[] _welcomeMessages = { "joined your party.", "is here.", "appeared.", "has spawned in the server.", "just showed up.", "joined. Welcome!", " is summoned." };

        private static readonly string[] _goodbyeMessages = { "just left.", "you will be missed.", "goodbye :(", "has quit.", "is no longer here.", "dissapeared." };

        public static void Handle(string message)
        {
            try
            {
                var jsonMessage = JsonSerializer.Deserialize<MemberMessageModel>(message);
                var dateT = new DateTime(jsonMessage.Timestamp).ToString("h:mm:ss");

                Random random = new Random();
                switch (jsonMessage.Type)
                {
                    case "join":
                        {
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.WriteLine($"[{dateT}] [JOIN] *{jsonMessage.Nickname}* {_welcomeMessages[random.Next(0, _welcomeMessages.Length)]}");
                            Console.ResetColor();
                            break;
                        }
                    case "leave":
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.WriteLine($"[{dateT}] [LEAVE] *{jsonMessage.Nickname}* {_goodbyeMessages[random.Next(0, _goodbyeMessages.Length)]}");
                            Console.ResetColor();
                            break;
                        }
                    case "publish":
                        {
                            Console.WriteLine($"[{dateT}] {jsonMessage.Nickname}: {jsonMessage.Message}");
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("JSON not recognized");
                Console.WriteLine(e.ToString());
            }
        }
    }
}