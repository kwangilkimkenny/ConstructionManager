using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class Truck_NavimeshAI : MonoBehaviour
{
    public Transform goal, goal_;
    NavMeshAgent agent;
    public Rigidbody rb;

    public Truck_NavimeshAI truckAgnet;

    // change target position from excavator to the tempRoad
	public bool switchTarget = true;

 
    public Rigidbody _rigidbody;
    float lastTimeMoving = 0;

    

    // Start is called before the first frame update
    void Start()
    {
        switchTarget = true;
        agent = GetComponent<NavMeshAgent>();
    }



    private void OnTriggerEnter(Collider other) 
	{

		if (other.gameObject.CompareTag ("Excavator"))

        {
			//Debug.Log("The truck hit the excavator");

            // Change truck posion
            rb.velocity = Vector3.zero;
            //Debug.Log("When truck hit the excavator, just stop!");

            //StartCoroutine(WaitForIt());

            //IEnumerator WaitForIt()
            //{
            //    yield return new WaitForSeconds(2.0f);

            //}

            switchTarget = true;

        }

		if (other.gameObject.CompareTag ("TempRoad"))
		{
            //Debug.Log("The truck hit the TempRoad");

            UnloadedRocks();

            switchTarget = false;

		}
	}

    //public void CheckAgentMovement()
    //{

    //    if(agent.velocity.magnitude > 1)
    //    {
    //        lastTimeMoving = Time.time;
    //    }

    //    if(Time.time > lastTimeMoving + 4)
    //    {
    //        agent.SetDestination(goal.transform.position);
    //        Debug.Log("Check agnet movement");
    //    }
    //}
    public bool checkLoadRock_;
    public void CheckLoadRocks(GameObject otherobj)
    {
        Transform[] allChildren = otherobj.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            // 차량에 바위가 실려있다면,
            if (child.name == "LoadedRocks")
            {
                checkLoadRock_ = true;
            }else
            {
                checkLoadRock_ = false;
            }

        }

        //checkLoadRock_ = otherobj.transform.Find("TruckMeshes").transform.FindChild("LoadedRocks").gameObject;
        Debug.Log("checkLoadRock :" + checkLoadRock_);

    }


    [System.Obsolete]
    public void LoadedRocks(GameObject otherobj)
    {

        Debug.Log("otherobj.transform :" + otherobj.transform);
        otherobj.transform.Find("TruckMeshes").transform.FindChild("LoadedRocks").gameObject.SetActive(true);

    }

    public void UnloadedRocks()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {

            if (child.name == "LoadedRocks")
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    // 작업이 끝나면 멈춤
    public void StopAllTruck()
    {
        var Trucks = GameObject.FindGameObjectsWithTag("truck");
        foreach (GameObject trItm in Trucks)
        {
            agent = trItm.GetComponent<NavMeshAgent>();
            agent.GetComponent<NavMeshAgent>().isStopped = true;
        }

    }

    // 상차하는 동안 ai agent는 멈추어있어야 함. 
    public void WaitUntilLoadingTruckAgent(GameObject otherobj)
    {
        agent = otherobj.GetComponent<NavMeshAgent>();
        agent.GetComponent<NavMeshAgent>().isStopped = true;
    }

    // 에이전트 다시 작동 
    public void ResumeMovingTruckAgent(GameObject otherobj)
    {
        agent = otherobj.GetComponent<NavMeshAgent>();
        agent.GetComponent<NavMeshAgent>().isStopped = false;
    }


    void Update()
	{
    if (switchTarget == false)
        {
        //Debug.Log("switchTarget :" + switchTarget);
        GameObject tg = FindNearestObjectByTag("Excavator");

        goal = tg.transform;


        agent = this.GetComponent<NavMeshAgent>();
        agent.SetDestination(goal.transform.position);


        //Debug.Log("go to the Excavator!");

        //CheckAgentMovement();

        }

    if (switchTarget == true)
        {
        //Debug.Log("switchTarget :" + switchTarget);
        GameObject tg_ = FindNearestObjectByTag("TempRoad");

        goal_ = tg_.transform;

        agent = this.GetComponent<NavMeshAgent>();
        agent.SetDestination(goal_.transform.position);

        //Debug.Log("go to the TempRoad!");

        //CheckAgentMovement();

        }
    }

    private GameObject FindNearestObjectByTag(string tag)
	{
		// 탐색할 오브젝트 목록을 List 로 저장
		var objects = GameObject.FindGameObjectsWithTag(tag).ToList();

		// LINQ 메소드를 이용해 가장 가까운 게임오브젝트 extract
		var neareastObject = objects
			.OrderBy(obj =>
			{
				return Vector3.Distance(transform.position, obj.transform.position);
			})
		.FirstOrDefault();

		return neareastObject;
	}

}
