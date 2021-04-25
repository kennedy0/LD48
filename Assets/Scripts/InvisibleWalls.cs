using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWalls : MonoBehaviour
{
    private Vector3 offset;
    private Transform cam;
    
    void Start()
    {
        cam = Camera.main.transform;
        offset = cam.position - transform.position;
    }

    void Update()
    {
        Vector3 newPos = transform.position;
        newPos.y = cam.position.y + offset.y;
        transform.position = newPos;
    }
}
