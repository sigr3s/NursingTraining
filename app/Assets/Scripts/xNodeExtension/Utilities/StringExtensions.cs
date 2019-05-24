using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
 
public static class StringExtensions
{

    public static string NicifyString(this string name){
        string[] nameParts = name.Split('/');

        for(int i = 0; i < nameParts.Length; i++){
            nameParts[i] = nameParts[i].SplitCamelCase();
        }

        return string.Join(" || ", nameParts);;
    }

    public static string SplitCamelCase(this string input)
    {
        if (input == null) return null;
        if (string.IsNullOrWhiteSpace(input)) return "";

        var separated = input;

        separated = SplitCamelCaseRegex.Replace(separated, @" $1").Trim();

        //Set ALL CAPS words
        if (_SplitCamelCase_AllCapsWords.Any())
            foreach (var word in _SplitCamelCase_AllCapsWords)
                separated = SplitCamelCase_AllCapsWords_Regexes[word].Replace(separated, word.ToUpper());

        //Capitalize first letter
        var firstChar = separated.First(); //NullOrWhiteSpace handled earlier
        if (char.IsLower(firstChar))
            separated = char.ToUpper(firstChar) + separated.Substring(1);

        return separated;
    }

    private static readonly Regex SplitCamelCaseRegex = new Regex(@"
        (
            (?<=[a-z])[A-Z0-9] (?# lower-to-other boundaries )
            |
            (?<=[0-9])[a-zA-Z] (?# number-to-other boundaries )
            |
            (?<=[A-Z])[0-9] (?# cap-to-number boundaries; handles a specific issue with the next condition )
            |
            (?<=[A-Z])[A-Z](?=[a-z]) (?# handles longer strings of caps like ID or CMS by splitting off the last capital )
        )"
        , RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
    );

    private static readonly string[] _SplitCamelCase_AllCapsWords =
            ("")
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(a => a.ToLowerInvariant().Trim())
            .ToArray()
            ;

    private static Dictionary<string, Regex> _SplitCamelCase_AllCapsWords_Regexes;
    private static Dictionary<string, Regex> SplitCamelCase_AllCapsWords_Regexes
    {
        get
        {
            if (_SplitCamelCase_AllCapsWords_Regexes == null)
            {
                _SplitCamelCase_AllCapsWords_Regexes = new Dictionary<string,Regex>();
                foreach(var word in _SplitCamelCase_AllCapsWords)
                    _SplitCamelCase_AllCapsWords_Regexes.Add(word, new Regex(@"\b" + word + @"\b", RegexOptions.Compiled | RegexOptions.IgnoreCase));
            }

            return _SplitCamelCase_AllCapsWords_Regexes;
        }
    }

}