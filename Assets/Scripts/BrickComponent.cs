using TMPro;
using UnityEngine;
using System.Collections;

public class BlockComponent : MonoBehaviour
{
    public int hitsToBreak = 1;
    private int currentHits = 0;
    private SpriteRenderer spriteRenderer;
    public GameObject scoreTextPrefab;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Obtener el componente BouncyBall de la pelota
            BouncyBall bouncyBall = collision.gameObject.GetComponent<BouncyBall>();

            if (bouncyBall != null)
            {
                currentHits++;
                Debug.Log($"Golpes recibidos: {currentHits}/{hitsToBreak} para el bloque {gameObject.name}");

                ChangeColor();

                if (currentHits >= hitsToBreak)
                {
                    // Obtener la puntuación acumulada del componente BouncyBall
                    int scoreToDisplay = bouncyBall.hasHitPaddleOrLost ? bouncyBall.baseScore : bouncyBall.additionalScoreAccumulated;

                    // Mostrar el texto de la puntuación sobre el bloque destruido
                    ShowScoreText(scoreToDisplay);

                    Debug.Log($"Bloque {gameObject.name} destruido después de {currentHits} golpes.");
                    Destroy(gameObject);
                }
            }
        }
    }

    private void ChangeColor()
    {
        float whitePercentage = Mathf.Clamp(currentHits * 0.2f, 0f, 1f);

        Color targetColor = Color.white;
        Color newColor = Color.Lerp(spriteRenderer.color, targetColor, whitePercentage);

        spriteRenderer.color = newColor;
    }

    public int blockScore = 10;

    public void ShowScoreText(int scoreToDisplay)
    {
        Debug.Log("Mostrando texto de puntuación.");

        Canvas canvas = FindObjectOfType<Canvas>();
        GameObject scoreText = Instantiate(scoreTextPrefab, canvas.transform);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        RectTransform rectTransform = scoreText.GetComponent<RectTransform>();

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPosition,
            canvas.worldCamera,
            out localPoint
        );

        rectTransform.localPosition = localPoint;
        rectTransform.anchoredPosition += new Vector2(0, 30f);

        TextMeshProUGUI textMesh = scoreText.GetComponent<TextMeshProUGUI>();
        textMesh.text = scoreToDisplay.ToString();

        StartCoroutine(FadeOutText(scoreText));
    }

    private IEnumerator FadeOutText(GameObject textObject)
    {
        TextMeshProUGUI textMesh = textObject.GetComponent<TextMeshProUGUI>();
        Color originalColor = textMesh.color;

        Debug.Log("Iniciando fade out para: " + textObject.name);

        yield return new WaitForSeconds(0.5f); // Esperar medio segundo

        float fadeDuration = 1f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsed / fadeDuration);
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(textObject);
    }
}