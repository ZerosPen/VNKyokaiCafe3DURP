using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DIALOGUE;

[System.Serializable]// able to see all privet variable
public class DialogContainer
{
    private const float Default_Fading_Speed = 1.0f;

    public GameObject root;
    public NameContainer nameContainer;
    public TextMeshProUGUI DialogText;

    private CanvasGroupController cgController;

    public void SetDialogueColor(Color color) => DialogText.color = color;
    public void SetDialogueFont(TMP_FontAsset font) => DialogText.font = font;
    public void SetDialogueFontSize(float size) => DialogText.fontSize = size;

    private bool initialized = false;
    public void Initialized()
    {
        if (initialized) 
            return;

        cgController = new CanvasGroupController(DialogController.Instance, root.GetComponent<CanvasGroup>());
    }

    public bool isVisible => cgController.isVisible;
    public Coroutine Show(float speed, bool immadiate = false) => cgController.Show(speed, immadiate);
    public Coroutine Hide(float speed, bool immadiate = false) => cgController.Hide(speed, immadiate);
}
