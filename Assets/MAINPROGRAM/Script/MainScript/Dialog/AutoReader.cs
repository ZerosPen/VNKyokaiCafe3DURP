using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DIALOGUE 
{
    public class AutoReader : MonoBehaviour
    {
        private const int Default_Character_Reads_Per_Second = 18;
        private const float Read_Time_Padding = 0.5f;
        private const float Max_Read_Time = 99f;
        private const float Min_Read_Time = 1f;

        private ConversationManager conversationManager;
        private TextArchitech textArchitech => conversationManager.TxtArch;

        public bool Skip { get; set; } = false;
        public float speed { get; set; } = 1f;

        private bool isOn => co_isRunning != null;
        private Coroutine co_isRunning = null;    

        public void Initialized(ConversationManager conversationManager)
        {
            this.conversationManager = conversationManager;
        }

        public void Enable()
        {
            if (isOn)
                return;

            co_isRunning = StartCoroutine(AutoRead());
        }

        public void Disable()
        {
            if(!isOn)
                return;
            
            StopCoroutine(co_isRunning);
            Skip = false;
        }

        private IEnumerator AutoRead()
        {
            if (!conversationManager.isRunning)
            {
                Disable();  
                yield break;
            }

            if (!textArchitech.isBuildingText && textArchitech.currentText != string.Empty)
                DialogController.Instance.OnUserPromt_Next();

            while(conversationManager.isRunning)
            {
                //read and wait
                if(!Skip)
                {
                    while (!textArchitech.isBuildingText)
                    {
                        yield return null;
                    }

                    float timeStart = Time.time;

                    while (textArchitech.isBuildingText)
                    {
                        yield return null;
                    }
                    float timeToRead = Mathf.Clamp(((float)textArchitech.tmp, Min_Read_Time, Max_Read_Time);

                }

                //skip
                if (Skip)
                {
                    textArchitech.forceComplete();
                    yield return new WaitForSeconds(0.05f);
                }

                DialogController.Instance.OnUserPromt_Next();
            }
            Disable();
        }
    }
}