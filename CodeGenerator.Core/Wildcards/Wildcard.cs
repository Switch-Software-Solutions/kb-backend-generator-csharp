using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Wildcards
{
	public class Wildcard
	{
		public string Type { get; set; }
		public string Name { get; set; }
		public string Format { get; set; }
		public string Plural { get; set; }
		public string RepeatBy { get; set; }
		public List<string> Exclude { get; set; }
		public string Transform { get; set; }
		public string TransformGroup { get; set; }

		public static List<string> GetWildcards(string template)
		{
			List<string> result = new List<string>();

			string pattern = Regex.Escape("[¡WILDCARD!]{") + ".*?" + Regex.Escape("}[¡WILDCARD!]");

			MatchCollection matches = Regex.Matches(template, pattern, RegexOptions.IgnoreCase);

			foreach (Match match in matches)
			{
				result.Add(match.Value.Replace("[¡WILDCARD!]", ""));
			}

			return result;
		}

		public static string ReplaceWildcard(string template, string wildcard, string value)
		{
			string result = template;

			wildcard = "[¡WILDCARD!]" + wildcard + "[¡WILDCARD!]";

			result = template.Replace(wildcard, value);

			return result;
		}
	}
}

