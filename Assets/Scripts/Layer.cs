using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : MonoBehaviour
{
    // assume max number of items per layer is 3
    const int MAX_NUM_ITEM = 4;
    private Vector3[] slotPosition;
    private GameObject[] items; 
    public int numFullSlot;

    // Start is called before the first frame update
    void Start()
    {
        slotPosition = new Vector3[MAX_NUM_ITEM] { (new Vector3(-1.5f, 0.1f, 0f) + transform.position), (new Vector3(-0.5f, 0.1f, 0f) + transform.position), (new Vector3(0.5f, 0.1f, 0f) + transform.position), (new Vector3(1.5f, 0.1f, 0f) + transform.position)};
        items = new GameObject[MAX_NUM_ITEM];
        numFullSlot = 0;
    }

    // place item at empty slot, if no empty slot, return false;
    public bool placeItem(GameObject item)
    {
        for (int i = 0; i < MAX_NUM_ITEM; i++)
        {
            if (!items[i])
            {
                item.transform.position = slotPosition[i];
                items[i] = item;
                numFullSlot++;
                return true;
            } 
        }
        return false;
    }

    // find and remove item from layer, if item is not found, return false;
    public bool removeItem(GameObject item) {
        for (int i = 0; i < MAX_NUM_ITEM; i++)
        {
            if (items[i] == item)
            {
                items[i] = null;
                numFullSlot--;
                return true;
            } 
        }
        return false;
    }

    // check if layer is full
    public bool isFull()
    {

        if (numFullSlot == MAX_NUM_ITEM)
        {
            return true;
        }

        return false;
    }
}
