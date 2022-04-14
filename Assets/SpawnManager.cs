using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] zombieePrefabs;
    public int number;
    public float spawnRadius;
    bool spawnOnStart = true;

    
    // Start is called before the first frame update
    void Start()
    {
        
        CreateAllZombiees();
    }

    private void CreateAllZombiees()
    {
        for (int i = 0; i < number; i++)
        {
            
            Vector3 randompoint = transform.position + Random.insideUnitSphere * spawnRadius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randompoint, out hit, 10f, NavMesh.AllAreas))
            {
                int k = Random.Range(0, zombieePrefabs.Length);
                Instantiate(zombieePrefabs[k], randompoint, Quaternion.identity);
                //Instantiate(zombieePrefabs[1], randompoint, Quaternion.identity);

            }
            else
            {
                i--;
            }



        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!spawnOnStart && gameObject.tag == "Player")
            CreateAllZombiees();
    }
}
