using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class API_test : MonoBehaviour
{
    [Header("Get all")]
    [SerializeField] private GameObject recordPrefab;
    List<GameObject> records = new List<GameObject>();

    [Header("Get by id")]
    [SerializeField] private TMP_InputField idId;
    [SerializeField] private TextMeshProUGUI nameId;
    [SerializeField] private TextMeshProUGUI objectTypeId;

    [Header("Create")]
    [SerializeField] private TMP_InputField nameCreate;
    [SerializeField] private TMP_InputField objectTypeCreate;
    [SerializeField] private TMP_InputField latitudeCreate;
    [SerializeField] private TMP_InputField longitudeCreate;
    [SerializeField] private TMP_InputField altitudeCreate;
    [SerializeField] private TMP_InputField roomIdCreate;
    [SerializeField] private TextMeshProUGUI nameCreateResult;
    [SerializeField] private TextMeshProUGUI objectTypeCreateResult;

    [Header("Update")]
    [SerializeField] private TMP_InputField idUpdate;
    [SerializeField] private TMP_InputField nameUpdate;
    [SerializeField] private TMP_InputField objectTypeUpdate;
    [SerializeField] private TMP_InputField latitudeUpdate;
    [SerializeField] private TMP_InputField longitudeUpdate;
    [SerializeField] private TMP_InputField altitudeUpdate;
    [SerializeField] private TMP_InputField roomIdUpdate;
    [SerializeField] private TextMeshProUGUI nameUpdateResult;
    [SerializeField] private TextMeshProUGUI objectTypeUpdateResult;

    [Header("Delete")]
    [SerializeField] private TMP_InputField idDelete;

    private const string BaseURL = "http://localhost:59105/";

    public void GenerateRequest_GetAll()
    {
        StartCoroutine(ProcessRequest_GetAll(BaseURL));
    }

    public void GenerateRequest_GetById()
    {
        StartCoroutine(ProcessRequest_GetById(BaseURL, idId.text));
    }

    public void GenerateRequest_Create()
    {
        StartCoroutine(ProcessRequest_Create(BaseURL));
    }

    public void GenerateRequest_Update()
    {
        StartCoroutine(ProcessRequest_Update(BaseURL, idUpdate.text));
    }

    public void GenerateRequest_Delete()
    {
        StartCoroutine(ProcessRequest_Delete(BaseURL, idDelete.text));
    }

    private IEnumerator ProcessRequest_GetAll(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri + "ArObjects"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Status Code: " + request.responseCode);

                JSONNode itemsData = JSON.Parse(request.downloadHandler.text);
                JSONArray result = itemsData.AsArray;
                for(int i = 0; i < result.Count; i++)
                {
                    GameObject record = Instantiate(recordPrefab);
                    record.transform.SetParent(GameObject.Find("RecordList").transform);
                    float yPosition = 0 - (i * 55);
                    record.transform.localPosition = new Vector3(0f, yPosition, 0f);
                    TextMeshProUGUI nameText = record.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI objectTypeText = record.transform.Find("ObjectType").GetComponent<TextMeshProUGUI>();
                    nameText.text = result[i]["name"];
                    objectTypeText.text = result[i]["objectType"];
                    records.Add(record);
                }
            }
        }
    }

    private IEnumerator ProcessRequest_GetById(string uri, string id)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri + "ArObjects/" + id))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Status Code: " + request.responseCode);
                JSONNode data = JSON.Parse(request.downloadHandler.text);

                string name = data["name"];
                string objectType = data["objectType"];

                nameId.text = name;
                objectTypeId.text = objectType;
            }
        }
    }

    private IEnumerator ProcessRequest_Create(string uri)
    {
        ArObjectModel model = new ArObjectModel(nameCreate.text, objectTypeCreate.text, float.Parse(latitudeCreate.text), float.Parse(longitudeCreate.text),
            float.Parse(altitudeCreate.text), int.Parse(roomIdCreate.text));

        string json = JsonUtility.ToJson(model);
        UnityWebRequest request = UnityWebRequest.Post(uri + "ArObjects", "");
        byte[] jsonToSend = new UTF8Encoding().GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        using (request)
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Status Code: " + request.responseCode);
                JSONNode data = JSON.Parse(request.downloadHandler.text);

                string name = data["name"];
                string objectType = data["objectType"];

                nameCreateResult.text = name;
                objectTypeCreateResult.text = objectType;
            }
        }
    }

    private IEnumerator ProcessRequest_Update(string uri, string id)
    {
        ArObjectModel model = new ArObjectModel(nameUpdate.text, objectTypeUpdate.text, float.Parse(latitudeUpdate.text), float.Parse(longitudeUpdate.text),
            float.Parse(altitudeUpdate.text), int.Parse(roomIdUpdate.text));

        string json = JsonUtility.ToJson(model);
        UnityWebRequest request = UnityWebRequest.Put(uri + "ArObjects/" + id, "fillerStringOtherwiseTheProgramGivesAnErrorHereThatThisFieldCantBeEmpty");
        byte[] jsonToSend = new UTF8Encoding().GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        using (request)
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Status Code: " + request.responseCode);
                JSONNode data = JSON.Parse(request.downloadHandler.text);

                string name = data["name"];
                string objectType = data["objectType"];

                nameUpdateResult.text = name;
                objectTypeUpdateResult.text = objectType;
            }
        }
    }

    private IEnumerator ProcessRequest_Delete(string uri, string id)
    {
        using (UnityWebRequest request = UnityWebRequest.Delete(uri + "ArObjects/" + id))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Status Code: " + request.responseCode);
            }
        }
    }
}
