using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstManager : MonoBehaviour
{
    // - circuit - s
    public List<Transform> waypoints = new List<Transform>();
    // - circuit - e

    public Text AmountOfRocks;

    public bool enableRockSpawn = true;

    public GameObject Rock;


    public GameObject[] spawnRocks;

    public GameObject spwanArea;
    BoxCollider spwanAreaCollider;

    public int rockSpawnAmount;
    public MoveToGoal_excavator checkAmtOfRocks;


    public ChangeTerrainHeight_lineMarker getTrainDataFromWorkArea;


    // 원본 터레인으로 Terrain Main_A로 작업하는 원본 Terrain
    public Terrain TerrainMain_A;



    // 터레인 생성을 위한 작업위치 추출
    public LineRenderer line_A;

    public float[,] shapeHeights_;
    public int left_;
    public int bottom_;


    [System.Obsolete]
    public void CalEvalTerrain(int amtOfRocks)
    {
        //Get the terrain heightmap width and height.
        int xRes = TerrainMain_A.terrainData.heightmapWidth;
        int yRes = TerrainMain_A.terrainData.heightmapHeight;

        //GetHeights - gets the heightmap points of the tarrain. Store them in array
        float[,] heights = TerrainMain_A.terrainData.GetHeights(0, 0, xRes, yRes);
        /* Set the positions to array "positions" */
        Vector3[] positions = new Vector3[line_A.positionCount];
        line_A.GetPositions(positions);


        float ftop = float.NegativeInfinity;
        float fright = float.NegativeInfinity;
        float fbottom = Mathf.Infinity;
        float fleft = Mathf.Infinity;
        for (int i = 0; i < line_A.positionCount; i++)
        {
            //find the outmost points
            if (ftop < positions[i].z)
            {
                ftop = positions[i].z;
            }
            if (fright < positions[i].x)
            {
                fright = positions[i].x;
            }
            if (fbottom > positions[i].z)
            {
                fbottom = positions[i].z;
            }
            if (fleft > positions[i].x)
            {
                fleft = positions[i].x;
            }
        }

        int top = Mathf.RoundToInt(ftop);
        int right = Mathf.RoundToInt(fright);
        int bottom = Mathf.RoundToInt(fbottom);
        int left = Mathf.RoundToInt(fleft);

        int terrainXmax = right - left; // the rightmost edge of the terrain
        int terrainZmax = top - bottom; // the topmost edge of the terrain

        shapeHeights_ = TerrainMain_A.terrainData.GetHeights(left, bottom, terrainXmax, terrainZmax);

        Vector2 point; //Create a point Vector2 point to match the shape

        /* Loop through all points in the rectangle surrounding the shape */

        float nNHeight = 0f;
        for (int i = 0; i < terrainZmax; i++)
        {
            point.y = i + bottom; //Add off set to the element so it matches the position of the line
            for (int j = 0; j < terrainXmax; j++)
            {
                point.x = j + left; //Add off set to the element so it matches the position of the line
                if (InsidePolygon(point, bottom))
                {
                    nNHeight = getTrainDataFromWorkArea.SetHeight / (getTrainDataFromWorkArea.SetHeight  + amtOfRocks);
                    shapeHeights_[i, j] = nNHeight; // set the height value to the terrain vertex
                }
            }
        }

        left_ = left;
        bottom_ = bottom;

        ////SetHeights to change the terrain height.
        //TerrainMain_A.terrainData.SetHeightsDelayLOD(left, bottom, shapeHeights_);
        //TerrainMain_A.ApplyDelayedHeightmapModification();

    }


    // - circuit - s
    private void OnDrawGizmosSelected()
    {
        if (waypoints.Count > 1)
        {
            Vector3 prev = waypoints[0].position;
            for (int i = 1; i < waypoints.Count; i++)
            {
                Vector3 next = waypoints[i].position;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
            Gizmos.DrawLine(prev, waypoints[0].position);
        }
    }
    // - circuit - e


    // 한번에 콜라이더의 위치에 바위 생성. 이 코드는 이제 사용하지 않음. //
    void SpawnRock()
    {
        if (enableRockSpawn)
        {
            
            for (int i = 0; i < rockSpawnAmount; i++)
            {
                Instantiate(Rock, Return_RandomPosition(), Quaternion.identity);
            }

        }

        //Debug.Log("Destroy spawnBox Collider!");
        Destroy(spwanAreaCollider.GetComponent<BoxCollider>());

    }

    [System.Obsolete]
    void SpawnRockInWorkArea()
    {
        if (enableRockSpawn)
        {
            // 생성할 바위를 반복문을 돌면셔 총 개수만큼 생성된다. 생성 방법을 변경해야 자연스럽게 토양의 위치가 변경되면서 작업 진행상황을 모니터링할 수 있어야 함.
            // 바위가 생성되는 지점과 생성 방식을 한번에 하나씩 생성되고, 굴삭기가 한번에 하나씩 작업할 수 있도록 구현한다.

            // 지형셋업 
            //shapeHeights_ = TerrainMain_A.terrainData.GetHeights(
            //        (getTrainDataFromWorkArea.leftBottom.Item1),
            //        (getTrainDataFromWorkArea.leftBottom.Item2),
            //         getTrainDataFromWorkArea.TerrainXMax_,
            //         getTrainDataFromWorkArea.TerrainZMax_);

            float[,] newSetHeight;

            for (int k = rockSpawnAmount; k >= 1; k--)
            {
                //StartCoroutine(TerrainDelay());

                //Debug.Log(" Start Instantiate");

                Instantiate(Rock, Return_WorkAreaPosition(k), Quaternion.identity);

                // 터레인의 높이 변경함. 한번에 하나씩 단계적 변형. 작업진행상황 시뮬레이션해야함.

                

                CalEvalTerrain(k);


                newSetHeight = shapeHeights_;

                //SetHeights to change the terrain height.
                TerrainMain_A.terrainData.SetHeightsDelayLOD(left_, bottom_, newSetHeight);
                TerrainMain_A.ApplyDelayedHeightmapModification();



                //// 조건문 추가 -> 중장비가 생성한 바위를 제거하면 이하 코드가 진행되어 지형의 높이를 변경함








                //// 원본 작업을 위한 터레인 변형값(높이) 적용을 위한 위치값 추출 
                //float posX = Mathf.Abs(getTrainDataFromWorkArea.leftBottom.Item1 - getTrainDataFromWorkArea.getWorkAreaList[k].Item1);

                //float posY = Mathf.Abs(getTrainDataFromWorkArea.leftBottom.Item2 - getTrainDataFromWorkArea.getWorkAreaList[k].Item2);

                //newSetHeight += getTrainDataFromWorkArea.SetHeight / rockSpawnAmount;

                //// 터레인의 지정위치 높이 적용값 가져오기 
                //shapeHeights_[getTrainDataFromWorkArea.getWorkAreaList[k].Item1, getTrainDataFromWorkArea.getWorkAreaList[k].Item2] = newSetHeight;


                //TerrainMain_A.terrainData.SetHeightsDelayLOD((int)posX, (int)posY, shapeHeights_);
                //TerrainMain_A.ApplyDelayedHeightmapModification();
            }


        }
    }



    //IEnumerator TerrainDelay()
    //{
    //    Debug.Log(" Delay start");
    //    yield return new WaitForSeconds(100f);
    //    Debug.Log(" Delay end");
    //}






    Vector3 Return_WorkAreaPosition(int i)
    {

        // 생성 위치에 콜라이더를 생성시킨다.
        // 생성위치의 중심점을 찾아낸다. Vector3 originPos. 작업영역의 포인트들에서 중심위치 찾기

        // 위치값 데이터 확인
        //for (int k = 1; k < getTrainDataFromWorkArea.getWorkAreaList.Count; k++)
        //    Debug.Log("getTrainDataFromWorkArea.getWorkAreaArray : " + getTrainDataFromWorkArea.getWorkAreaList[k]);

        // 추출한 위치값을 가져와서 적용할 터레인의 위치에 보정한다. 가져오는 값은 left, bottom 의 값이다.
        
        float posX = Mathf.Abs(getTrainDataFromWorkArea.leftBottom.Item1 - getTrainDataFromWorkArea.getWorkAreaList[i].Item1);

        float posY = Mathf.Abs(getTrainDataFromWorkArea.leftBottom.Item2 - getTrainDataFromWorkArea.getWorkAreaList[i].Item2);

        // test
        Vector3 respawnPos = new Vector3(posX + (posX * 0.1f), 50f, posY + (posY * 0.1f));

        return respawnPos;
    }




    public void CheckAmoutOfRocks()
    {
        if (spawnRocks != null)
        {
            spawnRocks = GameObject.FindGameObjectsWithTag("Rocks");
            // Debug.Log("spawnRocks:" + spawnRocks.Length);
            AmountOfRocks.text = spawnRocks.Length.ToString();
        }else
        {
            checkAmtOfRocks.WorkDone();
        }

    }


    Vector3 Return_RandomPosition()
    {
        Vector3 originPos = spwanArea.transform.position;
        float range_X = spwanAreaCollider.bounds.size.x;
        float range_Y = spwanAreaCollider.bounds.size.y;
        float range_Z = spwanAreaCollider.bounds.size.z;
        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        range_Z = Random.Range((range_Z / 2) * -1, range_Z / 2);
        Vector3 RandomPostion = new Vector3(range_X, range_Y, range_Z);

        Vector3 respawnPosition = originPos + RandomPostion;
        return respawnPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        //spwanAreaCollider = spwanArea.GetComponent<BoxCollider>();
        //SpawnRock();
    }

    [System.Obsolete]
    public void StartWork()
    {
        // 작업이 시작되기 전에 작업범위, 높이를 설정해야 한다. 조건문으로 판단
        // if 작업법위, 높이 지정되었는지 확인
        //    SpawnRock(); 작업 실행
        // else
        //    "작업범위와 높이를 설정하시오"


        Debug.Log("StartWork!");

        if (getTrainDataFromWorkArea.getWorkAreaList != null)
        {

            Debug.Log("Start...!");
            // collder를 생성하거나 줌심 위치를 추출해야 한다.

            //spwanAreaCollider = spwanArea.GetComponent<BoxCollider>();
            SpawnRockInWorkArea();
        }else
        {
            Debug.Log("Calcualte work area first.");
        }

    }


    private void Update()
    {
        CheckAmoutOfRocks();
    }




    //Checks if the given vertex is inside the the shape.
    bool InsidePolygon(Vector2 p, int terrainZmax)
    {
        // Assign the points that define the outline of the shape
        Vector3[] positions = new Vector3[line_A.positionCount];
        line_A.GetPositions(positions);

        int count = 0;
        Vector2 p1, p2;
        int n = positions.Length;

        // Find the lines that define the shape
        for (int i = 0; i < n; i++)
        {
            p1.y = positions[i].z;// - p.y;
            p1.x = positions[i].x;// - p.x;
            if (i != n - 1)
            {
                p2.y = positions[(i + 1)].z;// - p.y;
                p2.x = positions[(i + 1)].x;// - p.x;
            }
            else
            {
                p2.y = positions[0].z;// - p.y;
                p2.x = positions[0].x;// - p.x;
            }

            // check if the given point p intersects with the lines that form the outline of the shape.
            if (LinesIntersect(p1, p2, p, terrainZmax))
            {
                count++;
            }
        }

        // the point is inside the shape when the number of line intersections is an odd number
        if (count % 2 == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Function that checks if two lines intersect with each other
    bool LinesIntersect(Vector2 A, Vector2 B, Vector2 C, int terrainZmax)
    {
        Vector2 D = new Vector2(C.x, terrainZmax);
        Vector2 CmP = new Vector2(C.x - A.x, C.y - A.y);
        Vector2 r = new Vector2(B.x - A.x, B.y - A.y);
        Vector2 s = new Vector2(D.x - C.x, D.y - C.y);

        float CmPxr = CmP.x * r.y - CmP.y * r.x;
        float CmPxs = CmP.x * s.y - CmP.y * s.x;
        float rxs = r.x * s.y - r.y * s.x;

        if (CmPxr == 0f)
        {
            // Lines are collinear, and so intersect if they have any overlap

            return ((C.x - A.x < 0f) != (C.x - B.x < 0f))
                || ((C.y - A.y < 0f) != (C.y - B.y < 0f));
        }

        if (rxs == 0f)
            return false; // Lines are parallel.

        float rxsr = 1f / rxs;
        float t = CmPxs * rxsr;
        float u = CmPxr * rxsr;

        return (t >= 0f) && (t <= 1f) && (u >= 0f) && (u <= 1f);
    }


}
