using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SampleMeshCollider : MonoBehaviour
{
    // private float time = 0;
    SkinnedMeshRenderer meshRenderer;
    MeshCollider collider;

    void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        collider = GetComponent<MeshCollider>();
    }


    void Update()
    {
        // time += Time.deltaTime;
        // if (time >= 0.1f)
        // {
        //     time = 0;
        //     UpdateCollider();
        // }

        UpdateCollider();
    }
    
    
    public void UpdateCollider() 
    {
        Mesh colliderMesh = new Mesh();
        meshRenderer.BakeMesh(colliderMesh);
        collider.sharedMesh = null;
        collider.sharedMesh = colliderMesh;
    }

}




