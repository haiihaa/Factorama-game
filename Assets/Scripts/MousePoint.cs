using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HighlightableObject))]
[RequireComponent(typeof(AudioSource))]

public class MousePoint : MonoBehaviour
{
    public AudioClip pickSound;
    private HighlightableObject myHighLightEffect;
    Vector3 originalScale, higherScale;
    void Start()
    {
        myHighLightEffect = GetComponent<HighlightableObject>();
        originalScale = transform.localScale;
        higherScale = originalScale * 1.5f;
    }

    private void OnMouseEnter()
    {
        if (GetComponent<Item>().status != Item.ItemStatus.BeingHandled)
        {
            myHighLightEffect.ConstantOn(Color.white);
            transform.localScale = higherScale;
        }

    }

    private void OnMouseDown()
    {
        GetComponent<AudioSource>().PlayOneShot(pickSound);
    }

    private void OnMouseExit()
    {
        myHighLightEffect.ConstantOff();
        transform.localScale = originalScale;
    }

}
