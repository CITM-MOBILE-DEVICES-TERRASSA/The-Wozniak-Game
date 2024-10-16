using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    public Vector2Int size;
    public Vector2 offset;
    public GameObject brickPrefab;
    public Gradient gradient;
    private List<GameObject> activePowerUps = new List<GameObject>(); // Lista para almacenar los power-ups activos
    public GameObject[] powerUpPrefabs; // Prefabs de power-ups que puedes asignar en el inspector

    private void Awake()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                GameObject newBrick = Instantiate(brickPrefab, transform);
                newBrick.transform.position = transform.position + new Vector3((float)((size.x - 1) * .5f - i) * offset.x, j * offset.y, 0);
                newBrick.GetComponent<SpriteRenderer>().color = gradient.Evaluate((float)j / (size.y - 1));

                // Si el bloque tiene un power-up, guárdalo en la lista para destruirlo más tarde si es necesario
                PowerUpComponent powerUpComponent = newBrick.GetComponent<PowerUpComponent>();
                if (powerUpComponent != null && powerUpComponent.powerUpPrefab != null)
                {
                    activePowerUps.Add(powerUpComponent.powerUpPrefab);
                }

                // Probabilidad del 20% de añadir un power-up
                float randomChance = Random.Range(0f, 1f);
                if (randomChance <= 0.2f && powerUpPrefabs.Length > 0)
                {
                    // Seleccionar un power-up aleatorio
                    GameObject powerUp = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
                    newBrick.AddComponent<PowerUpComponent>().powerUpPrefab = powerUp;
                }
            }
        }
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public void Restart()
    {
        foreach (GameObject powerUp in activePowerUps)
        {
            if (powerUp != null)
            {
                Destroy(powerUp);
            }
        }

        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}