using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstManager : MonoBehaviour
{
    // - circuit - s
    public List<Transform> waypoints = new List<Transform>();
    // - circuit - e

    public Text AmountOfRocks;

    public bool enableRockSpawn = true;

    public GameObject Rock;


    public GameObject[] spawnRocks;

    public GameObject spwanArea;
    BoxCollider spwanAreaCollider;

    public int rockSpawnAmount;
    public MoveToGoal_excavator checkAmtOfRocks;


    // - circuit - s
    private void OnDrawGizmosSelected()
    {
        if (waypoints.Count > 1)
        {
            Vector3 prev = waypoints[0].position;
            for (int i = 1; i < waypoints.Count; i++)
            {
                Vector3 next = waypoints[i].position;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
            Gizmos.DrawLine(prev, waypoints[0].position);
        }
    }
    // - circuit - e



    void SpawnRock()
    {
        if (enableRockSpawn)
        {
            
            for (int i = 0; i < rockSpawnAmount; i++)
            {
                Instantiate(Rock, Return_RandomPosition(), Quaternion.identity);
            }

        }

        //Debug.Log("Destroy spawnBox Collider!");
        Destroy(spwanAreaCollider.GetComponent<BoxCollider>());

    }

    public void CheckAmoutOfRocks()
    {
        if (spawnRocks != null)
        {
            spawnRocks = GameObject.FindGameObjectsWithTag("Rocks");
            // Debug.Log("spawnRocks:" + spawnRocks.Length);
            AmountOfRocks.text = spawnRocks.Length.ToString();
        }else
        {
            checkAmtOfRocks.WorkDone();
        }

    }

    Vector3 Return_RandomPosition()
    {
        Vector3 originPos = spwanArea.transform.position;
        float range_X = spwanAreaCollider.bounds.size.x;
        float range_Y = spwanAreaCollider.bounds.size.y;
        float range_Z = spwanAreaCollider.bounds.size.z;
        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        range_Z = Random.Range((range_Z / 2) * -1, range_Z / 2);
        Vector3 RandomPostion = new Vector3(range_X, range_Y, range_Z);

        Vector3 respawnPosition = originPos + RandomPostion;
        return respawnPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        spwanAreaCollider = spwanArea.GetComponent<BoxCollider>();
        SpawnRock();
    }

    private void Update()
    {
        CheckAmoutOfRocks();
    }

}
