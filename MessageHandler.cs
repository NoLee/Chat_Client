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

        public static bool Handle(string message)
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
                            return true;
                        }
                    case "leave":
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.WriteLine($"[{dateT}] [LEAVE] *{jsonMessage.Nickname}* {_goodbyeMessages[random.Next(0, _goodbyeMessages.Length)]}");
                            Console.ResetColor();
                            return true;
                        }
                    case "publish":
                        {
                            Console.WriteLine($"[{dateT}] {jsonMessage.Nickname}: {jsonMessage.Message}");
                            return true;
                        }
                }
                return false;
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("!! Received a message with unrecognized format");
                Console.ResetColor();
                return true;
            }
        }
    }
}