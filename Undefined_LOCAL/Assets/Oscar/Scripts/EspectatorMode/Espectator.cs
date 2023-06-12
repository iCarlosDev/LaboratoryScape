using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Espectator : MonoBehaviour
{
    public static Espectator instance;
    public float speed;
    public float sensibility;
    public float cSpeed;
    public float cSensibility;
    float max = 90;
    float min = -90;
    float roty = 30;
    float rotx = -30;
    public GameObject cameraObject;
    Vector2 inputMov;
    public mouseMode currentMouseMode = mouseMode.Cinematic;
    public enum mouseMode
    {
        Normal, Cinematic
    };

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            if(currentMouseMode == mouseMode.Cinematic)
            {
                currentMouseMode= mouseMode.Normal;
            }
            else
            {
                currentMouseMode = mouseMode.Cinematic;
            }
        }
       

        if (Input.GetKey(KeyCode.Minus))
        {
            if (speed > 2.5f)
            {
                speed -= 2.5f;
            }
        }
        else if (Input.GetKey(KeyCode.Plus))
        {
            if (speed < 70)
            {
                speed += 5;
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(Vector3.up * -speed * Time.deltaTime);
        }

        inputMov.x = Input.GetAxisRaw("Horizontal");
        inputMov.y = Input.GetAxisRaw("Vertical");

        transform.Translate(cameraObject.transform.forward * speed * inputMov.y * Time.deltaTime);
        transform.Translate(cameraObject.transform.right * speed * inputMov.x * Time.deltaTime);


        if (currentMouseMode ==  mouseMode.Normal)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y");

            cameraObject.transform.localEulerAngles = new Vector3(cameraObject.transform.localEulerAngles.x + mouseY * sensibility * Time.deltaTime, cameraObject.transform.localEulerAngles.y + mouseX * sensibility * Time.deltaTime, 0);
        }
        else
        {

            rotx += Input.GetAxis("Mouse Y") * cSensibility;
            roty += Input.GetAxis("Mouse X") * cSensibility;

            rotx = Mathf.Clamp(rotx, min, max);

            Quaternion target = Quaternion.Euler(-rotx,roty , 0);
            cameraObject.transform.rotation = Quaternion.Slerp(cameraObject.transform.rotation, target, Time.deltaTime * cSpeed);
        }
    }
}
