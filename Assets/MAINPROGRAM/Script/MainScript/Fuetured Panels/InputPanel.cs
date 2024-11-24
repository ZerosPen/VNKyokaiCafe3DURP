using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class InputPanel : MonoBehaviour
{
    public static InputPanel instance { get; private set; } = null;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text titletext;
    [SerializeField] private Button acceptButton;
    [SerializeField] private TMP_InputField inputField;

    private CanvasGroupController cg;

    public string lastInput { get; private set; } = string.Empty;
    public bool isWaitingUserInput { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        cg = new CanvasGroupController(this, canvasGroup);
        
        cg.alpha = 0;
        cg.SetInteractableState(active: false);
        acceptButton.gameObject.SetActive(false);
        inputField.onValueChanged.AddListener(OnInputChange);

        acceptButton.onClick.AddListener(onAcceptInput);
    }

    public void Show(string title)
    {
        titletext.text = title;
        inputField.text = string.Empty;
        cg.Show();

        cg.SetInteractableState(active: true);
        isWaitingUserInput = true;
    }

    public void Hide() 
    {
        cg.Hide();
        cg.SetInteractableState(active: false);
        isWaitingUserInput = false;
    }

    public void onAcceptInput()
    {
        if (inputField.text == string.Empty)
            return;

        lastInput = inputField.text;
        Hide();
    }

    public void OnInputChange(string value)
    {
        acceptButton.gameObject.SetActive(HasvalidText());
    }

    private bool HasvalidText()
    {   
        return inputField.text != string.Empty;
    }
}
