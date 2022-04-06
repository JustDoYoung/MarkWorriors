using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndeadKnight_Meteor : MonoBehaviour
{
    public int damage = 2;
    public float kAdjust = 0.5f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Shake(1);
            Interaction.Attack attack = other.gameObject.GetComponent<Interaction.Attack>();
            attack.OnAttackHit(damage);
        }
    }
    public void Shake(float time)
    {
        StartCoroutine("IEShake", time);
    }
    IEnumerator IEShake(float time)
    {
        Vector3 origin = Vector3.zero;
        //time초 동안 화면을 흔들고 싶다.
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            Camera.main.transform.localPosition = origin + Random.insideUnitSphere * kAdjust; //1m짜리 구의 랜덤 점 하나를 찍는다.
            yield return 0;
        }
        //다 흔들고 나면 원래 위치로 돌려놓고 싶다.
        Camera.main.transform.localPosition = Vector3.zero;
    }
}
