using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    private void Awake()
    {
        Application.targetFrameRate = 40;
    }
    void Update()
    {
        transform.position = target.position + offset;
    }
}
