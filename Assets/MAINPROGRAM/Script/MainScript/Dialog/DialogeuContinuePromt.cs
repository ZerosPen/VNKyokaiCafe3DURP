using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace DIALOGUE
{
    public class DialogeuContinuePromt : MonoBehaviour
    {
        private RectTransform root;

        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshProUGUI tmpro; 

        public bool isShowwing => animator.gameObject.activeSelf;

        // Start is called before the first frame update
        void Start()
        {
            root = GetComponent<RectTransform>();

        }

        public void Show()
        {
            if(tmpro.text == string.Empty)
            {
                if(isShowwing)
                    Hide();

                return;
            }
                
            tmpro.ForceMeshUpdate();

            animator.gameObject.SetActive(true);    
            root.transform.SetParent(tmpro.transform);

            TMP_CharacterInfo finalCharacter = tmpro.textInfo.characterInfo[tmpro.textInfo.characterCount - 1];
            Vector3 targertPos = finalCharacter.bottomRight;
            float characterWidth = finalCharacter.pointSize * 0.5f;
            targertPos = new Vector3(targertPos.x + characterWidth, targertPos.y, 0);

            root.localPosition = targertPos;

        }

        public void Hide()
        {
            animator.gameObject.SetActive(false);
        }
    }
}