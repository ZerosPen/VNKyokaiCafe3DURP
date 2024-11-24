using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace DIALOGUE.LogicalLine
{
    public class LogicalLineManager
    {
        private DialogController dialogueController => DialogController.Instance;

        private List<ILogicalLines> logicalLines = new List<ILogicalLines>();

        public LogicalLineManager() => LoadLogicalLines();
        private void LoadLogicalLines()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] lineTpyes = assembly.GetTypes()
                                        .Where(t => typeof(ILogicalLines).IsAssignableFrom(t) && !t.IsInterface)
                                        .ToArray();

            foreach(Type lineTpye in lineTpyes)
            {
                ILogicalLines line = (ILogicalLines)Activator.CreateInstance(lineTpye);
                logicalLines.Add(line);
            }
        }

        public bool TryGetLogic(DailogLine line, out Coroutine logic)
        {
            foreach(var logicLine in logicalLines)
            {
                if (logicLine.Matches(line))
                {
                    logic = dialogueController.StartCoroutine(logicLine.Execute(line));
                    return true;
                }
            }

            logic = null;
            return false;
        }
    }
}

