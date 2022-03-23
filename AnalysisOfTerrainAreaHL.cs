using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

// 그리드 생성
// Fit on terrain size
// 그리드 중심위치 설정
// terrain에 raycast 발사 후 거리 측정(높이 측정해야 함), 단 측정거리는 입력값과 비교하여 H, L 결정
// 공사진행계획 수립(이것은 로직 생각해야 함)

public class AnalysisOfTerrainAreaHL : MonoBehaviour
{

    public string covertedStr;

    // 원본 터레인으로 Terrain Main_A로 작업하는 원본 Terrain
    public Terrain TerrainMain_A;

    //public GridGenLogic_Block pre_get_origins_giopos;

    public int rows = 10;
    public int columns = 10;
    public int scale;
    public GameObject gridPrefab;
    public Vector3 leftBottomLocation = new Vector3(0, 60, 0);


    // 생성된 gioPoint를 props 리스트에 등록해주기 위한 리스트
    public List<GameObject> props = new List<GameObject>();
    public List<Vector3> prebPosAll = new List<Vector3>();

    public string Post_PosEachBlock { get; private set; }

    public RaycastItemAligner rayAli;

    public int raycastDis = 100;

    //public Text earthVolumeText;
    //public Text post_earth_volume;
    //public Text getEarVolText;


    private bool get_e_volume = true;

    public float resultOfEarthVolume;
    public float re_post_e_volume;

    public Vector3 heightCheckObj;

    // 높이 설정
    public InputField setHeightInput;

    public Slider setHeightSliderInput;


    // 높이에 따른 컬러 설정
    public Color redColor = Color.red;
    public Color blueColor = Color.blue;
    public Color yellowColor = Color.yellow;
    public Color whiteColor = Color.white;

    private float red, orange, yellow, yellowGreen, green, white, blue, darkBlue, purple, deepPurple = 0;

    // 색깔로 작업면적비율 계산
    public Text digArea;
    public Text pileArea;

    // 작업면적 크기 계산
    public Text digFlatArea;
    public Text pileFlatArea;

    // 작업면적 부피 계산
    public Text digVolume;
    public Text pileVolume;

    // 중장비 투입값 입력
    public InputField excavatorInput;
    public InputField truckInput;
    public Text TimeToDigging;
    public Text TimeToPilling;


    public double digVolumeCal = 0f;
    public double pileVolumeCal = 0f;

    // 타일 색 변경 시뮬레이션 색상값 선언
    public Color32 redColor_, orangeColor_, yellowColor_, yellowGreenColor_, greenColor_, whiteColor_,
                   blueColor_, darkBlueColor_, litePurpleColor_, deepPurpleColor_;



    // Start is called before the first frame update
    [System.Obsolete]
    void Awake()
    {
        if (gridPrefab)
            GenerateGrid();
        else print("missing gridprefab, please assign.");

        // 생성된 프리팹의 모든 위치값을 추출 하여 저장한다. --> 이 값을 이제 lineRenderer로 보내서 도로를 그려주면된다.
        getPosOfPrefabs();
    }


    // GioPos 생성 - 버튼에 적용하는 실행함수
    [System.Obsolete]
    public void getGioPos()
    {
        //    // 리스트 변수 삭제, 다음 코드에서 새로 생성할거임
        //    this.gameObject.GetComponent<GridGenLogic_Block>().Post_PosEachBlock.Clear();

        if (gridPrefab)
        {
            //// 사전 GioPos위치값 추출을 위한 함수 실행
            //pre_get_origins_giopos = this.gameObject.GetComponent<GridGenLogic_Block>();
            //pre_get_origins_giopos.GetComponent<GridGenLogic_Block>().PreGetOriginGioPos();

            GenerateGrid();
        }
        else print("missing gridprefab, please assign.");

        // 생성된 프리팹의 모든 위치값을 추출 하여 저장한다. --> 이 값을 이제 lineRenderer로 보내서 도로를 그려주면된다.
        getPosOfPrefabs();
    }




    [System.Obsolete]
    public void getInfoOfTerrain()
    {
        //Get the terrain heightmap width and height.
        int xRes = TerrainMain_A.terrainData.heightmapWidth;

        int yRes = TerrainMain_A.terrainData.heightmapHeight;

        Debug.Log("Terrain X : Terrain Y : " + xRes + " : " + yRes);

        //GetHeights - gets the heightmap points of the tarrain. Store them in array
        float[,] heights = TerrainMain_A.terrainData.GetHeights(0, 0, xRes, yRes);

        scale = xRes / rows;
    }


    public void ResetHLAnalysis()
    {
        foreach (GameObject GidCube_ in GameObject.FindGameObjectsWithTag("GidCube"))
        {

            Destroy(GidCube_);
        }

        red = 0;
        orange = 0;
        yellow = 0;
        yellowGreen = 0;
        green = 0;
        blue = 0;
        darkBlue = 0;
        purple = 0;
        deepPurple = 0;

        digVolumeCal = 0;
        pileVolumeCal = 0;

        digArea.text = "Digging area ratio : " + "0" + "%";
        pileArea.text = "Pilling area ratio : " + "0" + "%";

        digFlatArea.text = "Digging area : " + "0" + "㎡";
        pileFlatArea.text = "Pilling area : " + "0" + "㎡";

        digVolume.text = "Digging Volume : " + "0" + "㎥ ";
        pileVolume.text = "Pilling Volume : " + "0" + "㎥ ";

        TimeToDigging.text = "Digging Duration : " + "0" + "Days " + "0" + " hrs";
        TimeToPilling.text = "Pilling Duration : " + "0" + "Days " + "0" + " hrs";


    }


    public object PositionRaycast(GameObject obj_)
    {
        RaycastHit hit;

        Physics.Raycast(obj_.transform.position, Vector3.down, out hit, raycastDis);

        heightCheckObj = hit.point;
        //Debug.Log("hit.point : " + hit.point);

        return heightCheckObj;
    }

    private int checkNum = 1;
    private Color resultColor;

    // 타일의 위치값과 높이를 계산한 색깔값을 저장하기위해서 딕셔너리 선언
    public Dictionary<string, List<Tuple<int, int, GameObject>>> eachPosWithColorValue = new Dictionary<string, List<Tuple< int, int, GameObject>>>();
    


    [System.Obsolete]
    public void GenerateGrid()
    {
        getInfoOfTerrain();


        // 저장형식은 색 + [i, j] 로 i,j는 좌표값

        // 활용은 색깔별로 위치값을 추출, 터레인의 높이를 변경하, 다시 컬러값 계산하는 방법으로 반복문 돌림 -- 이것이 시뮬레이션구현임 


        if ((checkNum % 2) != 0)
        {
            //Debug.Log("토공 전 지형의 부피 계산!");
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    GameObject obj = Instantiate(gridPrefab, new Vector3(leftBottomLocation.x + scale * i, leftBottomLocation.y, leftBottomLocation.z + scale * j), Quaternion.identity);

                    obj.transform.SetParent(gameObject.transform);

                    // class RaycastItemAligner 에서 위치값 obj를 입력하면 터레인에 레이캐스트를 하여 위치정보값을 추출 후 반환 
                    Vector3 eachHeight = (Vector3)PositionRaycast(obj);
                    //Debug.Log("PositionRaycast(obj).y : " + eachHeight.y);


                    // move obj to the terrain surface
                    //obj.transform.position = heightCheckObj;

                    //Debug.Log("obj posiiton : " + obj.transform.position);

                    string getHeight = setHeightInput.text;
                    float setHeiht_ = float.Parse(getHeight);


                    // slider input은 적용하지 않았음. 적용여부 결정해서 ui에 반영할 계획임
                    float setHeight__ = setHeightSliderInput.value;
                    //Debug.Log("sliderInput : " + setHeight__);


                    //Debug.Log("setHeight_ : " + setHeiht_);

                    //Debug.Log("eachHeight.y :" + eachHeight.y);
                    if (eachHeight.y > setHeiht_)
                    {

                        float inputEachHeight = eachHeight.y;
                        float inputHeightValue = setHeiht_;

                        if (inputEachHeight > (inputHeightValue + inputHeightValue * 0.8f))
                        {
                            redColor.a = 0.1f;
                            obj.GetComponent<Renderer>().material.color = Color.red;
                            //Debug.Log("Checked resultColor red");

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("red ? : " + getGameobjectColor(obj));
                            redColor_ = getGameobjectColor(obj);

                            var key = "red";

                            //eachPosWithColorValue["red"] = new List<Tuple<int, int>>();

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }

                        }
                        else if (inputEachHeight <= inputHeightValue + inputHeightValue * 0.8f && inputEachHeight > inputHeightValue + inputHeightValue * 0.6f)
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0f, 1); // 주황
                            //Debug.Log("Checked resultColor orange");
                            orange += 1;

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("orange ? : " + getGameobjectColor(obj));
                            orangeColor_ = getGameobjectColor(obj);

                            var key = "orange";

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }
                        }
                        else if (inputEachHeight <= inputHeightValue + inputHeightValue * 0.6f && inputEachHeight > inputHeightValue + inputHeightValue * 0.4f)
                        {
                            obj.GetComponent<Renderer>().material.color = Color.yellow;
                            //Debug.Log("Checked resultColor yellow");
                            yellow += 1;

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("yellow ? : " + getGameobjectColor(obj));
                            yellowColor_ = getGameobjectColor(obj);

                            var key = "yellow";

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }
                        }
                        else if (inputEachHeight <= inputHeightValue + inputHeightValue * 0.4f && inputEachHeight > inputHeightValue + inputHeightValue * 0.2f)
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(0.8f, 1f, 0f, 1f);
                            //Debug.Log("Checked resultColor yellowGreen");
                            yellowGreen += 1;

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("yellowGreen ? : " + getGameobjectColor(obj));
                            yellowGreenColor_ = getGameobjectColor(obj);

                            var key = "yellowGreen";

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }
                        }
                        else // 고도가 같을 경우 
                        {
                            obj.GetComponent<Renderer>().material.color = Color.green;
                            //Debug.Log("Checked resultColor green");
                            green += 1;

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("green ? : " + getGameobjectColor(obj));
                            greenColor_ = getGameobjectColor(obj);

                            var key = "green";

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }
                        }

                    }
                    else if (eachHeight.y == setHeiht_)
                    {
                        whiteColor.a = 0.1f;
                        obj.GetComponent<Renderer>().material.color = whiteColor;
                        white += 1;

                        // 오브젝트에 적용된 색깔 값 확인 
                        //Debug.Log("white ? : " + getGameobjectColor(obj));
                        whiteColor_ = getGameobjectColor(obj);


                        var key = "white";

                        if (eachPosWithColorValue.ContainsKey(key))
                        {
                            eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                        }
                        else
                        {
                            eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                        }
                    }
                    else
                    {
                        float inputEachHeight = eachHeight.y;
                        float inputHeightValue = setHeiht_;

                        if (inputEachHeight < inputHeightValue && inputEachHeight >= inputHeightValue - inputHeightValue * 0.2)
                        {
                            obj.GetComponent<Renderer>().material.color = Color.blue;
                            //Debug.Log("Checked resultColor blue");
                            blue += 1;

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("blue ? : " + getGameobjectColor(obj));
                            blueColor_ = getGameobjectColor(obj);

                            var key = "blue";

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }
                        }
                        else if ((inputEachHeight < inputHeightValue - inputHeightValue * 0.4) && (inputEachHeight >= inputHeightValue - inputHeightValue * 0.6))
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(0.2f, 0f, 0.7f, 1);  // 남색
                            //Debug.Log("Checked resultColor navy");
                            darkBlue += 1;

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("darkBlue ? : " + getGameobjectColor(obj));
                            darkBlueColor_ = getGameobjectColor(obj);

                            var key = "darkBlue";

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }
                        }
                        else if ((inputEachHeight < inputHeightValue - inputHeightValue * 0.6) && (inputEachHeight >= inputHeightValue - inputHeightValue * 0.8))
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(0.3f, 0f, 0.3f, 1); // purple
                            //Debug.Log("Checked resultColor purple");
                            purple += 1;

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("litePurple ? : " + getGameobjectColor(obj));
                            litePurpleColor_ = getGameobjectColor(obj);

                            var key = "litePurple";

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }
                        }
                        else
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(0.01f, 0.09f, 0.45f, 1); // darkblue
                            //obj.GetComponent<Renderer>().material.color = Color.; //black darkblue
                            //Debug.Log("Checked resultColor deep color");
                            deepPurple += 1;

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("deepPurple ? : " + getGameobjectColor(obj));
                            deepPurpleColor_ = getGameobjectColor(obj);

                            var key = "deepPurple";

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }
                            //var list = new List<Tuple<int, int>>();
                            //eachPosWithColorValue.Add(key, list);
                            //list.Add(Tuple.Create("default", "default"));
                        }
                    }

                    // 생성된 obj를 리스트에 등록해준다. 그러면 생성된 obj들을 모두 추적할 수 있다.
                    props.Add(obj);


                    // 토공량 계산하기 : 처음값 추출
                    //if (get_e_volume == true)
                    //{
                    //    rayAli.GetComponent<RaycastItemAligner>().EarthVolume(obj);
                    //    resultOfEarthVolume = rayAli.earthVolume;
                    //    earthVolumeText.text = "Pre Earth Volume : " + resultOfEarthVolume.ToString();

                    //}

                }
            }
            get_e_volume = false;
            checkNum += 1;
        }
        else
        {
            //Debug.Log("토공 후 지형의 부피 계산!");
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    GameObject obj = Instantiate(gridPrefab, new Vector3(leftBottomLocation.x + scale * i, leftBottomLocation.y, leftBottomLocation.z + scale * j), Quaternion.identity);

                    obj.transform.SetParent(gameObject.transform);

                    // class RaycastItemAligner 에서 위치값 obj를 입력하면 터레인에 레이캐스트를 하여 위치정보값을 추출 후 반환 
                    Vector3 eachHeight = (Vector3)PositionRaycast(obj);
                    //Debug.Log("PositionRaycast(obj).y : " + eachHeight.y);


                    // move obj to the terrain surface
                    //obj.transform.position = heightCheckObj;

                    //Debug.Log("obj posiiton : " + obj.transform.position);

                    string getHeight = setHeightInput.text;
                    float setHeiht_ = float.Parse(getHeight);


                    // slider input은 적용하지 않았음. 적용여부 결정해서 ui에 반영할 계획임
                    float setHeight__ = setHeightSliderInput.value;
                    //Debug.Log("sliderInput : " + setHeight__);


                    //Debug.Log("setHeight_ : " + setHeiht_);

                    //Debug.Log("eachHeight.y :" + eachHeight.y);
                    if (eachHeight.y > setHeiht_)
                    {

                        float inputEachHeight = eachHeight.y;
                        float inputHeightValue = setHeiht_;

                        if (inputEachHeight > (inputHeightValue + inputHeightValue * 0.8f))
                        {
                            redColor.a = 0.1f;
                            obj.GetComponent<Renderer>().material.color = Color.red;
                            //Debug.Log("Checked resultColor red");

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("red ? : " + getGameobjectColor(obj));
                            redColor_ = getGameobjectColor(obj);

                            var key = "red";

                            //eachPosWithColorValue["red"] = new List<Tuple<int, int>>();

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }

                        }
                        else if (inputEachHeight <= inputHeightValue + inputHeightValue * 0.8f && inputEachHeight > inputHeightValue + inputHeightValue * 0.6f)
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0f, 1); // 주황
                            //Debug.Log("Checked resultColor orange");
                            orange += 1;

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("orange ? : " + getGameobjectColor(obj));
                            orangeColor_ = getGameobjectColor(obj);

                            var key = "orange";

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }
                        }
                        else if (inputEachHeight <= inputHeightValue + inputHeightValue * 0.6f && inputEachHeight > inputHeightValue + inputHeightValue * 0.4f)
                        {
                            obj.GetComponent<Renderer>().material.color = Color.yellow;
                            //Debug.Log("Checked resultColor yellow");
                            yellow += 1;

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("yellow ? : " + getGameobjectColor(obj));
                            yellowColor_ = getGameobjectColor(obj);

                            var key = "yellow";

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }
                        }
                        else if (inputEachHeight <= inputHeightValue + inputHeightValue * 0.4f && inputEachHeight > inputHeightValue + inputHeightValue * 0.2f)
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(0.8f, 1f, 0f, 1f);
                            //Debug.Log("Checked resultColor yellowGreen");
                            yellowGreen += 1;

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("yellowGreen ? : " + getGameobjectColor(obj));
                            yellowGreenColor_ = getGameobjectColor(obj);

                            var key = "yellowGreen";

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }
                        }
                        else // 고도가 같을 경우 
                        {
                            obj.GetComponent<Renderer>().material.color = Color.green;
                            //Debug.Log("Checked resultColor green");
                            green += 1;

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("green ? : " + getGameobjectColor(obj));
                            greenColor_ = getGameobjectColor(obj);

                            var key = "green";

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }
                        }

                    }
                    else if (eachHeight.y == setHeiht_)
                    {
                        whiteColor.a = 0.1f;
                        obj.GetComponent<Renderer>().material.color = whiteColor;
                        white += 1;

                        // 오브젝트에 적용된 색깔 값 확인 
                        //Debug.Log("white ? : " + getGameobjectColor(obj));
                        whiteColor_ = getGameobjectColor(obj);


                        var key = "white";

                        if (eachPosWithColorValue.ContainsKey(key))
                        {
                            eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                        }
                        else
                        {
                            eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                        }
                    }
                    else
                    {
                        float inputEachHeight = eachHeight.y;
                        float inputHeightValue = setHeiht_;

                        if (inputEachHeight < inputHeightValue && inputEachHeight >= inputHeightValue - inputHeightValue * 0.2)
                        {
                            obj.GetComponent<Renderer>().material.color = Color.blue;
                            //Debug.Log("Checked resultColor blue");
                            blue += 1;

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("blue ? : " + getGameobjectColor(obj));
                            blueColor_ = getGameobjectColor(obj);

                            var key = "blue";

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }
                        }
                        else if ((inputEachHeight < inputHeightValue - inputHeightValue * 0.4) && (inputEachHeight >= inputHeightValue - inputHeightValue * 0.6))
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(0.2f, 0f, 0.7f, 1);  // 남색
                            //Debug.Log("Checked resultColor navy");
                            darkBlue += 1;

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("darkBlue ? : " + getGameobjectColor(obj));
                            darkBlueColor_ = getGameobjectColor(obj);

                            var key = "darkBlue";

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }
                        }
                        else if ((inputEachHeight < inputHeightValue - inputHeightValue * 0.6) && (inputEachHeight >= inputHeightValue - inputHeightValue * 0.8))
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(0.3f, 0f, 0.3f, 1); // purple
                            //Debug.Log("Checked resultColor purple");
                            purple += 1;

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("litePurple ? : " + getGameobjectColor(obj));
                            litePurpleColor_ = getGameobjectColor(obj);

                            var key = "litePurple";

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }
                        }
                        else
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(0.01f, 0.09f, 0.45f, 1); // darkblue
                            //obj.GetComponent<Renderer>().material.color = Color.; //black darkblue
                            //Debug.Log("Checked resultColor deep color");
                            deepPurple += 1;

                            // 오브젝트에 적용된 색깔 값 확인 
                            //Debug.Log("deepPurple ? : " + getGameobjectColor(obj));
                            deepPurpleColor_ = getGameobjectColor(obj);

                            var key = "deepPurple";

                            if (eachPosWithColorValue.ContainsKey(key))
                            {
                                eachPosWithColorValue[key].Add(new Tuple<int, int, GameObject>(i, j, obj));
                            }
                            else
                            {
                                eachPosWithColorValue[key] = new List<Tuple<int, int, GameObject>>();
                            }
                            //var list = new List<Tuple<int, int>>();
                            //eachPosWithColorValue.Add(key, list);
                            //list.Add(Tuple.Create("default", "default"));
                        }
                    }

                    // 생성된 obj를 리스트에 등록해준다. 그러면 생성된 obj들을 모두 추적할 수 있다.
                    props.Add(obj);


                    // 토공량 계산하기 : 처음값 추출
                    //if (get_e_volume == true)
                    //{
                    //    rayAli.GetComponent<RaycastItemAligner>().EarthVolume(obj);
                    //    resultOfEarthVolume = rayAli.earthVolume;
                    //    earthVolumeText.text = "Pre Earth Volume : " + resultOfEarthVolume.ToString();

                    //}

                }
            }
            //get_e_volume = true;
            //get_re_e_volume();
            //checkNum += 1;
        }
        //red, orange, yellow, yellowGreen, green,:  white   :blue, darkBlue, purple, litePurple,  deepPurple
        //Debug.Log("color counter check : " + red);
        float diggingArea = red + orange + yellow + yellowGreen;
        //Debug.Log("red + orange + yellow + yellowGreen :" + diggingArea);
        float pillingArea = green + blue + darkBlue + purple;
        //Debug.Log("green + blue + darkBlue + purple :" + pillingArea);
        float totalArea = red + orange + yellow + yellowGreen + green + blue + darkBlue + purple + deepPurple;
        //Debug.Log("color counter check totalArea : " + totalArea);

        // dig
        double redVol = red + (red * 0.8);
        double orangeVol = orange + orange * 0.6;
        double yellowVol = yellow + yellow * 0.4;
        double yelloGreenVol = yellowGreen + yellowGreen * 0.2;

        // pile
        double greenVol = green + green * 0.2;
        double blueVol = blue + blue * 0.4;
        double darkBlueVol = darkBlue + darkBlue * 0.6;
        double purpleVol = purple + purple * 0.8;

        digVolumeCal = redVol + orangeVol + yellowVol + yelloGreenVol;
        pileVolumeCal = greenVol + blueVol + darkBlueVol + purpleVol;
        //Debug.Log("digVolumeCal :" + digVolumeCal);
        //Debug.Log("pileVolumeCal :" + pileVolumeCal);

        // cal time duration to pile on the work area  : pilling color to yellow color
        string getExcavatorInput = excavatorInput.text;
        float getExcavatorInput_ = float.Parse(getExcavatorInput) * 0.2f;

        string getTruckInput = truckInput.text;
        float getTruckInput_ = float.Parse(getTruckInput) * 0.1f; // 0.4f라는 비율로 1대의 excavator에 트럭 비율 적용할 경우


        digArea.text = "Digging area ratio : " + ((diggingArea / totalArea) * 100).ToString() + "%";
        pileArea.text = "Pilling area ratio : " + ((pillingArea / totalArea) * 100).ToString() + "%";

        digFlatArea.text = "Digging area : " + diggingArea + "㎡";
        pileFlatArea.text = "Pilling area : " + pillingArea + "㎡";

        digVolume.text = "Digging Volume : " + digVolumeCal.ToString() + "㎥ ";
        pileVolume.text = "Pilling Volume : " + pileVolumeCal.ToString() + "㎥ ";

        // 중장비 투입 계수 적(현재는 임의 값 적용) min = 60 으로 설정하면, 
        double min = digVolumeCal / (getExcavatorInput_ + getTruckInput_) * 60;
        double days = Mathf.Round((float)min / 60 / 24);
        double hours = Mathf.Round((float)(min - days * 24 * 60) / 60);

        double minP = pileVolumeCal / (getExcavatorInput_ + getTruckInput_) * 60;
        double daysP = Mathf.Round((float)(minP / 60 / 24));
        double hoursP = Mathf.Round((float)(minP - daysP * 24 * 60) / 60);

        TimeToDigging.text = "Digging Duration : " + days.ToString() + "Days " + hours.ToString() + " hrs";
        TimeToPilling.text = "Pilling Duration : " + daysP.ToString() + "Days " + hoursP.ToString() + " hrs";

    }




    // 토공량 계산, 버튼으로 작동하기
    //public void get_re_e_volume()
    //{

    //    float get_earth_vol_value = resultOfEarthVolume - re_post_e_volume;
    //    getEarVolText.text = "Earth Volume : " + get_earth_vol_value.ToString();

    //}



    // 생성된 오브젝트의 위치를 추출해주는 함수를 만든다.
    public void getPosOfPrefabs()
    {
        for (int i = 0; i < props.Count; i++)
        {
            Vector3 getPosPreb = props[i].transform.position;
            // 생성된 프리팹의 위치값을 모두 저장한다.
            prebPosAll.Add(getPosPreb);
        }
    }

    // 작업과정 시뮬레이션 구현 : Digging, Pilling 순차적으로 표현하기
    // TerrainA의 지형정보 가져오기
    public ConstManager GetTerrainA;

    public void BtnStartCalculation()
    {
        StartSimulation();

    }

    public void BtnStarSimulation()
    {
        //StartSimulation();

        ////\Debug.Log("eachPosWithColorValue : " + eachPosWithColorValue);
        //foreach (KeyValuePair<string, List<Tuple<string, string>>> item in eachPosWithColorValue)
        //{
        //    Debug.Log("eachPosWithColorValue : " + item.Key + item.Value);
        //    ////Debug.Log("eachPosWithColorValue key : " + item.Key);
        //    //Debug.Log("eachPosWithColorValue value : " + item.Value[3]);
        //    //Debug.Log("eachPosWithColorValue value : " + item.Value[4]);
        //}

        // Delay
        //StartCoroutine("WaitForSecDelay");

        // 색기반 시뮬레이션을 위한 타일색 정보 추출
        StartCoroutine("CalTileObjColor");
        //CalTileObjColor();

    }

    // Delay
    IEnumerator WaitForSecDelay()
    {
        yield return new WaitForSeconds(2f);

    }

    // Get gameObject color info
    public Color32 getGameobjectColor(GameObject inputGameObject)
    {
        Color32 objColor;
        objColor = inputGameObject.GetComponent<MeshRenderer>().material.color;

        //Debug.Log("Tile Obj color: " + objColor);
        //Debug.Log(objColor.r + " " + objColor.g + " " + objColor.b + " " + objColor.a + " ");
        return objColor;
    }

    // Dictionary 사용하기 위한 클래스 선언 
    public class Item
    {
        private string keyColor;
        private int xPosNum;
        private int zPosNum;
        private GameObject titleObj;

        public Item(string _keyColor, int _xPosNum, int _zPosNum, GameObject _titleObj)
        {
            this.keyColor = _keyColor;
            this.xPosNum = _xPosNum;
            this.zPosNum = _zPosNum;
            this.titleObj = _titleObj;
        }

        public void show()
        {
            Debug.Log(this.keyColor);
            Debug.Log(this.xPosNum);
            Debug.Log(this.zPosNum);
            Debug.Log(this.titleObj);
        }

        public static implicit operator Item(List<Tuple<int, int, GameObject>> v)
        {
            throw new NotImplementedException();
        }
    }


    // 색변환 - 딕셔너리로 처리할 때 실행 코드
    public void ColorChangerUsigDictionary()
    {
        StartCoroutine("ColorChangerUsigDic");
    }


    public string green_str, yellowGreen_str, yellow_str, orange_str, red_str, blue_str, darkBlue_str, litePurple_str, deepPurple_str, white_str;

    public string convertToStr(Color32 inputColortype)
    {
        
        if (inputColortype.Equals(greenColor_))
        {
            covertedStr = "green";
        }
        else if (inputColortype.Equals(yellowGreenColor_))
        {
            covertedStr = "yellowGreen";
        }
        else if (inputColortype.Equals(yellowColor_))
        {
            covertedStr = "yellow";
        }
        else if (inputColortype.Equals(orangeColor_))
        {
            covertedStr = "orange";
        }
        else if (inputColortype.Equals(redColor_))
        {
            covertedStr = "red";
        }
        else if (inputColortype.Equals(blueColor_))
        {
            covertedStr = "blue";
        }
        else if (inputColortype.Equals(darkBlueColor_))
        {
            covertedStr = "darkBlue";
        }
        else if (inputColortype.Equals(litePurpleColor_))
        {
            covertedStr = "litePurple";
        }
        else if (inputColortype.Equals(deepPurpleColor_))
        {
            covertedStr = "deepPurple";
        }
        else if (inputColortype.Equals(whiteColor_))
        {
            covertedStr = "white";
        }

        return covertedStr;
    }




    IEnumerator ColorChangerUsigDic()
    {
        List<Color32> inputColor = new List<Color32>(new Color32[]
        {
            //"green","yellowGreen", "yellow", "orange", "red", "blue", "darkBlue", "litePurple", "deepPurple"
            greenColor_, yellowGreenColor_, yellowColor_, orangeColor_, redColor_, blueColor_, darkBlueColor_, litePurpleColor_, deepPurpleColor_, whiteColor_
        });
        // 색을 리스트로 지정, foreach 반복문에 적용해서 키 값으로 같은 색의 값을 모두 추출하여 정한 색과 같으면 색을 변경한.

        foreach (Color32 str in inputColor)
        {
            // color32 를 string 로 변환 
            var _str = convertToStr(str);
            Debug.Log("check key : " + _str);
            // dic values : eachPosWithColorValue
            if (eachPosWithColorValue.ContainsKey(_str)) // green, white... 이런식임

            {

                var itm__ = eachPosWithColorValue[_str];
                //Debug.Log("itm__ : " + itm__);
                foreach (Tuple<int, int, GameObject> eachitm in itm__)
                {
                    //Debug.Log("eachitm :" + eachitm.Item3); //eachitm :(27, 7, GioCube(Clone) (UnityEngine.GameObject))

                    Color32 get_obj_color = getGameobjectColor(eachitm.Item3);
                    //Debug.Log("get_obj_color firstCheck  :" + get_obj_color);

                    if (get_obj_color.Equals(greenColor_)) // get_obj_color vs greenColor_ 비교해야 함 
                    {
                        //Debug.Log("get_obj_color greenColor_ :" + get_obj_color);
                        //Debug.Log("yellowGreenColor_ :" + greenColor_);
                        eachitm.Item3.GetComponent<Renderer>().material.color = whiteColor;
                        //Debug.Log("Color changed to white!");

                        ////변경 후 딕셔너리의 키값을 갱신
                        //if (eachPosWithColorValue.ContainsKey(_str))
                        //{
                        //    var getValue = eachPosWithColorValue[_str];
                        //    eachPosWithColorValue.Remove(_str);
                        //    var nkey = "white";
                        //    foreach (Tuple<int, int, GameObject> iitm in getValue)
                        //    {
                        //        if (eachPosWithColorValue.ContainsKey(nkey))
                        //        {
                        //            eachPosWithColorValue[nkey].Add(iitm);
                        //            Debug.Log("Changed key to white!");
                        //        }
                        //        else
                        //        {
                        //            eachPosWithColorValue[nkey] = new List<Tuple<int, int, GameObject>>();
                        //            Debug.Log("Changed key to white!");
                        //        }

                        //    }
                        //} // 예외처리 안했음 

                        

                        

                        //Debug.Log("eachPosWithColorValue[_str][0] check1 :" + eachPosWithColorValue[_str][0]);
                        //Debug.Log("eachPosWithColorValue[_str][1] check1 :" + eachPosWithColorValue[_str][1]);
                        //// 키가 삭제되었는지 확인
                        //if (eachPosWithColorValue.ContainsKey(_str))
                        //{
                        //    Debug.Log("check 1 : Exsist Key");
                        //}
                        //else
                        //{
                        //    Debug.Log("check 1: Deleted Key");
                        //}

                        //eachPosWithColorValue.Remove(_str);
                        //// 키가 삭제되었는지 확인
                        //if (eachPosWithColorValue.ContainsKey(_str))
                        //{
                        //    Debug.Log("check 2 : Exsist Key");
                        //}else
                        //{
                        //    Debug.Log("check 2 : Deleted Key");
                        //}

                        //Debug.Log("eachPosWithColorValue[_str][0] check2  :" + eachPosWithColorValue[_str][0]);
                        //Debug.Log("eachPosWithColorValue[_str][1] check2  :" + eachPosWithColorValue[_str][1]);


                        //var key = "white";
                        //if (!(eachPosWithColorValue.ContainsKey(key)))
                        //{
                        //    eachPosWithColorValue.Add(key, eachPosWithColorValue[_str]);
                        //}


                    }
                    if (get_obj_color.Equals(yellowGreenColor_)) // yellowGreen > green
                    {
                        //Debug.Log("get_obj_color yellowGreenColor_ :" + get_obj_color);
                        //Debug.Log("yellowGreenColor_ :" + yellowGreenColor_);

                        eachitm.Item3.GetComponent<Renderer>().material.color = Color.green;
                        //Debug.Log("Color chanaged! green");
                        //yield return null;
                        //변경 후 딕셔너리의 키값을 갱신
      

                    }
                    if (get_obj_color.Equals(yellowColor_)) // yellow > yellowGreen
                    {
                        eachitm.Item3.GetComponent<Renderer>().material.color = new Color(0.8f, 1f, 0f, 1f);
                        //Debug.Log("Color chanaged! yellowGreen");
                        //yield return null;
                        //변경 후 딕셔너리의 키값을 갱신


                    }
                    if (get_obj_color.Equals(orangeColor_)) // orange > yellow
                    {
                        eachitm.Item3.GetComponent<Renderer>().material.color = Color.yellow;
                        //Debug.Log("Color chanaged! yellow");
                        //yield return null;

                    }
                    if (get_obj_color.Equals(redColor_)) // red > orage
                    {
                        eachitm.Item3.GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0f, 1); // 주황
                        //Debug.Log("Color chanaged! orage");
                        //yield return null;


                    }
                    // -----
                    if (get_obj_color.Equals(blueColor_)) // blue > white
                    {
                        eachitm.Item3.GetComponent<Renderer>().material.color = whiteColor;
                        //Debug.Log("Color chanaged! blue");
                        //yield return null;

                    }
                    if (get_obj_color.Equals(darkBlueColor_)) // darkBlue > blue
                    {
                        eachitm.Item3.GetComponent<Renderer>().material.color = Color.blue;
                        //Debug.Log("Color chanaged!  darkBlueColor_");
                        //yield return null;


                    }
                    if (get_obj_color.Equals(litePurpleColor_)) // litePupple > darkBlue
                    {
                        eachitm.Item3.GetComponent<Renderer>().material.color = new Color(0.2f, 0f, 0.7f, 1);  // litePupple
                        //Debug.Log("Color chanaged! litePupple");
                        //yield return null;


                    }
                    if (get_obj_color.Equals(deepPurpleColor_)) // deepPurple > litePurple
                    {
                        //eachObj.GetComponent<Renderer>().material.color = new Color(0.01f, 0.09f, 0.45f, 1); // darkblue
                        eachitm.Item3.GetComponent<Renderer>().material.color = darkBlueColor_; // darkblue


                    }
                    else if (get_obj_color.Equals(whiteColor_))
                    {
                        //Debug.Log("Color chanaged! white color");
                        eachitm.Item3.GetComponent<Renderer>().material.color = whiteColor;
                        //eachObj.GetComponent<Renderer>().material.color = new Color();


                    }


                    yield return new WaitForSeconds(0.0f);

                }

                //변경 후 딕셔너리의 키값을 갱신.  여기서 _str이 흰색일 경우에서 점차 색이 바뀌기 시작하면 해당 딕셔너리의 키를 개별적으로 변경해야 함 
                //if (eachPosWithColorValue.ContainsKey(_str))
                //{
                //    var getValue = eachPosWithColorValue[_str];
                //    eachPosWithColorValue.Remove(_str);
                //    var nkey = "white";
                //    foreach (Tuple<int, int, GameObject> iitm in getValue)
                //    {
                //        if (eachPosWithColorValue.ContainsKey(nkey))
                //        {
                //            eachPosWithColorValue[nkey].Add(iitm);
                //            Debug.Log("Changed key to white!");
                //        }
                //        else
                //        {
                //            eachPosWithColorValue[nkey] = new List<Tuple<int, int, GameObject>>();
                //            Debug.Log("Changed key to white!");
                //        }

                //    }
                //} // 예외처리 안했음 
            }
        }
        Debug.Log("Simulation done.");
        // 1차 변환이 끝나면, 다시 타일의 값을 추출하고, 시뮬레이션을 시작한다 모든 타일이 흰색으로 바뀔때까지 반복한다 
        
    }


    // 색변환 - 리스트로 처리할 때 실행 코드 
    IEnumerator ColorChanger(Color32 getObjColor, GameObject eachObj)
    {

        //int loopNum = 0;

        if (!getObjColor.Equals(whiteColor_))
        {
            //Debug.Log("!getObjColor.Equals(whiteColor_)");
            //Debug.Log("getEachObjColor : " + getObjColor);
            //Debug.Log("getEachObjColor greenColor_: " + greenColor_); // RGBA(0, 255, 0, 255)
            //Debug.Log("getEachObjColor yellowColor_: " + yellowColor_); // RGBA(255, 235, 4, 255)
            //Debug.Log("getEachObjColor orangeColor_: " + orangeColor_); // RGBA(255, 128, 0, 255)
            //Debug.Log("getEachObjColor redColor_: " + redColor_); // RGBA(255, 0, 0, 255)
            //Debug.Log("getEachObjColor blueColor_: " + blueColor_); // RGBA(0, 0, 255, 255)
            //Debug.Log("getEachObjColor darkBlueColor_: " + darkBlueColor_); // RGBA(51, 0, 178, 255)
            //Debug.Log("getEachObjColor litePurpleColor_: " + litePurpleColor_); // RGBA(76, 0, 76, 255)
            //Debug.Log("getEachObjColor deepPurpleColor_: " + deepPurpleColor_); // RGBA(3, 23, 115, 255)

            Debug.Log("each Color chanaged!");

            if (getObjColor.Equals(greenColor_)) // green > white
            {
                eachObj.GetComponent<Renderer>().material.color = whiteColor;
                //Debug.Log("Color chanaged! whiteColor");
                //yield return null;
            
            }
            else if (getObjColor.Equals(yellowGreenColor_)) // yellowGreen > green
            {
                eachObj.GetComponent<Renderer>().material.color = Color.green;
                //Debug.Log("Color chanaged! green");
                //yield return null;
               
            }
            else if (getObjColor.Equals(yellowColor_)) // yellow > yellowGreen
            {
                eachObj.GetComponent<Renderer>().material.color = new Color(0.8f, 1f, 0f, 1f);
                //Debug.Log("Color chanaged! yellowGreen");
                //yield return null;
                
            }
            else if (getObjColor.Equals(orangeColor_)) // orange > yellow
            {
                eachObj.GetComponent<Renderer>().material.color = Color.yellow;
                //Debug.Log("Color chanaged! yellow");
                //yield return null;
                
            }
            else if (getObjColor.Equals(redColor_)) // red > orage
            {
                eachObj.GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0f, 1); // 주황
                //Debug.Log("Color chanaged! orage");
                //yield return null;
                
            }
            // -----
            else if (getObjColor.Equals(blueColor_)) // blue > white
            {
                eachObj.GetComponent<Renderer>().material.color = whiteColor;
                //Debug.Log("Color chanaged! white");
                //yield return null;
                
            }
            else if (getObjColor.Equals(Color.blue)) // darkBlue > blue
            {
                eachObj.GetComponent<Renderer>().material.color = whiteColor;
                //Debug.Log("Color chanaged!  blue");
                //yield return null;
                
            }
            else if (getObjColor.Equals(darkBlueColor_)) // darkBlue > blue
            {
                eachObj.GetComponent<Renderer>().material.color = Color.blue;
                //Debug.Log("Color chanaged!  blue");
                //yield return null;
                
            }
            else if (getObjColor.Equals(litePurpleColor_)) // litePupple > darkBlue
            {
                eachObj.GetComponent<Renderer>().material.color = new Color(0.2f, 0f, 0.7f, 1);  // 남색
                //Debug.Log("Color chanaged!");
                //yield return null;
               
            }
            else if (getObjColor.Equals(deepPurpleColor_)) // deepPurple > litePurple
            {
                //eachObj.GetComponent<Renderer>().material.color = new Color(0.01f, 0.09f, 0.45f, 1); // darkblue
                eachObj.GetComponent<Renderer>().material.color = darkBlueColor_; // darkblue
                //Debug.Log("Color chanaged! litePurple");
                //yield return null;
                
            }
            else if (getObjColor.Equals(whiteColor_))
            {
                //Debug.Log("Color chanaged! darkblue");
                
                //eachObj.GetComponent<Renderer>().material.color = new Color();
                
            }

            // while 예외처리 
            //if (loopNum++ > 10000)
            //    throw new Exception("Infinite Loop");
                



            //yield return new WaitForSeconds(0.1f);
            yield return null;
            //Debug.Log("Done. " + Time.time);
            //Debug.Log("All Color chanaged!");
        }else
        {
            Debug.Log("Color chanaged to white!");
        }
    }

    // 생성된 타일의 색을 변경하는 시뮬레이션 구현
    // 순서는 white를 중심으로 양쪽으로 전개, 높이 차이가 적은 순서로 시뮬레이션 진행
    // red <- orange <- yellow <- yellowGreen <- green,:  white :blue -> darkBlue -> purple -> deepPurple
    // 
    //public void CalTileObjColor()
    IEnumerator CalTileObjColor()
    {
        // 1)생성된 컬러타일 오브젝트를 모두 불러옴, 오브젝트의 컬러값을 추출
        // props 개수 확인
        //int propsNumber = props.Count;
        //Debug.Log("propsNumber :" + propsNumber); // 2500개 맞음!

        //WaitForSeconds wait = new WaitForSeconds(2f);

        foreach (GameObject eachObj in props)
        {
            Debug.Log("eachObj : " + eachObj);

            Color32 getObjColor = getGameobjectColor(eachObj);

            StartCoroutine(ColorChanger(getObjColor, eachObj));

            yield return new WaitForSeconds(0.0f);

            //StartCoroutine("WaitForSecDelay");


        }

        // purple ? : RGBA(51, 0, 178, 255) ,deepPurple ? : RGBA(3, 23, 115, 255),  litePurple ? : RGBA(76, 0, 76, 255).. 이런식임 

        // 아래 값을 가지고 컬러값을 비교하여 조건에따라 타일 오브젝트의 색을 변경하는 시뮬레이션을 구현하면 됨 !!!!!!!
        // redColor_, orangeColor_, yellowColor_, yellowGreenColor_, greenColor_,
        // whiteColor_,
        // blueColor_, darkBlueColor_, litePurpleColor_, deepPurpleColor_;

        // 2)조건을 적용하여 if green -> whire로 변경, if yellowGreen -> green ... 이런 방식으로 오브젝트를 모두 흰 색으로 변경하고
        // 3)변경 후 변경  흰색으로 변하면 최종적으로 변경 후에 다음 변경 타일의 위치를 화살표 오브젝트를 생성하여 방향 표1
    }


    /// <summary>
    /// // 이하 코드는 사용하지 않음 

    public void titleColorSecondLoopCalculation()
    {
        // 생성된 타일 오브젝트 모두 가져오기
        GameObject getTileGObj;
        getTileGObj = GameObject.FindGameObjectWithTag("GridCube");
        // 가져온 게임오브젝트의 색상값을 키값으로 딕셔너리 만들/
    }
    /// </summary>


    [System.Obsolete]
    public void StartSimulation()
    {
        // 작업 기준 높이설정 setHeight 가져오기
        string getHeight = setHeightInput.text;
        float setHeiht_ = float.Parse(getHeight);
        //Debug.Log("get setHeight form Constmanager : " + setHeiht_);

        // 구간별 작업량 색으로 표현
        GenerateGrid();

        // 작업순서 정하기(매번의 작업 수행 후 지형높이 색으로 계산하는 부분 reset + run 랜더링 실시) 
        // 1)저지대(기준높이 이하) 밝은부분부터 어두운 부분 순서로 흰색(기준높)로 순차적 작업진행
        //Get the terrain heightmap width and height. xRes : 513, yRes: 513
        int xRes = GetTerrainA.TerrainMain_A.terrainData.heightmapWidth;
        int yRes = GetTerrainA.TerrainMain_A.terrainData.heightmapHeight;

        //GetHeights - gets the heightmap points of the tarrain. Store them in array
        //float[,] heights = GetTerrainA.TerrainMain_A.terrainData.GetHeights(0, 0, xRes, yRes);

        var shapeHeights_ = GetTerrainA.TerrainMain_A.terrainData.GetHeights(0, 0, xRes, yRes);

        /////////////float[,] newSetHeight;

        // 점진적으로 변경해야 하는 코드를 넣어야 함 !!!!!!!!!
        // 조건문으로 1/10 씩 바뀌도록 목표 높이에 도달하면 멈추는 코드 적용해야 함

        // 특정색의 높이값을 추출하여 한번에 한단계씩 높이를 변경해야 함

        for (int i = 0; i < shapeHeights_.GetLength(0); i++)
        {
            for (int j = 0; j < shapeHeights_.GetLength(1); j++)
            {
                //Debug.Log("shapeHeights_ = " + i + " : " + j);
            }
        }



        // 값 확인 
        //foreach (KeyValuePair<string, List<Tuple<int, int>>> item in eachPosWithColorValue)
        //{
        //Debug.Log("eachPosWithColorValue : " + item.Key + item.Value);
        //    Debug.Log("eachPosWithColorValue key : " + item.Key);
        //    Debug.Log("eachPosWithColorValue value 1 : " + item.Value[0].Item1);
        //    Debug.Log("eachPosWithColorValue value 2 : " + item.Value[0].Item2);
        //}



        // 색 추출 순서
        // red, orange, yellow, yellowGreen, green, white, blue, darkBlue, purple, deepPurple
        // Digging priority : green, yelloGreen, yellow, orange, red
        // Pilling priority : blue, darkBlue, purple, deepPurple

        // Debug.Log("eachPosWithColorValue : " + eachPosWithColorValue);


        //////// 이하의 분석 부분은 타일 색상 변화 시뮬레이션이 시작전과 끝난 후 한번씩만 계산하자 속도가 매우 느려짐  /////
        //// 테러인의 데이터를 모두 추출하여 분석 -----------------

        //var tData = GetTerrainA.TerrainMain_A.terrainData;
        //int xResolution = tData.heightmapWidth;
        //int zResolution = tData.heightmapHeight;
        //float[,] heights = tData.GetHeights(0, 0, xResolution, zResolution);
        //Debug.Log("height : " + heights);

        //List<Tuple<int, int>> list_XZ = new List<Tuple<int, int>>();

        //for (int i = 0; i < xResolution; i++)
        //{
        //    for (int j = 0; j < zResolution; j++)
        //    {
        //        list_XZ.Add(new Tuple<int, int>(i, j));

        //    }
        //}

        //Dictionary<Tuple<int, int>, float> list_gioSet = new Dictionary<Tuple<int, int>, float>();

        //int k = 0;
        //foreach (float hitem in heights)
        //{

        //    list_gioSet.Add(list_XZ[k], hitem);
        //    k += 1;

        //    // height hitem : 0.04492462

        //    // 데이터 확인
        //    Debug.Log("Terrain pos and height: " + list_gioSet);
        //    // Terrain pos and height: System.Collections.Generic.Dictionary`2[System.Tuple`2[System.Int32,System.Int32],System.Single]
        //}

        //// -------------------



        foreach (KeyValuePair<string, List<Tuple<int, int, GameObject>>> item in eachPosWithColorValue)
        {
            if (item.Key == "green")
            {
                //높이 변경 위치값 extract
                var ittm1 = item.Value[0].Item1;
                var ittm2 = item.Value[0].Item2;
                Debug.Log("Green");

                //Physics.Raycast(obj_.transform.position, Vector3.down, out hit, raycastDis);
                //heightCheckObj = hit.point;

                //게임오브젝트 생성, 여기서 레이케스트를 발사



                // obj 는 처음에 타일 계산때 저장한 값으로 여기서 레이케스트를 발사하는 주ㅊㅔ가 되는게임오브젝트. 어떻게 불러올지 생각해야 함. 


                Ray ray = new Ray(item.Value[0].Item3.transform.position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, raycastDis))
                {
                    Debug.Log("hit ray on terrain-----------------------------------");
                    Debug.Log("Hit point x : x = " + hit.point.x + ":" + hit.point.z);
                    float relativeHitTerX = (hit.point.x - GetTerrainA.TerrainMain_A.transform.position.x) / GetTerrainA.TerrainMain_A.terrainData.size.x;
                    float relativeHitTerZ = (hit.point.z - GetTerrainA.TerrainMain_A.transform.position.z) / GetTerrainA.TerrainMain_A.terrainData.size.z;
                    Debug.Log("relativeHitTerX : relativeHitTerZ = " + relativeHitTerX + ":" + relativeHitTerZ);

                    //float relativeTerCoordX = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerX;
                    //float relativeTerCoordZ = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerZ;


                    // 높이를 조정할 위치를 파악relativeHitTerX
                    //Debug.Log("relativeTerCoordX : relativeTerCoordX = " + relativeTerCoordX + " : " + relativeTerCoordX);

                    // 타일의 좌표값에 해당하는 부분의 지형정보만 변경해야, 그 외의 지형정보는 유지해야 함
                    //shapeHeights_ = [relativeTerCoordX, relativeHitTerZ]; // set the height value to the terrain vertex

                    //SetHeights to change the terrain height.
                    //GetTerrainA.TerrainMain_A.terrainData.SetHeightsDelayLOD(0, 0, shapeHeights_);
                    //GetTerrainA.TerrainMain_A.ApplyDelayedHeightmapModification();










                }


                ////    // 타일의 좌표값에 해당하는 부분의 지형정보만 변경해야, 그 외의 지형정보는 유지해야 함
                ////if (shapeHeights_)
                ////}
                //    shapeHeights_ = [relativeTerCoordX, relativeTerCoordX]; // set the height value to the terrain vertex

                //    //SetHeights to change the terrain height.
                //    GetTerrainA.TerrainMain_A.terrainData.SetHeightsDelayLOD(0, 0, shapeHeights_);
                //    GetTerrainA.TerrainMain_A.ApplyDelayedHeightmapModification();
                }
            else if (item.Key == "yellowGreen")
            {
                //높이 변경 위치값 추
                var ittm1 = item.Value[0].Item1;
                var ittm2 = item.Value[0].Item2;
                Debug.Log("yellowGreen");

                Ray ray = new Ray(item.Value[0].Item3.transform.position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f))
                {
                    Debug.Log("hit ray on terrain");
                    float relativeHitTerX = (hit.point.x - GetTerrainA.TerrainMain_A.transform.position.x) / GetTerrainA.TerrainMain_A.terrainData.size.x;
                    float relativeHitTerZ = (hit.point.z - GetTerrainA.TerrainMain_A.transform.position.z) / GetTerrainA.TerrainMain_A.terrainData.size.z;

                    //float relativeTerCoordX = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerX;
                    //float relativeTerCoordZ = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerZ;
                    //Debug.Log("relativeTerCoordX : relativeTerCoordX = " + relativeTerCoordX + " : " + relativeTerCoordZ);

                }
            }
            else if (item.Key == "yellow")
            {
                //높이 변경 위치값 추
                var ittm1 = item.Value[0].Item1;
                var ittm2 = item.Value[0].Item2;
                Debug.Log("yellow");

                Ray ray = new Ray(item.Value[0].Item3.transform.position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f))
                {
                    Debug.Log("hit ray on terrain");
                    float relativeHitTerX = (hit.point.x - GetTerrainA.TerrainMain_A.transform.position.x) / GetTerrainA.TerrainMain_A.terrainData.size.x;
                    float relativeHitTerZ = (hit.point.z - GetTerrainA.TerrainMain_A.transform.position.z) / GetTerrainA.TerrainMain_A.terrainData.size.z;

                    //float relativeTerCoordX = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerX;
                    //float relativeTerCoordZ = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerZ;
                    //Debug.Log("relativeTerCoordX : relativeTerCoordX = " + relativeTerCoordX + " : " + relativeTerCoordZ);

                }
            }
            else if (item.Key == "orange")
            {
                //높이 변경 위치값 추
                var ittm1 = item.Value[0].Item1;
                var ittm2 = item.Value[0].Item2;
                Debug.Log("orange");

                Ray ray = new Ray(item.Value[0].Item3.transform.position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f))
                {
                    float relativeHitTerX = (hit.point.x - GetTerrainA.TerrainMain_A.transform.position.x) / GetTerrainA.TerrainMain_A.terrainData.size.x;
                    float relativeHitTerZ = (hit.point.z - GetTerrainA.TerrainMain_A.transform.position.z) / GetTerrainA.TerrainMain_A.terrainData.size.z;

                    //float relativeTerCoordX = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerX;
                    //float relativeTerCoordZ = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerZ;
                    //Debug.Log("relativeTerCoordX : relativeTerCoordX = " + relativeTerCoordX + " : " + relativeTerCoordZ);

                }
            }
            else if (item.Key == "red")
            {
                //높이 변경 위치값 추
                var ittm1 = item.Value[0].Item1;
                var ittm2 = item.Value[0].Item2;
                Debug.Log("red");

                Ray ray = new Ray(item.Value[0].Item3.transform.position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f))
                {
                    float relativeHitTerX = (hit.point.x - GetTerrainA.TerrainMain_A.transform.position.x) / GetTerrainA.TerrainMain_A.terrainData.size.x;
                    float relativeHitTerZ = (hit.point.z - GetTerrainA.TerrainMain_A.transform.position.z) / GetTerrainA.TerrainMain_A.terrainData.size.z;

                    //float relativeTerCoordX = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerX;
                    //float relativeTerCoordZ = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerZ;
                    //Debug.Log("relativeTerCoordX : relativeTerCoordX = " + relativeTerCoordX + " : " + relativeTerCoordZ);

                }
            }
            else if (item.Key == "blue")
            {
                //높이 변경 위치값 추
                var ittm1 = item.Value[0].Item1;
                var ittm2 = item.Value[0].Item2;
                Debug.Log("blue");

                Ray ray = new Ray(item.Value[0].Item3.transform.position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f))
                {
                    float relativeHitTerX = (hit.point.x - GetTerrainA.TerrainMain_A.transform.position.x) / GetTerrainA.TerrainMain_A.terrainData.size.x;
                    float relativeHitTerZ = (hit.point.z - GetTerrainA.TerrainMain_A.transform.position.z) / GetTerrainA.TerrainMain_A.terrainData.size.z;

                    //float relativeTerCoordX = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerX;
                    //float relativeTerCoordZ = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerZ;
                    //Debug.Log("relativeTerCoordX : relativeTerCoordX = " + relativeTerCoordX + " : " + relativeTerCoordZ);

                }

            }
            else if (item.Key == "darkBlue")
            {
                //높이 변경 위치값 추
                var ittm1 = item.Value[0].Item1;
                var ittm2 = item.Value[0].Item2;
                Debug.Log("darkBlue");

                Ray ray = new Ray(item.Value[0].Item3.transform.position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f))
                {
                    float relativeHitTerX = (hit.point.x - GetTerrainA.TerrainMain_A.transform.position.x) / GetTerrainA.TerrainMain_A.terrainData.size.x;
                    float relativeHitTerZ = (hit.point.z - GetTerrainA.TerrainMain_A.transform.position.z) / GetTerrainA.TerrainMain_A.terrainData.size.z;

                    //float relativeTerCoordX = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerX;
                    //float relativeTerCoordZ = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerZ;
                    //Debug.Log("relativeTerCoordX : relativeTerCoordX = " + relativeTerCoordX + " : " + relativeTerCoordZ);

                }
            }
            else if (item.Key == "purple")
            {
                //높이 변경 위치값 추
                var ittm1 = item.Value[0].Item1;
                var ittm2 = item.Value[0].Item2;
                Debug.Log("purple");

                Ray ray = new Ray(item.Value[0].Item3.transform.position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f))
                {
                    float relativeHitTerX = (hit.point.x - GetTerrainA.TerrainMain_A.transform.position.x) / GetTerrainA.TerrainMain_A.terrainData.size.x;
                    float relativeHitTerZ = (hit.point.z - GetTerrainA.TerrainMain_A.transform.position.z) / GetTerrainA.TerrainMain_A.terrainData.size.z;

                    //float relativeTerCoordX = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerX;
                    //float relativeTerCoordZ = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerZ;
                    //Debug.Log("relativeTerCoordX : relativeTerCoordX = " + relativeTerCoordX + " : " + relativeTerCoordZ);

                }
            }
            else if (item.Key == "deepPurple")
            {
                //높이 변경 위치값 추
                var ittm1 = item.Value[0].Item1;
                var ittm2 = item.Value[0].Item2;
                Debug.Log("deepPurple");

                Ray ray = new Ray(item.Value[0].Item3.transform.position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f))
                {
                    float relativeHitTerX = (hit.point.x - GetTerrainA.TerrainMain_A.transform.position.x) / GetTerrainA.TerrainMain_A.terrainData.size.x;
                    float relativeHitTerZ = (hit.point.z - GetTerrainA.TerrainMain_A.transform.position.z) / GetTerrainA.TerrainMain_A.terrainData.size.z;

                    //float relativeTerCoordX = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerX;
                    //float relativeTerCoordZ = GetTerrainA.TerrainMain_A.terrainData.heightmapResolution * relativeHitTerZ;
                    //Debug.Log("relativeTerCoordX : relativeTerCoordX = " + relativeTerCoordX + " : " + relativeTerCoordZ);

                }
                ////높이 변경하기 20%씩 변경!
                //shapeHeights_[i, j] = setHeiht_ / 5; // set the height value to the terrain vertex

                ////SetHeights to change the terrain height.
                //GetTerrainA.TerrainMain_A.terrainData.SetHeightsDelayLOD(0, 0, shapeHeights_);
                //GetTerrainA.TerrainMain_A.ApplyDelayedHeightmapModification();
            }
        }



    }
    //}




    //foreach (KeyValuePair<string, List<Tuple<string, string>>> item in eachPosWithColorValue)
    //{
    //    //Debug.Log("eachPosWithColorValue : " + item.Key + item.Value);
    //    //Debug.Log("eachPosWithColorValue key : " + item.Key);
    //    //Debug.Log("eachPosWithColorValue value : " + item.Value);

    //    //switch (item.Key)
    //    //{
    //    //    case "green":
    //    //        Debug.Log("green");
    //    //        //shapeHeights_ = [item.Value[0], item.Value[1]];
    //    //        break;

    //    //    case "yelloGreen":
    //    //        Debug.Log("yelloGreen");
    //    //        break;
    //    //    case "yellow":
    //    //        Debug.Log("yellow");
    //    //        break;
    //    //    case "orange":
    //    //        Debug.Log("orange");
    //    //        break;
    //    //    case "red":
    //    //        Debug.Log("red");
    //    //        break;

    //    //    case "blue":
    //    //        Debug.Log("blue");
    //    //        break;
    //    //    case "darkBlue":
    //    //        Debug.Log("darkBlue");
    //    //        break;
    //    //    case "purple":
    //    //        Debug.Log("purple");
    //    //        break;
    //    //    case "deepPurple":
    //    //        Debug.Log("deepPurple");
    //    //        break;
    //    //}

    //}



    //for (int i = 0; i < yRes; i++)
    //{
    //    for (int j = 0; j < xRes; j++)
    //    {
    //        //높이 변경하기
    //        shapeHeights_[i, j] = setHeiht_; ; // set the height value to the terrain vertex
    //    }
    //}



    //SetHeights to change the terrain height.
    //GetTerrainA.TerrainMain_A.terrainData.SetHeightsDelayLOD(0, 0, shapeHeights_);
    //GetTerrainA.TerrainMain_A.ApplyDelayedHeightmapModification();


    // 2)고지대(기준높이 이상) 기준높이로부터 가장 밝은(높은)부분으로 순차적 작업진행

    // 실행액션은 버튼으로 구현하기
    //}
    //}

    

    // Update is called once per frame
    void Update()
    {
        
    }
}