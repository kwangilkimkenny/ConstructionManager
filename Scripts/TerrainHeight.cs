using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainHeight : MonoBehaviour
{

    public Terrain TerrainMain;
    public Vector3 terrainPos;
    public Vector3 terrainPos_;
    public Vector3 terrainPos__;

    public GameObject areaOfWorkRaycast;

    public float earthVolume = 0f;
    public float earthVolume_ = 0f;

    public float raycastDis = 100f;

    void OnGUI()
    {
        if(GUI.Button (new Rect(30,30,200,30), "Change Terrain Height"))
        {
            // get the terrain heightmap width and height
            int xRes = TerrainMain.terrainData.heightmapResolution;
            int yRex = TerrainMain.terrainData.alphamapHeight;

            Debug.Log(xRes + ", " + yRex);

            int xBase = 0;
            int yBase = 0;



            // GetHeights - gets the heightmap point of the terrain.
            // Store those values in a float array.
            float[,] heights = TerrainMain.terrainData.GetHeights(xBase, yBase, xRes, yRex);


            // Manuplate the height data
            heights[10, 10] = 1f; // 0 ~ 1. 1 being the maximum possible height.


            // SetHeights to change the terrain height.
            TerrainMain.terrainData.SetHeights(0,0,heights);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {


    }

    public object PositionRaycast(GameObject obj_)
    {
        RaycastHit hit;

        Physics.Raycast(obj_.transform.position, Vector3.down, out hit, raycastDis);

        terrainPos = hit.point;

        return terrainPos;
    }

    // ??? ???? - Pre
    public object EarthVolume(GameObject obj_)
    {
        RaycastHit hit;

        Physics.Raycast(obj_.transform.position, Vector3.down, out hit, raycastDis);

        terrainPos_ = hit.point;

        earthVolume += Math.Abs(terrainPos_.y); // 1 * 1 * y
        //Debug.Log("Terrain Volume : " + earthVolume);


        return earthVolume;
    }

    // ??? ???? - Post
    public object Post_EarthVolume(GameObject obj_)
    {
        RaycastHit hit;

        Physics.Raycast(obj_.transform.position, Vector3.down, out hit, raycastDis);

        terrainPos__ = hit.point;

        earthVolume_ += Math.Abs(terrainPos__.y); // 1 * 1 * y
        //Debug.Log("Terrain Volume : " + earthVolume_);


        return earthVolume_;
    }

}
