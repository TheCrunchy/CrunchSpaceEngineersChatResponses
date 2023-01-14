using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrunchSpaceEngineersChatResponses.Models;

namespace CrunchSpaceEngineersChatResponses
{
    public class Config
    {
        public List<ConfiguredMessage> Messages = new List<ConfiguredMessage>();
        public int CooldownInSeconds = 30;
    }
}
