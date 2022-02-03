using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Terrain data calculator

public class TerrainHeightCalculator : MonoBehaviour
{
    public Terrain TerrainBase;

    // 공사전 원본 터레인 
    public Terrain TerrainMain_A;

    // 공사전 원본 터레인을 복사한 터레인 
    public Terrain TerrainMain_B;

    public float[,] heightsA;
    public float[,] heightsB;


    public float volumeOfEarthWork;
    public float volumeOfEarthWorkAbs;

    public Text getTheVolumeOfEarthWorkText;
    public Text getTheAbsoluteVolumeOfEarthWorkText;







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

        //Debug.Log(xRes + ", " + yRex);

        int xBase = 0;
        int yBase = 0;



        // GetHeights - gets the heightmap point of the terrain.
        // Store those values in a float array.
        heightsA = TerrainMain_A.terrainData.GetHeights(xBase, yBase, xRes, yRex);
        //Debug.Log("heightsA : " + heightsA);

        //// view data of the terrain
        //foreach (var i in heightsA)
        //{
        //    Debug.Log("Terrain_A heightmap data : " + i);
        //}
    }

    public void getHeightMapData_B()
    {
        // get the terrain heightmap width and height
        int xRes = TerrainMain_B.terrainData.heightmapResolution;
        int yRex = TerrainMain_B.terrainData.alphamapHeight;

        //Debug.Log(xRes + ", " + yRex);

        int xBase = 0;
        int yBase = 0;



        // GetHeights - gets the heightmap point of the terrain.
        // Store those values in a float array.
        heightsB = TerrainMain_B.terrainData.GetHeights(xBase, yBase, xRes, yRex);
        //Debug.Log("heightsB : " + heightsB);

        //// view data of the terrain
        //foreach (float i in heightsB)
        //{
        //    Debug.Log("Terrain_B heightmap data : " + i);
        //}
    }



    // ??? 해결해야 함. float[,] 값을 확인할 것. TerainMain_A의 값을 계산한 후 terrainMain_B에 복사하는 코드 완성
    // 복사한 코드를 가지고 일부 터레인을 변경한 후 최종 토공량을 계산하는 로직 개발 해야함!

    public void CopyTerrainData()
    {
        getHeightMapData_A();

        TerrainMain_B.terrainData.SetHeights(0, 0, heightsA);

    }



    public void ResetTerrain()
    {

        int TerrainHeightmapWidth = TerrainBase.terrainData.heightmapResolution;
        int TerrainHeightmapHeight = TerrainBase.terrainData.alphamapHeight;

        float[,] heightMap =  new float[TerrainHeightmapWidth, TerrainHeightmapHeight];

        for (int x = 0; x < TerrainHeightmapWidth; x++)
            for (int z = 0; z < TerrainHeightmapHeight; z++)
            {
                heightMap[x, z] = 0;
            }
        TerrainMain_B.terrainData.SetHeights(0, 0, heightMap);
    }


    public void GetTheVolumeOfTerrain()
    {
        getHeightMapData_A();
        getHeightMapData_B();

        float getTrA = 0;
        float getTrB = 0;

        Debug.Log("heightsA length : " + heightsA.Length);
        Debug.Log("heightsB length : " + heightsB.Length);
        Debug.Log("단위면적당 측정 지점수 /1M x 1M:" + heightsB.Length / 2500);
        Debug.Log("단위면적당 측정 지점  /1cm x 1cm:" + heightsB.Length / 250000);

        foreach (float i in heightsA)
        {
            getTrA += i;
            
        }

        foreach (float k in heightsB)
        {
            getTrB += k;
        }

        //Debug.Log("getTrA :" + getTrA);
        //Debug.Log("getTrB :" + getTrB);

        volumeOfEarthWork = getTrB - getTrA;
        volumeOfEarthWorkAbs = Math.Abs(getTrA - getTrB);
        Debug.Log("volumeOfEarthWork :" + volumeOfEarthWork / (heightsB.Length / 2500));
        //getTheVolumeOfEarthWorkText.text = "The volume of earthwork : " + Math.Ceiling((volumeOfEarthWork / (heightsB.Length / 250))).ToString() + "m³";



        getTheVolumeOfEarthWorkText.text = "The volume of earthwork : " + (volumeOfEarthWork / (heightsB.Length / 2500) * 100).ToString() + "㎥";
        getTheAbsoluteVolumeOfEarthWorkText.text = "The absolute volume of earthwork  : " + (volumeOfEarthWorkAbs / (heightsB.Length / 2500) * 100).ToString() + "㎥";


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
