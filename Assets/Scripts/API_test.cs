using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class API_test : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemTitleUI;
    [SerializeField] private TextMeshProUGUI _itemCompletedUI;

    private const string URL = "http://jsonplaceholder.typicode.com/todos";
    //private const string URL = "http://localhost:59105/ArObjects";

    public void GenerateRequest()
    {
        StartCoroutine(ProcessRequest(URL));
    }

    private IEnumerator ProcessRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();

            if(request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                // Debug.Log(request.downloadHandler.text);

                JSONNode itemsData = JSON.Parse(request.downloadHandler.text);


                int randomNum = Random.Range(1, itemsData.Count);

                Debug.Log("The generated item is: \nTitle: " + itemsData[randomNum]["title"]);
                //Debug.Log("The generated item is: \nName: " + itemsData[randomNum]["name"]);

                string itemTitle = itemsData[randomNum]["title"];
                bool itemCompleted = itemsData[randomNum]["completed"];

                _itemTitleUI.text = itemTitle;
                _itemCompletedUI.text = itemCompleted.ToString();
            }
        }
    }
}
