using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    public GameObject cube;
    public Vector3 direction = new Vector3(0f, 0f, 0f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float dist = 10; 
        transform.position = new Vector3(
            Mathf.Sin(Mathf.Deg2Rad * Time.frameCount / 2) * dist,
            2,
            Mathf.Cos(Mathf.Deg2Rad * Time.frameCount / 2) * dist);
    }
}
