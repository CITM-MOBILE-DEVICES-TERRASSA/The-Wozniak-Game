using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BouncyBall : MonoBehaviour
{
    public float minY = -5.5f;
    public float maxVelocity = 15f;
    public float launchForce = 10f;

    public bool isLaunched = false;
    public bool isKinematic = true;

    Rigidbody2D rb;

    public bool constrainInsideScreen = false;
    public int score = 0;
    public int lives = 3;

    private int wallBounceCount = 0;
    public int maxWallBounces = 10;

    public TextMeshProUGUI scoreTxt;
    public GameObject[] livesImage;
    public GameObject gameOverPanel;
    public GameObject youWinPanel;
    public PlayerMovement paddle;
    public int maxScore = 0;
    public TextMeshProUGUI maxScoreTxt;
    public int baseScore = 10;
    public int additionalScore = 5;
    public int additionalScoreAccumulated = 0;
    public bool hasHitPaddleOrLost = true;


    private AudioManager audioManager;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        if (paddle == null)
        {
            paddle = GameObject.FindObjectOfType<PlayerMovement>();
        }

        maxScore = PlayerPrefs.GetInt("00000", 0);
        maxScoreTxt.text = maxScore.ToString("00000");

        audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        if (constrainInsideScreen)
        {
            Vector3 position = transform.position;
            position.x = Mathf.Clamp(position.x, -2.808f, 2.808f);
            position.y = Mathf.Clamp(position.y, -4.997f, 4.997f);
            transform.position = position;
        }

        if (!isLaunched && Input.GetKeyDown(KeyCode.Space))
        {
            LaunchBall();
        }

        if (!isLaunched && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                LaunchBall();
            }
        }

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

                if (lives >= 0)
                {
                    livesImage[lives].SetActive(false);
                }

                if (lives <= 0)
                {
                    GameOver();
                }
                else
                {
                    ResetBall();
                }
            }
        }

        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
        }

        if (GameObject.FindGameObjectsWithTag("Brick").Length == 0)
        {
            audioManager.PlaySound(AudioManager.SoundType.GameOver);
            ResetBall();
            youWinPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void LaunchBall()
    {
        rb.isKinematic = false;
        rb.AddForce(Vector2.up * launchForce, ForceMode2D.Impulse);
        isLaunched = true;
    }

    public void ResetBall()
    {
        transform.position = new Vector3(0, -3.4f, 0);
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        isLaunched = false;
        paddle.ResetPosition();
        additionalScoreAccumulated = 0;
        hasHitPaddleOrLost = true;
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
                score += baseScore;
                additionalScoreAccumulated = baseScore;
                puntosBloque = baseScore;
            }
            else
            {
                additionalScoreAccumulated += additionalScore;
                score += additionalScoreAccumulated;
                puntosBloque = additionalScoreAccumulated;
            }

            hasHitPaddleOrLost = false;
            scoreTxt.text = score.ToString("00000");
            blockComponent.ShowScoreText(puntosBloque);

            if (score > maxScore)
            {
                maxScore = score;
                maxScoreTxt.text = maxScore.ToString("00000");
                PlayerPrefs.SetInt("00000", maxScore);
                PlayerPrefs.Save();
            }
        }
        else if (collision.gameObject.CompareTag("Paddle"))
        {
            wallBounceCount = 0;
            hasHitPaddleOrLost = true;
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            wallBounceCount++;

            if (wallBounceCount >= maxWallBounces)
            {
                RedirectToPaddle();
            }
        }

        float speedIncreaseFactor = 1.05f;
        rb.velocity *= speedIncreaseFactor;

        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }

        if (Mathf.Abs(rb.velocity.y) < 0.5f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + 0.5f * Mathf.Sign(rb.velocity.y == 0 ? 1 : rb.velocity.y));
        }

        if (Mathf.Abs(rb.velocity.x) < 0.5f)
        {
            rb.velocity = new Vector2(rb.velocity.x + 0.5f * Mathf.Sign(rb.velocity.x == 0 ? 1 : rb.velocity.x), rb.velocity.y);
        }
    }

    public void RedirectToPaddle()
    {
        Vector2 directionToPaddle = (paddle.transform.position - transform.position).normalized;
        rb.velocity = directionToPaddle * rb.velocity.magnitude;
        wallBounceCount = 0;
    }

    void GameOver()
    {
        audioManager.PlaySound(AudioManager.SoundType.GameOver);
        ResetBall();
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }
}