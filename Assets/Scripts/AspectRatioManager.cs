using UnityEngine;

public class AspectRatioManager : MonoBehaviour
{
    public float targetAspect = 9f / 16f; // Relación de aspecto 9:16
    public GameObject blackBars; // Referencia al objeto de bandas negras

    void Start()
    {
        UpdateAspectRatio();
    }

    void UpdateAspectRatio()
    {
        // Obtiene la cámara principal
        Camera camera = Camera.main;

        // Calcula la relación de aspecto actual
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Rect rect = camera.rect;

        // Desactiva el panel al inicio
        blackBars.SetActive(false);

        if (scaleHeight < 1.0f)
        {
            // Bandas negras en los lados
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            // Activa las bandas laterales
            blackBars.SetActive(true);
            // Aquí ajustas el tamaño de las bandas laterales
            blackBars.transform.localScale = new Vector3(1f, scaleHeight, 1f);
        }
        else
        {
            // Bandas negras arriba y abajo
            float scaleWidth = 1.0f / scaleHeight;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            // Activa las bandas superiores e inferiores
            blackBars.SetActive(true);
            // Ajusta el tamaño de las bandas superiores e inferiores
            blackBars.transform.localScale = new Vector3(scaleWidth, 1f, 1f);
        }

        camera.rect = rect;
    }
}
