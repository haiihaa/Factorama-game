using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveWorker : Worker
{
    //public Vector3 defaultposition;
    public RecivingConveyor conveyor;

    // Update is called once per frame
    void Update()
    {
        ReceivingWorkerFlow();
    }

    private void ReceivingWorkerFlow()
    {
        float step = speed * Time.deltaTime;
        Vector3 target;

        switch (status)
        {
            case WorkerStatus.PickingUpItem:
                Vector3 pick_up_dest = itemScript.gameObject.transform.position;

                target = new Vector3(pick_up_dest.x, transform.position.y, pick_up_dest.z);

                Vector3 deltaPos = target - transform.position;

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
                Vector3 drop_dest = itemScript.shelf.trigger.transform.position;

                transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("_RimPower", 0.8f);

                target = new Vector3(drop_dest.x, transform.position.y, drop_dest.z);
                // Check if Worker has dropped off the item
                if (!picked_up_item)
                {
                    status = WorkerStatus.Walkingback;
                    itemScript.dropItem();
                    itemScript = null;
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
                transform.LookAt(Vector3.right);

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

    // Handle collisions
    void OnTriggerEnter(Collider other)
    {
        if (itemScript != null)
        {
            if (other.transform == this.itemScript.transform)
            {
                // Set speed corresponding to weight
                speed = itemScript.getSpd();
                picked_up_item = true;
                conveyor.takeItem(itemScript.gameObject);
            }
            if (other.transform == this.itemScript.shelf.trigger.transform)
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
