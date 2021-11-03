using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bin : MonoBehaviour
{
    public Conveyor conveyor;
    public StoringConveyor storingConveyor;
    public Material dissolveMat;
    private void OnTriggerEnter(Collider other)
    {
        // Only remove basket when it contains no item
        // Else, have to wait for worker to handle the 
        if (other.gameObject.tag == "Basket" && other.gameObject.GetComponent<Basket>().isRemovable())
        {
            //Debug.Log("From Bin: removed basket");
            conveyor.removeItem(other.gameObject);
            StartCoroutine(Dissole(other.gameObject));
            storingConveyor.updateCurrOnBelt();
        }

        // Should not destroy item because it is ordered on the order bar
        // if (other.gameObject.tag == "Item")
        // {
        //      Destroy(other.gameObject);
        // }
        IEnumerator Dissole(GameObject obj)
        {
            // Change the Object's Material to the Dissolve Shader
            dissolveMat.SetFloat("_TimeOfDissolve", Time.timeSinceLevelLoad);
            obj.GetComponent<MeshRenderer>().material = dissolveMat;

            // Takes 1 seconds to dissolve
            yield return new WaitForSeconds(1.5f);

            // Destroy object once it has been dissolved
            Destroy(obj);

        }
    }
}
