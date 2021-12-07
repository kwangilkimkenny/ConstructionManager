using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExcavatorController : MonoBehaviour
{

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            anim.SetInteger("AgentAnimation_Work", 1);
            Debug.Log("W key was pressed : AgentAnimation_Work");
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            anim.SetInteger("AgentAnimation_Work", 0);
            Debug.Log("W key was pressed : AgentAnimation_Work");
        }


    }
}
