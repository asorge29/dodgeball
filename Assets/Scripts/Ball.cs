using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector]
    public bool live;
    
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
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            if (!live)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (live)
            {
                live = false;
            }
        }
    }
}
