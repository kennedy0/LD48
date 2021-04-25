using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructableTiles : MonoBehaviour
{
    public int HP;
    public float InvincibilityTime;
    public float DestroyDistance;
    
    private Tilemap tilemap;
    private bool canTakeDamage;
    private Transform cam;
    
    void Start()
    {
        tilemap = GameObject.Find("Grid").transform.Find("Tilemap").GetComponent<Tilemap>();
        canTakeDamage = true;
        cam = Camera.main.transform;
    }

    private void Update()
    {
        if (transform.position.y - cam.position.y > DestroyDistance)
        {
            Destroy(gameObject);
        }

        if (HP <= 0)
        {
            Die();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Drill") && canTakeDamage)
        {
            StartCoroutine(Damage());
        }
    }

    private IEnumerator Damage()
    {
        canTakeDamage = false;
        HP -= 1;
        float t = InvincibilityTime;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        canTakeDamage = true;
    }

    private void Die()
    {
        Vector3Int tilePosition = tilemap.WorldToCell(transform.position);
        tilemap.SetTile(tilePosition, null);
        Destroy(gameObject);
    }
}
