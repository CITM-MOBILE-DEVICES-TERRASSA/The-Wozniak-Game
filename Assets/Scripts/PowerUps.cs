using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { ExtraBalls, LargerPaddle, FasterBall }
    public PowerUpType powerUpType;

    public float fallSpeed = 2f; // Velocidad de caída constante
    public float powerUpDuration = 10f; // Duración de los power-ups temporales

    private PlayerMovement paddle;
    private BouncyBall ball;
    public LayerMask ballLayer; // Capa para las bolas

    private bool isPaddleLarge = false; // Verificar si el paddle ya está agrandado
    private float enlargeRemainingTime = 0f; // Tiempo restante para el power-up del paddle grande

    void Start()
    {
        // Buscar el paddle y la pelota en la escena
        paddle = GameObject.FindObjectOfType<PlayerMovement>();
        ball = GameObject.FindObjectOfType<BouncyBall>();
    }

    void Update()
    {
        // Mover el power-up hacia abajo a una velocidad constante
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Si el power-up colisiona con el paddle
        if (other.gameObject.CompareTag("Paddle"))
        {
            ActivatePowerUp();
        }
    }

    public void ActivatePowerUp()
    {
        switch (powerUpType)
        {
            case PowerUpType.ExtraBalls:
                RegenerateLife();
                break;
            case PowerUpType.LargerPaddle:
                if (!isPaddleLarge) // Agrandar el paddle si no está ya agrandado
                {
                    StartCoroutine(EnlargePaddle());
                }
                else
                {
                    // Si ya está agrandado, acumular el tiempo
                    enlargeRemainingTime += powerUpDuration;
                }
                break;
            case PowerUpType.FasterBall:
                SpeedUpBall();
                break;
        }

        // Destruir el power-up después de activarlo
        Destroy(gameObject);
    }
    void RegenerateLife()
    {
        // Verificar si el jugador tiene menos de 5 vidas
        if (ball.lives < 5)
        {
            ball.lives += 1; // Regenerar una vida
            ball.livesImage[ball.lives].SetActive(true);
            Debug.Log("Vida regenerada! Vidas actuales: " + ball.lives);
        }

    }


    // Power-up 2: Paddle más grande por un tiempo, no acumulable en tamaño
    IEnumerator EnlargePaddle()
    {
        isPaddleLarge = true;
        enlargeRemainingTime = powerUpDuration; // Iniciar el tiempo de mejora

        Vector3 originalScale = paddle.transform.localScale;

        if (isPaddleLarge)
        {
            paddle.transform.localScale = new Vector3(originalScale.x * 1.5f, originalScale.y, originalScale.z);

        }

        while (enlargeRemainingTime > 0)
        {
            enlargeRemainingTime -= Time.deltaTime;
            yield return null;
        }

        // Después de que el tiempo de la mejora se agote, volver al tamaño normal
        paddle.transform.localScale = originalScale;
        isPaddleLarge = false;
    }

    // Power-up 3: Aumentar la velocidad de la pelota
    void SpeedUpBall()
    {
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.velocity = rb.velocity * 1.5f; // Incrementar la velocidad de la pelota en un 50%
    }
}