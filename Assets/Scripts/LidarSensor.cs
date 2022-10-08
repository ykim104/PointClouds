using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidarSensor : MonoBehaviour
{
    // lidar
    public bool collectBackground = false;
    public float frequency = 30f;
    float timeElapsed = 0;
    float interval;

    public float vertMaxAngle = 10;
    public float vertMinAngle = -10;
    public float vertIncrement = 0.125f;
    
    public float fieldOfVewAngle = 60f;
    public float azimuthIncrement = 0.125f;
    public float maxRange = 100f;
    
    public bool colorByDistance = false;
    public bool colorByObject = false;
    public List<Color> colors;
    public List<Collider> colliders;

    [HideInInspector]
    public int numberOfVerticleLayers;
    [HideInInspector]
    public int numberOfAzimuthIncrements = 360;
    
 
    [HideInInspector]
    public float[] distances;
    
    [HideInInspector]
    public float[] azimuts;
    
	
   	// store pointclouds in a mesh
    [HideInInspector]
	public Mesh m_Mesh;
	List<Vector3> m_Vertices = new List<Vector3>();
	List<Vector2> m_UVs = new List<Vector2>();
	List<Color> m_Colors = new List<Color>();

	 
    // Start is called before the first frame update
    void Awake()
    {        
        interval = (float) (1/frequency);

        numberOfVerticleLayers = (int) ((vertMaxAngle - vertMinAngle) / vertIncrement);
        numberOfAzimuthIncrements =(int) (fieldOfVewAngle/azimuthIncrement);

        azimuts = new float[numberOfAzimuthIncrements];
        distances = new float[numberOfVerticleLayers* numberOfAzimuthIncrements];

        // create mesh
        m_Mesh = new Mesh();
        m_Mesh.vertices = m_Vertices.ToArray();
        m_Mesh.uv = m_UVs.ToArray();
        //m_Mesh.triangles = m_Triangles.ToArray();
    }

    // Update is called once per frame
    void Update()
    {

        timeElapsed += Time.deltaTime; //in seconds
        if(timeElapsed>=interval)
        {
            timeElapsed=0;
            Render();
        }
    }

    void Render()
    {
        Vector3 fwd = new Vector3(0, 0, 1);
        Vector3 dir;
        RaycastHit hit;
        int indx = 0;
        float angle;

        //azimut angles
        m_Vertices.Clear();
        m_UVs.Clear();
        m_Colors.Clear();

        for (int layer = 0; layer < numberOfVerticleLayers; layer++)
        {
            for (int incr = 0; incr < numberOfAzimuthIncrements; incr++)
            {
                angle = vertMinAngle + (float)layer * vertIncrement;
                azimuts[incr] = incr * azimuthIncrement;
                dir = transform.rotation * Quaternion.Euler(-angle, azimuts[incr], 0) * fwd;
                
                //Debug.DrawRay(transform.position, dir*maxRange, Color.green);
                if (Physics.Raycast(transform.position, dir, out hit, maxRange))
                {
                    //Debug.DrawRay(transform.position, dir * hit.distance, Color.green);
                    var hitPosition = dir*hit.distance + transform.position;
                    m_Vertices.Add(hitPosition);
                    m_UVs.Add(new Vector2(hitPosition.x, hitPosition.z));

                    
                    if(colorByDistance && !colorByObject)
                    {
                        if(colors.Count < 2)
                            Debug.LogError("Add up to two colors to the list.");
                        m_Colors.Add(Color.Lerp(colors[0], colors[1], hit.distance/maxRange));
                    }
                    else if(colorByObject && !colorByDistance)
                    {
                        var hitName = hit.collider.gameObject.name;

                        int ind = colliders.IndexOf(hit.collider); 
                        m_Colors.Add(colors[ind]);
                    }                
                    else if(!colorByDistance && !colorByObject)
                        m_Colors.Add(Color.white);  
                    else if(colorByDistance && colorByObject)
                        Debug.LogError("Can only have one option checked.");
                }
                else
                {
                    var hitPosition = new Vector3(Mathf.Infinity,Mathf.Infinity,Mathf.Infinity);
                    if(collectBackground)
                        hitPosition = dir*maxRange + transform.position;
                    
                    m_Vertices.Add(hitPosition);
                    m_UVs.Add(new Vector2(hitPosition.x, hitPosition.z));

                    Color transparentColor = Color.white;
                    transparentColor.a = 1f;
                    m_Colors.Add(transparentColor);
                }
            }
        }

        m_Mesh.vertices = m_Vertices.ToArray();
        m_Mesh.uv = m_UVs.ToArray();
        m_Mesh.colors = m_Colors.ToArray();
        //m_Mesh.RecalculateNormals();
        //m_Mesh.RecalculateBounds();
        //m_Mesh.RecalculateTangents();
    }
}
