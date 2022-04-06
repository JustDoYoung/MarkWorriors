using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class DamageUp : MonoBehaviour
{
    private float moveSpeed;
    private float alphaSpeed;
    private float destroyTime;
    TextMeshPro text;
    Color alpha;
    int damage;
    public int DAMAGE
    {
        get { return damage; }
        set
        {
            damage = value;
        }
    }
    private void Awake()
    {
        moveSpeed = 1.0f;
        alphaSpeed = 1.0f;
        destroyTime = 2.0f;
        text = GetComponent<TextMeshPro>();
    }
    void Start()
    {
        alpha = text.color;
        text.text = DAMAGE.ToString();
        Invoke("DestroyObject", destroyTime);
    }

    void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0)); // 텍스트 위치

        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed); // 텍스트 알파값
        text.color = alpha;
    }
    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
