using UnityEngine.UI;
using TMPro;
using SimpleJSON;
using System;
using UnityEngine;
using System.Collections.Generic;

public class UI : MonoBehaviour
{
    public static UI instance;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PreLoadSegments(10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public RectTransform container;

    public GameObject segmentPrefab;

    private List<GameObject> segments = new List<GameObject>();

    [Header("Info Dropdown")]

    public RectTransform infoDropdown;

    public TextMeshProUGUI infoDropdownText;

    public TextMeshProUGUI infoDropdownAddressText;

    GameObject CreateNewSegment()
    {
        GameObject segment = Instantiate(segmentPrefab);
        //segment.transform.parent = container.transform;
        segment.transform.SetParent(container.transform);
        segments.Add(segment);

        segment.GetComponent<Button>().onClick.AddListener(() => { OnShowMoreInfo(segment); });

        segment.SetActive(false);

        return segment;
    }

    void PreLoadSegments(int amount)
    {
        for(int x = 0; x < amount; ++x)
        {
            CreateNewSegment();
        }
    }

    public void SetSegments(JSONNode records)
    {
        DeactivateAllSegments();

        for(int x = 0; x < records.Count; ++x)
        {
            // create a new segment if we don't have enough
            GameObject segment = x < segments.Count ? segments[x] : CreateNewSegment();
            segment.SetActive(true);
            // get the location and date texts
            TextMeshProUGUI locationText = segment.transform.Find("LocationText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI dateText = segment.transform.Find("DateText").GetComponent<TextMeshProUGUI>();
            // set them
            locationText.text = records[x]["Suburb"];
            dateText.text = GetFormattedDate(records[x]["Display Date"]);
        }
        
        container.sizeDelta = new Vector2(container.sizeDelta.x, GetContainerHeight(records.Count));
    }

    void DeactivateAllSegments()
    {
        foreach(GameObject segment in segments)
        {
            segment.SetActive(false);
        }
    }

    string GetFormattedDate(string rawDate)
    {
        DateTime date = DateTime.Parse(rawDate);

        return string.Format("{0}/{1}/{2}", date.Day, date.Month, date.Year);
    }

    float GetContainerHeight(int count)
    {
        float height = 0.0f;

        height += count * (segmentPrefab.GetComponent<RectTransform>().sizeDelta.y + 1);
        
        height += count * container.GetComponent<VerticalLayoutGroup>().spacing;
        
        height += infoDropdown.sizeDelta.y;

        return height;
    }

    public void OnShowMoreInfo(GameObject segmentObject)
    {
        // get the index of the segment
        int index = segments.IndexOf(segmentObject);
        // if we're pressing the segment that's already open, close the dropdown
        if (infoDropdown.transform.GetSiblingIndex() == index + 1 && infoDropdown.gameObject.activeInHierarchy)
        {
            infoDropdown.gameObject.SetActive(false);
            return;
        }
        infoDropdown.gameObject.SetActive(true);
        // get only the records
        JSONNode records = AppManager.instance.jsonResult["result"]["records"];
        // set the dropdown to appear below the selected segment
        infoDropdown.transform.SetSiblingIndex(index + 1);
        // set dropdown info text
        infoDropdownText.text += "Starts at " + GetFormattedTime(records[index]["Times(s)"]);
        infoDropdownText.text += "\n" + records[index]["Event Type"] + " Event";
        infoDropdownText.text += "\n" + records[index]["Display Type"];
        // set dropdown address text
        if (records[index]["Display Address"].ToString().Length > 2)
            infoDropdownAddressText.text = records[index]["Display Address"];
        else
            infoDropdownAddressText.text = "Address not specified";

    }

    string GetFormattedTime(string rawTime)
    {
        // get the hours and minutes from the raw time
        string[] split = rawTime.Split(":"[0]);
        int hours = int.Parse(split[0]);
        // converts it to "[hours]:[mins] (AM / PM)"
        return string.Format("{0}:{1} {2}", hours > 12 ? hours - 12 : hours, split[1], hours > 12 ? "PM" : "AM");
    }

    public void OnSearchBySuburb(TextMeshProUGUI input)
    {
        // get and set the data
        AppManager.instance.StartCoroutine("GetData", input.text);
        // disable the info dropdown
        infoDropdown.gameObject.SetActive(false);
    }
}
