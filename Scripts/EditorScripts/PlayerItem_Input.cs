using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItem_Input : MonoBehaviour
{
    void Start()
    {
        GetComponent<InputField>().characterLimit = 2;
        GetComponent<InputField>().characterValidation = InputField.CharacterValidation.Integer;
        if (GetComponent<InputField>().text.Length == 0) { GetComponent<InputField>().text = "0"; }
    }

}
