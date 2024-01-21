using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;

public class Placing : MonoBehaviour
{
    public GameObject indicatorobj;
    public GameObject FurnitureList;
    public FloorInputManager floorInputManager;
    public Grid grid;
    public UIController uic;

    public List<Vector3Int> OccupiedCells;
    public List<Vector3Int> CheckedCells;
    public List<Vector3Int> AdjacentCells;
    private FurnitureData dataInCheck;

    public Material Green;
    public Material Red;
    public Material Grey;

    public GameObject MovingObject;
    public GameObject PlacementUI;

    private GameObject ObjPlaceholder;
    public GameObject lastobj;
    public bool isLoading = false;

    public bool SnapMode = false;

    private int xmin = -6, xmax = 5, zmin = 2, zmax = 13;
    private int xmod = 1, ymod = 1;
    private int objx, objy;
    private int checkx = 0, checky = 0;


    public int ObjectID = -1;
    public int objectRotation = 0;

    public int RotationAngle = 0;
    public int xoffset = 0, yoffset = 0;


    [SerializeField]
    private Vector3Int cellpos;


    public bool isplacing = false;
    private void Start()
    {
        for (int x = xmin; x < xmax; x++)
        {
            OccupiedCells.Add(new Vector3Int(x, 0, zmin));
            OccupiedCells.Add(new Vector3Int(x, 0, zmax));
        }

        for (int z = zmin; z < zmax; z++)
        {
            OccupiedCells.Add(new Vector3Int(xmin, 0, z));
            OccupiedCells.Add(new Vector3Int(xmax, 0, z));
        }
        floorInputManager.Escape += KillSettingsUI;
    }

    public void EnterPlacementMode(int ObjID)
    {
        CalculateAdjacentCells();
        isplacing = true;
        ObjectID = ObjID;
        floorInputManager.Clicked += PlaceObject;
        floorInputManager.Escape += ExitPlacementMode;

        KillSettingsUI();
        findData();
        objx = dataInCheck.Size.x; objy = dataInCheck.Size.y;
        checkx = objx; checky = objy;
        ObjPlaceholder = Instantiate(dataInCheck.Prefab);
        ObjPlaceholder.GetComponentInChildren<MeshCollider>().enabled = false;
        ObjPlaceholder.transform.position = indicatorobj.transform.position;
        ObjPlaceholder.transform.SetParent(indicatorobj.transform);
        PlacementUI.SetActive(true);
        Rotate(objectRotation);

    }

    public void PlaceObject()
    {
        if (EventSystem.current.IsPointerOverGameObject() && !isLoading)
            return;

        foreach (var cell in CheckedCells) 
        {
            if (OccupiedCells.Contains(cell))
                return;
        }

        if (SnapMode && !AdjacentCells.Contains(CheckedCells[0]))
            return;

        foreach (var cell in CheckedCells)
        {
            OccupiedCells.Add(cell);
        }

        if (MovingObject != null)
        {
            MovingObject.transform.position = grid.CellToWorld(CheckedCells[0]);
            MovingObject.GetComponent<FurnitureScript>().TakenCells.AddRange(CheckedCells);
            MovingObject.GetComponent<FurnitureScript>().rotation = objectRotation;
            MovingObject.transform.position += new Vector3(xoffset, 0, yoffset);
            MovingObject.transform.rotation = Quaternion.Euler(0, RotationAngle, 0);
            MovingObject = null;
            ExitPlacementMode();
        }
        else
        {
            lastobj = Instantiate(dataInCheck.Prefab);
            lastobj.transform.SetParent(FurnitureList.transform);
            lastobj.transform.position = grid.CellToWorld(CheckedCells[0]);

            lastobj.transform.position += new Vector3(xoffset, 0, yoffset);
            lastobj.transform.rotation = Quaternion.Euler(0, RotationAngle, 0);
            lastobj.GetComponent<FurnitureScript>().TakenCells.AddRange(CheckedCells);
            lastobj.GetComponent<FurnitureScript>().rotation = objectRotation;
        }
        CheckedCells.Clear();
        CalculateAdjacentCells();
    }
    public void findData()
    {
        foreach(FurnitureData data in uic.FurnitureList)
        {
            if (data.ID == ObjectID)
                dataInCheck = data;
        }
    }

    public List<Vector3Int> CheckedCellsList()
    {
        Vector3Int cellpos = grid.WorldToCell(floorInputManager.GetMousePos());
        List<Vector3Int> Cellslist = new List<Vector3Int> { };

        if (dataInCheck == null) return Cellslist;

        for (int x = 0; x < checkx; x++)
            {
            for (int y = 0; y < checky; y++)
                {
                    Cellslist.Add(cellpos + new Vector3Int(x * xmod, 0, y * ymod));
                }
            }
        return Cellslist;

    }

    public void ExitPlacementMode()
    {
        isplacing = false;
        ObjectID = -1;
        indicatorobj.GetComponentInChildren<MeshRenderer>().material = Grey;
        floorInputManager.Clicked -= PlaceObject;
        floorInputManager.Escape -= ExitPlacementMode;
        CheckedCells.Clear();
        Destroy(ObjPlaceholder);
        PlacementUI.SetActive(false);
        objectRotation = 0;
        CalculateAdjacentCells();
    }

    public void KillSettingsUI()
    {
        GameObject ExistingUI = GameObject.Find("FurnitureSettingsUI(Clone)");
        if (ExistingUI != null)
        {
            Destroy(ExistingUI);
        }

    }

    public void Rotate(int rotation)
    {
        switch (rotation)
        {
            case 0:
                ymod = 1;
                xmod = 1;
                checkx = objx;
                checky = objy;
                RotationAngle = 0;
                xoffset = 0;
                yoffset = 0;
                break;
            case 1:
                ymod = 1;
                xmod = -1;
                checkx = objy;
                checky = objx;
                RotationAngle = -90;
                xoffset = 1;
                yoffset = 0;
                break;
            case 2:
                ymod = -1;
                xmod = -1;
                checkx = objx;
                checky = objy;
                RotationAngle = -180;
                xoffset = 1;
                yoffset = 1;
                break;
            case 3:
                ymod = -1;
                xmod = 1;
                checkx = objy;
                checky = objx;
                RotationAngle = -270;
                xoffset = 0;
                yoffset = 1;
                break;
        }
        if(ObjPlaceholder != null)
        {
            ObjPlaceholder.transform.position = indicatorobj.transform.position + new Vector3(xoffset, 0, yoffset);
            ObjPlaceholder.transform.rotation = Quaternion.Euler(0, RotationAngle, 0);
        }

    }

    public void CalculateAdjacentCells()
    {
        AdjacentCells.Clear();
        if (OccupiedCells.Count < 45) return;
        for(int i = 44; i < OccupiedCells.Count; i++)
        {
            Vector3Int cell = OccupiedCells[i];
            if (!OccupiedCells.Contains(cell + new Vector3Int(-1, 0, 0)))
            {
                AdjacentCells.Add(cell + new Vector3Int(-1, 0, 0));
            }
            if (!OccupiedCells.Contains(cell + new Vector3Int(1, 0, 0)))
            {
                AdjacentCells.Add(cell + new Vector3Int(1, 0, 0));
            }
            if (!OccupiedCells.Contains(cell + new Vector3Int(0, 0, 1)))
            {
                AdjacentCells.Add(cell + new Vector3Int(0, 0, 1));
            }
            if (!OccupiedCells.Contains(cell + new Vector3Int(0, 0, -1)))
            {
                AdjacentCells.Add(cell + new Vector3Int(0, 0, -1));
            }
        }
    }

    public void TriggerRotation()
    {
        objectRotation++;
        if (objectRotation > 3) objectRotation = 0;
        Rotate(objectRotation);
    }

    // Update is called once per frame
    void Update()
    {
        if(isplacing && Input.GetKeyDown(KeyCode.R))
        {
            TriggerRotation();
        }

        CheckedCells = CheckedCellsList();
        cellpos = grid.WorldToCell(floorInputManager.GetMousePos());
        foreach (Vector3Int cell in CheckedCells)
        {
            if (isplacing && (OccupiedCells.Contains(cell) || (SnapMode && !AdjacentCells.Contains(CheckedCells[0]))))
            {
                indicatorobj.GetComponentInChildren<MeshRenderer>().material = Red;
                if(ObjPlaceholder != null)
                {
                    for (int i = 0; i < ObjPlaceholder.GetComponentInChildren<MeshRenderer>().materials.Length; i++)
                    {
                        Material[] mats = ObjPlaceholder.GetComponentInChildren<MeshRenderer>().materials;
                        mats[i] = Red;
                        ObjPlaceholder.GetComponentInChildren<MeshRenderer>().materials = mats;
                    }
                }
                break;
            }
            else if (isplacing && (!OccupiedCells.Contains(cell) || (SnapMode && AdjacentCells.Contains(CheckedCells[0]))))
            {
                indicatorobj.GetComponentInChildren<MeshRenderer>().material = Green;
                if (ObjPlaceholder != null)
                {
                    for (int i = 0; i < ObjPlaceholder.GetComponentInChildren<MeshRenderer>().materials.Length; i++)
                    {
                        Material[] mats = ObjPlaceholder.GetComponentInChildren<MeshRenderer>().materials;
                        mats[i] = Green;
                        ObjPlaceholder.GetComponentInChildren<MeshRenderer>().materials = mats;
                    }
                }
            }
        }
        if(!SnapMode || (SnapMode && AdjacentCells.Contains(CheckedCells[0])))
            indicatorobj.transform.position = grid.CellToWorld(cellpos);
        else
            indicatorobj.transform.position = floorInputManager.GetMousePos();
    }
}
