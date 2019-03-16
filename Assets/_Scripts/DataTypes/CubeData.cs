using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CubeData
{

    public int type;
    public ElemData[] sides = new ElemData[4];
    
   
   

}

[System.Serializable]
public struct ElemData
{
    public int sideMat;
    public int[] elemColors;
}

