using UnityEngine;

public class BlockComponent : MonoBehaviour
{
    public int hitsToBreak = 1; // Golpes necesarios para romper el bloque
    private int currentHits = 0; // Contador de golpes recibidos
    private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer del bloque

    private void Start()
    {
        // Obtener el SpriteRenderer en el Start
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            currentHits++; // Aumentar el contador de golpes
            Debug.Log($"Golpes recibidos: {currentHits}/{hitsToBreak} para el bloque {gameObject.name}");

            // Cambiar el color del bloque
            ChangeColor();

            if (currentHits >= hitsToBreak) // Verificar si se alcanzó el número de golpes necesarios
            {
                Destroy(gameObject); // Destruir el bloque
                Debug.Log($"Bloque {gameObject.name} destruido después de {currentHits} golpes.");
            }
        }
    }

    private void ChangeColor()
    {
        // Calcular el nuevo porcentaje de blanco en función del número de golpes
        float whitePercentage = Mathf.Clamp(currentHits * 0.1f, 0f, 1f); // Aumentar el componente blanco hasta un máximo del 100%

        // Cambiar el color usando Lerp
        Color targetColor = Color.white; // Color blanco
        Color newColor = Color.Lerp(spriteRenderer.color, targetColor, whitePercentage); // Interpolar hacia el color blanco

        spriteRenderer.color = newColor; // Aplicar el nuevo color
    }
}