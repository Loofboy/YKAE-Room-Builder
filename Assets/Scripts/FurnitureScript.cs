using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureScript : MonoBehaviour
{
    public FurnitureData data;
    public int[] componentMaterials;
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

    public void SetMaterial(int componentint, int materialint)
    {
        componentMaterials[componentint] = materialint;
        GetComponentInChildren<Renderer>().materials[componentint] = data.Components[componentint].materials[materialint];
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
