using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameObject playCharacter;

    private void Awake() {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        Cursor.lockState = CursorLockMode.Locked;

        playCharacter = GameObject.Find("Player");

        // 테스트
        StartCoroutine(Test());
    }

    IEnumerator Test(){
        yield return new WaitForSeconds(2f);
        SswCharacterStatus status = playCharacter.GetComponent<SswCharacterStatus>();
        status.SetDamage(3);
        status.SetDamage(-1);
        status.SetDamage(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExpIncrease(int exp){
        if(playCharacter == null){
            return;
        }
        SswCharacterStatus status = playCharacter.GetComponent<SswCharacterStatus>();
        status.ExpIncrease(exp);
    }
}
