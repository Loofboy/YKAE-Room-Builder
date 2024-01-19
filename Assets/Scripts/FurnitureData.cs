using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Furniture Data")]
public class FurnitureData : ScriptableObject
{
    [System.Serializable]
    public class SwatchContainer
    {
        public List<Material> materials;
        //public Sprite Matsprite;
        public string Name;
    }

    public string Name;
    public int ID;
    public GameObject Prefab;
    public Sprite Image;
    public Vector2Int Size;

    public List<SwatchContainer> Components;
    

}
