using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Crown.Utilities
{
    public static class Parsers
    {
        public static string Parse(string[] values, params Func<string[], string[]>[] actions)
        {
            var value = values;
            foreach (var action in actions)
            {
                value = action(value);
            }

            return string.Join(" ",value);
        }
        public static string[] SplitByCapitalLetter(string value)
        {
            List<string> words = new List<string>();
            List<char> singleWord = new List<char>();

            foreach (char c in value)
            {
                if (char.IsUpper(c) && singleWord.Count > 0)
                {
                    words.Add(new string(singleWord.ToArray()));
                    singleWord.Clear();
                    singleWord.Add(c);
                }
                else
                {
                    singleWord.Add(c);
                }
            }

            words.Add(new string(singleWord.ToArray()));

            return words.ToArray();
        }

        public static string[] ToLower(string[] values)
        {
            List<string> newValues = new List<string>(values.Length);
            foreach (var value in values)
            {
                newValues.Add(value.ToLower());
            }

            return newValues.ToArray();
        }

        public static string Capitalize(string value)
        {
            var first = value[0].ToString().ToUpper();
            var others = string.Join("", value.Skip(1));
            return string.Join("", first, others);
        }

        public static string[] Capitalize(string[] values)
        {
            var all = values.ToList();
            all[0] = Capitalize(all[0]);
            return all.ToArray();
        }

        public static string Format(string format, params object[] parameters)
        {
            var formatted = string.Format(format, parameters);
            if (string.IsNullOrWhiteSpace(formatted))
            {
                formatted= string.Join(" ", parameters);
            }

            return Capitalize(formatted);
        }

        public static string NullIfEmpty(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return value;
        }
    }
}
