using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DIALOGUE.LogicalLine
{
    public class LL_Choice : ILogicalLines
    {
        public string keyWord => "choice";
        private const char Encapsulate_Start = '{';
        private const char Encapsulate_End = '}';
        private const char Choice_Identifier = '-';

        public IEnumerator Excute(DailogLine line)
        {
            RawChoiceData data = RipChoiceData();

            List<Choice> choices = GetChoicesFormData(data);

            string title = line.dialogueData.rawData;
            ChoicePanel panel = ChoicePanel.instance;

            string[] choiceTitle = choices.Select(c => c.title).ToArray();

            panel.Show(title, choiceTitle);

            while(panel.isWaitingPlayerChoice)
                yield return null;

            Choice selectedChoice = choices[panel.lastDecision.answerIndex];

            Conversation newConversation = new Conversation(selectedChoice.resultLines);
            DialogController.Instance.conversationManager.conversation.SetProgress(data.endingIndex);
            DialogController.Instance.conversationManager.EnqueuePriority(newConversation);
        }

        private RawChoiceData RipChoiceData()
        {
            Conversation currentConversation = DialogController.Instance.conversationManager.conversation;
            int currentProgress = DialogController.Instance.conversationManager.conversationProgress;
            int EncapsulateDepth = 0;
            RawChoiceData data = new RawChoiceData { lines = new List<string>(), endingIndex = 0 };

            for(int i = 0; i < currentConversation.Count; i++)
            {
                string line = currentConversation.GetLines()[i];
                data.lines.Add(line);

                if (isEncapsulateStart(line))
                {
                    EncapsulateDepth++;
                    continue;
                }

                if (isEncapsulateEnd(line))
                {
                    EncapsulateDepth--;
                    if(EncapsulateDepth == 0)
                    {
                        data.endingIndex = i;
                        break;
                    }
                }
            }
            return data;
        }

        private List<Choice> GetChoicesFormData(RawChoiceData data)
        {
            List<Choice> choices = new List<Choice>();
            int encapsulateonDepth = 0;
            bool isFirstChoice = true;

            Choice choice = new Choice{title = string.Empty,resultLines = new List<string>(),};

            foreach(var line in data.lines.Skip(1))
            {
                if(isChoiceStart(line) && encapsulateonDepth == 1)
                {
                    if (!isFirstChoice)
                    {
                        choices.Add(choice);
                        choice = new Choice { title = string.Empty, resultLines = new List<string>() };
                    }
                    choice.title = line.Trim().Substring(1);
                    isFirstChoice = false;
                    continue;
                }
                addLineToResult(line, ref choice, ref encapsulateonDepth);
            }
            if(!choices.Contains(choice))
                choices.Add(choice);

            return choices;
        }

        private void addLineToResult(string line, ref Choice choice, ref int encapsulatedepth)
        {
            line.Trim();

            if (isEncapsulateStart(line))
            {
                if(encapsulatedepth > 0)
                    choice.resultLines.Add(line);
                encapsulatedepth++;
                return;
            }

            if (isEncapsulateEnd(line))
            {
                encapsulatedepth--;

                if(encapsulatedepth == 0)
                    choice.resultLines.Add(line);

                return;
            }

            choice.resultLines.Add(line);
        }

        public bool Matches(DailogLine line)
        {
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyWord);
        }

        private bool isEncapsulateStart(string line) => line.Trim().StartsWith(Encapsulate_Start);
        private bool isEncapsulateEnd(string line) => line.Trim().StartsWith(Encapsulate_End);
        private bool isChoiceStart(string line) => line.Trim().StartsWith(Choice_Identifier);

        private struct RawChoiceData
        {
            public List<string> lines;
            public int endingIndex;
        }

        private struct Choice
        {
            public string title;
            public List<string> resultLines;
        }
    }
}