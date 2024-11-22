using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasGroupController
{
    private const float Default_Fading_Speed = 1.0f;

    private MonoBehaviour owner;
    private CanvasGroup rootCG;

    private Coroutine co_showing = null;
    private Coroutine co_hiding = null;

    public bool isShowing => co_showing != null;
    public bool isHiding => co_hiding != null;
    public bool isFading => isShowing || isHiding;

    public bool isVisible => co_showing != null || rootCG.alpha < 0;

    public CanvasGroupController(MonoBehaviour owner, CanvasGroup rootCG)
    {
        this.owner = owner;
        this.rootCG = rootCG;
    }

    public Coroutine Show(float speed = 1f, bool immadiate = false)
    {
        if (isShowing)
            return co_showing;

        else if (isHiding)
        {
            DialogController.Instance.StopCoroutine(co_hiding);
            co_hiding = null;
        }

        co_showing = DialogController.Instance.StartCoroutine(Fading(1, speed, immadiate));
        return co_showing;
    }

    public Coroutine Hide(float speed = 1f, bool immadiate = false)
    {
        if (isHiding)
            return co_hiding;

        else if (isShowing)
        {
            DialogController.Instance.StopCoroutine(co_showing);
            co_showing = null;
        }

        co_hiding = DialogController.Instance.StartCoroutine(Fading(0, speed, immadiate));
        return co_hiding;
    }

    private IEnumerator Fading(float alpha, float speed = 1f, bool immadiate = false)
    {
        CanvasGroup cg = rootCG;

        if(immadiate)
            cg.alpha = alpha; 

        while (cg.alpha != alpha)
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, alpha, Time.deltaTime * Default_Fading_Speed * speed);
            yield return null;
        }

        co_hiding = null;
        co_showing = null;
    }
}