using TMPro;
using UnityEngine;
using System.Collections;

public class BlockComponent : MonoBehaviour
{
    public int hitsToBreak = 1;
    public int currentHits = 0;
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
            BouncyBall bouncyBall = collision.gameObject.GetComponent<BouncyBall>();

            if (bouncyBall != null)
            {
                currentHits++;
                Debug.Log($"Golpes recibidos: {currentHits}/{hitsToBreak} para el bloque {gameObject.name}");

                ChangeColor();

                if (currentHits >= hitsToBreak)
                {
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

    
}