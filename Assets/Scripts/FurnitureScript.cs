using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class FurnitureScript : MonoBehaviour
{
    public FurnitureData data;
    public List<Vector3Int> TakenCells = new List<Vector3Int>();
    public int[] componentMaterials;
    public int rotation;

    public GameObject SettingsUI;
    // Start is called before the first frame update
    void Start()
    {
        componentMaterials = new int[GetComponentInChildren<Renderer>().materials.Length];
        for(int i = 0; i < componentMaterials.Length; i++) 
        {
            componentMaterials[i] = 0;
            SetMaterial(i, componentMaterials[i]);
        }
    }
    public void SpawnUI()
    {
        GameObject ExistingUI = GameObject.Find("FurnitureSettingsUI(Clone)");
        if(ExistingUI != null)
        {
            if (ExistingUI.transform.IsChildOf(transform))
            {
                Destroy(ExistingUI);
                return;
            }
            else
            {
                Destroy(ExistingUI);
            }
        } 
        var ui = Instantiate(SettingsUI, transform.position, Quaternion.identity);
        ui.transform.SetParent(transform);
        ui.transform.position = new Vector3(0, 0, 0);
    }

    public void SetMaterial(int componentint, int materialint)
    {
        Debug.Log(componentint + " " + materialint);
        Material[] mats = GetComponentInChildren<Renderer>().materials;
        mats[componentint] = data.Components[componentint].materials[materialint];
        componentMaterials[componentint] = materialint;
        GetComponentInChildren<Renderer>().materials = mats;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
