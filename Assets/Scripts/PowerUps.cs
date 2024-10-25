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

    public bool isPaddleLarge = false;

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

                LargerPaddle();
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

    void LargerPaddle()
    {
        if (!isPaddleLarge)
        {
            isPaddleLarge = true;
            Debug.Log("paddle== "+ isPaddleLarge);
            Vector3 originalScale = paddle.transform.localScale;

            paddle.transform.localScale = new Vector3(originalScale.x + 1, originalScale.y, originalScale.z);
        }
    }


    void SpeedUpBall()
    {
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.velocity = rb.velocity * 0.5f;
    }
}