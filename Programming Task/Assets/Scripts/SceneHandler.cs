using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class DataObject
{
	public string question;
	public string published_at;
	public ArrayList choice;
	public ArrayList votes;
}

public class SceneHandler : MonoBehaviour
{
	public Text questionText;

	public Transform content;

	public RectTransform palettePrefab;

	public Button refreshButton;

	string jsonPath;

	float positionVal = 20f;

	//Creates the objects of the data from json
	private void CreateObjects (string choice, string votes)
	{
		RectTransform rt = (RectTransform)Instantiate (palettePrefab, new Vector3 (0f, 0f, 0f), Quaternion.identity);
		rt.transform.SetParent (content.transform, false);
		rt.anchoredPosition = new Vector2 (positionVal, 0);
		positionVal += rt.rect.width + 200f;
		content.GetComponent<RectTransform> ().sizeDelta = new Vector2 (positionVal, 200);

		rt.Find ("Choice").GetComponent<Text> ().text = choice;
		rt.Find ("Votes").GetComponent<Text> ().text = votes;
		refreshButton.GetComponent<Button> ().interactable = true;
	}

	public void Refresh ()
	{
		jsonPath = Application.dataPath + ("/Resources/questions.json");

		//Checks if the local Resources folder exists or not
		//If doesn't then creates one
		if (!System.IO.Directory.Exists (Application.dataPath + "/Resources"))
			System.IO.Directory.CreateDirectory (Application.dataPath + "/Resources");

		StartCoroutine (DownloadAndProcessJSONFile ());
	}

	private IEnumerator DownloadAndProcessJSONFile ()
	{
		string url = "https://private-5b1d8-sampleapi187.apiary-mock.com/questions";

		if (File.Exists (jsonPath)) {
			WWW www = new WWW (url);
			yield return www;

			//Checks if the database on the server has changed
			//If not then loads data locally
			if (www.text.Equals (File.ReadAllText (jsonPath)))
				ProcessJSONFile (File.ReadAllText (jsonPath));
		
			Debug.Log ("File exists!");
		} else {
			WWW www = new WWW (url);

			if (!www.isDone)
				refreshButton.GetComponent<Button> ().interactable = false;
			
			yield return www;

			if (www.error == null) {
				//Download the database locally the first time
				//or If there is any kind of updated data on the server database overwrites the local file
				//Write to json file saved to lacal directory and then load the data
				File.WriteAllText (jsonPath, www.text);
				ProcessJSONFile (File.ReadAllText (jsonPath));
			} else
				Debug.Log ("ERROR: " + www.error);
		}
	}

	//Processes json string and retrieve the formatted data
	private void ProcessJSONFile (string jsonString)
	{
		JsonData jData = JsonMapper.ToObject (jsonString);

		DataObject dObj = new DataObject ();

		questionText.text = dObj.question = jData [0] ["question"].ToString ();

		dObj.published_at = jData [0] ["published_at"].ToString ();

		dObj.choice = dObj.votes = new ArrayList ();

		for (int i = 0; i < jData [0] ["choices"].Count; i++) {
			dObj.choice.Add (jData [0] ["choices"] [i] ["choice"].ToString ());
			dObj.votes.Add (jData [0] ["choices"] [i] ["votes"].ToString ());

			CreateObjects (jData [0] ["choices"] [i] ["choice"].ToString (), jData [0] ["choices"] [i] ["votes"].ToString ());
		}
	}
}