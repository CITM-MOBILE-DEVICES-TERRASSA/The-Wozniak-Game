using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    public float maxX = 7.5f;
    bool automaticGameplay = false;
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
        if (ball != null && ball.isLaunched)
        {
            movementHorizontal = Input.GetAxis("Horizontal");

            if (!automaticGameplay)
            {
                if ((movementHorizontal > 0 && transform.position.x < maxX) || (movementHorizontal < 0 && transform.position.x > -maxX))
                {
                transform.position += Vector3.right * movementHorizontal * speed * Time.deltaTime;
                }

                Vector3 targetPosition = transform.position;

                if (Input.GetMouseButton(0))
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mousePosition.z = 0;

                    if (mousePosition.x < maxX && mousePosition.x > -maxX)
                    {
                        targetPosition.x = mousePosition.x;
                    }
                }

                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Moved)
                    {
                        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                        touchPosition.z = 0;

                        if (touchPosition.x < maxX && touchPosition.x > -maxX)
                        {
                            transform.position = new Vector3(touchPosition.x, transform.position.y, transform.position.z);
                        }
                    }
                }
            }
            else
            {
                AutomaticGameplay();
            }
        }
    }

    public void AutomaticGameplay()
    {
        transform.position = new Vector3(ball.transform.position.x,-3.8f, 0);
    }

    public void ToggleAutomaticGameplay()
    {
        automaticGameplay = !automaticGameplay;
    }

    public void ResetPosition()
    {
        transform.position = new Vector3(0, -3.8f, 0);
    }
}
