using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lidar : MonoBehaviour {

	public float m_frequency = 30.0f;
	
    public float m_markerScale = 0.05f;
    public float m_markerDelay = 0.25f;
    // public string m_markerShape = "sphere";
    
    public float maxAngle = 10;
    public float minAngle = -10;
    public int numberOfLayers = 16;
    public int numberOfIncrements = 360;
    public float maxRange = 100f;

    float vertIncrement;
    float azimutIncrAngle;
    
 
    [HideInInspector]
    public float[] distances;
    public float[] azimuts;

    public GameObject pointObject;

	float timeElapsed = 0.0f;
	float interval;


	
    // Use this for initialization
    void Start () {
        distances = new float[numberOfLayers* numberOfIncrements];
        azimuts = new float[numberOfIncrements];
        vertIncrement = (float)(maxAngle - minAngle) / (float)(numberOfLayers - 1);
        azimutIncrAngle = (float)(360.0f / numberOfIncrements);
        interval = 1.0f/m_frequency;
    }

	void Update()
	{
		Vector3 fwd = new Vector3(0, 0, 1);
        Vector3 dir;
        RaycastHit hit;
        int indx = 0;
        float angle;

        //azimut angles
        for (int incr = 0; incr < numberOfIncrements; incr++)
        {
            for (int layer = 0; layer < numberOfLayers; layer++)
            {
                //print("incr "+ incr +" layer "+layer+"\n");
                //indx = layer + incr * numberOfLayers;
                angle = minAngle + (float)layer * vertIncrement;
                azimuts[incr] = incr * azimutIncrAngle;
                dir = transform.rotation * Quaternion.Euler(-angle, azimuts[incr], 0)*fwd;
                //print("idx "+ indx +" angle " + angle + "  azimut " + azimut + " quats " + Quaternion.Euler(-angle, azimut, 0) + " dir " + dir+ " fwd " + fwd+"\n");

		Debug.Log(Physics.Raycast(transform.position, dir * 100f, out hit, maxRange));
                if (Physics.Raycast(transform.position, dir, out hit, maxRange))
                {
                    Debug.DrawRay(transform.position, dir * hit.distance, Color.green);

                    var _obj = Instantiate(pointObject, dir * hit.distance + transform.position, transform.rotation);
                    Destroy(_obj, m_markerDelay);

                      
                    // // draw a sphere
                    // if(m_markerShape=="sphere")
                    // {
                    //     var mySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //     mySphere.transform.position = dir * hit.distance + transform.position;
                    //     mySphere.transform.localScale = new Vector3(m_markerScale, m_markerScale, m_markerScale);
                    //     Destroy(mySphere, m_markerDelay);
                    // }
                    //Debug.Log("hit distance: " + (float)hit.distance);
                    //distances[indx] = (float)hit.distance;

                }
                else
                {
                    //Debug.DrawRay(transform.position, dir * 100.0f, Color.green);
                    //distances[indx] = 100.0f;
                }
            }
        }

    }

	
	// Update is called once per frame
    /*
	void Update () {
	    timeElapsed += Time.deltaTime;
	    //Debug.Log($"Time Elapsed {timeElapsed}");
		if (timeElapsed >= interval)
		{
		    timeElapsed -= interval;
		    RenderOnTick();
		}
     }
     */
}
