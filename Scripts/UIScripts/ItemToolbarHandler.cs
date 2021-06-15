using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolbarHandler : MonoBehaviour
{
    public GameObject btnWire;
    public GameObject btnMove;
    public GameObject btnDelete;

    public void ShowToolbar(Vector2 position, GameObject obj)
    {
        Vector2 offset = new Vector2(0, btnWire.GetComponent<RectTransform>().rect.height / 2);
        gameObject.transform.localPosition = position - (offset);

        if (obj.GetComponent<PuzzleObjectBase>().bMovable)
        {
            btnMove.GetComponent<Button>().interactable = true;
            btnDelete.GetComponent<Button>().interactable = true;
        }
        else
        {
            btnMove.GetComponent<Button>().interactable = false;
            btnDelete.GetComponent<Button>().interactable = false;
        }

        if (obj.GetComponent<PuzzleObjectBase>().bWirable)
        {
            btnWire.GetComponent<Button>().interactable = true;
        }
        else
        {
            btnWire.GetComponent<Button>().interactable = false;
        }

        gameObject.SetActive(true);
    }
}
