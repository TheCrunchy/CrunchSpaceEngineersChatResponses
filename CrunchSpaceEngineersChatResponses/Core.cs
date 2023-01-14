using Sandbox.Game.Entities.Cube;
using Sandbox.Game.GameSystems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CrunchSpaceEngineersChatResponses.Models;
using NLog;
using Sandbox.Engine.Multiplayer;
using Sandbox.Game.Multiplayer;
using Torch;
using Torch.API;
using Torch.API.Managers;
using Torch.API.Plugins;
using Torch.API.Session;
using Torch.Managers;
using Torch.Managers.ChatManager;
using Torch.Managers.PatchManager;
using Torch.Session;
using VRageMath;

namespace CrunchSpaceEngineersChatResponses
{
    public class Core : TorchPluginBase
    {
        public override void Init(ITorchBase torch)
        {
            base.Init(torch);

            var sessionManager = Torch.Managers.GetManager<TorchSessionManager>();

            if (sessionManager != null)
            {
                sessionManager.SessionStateChanged += SessionChanged;
            }

            SetupConfig();

        }

        public static string Path;
        public static void LoadConfig()
        {
            FileUtils utils = new FileUtils();
            var path = "\\CrunchChatResponses\\Config.xml";
            config = utils.ReadFromXmlFile<Config>(Path + path);
        }
        private void SetupConfig()
        {
            FileUtils utils = new FileUtils();
            var path = "\\CrunchChatResponses\\Config.xml";
            Path = StoragePath;
            Directory.CreateDirectory(Path + "\\CrunchChatResponses\\");
            if (File.Exists(StoragePath + path))
            {
                config = utils.ReadFromXmlFile<Config>(StoragePath + path);
                utils.WriteToXmlFile<Config>(StoragePath + path, config, false);
            }
            else
            {
                config = new Config();
                config.Messages.Add(new ConfiguredMessage()
                {
                    WordsToLookFor = new List<string>()
                    {
                        "hi", "dave"
                    },
                    Response = "Hello {player}"
                });
                utils.WriteToXmlFile<Config>(StoragePath + path, config, false);
            }

        }

        private void SessionChanged(ITorchSession session, TorchSessionState newState)
        {
            var _chatmanager = Torch.CurrentSession.Managers.GetManager<ChatManagerServer>();

            if (_chatmanager != null)
            {
                {
                    _chatmanager.MessageProcessing += ChatHandler;
                }
            }
        }

        public static Config config;

        public static DateTime NextResponse = DateTime.Now;
        public static void ChatHandler(TorchChatMessage msg, ref bool consumed)
        {
            if (DateTime.Now >= NextResponse)
            {
                NextResponse = DateTime.Now.AddSeconds(config.CooldownInSeconds);
            }
            else
            {
                return;
            }
            if (msg.AuthorSteamId == null)
            {
                return;
            }

            if (msg.Channel == Sandbox.Game.Gui.ChatChannel.Private)
            {
                return;
            }

            if (msg.Message.StartsWith("!"))
            {
                return;
            }

            var response = GetResponse(msg.Message);
            if (response == null)
            {
                return;
            }

            var messageResponse = response.Response.Replace("{player}", msg.Author);
            SendMessage(response.Prefix, messageResponse, new Color(response.r, response.g, response.b), 0);
        }

        public static void SendMessage(string author, string message, Color color, long steamID)
        {
            ScriptedChatMsg scriptedChatMsg1 = new ScriptedChatMsg();
            scriptedChatMsg1.Author = author;
            scriptedChatMsg1.Text = message;
            scriptedChatMsg1.Font = "White";
            scriptedChatMsg1.Color = color;
            scriptedChatMsg1.Target = Sync.Players.TryGetIdentityId((ulong)steamID);
            ScriptedChatMsg scriptedChatMsg2 = scriptedChatMsg1;
            MyMultiplayerBase.SendScriptedChatMessage(ref scriptedChatMsg2);
        }

        public static ConfiguredMessage GetResponse(string message)
        {
            foreach (var configMessage in config.Messages)
            {
                var has = new List<string>();
                foreach (var word in message.Split(' ').ToList())
                {
                    if (configMessage.WordsToLookFor.Any(x => x.ToLower() == word.ToLower()))
                    {
                        if (!has.Contains(word))
                        {
                            has.Add(word);
                        }
                    }

                    var max = configMessage.WordsToLookFor.Count;
                    var percent = has.Count + max;
                    if (percent >= configMessage.PercentToHave)
                    {
                        return configMessage;
                    }
                }
            }

            return null;
        }
    }
}

