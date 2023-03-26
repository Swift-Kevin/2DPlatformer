using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWalkthrough : MonoBehaviour
{
    [SerializeField] private CanvasGroup alphaPanel;
    [SerializeField] private GameObject obj;
    private void Awake()
    {
        alphaPanel.alpha = 0;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (obj.tag == "WalkHUD")
            alphaPanel.alpha = 1;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (obj.tag == "WalkHUD")
            alphaPanel.alpha = 0;
    }
}
