using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;

public class InputPanelTest : MonoBehaviour
{
    public InputPanel panel;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(test());
    }

    // Update is called once per frame
    IEnumerator test()
    {
        Character Makira = CharacterManager.Instance.createChracter("Makira", revealAfterCreation: true);

        yield return Makira.Say("Hello, sir{wc 0.5} what's your name?");

        panel.Show("What's is ypur name?");

        while(panel.isWaitingUserInput)
            yield return null;

        string CharacternName = panel.lastInput;

        yield return Makira.Say($"It's very nice to meet you, {CharacternName}");
    }
}
