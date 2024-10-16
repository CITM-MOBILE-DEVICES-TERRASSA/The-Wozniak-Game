using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BouncyBall : MonoBehaviour
{
    public float minY = -5.5f;
    public float maxVelocity = 15f;
    public float launchForce = 10f; // Fuerza inicial
    public bool isLaunched = false; // Estado de si la pelota fue lanzada o no
    Rigidbody2D rb;

    public bool constrainInsideScreen = false;

    public int score = 0;
    public int lives = 3;

    private int wallBounceCount = 0; // Contador de rebotes en la pared
    public int maxWallBounces = 10; // Rebotes máximos antes de cambiar dirección

    public TextMeshProUGUI scoreTxt;
    public GameObject[] livesImage;

    public GameObject gameOverPanel;

    // Nueva variable para el paddle
    public PlayerMovement paddle; // Asigna esto en el Inspector o busca en Start()

    public int maxScore = 0;  // Variable para almacenar la puntuación máxima
    public TextMeshProUGUI maxScoreTxt; // Text para mostrar la puntuación máxima

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero; // Asegurarse de que la pelota esté quieta
        rb.isKinematic = true; // Evitar que se mueva al inicio

        // Si no se asigna el paddle en el Inspector, se busca automáticamente
        if (paddle == null)
        {
            paddle = GameObject.FindObjectOfType<PlayerMovement>();
        }

        // Cargar la puntuación máxima guardada
        maxScore = PlayerPrefs.GetInt("00000", 0);

        // Actualizar la puntuación máxima en la pantalla
        maxScoreTxt.text = maxScore.ToString("00000");
    }

    void Update()
    {
        if (constrainInsideScreen)
        {
            // Mantener la bola dentro de los límites de la pantalla
            Vector3 position = transform.position;
            position.x = Mathf.Clamp(position.x, -2.808f, 2.808f); // Ajusta los límites según tu juego
            position.y = Mathf.Clamp(position.y, -4.997f, 4.997f);
            transform.position = position;
        }


        if (!isLaunched && Input.GetKeyDown(KeyCode.Space))
        {
            LaunchBall(); // Lanza la pelota al presionar espacio
        }

        // Reiniciar la posición si cae demasiado bajo
        if (transform.position.y < minY)
        {
            if (lives <= 0)
            {
                GameOver();
            }
            else
            {
                ResetBall();
                lives--;
                livesImage[lives].SetActive(false);
            }
        }

        // Limitar la velocidad máxima
        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
        }
    }

    private void LaunchBall()
    {
        rb.isKinematic = false; // Habilitar la física de la pelota
        rb.AddForce(Vector2.up * launchForce, ForceMode2D.Impulse); // Lanzar hacia arriba con una fuerza
        isLaunched = true; // Marcar como lanzada
    }

    private void ResetBall()
    {
        transform.position = new Vector3(0, -3.4f, 0); // Cambiar posición de reaparición a (0, -3.4)
        rb.velocity = Vector3.zero;
        rb.isKinematic = true; // Detener movimiento
        isLaunched = false; // Lista para ser lanzada nuevamente

        // Llamar al método ResetPosition() del paddle si existe
        if (paddle != null)
        {
            paddle.ResetPosition();
        }
        else
        {
            Debug.LogError("Paddle no está asignado o no se encuentra en la escena.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            Destroy(collision.gameObject);
            wallBounceCount = 0; // Resetear el contador si toca un bloque

            score += 10;
            scoreTxt.text = score.ToString("00000");

            if (score > maxScore)
            {
                maxScore = score;
                maxScoreTxt.text = maxScore.ToString("00000");

                // Guardar la nueva puntuación máxima
                PlayerPrefs.SetInt("00000", maxScore);
                PlayerPrefs.Save();  // Guardar los datos
            }
        }
        else if (collision.gameObject.CompareTag("Paddle"))
        {
            wallBounceCount = 0; // Resetear el contador si toca el paddle
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            wallBounceCount++; // Incrementar el contador si toca la pared

            if (wallBounceCount >= maxWallBounces)
            {
                // Cambiar la dirección de la pelota hacia el paddle
                RedirectToPaddle();
            }
        }



        // Aumentar la velocidad de la pelota después de cada rebote
        float speedIncreaseFactor = 1.05f; // Incremento del 5% en cada colisión
        rb.velocity *= speedIncreaseFactor;

        // Limitar la velocidad máxima
        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity; // Normalizar y mantener la velocidad máxima
        }

        // Comprobar si la velocidad vertical (Y) es demasiado baja (rebote lateral)
        if (Mathf.Abs(rb.velocity.y) < 0.5f)
        {
            // Forzar un ajuste en la velocidad vertical
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + 0.5f * Mathf.Sign(rb.velocity.y == 0 ? 1 : rb.velocity.y));
        }

        // Comprobar si la velocidad horizontal (X) es demasiado baja (rebote completamente vertical)
        if (Mathf.Abs(rb.velocity.x) < 0.5f)
        {
            // Forzar un ajuste en la velocidad horizontal
            rb.velocity = new Vector2(rb.velocity.x + 0.5f * Mathf.Sign(rb.velocity.x == 0 ? 1 : rb.velocity.x), rb.velocity.y);
        }
    }

    private void RedirectToPaddle()
    {
        // Calcular la dirección hacia el paddle
        Vector2 directionToPaddle = (paddle.transform.position - transform.position).normalized;

        // Asignar la nueva dirección hacia el paddle
        rb.velocity = directionToPaddle * rb.velocity.magnitude; // Mantener la velocidad actual
        wallBounceCount = 0; // Reiniciar el contador después de redirigir
    }

    void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
        GameObject[] powerUps = GameObject.FindGameObjectsWithTag("PowerUp");
        foreach (GameObject powerUp in powerUps)
        {
                Destroy(powerUp);
        }
    }
}