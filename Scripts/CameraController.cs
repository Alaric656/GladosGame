using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private Vector2 xBounds; //stores the left and right limits of camera movement
    private Vector2 yBounds;
    private Vector2 zBounds;

    public bool bPlayerMovement;

    private float fPanSpeed = 15f;

    void Start()
    {
        xBounds = new Vector2(-5, 5);//left , right
        yBounds = new Vector2(-5, 5);//bottom , top
        zBounds = new Vector2(-8, -12); //zoom in , zoom out

        bPlayerMovement = false;   
    }

    public void SetZoomBounds(Vector2 z)
    {
        zBounds = z;
    }

    public void SetCameraBounds(Vector2 x, Vector2 y)
    {
        xBounds = x;
        yBounds = y;
    }

    public void ResetPosition()
    {
        transform.position = new Vector3(0, 0, -10);
    }

    void Update()
    {
        if (bPlayerMovement)
        {
            Vector2 movementVector = GetKeys();
            Vector3 movement = new Vector3(0,0,0);
            if(transform.position.x + (movementVector.x * fPanSpeed * Time.deltaTime) >= xBounds.x && transform.position.x + (movementVector.x * fPanSpeed * Time.deltaTime) <= xBounds.y)
            {
                movement.x = movementVector.x * fPanSpeed * Time.deltaTime;
            }
            if (transform.position.y + (movementVector.y * fPanSpeed * Time.deltaTime) >= yBounds.x && transform.position.y + (movementVector.y * fPanSpeed * Time.deltaTime) <= yBounds.y)
            {
                movement.y = movementVector.y * fPanSpeed * Time.deltaTime;
            }
            
            transform.position += movement;

            //TODO: ZOOMING
        }
    }


    private Vector2 GetKeys()
    {
        int xMovement = 0;
        int yMovement = 0;
        if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow))
        {
            xMovement += -1;
        }
        if(Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow))
        {
            xMovement += 1;
        }
        if (Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow))
        {
            yMovement += 1;
        }
        if (Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow))
        {
            yMovement -= 1;
        }
        Vector2 output = new Vector2(xMovement, yMovement);
        return output;
    }
}
