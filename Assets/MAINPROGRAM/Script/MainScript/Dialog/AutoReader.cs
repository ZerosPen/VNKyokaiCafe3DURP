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
        private const string Status_Text_Auto = "Auto";
        private const string Status_Text_Skip = "Skipping"; 

        private ConversationManager conversationManager;
        private TextArchitech textArchitech => conversationManager.TxtArch;

        public bool Skip { get; set; } = false;
        public float speed { get; set; } = 1f;

        public bool isOn => co_isRunning != null;
        private Coroutine co_isRunning = null;

        [SerializeField] private TextMeshProUGUI statusText;

        public void Initialized(ConversationManager conversationManager)
        {
            this.conversationManager = conversationManager;
            
            statusText.text = string.Empty;
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
            co_isRunning = null;
            statusText.text = string.Empty;
        }

        private IEnumerator AutoRead()
        {
            if (!conversationManager.isRunning)
            {
                Disable();  
                yield break;
            }

            if (!textArchitech.isBuildingText && textArchitech.currentText != string.Empty)
                DialogController.Instance.OnSystemPromt_Next();

            while(conversationManager.isRunning)
            {
                //read and wait
                if(!Skip)
                {
                    while (!textArchitech.isBuildingText && !conversationManager.isWaitingAutoTimer)
                    {
                        yield return null;
                    }

                    float timeStart = Time.time;

                    while (textArchitech.isBuildingText || conversationManager.isWaitingAutoTimer)
                    {
                        yield return null;
                    }

                    float timeToRead = Mathf.Clamp(((float)textArchitech.Tmpros.textInfo.characterCount / Default_Character_Reads_Per_Second), Min_Read_Time, Max_Read_Time);

                    timeToRead = Mathf.Clamp((timeToRead - (Time.time - timeStart)), Min_Read_Time, Max_Read_Time);
                    timeToRead = (timeToRead / speed) + Read_Time_Padding;

                    yield return new WaitForSeconds(timeToRead); 
                }

                //skip
                if (Skip)
                {
                    textArchitech.forceComplete();
                    yield return new WaitForSeconds(0.05f);
                }

                DialogController.Instance.OnSystemPromt_Next();
            }
            Disable();
        }

        public void Toggle_Auto()
        {
            bool prevState = Skip;
            Skip = false;
            
            if(Skip)
                Enable();
            else
            {
                if(!isOn)
                    Enable();
                else
                    Disable();
            }
            statusText.text = Status_Text_Auto;
        }

        public void Toggle_Skip()
        {
            bool prevState = Skip;
            Skip = true;

            if (!prevState)
                Enable();
            else
            {
                if (!isOn)
                    Enable();
                else
                    Disable();
            }
            statusText.text = Status_Text_Skip;
        }
    }
}