using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrunchSpaceEngineersChatResponses.Models
{
    public class ConfiguredMessage
    {
        public List<String> WordsToLookFor = new List<string>();
        public decimal PercentToHave = 0.7m;
        public string Response = "Response Message";
        public string Prefix = "Dave";
        public int r = 255;
        public int g = 255;
        public int b = 255;
    }
}
