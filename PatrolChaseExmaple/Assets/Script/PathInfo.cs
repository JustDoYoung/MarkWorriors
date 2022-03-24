using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathInfo : MonoBehaviour
{
    public static PathInfo instance;
    private void Awake()
    {
        PathInfo.instance = this;
        //wayPoints.Add();
    }

    public GameObject[] wayPoints;
    //public GameObject[] wayPoints = new ~;

}
