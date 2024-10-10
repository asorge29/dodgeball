using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector]
    public bool live;

    [HideInInspector]
    public int parent;
    
    void Start()
    {
        live = true;
    }


    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") && live)
        {
            live = false;
            parent = 0;
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (!live && player.ammo < player.maxAmmo)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (live && collision.gameObject.transform.GetInstanceID() != parent)
            {
                live = false;
                parent = 0;
            }
        }
    }
}
