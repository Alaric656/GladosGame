using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolkit_objectButton : MonoBehaviour
{

    private GameObject thisObject;
    public void SetObject(GameObject newObject, string Text="")
    {
        thisObject = newObject;
        transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = thisObject.GetComponent<SpriteRenderer>().sprite;
        //if(thisObject.GetComponent<SpriteRenderer>().sprite.rect.width < GetComponent<RectTransform>().rect.width || thisObject.GetComponent<SpriteRenderer>().sprite.rect.height < GetComponent<RectTransform>().rect.height)
        //{
        transform.Find("Image").localScale = (thisObject.GetComponent<SpriteRenderer>().sprite.rect.size / thisObject.GetComponent<SpriteRenderer>().sprite.rect.size.magnitude);
        //}
        transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = Text;
    }
    public GameObject GetObject()
    {
        return thisObject;
    }

    public void OnClick()//called in the unity UI controls, inspect properties of this GameObject
    {
        GameObject.Find("Toolkit").GetComponent<ToolkitController>().SelectTileToBuild(thisObject);
    }

}
