using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { ExtraBalls, LargerPaddle, FasterBall }
    public PowerUpType powerUpType;

    public float fallSpeed = 2f; // Velocidad de ca�da constante
    public float powerUpDuration = 10f; // Duraci�n de los power-ups temporales

    private PlayerMovement paddle;
    private BouncyBall ball;
    public LayerMask ballLayer; // Capa para las bolas

    private bool isPaddleLarge = false; // Verificar si el paddle ya est� agrandado
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
                SpawnExtraBalls();
                break;
            case PowerUpType.LargerPaddle:
                if (!isPaddleLarge) // Agrandar el paddle si no est� ya agrandado
                {
                    StartCoroutine(EnlargePaddle());
                }
                else
                {
                    // Si ya est� agrandado, acumular el tiempo
                    enlargeRemainingTime += powerUpDuration;
                }
                break;
            case PowerUpType.FasterBall:
                SpeedUpBall();
                break;
        }

        // Destruir el power-up despu�s de activarlo
        Destroy(gameObject);
    }

    void SpawnExtraBalls()
    {
        for (int i = 0; i < 2; i++)
        {
            // Calcular la posici�n de spawn ligeramente por encima del paddle
            Vector3 spawnPosition = paddle.transform.position + new Vector3(0, 0.5f, 0); // Ajusta la altura seg�n sea necesario

            // Instanciar la bola desde la nueva posici�n
            GameObject newBall = Instantiate(ball.gameObject, spawnPosition, Quaternion.identity);
            Rigidbody2D rb = newBall.GetComponent<Rigidbody2D>();
            newBall.GetComponent<BouncyBall>().isLaunched = true;
            rb.isKinematic = false;

            // Establecer una velocidad inicial hacia arriba
            rb.velocity = new Vector2(Random.Range(-2f, 2f), 5f); // Cambia el rango seg�n sea necesario

            // Asignar la capa de las bolas para evitar colisiones entre s�
            newBall.layer = ballLayer;

            // Asegurarse de que las bolas no salgan de la pantalla
            newBall.GetComponent<BouncyBall>().constrainInsideScreen = true;
        }
    }

    // Power-up 2: Paddle m�s grande por un tiempo, no acumulable en tama�o
    IEnumerator EnlargePaddle()
    {
        isPaddleLarge = true;
        enlargeRemainingTime = powerUpDuration; // Iniciar el tiempo de mejora

        Vector3 originalScale = paddle.transform.localScale;
        paddle.transform.localScale = new Vector3(originalScale.x * 1.5f, originalScale.y, originalScale.z);

        while (enlargeRemainingTime > 0)
        {
            enlargeRemainingTime -= Time.deltaTime;
            yield return null;
        }

        // Despu�s de que el tiempo de la mejora se agote, volver al tama�o normal
        paddle.transform.localScale = originalScale;
        isPaddleLarge = false;
    }

    // Power-up 3: Aumentar la velocidad de la pelota
    void SpeedUpBall()
    {
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.velocity = rb.velocity * 1.5f; // Incrementar la velocidad de la pelota en un 50%
    }
}