using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dvinun.DesignPatterns.Behavioral
{
    partial class Interpreter
    {
        class Utils
        {
            static public void PrintRegexMatch(Match match, MethodBase methodBase)
            {
                if (match.Success)
                {
                    Console.WriteLine("Regex match in function {0}.{1} - Value: {2}", methodBase.ReflectedType.Name, methodBase.Name, match.Value);
                }
                else
                {
                    Console.WriteLine("Regex didn't match function {0}.{1}", methodBase.ReflectedType.Name, methodBase.Name);
                }
            }

            internal static string Trim(string mainValue, string value)
            {
                if (string.IsNullOrEmpty(mainValue)) return value;
                if (string.IsNullOrEmpty(value)) return mainValue;

                int index = mainValue.IndexOf(value);
                if (index < 0) return mainValue;

                return mainValue.Substring(index + value.Length);
            }
        }
    }
}
