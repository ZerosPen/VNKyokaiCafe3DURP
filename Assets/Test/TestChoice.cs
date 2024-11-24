using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestChoice : MonoBehaviour
{
    ChoicePanel choicePanel;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(test());
    }

    IEnumerator test()
    {
        choicePanel = ChoicePanel.instance;

        string[] choices = new string[]
        {
            "Witness? Is that camere on?",
            "Oh, nah!",
            "I did't see nothing!",
            "Matta' Fact- I'm blind in my left eye and 43% blind in my right eye."
        };
        choicePanel.Show("Did ypu see the witness?", choices);

        while (choicePanel.isWaitingPlayerChoice)
            yield return null;

        var decision = choicePanel.lastDecision;

        Debug.Log($"Made decision {decision.answerIndex} '{decision.choices[decision.answerIndex]}'");
    }
}
