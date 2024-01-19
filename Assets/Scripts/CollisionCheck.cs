using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollisionCheck : MonoBehaviour
{
    public Placing placer;
    // Start is called before the first frame update
    private void Start()
    {
        placer = GameObject.Find("GridParent").GetComponent<Placing>();
    }
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject() || placer.isplacing)
            return;
        transform.parent.GetComponent<FurnitureScript>().SpawnUI();
    }
}
