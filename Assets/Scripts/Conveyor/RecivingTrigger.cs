using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecivingTrigger : MonoBehaviour
{
    public bool isUsed;
    public RecivingConveyor parent;
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Item")
        {
            parent.checkDest(this, other.gameObject);
        }
    }

}
