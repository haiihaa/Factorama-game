using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickWorker : Worker
{
    // Update is called once per frame
    void Update()
    {
        PickingWorkerFlow();
    }

    private void PickingWorkerFlow()
    {
        float step = speed * Time.deltaTime;
        Vector3 target;

        switch (status)
        {
            case WorkerStatus.PickingUpItem:
                // Move towards the Shelf which holds the item
                Vector3 pick_up_dest = itemScript.shelf.trigger.transform.position;
                target = new Vector3(pick_up_dest.x, transform.position.y, pick_up_dest.z);

                transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("_RimPower", 0.8f);

                // Check if Worker has picked up the item
                if (picked_up_item)
                {
                    status = WorkerStatus.DroppingOffItem;
                    itemScript.pickUpItem();
                }
                else
                {
                    anim.SetInteger("AnimationPar", 1);
                    transform.LookAt(target);
                    transform.position = Vector3.MoveTowards(transform.position, target, step);
                }
                break;

            case WorkerStatus.DroppingOffItem:
                // Move towards the free basket that is further to the right
                Vector3 drop_dest = itemScript.basketScript.transform.position;
                target = new Vector3(drop_dest.x, transform.position.y, drop_dest.z);

                transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("_RimPower", 0.8f);

                // Check if Worker has dropped off the item
                if (!picked_up_item)
                {
                    status = WorkerStatus.Walkingback;
                    itemScript.dropItem();
                }
                else
                {
                    transform.LookAt(target);
                    transform.position = Vector3.MoveTowards(transform.position, target, step);
                }
                break;

            case WorkerStatus.Idle:
                transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("_RimPower", 0.0f);
                anim.SetInteger("AnimationPar", 0);
                transform.LookAt(Vector3.forward);
                //transform.position = Vector3.MoveTowards(transform.position, defaultposition, step);
                break;

            case WorkerStatus.Walkingback:
                transform.LookAt(defaultposition);
                transform.position = Vector3.MoveTowards(transform.position, defaultposition, step);
                if (transform.position == defaultposition)
                {
                    status = WorkerStatus.Idle;
                }
                break;

            default: break;
        }
    }

    // Handling Collisions
    void OnTriggerEnter(Collider other)
    {
        if (itemScript != null)
        {
            // Has the worker hit the shelf?
            if (other.transform == this.itemScript.shelf.trigger.transform)
            {
                // Set speed corresponding to weight
                speed = itemScript.getSpd();
                picked_up_item = true;
                this.itemScript.shelf.removeItem(this.itemScript.gameObject);
            }
            // Has the worked hit the basket?
            if (other.transform == this.itemScript.basketScript.transform)
            {
                picked_up_item = false;
                // Set speed back to normal
                // If under no-energymode
                if (Camera.main.GetComponent<gameController>().isTired())
                {
                    speed = levelData.workerSpd / 2;
                }
                else
                {
                    speed = levelData.workerSpd;
                }
            }
        }
    }
}
