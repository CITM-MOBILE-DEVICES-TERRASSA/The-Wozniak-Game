using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class LevelGenerator : MonoBehaviour
{
    public Vector2Int size;
    public Vector2 offset;
    public GameObject brickPrefab;
    public Gradient gradient;
    private List<GameObject> activePowerUps = new List<GameObject>();
    public GameObject[] powerUpPrefabs;
    private PowerUp powerUp;
    public GameObject victoryPanel;
    private PlayerMovement paddle;
    public int blockHitsRequired = 1;

    private void Start()
    {
        paddle = GameObject.FindObjectOfType<PlayerMovement>();
        StartCoroutine(RestoreGameState());
        GenerateLevel();
    }

    private IEnumerator RestoreGameState()
    {
        yield return null;

        int vidasGuardadas = PlayerPrefs.GetInt("Vidas", 3);
        BouncyBall ball = FindObjectOfType<BouncyBall>();
        if (ball != null)
        {
            ball.lives = vidasGuardadas;
            for (int i = 0; i < ball.livesImage.Length; i++)
            {
                ball.livesImage[i].SetActive(i < ball.lives);
            }

            int scoreGuardado = PlayerPrefs.GetInt("ActualScore", 0);
            ball.score = scoreGuardado;
            ball.scoreTxt.text = ball.score.ToString("00000");

            int maxScoreGuardado = PlayerPrefs.GetInt("00000", 0);
            ball.maxScore = maxScoreGuardado;
            ball.maxScoreTxt.text = ball.maxScore.ToString("00000");
        }

        int golpesGuardados = PlayerPrefs.GetInt("BlockHits", 1);
        LevelGenerator levelGenerator = FindObjectOfType<LevelGenerator>();
        if (levelGenerator != null)
        {
            levelGenerator.blockHitsRequired = golpesGuardados;
            levelGenerator.GenerateLevel();
        }
    }

    public void GenerateLevel()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                GameObject newBrick = Instantiate(brickPrefab, transform);
                newBrick.transform.position = transform.position + new Vector3((float)((size.x - 1) * .5f - i) * offset.x, j * offset.y, 0);
                newBrick.GetComponent<SpriteRenderer>().color = gradient.Evaluate((float)j / (size.y - 1));

                BlockComponent blockComponent = newBrick.AddComponent<BlockComponent>();
                blockComponent.hitsToBreak = blockHitsRequired;

                float randomChance = Random.Range(0f, 1f);
                if (randomChance <= 0.2f && powerUpPrefabs.Length > 0)
                {
                    GameObject powerUp = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
                    newBrick.AddComponent<PowerUpComponent>().powerUpPrefab = powerUp;
                }
            }
        }

        paddle.transform.localScale = new Vector3(1.4f, 0.2f, 1f);
        PlayerPrefs.SetInt("BlockHits", blockHitsRequired);
        PlayerPrefs.Save();
    }

    public void LevelCompleted()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        blockHitsRequired++;
        ClearLevel();
        DestroyAllPowerUps();

        PlayerPrefs.SetInt("BlockHits", blockHitsRequired);
        PlayerPrefs.SetInt("Vidas", FindObjectOfType<BouncyBall>().lives);
        PlayerPrefs.SetInt("ActualScore", FindObjectOfType<BouncyBall>().score);
        PlayerPrefs.SetInt("00000", FindObjectOfType<BouncyBall>().maxScore);
        PlayerPrefs.Save();

        GenerateLevel();
    }

    private void ClearLevel()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
            DestroyAllPowerUps();
        }
    }

    public void Restart()
    {
        PowerUpComponent[] powerUpComponents = FindObjectsOfType<PowerUpComponent>();
        foreach (var component in powerUpComponents)
        {
            component.SetLevelResetting(true);
        }

        PlayerPrefs.DeleteKey("ActualScore");
        PlayerPrefs.DeleteKey("Vidas");
        PlayerPrefs.DeleteKey("BlockHits");
        PlayerPrefs.Save();
        DestroyAllPowerUps();

        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void DestroyAllPowerUps()
    {
        GameObject[] activePowerUps = GameObject.FindGameObjectsWithTag("PowerUp");
        foreach (GameObject powerUp in activePowerUps)
        {
            Destroy(powerUp);
        }
    }
}