using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DIALOGUE.LogicalLine
{
    public class LL_Input : ILogicalLines
    {
        public string keyword => "input";

        IEnumerator ILogicalLines.Execute(DailogLine line)
        {
            string title = line.dialogueData.rawData;
            InputPanel panel = InputPanel.instance;
            panel.Show(title);

            while(panel.isWaitingUserInput)
                yield return null;
        }

        bool ILogicalLines.Matches(DailogLine line)
        {
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
        }
    }
}