using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomChanger : MonoBehaviour
{
    public List<Material> Floormaterials = new List<Material>();
    public List<Material> Wallmaterials = new List<Material>();

    public int Floormatnum = 0;
    public int Wallmatnum = 0;

    public GameObject FloorContainer;
    public GameObject WallContainer;

    public GameObject Floor;
    public List<GameObject> Walls = new List<GameObject>();

    public GameObject MatButton;
    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach (Material mat in Floormaterials)
        {
            GameObject item = Instantiate(MatButton);
            item.transform.SetParent(FloorContainer.transform, false);
            Sprite sprite = Sprite.Create(
                    (Texture2D)mat.mainTexture,
                    new Rect(0, 0, mat.mainTexture.width, mat.mainTexture.height),
                    new Vector2(0.5f, 0.5f)
                );
            item.transform.GetComponent<Image>().sprite = sprite;
            item.transform.GetComponent<Image>().color = mat.color;
            int j = i;
            item.GetComponent<Button>().onClick.AddListener(delegate { ChangeFloor(j); });
            i++;
        }
        i = 0;
        foreach (Material mat in Wallmaterials)
        {
            GameObject item = Instantiate(MatButton);
            item.transform.SetParent(WallContainer.transform, false);
            Sprite sprite = Sprite.Create(
                    (Texture2D)mat.mainTexture,
                    new Rect(0, 0, mat.mainTexture.width, mat.mainTexture.height),
                    new Vector2(0.5f, 0.5f)
                );
            item.transform.GetComponent<Image>().sprite = sprite;
            item.transform.GetComponent<Image>().color = mat.color;
            int j = i;
            item.GetComponent<Button>().onClick.AddListener(delegate { ChangeWalls(j); });
            i++;
        }
    }

    // Update is called once per frame
    public void ChangeFloor(int matnum)
    {
        Floor.GetComponent<Renderer>().material = Floormaterials[matnum];
        Floormatnum = matnum;
    }
    public void ChangeWalls(int matnum)
    {
        foreach(GameObject wall in Walls)
        {
            wall.GetComponent<Renderer>().material = Wallmaterials[matnum];
        }
        Wallmatnum = matnum;
    }
}
