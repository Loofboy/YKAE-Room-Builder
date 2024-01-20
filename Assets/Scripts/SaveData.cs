using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SaveData
{
    public List<FurnitureObject> Furnitures;

    public int wallmatnum;
    public int floormatnum;
}

[Serializable]
public class FurnitureObject
{
    public int ObjID;
    public List<Vector3Int> TakenCells;
    public int rotation;
    public int[] componentMaterials;
}
