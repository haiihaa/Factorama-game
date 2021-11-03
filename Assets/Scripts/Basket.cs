using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    public StoringConveyor conveyor;
    public GameObject item;
    private bool empty = true;
    private bool removable = false;
    public void PlaceItem()
    {
        empty = false;
    }

    public bool isEmpty()
    {
        return empty;
    }

    static public Basket FindBasket()
    {
        // Find all baskets in scene
        GameObject[] allBaskets = GameObject.FindGameObjectsWithTag("Basket");
        List<GameObject> baskets = new List<GameObject>();

        // Remove baskets that are lower than the 6.5 on the Z axis
        // Workers can access baskets that are empty on the vertical and lower horizontal conveyer belt
        foreach (GameObject obj in allBaskets)
        {
            if (obj.transform.position.z < 3.0f)
            {
                baskets.Add(obj);
            }
        }

        if (baskets.Count == 0)
        {
            // Baskets Available
            print("No Basket Available");
            return null;
        }

        // Find the basket at the very first on belt
        //Debug.Log(string.Format("From Basket: baskets on belt is: {0}", baskets.Length));
        for (int i = 0; i < baskets.Count; i++)
        {
            Basket basket = baskets[i].GetComponent<Basket>();
            if (basket.isEmpty())
            {
                return basket;
            }
        }
        // If no available basket, return null
        return null;
    }

    private void OnDestroy()
    {
        if (!empty)
        {
            Destroy(item);
        }
    }

    public void SendToPack(GameObject item)
    {
        this.item = item;
        conveyor.move();
    }

    public void updateRemovedItem()
    {
        //Debug.Log("From Basket: removed item from basket!");
        item = null;
        removable = true;
        empty = true;
    }

    public bool isRemovable()
    {
        return removable;
    }
}
