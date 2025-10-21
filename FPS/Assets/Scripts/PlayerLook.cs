//This script handls player look logics

using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private float xRotation = 0f;

    public float xSensitivity = 30f;
    public float ySensitivity = 30f;
    
    public void ProcessLook(Vector2 input)
    {
        float mousex = input.x;
        float mousey = input.y;

        //calculate camera rotation for looking up and down
        xRotation -= (mousey * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        //Apply rotation to the cam
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //Rotate player to look left and right
        transform.Rotate(Vector3.up * (mousex * Time.deltaTime) * xSensitivity);
    }
}
