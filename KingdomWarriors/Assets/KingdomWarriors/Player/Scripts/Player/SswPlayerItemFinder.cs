using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SswPlayerItemFinder : MonoBehaviour
{
    private SswPlayerMediator PlayerMediator {get; set;}

    // Start is called before the first frame update
    void Start()
    {
        PlayerMediator = GetComponent<SswPlayerMediator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other) {
        GameItem itemObj = other.gameObject.GetComponent<GameItem>();
         if(itemObj != null && Input.GetKeyDown(KeyCode.E)){
            PickUpItem(itemObj);
         }
    }

    private void PickUpItem(GameItem item){
        print("아아템");
        PlayerMediator.PickUpItem(item);
    }
}
