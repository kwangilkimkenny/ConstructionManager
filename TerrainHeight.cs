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


    float distance = 10000f;
    private bool isMousePressed;
    private List<Vector3> pointsList;
    private Vector3 mousePos;



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
        isMousePressed = false;
        pointsList = new List<Vector3>();
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




    // 마우스로 테러인의 위치를 추출한다.
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMousePressed = true;
            pointsList.RemoveRange(0, pointsList.Count);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isMousePressed = false;
        }

        if (isMousePressed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distance))
            {
                if (!pointsList.Contains(hit.point))
                {
                    //Debug.Log("Terrain posiotion : " + hit.point);

                    //Vector3 getRange = new Vector3(RangeAttribute(-range.x, range.x),
                    //                               RectTransform(-range.y, range.y),
                    //                               RectTransform(-range.z, range.z));
                    pointsList.Add(hit.point);
                }
            }
        }
    }





    //Terrain terrain;
    //TerrainData terrainData;
    //TerrainPointData[] points;


    //float[] strength;
    //int size;

    //float GetStrength(int x, int y)
    //{
    //    x = Mathf.Clamp(x, 0, size - 1);
    //    y = Mathf.Clamp(y, 0, size - 1);

    //    return strength[y * size + x];
    //}

    //public void Initialize(Texture2D brushTexture, int size)
    //{
    //    if (terrain == null)
    //        terrain = GetComponent<Terrain>();

    //    terrainData = terrain.terrainData;

    //    points = new TerrainPointData[terrainData.alphamapHeight * terrainData.alphamapWidth];



    //    for (int y = 0; y < terrainData.alphamapHeight; y++)
    //    {
    //        for (int x = 0; x < terrainData.alphamapWidth; x++)
    //        {
    //            //Normalize Coordinates
    //            float x01 = (float)x / terrainData.alphamapWidth;
    //            float y01 = (float)y / terrainData.alphamapHeight;

    //            //Get Height at point
    //            float height = terrainData.GetHeight(Mathf.RoundToInt(y01 * terrainData.heightmapHeight), Mathf.RoundToInt(x01 * terrainData.heightmapWidth));

    //            //Get Normal at point
    //            //Vector3 normal = terrainData.GetInterpolatedNormal(y01, x01);

    //            //Get Steepness at point
    //            float steepness = terrainData.GetSteepness(y01, x01);

    //            points[y * terrainData.alphamapHeight + x] = new TerrainPointData(x, y, height, steepness);//, normal);


    //        }
    //    }

    //    this.size = size;
    //    //size = brushTexture.height;

    //    strength = new float[size * size];

    //    for (int i = 0; i < size; i++)
    //    {
    //        for (int j = 0; j < size; j++)
    //        {
    //            strength[i * size + j] = brushTexture.GetPixelBilinear((float)j / size, (float)i / size).a;
    //        }
    //    }
    //}

    //public void Paint(Vector2 mousePosition, int textureIndex = 1, float opacity = 1)
    //{
    //    int Size = 8 * size;
    //    int num = Mathf.FloorToInt(mousePosition.x * terrainData.alphamapWidth);
    //    int num2 = Mathf.FloorToInt(mousePosition.y * terrainData.alphamapHeight);
    //    int num3 = Mathf.RoundToInt((float)size) / 2;
    //    int num4 = Mathf.RoundToInt((float)size) % 2;
    //    int x = Mathf.Clamp(num - num3, 0, terrainData.alphamapWidth - 1);
    //    int y = Mathf.Clamp(num2 - num3, 0, terrainData.alphamapHeight - 1);
    //    int num7 = Mathf.Clamp((num + num3) + num4, 0, terrainData.alphamapWidth);
    //    int num8 = Mathf.Clamp((num2 + num3) + num4, 0, terrainData.alphamapHeight);
    //    int width = num7 - x;
    //    int height = num8 - y;

    //    float[,,] splatmapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

    //    for (int i = 0; i < height; i++)
    //    {
    //        for (int j = 0; j < width; j++)
    //        {
    //            int ix = (x + j) - ((num - num3) + num4);
    //            int iy = (y + i) - ((num2 - num3) + num4);
    //            splatmapData[y + i, x + j, textureIndex] = GetStrength(ix, iy) * opacity;
    //        }
    //    }

    //    terrainData.SetAlphamaps(0, 0, splatmapData);
    //}


}
