using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public GameObject camera1;
    public GameObject camera2;
    public GameObject camera3;
    public GameObject camera4;

    public void EnableCamera1()
    {
        camera1.SetActive(true);
        camera2.SetActive(false);
        camera3.SetActive(false);
        camera4.SetActive(false);
    }
    public void EnableCamera2()
    {
        camera1.SetActive(false);
        camera2.SetActive(true);
        camera3.SetActive(false);
        camera4.SetActive(false);
    }
    public void EnableCamera3()
    {
        camera1.SetActive(false);
        camera2.SetActive(false);
        camera3.SetActive(true);
        camera4.SetActive(false);
    }
    public void EnableCamera4()
    {
        camera1.SetActive(false);
        camera2.SetActive(false);
        camera3.SetActive(false);
        camera4.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        EnableCamera1();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            EnableCamera1();
        }
        else if (Input.GetKeyDown("2"))
        {
            EnableCamera2();
        }
        else if (Input.GetKeyDown("3"))
        {
            EnableCamera3();
        }
        else if (Input.GetKeyDown("4"))
        {
            EnableCamera4();
        }
    }
}
