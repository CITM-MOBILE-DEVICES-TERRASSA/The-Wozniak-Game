using UnityEngine;

public class AspectRatioManager : MonoBehaviour
{
    public float targetAspect = 9f / 16f;
    public GameObject blackBars;

    void Start()
    {
        UpdateAspectRatio();
    }

    void UpdateAspectRatio()
    {
        Camera camera = Camera.main;

        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Rect rect = camera.rect;

        blackBars.SetActive(false);

        if (scaleHeight < 1.0f)
        {
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            blackBars.SetActive(true);
            blackBars.transform.localScale = new Vector3(1f, scaleHeight, 1f);
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            blackBars.SetActive(true);
            blackBars.transform.localScale = new Vector3(scaleWidth, 1f, 1f);
        }
        
        camera.rect = rect;
    }
}