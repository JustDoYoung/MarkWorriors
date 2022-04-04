using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SswPlayerEffector : MonoBehaviour
{
    public GameObject levelUpEffectFactory;
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

    public void LevelUp(){
        GameObject effect = Instantiate(levelUpEffectFactory, transform);
        effect.transform.position += Vector3.down;
        Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
    }

    public void AttackEffect(Vector3 position){
        Instantiate(hitEffectFactory, position, Quaternion.identity);
    }

    public void RecoveryHpEffect(){
        GameObject effect =  Instantiate(recoveryHpEffectFactory, transform);
        effect.transform.position += Vector3.down;
        Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
    }
}
