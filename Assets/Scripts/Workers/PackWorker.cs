using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]

public class PackWorker : Worker
{
    private bool packing = false;
    private GameObject progressBar;
    private Slider slider;
    private Vector3 vanBack = new Vector3(15.15f, 0.0f, -6.7f);
    public Object PackingProgressBar;
    private GameObject packingStation;

    public AudioClip clip1,clip2;

    // Update is called once per frame
    void Update()
    {
        PackingWorkerFlow();
    }

    private void PackingWorkerFlow()
    {
        float step = speed * Time.deltaTime;
        Vector3 target;

        switch (status)
        {
            case WorkerStatus.PickingUpItem:
                // Pick up item from the basket
                Vector3 pick_up_dest = itemScript.gameObject.transform.position;
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
                Vector3 drop_dest = new Vector3(0.0f, 0.0f, 0.0f);
                transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("_RimPower", 0.8f);
                // If the item is packed we move towards the Van, otherwise we move towards the packing station
                if (itemScript.isPacked)
                {
                    drop_dest = vanBack;
                }
                else
                {
                    // Packing Station Location
                    drop_dest = itemScript.GetPackingStation().transform.position;
                }

                target = new Vector3(drop_dest.x, transform.position.y, drop_dest.z);

                if (packing)
                {
                    // Wait 2 seconds and then box appears around item
                    StartCoroutine(PackingProcess());
                }
                else
                {
                    // Check if Worker has dropped off the item
                    if (!picked_up_item)
                    {
                        status = WorkerStatus.Walkingback;
                        itemScript.dropItem();
                    }
                    else
                    {
                        // If target is van's back --> move L
                        if (target == vanBack)
                        {
                            LMovetoDestination(target);
                        }
                        else
                        {
                            transform.LookAt(target);
                            transform.position = Vector3.MoveTowards(transform.position, target, step);
                        }
                    }
                }
                break;

            case WorkerStatus.Idle:
                transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("_RimPower", 0.0f);
                anim.SetInteger("AnimationPar", 0);
                transform.LookAt(Vector3.back);
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

    // Wait 2 seconds then then item has been packed
    IEnumerator PackingProcess()
    {
        anim.SetInteger("AnimationPar", 0);

        // Show the progress bar above the worker
        if (progressBar == null)
        {
            progressBar = Instantiate(PackingProgressBar, new Vector3(transform.position.x, transform.position.y + 3.0f, transform.position.z), Quaternion.identity) as GameObject;
            slider = progressBar.transform.GetChild(0).GetComponent<Slider>();
        }

        // Takes 2 seconds to reach the end of the progress bar
        if (slider != null)
        {
            if (slider.value < 1)
            {
                slider.value += 0.5f * Time.deltaTime;
            }
        }

        // Play the particle system
        ParticleSystem part = null;
        if (packingStation == null)
        {
            packingStation = itemScript.GetPackingStation();
            part = packingStation.GetComponentInChildren<ParticleSystem>();
            part.Play();
        }

        yield return new WaitUntil(delegate () { return slider.value >= 1.0f; });
        GetComponent<AudioSource>().clip = clip2;
        GetComponent<AudioSource>().Play();
        packing = false;
        itemScript.PackItem();
        anim.SetInteger("AnimationPar", 1);

        // Stop the packing particle sys
        if (part != null && part.isPlaying)
        {
            part.Stop();
        }
        // Get rid of the progress bar and packing station
        Destroy(progressBar);
        progressBar = null;
        packingStation = null;
    }

    // Handling Collisions
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(string.Format("From PackWorker: Collided with {0}", other.tag));
        // Has the worker hit the basket that the item arrives in?
        if (itemScript != null)
        {
            if (itemScript.basketScript && other.transform == itemScript.basketScript.transform)
            {
                // Set speed corresponding to weight
                speed = itemScript.getSpd();
                picked_up_item = true;
                itemScript.basketScript.updateRemovedItem();
            }
            // Has the Worker reached the packing station?
            if (other.transform == this.itemScript.GetPackingStation().transform && itemScript.getStatus() == Item.ItemStatus.BeingHandled)
            {
                packing = true;
            }
            // Has the Worker reached the Van?
            if (other.tag == "Van")
            {
                picked_up_item = false;
                GetComponent<AudioSource>().PlayOneShot(clip1);
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
