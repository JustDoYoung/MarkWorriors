using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject bulletFactory;
    public int maxCount = 20;

    private readonly Dictionary<string, List<GameObject>> activeListDictionary = new Dictionary<string, List<GameObject>>();
    private readonly Dictionary<string, Queue<GameObject>> deactiveQueueDictionary = new Dictionary<string, Queue<GameObject>>();

    public static ObjectPool instance;

    private void Awake()
    {   
        instance = this;
        CreateInstance("BulletFactory", maxCount);
        transform.SetSiblingIndex(transform.hierarchyCount);
    }

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Queue<GameObject> GetDeactiveQueue(string key){
        if(deactiveQueueDictionary.ContainsKey(key) == false){
            deactiveQueueDictionary.Add(key, new Queue<GameObject>());
        }
        return deactiveQueueDictionary[key];
    }

    private List<GameObject> GetActiveList(string key){
        if(activeListDictionary.ContainsKey(key) == false){
            activeListDictionary.Add(key, new List<GameObject>());
        }
        return activeListDictionary[key];
    }

    public void CreateInstance(string prefabName, int amount){
        GameObject rootObj = new GameObject(prefabName);
        rootObj.transform.parent = transform;

        string key = prefabName;
        Queue<GameObject> deactiveQueue = GetDeactiveQueue(key);
        GameObject objectFactory = (GameObject) Resources.Load<GameObject>(key);

        for(int i=0; i<amount; i++){
            GameObject gameObject = Instantiate(objectFactory);
            gameObject.SetActive(false);
            gameObject.transform.parent = rootObj.transform;
            int index = gameObject.name.IndexOf("Factory(Clone)");
            if(index > 0){
                gameObject.name = gameObject.name.Substring(0, index);
            }
            deactiveQueue.Enqueue(gameObject);
        }
    }

    public GameObject GetAvailableObject(string key = "BulletFactory"){
        Queue<GameObject> deactiveQueue = GetDeactiveQueue(key);
        List<GameObject>  activeList =  GetActiveList(key);

        if(deactiveQueue.Count <= 0){
            return null;
        }

        GameObject gameObject = deactiveQueue.Dequeue();
        activeList.Add(gameObject);
        gameObject.SetActive(true);
        return gameObject;
    }   

    public void ReturnObject(GameObject targetObj, string key = "BulletFactory"){
        Queue<GameObject> deactiveQueue = GetDeactiveQueue(key);
        List<GameObject>  activeList =  GetActiveList(key);

        if(deactiveQueue.Contains(targetObj)){
            return;
        }
        targetObj.SetActive(false);
        activeList.Remove(targetObj);
        deactiveQueue.Enqueue(targetObj);
    }

    public static bool HasGameObject(GameObject gameObject, string key = "BulletFactory"){
        List<GameObject> activeList = ObjectPool.instance.GetActiveList(key);
        return activeList.Contains(gameObject);
    }

    // public GameObject GetInactiveBulletOld(){
    //     foreach(GameObject obj in list){
    //         if(obj.activeSelf){
    //             continue;
    //         }

    //         obj.SetActive(true);
    //         return obj;
    //     }

    //     return null;
    // }
}
