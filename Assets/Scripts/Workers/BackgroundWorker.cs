using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundWorker : MonoBehaviour
{
    private Vector3 destination = new Vector3(0, -1.0f, 0.0f);
    private float speed = 2.0f;
    private Animator anim;
    private bool paused = true;

    void Start()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
        StartCoroutine(Wait());
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused){
            if (destination.y == -1.0f)
            {
                anim.SetInteger("AnimationPar", 0);
                GetRandomPoint();
            } 
            else 
            {
                if (transform.position == destination)
                {
                    destination.y = -1.0f;
                    StartCoroutine(Wait());
                }
                else 
                {
                    anim.SetInteger("AnimationPar", 1);
                    transform.LookAt(destination);
                    transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                }
            }
        }
    }

    // Find a random position within the break room
    void GetRandomPoint()
    {
        // X value range: (-21.5, -11.0)
        // Y value always 0 
        // Z value range: (-4.5, 4)
        destination = new Vector3(Random.Range(-21.5f, -11.0f), 0, Random.Range(-4.5f, 4f)); 
    }
    IEnumerator Wait()
    {
        paused = true;
        anim.SetInteger("AnimationPar", 0);
        yield return new WaitForSeconds(Random.Range(1.0f, 12.0f));
        paused = false;
    }
}
