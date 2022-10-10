using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ArObjectModel
{
    public ArObjectModel() { }

    public ArObjectModel(string name, string objectType, float latitude, float longitude, float altitude, int roomId)
    {
        Name = name;
        ObjectType = objectType;
        Latitude = latitude;
        Longitude = longitude;
        Altitude = altitude;
        RoomId = roomId;
    }

    public string Name; //{ get; set; }
    public string ObjectType; //{ get; set; }
    public float Latitude; //{ get; set; }
    public float Longitude; //{ get; set; }
    public float Altitude; //{ get; set; }
    public int RoomId; //{ get; set; }
}
