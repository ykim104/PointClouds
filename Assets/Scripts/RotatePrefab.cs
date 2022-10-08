using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePrefab : MonoBehaviour
{
    public GameObject source;
    public int speed=100;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var objectRotation = transform.rotation;
        source.transform.RotateAround(source.transform.position, Vector3.up, speed * Time.deltaTime);
    }
}
