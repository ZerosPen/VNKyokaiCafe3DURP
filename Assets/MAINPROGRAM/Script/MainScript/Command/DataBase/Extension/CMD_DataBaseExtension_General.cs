using DIALOGUE;
using System;
using System.Collections;
using System.Collections.Generic;
using TESTING;
using UnityEngine;

namespace Commands
{
    public class CMD_DataBaseExtension_General : cmd_DataBaseExtension
    {
        private static string[] Param_Speed => new string[] { "-spd", "-speed" };
        private static string[] Param_Immadiate => new string[] { "-i", "-immadiate" };
        private static string[] Param_FilePath => new string[] { "-fl", "-file" ,"-filepath" };
        private static string[] Param_Enqueue => new string[] { "-e", "-enqueue" };


        new public static void Extend(CommandDataBase dataBase)
        {
            dataBase.addCommand("wait", new Func<string, IEnumerator>(Wait));

            //command for DialogueController
            dataBase.addCommand("showui", new Func<string[], IEnumerator>(ShowDialogueController));
            dataBase.addCommand("hideui", new Func<string[], IEnumerator>(HideDialogueController));


            //command for dialogue box
            dataBase.addCommand("showdb", new Func<string[], IEnumerator>(ShowDialogueBox));
            dataBase.addCommand("hidedb", new Func<string[], IEnumerator>(HideDialogueBox));

            dataBase.addCommand("load", new Action<string[]>(loadNewDialogueFile));
        }

        private static void loadNewDialogueFile(string[] data)
        {
            string fileName = string.Empty;
            bool enqueue = false;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(Param_FilePath, out fileName);

            parameters.TryGetValue(Param_Enqueue, out enqueue, defaultValue: false);

            string filePath = FilePaths.GetPathToResource(FilePaths.resources_DialogueFiles, fileName); 
            
            TextAsset file = Resources.Load<TextAsset>(filePath);

            if(file == null) 
            {
                Debug.LogWarning($"File '{filePath}' could not be loaded from dialogue files. please ensure it exists within the '{FilePaths.resources_DialogueFiles}' resources folder.");
                return;
            }

            List<string> lines = FileManager.readTxtAsset(file, includeBlankLines: true );
            Conversation newConversation = new Conversation(lines);

            if (enqueue)
                DialogController.Instance.conversationManager.Enqueue(newConversation);
            else
                DialogController.Instance.conversationManager.startConversation(newConversation);
        }

        private static IEnumerator Wait(string data)
        {
            if (float.TryParse(data, out float time))
            {
                yield return new WaitForSeconds(time);
            }
        }

        private static IEnumerator ShowDialogueBox(string[] data)
        {
            float speed;
            bool immadiate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(Param_Speed, out speed, defaultValue: 1f);

            parameters.TryGetValue(Param_Immadiate, out immadiate, defaultValue: false);

            yield return DialogController.Instance.DialogContainer.Show(speed, immadiate);
        }

        private static IEnumerator HideDialogueBox(string[] data)
        {
            float speed;
            bool immadiate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(Param_Speed, out speed, defaultValue: 1f);

            parameters.TryGetValue(Param_Immadiate, out immadiate, defaultValue: false);

            yield return DialogController.Instance.DialogContainer.Hide(speed, immadiate);
        }

        private static IEnumerator ShowDialogueController(string[] data) 
        {
            float speed;
            bool immadiate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(Param_Speed, out speed, defaultValue: 1f);

            parameters.TryGetValue(Param_Immadiate, out immadiate, defaultValue: false);

            yield return DialogController.Instance.Show(speed, immadiate);
        }

        private static IEnumerator HideDialogueController(string[] data) 
        {
            float speed;
            bool immadiate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(Param_Speed, out speed, defaultValue: 1f);

            parameters.TryGetValue(Param_Immadiate, out immadiate, defaultValue: false);

            yield return DialogController.Instance.Hide(speed, immadiate);
        }
    }
}