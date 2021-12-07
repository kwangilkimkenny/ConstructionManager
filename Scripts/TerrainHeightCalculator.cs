using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Terrain data calculator

public class TerrainHeightCalculator : MonoBehaviour
{

    public Terrain TerrainMain_A;
    public Terrain TerrainMain_B;

    public Vector3 terrainPos;
    public Vector3 terrainPos_;
    public Vector3 terrainPos__;

    public GameObject areaOfWorkRaycast;

    public float earthVolume = 0f;
    public float earthVolume_ = 0f;

    public float raycastDis = 100f;


    // Start is called before the first frame update
    void Start()
    {
        getHeightMapData_A();
        getHeightMapData_B();
    }

    public void getHeightMapData_A()
    {
        // get the terrain heightmap width and height
        int xRes = TerrainMain_A.terrainData.heightmapResolution;
        int yRex = TerrainMain_A.terrainData.alphamapHeight;

        Debug.Log(xRes + ", " + yRex);

        int xBase = 0;
        int yBase = 0;



        // GetHeights - gets the heightmap point of the terrain.
        // Store those values in a float array.
        float[,] heightsA = TerrainMain_A.terrainData.GetHeights(xBase, yBase, xRes, yRex);
        Debug.Log("heights : " + heightsA);

        //// view data of the terrain
        //foreach (float i in heightsA)
        //{
        //    Debug.Log("Terrain_A heightmap data : " + i);
        //}
    }

    public void getHeightMapData_B()
    {
        // get the terrain heightmap width and height
        int xRes = TerrainMain_B.terrainData.heightmapResolution;
        int yRex = TerrainMain_B.terrainData.alphamapHeight;

        Debug.Log(xRes + ", " + yRex);

        int xBase = 0;
        int yBase = 0;



        // GetHeights - gets the heightmap point of the terrain.
        // Store those values in a float array.
        float[,] heightsB = TerrainMain_B.terrainData.GetHeights(xBase, yBase, xRes, yRex);
        Debug.Log("heights : " + heightsB);

        //// view data of the terrain
        //foreach (float i in heightsB)
        //{
        //    Debug.Log("Terrain_B heightmap data : " + i);
        //}
    }





    // ??? 해결해야 함. float[,] 값을 확인할 것. TerainMain_A의 값을 계산한 후 terrainMain_B에 복사하는 코드 완성
    // 복사한 코드를 가지고 일부 터레인을 변경한 후 최종 토공량을 계산하는 로직 개발 해야함!

    //public void CopyTerrainData()
    //{
    //    float[,] heightsA = getHeightMapData_A();
    //    foreach (float i in heightsA)
    //    {
    //        TerrainMain_B.terrainData.SetHeights(0, 0, i);
    //    }
        


    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
