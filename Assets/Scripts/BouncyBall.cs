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
    public int maxWallBounces = 10; // Rebotes m�ximos antes de cambiar direcci�n

    public TextMeshProUGUI scoreTxt;
    public GameObject[] livesImage;

    public GameObject gameOverPanel;

    // Nueva variable para el paddle
    public PlayerMovement paddle; // Asigna esto en el Inspector o busca en Start()

    public int maxScore = 0;  // Variable para almacenar la puntuaci�n m�xima
    public TextMeshProUGUI maxScoreTxt; // Text para mostrar la puntuaci�n m�xima

    // Nuevas variables para la puntuaci�n
    public int baseScore = 10;  // Puntuaci�n base por el primer bloque
    public int additionalScore = 5; // Puntos adicionales por bloques consecutivos
    public int additionalScoreAccumulated = 0; // Puntos acumulados por bloques destruidos
    public bool hasHitPaddleOrLost = true; // Para controlar si la bola ha vuelto al paddle o ha perdido

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero; // Asegurarse de que la pelota est� quieta
        rb.isKinematic = true; // Evitar que se mueva al inicio

        // Si no se asigna el paddle en el Inspector, se busca autom�ticamente
        if (paddle == null)
        {
            paddle = GameObject.FindObjectOfType<PlayerMovement>();
        }

        // Cargar la puntuaci�n m�xima guardada
        maxScore = PlayerPrefs.GetInt("00000", 0);

        // Actualizar la puntuaci�n m�xima en la pantalla
        maxScoreTxt.text = maxScore.ToString("00000");
    }

    void Update()
    {
        if (constrainInsideScreen)
        {
            // Mantener la bola dentro de los l�mites de la pantalla
            Vector3 position = transform.position;
            position.x = Mathf.Clamp(position.x, -2.808f, 2.808f); // Ajusta los l�mites seg�n tu juego
            position.y = Mathf.Clamp(position.y, -4.997f, 4.997f);
            transform.position = position;
        }

        if (!isLaunched && Input.GetKeyDown(KeyCode.Space))
        {
            LaunchBall(); // Lanza la pelota al presionar espacio
        }

        // Reiniciar la posici�n si cae demasiado bajo
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

                if (lives >= 0) // Verifica que a�n haya vidas disponibles
                {
                    livesImage[lives].SetActive(false);
                }

                if (lives <= 0)
                {
                    GameOver(); // Llamar a GameOver si acabas de perder la �ltima vida
                }
                else
                {
                    ResetBall(); // Si a�n tienes vidas, restablecer la pelota
                }
            }
        }

        // Limitar la velocidad m�xima
        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
        }

        if (GameObject.FindGameObjectsWithTag("Brick").Length == 0)
        {
            ResetBall();
            LevelGenerator levelGenerator = FindObjectOfType<LevelGenerator>();
            levelGenerator.LevelCompleted(); // Avanzar al siguiente nivel
        }
    }

    public void LaunchBall()
    {
        rb.isKinematic = false; // Habilitar la f�sica de la pelota
        rb.AddForce(Vector2.up * launchForce, ForceMode2D.Impulse); // Lanzar hacia arriba con una fuerza
        isLaunched = true; // Marcar como lanzada
    }

    public void ResetBall()
    {
        transform.position = new Vector3(0, -3.4f, 0);
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        isLaunched = false;

        paddle.ResetPosition();

        // Resetear la l�gica de puntuaci�n
        additionalScoreAccumulated = 0; // Reseteamos la puntuaci�n acumulada
        hasHitPaddleOrLost = true; // Marcamos que la bola ha vuelto al paddle o ha perdido
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            wallBounceCount = 0;

            int puntosBloque = hasHitPaddleOrLost ? baseScore : additionalScoreAccumulated;
            BlockComponent blockComponent = collision.gameObject.GetComponent<BlockComponent>();

            if (hasHitPaddleOrLost)
            {
                score += baseScore; // Sumar 10 puntos por el primer bloque
                additionalScoreAccumulated = baseScore; // Iniciar la acumulaci�n desde la base
                puntosBloque = baseScore; // Puntos a mostrar
            }
            else
            {
                additionalScoreAccumulated += additionalScore; // Sumar 5 puntos adicionales
                score += additionalScoreAccumulated; // Sumar al score la puntuaci�n acumulada
                puntosBloque = additionalScoreAccumulated; // Puntos a mostrar
            }

            hasHitPaddleOrLost = false; // Marcamos que la bola ha golpeado un bloque

            scoreTxt.text = score.ToString("00000");
            Debug.Log("Puntos bloque "+ score);
            blockComponent.ShowScoreText(puntosBloque);

            if (score > maxScore)
            {
                maxScore = score;
                maxScoreTxt.text = maxScore.ToString("00000");

                // Guardar la nueva puntuaci�n m�xima
                PlayerPrefs.SetInt("00000", maxScore);
                PlayerPrefs.Save();  // Guardar los datos
            }
        }
    else if (collision.gameObject.CompareTag("Paddle"))
    {
        wallBounceCount = 0; // Resetear el contador si toca el paddle
        hasHitPaddleOrLost = true; // La bola ha vuelto al paddle
    }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            wallBounceCount++; // Incrementar el contador si toca la pared

            if (wallBounceCount >= maxWallBounces)
            {
                // Cambiar la direcci�n de la pelota hacia el paddle
                RedirectToPaddle();
            }
        }

        // Aumentar la velocidad de la pelota despu�s de cada rebote
        float speedIncreaseFactor = 1.05f; // Incremento del 5% en cada colisi�n
        rb.velocity *= speedIncreaseFactor;

        // Limitar la velocidad m�xima
        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity; // Normalizar y mantener la velocidad m�xima
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

    public void RedirectToPaddle()
    {
        // Calcular la direcci�n hacia el paddle
        Vector2 directionToPaddle = (paddle.transform.position - transform.position).normalized;

        // Asignar la nueva direcci�n hacia el paddle
        rb.velocity = directionToPaddle * rb.velocity.magnitude; // Mantener la velocidad actual
        wallBounceCount = 0; // Reiniciar el contador despu�s de redirigir
    }

    void GameOver()
    {
        ResetBall();
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }
}
