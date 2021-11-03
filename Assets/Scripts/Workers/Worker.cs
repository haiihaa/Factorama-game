using UnityEngine;
using System.Collections;

public class Worker : MonoBehaviour
{

    public enum WorkerStatus
    {
        PickingUpItem,
        DroppingOffItem,
        Idle,
        Walkingback
    }

    // Variables
    public int id;
    //public float salary;
    public static float hireRate, tiredTime;
    //public List<Item> carriedItems;
    public float speed = levelData.workerSpd;
    public Vector3 defaultposition;

    protected ParticleSystem tiredEffect;

    protected Item itemScript;
    protected WorkerStatus status = WorkerStatus.Idle;
    protected Animator anim;

    protected bool picked_up_item = false;

    //protected bool isTired = false;

    protected virtual void Start()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
        tiredEffect = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {

    }

    virtual public void moveToItem(GameObject item)
    {
        this.itemScript = item.GetComponent<Item>();
        this.status = WorkerStatus.PickingUpItem;
    }

    protected void LMovetoDestination(Vector3 destination)
    {
        if (transform.position.z != destination.z)
        {
            Vector3 pivot = new Vector3(transform.position.x, transform.position.y, destination.z);
            transform.LookAt(pivot);
            transform.position = Vector3.MoveTowards(transform.position, pivot, speed * Time.deltaTime);
        }
        else
        {
            transform.LookAt(destination);
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        }
    }

    public void getBuff() { }

    public WorkerStatus getStatus()
    {
        return this.status;
    }

    // Reset the speed based on the weight of item
    public void setCurrSpd(float spd)
    {
        speed = spd;
    }

    public void getDebuff()
    {
        if (speed == levelData.workerSpd)
        {
            speed = levelData.workerSpd / 2;
        }
        //isTired = true;
        if (tiredEffect.isStopped)
        {
            tiredEffect.Play();
        }
    }

    public void getSalary()
    {
        if (speed == levelData.workerSpd / 2)
        {
            speed = levelData.workerSpd;
        }
        //isTired = false;
        if (tiredEffect.isPlaying)
        {
            tiredEffect.Stop();
        }
    }
}
