using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public Layer[] layers;

    public GameObject trigger;
    // assume max number of items per layer is 4
    const int MAX_NUM_ITEM = 4;
    private int currentOnShelf = 0;


    // place item from shelf, if no empty space return false;
    public bool placeItem(GameObject item)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            if (!layers[i].isFull())
            {
                layers[i].placeItem(item);
                // Record Item's stat to create order
                orderBar.recordItemOnShelves(item);
                return true;
            }
        }
        return false;
    }

    // find and remove item from shelf, if not found return false;
    public bool removeItem(GameObject item)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].removeItem(item))
            {
                currentOnShelf--;
                return true;
            }
        }
        return false;
    }

    // check if the shelf is full
    public bool isFull()
    {
        return (currentOnShelf == MAX_NUM_ITEM * layers.Length);
    }

    // Find empty shelf
    public static GameObject findEmptyShelf()
    {
        GameObject[] shelves = GameObject.FindGameObjectsWithTag("Shelf");
        for (int i = 0; i < shelves.Length; i++)
        {
            if (!shelves[i].GetComponent<Shelf>().isFull())
            {
                //Debug.Log(currentOnShelf);
                return shelves[i];
            }
        }
        return null;
    }
    
    public void updateCurrOnShelves()
    {
        currentOnShelf++;
    }

}
