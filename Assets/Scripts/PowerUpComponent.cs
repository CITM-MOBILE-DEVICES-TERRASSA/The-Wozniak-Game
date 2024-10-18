using UnityEngine;

public class PowerUpComponent : MonoBehaviour
{
    public GameObject powerUpPrefab;

    private bool isLevelResetting = false;

    private void OnDestroy()
    {
        if (isLevelResetting)
            return;

        Debug.Log(gameObject.name + " ha sido destruido.");

        if (powerUpPrefab != null && !isLevelResetting)
        {
            GameObject powerUp = Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
            powerUp.tag = "PowerUp";
        }
    }

    public void SetLevelResetting(bool resetting)
    {
        isLevelResetting = resetting;
    }
}
