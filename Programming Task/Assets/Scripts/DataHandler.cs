using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class DataObject
{
    public string question;
    public string published_at;
    public ArrayList choice;
    public ArrayList votes;
}

public class DataHandler : MonoBehaviour
{
    public SceneHandler sceneHandle;

    public Button refreshButton;

    public void Refresh()
    {
        jsonPath = Application.dataPath + ("/Resources/questions.json");

        StartCoroutine(DownloadAndProcessJSONFile());
    }

    string jsonPath;

    IEnumerator DownloadAndProcessJSONFile()
    {
        if (File.Exists(jsonPath))
        {
            ProcessJSONFile(File.ReadAllText(jsonPath));
        }
        else
        {
            string url = "https://private-5b1d8-sampleapi187.apiary-mock.com/questions";
            WWW www = new WWW(url);

            if (!www.isDone)
                refreshButton.interactable = false;

            yield return www;
            if (www.error == null)
            {
                SaveJson(www.text);
                refreshButton.interactable = true;
            }
            else
            {
                Debug.Log("ERROR: " + www.error);
            }
        }
    }

    //Save json file to lacal directory and then load the data
    void SaveJson(string jsonString)
    {
        File.WriteAllText(jsonPath, jsonString);
        ProcessJSONFile(File.ReadAllText(jsonPath));
    }

    //Processes json string and retrieve the formatted data
    private void ProcessJSONFile(string jsonString)
    {
        JsonData jData = JsonMapper.ToObject(jsonString);

        DataObject dObj = new DataObject();

        sceneHandle.questionText.text = dObj.question = jData[0]["question"].ToString();

        dObj.published_at = jData[0]["published_at"].ToString();

        dObj.choice =
                dObj.votes = new ArrayList();

        for (int i = 0; i < jData[0]["choices"].Count; i++)
        {
            dObj.choice.Add(jData[0]["choices"][i]["choice"].ToString());
            dObj.votes.Add(jData[0]["choices"][i]["votes"].ToString());

            sceneHandle.CreateObjects(jData[0]["choices"][i]["choice"].ToString(), jData[0]["choices"][i]["votes"].ToString());

            if (i == jData[0]["choices"].Count) sceneHandle.hasData = true;
        }
    }
}