using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_Btn : MonoBehaviour
{
    public TerrainHeightCalculator copyFuntion;





    public void copyToTerrainDataToAnotherTerrain()
    {
        copyFuntion.CopyTerrainData();
    }

    public void ResetTerrainBtn()
    {
        copyFuntion.ResetTerrain();
    }

    // get the volume of earthwork : Terrain_A - Terrrain_B  
    public void GetTheVolumeOfTerrainBtn()
    {
        copyFuntion.GetTheVolumeOfTerrain();


    }

}
