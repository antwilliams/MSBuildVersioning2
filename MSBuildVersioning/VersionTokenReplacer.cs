using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MSBuildVersioning
{
    public delegate string TokenFunction();
    public delegate string TokenFunction<T>(T arg);
    public delegate string TokenFunction<T1, T2>(T1 arg1, T2 arg2);

    /// <summary>
    /// Replaces tokens in a string with basic project versioning information.
    /// </summary>
    public class VersionTokenReplacer
    {
        private IList<Token> tokens;

        public VersionTokenReplacer()
        {
            tokens = new List<Token>();

            AddToken("YEAR", () => DateTime.Now.ToString("yyyy"));
            AddToken("MONTH", () => DateTime.Now.ToString("MM"));
            AddToken("DAY", () => DateTime.Now.ToString("dd"));
            AddToken("DATE", () => DateTime.Now.ToString("yyyy-MM-dd"));
            AddToken("DATETIME", () => DateTime.Now.ToString("s"));

            AddToken("UTCYEAR", () => DateTime.UtcNow.ToString("yyyy"));
            AddToken("UTCMONTH", () => DateTime.UtcNow.ToString("MM"));
            AddToken("UTCDAY", () => DateTime.UtcNow.ToString("dd"));
            AddToken("UTCDATE", () => DateTime.UtcNow.ToString("yyyy-MM-dd"));
            AddToken("UTCDATETIME", () => DateTime.UtcNow.ToString("s"));

            AddToken("USER", () => Environment.UserName);
            AddToken("MACHINE", () => Environment.MachineName);
            AddToken("ENVIRONMENT", GetEnvironmentValue);
        }

        protected void AddToken(string tokenName, TokenFunction function)
        {
            tokens.Add(new NoArgsToken
            {
                tokenName = tokenName,
                function = function
            });
        }

        protected void AddToken(string tokenName, TokenFunction<int> function)
        {
            tokens.Add(new IntArgToken
            {
                tokenName = tokenName,
                function = function
            });
        }

        protected void AddToken(string tokenName, TokenFunction<string> function)
        {
            tokens.Add(new StringArgToken
            {
                tokenName = tokenName,
                function = function
            });
        }

        protected void AddToken(string tokenName, TokenFunction<string, string> function)
        {
            tokens.Add(new TwoStringArgToken
            {
                tokenName = tokenName,
                function = function
            });
        }

        public virtual string Replace(string content)
        {
            foreach (Token token in tokens)
            {
                content = token.Replace(content);
            }
            return content;
        }

        private abstract class Token
        {
            public string tokenName;

            public abstract string Replace(string str);
        }

        private class NoArgsToken : Token
        {
            public TokenFunction function;

            public override string Replace(string str)
            {
                string token = "$" + tokenName + "$";
                if (str.Contains(token))
                {
                    str = str.Replace(token, function());
                }
                return str;
            }
        }

        private class IntArgToken : Token
        {
            public TokenFunction<int> function;

            public override string Replace(string str)
            {
                MatchCollection revnumModMatches = Regex.Matches(str,
                    @"\$" + tokenName + @"\((\d+)\)\$");
                foreach (Match match in revnumModMatches)
                {
                    string token = match.Groups[0].Value;
                    int arg = int.Parse(match.Groups[1].Value);
                    str = str.Replace(token, function(arg));
                }
                return str;
            }
        }

        private class StringArgToken : Token
        {
            public TokenFunction<string> function;

            public override string Replace(string str)
            {
                MatchCollection revnumModMatches = Regex.Matches(str,
                    @"\$" + tokenName + @"\(""(.+?)""\)\$");
                foreach (Match match in revnumModMatches)
                {
                    string token = match.Groups[0].Value;
                    string arg = match.Groups[1].Value;
                    str = str.Replace(token, function(arg));
                }
                return str;
            }
        }

        private class TwoStringArgToken : Token
        {
            public TokenFunction<string,string> function;

            public override string Replace(string str)
            {
                MatchCollection revnumModMatches = Regex.Matches(str,
                    @"\$" + tokenName + @"\(""(.+?)"",""(.+?)""\)\$");
                foreach (Match match in revnumModMatches)
                {
                    string token = match.Groups[0].Value;
                    string arg1 = match.Groups[1].Value;
                    string arg2 = match.Groups[2].Value;
                    str = str.Replace(token, function(arg1, arg2));
                }
                return str;
            }
        }

        private string GetEnvironmentValue(string name, string defaultValue)
        {
            var returnValue = Environment.GetEnvironmentVariable(name);
            return returnValue ?? defaultValue;
        }
    }
}
