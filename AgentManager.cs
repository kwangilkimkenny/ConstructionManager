using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{

	GameObject[] agents;

	// Use this for initialization
	void Start()
	{
		agents = GameObject.FindGameObjectsWithTag("Excavator");
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
