using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{


    Player player;
    float hue;
    SpriteRenderer render;
    [SerializeField]
    GameObject colorChanger;

    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
        player = FindObjectOfType<Player>();
        if (player != null)
        {
            hue = player.hueValue;
            hue += 0.1f;
            if (hue >= 1)
            {
                hue = 0;
            }
            var color = Color.HSVToRGB(hue, 0.8f, 0.9f);
            render.color = color;
        }
        else Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(Instantiate(colorChanger, other.gameObject.transform.position, Quaternion.identity), 0.5f);
            AllObsColorChange();
        }
    }

    private void AllObsColorChange()
    {
        player.SetBackgroundColor();
    }
}
