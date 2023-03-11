using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    // GP = Gliding Power
    [Header("Gliding Attributes")]
    [SerializeField] private Image glidingBarFill;
    [SerializeField] private Slider slider;
    [SerializeField] private float maxGP = 100f;
    [SerializeField] private float currentGP;
    [SerializeField] private float usingGP;
    
    private bool canGlide;

    private void Start()
    {
        slider.maxValue = maxGP;
        currentGP = maxGP;

        slider.value = currentGP;
        usingGP = 0f;
    }
    
    public void UseGP(float amount)
    {
        if (currentGP - amount >= 0)
        {
            currentGP -= amount;
            slider.value = currentGP;
        }
    }

    public void RegenGP()
    {
        if (currentGP < maxGP)
        {
            currentGP += 5f * Time.deltaTime;
            slider.value = currentGP;
        }
    }

    public bool CanGlide()
    {
        canGlide = currentGP > usingGP ? canGlide = true : canGlide = false;
        return canGlide;
    }

    public bool IsGPMaxed()
    {
        return currentGP == maxGP;
    }
}
