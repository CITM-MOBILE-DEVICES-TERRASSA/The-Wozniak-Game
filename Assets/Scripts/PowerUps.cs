using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { ExtraBalls, LargerPaddle, FasterBall }
    public PowerUpType powerUpType;

    public float fallSpeed = 2f;
    public float powerUpDuration = 10f;

    private PlayerMovement paddle;
    private BouncyBall ball;
    public LayerMask ballLayer;

    private bool isPaddleLarge = false;
    private float enlargeRemainingTime = 0f;

    void Start()
    {
        paddle = GameObject.FindObjectOfType<PlayerMovement>();
        ball = GameObject.FindObjectOfType<BouncyBall>();
    }

    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        if (transform.position.y < -5.5f)
        {
            Destroy(gameObject);  // Elimina el power-up
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
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
                // Si el paddle ya est� alargado, solo sumamos tiempo, si no, comenzamos la corrutina
                if (!isPaddleLarge)
                {
                    StartCoroutine(EnlargePaddle());
                }
                else
                {
                    enlargeRemainingTime += powerUpDuration;  // Sumar tiempo si ya est� grande
                }
                break;
            case PowerUpType.FasterBall:
                SpeedUpBall();
                break;
        }

        Destroy(gameObject); // Destruimos el power-up cuando se activa
    }

    void RegenerateLife()
    {
        if (ball.lives < 3)
        {
            ball.lives += 1;
            ball.livesImage[ball.lives - 1].SetActive(true);
            Debug.Log("Vida regenerada! Vidas actuales: " + ball.lives);
        }
    }

    IEnumerator EnlargePaddle()
    {
        // Si ya est� alargado, simplemente sumamos tiempo y salimos
        if (isPaddleLarge)
        {
            enlargeRemainingTime += powerUpDuration;
            yield break;
        }

        // Establecemos el estado del paddle como alargado y guardamos el tiempo restante
        isPaddleLarge = true;
        enlargeRemainingTime = powerUpDuration;

        // Guardamos el tama�o original del paddle
        Vector3 originalScale = paddle.transform.localScale;

        // Alargamos el paddle
        paddle.transform.localScale = new Vector3(originalScale.x + 2, originalScale.y, originalScale.z);

        // Temporizador de duraci�n del power-up
        while (enlargeRemainingTime > 0)
        {
            enlargeRemainingTime -= Time.deltaTime;
            yield return null;
        }

        // Una vez el tiempo se agote, volvemos al tama�o original
        paddle.transform.localScale = originalScale;
        isPaddleLarge = false;
    }


    void SpeedUpBall()
    {
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.velocity = rb.velocity * 0.5f;
    }
}