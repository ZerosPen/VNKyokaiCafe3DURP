using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(test());
    }

    Character CreatCharacter(string name) => CharacterManager.Instance.createChracter(name);

    IEnumerator test()
    {
        Character Monk = CreatCharacter("Monk as Generic");

        yield return Monk.Say("Normal Config data");

        Monk.SetDialogueColor(Color.blue);
        Monk.SetNameColor(Color.green);

        yield return Monk.Say("Customize Config data");

        Monk.ResetConfiguratioData();

        yield return Monk.Say("back too Normal Config data");
    }
}
