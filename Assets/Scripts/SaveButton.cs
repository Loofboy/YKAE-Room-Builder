using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveButton : MonoBehaviour
{
    public UIController Controller;
    public TextMeshProUGUI input;
    // Start is called before the first frame update
    public void PressSave()
    {
        Controller.SaveRoom(input.text);
        Controller.ToggleSaveMenu();
    }
}
