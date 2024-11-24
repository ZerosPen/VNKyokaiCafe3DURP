using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChoicePanel : MonoBehaviour
{
    //intance this script
    public static ChoicePanel instance { get; private set; }

    private const float Button_Min_Width = 50;
    private const float Button_Max_Width = 1000;
    private const float Button_Width_Padding = 25;

    private const float Button_Height_PerLine = 50;
    private const float Button_Height_Padding = 20;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titletext;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private HorizontalLayoutGroup buttonLayouGroup;

    private CanvasGroupController cg = null;

    private List<ChoiceButton> Buttons = new List<ChoiceButton>();

    public ChoicePanelDecision lastDecision {  get; private set; }

    public bool isWaitingPlayerChoice{ get; private set; }

    private void Awake()
    {
        instance = this;

        cg = new CanvasGroupController(this, canvasGroup);
        cg.alpha = 0;
        cg.SetInteractableState(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Show(string question, string[] choices)
    {
        lastDecision = new ChoicePanelDecision(question, choices);

        isWaitingPlayerChoice = true;

        cg.Show();
        cg.SetInteractableState(active: true);

        titletext.text = question;
        StartCoroutine(GenerateChoices(choices));
       
    }

    private IEnumerator GenerateChoices(string[] choices)
    {
        float maxWidth = 0;

        for(int i  = 0; i < choices.Length; i++)
        {
            ChoiceButton choiceButton;
            if(i < Buttons.Count)
            {
                choiceButton = Buttons[i];
            }
            else
            {
                GameObject newButtonObject = Instantiate(choiceButtonPrefab, buttonLayouGroup.transform);
                newButtonObject.SetActive(true);

                Button newButton =  newButtonObject.GetComponent<Button>();
                TextMeshProUGUI newTitle  = newButton.GetComponentInChildren<TextMeshProUGUI>();
                LayoutElement newLayout = newButton.GetComponent<LayoutElement>();

                choiceButton = new ChoiceButton { button = newButton, layout = newLayout, title = newTitle };

                Buttons.Add(choiceButton);
            }

            choiceButton.button.onClick.RemoveAllListeners();
            int buttonIndex = i;
            choiceButton.button.onClick.AddListener(() => AcceptAnswer(buttonIndex));
            choiceButton.title.text = choices[i];

            float buttonWidth = Mathf.Clamp(Button_Width_Padding + choiceButton.title.preferredWidth, Button_Min_Width, Button_Max_Width);
            maxWidth = Mathf.Max(maxWidth, buttonWidth);
        }
        foreach (var button in Buttons)
        {
            button.layout.preferredWidth = maxWidth;
        }

        for(int i = 0; 9 < Buttons.Count; i++)
        {
            bool show = i < choices.Length;
            Buttons[i].button.gameObject.SetActive(show);
        }
        yield return new WaitForEndOfFrame();

        foreach (var button in Buttons)
        {
            int lines = button.title.textInfo.lineCount;
            button.layout.preferredWidth = Button_Height_Padding + (Button_Height_PerLine * lines);
        }
    }

    public void Hide()
    {
        cg.Hide();
        cg.SetInteractableState(false);
    }

    private void AcceptAnswer(int index)
    {
        if (index < 0 || index > lastDecision.choices.Length - 1)
            return;

        lastDecision.answerIndex = index;
        isWaitingPlayerChoice = false;
        Hide();
    }

    public class ChoicePanelDecision 
    { 
        public string question = string.Empty;
        public int answerIndex = -1;
        public string[] choices = new string[0];
        
        public ChoicePanelDecision(string question, string[] choices)
        {
            this.question = question;
            this.choices = choices;
            answerIndex = -1;
        }
    }

    private struct ChoiceButton 
    { 
        public Button button;
        public TextMeshProUGUI title;
        public LayoutElement layout;
    }
}
