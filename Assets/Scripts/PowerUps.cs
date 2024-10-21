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
            ball.livesImage[ball.lives - 1].SetActive(true);
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