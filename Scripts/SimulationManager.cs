using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SimulationManager : MonoBehaviour
{

    private RaycastHit hit_click;

    public GameObject Flag;

    public AnalysisOfTerrainAreaHL AnalysisOfTerrainAreaHL_;


    // Start is called before the first frame update
    void Start()
    {
        
    }


    // 가장 가까운 게임오브젝트 찾기 
    private GameObject FindNearestObjectByTag(string tag, GameObject inputObj)
    {
        // 탐색할 오브젝트s 목록을 List 로 저장
        var objects = GameObject.FindGameObjectsWithTag(tag).ToList();

        // LINQ 메소드를 이용해 가장 가까운 게임오브젝트 
        var neareastObject = objects
            .OrderBy(obj =>
            {
               
                return Vector3.Distance(inputObj.transform.position, obj.transform.position);

            })
        .FirstOrDefault();

        return neareastObject;
    }

    // Get Color32 value of GameObject
    public Color32 checkColVal(GameObject inpObj)
    {
        Color32 objColor;
        objColor = inpObj.GetComponent<MeshRenderer>().material.color;

        //Debug.Log("iden_color :" + objColor);
        return objColor;
    }



    public float timer = 0.0f;
    public float waitingTime = 5f;

    public List<GameObject> selected_cube_obj = new List<GameObject>();
    private GameObject[] allCube;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            int layerMask = 1 << LayerMask.NameToLayer("GioCube");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit_click, Mathf.Infinity, layerMask))
            {
                string objectName = hit_click.collider.gameObject.name;
                //Debug.Log("Hit click cube :" + hit_click.transform.gameObject);
                //Debug.Log("hitted obj name :" + objectName);
                //Debug.Log("Hit click cube position :" + hit_click.transform.gameObject.transform.position);

                // get clicked color
                Color32 objColor__;
                var objClicked = hit_click.collider.gameObject;
                objColor__ = objClicked.GetComponent<MeshRenderer>().material.color;
                Debug.Log("Clicked objColor__ :" + objColor__);


                //클릭한 오브젝트의 색을 규칙에 의해서 변경시킨다. 여기서부터 작업시작하게됨!
                //StartCoroutine(changeColorTiles(objClicked));

                // change TAG name of click obj , 클릭한 자신의 오브젝트는 가까운 거리계산에서 제외 
                //hit_click.transform.gameObject.tag = "GidCube_";




                //// 1.클릭한 오브젝트에서 가장 가까운 오브젝트를 찾는다. objClicked
                //GameObject nerObj = FindNearestObjectByTag("GidCube", objClicked);
                //Debug.Log("클릭한 오브젝트에서 가장 가까운 오브젝트 :" + nerObj);

                //// 2.클릭한 오브젝트의 가장 가까운 오브젝트의 색깔값을 추출한다. 변경전에 추출한다.
                //Color32 colVal_nerObj = checkColVal(nerObj);
                //Debug.Log("클릭한 오브젝트에서 가장 가까운 오브젝트의 색깔값 : " + colVal_nerObj);

                //// 3. 오브젝트의 색을 변경시킨다.
                //StartCoroutine(changeColorTiles(nerObj));




                // 4. 색이 변경된 오브젝트에서 다른 가장 가까운 오브젝트를 찾는다. 색을 비교해서 변경전과 같은 색일 경우, 색을 변경하고 아니면 다시 가까운 게임오브젝트를 찾는다. 다 찾으면 멈춘다.

                // 오브젝트 변경전의 색과 처음 클릭했던 오브젝트의 색이 같지 않을때는 색을 바꾸도록 한다 

                // 선택된 색의 타일 게임오브젝트 추출 후 리스트에 담기
                List<GameObject> nObjList = new List<GameObject>();
                // 모든 타일 오브젝트를 리스트에 담기 
                allCube = GameObject.FindGameObjectsWithTag("GidCube");

                // 반복문으로 비교하여 변경전 추출한 게임오브젝트의 색과 같다면, 변경해야 할 색이기 때문에 리스트에 담는다, 단 거리순 순서는 정렬안된다 그래서 거리순으로 정렬이 필요하다 
                foreach (GameObject each_obj in allCube)
                {
                    Color32 objColor;
                    objColor = each_obj.GetComponent<MeshRenderer>().material.color;
                    if (objColor__.Equals(objColor))
                    {
                        nObjList.Add(each_obj);
                    }
                }


                // 선택= Mathf.Infinity;  게임오브젝트 우선 변경 
                changeColorTiles(objClicked);

                // 오브젝트 리스트 개수 
                int NumsObjList = nObjList.Count();

                // 오브젝트 리스트 개수, 선택한 오브젝트, 오브젝트 리스트
                StartCoroutine(procedualColorChanger(NumsObjList, objClicked, nObjList));


                // nObjList 에서 최초에 선택했던 게임오브젝트 objClicked와 가까운 순서대로 리스트를 새롭게 정렬한다
                // 정렬한 리스트를 순서대로 색을 바꿔나가면서 표시를 하면 작업순서가 결정된다

                //// 가장 가까운 순서대로 게임오브젝트 리스트로 정렬하기,하지만 가장 가까운 오브젝트만 선택하여 색을 변경함 
                //var nerestList = nObjList.OrderBy(
                //    x => Vector2.Distance(transform.position, x.transform.position)
                //    ).ToList();



                //foreach (GameObject ii in nObjList)
                //{
                //    Debug.Log("정렬 전 리스트 : " + ii.transform.position);
                //}

                //foreach (GameObject kk in nerestList)
                //{
                //    Debug.Log("정렬 후 리스트 : " + kk.transform.position);
                //}



                //// selected_cube_obj = nerestList.ToList();

                //// 가장 가까운 한개의 게임오브젝트만 변경 
                //changeColorTiles(nerestList[0]);

                //// 변경할 리스트에서 변경한 리스트 제외시키기
                //nerestList.Remove(nerestList[0]);


                //// 제외할 리스트가 없을때까지 반복, 즉 모든 것을 변경 반복
                //if (null != nerestList)
                //{
                //    // 변경한 게임오브젝트에서 다시 가장 가까운 게임오브젝트 리스트로 반환 
                //    var reList = FindNerObj(nerestList);

                //    for (int i = 0; i < reList.Count; i++)
                //    {

                //        // 왜 모든 게임오브젝트의 색이 변화하지 않는?????

                //        // 색 변경하기 
                //        ChangeColorEachItm(reList[i]);
                //        i += 1;

                //        //Debug.Log("change color tiles... !");
                //        Debug.Log("check obj tiles : " + reList[i].transform.position);


                //        // 변경한 후 리스트에서 제외한.
                //        //nerestList.Remove(nerestList[i]);
                //        //Debug.Log("remove obj!");
                //    }
                //    //// 변경한 후 리스트에서 제외한.
                //    //nerestList.Remove(nerestList[0]);
                //    //Debug.Log("remove obj!");
                //}

                


                // 클릭으로 선택된 동일한 색의 게임오브젝트를 모두 바꾸는 하지만 순차자적 방법
                //StartCoroutine(ChangeColors(nerestList));



                //Debug.Log("ner obj position :" + nerObj.transform.position);

                // 가장 가까운 obj를 찾게되면 색을 정해진 규칙에 따라 변경해 나감
                // 1)obj들 중에서 같은색 오브젝트만 추출, 우선 obj의 색을 추출한다
                // 안됨 -> Color32 iden_color = AnalysisOfTerrainAreaHL_.getGameobjectColor(nerObj);



                // 같은 색의 오브젝트를 찾아내어 색이 변경될때까지 진행함, 동시에 아래 코드로 깃발 추가로 시각(일)

                // 1)같은 색의 오브젝트를 찾아내어 새로운 리스트에 담기
                //List<GameObject> nObjList = new List<GameObject>();



                ////리스트에서 가장 가까운 게임오브젝트의 색을 흰색이 될 때까지 변경시키기 
                //foreach (GameObject obj_ in nObjList)
                //{
                //    // 색을 비교하여 1차 변화시키, 향후 반복실행하여 모두 화이트로 !
                //    StartCoroutine(changeColorTiles(obj_));

                //}



                ///// 플래그를 생성함 - 작업순서를 정보 제공 
                //if (null == GameObject.FindGameObjectWithTag("Flag"))
                //{
                //    // Instatiate flag gameobject on the positon
                //    Instantiate(Flag, nerObj.transform.position, Flag.transform.rotation);

                //    // 원래대로 tag name 변환시켜줌
                //    //hit_click.transform.gameObject.tag = "GidCube";
                //}
                //else
                //{
                //    Destroy(GameObject.FindGameObjectWithTag("Flag"), 0.1f);
                //}

            }
        }
 
    }


    private GameObject nerObj;
    public float range = 20f; // 30m를 작업 범위로 지3

    IEnumerator procedualColorChanger(int ListNum, GameObject inpObj, List<GameObject> objList)
    {
        Debug.Log("변환 시작");

        while (ListNum > 0)
        {
            //Debug.Log("입력시 총 리스트 개수 : " + objList.Count());

            //Debug.Log("새로 입력하는 오브젝트 : " + nerObj);

            //Debug.Log("게임오브젝트 갯수 : " + ListNum); // 15 -> 1 까지 순차적으로 줄어듬 
            //objList.Remove(inpObj);

            //float shortDist = Mathf.Infinity; // 가장 짧은 거리를 무한으로 설정
            // --------------------

            float shortDist = Mathf.Infinity;

            if (null != objList)
            {
                foreach (GameObject fObj in objList)
                {
                    float Distance_ = Vector3.Distance(inpObj.transform.position, fObj.transform.position);
                    if (Distance_ < shortDist)
                    {
                        shortDist = Distance_;
                        nerObj = fObj;
                        //shortDist = Distance_;
                    }

                }


                //Debug.Log(nerObj);
                //nerObj.GetComponent<Renderer>().material.color = Color.green;
                // --------------------
                StartCoroutine(changeColorTiles(nerObj));

                // Arrow instantiate
                ArrowVector(Flag, inpObj, nerObj);
            }


            //Debug.Log("삭제할 오브젝트 : " + nerObj);

            objList.Remove(nerObj);

            //Debug.Log("삭제한 오브젝트: " + nerObj);

            // 리스트에 값 존재여부 확인
            //if (objList.Contains(nerObj))
            //{
            //    Debug.Log("{0}을 찾았습니다." + nerObj);
            //}
            //else
            //{
            //    Debug.Log("{0}을 찾지 못했습니다." + nerObj);
            //}
            


            inpObj = nerObj; // 결과값을 입력값으로 재입력

            //Debug.Log("SelectedObj :" + inpObj);

            ListNum--;


            //Debug.Log("출력시 총 리스트 개수 : " + objList.Count());

            yield return new WaitForSeconds(0.0f);

        }
        Debug.Log("변경작업 끝!");
    }





    // 가장 가까운 게임오브제트를 순서대로 정렬하여 리스트로 반환
    private List<GameObject> FindNerObj(List<GameObject> inp_list)
    {

        var nerestList = inp_list.OrderBy(
                   x => Vector2.Distance(transform.position, x.transform.position)
                   ).ToList();


        return nerestList;
    }

    private IEnumerator ChangeColorEachItm(GameObject inputObj)
    {
        StartCoroutine(changeColorTiles(inputObj));
        yield return new WaitForSeconds(0.0f);
    }


    private IEnumerator ChangeColors(List<GameObject> nerestList)
    {
        // 정렬된 리스트에서 하나씩 꺼내어서 색을 바꿔나간다 
        foreach (GameObject nObj in nerestList)
        {

            StartCoroutine(changeColorTiles(nObj));

            //Debug.Log("ChangeColors timmmmmmer");

            yield return new WaitForSeconds(0.0f);

        }
    }

    //IEnumerator ChangeColors(inputList)
    //{
    //    // 정렬된 리스트에서 하나씩 꺼내어서 색을 바꿔나간다 
    //    foreach (GameObject nObj in inputList)
    //    {
    //        //timer += Time.deltaTime;
    //        //if (timer > waitingTime)
    //        //{
    //        //    timer = 0f;
    //        //    Debug.Log("timmmmmmer");
    //        //}

    //        StartCoroutine(changeColorTiles(nObj));

    //        //Debug.Log("ChangeColors timmmmmmer");

    //        yield return new WaitForSeconds(0.0f);




    //    }

    //}

    


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



    IEnumerator changeColorTiles(GameObject inpObj)
    {
        //Debug.Log("Changed color of tiles.. based on clicked!");
        Color32 get_obj_color_ = inpObj.GetComponent<MeshRenderer>().material.color;
        //Debug.Log("input_obj color :" + get_obj_color_);
        Color32 green_ = new Color32(0, 255, 0, 255);
        Color32 yellowGreen_ = new Color32(255, 235, 4, 255);
        Color32 liteGreen_ = new Color32(204, 255, 0, 255);
        Color32 orange_ = new Color32(255, 128, 0, 255);
        Color32 red_ = new Color32(255, 0, 0, 255);
        Color32 blue_ = new Color32(0, 0, 255, 255);
        Color32 darkBlue_ = new Color32(51, 0, 178, 255);
        Color32 litePurple_ = new Color32(76, 0, 76, 255);
        Color32 deepPurple_ = new Color32(3, 23, 115, 255);



        //if (get_obj_color_.Equals(green_)) // RGBA(0, 255, 0, 255)
        //{
        //    Debug.Log("change green to white" + get_obj_color_);
        //    inpObj.GetComponent<Renderer>().material.color = Color.white;
        //}
        // 이 부분을 각 색으로 변경해야 함 Color.green -> yellowGreen의 RGB 값으로 이하 적 // red > orage
        if (get_obj_color_.Equals(yellowGreen_)) // yellowGreen > green
        {
            Debug.Log("change yellowGreen to green");
            inpObj.GetComponent<Renderer>().material.color = Color.green;
            // 플래그를 생성함 - 작업순서를 정보 제공 
            //Instantiate(Flag, inpObj.transform.position, Flag.transform.rotation);

        }

        if (get_obj_color_.Equals(liteGreen_)) // liteGreen > white
        {
            Debug.Log("change yellowGreen to green");
            inpObj.GetComponent<Renderer>().material.color = Color.white;
            // 플래그를 생성함 - 작업순서를 정보 제공 
            //Instantiate(Flag, inpObj.transform.position, Flag.transform.rotation);

        }


        if (get_obj_color_.Equals(orange_)) // orange > yellow
        {
            Debug.Log("change orange to yellow");
            inpObj.GetComponent<Renderer>().material.color = Color.yellow;
            // 플래그를 생성함 - 작업순서를 정보 제공 
            //Instantiate(Flag, inpObj.transform.position, Flag.transform.rotation);

        }

        if (get_obj_color_.Equals(red_)) // red > orange
        {
            Debug.Log("change red to orange");
            inpObj.GetComponent<Renderer>().material.color = orange_;
            // 플래그를 생성함 - 작업순서를 정보 제공 
            //Instantiate(Flag, inpObj.transform.position, Flag.transform.rotation);

        }

        if (get_obj_color_.Equals(green_)) // red > orange
        {
            Debug.Log("change green to white");
            inpObj.GetComponent<Renderer>().material.color = Color.white;
            // 플래그를 생성함 - 작업순서를 정보 제공 
            //Instantiate(Flag, inpObj.transform.position, Flag.transform.rotation);

        }


        if (get_obj_color_.Equals(blue_)) // blue > white
        {
            Debug.Log("change blue to white");
            inpObj.GetComponent<Renderer>().material.color = Color.white;
            // 플래그를 생성함 - 작업순서를 정보 제공 
            //Instantiate(Flag, inpObj.transform.position, Flag.transform.rotation);

        }

        if (get_obj_color_.Equals(darkBlue_)) // darkBlue > blue
        {
            Debug.Log("change darkblue to blue");
            inpObj.GetComponent<Renderer>().material.color = Color.blue;
            // 플래그를 생성함 - 작업순서를 정보 제공 
            //Instantiate(Flag, inpObj.transform.position, Flag.transform.rotation);

        }

        if (get_obj_color_.Equals(litePurple_)) // litePurple > blue
        {
            Debug.Log("change litePruple to darkBlue");
            inpObj.GetComponent<Renderer>().material.color = darkBlue_;
            // 플래그를 생성함 - 작업순서를 정보 제공 
            //Instantiate(Flag, inpObj.transform.position, Flag.transform.rotation);

        }

        if (get_obj_color_.Equals(deepPurple_)) // deepPurple > litePurple
        {
            Debug.Log("change deepPurple to litePurple");
            inpObj.GetComponent<Renderer>().material.color = litePurple_;
            // 플래그를 생성함 - 작업순서를 정보 제공 
            //Instantiate(Flag, inpObj.transform.position, Flag.transform.rotation);

        }

        else if (get_obj_color_.Equals(Color.white))
        {
            Debug.Log("no change beacuse already white");
            inpObj.GetComponent<Renderer>().material.color = Color.white;
            // 플래그를 생성함 - 작업순서를 정보 제공 
            //Instantiate(Flag, inpObj.transform.position, Flag.transform.rotation);

        }

        yield return new WaitForSeconds(0.0f);
    }




    // ArrowObj의 방향을 설정하는 메소드 
    public void ArrowVector(GameObject ArrowObj, GameObject startObj, GameObject targhetObj)
    {
        Debug.Log("Instantiate Arrow!");
        Vector3 dir = targhetObj.transform.position - startObj.transform.position;
        dir.y = 0f;
        Quaternion rot = Quaternion.LookRotation(dir.normalized);
        //ArrowObj.transform.rotation = rot;

        Instantiate(ArrowObj, startObj.transform.position, ArrowObj.transform.rotation = rot);

    }
}
