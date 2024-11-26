using Characters;
using Commands;
using DIALOGUE.LogicalLine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    public class ConversationManager
    {
        private DialogController dialogController => DialogController.Instance;
        private Coroutine process = null;
        public bool isRunning => process != null;

        public  TextArchitech TxtArch = null;
        private bool UserPromt = false;

        private LogicalLineManager logicalLineManager;

        public Conversation conversation => (conversationQueue.IsEmpty() ? null : conversationQueue.top);
        public int conversationProgress => (conversationQueue.IsEmpty() ? -1 : conversationQueue.top.GetProgress());

        private ConversationQueue conversationQueue;

        public ConversationManager(TextArchitech TxtArch)
        {
            this.TxtArch = TxtArch;
            dialogController.onUserPrompt_Next += OnUserPromt_Next;

            logicalLineManager = new LogicalLineManager();

            conversationQueue = new ConversationQueue();
        }

        public void Enqueue(Conversation conversation) => conversationQueue.Enqueue(conversation); 
        public void EnqueuePriority(Conversation conversation) => conversationQueue.EnqueuePriority(conversation);

        private void OnUserPromt_Next()
        {
            UserPromt = true;
        }

        public Coroutine startConversation(Conversation conversation)
        {
            stopConversation();

            conversationQueue.Clear();

            Enqueue(conversation);  

            process = dialogController.StartCoroutine(RunningConversation());

            return process;
        }

        public void stopConversation()
        {
            if (!isRunning)
            {
                return;
            }
            dialogController.StopCoroutine(process);
            process = null;
        }

        IEnumerator RunningConversation()
        {
            while(!conversationQueue.IsEmpty())
            {
                Conversation currentConversation = conversation;

                if (currentConversation.HasReacedEnd())
                {
                    conversationQueue.Dequeue();
                    continue;
                }

                string rawLine = currentConversation.CurrentLine();

                if (string.IsNullOrWhiteSpace(rawLine))
                {
                    TryAdvanceConversation(currentConversation);
                    continue;
                }

                DailogLine line = DailogParser.Parse(rawLine);

                if(logicalLineManager.TryGetLogic(line, out Coroutine logic))
                {
                    yield return logic;
                }
                else
                {
                    //show dialogue
                    if (line.hasDialogue)
                    {
                        yield return Line_RunDialogue(line);
                    }

                    // shoew commmand
                    if (line.hasCommands)
                    {
                        yield return Line_RunCommands(line);
                    }

                    //wait for user input if dialogue has  in this line
                    if (line.hasDialogue)
                    {
                        //wiat for userInput
                        yield return WaitForUserInput();

                        CommandManager.Instance.StopAllProcesses();
                    }
                }

                TryAdvanceConversation(currentConversation);
            }
            process = null;
        }

        private void TryAdvanceConversation(Conversation convesation)
        {
            conversation.IncrementProgress();

            if (conversation != conversationQueue.top)
            {
                return;
            }

            if (conversation.HasReacedEnd())
                conversationQueue.Dequeue();
        }

        IEnumerator Line_RunDialogue(DailogLine line)
        {
            //show or hide char name
            if (line.hasSpeaker)
                handleSpeakLogic(line.speakerData);

            if(!dialogController.DialogContainer.isVisible)
                dialogController.DialogContainer.Show(1f, true);

            //build dailog
            yield return BuildLineSegments(line.dialogueData);

        }

        private void handleSpeakLogic(DL_SpeakerData speakerData)
        {
            // Determine if a character must be created based on the speaker data
            bool characterMustCreate = speakerData.makeCharacterEnter || speakerData.isCastingPos || speakerData.isCastExpresion;

            // Retrieve or create the character
            Character character = CharacterManager.Instance.GetCharacter(speakerData.name, creatIfDoesNotAxist: characterMustCreate);

            if (speakerData.makeCharacterEnter && (!character.isVisible && !character.isShowing))
            {
                character.Show();
            }

            // Display the speaker's name in the dialogue
            dialogController.showSpeakerName(TagManager.Inject (speakerData.displayName));

            DialogController.Instance.ApplySpeakerDataToDialogContainer(speakerData.name);

            // Check if the character should move and process casting expressions
            if (speakerData.isCastingPos)
            {
                // Move the character to the specified position
                character.MoveToNewPosition(speakerData.castPosition);
            }

            // Check for casting expressions
            if (speakerData.isCastExpresion)
            {
                foreach (var ce in speakerData.CastExpresion)
                    character.OnReceiveCastingExpression(ce.layer,ce.expression);
            } 
        }

        IEnumerator Line_RunCommands(DailogLine line)
        {
           List<DL_CommandData.Command> commands = line.commandData.commands;
           foreach(DL_CommandData.Command command in commands)
            {
                if (command.waitForComplete || command.name == "wait")
                {
                    CoroutineWrapper cw = CommandManager.Instance.Executed(command.name, command.arguments);
                    while(!cw.isDone)
                    {
                        if (UserPromt)
                        {
                            CommandManager.Instance.StopCurrentProcess();
                            UserPromt = false;
                        }
                        yield return null;
                    }
                }
                else
                    CommandManager.Instance.Executed(command.name, command.arguments);
            }
           yield return null;
        }

        IEnumerator BuildLineSegments(DL_DialogueData line)
        {
            for(int i = 0; i < line.segments.Count; i++)
            {
                DL_DialogueData.Dialogue_Segment segment = line.segments[i];

                yield return WaitForDialogueSegmentSingalToTrigger(segment);

                yield return BuildDialogue(segment.dialogue, segment.appendTxt);

            }
        }

        public bool isWaitingAutoTimer { get; private set; } = false;
        IEnumerator WaitForDialogueSegmentSingalToTrigger(DL_DialogueData.Dialogue_Segment segment)
        {
            switch(segment.startSingal)
            {
                case DL_DialogueData.Dialogue_Segment.StartSingal.C:
                case DL_DialogueData.Dialogue_Segment.StartSingal.A:
                    yield return WaitForUserInput();
                    break;
                case DL_DialogueData.Dialogue_Segment.StartSingal.WA:
                case DL_DialogueData.Dialogue_Segment.StartSingal.WC:
                    isWaitingAutoTimer = true;
                    yield return new WaitForSeconds(segment.singalDelay);
                    isWaitingAutoTimer = false;
                    break;
                default:
                    break;
            }
        }

        IEnumerator BuildDialogue(string dailogue, bool Append = false)
        {

            dailogue = TagManager.Inject(dailogue);

            if (!Append)
                TxtArch.Build(dailogue);
            else
                TxtArch.Append(dailogue);

            while (TxtArch.isBuildingText)
            {
                if (UserPromt)
                {
                    if (!TxtArch.speedUP)
                    {
                        TxtArch.speedUP = true;
                    }
                    else
                        TxtArch.forceComplete();
                }
                yield return null;
            }
        }

        IEnumerator WaitForUserInput()
        {
            dialogController.promt.Show();

            while (!UserPromt)
            {
                yield return null;
            }

            dialogController.promt.Hide();

            UserPromt = false;
        }
    }
}
