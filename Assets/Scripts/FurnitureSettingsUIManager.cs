using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureSettingsUIManager : MonoBehaviour
{
    public Button deleteButton;
    public Button moveButton;
    public Button rotateButton;
    public Button swatchButton;

    public GameObject SwatchPanel;

    public GameObject parentObject;
    public GameObject ComponentItemPrefab;
    public GameObject SwatchItemPrefab;
    public FurnitureScript furnitureScript;
    public FurnitureData furnitureData;

    public Placing placer;

    private bool swatchopen = false;
    private bool swatchgenerated = false;
    // Start is called before the first frame update
    void Start()
    {
        placer = GameObject.Find("GridParent").GetComponent<Placing>();
        Camera camera = Camera.main;
        //GetComponent<Canvas>().worldCamera = camera;
        //GetComponent<Canvas>().planeDistance = 1;
        //transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
        //transform.GetChild(0).position = new Vector3(0, 0, 0);
        transform.GetChild(0).position = new Vector3(Input.mousePosition.x - 960, Input.mousePosition.y - 520, 0);

        parentObject = transform.parent.gameObject;
        furnitureScript = parentObject.GetComponent<FurnitureScript>();
        furnitureData = furnitureScript.data;
    }

    public void DeleteObject()
    {
        foreach (var cell in furnitureScript.TakenCells) {
            placer.OccupiedCells.Remove(cell);
        }
        Destroy(parentObject);
    }

    public void MoveObject()
    {
        parentObject.transform.position += new Vector3(0, 45, 0);
        foreach (var cell in furnitureScript.TakenCells)
        {
            placer.OccupiedCells.Remove(cell);
        }
        furnitureScript.TakenCells.Clear();
        placer.MovingObject = parentObject;
        placer.objectRotation = furnitureScript.rotation;
        placer.EnterPlacementMode(furnitureData.ID);
    }

    public void ToggleSwatchMenu()
    {
        if (!swatchopen)
        {
            SwatchPanel.transform.parent.gameObject.SetActive(true);
            if (!swatchgenerated)
                GenerateSwatches();
            swatchopen = true;
        }
        else
        {
            SwatchPanel.transform.parent.gameObject.SetActive(false);
            swatchopen = false;
        }
    }

    public void GenerateSwatches()
    {
        for(int comp = 0; comp < furnitureData.Components.Count; comp++)
        {
            GameObject item = Instantiate(ComponentItemPrefab);
            item.transform.SetParent(SwatchPanel.transform, false);
            item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = furnitureData.Components[comp].Name;
            for(int mat = 0; mat < furnitureData.Components[comp].materials.Count; mat++)
            {
                GameObject swatch = Instantiate(SwatchItemPrefab);
                swatch.transform.SetParent(item.transform.GetChild(1).GetChild(0).transform, false);
                Sprite sprite = Sprite.Create(
                    (Texture2D)furnitureData.Components[comp].materials[mat].mainTexture,
                    new Rect(0, 0, furnitureData.Components[comp].materials[mat].mainTexture.width, furnitureData.Components[comp].materials[mat].mainTexture.height),
                    new Vector2(0.5f, 0.5f)
                );
                swatch.transform.GetComponent<Image>().sprite = sprite;
                swatch.transform.GetComponent<Image>().color = furnitureData.Components[comp].materials[mat].color;
                int i = comp;
                int j = mat;
                swatch.GetComponent<Button>().onClick.AddListener(delegate { furnitureScript.SetMaterial(i, j); });
            }
        }
        swatchgenerated = true;
    }


    // Update is called once per frame
    void Update()
    {
 
    }
}
