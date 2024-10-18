using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    public Vector2Int size;
    public Vector2 offset;
    public GameObject brickPrefab;
    public Gradient gradient;
    private List<GameObject> activePowerUps = new List<GameObject>();
    public GameObject[] powerUpPrefabs;

    public int blockHitsRequired = 1;

    private void Awake()
    {
        GenerateLevel();
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
                blockComponent.hitsToBreak = blockHitsRequired; // Asegúrate de que el valor se esté estableciendo correctamente
                Debug.Log($"Bloque creado en {i}, {j} con {blockHitsRequired} golpes requeridos."); // Mensaje de depuración

                // Generar un power-up aleatorio con un 20% de probabilidad
                float randomChance = Random.Range(0f, 1f);
                if (randomChance <= 0.2f && powerUpPrefabs.Length > 0)
                {
                    GameObject powerUp = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
                    newBrick.AddComponent<PowerUpComponent>().powerUpPrefab = powerUp;
                }
            }
        }
    }

    public void LevelCompleted()
    {

        blockHitsRequired++;
        ClearLevel();

        DestroyAllPowerUps();

        GenerateLevel();
    }

    private void ClearLevel()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject); // Eliminar todos los bloques del nivel actual
        }
    }

    public void Restart()
    {
        PowerUpComponent[] powerUpComponents = FindObjectsOfType<PowerUpComponent>();
        foreach (var component in powerUpComponents)
        {
            component.SetLevelResetting(true);
        }


        DestroyAllPowerUps();

        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void DestroyAllPowerUps()
    {
        GameObject[] activePowerUps = GameObject.FindGameObjectsWithTag("PowerUp");
        foreach (GameObject powerUp in activePowerUps)
        {
            Destroy(powerUp);
        }
        Debug.Log("Todos los Power-ups han sido destruidos.");  
    }
}

