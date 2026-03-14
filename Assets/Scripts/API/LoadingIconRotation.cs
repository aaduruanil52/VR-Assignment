using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingIconRotation : MonoBehaviour
{
    public float speed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,0,speed*Time.deltaTime);
    }
}
