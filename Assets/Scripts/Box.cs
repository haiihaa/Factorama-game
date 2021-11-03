using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    Item item;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (item != null){
            // follow item
            transform.position = new Vector3(item.transform.position.x, item.transform.position.y, item.transform.position.z);
        }
    }

    public void BoxItem(Item i){
        item = i;
    }
}
