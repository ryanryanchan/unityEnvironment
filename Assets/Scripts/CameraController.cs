using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject targetObject;

    // Update is called once per frame
    void Update()
    {
        float totalX = 0f, totalY = 0f, totalZ = 0f;
        Vector3 avgForward = Vector3.zero;

        foreach (Transform child in targetObject.transform)
        {
            totalX += child.transform.position.x;
            totalY += child.transform.position.y;
            totalZ += child.transform.position.z;
            avgForward += child.transform.forward;
        }
        Vector3 center = new Vector3(totalX, totalY, totalZ);
        center /= targetObject.transform.childCount;

        avgForward /= targetObject.transform.childCount;
        //transform.position = center;
        //transform.forward = avgForward;

        transform.LookAt(center);
    }
}
