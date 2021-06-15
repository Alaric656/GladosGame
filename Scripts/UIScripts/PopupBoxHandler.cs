using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupBoxHandler : MonoBehaviour
{
    //public Vector2 offset = new Vector2(0, 50);
    public GameObject text;
    public Canvas UICanvas;

    public void ShowText(string newText, Vector2 position)
    {
        text.GetComponent<TextMeshProUGUI>().text = newText;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        Vector2 offset = new Vector2(0, gameObject.GetComponent<RectTransform>().rect.height/2);
        gameObject.transform.localPosition = position + (offset);
    }
}
