using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placing : MonoBehaviour
{
    public GameObject indicatorobj;
    public FloorInputManager floorInputManager;
    public Grid grid;
    public UIController uic;

    public List<Vector3Int> OccupiedCells;

    public Material Green;
    public Material Red;
    public Material Grey;

    public int ObjectID = -1;

    private bool isplacing = false;
    // Start is called before the first frame update

    public void EnterPlacementMode(int ObjID)
    {
        isplacing = true;
        ObjectID = ObjID;
        floorInputManager.Clicked += PlaceObject;
        floorInputManager.Escape += ExitPlacementMode;
    }

    public void PlaceObject()
    {
        Vector3Int cellpos = grid.WorldToCell(floorInputManager.GetMousePos());
        if (OccupiedCells.Contains(cellpos)) return;
        foreach(FurnitureData data in uic.FurnitureList)
        {
            if(data.ID == ObjectID)
            {
                for(int x = 0; x < data.Size.x; x++)
                {
                    for(int y = 0; y < data.Size.y; y++)
                    {
                        OccupiedCells.Add(cellpos + new Vector3Int(x, 0, y));
                    }
                }
                GameObject obj = Instantiate(data.Prefab);
                obj.transform.position = grid.CellToWorld(cellpos);
            }
        }
    }

    public void ExitPlacementMode()
    {
        isplacing = false;
        ObjectID = -1;
        indicatorobj.GetComponentInChildren<MeshRenderer>().material = Grey;
        floorInputManager.Clicked -= PlaceObject;
        floorInputManager.Escape -= ExitPlacementMode;

    }


    // Update is called once per frame
    void Update()
    {
        Vector3Int cellpos = grid.WorldToCell(floorInputManager.GetMousePos());
        if (isplacing && OccupiedCells.Contains(cellpos))
        {
            indicatorobj.GetComponentInChildren<MeshRenderer>().material = Red;
        }
        else if(isplacing && !OccupiedCells.Contains(cellpos))
        {
            indicatorobj.GetComponentInChildren<MeshRenderer>().material = Green;
        }
        indicatorobj.transform.position = grid.CellToWorld(cellpos);
    }
}
