using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class UIController : MonoBehaviour
{
    public Animator ObjectUIAnim;
    public Placing placer;

    public List<FurnitureData> FurnitureList;
    public GameObject ObjectSlot;
    public GameObject Catalogue;

    private bool ObjectUIState = false;
    private bool isPlacing = false;
    private int placingID = -1;
    // Start is called before the first frame update
    void Start()
    {
        foreach(FurnitureData data in FurnitureList)
        {
            GameObject slot = Instantiate(ObjectSlot);
            slot.transform.SetParent(Catalogue.transform);
            slot.transform.localScale = Vector3.one;
            slot.transform.GetChild(0).GetComponent<Image>().sprite = data.Image;
            slot.GetComponentInChildren<Button>().onClick.AddListener(delegate { TogglePlacementMode(data.ID); });
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPlacing)
                isPlacing = false;
            else
                DeactivateObjectUI();
        }
    }
    public void TogglePlacementMode(int ObjID)
    {
        if (isPlacing && placingID == ObjID)
        {
            placer.ExitPlacementMode();
            placingID = -1;
            isPlacing = false;
        }
        else if(isPlacing && placingID != ObjID)
        {
            placer.ExitPlacementMode();
            placer.EnterPlacementMode(ObjID);
            placingID = ObjID;
            isPlacing = true;
        }
        else
        {
            placer.EnterPlacementMode(ObjID);
            placingID = ObjID;
            isPlacing = true;
        }
    }

    public void ToggleObjectUI()
    {
        if (ObjectUIState)
            DeactivateObjectUI();
        else
            ActivateObjectUI();
    }
    public void ActivateObjectUI()
    {
        ObjectUIAnim.SetBool("IsActive", true);
        ObjectUIState = true;
    }

    public void DeactivateObjectUI()
    {
        ObjectUIAnim.SetBool("IsActive", false);
        ObjectUIState = false;
    }
}
