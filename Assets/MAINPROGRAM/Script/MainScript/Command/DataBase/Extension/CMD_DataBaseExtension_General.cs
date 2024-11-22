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


        new public static void Extend(CommandDataBase dataBase)
        {
            dataBase.addCommand("wait", new Func<string, IEnumerator>(Wait));

            //command for DialogueController
            dataBase.addCommand("showui", new Func<string[], IEnumerator>(ShowDialogueController));
            dataBase.addCommand("hideui", new Func<string[], IEnumerator>(HideDialogueController));


            //command for dialogue box
            dataBase.addCommand("showdb", new Func<string[], IEnumerator>(ShowDialogueBox));
            dataBase.addCommand("hidedb", new Func<string[], IEnumerator>(HideDialogueBox));
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