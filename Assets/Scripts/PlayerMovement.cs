using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    public float maxX = 7.5f;

    private float movementHorizontal;

    // Referencia al script de la pelota
    public BouncyBall ball;

    void Start()
    {
        // Si la pelota no está asignada en el Inspector, intenta encontrarla automáticamente
        if (ball == null)
        {
            ball = GameObject.FindObjectOfType<BouncyBall>();
        }
    }

    void Update()
    {
        // Solo permite mover el paddle si la pelota ha sido lanzada
        if (ball != null && ball.isLaunched)
        {
            movementHorizontal = Input.GetAxis("Horizontal");

            // Restringir el movimiento del paddle dentro de los límites
            if ((movementHorizontal > 0 && transform.position.x < maxX) || (movementHorizontal < 0 && transform.position.x > -maxX))
            {
                transform.position += Vector3.right * movementHorizontal * speed * Time.deltaTime;
            }
        }
    }

    // Método para resetear la posición del paddle
    public void ResetPosition()
    {
        transform.position = new Vector3(0, -3.8f, 0); // Coloca el paddle en la posición inicial
    }
}
