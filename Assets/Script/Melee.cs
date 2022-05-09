using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    public float Limit;

    // Start is called before the first frame update
    void Awake()
    {
        Limit = 1.3f;
        if (gameObject.name.Contains("(Clone)")) Destroy(gameObject, Limit);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
