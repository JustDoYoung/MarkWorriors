using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SswPlayerEffect : MonoBehaviour
{
    public GameObject hitEffectFactory;
    
    public GameObject recoveryHpEffectFactory;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttackEffect(Vector3 position, Quaternion rotation){
        Instantiate(hitEffectFactory, position, rotation);
    }

    public void RecoveryHpEffect(){
        Instantiate(recoveryHpEffectFactory, transform.position, Quaternion.identity);
    }
}
