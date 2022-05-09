using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Limit;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Platform") Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Awake()
    {
        Limit = 5.0f;
        if (gameObject.name.Contains("(Clone)")) Destroy(gameObject, Limit);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
