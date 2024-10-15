using UnityEngine;

public class PowerUpComponent : MonoBehaviour
{
    public GameObject powerUpPrefab; // El prefab del power-up

    private void OnDestroy()
    {
        // Cuando el bloque se destruye, genera el power-up
        if (powerUpPrefab != null)
        {
            Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
        }
    }
}
