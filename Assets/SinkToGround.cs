using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkToGround : MonoBehaviour
{
    float destroyHeight;
    public float delayTime;
    // Start is called before the first frame update
    void Start()
    {
        delayTime = 5f;
        if(this.gameObject.tag == "RagDollZombiee")
        {
            // ReadyToSink();
            Invoke("ReadyToSink", delayTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //RangDoll goes into Burried.
    public void SinkIntoGround()
    {
        this.transform.Translate(0f, -0.001f, 0f);
        if (transform.position.y < destroyHeight)
        {
            Destroy(gameObject);
        }
       
    }
    public void ReadyToSink()
    {
        destroyHeight = Terrain.activeTerrain.SampleHeight(this.transform.position);
        Collider[] colliderList = this.transform.GetComponentsInChildren<Collider>();
        foreach (Collider item in colliderList)
        {
            Destroy(item);   
        }
        InvokeRepeating("SinkIntoGround", 3f, 0.1f);
    }
}
