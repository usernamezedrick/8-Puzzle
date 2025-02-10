using UnityEngine;
using UnityEngine.UI;

public class BackgroundColorChanger : MonoBehaviour
{
    public Image backgroundImage;
    public Color startColor = new Color(0.77f, 0.87f, 0.97f, 1f); 
    public Color endColor = new Color(0.97f, 0.84f, 0.85f, 1f); 
    public float speed = 1.5f;

    void Update()
    {
        if (backgroundImage == null) return;
        float t = (Mathf.Sin(Time.time * speed) + 1) / 2;
        backgroundImage.color = Color.Lerp(startColor, endColor, t);
    }
}
