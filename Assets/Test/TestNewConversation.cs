using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNewConversation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(test());
    }
    IEnumerator test()
    {
        List<string> lines = new List<string>()
        {
            "line 1 jhasjfhasjfhajkfhdjakfa.",
            "line 2 fjasfhfhdajkfsdjkfhlkFHAKS.",
            "line 3 FHDUFHKJDSKJSHFJKSHFJS.",
        };

        yield return DialogController.Instance.Say(lines);

        DialogController.Instance.Hide(1);
    }

    // Update is called once per frame
    void Update()
    {
        List<string> lines = new List<string>();
        Conversation conversation = null;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            lines = new List<string>()
            {
                "This is start of an enqueued conversation",
                "We can keep going!"
            };
            conversation = new Conversation(lines);
            DialogController.Instance.conversationManager.Enqueue(conversation);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            lines = new List<string>()
            {
                "This is Importan NEWS!",
                "We have the prioriry!",
                "Hello"
            };
            conversation = new Conversation(lines);
            DialogController.Instance.conversationManager.EnqueuePriority(conversation);
        }

    }
}
