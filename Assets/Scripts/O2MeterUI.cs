using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class O2MeterUI : MonoBehaviour
{
    public Sprite[] Sprites;

    private PlayerController playerController;
    private Image image;

    private void Start()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        image = GetComponent<Image>();
    }

    private void Update()
    {
        float percent = Mathf.Clamp01(playerController.Oxygen / playerController.MaxOxygen);
        int frame = Mathf.FloorToInt(Sprites.Length * percent);
        if (frame >= Sprites.Length)
        {
            frame = Sprites.Length - 1;
        }

        if (frame < 0)
        {
            frame = 0;
        }

        image.sprite = Sprites[frame];
    }
}
