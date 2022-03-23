using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class RaycastItemAligner : MonoBehaviour
{

    public float raycastDis = 100f;
    //public GameObject GioPrefab;
    public Vector3 itmPos;
    private Vector3 itmPos_;
    private Vector3 itmPos__;

    public float earthVolume = 0f;
    public float earthVolume_ = 0f;


    public object PositionRaycast(GameObject obj_)
    {


        RaycastHit hit;


        Physics.Raycast(obj_.transform.position, Vector3.down, out hit, raycastDis);

        itmPos = hit.point;

        return itmPos;

    }


    // 토공량 계산하기 - Pre
    public object EarthVolume(GameObject obj_)
    {
        RaycastHit hit;

        Physics.Raycast(obj_.transform.position, Vector3.down, out hit, raycastDis);

        itmPos_ = hit.point;

        earthVolume += Math.Abs(itmPos_.y); // 1 * 1 * y
        //Debug.Log("Terrain Volume : " + earthVolume);


        return earthVolume;
    }

    // 토공량 계산하기 - Post
    public object Post_EarthVolume(GameObject obj_)
    {
        RaycastHit hit;

        Physics.Raycast(obj_.transform.position, Vector3.down, out hit, raycastDis);

        itmPos__ = hit.point;

        earthVolume_ += Math.Abs(itmPos__.y); // 1 * 1 * y
        //Debug.Log("Terrain Volume : " + earthVolume_);


        return earthVolume_;
    }



}
