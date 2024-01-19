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
    private FurnitureData dataInCheck;

    public Material Green;
    public Material Red;
    public Material Grey;

    public GameObject MovingObject;

    private GameObject ObjPlaceholder;

    private int xmin = -6, xmax = 5, zmin = 2, zmax = 13;
    private int mod = 1;
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
        Rotate(objectRotation);

    }

    public void PlaceObject()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        foreach (var cell in CheckedCells) 
        {
            if (OccupiedCells.Contains(cell))
                return;
        }
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
            GameObject obj = Instantiate(dataInCheck.Prefab);
            obj.transform.SetParent(FurnitureList.transform);
            obj.transform.position = grid.CellToWorld(CheckedCells[0]);

            obj.transform.position += new Vector3(xoffset, 0, yoffset);
            obj.transform.rotation = Quaternion.Euler(0, RotationAngle, 0);
            obj.GetComponent<FurnitureScript>().TakenCells.AddRange(CheckedCells);
            obj.GetComponent<FurnitureScript>().rotation = objectRotation;
        }
        CheckedCells.Clear();
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
                    Cellslist.Add(cellpos + new Vector3Int(x * mod, 0, y * mod));
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
        objectRotation = 0;
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
                mod = 1;
                checkx = objx;
                checky = objy;
                RotationAngle = 0;
                xoffset = 0;
                yoffset = 0;
                break;
            case 1:
                mod = -1;
                checkx = objy;
                checky = objx;
                RotationAngle = -90;
                xoffset = 1;
                yoffset = 0;
                break;
            case 2:
                mod = -1;
                checkx = objx;
                checky = objy;
                RotationAngle = -180;
                xoffset = 1;
                yoffset = 1;
                break;
            case 3:
                mod = 1;
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

    // Update is called once per frame
    void Update()
    {
        if(isplacing && Input.GetKeyDown(KeyCode.R))
        {
            objectRotation++;
            if (objectRotation > 3) objectRotation = 0;
            Rotate(objectRotation);
        }

        CheckedCells = CheckedCellsList();
        cellpos = grid.WorldToCell(floorInputManager.GetMousePos());
        foreach (Vector3Int cell in CheckedCells)
        {
            if (isplacing && OccupiedCells.Contains(cell))
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
            else if (isplacing && !OccupiedCells.Contains(cell))
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
        indicatorobj.transform.position = grid.CellToWorld(cellpos);
    }
}
