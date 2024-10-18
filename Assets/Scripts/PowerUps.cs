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
                if (!isPaddleLarge)
                {
                    StartCoroutine(EnlargePaddle());
                }
                else
                {
                    enlargeRemainingTime += powerUpDuration;
                }
                break;
            case PowerUpType.FasterBall:
                SpeedUpBall();
                break;
        }

        Destroy(gameObject);
    }
    void RegenerateLife()
    {
        if (ball.lives < 3)
        {
            ball.lives += 1;
            ball.livesImage[ball.lives-1].SetActive(true);
            Debug.Log("Vida regenerada! Vidas actuales: " + ball.lives);
        }



    }

    IEnumerator EnlargePaddle()
    {
        isPaddleLarge = true;
        enlargeRemainingTime = powerUpDuration;

        Vector3 originalScale = paddle.transform.localScale;

        if (isPaddleLarge)
        {
            paddle.transform.localScale = new Vector3(originalScale.x + 2, originalScale.y, originalScale.z);
        }

        while (enlargeRemainingTime > 0)
        {
            enlargeRemainingTime -= Time.deltaTime;
            yield return null;
        }

        paddle.transform.localScale = originalScale;
        isPaddleLarge = false;
    }

    void SpeedUpBall()
    {
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.velocity = rb.velocity * 0.5f;
    }
}