using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoringConveyor : MonoBehaviour
{
    public GameObject basketPrefab;
    public Conveyor[] conveyors;
    public Vector3 startPos; // where basket will be generate

    private const int maxOnBelt = 8;
    private int currOnBelt = 3;

    // Update is called once per frame
    void Update()
    {
        // Use this when conveyor belt is full but is movable cuz the last basket is removable
        if (Input.GetKeyDown(KeyCode.Space))
        {
            move();
        }
        // Check if the belt is full but it's still movable
        // when the last basket is removable
        // if (currOnBelt == maxOnBelt)
        // {
        //    move();
        // }
    }

    public void move()
    {
        if (isMovable())
        {
            // move all basket in conveyors
            turnOn();
            Invoke("turnOff", 2.0f);
        }
    }

    private void turnOn()
    {
        for (int i = 0; i < conveyors.Length; i++)
        {
            conveyors[i].isRunning = true;
        }
    }

    private void turnOff()
    {
        for (int i = 0; i < conveyors.Length; i++)
        {
            conveyors[i].isRunning = false;
        }
        addBasket();
    }

    private void addBasket()
    {
        // Max basket placed on belt now - 
        // wait for pack-worker to handle items before adding more basket
        if (currOnBelt < maxOnBelt)
        {
            // create basket
            GameObject item = GameObject.Instantiate(basketPrefab);
            item.transform.position = startPos;

            // set link
            Basket basket = item.GetComponent<Basket>();
            basket.conveyor = this;
            currOnBelt++;
            //Debug.Log(string.Format("From storingConveyor: baskets on belt is: {0}", currOnBelt));
        }
    }

    public void updateCurrOnBelt()
    {
        currOnBelt--;
    }

    private bool isMovable()
    {
        GameObject[] baskets = GameObject.FindGameObjectsWithTag("Basket");
        //Debug.Log(baskets[0].GetComponent<Basket>().isRemovable());
        if (baskets.Length != 0 && currOnBelt == maxOnBelt && !baskets[0].GetComponent<Basket>().isRemovable())
        {
            return false;
        }
        //Debug.Log("From storingConveyor: belt is movable!");
        return true;
    }
}

