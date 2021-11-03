using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public Vector3 direction;
    public float speed;
    public bool isRunning;
    private List<GameObject> baskets;

    // Start is called before the first frame update
    void Start()
    {
        baskets = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            // move baskets in direction
            foreach (GameObject basket in baskets)
            {
                basket.transform.position += direction * speed * Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Basket")
        {
            baskets.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Basket")
        {
            baskets.Remove(other.gameObject);
        }
    }

    public void removeItem(GameObject item)
    {
        baskets.Remove(item);
    }

}
