using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// 그리드 생성
// Fit on terrain size
// 그리드 중심위치 설정
// terrain에 raycast 발사 후 거리 측정(높이 측정해야 함), 단 측정거리는 입력값과 비교하여 H, L 결정
// 공사진행계획 수립(이것은 로직 생각해야 함)

public class AnalysisOfTerrainAreaHL : MonoBehaviour
{
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

    private float red, orange, yellow, yellowGreen, green, blue, darkBlue, purple, deepPurple = 0;

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

    [System.Obsolete]
    public void GenerateGrid()
    {
        getInfoOfTerrain();

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
                    float setHeight__  = setHeightSliderInput.value;
                    //Debug.Log("sliderInput : " + setHeight__);


                    //Debug.Log("setHeight_ : " + setHeiht_);

                    Debug.Log("eachHeight.y :" + eachHeight.y);
                    if (eachHeight.y > setHeiht_)
                    {

                        float inputEachHeight = eachHeight.y;
                        float inputHeightValue = setHeiht_;

                        if (inputEachHeight > (inputHeightValue + inputHeightValue * 0.8f))
                        {
                            redColor.a = 0.1f;
                            obj.GetComponent<Renderer>().material.color = Color.red;
                            Debug.Log("Checked resultColor red");
                            red += 1;
                        }
                        else if (inputEachHeight <= inputHeightValue + inputHeightValue * 0.8f && inputEachHeight > inputHeightValue + inputHeightValue * 0.6f)
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0f, 1); // 주황
                            Debug.Log("Checked resultColor orange");
                            orange += 1;
                        }
                        else if (inputEachHeight <= inputHeightValue + inputHeightValue * 0.6f && inputEachHeight > inputHeightValue + inputHeightValue * 0.4f)
                        {
                            obj.GetComponent<Renderer>().material.color = Color.yellow;
                            Debug.Log("Checked resultColor yellow");
                            yellow += 1;
                        }
                        else if (inputEachHeight <= inputHeightValue + inputHeightValue * 0.4f && inputEachHeight > inputHeightValue + inputHeightValue * 0.2f)
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(0.8f, 1f, 0f, 1f);
                            Debug.Log("Checked resultColor yellow");
                            yellowGreen += 1;
                        }
                        else // 고도가 같을 경우 
                        {
                            obj.GetComponent<Renderer>().material.color = Color.green;
                            Debug.Log("Checked resultColor green");
                            green += 1;
                        }

                    }
                    else if (eachHeight.y == setHeiht_)
                    {
                        whiteColor.a = 0.1f;
                        obj.GetComponent<Renderer>().material.color = whiteColor;
                        yellow += 1;
                    }
                    else
                    {
                        float inputEachHeight = eachHeight.y;
                        float inputHeightValue = setHeiht_;

                        if (inputEachHeight < inputHeightValue && inputEachHeight >= inputHeightValue - inputHeightValue * 0.2)
                        {
                            obj.GetComponent<Renderer>().material.color = Color.blue;
                            Debug.Log("Checked resultColor blue");
                            blue += 1;
                        }
                        else if ((inputEachHeight < inputHeightValue - inputHeightValue * 0.4) && (inputEachHeight >= inputHeightValue - inputHeightValue * 0.6))
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(0.2f, 0f, 0.7f, 1);  // 남색
                            Debug.Log("Checked resultColor navy");
                            darkBlue += 1;
                        }
                        else if ((inputEachHeight < inputHeightValue - inputHeightValue * 0.6) && (inputEachHeight >= inputHeightValue - inputHeightValue * 0.8))
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(0.3f, 0f, 0.3f, 1); // purple
                            Debug.Log("Checked resultColor purple");
                            purple += 1;
                        }
                        else
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(0.01f, 0.09f, 0.45f, 1); // darkblue
                            //obj.GetComponent<Renderer>().material.color = Color.; //black darkblue
                            Debug.Log("Checked resultColor deep color");
                            deepPurple += 1;
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

                    //Debug.Log("setHeight_ : " + setHeiht_);


                    float setHeight__ = setHeightSliderInput.value;
                    //Debug.Log("sliderInput : " + setHeight__);

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
                            red += 1;
                        }
                        else if (inputEachHeight <= inputHeightValue + inputHeightValue * 0.8f && inputEachHeight > inputHeightValue + inputHeightValue * 0.6f)
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0f, 1); // 주황
                            //Debug.Log("Checked resultColor 주황");
                            orange += 1;
                        }
                        else if (inputEachHeight <= inputHeightValue + inputHeightValue * 0.6f && inputEachHeight > inputHeightValue + inputHeightValue * 0.4f)
                        {
                            obj.GetComponent<Renderer>().material.color = Color.yellow;
                            //Debug.Log("Checked resultColor yellow");
                            yellow += 1;
                        }
                        else if (inputEachHeight <= inputHeightValue + inputHeightValue * 0.4f && inputEachHeight > inputHeightValue + inputHeightValue * 0.2f)
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(0.8f, 1f, 0f, 1f);
                            //Debug.Log("Checked resultColor yellowGreen");
                            yellowGreen += 1;
                        }
                        else // 고도가 같을 경우 
                        {
                            obj.GetComponent<Renderer>().material.color = Color.green;
                            //Debug.Log("Checked resultColor green");
                            green += 1;
                        }

                    }
                    else if (eachHeight.y == setHeiht_)
                    {
                        whiteColor.a = 0.1f;
                        obj.GetComponent<Renderer>().material.color = whiteColor;
                        yellow += 1;
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
                        }
                        else if ((inputEachHeight < inputHeightValue - inputHeightValue * 0.4) && (inputEachHeight >= inputHeightValue - inputHeightValue * 0.6))
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(0.2f, 0f, 0.7f, 1);  // 남색
                            //Debug.Log("Checked resultColor navy");
                            darkBlue += 1;
                        }
                        else if ((inputEachHeight < inputHeightValue - inputHeightValue * 0.6) && (inputEachHeight >= inputHeightValue - inputHeightValue * 0.8))
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(0.3f, 0f, 0.3f, 1); // purple
                            //Debug.Log("Checked resultColor purple");
                            purple += 1;
                        }
                        else
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(0.01f, 0.09f, 0.45f, 1); // darkblue
                            //obj.GetComponent<Renderer>().material.color = Color.; //black darkblue
                            //Debug.Log("Checked resultColor deep color");
                            deepPurple += 1;
                        }
                    }
                }
            }
            //get_e_volume = true;
            //get_re_e_volume();
            //checkNum += 1;
        }
        //red, orange, yellow, yellowGreen, green, blue, darkBlue, purple, deepPurple
        //Debug.Log("color counter check : " + red);
        float diggingArea = red + orange + yellow + yellowGreen;
        Debug.Log("red + orange + yellow + yellowGreen :" + diggingArea);
        float pillingArea = green + blue + darkBlue + purple;
        Debug.Log("green + blue + darkBlue + purple :" + pillingArea);
        float totalArea = red + orange + yellow + yellowGreen + green + blue + darkBlue + purple + deepPurple;
        Debug.Log("color counter check totalArea : " + totalArea);

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
        Debug.Log("digVolumeCal :" + digVolumeCal);
        Debug.Log("pileVolumeCal :" + pileVolumeCal);

        // cal time duration to pile on the work area  : pilling color to yellow color
        string getExcavatorInput = excavatorInput.text;
        float getExcavatorInput_ = float.Parse(getExcavatorInput) * 0.2f;

        string getTruckInput = truckInput.text;
        float getTruckInput_ = float.Parse(getTruckInput) * 0.1f; // 0.4f라는 비율로 1대의 excavator에 트럭 비율 적용할 경우


        digArea.text = "Digging area ratio : " + ((diggingArea / totalArea)*100).ToString() + "%";
        pileArea.text = "Pilling area ratio : " + ((pillingArea / totalArea)*100).ToString() + "%";

        digFlatArea.text = "Digging area : " + diggingArea + "㎡";
        pileFlatArea.text = "Pilling area : " + pillingArea +  "㎡";

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




    // Update is called once per frame
    void Update()
    {

    }
}