using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DylanTwitch.Util
{
    public static class TranslateableString
    {
        public static string Translate(string message, Dictionary<string, string> variables)
        {
            return variables.Aggregate(message, (current, variable) => current.Replace(variable.Key, variable.Value));
        }
    }
}
