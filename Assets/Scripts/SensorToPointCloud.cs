using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorToPointCloud : MonoBehaviour
{
    [Header("Static Setups")]

    [SerializeField] protected  ComputeShader computeShader = default;

    [SerializeField] protected GameObject sourceSensor;
    protected Mesh sourceMesh;// = default;

    [SerializeField] Material material = default;

    [SerializeField] Mesh pointsMesh = default;

    [SerializeField] private bool setColorFromTexture = false;

    //[SerializeField] private 
    bool useNormalDirection = false;

    [SerializeField] Texture2D sourceMeshTexture = default;
    Texture2D positionMap;

    [Header("Dynamic Setups")]
    [SerializeField, Range(0.001f, 0.2f)] float pointSize = 0.02f; //size
    //[SerializeField] Color pointsColor = Color.white;
    [SerializeField, Range(0.0f, 1.0f)] float colorIntensity = 0.5f;
    //[SerializeField, Range(0.0f, 1.0f)] 
    float colorFromTextureLerp = 0;

    //[SerializeField] private 
    bool useAlpha = true;



    ComputeBuffer positionsBuffer;
    ComputeBuffer uvsBuffer;
    ComputeBuffer normalsBuffer;
    ComputeBuffer colorsBuffer;

    Bounds bounds;
    Vector3[] vertices;
    Vector2[] uvs;
    Vector3[] normals;
    Color[] colors;



    private void Start()
    {
        positionsBuffer = new ComputeBuffer(1,3*4);
        uvsBuffer = new ComputeBuffer(1,2*4);
        colorsBuffer = new  ComputeBuffer(1,4*4);
        normalsBuffer = new ComputeBuffer(1,3*4);


        sourceMesh = sourceSensor.GetComponent<LidarSensor>().m_Mesh;
        if(sourceMesh==null || sourceMesh.vertices.Length == 0)
        	return;

        InitializeFromMeshData();
        SetBound();
    }

    protected void InitializeFromMeshData()
    {
        GetPositionsDataFromMesh();
        SetStaticMaterialData();    
    }

    private void GetPositionsDataFromMesh()
    {
        vertices = sourceMesh.vertices;
        positionsBuffer.Release();
        positionsBuffer = new ComputeBuffer(vertices.Length, 3 * 4);
        positionsBuffer.SetData(vertices);

        computeShader.SetBuffer(0, "_Positions", positionsBuffer);
        material.SetBuffer("_Positions", positionsBuffer);
    }

    private void SetStaticMaterialData()
    {
        material.SetFloat("_UseNormals", 0);
        colorFromTextureLerp = 0;
        if (setColorFromTexture)
            SetUVAndTextureData();
        else
            colorFromTextureLerp=1;

        if (useNormalDirection)
            SetNormalsData();
    }

    private void SetUVAndTextureData()
    {
        uvs = sourceMesh.uv;
        uvsBuffer.Release();
        uvsBuffer = new ComputeBuffer(uvs.Length, 2 * 4);
        uvsBuffer.SetData(uvs);
        computeShader.SetBuffer(0, "_UVs", uvsBuffer);
        
        colors = sourceMesh.colors;
        colorsBuffer.Release();
        colorsBuffer = new ComputeBuffer(colors.Length, 4*4);
        colorsBuffer.SetData(colors);
        computeShader.SetBuffer(0, "_Colors", colorsBuffer);

        material.SetBuffer("_uvs", uvsBuffer);
        material.SetBuffer("_Colors", colorsBuffer);
        //material.SetTexture("_MainTex", sourceMeshTexture);
        //colorFromTextureLerp = 1;
    }


    private void SetNormalsData()
    {
        normals = sourceMesh.normals;
        

        normalsBuffer = new ComputeBuffer(normals.Length, 3 * 4);
        normalsBuffer.SetData(normals);
        computeShader.SetBuffer(0, "_Normals", normalsBuffer);
        material.SetBuffer("_Normals", normalsBuffer);
        material.SetFloat("_UseNormals", 1);
    }

    protected void DispachComputeShader()
    {
        int groups = Mathf.CeilToInt(vertices.Length / 64f);
        computeShader.Dispatch(0, groups, 1, 1);
    }

    protected void Update()
    {
        sourceMesh = sourceSensor.GetComponent<LidarSensor>().m_Mesh;
        if(sourceMesh==null || sourceMesh.vertices.Length == 0)
        	return;
        
        //InitializeFromMeshData();
        SetBound();
   
        GetPositionsDataFromMesh();
        SetStaticMaterialData();   
        
        SetMaterialDynamicData();
        DrawInstanceMeshes();
    
        GenerateTexture2D();
    }

    protected void DrawInstanceMeshes()
    {
        //Debug.Log($"[Draw] {positionsBuffer.count}");
        Graphics.DrawMeshInstancedProcedural(pointsMesh, 0, material, bounds, positionsBuffer.count);
    }

    protected virtual void SetMaterialDynamicData()
    {
        material.SetFloat("_Step", pointSize);
        material.SetFloat("_scale", this.transform.localScale.x);
        material.SetFloat("_intensity", colorIntensity);
        material.SetVector("_worldPos", this.transform.position);
        //material.SetVector("_color", new Vector4(pointsColor.r, pointsColor.g, pointsColor.b, 1));
        material.SetFloat("_ColorFromTextureLerp", colorFromTextureLerp);
        material.SetFloat("_UseAlpha", useAlpha?1:0);
        material.SetMatrix("_quaternion", Matrix4x4.TRS(new Vector3(0, 0, 0), transform.rotation, new Vector3(1, 1, 1)));
        //material.SetMatrix("_Transform", transform.localToWorldMatrix);
    }

    protected void SetBound()
    {
        bounds = new Bounds(Vector3.zero, Vector3.one * 200);
    }


    protected virtual void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
        if (setColorFromTexture)
        {
            uvsBuffer.Release();
            uvsBuffer = null;
            colorsBuffer.Release();
            colorsBuffer = null;
        }
        if (useNormalDirection)
        {
            normalsBuffer.Release();
            normalsBuffer = null;
        }
    }

    void GenerateTexture2D()
    {
        var _pointCount = sourceMesh.vertices.Length;
        
        if(true)//sourceSensor.GetComponent<LidarSensor>().collectBackground)
        {
            var textureWidth = sourceSensor.GetComponent<LidarSensor>().numberOfAzimuthIncrements;
            var textureHeight = sourceSensor.GetComponent<LidarSensor>().numberOfVerticleLayers;//numberOfVerticleLayers;
            
            sourceMeshTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
            sourceMeshTexture.filterMode = FilterMode.Point;
            sourceMeshTexture.SetPixels(sourceMesh.colors);
        }
        /*
        else
        {
            var textureWidth = Mathf.CeilToInt(Mathf.Sqrt(_pointCount));
            var textureHeight = textureWidth;
            
            sourceMeshTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
            sourceMeshTexture.filterMode = FilterMode.Point;

            var i1 = 0;
            var i2 = 0U;
            
            for(var y=0; y<textureHeight; y++)
            {
                for(var x=0; x<textureWidth; x++)
                {
                    //var i = i1;
                    var i = i1 < _pointCount ? i1 : (int)(i2 % _pointCount);
                    sourceMeshTexture.SetPixel(x, y, sourceMesh.colors[i]);
                    
                    i1 ++;
                    i2 += 132049U; // prime
                }
            }
        }
        */
        sourceMeshTexture.Apply(true,true);
    }
}

