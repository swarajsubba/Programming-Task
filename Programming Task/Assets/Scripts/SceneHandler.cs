using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    public Text questionText;

    public Transform content;

    public RectTransform palettePrefab;

    float positionVal = 20f;

    public bool hasData = false;

    //Creates the objects of the data from json
    public void CreateObjects(string choice, string votes)
    {
        if (!hasData)
        {
            RectTransform rt = (RectTransform)Instantiate(palettePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            rt.transform.SetParent(content.transform, false);
            rt.anchoredPosition = new Vector2(positionVal, 0);
            positionVal += rt.rect.width + 200f;
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(positionVal, 200);

            //Debug.Log(positionVal);
            rt.Find("Choice").GetComponent<Text>().text = choice;
            rt.Find("Votes").GetComponent<Text>().text = votes;
        }
    }
}