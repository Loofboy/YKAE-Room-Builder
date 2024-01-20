using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public Placing placer;
    public RoomChanger roomChanger;
    public SaveData save = new SaveData();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LoadGameData(SaveData data)
    {
        foreach(FurnitureObject obj in data.Furnitures)
        {
            placer.ObjectID = obj.ObjID;
            placer.CheckedCells = obj.TakenCells;
            placer.Rotate(obj.rotation);
            placer.isLoading = true;
            placer.findData();
            placer.PlaceObject();
            
            
            FurnitureScript script = placer.lastobj.GetComponent<FurnitureScript>();
            script.rotation = obj.rotation;
            for(int i = 0; i < obj.componentMaterials.Length; i++)
            {
                script.prearray[i] = obj.componentMaterials[i];
                //script.SetMaterial(i, obj.componentMaterials[i]);
            }
        }

        //roomChanger.Wallmatnum = data.wallmatnum;
        //roomChanger.Floormatnum = data.floormatnum;

        roomChanger.ChangeFloor(data.floormatnum);
        roomChanger.ChangeWalls(data.wallmatnum);

        placer.isLoading = false;
    }

    public void SaveGameData()
    {
        List<FurnitureObject> furni = new List<FurnitureObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            FurnitureScript data = transform.GetChild(i).GetComponent<FurnitureScript>();
            FurnitureObject obj = new();

            obj.TakenCells = data.TakenCells;
            obj.componentMaterials = data.componentMaterials;
            obj.ObjID = data.data.ID;
            obj.rotation = data.rotation;


            furni.Add(obj);
        }
        save.Furnitures = furni;
        save.wallmatnum = roomChanger.Wallmatnum;
        save.floormatnum = roomChanger.Floormatnum;
    }
}
