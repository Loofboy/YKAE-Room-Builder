using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FloorInputManager : MonoBehaviour
{
    public Camera cam;
    private Vector3 lastpos;

    public LayerMask floorLayerMask;
    public event Action Clicked;
    public event Action Escape;

    public Vector3 GetMousePos()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit, 100, floorLayerMask))
        {
            lastpos = hit.point;
        }
        return lastpos;
    }
    private void OnMouseDown()
    {
        Debug.Log("Clicked");
        Clicked?.Invoke();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Escape?.Invoke();
        }
    }
}
