using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class MoveToGoal_truck : MonoBehaviour
{
	// -- car --
	public WheelCollider[] wheels;
	public Transform[] wheels_mesh;
	public float wheel_torque = 2000;
	public float brake_torque = 500;
	public float max_steerangle = 30;

	private Vector3 wheel_position;
	private Quaternion wheel_rotation;

	[HideInInspector]
	public Rigidbody _rigidbody;

	private Vector3 savedPauseVelocity;
	private Vector3 savedPauseAngularVelocity;

	public bool isAICart;

	public float maxSpeed = 30;
    // -- car --

    // -- ai -- s
    public ConstManager circuit;
    private MoveToGoal_truck cart;
    public float steeringSensitivity = 0.01f;
    public float breakingSensitivity = 1.0f;
    public float accelerationSensitivity = 0.3f;

    public GameObject trackerPrefab;
    NavMeshAgent agent;

    int currentTrackerWP;
    float lookAhead = 10;

    float lastTimeMoving = 0;
    // -- ai -- e

    public float current_speed;

	//
	public float speed = 2.0f;
	public float accuracy = 0.1f;

	public Transform goal, goal_;
	


	// change target position from excavator to the tempRoad
	public bool switchTarget = true;



	private void Start()
	{
        switchTarget = true;


        _rigidbody = GetComponent<Rigidbody>();
		maxSpeed = 30;

        // -- ai -- s
        cart = GetComponent<MoveToGoal_truck>();
        GameObject tracker = Instantiate(trackerPrefab, cart.transform.position, cart.transform.rotation) as GameObject;
        agent = tracker.GetComponent<NavMeshAgent>();
        // -- ai -- e
    }

    // -- ai -- s
    void ProgressTracker()
    {
        if (Vector3.Distance(agent.transform.position, cart.transform.position) > lookAhead)
        {
            agent.isStopped = true;
            return;
        }
        else
        {
            agent.isStopped = false;
        }

        agent.SetDestination(circuit.waypoints[currentTrackerWP].position);


        if (Vector3.Distance(agent.transform.position, circuit.waypoints[currentTrackerWP].position) < 4)
        {
            currentTrackerWP++;
            if (currentTrackerWP >= circuit.waypoints.Count)
                currentTrackerWP = 0;
        }
    }

    void ResetLayer()
    {
        cart.gameObject.layer = 10;
        cart.gameObject.transform.Rotate(0, 180.0f, 0);
    }




    public void AccelerateCart(float v, float h, float b)
    {
        current_speed = Mathf.RoundToInt(_rigidbody.velocity.magnitude * 3.6f);

        if (v > 0)
        {
            if (current_speed < maxSpeed)
            {
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].motorTorque = Mathf.Clamp(v, -1f, 1f) * wheel_torque;
                }
            }
            else
            {
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].motorTorque = 0;
                }
            }
        }
        else if (v < 0)
        {
            if (current_speed > -5)
            {
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].motorTorque = Mathf.Clamp(v, -1f, 1f) * wheel_torque;
                }
            }
            else
            {
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].motorTorque = 0;
                }
            }
        }

        if (v == 0 && !isAICart)
        {
            b = 0.3f;
        }

        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].brakeTorque = Mathf.Clamp(b, 0f, 1f) * brake_torque;
        }

        wheels[0].steerAngle = Mathf.Clamp(h, -1f, 1f) * max_steerangle;
        wheels[1].steerAngle = Mathf.Clamp(h, -1f, 1f) * max_steerangle;
        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].GetWorldPose(out wheel_position, out wheel_rotation);
            wheels_mesh[i].position = wheel_position;
            wheels_mesh[i].rotation = wheel_rotation;
        }
    }

    public void CheckForSkid()
    {
        int numSkidding = 0;
        for (int i = 2; i < 4; i++)
        {
            WheelHit wheelHit;
            wheels[i].GetGroundHit(out wheelHit);
        }

        if (numSkidding == 0)
        {
            // stop skid
        }
    }

    public void OnPause(bool pause)
    {
        if (pause)
        {
            savedPauseVelocity = _rigidbody.velocity;
            savedPauseAngularVelocity = _rigidbody.angularVelocity;
            if (_rigidbody.isKinematic == false)
            {
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
        }
        else
        {
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = savedPauseVelocity;
            _rigidbody.angularVelocity = savedPauseAngularVelocity;
        }
    }

    private void OnCollisionEnter(Collision collision)
	{

		if (collision.transform.CompareTag("Excavator"))
		{
			Debug.Log("The truck hit the excavator");

			// Change truck posion
			this.transform.Rotate(Vector3.up, 180 * Time.deltaTime, Space.World);

			switchTarget = true;

		}

		if (collision.transform.CompareTag("TempRoad"))
		{
			Debug.Log("The truck hit the TempRoad");

			switchTarget = false;

		}
	}



	// Update is called once per frame
	void Update()
	{

        if (switchTarget == false)
        {
            Debug.Log("switchTarget to Excavator:" + switchTarget);
            ProgressTracker();

            Vector3 localTarget;
            float targetAngle;

            if (cart._rigidbody.velocity.magnitude > 1)
            {
                lastTimeMoving = Time.time;
            }

            if (Time.time > lastTimeMoving + 4)
            {
                // waypoints[1] : Excavator

                GameObject tg = FindNearestObjectByTag("Excavator");

                goal = tg.transform;


                cart.transform.position = goal.position + Vector3.up * 10;
                agent.transform.position = cart.transform.position;
                agent.transform.Rotate(0, 180, 0);

                //cart.gameObject.layer = 8;

                //Invoke("ResetLayer", 3);
            }

            localTarget = cart.transform.InverseTransformPoint(agent.transform.position);
            targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;


            float steer = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(cart.current_speed);
            float speedFactor = cart.current_speed / 30;
            float corner = Mathf.Clamp(Mathf.Abs(targetAngle), 0, 90);
            float cornerFactor = corner / 90f;

            float brake = 0;
            //if (corner > 10 && speedFactor > 0.1f)
            //    brake = Mathf.Lerp(0, 1 + speedFactor * breakingSensitivity, cornerFactor);

            float accel = 1f;
            if (corner > 20 && speedFactor > 0.1f && speedFactor > 0.2f)
                accel = Mathf.Lerp(0, 1 * accelerationSensitivity, 1 - cornerFactor);

            cart.AccelerateCart(accel, steer, brake);
        }

        if (switchTarget == true)
        {
            Debug.Log("switchTarget to TempRoad :" + switchTarget);
            ProgressTracker();

            Vector3 localTarget;
            float targetAngle;

            if (cart._rigidbody.velocity.magnitude > 1)
            {
                lastTimeMoving = Time.time;
            }

            if (Time.time > lastTimeMoving + 4)
            {
                // waypoints[0] : TempRoad

                GameObject tg = FindNearestObjectByTag("TempRoad");

                goal = tg.transform;

                cart.transform.position = goal.position + Vector3.up * 10;
                agent.transform.position = cart.transform.position;
                agent.transform.Rotate(0, 180, 0);
                //cart.gameObject.layer = 8;

                //Invoke("ResetLayer", 3);
            }

            localTarget = cart.transform.InverseTransformPoint(agent.transform.position);
            targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;


            float steer = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(cart.current_speed);
            float speedFactor = cart.current_speed / 30;
            float corner = Mathf.Clamp(Mathf.Abs(targetAngle), 0, 90);
            float cornerFactor = corner / 90f;

            float brake = 0;
            //if (corner > 10 && speedFactor > 0.1f)
            //    brake = Mathf.Lerp(0, 1 + speedFactor * breakingSensitivity, cornerFactor);

            float accel = 1f;
            if (corner > 20 && speedFactor > 0.1f && speedFactor > 0.2f)
                accel = Mathf.Lerp(0, 1 * accelerationSensitivity, 1 - cornerFactor);

            cart.AccelerateCart(accel, steer, brake);
        }
        

        //if (switchTarget == false)
        //      {
        //	Debug.Log("switchTarget :" + switchTarget);
        //	GameObject tg = FindNearestObjectByTag("Excavator");

        //	goal = tg.transform;

        //	Vector3 lookAtGoal = new Vector3(goal.position.x,
        //									goal.position.y,
        //									goal.position.z);
        //	this.transform.LookAt(lookAtGoal);
        //	Vector3 direction = lookAtGoal - this.transform.position;
        //	Debug.DrawRay(this.transform.position, direction, Color.red);
        //	if (Vector3.Distance(transform.position, direction) > accuracy)
        //		this.transform.Translate(0, 0, speed * Time.deltaTime);
        //	Debug.Log("go to the Excavator!");
        //}

        //if (switchTarget == true)
        //      {
        //	Debug.Log("switchTarget :" + switchTarget);
        //	GameObject tg_ = FindNearestObjectByTag("TempRoad");

        //	goal_ = tg_.transform;

        //	Vector3 lookAtGoal = new Vector3(goal_.position.x,
        //									goal_.position.y,
        //									goal_.position.z);
        //	this.transform.LookAt(lookAtGoal);
        //	Vector3 direction = lookAtGoal - this.transform.position;
        //	Debug.DrawRay(this.transform.position, direction, Color.red);
        //	if (Vector3.Distance(transform.position, direction) > accuracy)
        //		this.transform.Translate(0, 0, speed * Time.deltaTime);
        //	Debug.Log("go to the TempRoad!");
        //}


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
