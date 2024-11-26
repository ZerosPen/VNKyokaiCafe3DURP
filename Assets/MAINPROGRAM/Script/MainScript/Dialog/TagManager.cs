using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using UnityEditor;
using System.Linq;
using System.Text;

public class TagManager
{
    private static readonly Dictionary<string, Func<string>> tags = new Dictionary<string, Func<string>>()
    {
        { "<mainChar>",         () => "Zero"},
        { "<time>",             () => DateTime.Now.ToString("hh:mm tt") },
        { "<playerLevel>",      () => "15" },
        { "<input>",            () => InputPanel.instance.lastInput },
        { "<tempVal>",          () => "42" },
    };

    private static readonly Regex tagRegex = new Regex("<\\w+>");

    public static string Inject(string text, bool injectTags = true, bool injectVariable = true  )
    {
        if (injectTags)
            text = InjectTags(text);

        if (injectVariable)
            text = InjectVariabel(text);

        return text;
    }

    public static string InjectTags(string value)
    {
        if (tagRegex.IsMatch(value))
        {
            foreach (Match match in tagRegex.Matches(value))
            {
                if (tags.TryGetValue(match.Value, out var tagValueRequest))
                {
                    value = value.Replace(match.Value, tagValueRequest());
                }
            }
        }
        return value;
    }

    public static string InjectVariabel(string value)
    {
        var matches = Regex.Matches(value, VariableStore.Regex_Variable_Id);
        var matchesList = matches.Cast<Match>().ToList();

        for (int i = matchesList.Count - 1; i >= 0; i--)
        {
            var match = matchesList[i];
            string variableName = match.Value.TrimStart(VariableStore.Variable_Id);

            // Log the variable name for debugging
            UnityEngine.Debug.Log($"Extracted variable name: {variableName}");

            if (!VariableStore.TryGetValue(variableName, out object variableValue))
            {
                // This line will log if the variable is not found
                UnityEngine.Debug.LogError($"Variable '{variableName}' not found in VariableStore.");
                continue; // Skip to the next variable
            }

            int lengthToRemove = match.Index + match.Length > value.Length ? value.Length - match.Index : match.Length;

            value = value.Remove(match.Index, lengthToRemove);
            value = value.Insert(match.Index, variableName.ToString());
        }

        return value;
    }
}
