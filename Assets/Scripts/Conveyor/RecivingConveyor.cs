using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecivingConveyor : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public GameObject[] prefabs;
    private bool isRunning = true;
    public GameObject triggerPrefabs;

    private int prefabLength; // Get items type corresponding to level
    //public Shelf shelf;
    private RecivingTrigger[] triggers;
    private class ItemOnBelt
    {
        public GameObject item;
        public bool isFreeze;
        public RecivingTrigger trigger;

        public ItemOnBelt(GameObject item, RecivingTrigger trigger)
        {
            this.item = item;
            this.isFreeze = false;
            this.trigger = trigger;
        }
    }
    private List<ItemOnBelt> onBelt;
    private const int MAX_LOAD = 5;
    private const float waitTime = 3.0f; // time between creation
    private IEnumerator coroutine;

    // check whether item reach the destination
    public void checkDest(RecivingTrigger trigger, GameObject item)
    {
        // check if item is in onBelt list
        for (int i = 0; i < onBelt.Count; i++)
        {
            if (onBelt[i].item == item)
            {
                if (onBelt[i].trigger == trigger)
                {
                    // item reached destination, stop movement
                    onBelt[i].isFreeze = true;
                }
                break;
            }
        }
    }

    // remove item from onBelt list
    public void takeItem(GameObject item)
    {
        // check if item is in onBelt list
        for (int i = 0; i < onBelt.Count; i++)
        {
            if (onBelt[i].item == item)
            {
                for (int j = 0; j < triggers.Length; j++)
                {
                    if (onBelt[i].trigger == triggers[j])
                    {
                        triggers[j].isUsed = false;
                        updateDest(j, i);
                    }
                }
                onBelt.Remove(onBelt[i]);

                if ((onBelt.Count == MAX_LOAD - 1) && !isRunning)
                {
                    Invoke("run", waitTime);
                }
                break;
            }
        }
    }

    // set up for running
    private void Start()
    {
        onBelt = new List<ItemOnBelt>();
        triggers = new RecivingTrigger[MAX_LOAD];
        prefabLength = Camera.main.GetComponent<gameController>().getMaxItem(prefabs.Length);

        // set up triggers
        for (int i = 0; i < MAX_LOAD; i++)
        {
            GameObject item = GameObject.Instantiate(triggerPrefabs);

            item.transform.position += new Vector3(i * 2.0f + 1, 0, 0);
            triggers[i] = item.GetComponent<RecivingTrigger>();
            triggers[i].isUsed = false;
            triggers[i].parent = this;
        }

        Invoke("delayStart", 3.0f);
    }

    private void delayStart()
    {
        // call for create random item
        coroutine = CreateRandomItem();
        StartCoroutine(coroutine);
    }

    private void Update()
    {
        // test only
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     takeItem(onBelt[1].item);
        // }
        // If in countdown, do nothing
        if (Camera.main.GetComponent<gameController>().inCountDown())
        {
            return;
        }

        // move item toward destination trigger
        float step = speed * Time.deltaTime;
        for (int i = 0; i < onBelt.Count; i++)
        {
            if (!onBelt[i].isFreeze)
            {
                onBelt[i].item.transform.position = Vector3.MoveTowards(onBelt[i].item.transform.position, onBelt[i].trigger.transform.position, step);
            }
        }
    }

    private void Stop()
    {
        isRunning = false;
        StopCoroutine(coroutine);
    }

    private void run()
    {
        isRunning = true;
        StartCoroutine(coroutine);
    }

    // set item's destination at the end, if no free slot, will the start of conveyor
    private RecivingTrigger toEnd()
    {
        // check which trigger is the current end
        for (int i = (triggers.Length - 1); i > 0; i--)
        {
            if (!triggers[i].isUsed)
            {
                triggers[i].isUsed = true;
                return triggers[i];
            }
        }
        return triggers[0];
    }

    // index refer to the trigger of removed item
    private void updateDest(int triggerindex, int itemIndex)
    {

        // move all link by one index
        if (onBelt.Count == 1)
        {
            //triggers[triggerindex].isUsed = false;
            return;
        }

        for (int i = itemIndex + 1; i < onBelt.Count; i++)
        {
            onBelt[i].trigger.isUsed = false;
            onBelt[i].trigger = toEnd();
        }
    }

    // create random item from prefabs
    private IEnumerator CreateRandomItem()
    {
        while (isRunning)
        {
            // pick a random item from prefabs 
            GameObject prefab = prefabs[Random.Range(0, prefabLength)];

            // create a new item at current position
            GameObject item = GameObject.Instantiate(prefab);
            item.transform.position = triggers[0].transform.position;

            // add require dependency to item
            Item it = item.GetComponent<Item>();
            //it.shelf = shelf;

            // add item to onBelt list
            ItemOnBelt itemOnBelt = new ItemOnBelt(item, toEnd());
            onBelt.Add(itemOnBelt);

            //TODO: Assign name and reward to the item

            // check if maxload is reached
            if (onBelt.Count == MAX_LOAD)
            {
                Stop();
            }

            // call for next item creation
            yield return new WaitForSeconds(waitTime);
        }
        yield return null;
    }
}
