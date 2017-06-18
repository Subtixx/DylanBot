using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands
{
    public class CustomCommandsSettings
    {
        public bool Active = true;

        public Dictionary<string, string> RegisteredCommands = new Dictionary<string, string>()
        {
            {"about", "DylanBot was made by Subtixx! $username, check my source code at: https://github.com/Subtixx/DylanBot" }
        };
    }
}