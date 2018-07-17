using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlashingText : MonoBehaviour {

    public float Speed = 3;

    TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    float p_t;
    void Flash()
    {
        p_t += Time.deltaTime * Speed;
        text.alpha = Mathf.Abs(Mathf.Sin(p_t * Mathf.Deg2Rad)) * 0.7f + 0.3f;
    }

    private void Update()
    {
        Flash();
    }
}
