using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { melee }
    public Type type;
    public int damage;
    // Start is called before the first frame update
    private void Awake()
    {
        Application.targetFrameRate = 40;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
