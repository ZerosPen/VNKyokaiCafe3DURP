using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DIALOGUE.LogicalLine
{
    public class LL_Choice : ILogicalLines
    {
        public string keyword => "choice";
        private const char Encapsulate_Start = '{';
        private const char Encapsulate_End = '}';
        private const char Choice_Identifier = '-';

        public IEnumerator Execute(DailogLine line) // Corrected spelling
        {
            RawChoiceData data = RipChoiceData();

            List<Choice> choices = GetChoicesFormData(data);

            string title = line.dialogueData.rawData;
            ChoicePanel panel = ChoicePanel.instance;

            string[] choiceTitle = choices.Select(c => c.title).ToArray();

            panel.Show(title, choiceTitle);

            while (panel.isWaitingPlayerChoice)
                yield return null;

            if (panel.lastDecision.answerIndex < 0 || panel.lastDecision.answerIndex >= choices.Count)
            {
                // Handle error: invalid choice index
                yield break; // or return early
            }

            Choice selectedChoice = choices[panel.lastDecision.answerIndex];

            Conversation newConversation = new Conversation(selectedChoice.resultLines);
            DialogController.Instance.conversationManager.conversation.SetProgress(data.endingIndex);
            DialogController.Instance.conversationManager.EnqueuePriority(newConversation);
        }

        public bool Matches(DailogLine line)
        {
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
        }

        private RawChoiceData RipChoiceData()
        {
            // Add null checks for safety
            if (DialogController.Instance == null ||
                DialogController.Instance.conversationManager == null ||
                DialogController.Instance.conversationManager.conversation == null)
            {
                // Handle error
                return new RawChoiceData { lines = new List<string>(), endingIndex = 0 };
            }

            Conversation currentConversation = DialogController.Instance.conversationManager.conversation;
            int currentProgress = DialogController.Instance.conversationManager.conversationProgress;
            int encapsulationDepth = 0;
            RawChoiceData data = new RawChoiceData { lines = new List<string>(), endingIndex = 0 };

            for (int i = currentProgress; i < currentConversation.Count; i++)
             {
                string line = currentConversation.GetLines()[i];
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

        private List<Choice> GetChoicesFormData(RawChoiceData data)
        {
            List<Choice> choices = new List<Choice>();
            int encapsulationDepth = 0;
            bool isFirstChoice = true;

            Choice choice = new Choice { title = string.Empty, resultLines = new List<string>(), };

            foreach (var line in data.lines.Skip(1))
            {
                if (isChoiceStart(line) && encapsulationDepth == 1)
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
                addLineToResult(line, ref choice, ref encapsulationDepth);
            }
            if (!choices.Any(c => c.title == choice.title)) // Check for duplicates by title
                choices.Add(choice);

            return choices;
        }

        private void addLineToResult(string line, ref Choice choice, ref int encapsulationDepth)
        {
            line = line.Trim(); // Store the trimmed value

            if (isEncapsulateStart(line))
            {
                if (encapsulationDepth > 0)
                    choice.resultLines.Add(line);
                encapsulationDepth++;
                return;
            }

            if (isEncapsulateEnd(line))
            {
                encapsulationDepth--;

                if (encapsulationDepth > 0)
                    choice.resultLines.Add(line);

                return;
            }

            choice.resultLines.Add(line);
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