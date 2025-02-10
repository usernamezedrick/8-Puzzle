using UnityEngine;
using UnityEngine.UI;

public class BreathingButton : MonoBehaviour
{
    public Outline outlineEffect;
    public Color startColor = new Color(0.88f, 0.73f, 0.89f, 1f); 
    public Color endColor = new Color(0.82f, 0.57f, 0.74f, 1f); 
    public float speed = 2f;

    void Update()
    {
        if (outlineEffect == null) return;
        float t = (Mathf.Sin(Time.time * speed) + 1) / 2;
        outlineEffect.effectColor = Color.Lerp(startColor, endColor, t);
    }
}
