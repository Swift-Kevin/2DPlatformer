using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWalkthrough : MonoBehaviour
{
    [SerializeField] private GameObject walkPanel;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        walkPanel.SetActive(true);

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        walkPanel.SetActive(false);
    }
}
