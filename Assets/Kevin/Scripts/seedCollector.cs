using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class seedCollector : MonoBehaviour
{
    public Text seedCountText;
    int seedCount = 0;

    // Update is called once per frame
    void Update()
    {
        seedCountText.text = seedCount.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Seed"))
        {
            Destroy(collision.gameObject);
            seedCount++;
        }
    }
}
