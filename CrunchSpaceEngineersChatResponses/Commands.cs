using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torch.Commands;
using Torch.Commands.Permissions;
using VRage.Game.ModAPI;

namespace CrunchSpaceEngineersChatResponses
{
    public class Commands : CommandModule
    {
        [Command("chatbot reload", "Reload the config")]
        [Permission(MyPromoteLevel.Admin)]
        public void ReloadConfig()
        {
            Core.LoadConfig();

            Context.Respond("Reloaded config");
        }
    }
}
