using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolLocation : MonoBehaviour
{
    public static PatrolLocation instance;
    private void Awake()
    {
        PatrolLocation.instance = this;
    }
    public GameObject[] patrolPoints;
}
