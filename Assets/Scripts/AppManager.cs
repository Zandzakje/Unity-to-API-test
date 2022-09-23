//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using System.Linq;
using SimpleJSON;
using System.Collections;

public class AppManager : MonoBehaviour
{
    public static AppManager instance;
    //public static AppManager instance { get; private set; }

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string url;
    public JSONNode jsonResult;

    public enum Duration
    {
        Today = 0,
        Week = 1,
        Month = 2,
        All = 3
    }

    IEnumerator GetData(string location)
    {
        // create the web request and download handler
        UnityWebRequest webReq = new UnityWebRequest();
        webReq.downloadHandler = new DownloadHandlerBuffer();
        // build the url and query
        webReq.url = string.Format("{0}&q={1}", url, location);
        // send the web request and wait for a returning result
        yield return webReq.SendWebRequest();
        // convert the byte array and wait for a returning result
        string rawJson = Encoding.Default.GetString(webReq.downloadHandler.data);
        // parse the raw string into a json result we can easily read
        jsonResult = JSON.Parse(rawJson);
    }

    public void FilterByDuration(int durIndex)
    {
        Duration dur = (Duration)durIndex;

        JSONArray records = jsonResult["result"]["records"].AsArray;

        DateTime maxDate = new DateTime();

        switch(dur)
        {
            case Duration.Today:
                maxDate = DateTime.Now.AddDays(1);
                break;
            case Duration.Week:
                maxDate = DateTime.Now.AddDays(7);
                break;
            case Duration.Month:
                maxDate = DateTime.Now.AddMonths(1);
                break;
            case Duration.All:
                maxDate = DateTime.MaxValue;
                break;
        }

        JSONArray filteredRecords = new JSONArray();

        for(int x = 0; x < records.Count; ++x)
        {
            DateTime recordDate = DateTime.Parse(records[x]["Display Date"]);

            if(recordDate.Ticks < maxDate.Ticks)
            //{
                filteredRecords.Add(records[x]);
            //}

            UI.instance.SetSegments(filteredRecords);
        }
    }
}
