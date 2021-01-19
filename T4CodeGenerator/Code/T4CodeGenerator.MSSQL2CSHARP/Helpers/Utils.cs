using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace T4CodeGenerator.MSSQL2CSHARP.Helpers
{
    public static class Utils
    {
        public static string RemoveId(string value)
        {
            return Regex.Match(value, "(.+)Id").Groups[1].ToString();
        }

        public static string FirstLetterToLower(string value)
        {
            return value.ToLower()[0] + value.Substring(1, value.Length - 1);
        }

        public static string FirstLetterToUpper(string value)
        {
            return value.ToUpper()[0] + value.Substring(1, value.Length - 1);
        }

        public static string CustomBHC(string value)
        {
            string result = value.Replace("trc_", "");
            result = result.Replace("_", " ");
            return result.ToUpper()[0] + result.Substring(1, result.Length - 1);

            //return value.ToUpper()[0] + value.Substring(1, value .Length - 1);
        }
    }
}
