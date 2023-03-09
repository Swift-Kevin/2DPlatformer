using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    [Header("Gliding Attributes")]
    [SerializeField] private Image glidingBarFill;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float desiredDuration = 3f;
    [SerializeField] private float rechargeDuration = 3f;
    [SerializeField] private float elapsedTime;
    [SerializeField] private float glidingAmount = 100;
    [SerializeField] private float currentGlidingPower;
    [SerializeField] private float maxGlidingPower;
    private Coroutine regenerateStamina;
    private WaitForSeconds regenTick = new WaitForSeconds(0.1f);

    private void Start()
    {
        currentGlidingPower = maxGlidingPower;
        glidingBarFill.fillAmount = maxGlidingPower;
    }
    
    public void UseStamina(int amount)
    {
        if (currentGlidingPower - amount < 0)
        {
            currentGlidingPower -= amount;
            glidingBarFill.fillAmount = currentGlidingPower;
            if (regenerateStamina != null)
                StopCoroutine(regenerateStamina);

            regenerateStamina = StartCoroutine(StaminaRegen());
        }
        else
        {
            Debug.Log("Not Enough Gliding Power");
        }
    }
    private IEnumerator StaminaRegen()
    {
        yield return new WaitForSeconds(2);
        while (currentGlidingPower < maxGlidingPower)
        {
            float percentFilled = elapsedTime / desiredDuration;
            currentGlidingPower =
            glidingBarFill.fillAmount = Mathf.Lerp(0, 1, Mathf.SmoothStep(0, 1, percentFilled));
            yield return regenTick;
        }
    }
}