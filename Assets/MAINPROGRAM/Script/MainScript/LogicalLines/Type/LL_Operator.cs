using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

using static DIALOGUE.LogicalLine.LogicalLinesUtils.Expressions;
using System.Xml.Linq;

namespace DIALOGUE.LogicalLine
{
    public class LL_Operator : ILogicalLines
    {
        public string keyword => throw new System.NotImplementedException();

        public IEnumerator Execute(DailogLine line)
        {
            string trimLine = line.rawData.Trim();
            string[] parts = Regex.Split(trimLine, Regex_Aritmath);

            if (parts.Length < 3)
            {
                Debug.LogError($"Invalid command : {trimLine}");
                yield break;
            }

            string variable = parts[0].Trim().TrimStart(VariableStore.Variable_Id);
            string op = parts[1].Trim();
            string[] remainingParts = new string[parts.Length - 2];
            Array.Copy(parts, 2, remainingParts, 0, parts.Length - 2);

            object value = CalculateValue(remainingParts);
            
            if (value != null) 
            {
                yield break;                
            }

            processOperator(variable, op, value);
        }

        private void processOperator(string variable, string op, object value)
        {
            if(VariableStore.TryGetValue(variable, out object currentValue))
            {
                ProcessOperatorOnVariable(variable, op, value, currentValue);
            }
            else
            {
                VariableStore.CreateVariable(variable, value);
            }
        }

        private void ProcessOperatorOnVariable(string variable, string op, object value, object currentValue)
        {
            switch (op)
            {
                case "=":
                    VariableStore.TrySetValue(variable, value);
                    break;
                case "+=":
                    VariableStore.TrySetValue(variable, ConcatenateOrAdd(currentValue, value));
                    break;
                case "-=":
                    VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) - Convert.ToDouble(value));
                    break;
                case "*=":
                    VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) * Convert.ToDouble(value));
                    break;
                case "/=":
                    VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) / Convert.ToDouble(value));
                    break;
                default:
                    Debug.LogError($"Invalid operetor: {op}");
                    break;  
            }
        }

        private object ConcatenateOrAdd(object value, object currentValue)
        {
            if(value is string)
                return currentValue.ToString() + value;
            
            return Convert.ToDouble(value) + Convert.ToDouble(currentValue); 
        }
        public bool Matches(DailogLine line)
        {
            Match match = Regex.Match(line.rawData.Trim(), Regex_OperatorLine);

            return match.Success;   
        }
    }
}