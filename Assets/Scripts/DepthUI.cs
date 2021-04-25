using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DepthUI : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    private Transform cam;

    private void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        cam = Camera.main.GetComponent<Transform>();
    }

    private void Update()
    {
        int depth = Mathf.FloorToInt(Mathf.Abs(cam.transform.position.y) * 5);
        tmp.text = $"{depth} m";
    }
}
