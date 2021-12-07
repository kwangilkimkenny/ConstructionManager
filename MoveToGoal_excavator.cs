using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MoveToGoal_excavator : MonoBehaviour {

	public float speed = 2.0f;
	public float accuracy = 0.1f;

	public Transform goal;

	public Animator anim;

	//public MoveToGoal_truck checkCollision;
	public Truck_NavimeshAI truckAgent;

	public ConstManager amountOfRocks;

	public Component[] ldRocks;
	// 트럭에 바위를 상차할 수 있는 양으로 중장비가 해당 양을 채우면 작업을 더이상 진행하지 않아야 함(카운트 중지)
	public int AmountOfDigForLoadTruck  = 5;



	int Cnt = 0;

	[System.Obsolete]
    private void OnTriggerEnter(Collider other)
	{
		
		if (other.gameObject.CompareTag("Rocks")) 
        {
			//Debug.Log("hit Rocks");
			
			if (Cnt < AmountOfDigForLoadTruck) // 5
            {
				Destroy(other.gameObject, 1);
				ExcavatorGetRocks();
				Cnt++;
				
				//truckAgent.WaitUntilLoadingTruckAgent(other.gameObject);
			}
			else
			{
				Debug.Log("excavator wait");
				//anim.SetTrigger("StayWait");
				Destroy(other.gameObject, 1);
				ExcavatorGetRocks();
				Cnt++;
			}
		}


        if (other.gameObject.CompareTag("Truck"))
        {
            //Debug.Log("hit Truck");

            truckAgent.CheckLoadRocks(other.gameObject);

            //full loaded
            //if (Cnt == AmountOfDigForLoadTruck - 1 && truckAgent.checkLoadRock_ == false)
			if (truckAgent.checkLoadRock_ == false)
				{
                MoveRocksToTruck();
                truckAgent.LoadedRocks(other.gameObject);

                Cnt -= 5;

                //truckAgent.ResumeMovingTruckAgent(other.gameObject);

            }
            else
            {
                //truckAgent.WaitUntilLoadingTruckAgent(other.gameObject);
            }




        }
        Debug.Log("Cnt ==> : " + Cnt);
	}

	IEnumerator LoadRocks()
	{
		yield return new WaitForSeconds(3f);
	}


	// private void OnTriggerEnter(Collider other)
	// {
	//     if (other.gameObject.CompareTag("Rocks"))
	//     {
	//Destroy(other.gameObject);
	//amountOfRocks.CheckAmoutOfRocks();
	//     }
	// }

	private void MoveRocksToTruck()
	{
		//Debug.Log("Excavator activate Folk");
		anim.SetTrigger("AgentAnimation_Work");
		
		//Debug.Log("Loading the rocks to the truck");
	}

	// Activate Excavator Digging
	private void ExcavatorGetRocks()
	{
		//Debug.Log("Digging");
		StartCoroutine("LoadRocks");
		anim.SetTrigger("AgnetAnimatikon_Only_Dig");


	}

	public void WorkDone()
    {
		Debug.Log("End of work.");
    }


	// Use this for initialization
	void Start () {

	}
	
	//// Update is called once per frame
	//void LateUpdate () {
	//	GameObject tg = FindNearestObjectByTag("Rocks");

	//	goal = tg.transform;

	//	this.transform.LookAt(goal.position);
	//	Vector3 direction = goal.position - this.transform.position;
	//	Debug.DrawRay(this.transform.position, direction, Color.red);
	//	if (direction.magnitude > accuracy)
	//		this.transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);


	//}


	// Update is called once per frame
	void Update()
	{
		
		//Debug.Log("중장비가 바위를 추적하여 추출한다...");
		GameObject tg = FindNearestObjectByTag("Rocks");

		if (tg != null)
        {
			goal = tg.transform;

			Vector3 lookAtGoal = new Vector3(goal.position.x,
											this.transform.position.y,
											goal.position.z);
			this.transform.LookAt(lookAtGoal);
			if (Vector3.Distance(transform.position, lookAtGoal) > accuracy)
				this.transform.Translate(0, 0, speed * Time.deltaTime);
		}else
        {
			this.transform.Translate(0, 0, 0);
			Debug.Log("Complete!");
			truckAgent.StopAllTruck();
        }

	


	
	}


	private GameObject FindNearestObjectByTag(string tag)
	{
		
		// 탐색할 오브젝트 목록을 List 로 저장
		var objects = GameObject.FindGameObjectsWithTag(tag).ToList();

		// LINQ 메소드를 이용해 가장 가까운 게임오브젝트 추ㅊ
		var neareastObject = objects
			.OrderBy(obj =>
			{
				return Vector3.Distance(transform.position, obj.transform.position);
			})
		.FirstOrDefault();

		return neareastObject;
	}

}
