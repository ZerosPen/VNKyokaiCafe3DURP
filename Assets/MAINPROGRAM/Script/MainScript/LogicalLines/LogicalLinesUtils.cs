using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DIALOGUE.LogicalLine
{
    public static class LogicalLinesUtils
    {
        public static class Encapsulate
        {
            public struct EncapsulateData
            {
                public List<string> lines;
                public int startingIndex;
                public int endingIndex;
            }

            private const char Encapsulate_Start = '{';
            private const char Encapsulate_End = '}';

            public static EncapsulateData RipEncapsulateData(Conversation conversation, int startingIndex, bool ripHeaderEncapsulaters = false) 
            {
                int encapsulationDepth = 0;
                EncapsulateData data = new EncapsulateData { lines = new List<string>(), startingIndex = startingIndex , endingIndex = 0 };

                for (int i = startingIndex; i < conversation.Count; i++)
                {
                    string line = conversation.GetLines()[i];

                    if(ripHeaderEncapsulaters || (encapsulationDepth > 0 && !isEncapsulateEnd(line)))
                        data.lines.Add(line);

                    if (isEncapsulateStart(line))
                    {
                        encapsulationDepth++;
                        continue;
                    }

                    if (isEncapsulateEnd(line))
                    {
                        encapsulationDepth--;
                        if (encapsulationDepth == 0)
                        {
                            data.endingIndex = i;
                            break;
                        }
                    }
                }
                return data;
            }


            public static bool isEncapsulateStart(string line) => line.Trim().StartsWith(Encapsulate_Start);
            public static bool isEncapsulateEnd(string line) => line.Trim().StartsWith(Encapsulate_End);
        }

        public static class Expressions 
        { 
            public static HashSet<string> Operators = new HashSet<string>() { "-", "-=", "+", "+=", "*", "*=", "/", "/=", "=" };
            public static readonly string Regex_Aritmath = @"([-+*/=]=?)";
            public static readonly string Regex_OperatorLine = @"^\$\w+\s*(=|\+=|-=|\*=|/=|)\s*";
            

            public static object CalculateValue(string[] expressionPart)
            {
                List<string> operandStrings = new List<string>();
                List<string> operatorStrings = new List<string>();
                List<object> operands = new List<object>();     
                
                for(int i = 0; i < expressionPart.Length; i++)
                {
                    string part = expressionPart[i].Trim();

                    if (part == string.Empty)
                        continue;

                    if(Operators.Contains(part))
                        operatorStrings.Add(part);
                    else
                        operandStrings.Add(part);
                }

                foreach(string operandString in operandStrings) 
                {
                    operands.Add(ExtractValue(operandString));
                }

                CalculatedValue_DivAndMulti(operatorStrings, operands);

                CalculatedValue_AddAndSubtrac(operatorStrings, operands);

                return operands[0];
            }

            private static void CalculatedValue_DivAndMulti(List<string> operatorStrings, List<object> operands)
            {
                for(int i = 0; i < operatorStrings.Count; i++)
                {
                    string operatorString = operatorStrings[i];
                    if( operatorString == "*" || operatorString == "/")
                    {
                        double leftOperand = Convert.ToDouble(operands[i]);
                        double rightOperand = Convert.ToDouble(operands[i + 1]);
                    
                        if(operatorString == "*")
                        {
                            operands[i] = leftOperand * rightOperand;
                        }
                        else
                        {
                            if (rightOperand == 0)
                            {
                                Debug.LogError("Cannot Divide By 0");
                                return;
                            }

                            operands[i] = leftOperand / rightOperand;
                        }
                    }
                    operands.RemoveAt(i + 1);
                    operatorStrings.RemoveAt(i);
                    i--;
                }
            }

            private static void CalculatedValue_AddAndSubtrac(List<string> operatorStrings, List<object> operands)
            {
                for (int i = 0; i < operatorStrings.Count; i++)
                {
                    string operatorString = operatorStrings[i];
                    if (operatorString == "+" || operatorString == "-")
                    {
                        double leftOperand = Convert.ToDouble(operands[i]);
                        double rightOperand = Convert.ToDouble(operands[i + 1]);

                        if(operatorString == "+")
                        {
                            operands[i] = leftOperand + rightOperand;
                        }
                        else
                            operands[i] = leftOperand - rightOperand;

                        operands.RemoveAt(i + 1);
                        operatorStrings.RemoveAt(i);
                        i--;
                    }
                }
            }

            private static object ExtractValue (string value) 
            {
                bool negate = false;

                if(value.StartsWith("!"))
                {
                    negate = true;
                    value = value.Substring(1);
                }

                if (value.StartsWith(VariableStore.Variable_Id))
                {
                    string variableName = value.TrimStart(VariableStore.Variable_Id);
                    if (!VariableStore.HasVariable(variableName))
                    {
                        Debug.LogError($"Variable {variableName} does not exists");
                        return null;
                    }

                    VariableStore.TryGetValue(variableName, out object val);

                    if (val is bool boolVal && negate)
                    {
                        return !boolVal;
                    }
                    return val;
                }
                else if (value.StartsWith('\"') && value.EndsWith('\"'))
                {
                    value = TagManager.InjectVariabel(value);
                    return value.Trim('"');
                }
                    
                else
                {
                    if (int.TryParse(value, out int intValue))
                    {
                        return intValue;
                    }

                    else if (float.TryParse(value, out float floatValue))
                    {
                        return floatValue;
                    }

                    else if (bool.TryParse(value, out bool boolValue))
                    {
                        return negate ? !boolValue : boolValue;
                    }
                    else
                        return value; 
                }
            }
        }
    }
}