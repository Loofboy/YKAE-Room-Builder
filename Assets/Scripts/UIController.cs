using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEditor;
using System.IO;

[System.Serializable]
public class NamesList
{
    public List<string> FileNames;
}
public class UIController : MonoBehaviour
{
    public Animator ObjectUIAnim;
    public Placing placer;
    public RoomChanger roomChanger;

    public List<FurnitureData> FurnitureList;
    public GameObject ObjectSlot;
    public GameObject Catalogue;
    public GameObject PlacementUI;
    public GameObject MenuUI;
    public GameObject RoomSwatchUI;
    public GameObject LoadMenu;
    public GameObject SaveMenu;
    public Image PlacementButtonImage;

    public Sprite GridImage;
    public Sprite SnapImage;

    public TMP_Dropdown dropdown;

    public DataManager dataman;

    public GameObject SpawnedFunitureContainer;

    public string namepath;

    public NamesList nameslist;

    private bool MenuUIState = false;
    private bool ObjectUIState = false;
    private bool isPlacing = false;
    private bool roomswatchstate = false;
    private bool loadmenustate = false;
    private bool savemenustate = false;
    private int placingID = -1;
    // Start is called before the first frame update
    void Start()
    {
        namepath = Application.streamingAssetsPath + "/SaveList.json";
        Directory.CreateDirectory(Application.streamingAssetsPath);
        if (!File.Exists(namepath))
        {
            var namecontent = JsonUtility.ToJson(nameslist);
            File.WriteAllText(namepath, namecontent);
        }
        else
        {
            var content = File.ReadAllText(namepath);
            nameslist = JsonUtility.FromJson<NamesList>(content);
        }

        foreach (FurnitureData data in FurnitureList)
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

    public void ToggleSnapMode()
    {
        placer.CalculateAdjacentCells();
        placer.SnapMode = !placer.SnapMode;

        if (placer.SnapMode)
            PlacementButtonImage.sprite = SnapImage;
        else
            PlacementButtonImage.sprite = GridImage;
    }

    public void ToggleRoomSwatch()
    {
        RoomSwatchUI.SetActive(!roomswatchstate);
        roomswatchstate = !roomswatchstate;
    }

    public void ToggleLoadMenu()
    {
        if (savemenustate) ToggleSaveMenu();
        placer.ExitPlacementMode();
        dropdown.ClearOptions();
        if (!loadmenustate)
        {
            loadmenustate = true;
            LoadMenu.SetActive(true);
            if (!string.IsNullOrEmpty(namepath))
            {
                var content = File.ReadAllText(namepath);
                nameslist = JsonUtility.FromJson<NamesList>(content);
            }
            dropdown.value = 0;
            dropdown.options.Add(new TMP_Dropdown.OptionData(text: ""));
            
            foreach (string name in nameslist.FileNames)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData(text: name));
            }
            //dropdown.options.RemoveAt(0);
            dropdown.onValueChanged.AddListener(delegate { if(dropdown.value != 0) LoadRoom(nameslist.FileNames[dropdown.value - 1]); });
        }
        else
        {
            loadmenustate = false;
            LoadMenu.SetActive(false);
        }
    }

    public void ToggleSaveMenu()
    {
        if (loadmenustate) ToggleLoadMenu();
        placer.ExitPlacementMode();
        savemenustate = !savemenustate;
        SaveMenu.SetActive(savemenustate);
    }

    public void NewRoom()
    {
        for(int i = SpawnedFunitureContainer.transform.childCount - 1; i > -1; i--)
        {
            foreach (var cell in SpawnedFunitureContainer.transform.GetChild(i).GetComponent<FurnitureScript>().TakenCells)
            {
                placer.OccupiedCells.Remove(cell);
            }
            Destroy(SpawnedFunitureContainer.transform.GetChild(i).gameObject);
        }
        placer.CalculateAdjacentCells();
        roomChanger.ChangeFloor(0);
        roomChanger.ChangeWalls(0);
    }

    public void SaveRoom(string name)
    {
        if (!string.IsNullOrEmpty(namepath))
        {
            var content = File.ReadAllText(namepath);
            nameslist = JsonUtility.FromJson<NamesList>(content);
        }

        if (!nameslist.FileNames.Contains(name))
        {
            nameslist.FileNames.Add(name);
        }

        var namecontent = JsonUtility.ToJson(nameslist);
        File.WriteAllText(namepath, namecontent);

        dataman.SaveGameData();
        SaveData saver = dataman.save;
        var path = Application.streamingAssetsPath + "/" + name + ".json";
        if (!string.IsNullOrEmpty(path))
        {
            var content = JsonUtility.ToJson(saver);
            File.WriteAllText(path, content);
        }
    }
    public void LoadRoom(string name)
    {
        NewRoom();
        ToggleLoadMenu();
        var path = Application.streamingAssetsPath + "/" + name + ".json";
        if (!string.IsNullOrEmpty(path))
        {
            var content = File.ReadAllText(path);
            SaveData saver = JsonUtility.FromJson<SaveData>(content);
            dataman.LoadGameData(saver);
        }
        placer.CalculateAdjacentCells();
    }

    public void QuitProgram()
    {
        Application.Quit();
    }

    public void ToggleMenuUI()
    {
        MenuUI.SetActive(!MenuUIState);
        MenuUIState = !MenuUIState;
    }

    public void ToggleObjectUI()
    {
        ObjectUIAnim.SetBool("IsActive", !ObjectUIState);
        ObjectUIState = !ObjectUIState;
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
