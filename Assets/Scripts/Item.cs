using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemStatus
    {
        AwaitingWorker,
        BeingHandled,
        StageFinished
    }

    public enum ItemStage
    {
        ReceivingStage,
        PickingStage,
        PackingStage
    }

    public enum ItemWeight
    {
        Light,
        Medium,
        Heavy
    }

    public ItemStatus status = ItemStatus.AwaitingWorker;
    private ItemStage stage = ItemStage.ReceivingStage;
    public ItemWeight weight;
    private Worker des_worker;

    public Shelf shelf;
    public Basket basketScript;
    public Object boxPrefab;
    public int id;
    public int size;
    public bool isPacked;
    // Item's name and price
    public string name;
    private float reward;
    private Vector3 boxSize;
    private float fixedSpd; // Fixed Speed of worker when handle this item
    private float spd; // Speed that gets affected by debuff event

    private GameObject box;


    // Start
    void Start()
    {
        switch (weight)
        {
            case ItemWeight.Light:
                reward = 20;
                fixedSpd = 4.0f;
                boxSize = new Vector3(1.0f, 1.0f, 1.0f);
                break;
            case ItemWeight.Medium:
                reward = 30;
                fixedSpd = 3.0f;
                boxSize = new Vector3(1.5f, 1.5f, 1.5f);
                break;
            default:
                reward = 50;
                fixedSpd = 2.0f;
                boxSize = new Vector3(2.0f, 2.0f, 2.0f);
                break;
        }
        spd = fixedSpd;
    }

    // Update is called once per frame
    void Update()
    {
        switch (stage)
        {
            case ItemStage.ReceivingStage:
                ReceivingLogic();
                break;
            case ItemStage.PickingStage:
                PickingLogic();
                break;
            case ItemStage.PackingStage:
                PackingLogic();
                break;
            default:
                break;
        }
    }

    void ReceivingLogic()
    {
        switch (status)
        {
            case ItemStatus.AwaitingWorker:
                // Temporarily simulating Conveyer Belt
                break;

            case ItemStatus.BeingHandled:
                // Follow Worker
                transform.position = new Vector3(des_worker.transform.position.x, des_worker.transform.position.y + 2.0f, des_worker.transform.position.z);
                break;

            case ItemStatus.StageFinished:
                // Find the storage position on the shelf
                shelf.placeItem(this.gameObject);

                // Advance the stage, reset worker and status
                stage = ItemStage.PickingStage;
                des_worker = null;
                status = ItemStatus.AwaitingWorker;
                break;

            default: break;
        }
    }

    void PickingLogic()
    {
        switch (status)
        {
            case ItemStatus.AwaitingWorker:
                break;

            case ItemStatus.BeingHandled:
                // Follow Worker
                transform.position = new Vector3(des_worker.transform.position.x, des_worker.transform.position.y + 2.0f, des_worker.transform.position.z);
                break;

            case ItemStatus.StageFinished:
                // TODO: Need to add some logic to follow basket until basket leaves the scene
                transform.position = new Vector3(basketScript.transform.position.x, basketScript.transform.position.y + 0.1f, basketScript.transform.position.z);
                basketScript.SendToPack(this.gameObject);

                // Advance the stage, reset worker and status
                stage = ItemStage.PackingStage;
                des_worker = null;
                status = ItemStatus.AwaitingWorker;
                break;

            default: break;
        }
    }

    void PackingLogic()
    {
        switch (status)
        {
            case ItemStatus.AwaitingWorker:
                transform.position = new Vector3(basketScript.transform.position.x, basketScript.transform.position.y + 0.1f, basketScript.transform.position.z);
                break;

            case ItemStatus.BeingHandled:
                // Follow Worker
                transform.position = new Vector3(des_worker.transform.position.x, des_worker.transform.position.y + 2.0f, des_worker.transform.position.z);
                break;

            case ItemStatus.StageFinished:
                Destroy(box);
                Destroy(gameObject);
                break;

            default: break;
        }
    }

    // When Item is Clicked, check for Idle Workers at the designated stage. Then tell them to move to Item.
    void OnMouseDown()
    {
        if (des_worker == null)
        {
            CheckForIdleWorkers();
        }
        if (des_worker != null)
        {
            switch (stage)
            {
                // receiving stage
                case ItemStage.ReceivingStage:
                    GameObject availableShelf = Shelf.findEmptyShelf();
                    if (availableShelf)
                    {
                        // Update curr # of item on shelf: cuz when you click the item successfully,
                        // It must go to shelves, you cant redo
                        shelf = availableShelf.GetComponent<Shelf>();
                        shelf.updateCurrOnShelves();
                        des_worker.moveToItem(this.gameObject); // Tell worker to pick up item
                    }
                    else
                    {
                        StartCoroutine(Camera.main.GetComponent<notiPanel>().showNoti("Storage"));
                        Debug.Log("Storage is currently full!");
                    }
                    break;
                // packing stage
                case ItemStage.PackingStage:
                    if (!statusBar.vanIsFull() && !statusBar.vanOnDelivery())
                    {
                        // Update curr # of item on van
                        statusBar.updateItemOnVan(reward);
                        des_worker.moveToItem(this.gameObject); // Tell worker to pick up item
                    }
                    else if (statusBar.vanOnDelivery())
                    {
                        StartCoroutine(Camera.main.GetComponent<notiPanel>().showOnDeliveryNoti());
                        Debug.Log("Van is currently on delivery!");
                    }
                    else if (statusBar.vanIsFull())
                    {
                        StartCoroutine(Camera.main.GetComponent<notiPanel>().showNoti("Van"));
                        GetComponent<AudioSource>().PlayOneShot(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<gameController>().fullStorage);
                        Debug.Log("Van is currently full!");
                    }
                    break;
                // Else: picking stage
                default:
                    if (!orderBar.checkOrder(name))
                    {
                        // Remind the player to keep track of the orderBar
                        StartCoroutine(Camera.main.GetComponent<notiPanel>().showOrderBarReminder());
                        return;
                    }
                    FindBasket();
                    if (basketScript)
                    {
                        basketScript.PlaceItem();
                        des_worker.moveToItem(this.gameObject); // Tell worker to pick up item
                    }
                    else
                    {
                        StartCoroutine(Camera.main.GetComponent<notiPanel>().showNoti("Belt"));
                        Debug.Log("Conveyor belt is full!");
                    }
                    break;
            }
        }
    }

    public void pickUpItem()
    {
        status = ItemStatus.BeingHandled;
    }

    public void dropItem()
    {
        status = ItemStatus.StageFinished;
    }

    void CheckForIdleWorkers()
    {
        // Find all the workers assigned to current stage
        var workers = new GameObject[0];
        switch (stage)
        {
            case ItemStage.ReceivingStage:
                workers = GameObject.FindGameObjectsWithTag("Worker - Receive");
                break;
            case ItemStage.PickingStage:
                workers = GameObject.FindGameObjectsWithTag("Worker - Pick");
                break;
            case ItemStage.PackingStage:
                workers = GameObject.FindGameObjectsWithTag("Worker - Pack");
                break;
            default: break;
        }

        // Loop through all available workers
        foreach (GameObject obj in workers)
        {
            Worker worker = obj.GetComponent<Worker>();

            // Idle workers can go interact with item
            if (worker.getStatus() == Worker.WorkerStatus.Idle || worker.getStatus() == Worker.WorkerStatus.Walkingback)
            {
                des_worker = worker;
            }
        }

        if (des_worker == null)
        {
            // No Worker is Idle - Play a sound to notify player
            print("No Idle Worker");
            // Fire the notification to the screen
            StartCoroutine(Camera.main.GetComponent<notiPanel>().showNoIdleWorker());
        }
    }

    // Find the Packing Station object according to the item's weight type
    public GameObject GetPackingStation()
    {
        var objs = new GameObject[0];
        switch (weight)
        {
            case ItemWeight.Light:
                objs = GameObject.FindGameObjectsWithTag("Light Packing Station");
                break;
            case ItemWeight.Medium:
                objs = GameObject.FindGameObjectsWithTag("Medium Packing Station");
                break;
            case ItemWeight.Heavy:
                objs = GameObject.FindGameObjectsWithTag("Heavy Packing Station");
                break;
            default: break;
        }

        // There is only 1 packing station of each type
        return objs[0];
    }

    // Find the closest basket to the right then attach it to this item
    public void FindBasket()
    {
        Basket basket = Basket.FindBasket();
        if (basket != null)
        {
            basketScript = basket;
        }
    }

    // Instantiate a Box around the item
    public void PackItem()
    {
        if (!isPacked)
        {
            // Create a box according to item's weight
            box = Instantiate(boxPrefab, transform.position, Quaternion.identity) as GameObject;
            box.transform.localScale = boxSize;

            // Make item small so it doesnt conflict with box
            transform.localScale *= 0.2f;

            Box boxScript = box.GetComponent<Box>();
            boxScript.BoxItem(this.gameObject.GetComponent<Item>());
            isPacked = true;
            // Remove the item's orderCard
            orderBar.removeCard(this);
        }
    }

    public ItemStatus getStatus()
    {
        return status;
    }

    // Method to get name and reward of item
    public string getName()
    {
        return name;
    }
    public float getReward()
    {
        return reward;
    }

    public void getDebuff()
    {
        if (spd == fixedSpd)
        {
            spd = fixedSpd / 2;
        }

        if (des_worker)
        {
            des_worker.setCurrSpd(spd);
        }
    }

    public void payWage()
    {
        if (spd == fixedSpd / 2)
        {
            spd = fixedSpd;
        }
        if (des_worker)
        {
            des_worker.setCurrSpd(spd);
        }
    }

    public float getSpd()
    {
        return spd;
    }
}
