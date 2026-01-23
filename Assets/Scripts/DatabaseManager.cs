using UnityEngine;
using System.IO;
using SQLite;
using Unity.VisualScripting;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using System.Data.Common;

public class Buildings
{
    public string BuildingName { get; set; }
}

public class Egdes
{
    public string BuildingName { get; set; }
    public int Floor { get; set; }
    public int FromNode { get; set; }
    public int ToNode { get; set; }
}

public class Nodes
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string BuildingName { get; set; }
    public int Floor { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }
}

public class ExitNodes
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string BuildingName { get; set; }
    public int Floor { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }
}

// public class RoomNodes
// {
//     public string BuildingName { get; set; }
//     public int Floor { get; set; }
//     public int RoomNumber { get; set; }
//     public float PosX { get; set; }
//     public float PosY { get; set; }
//     public float PosZ { get; set; }
// }

public class DatabaseManager : MonoBehaviour
{
    public SQLiteConnection db; 
    public GameObject nodes;
    public GameObject exitNodes;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        string path = Path.Combine(Application.persistentDataPath, "fire_escape.db");
        Debug.Log(Application.persistentDataPath);
        db = new SQLiteConnection(path);

        //Debug.Log(GetNodes("Tower")[3].Id);
        //Debug.Log(GetExitNodes("Tower")[1].Id);

        // db.CreateTable<Buildings>();
        // // //db.CreateTable<RoomNodes>();
        // db.CreateTable<Nodes>();
        // db.CreateTable<ExitNodes>();
        // // db.CreateTable<Egdes>();

        // for (int i = 0; i < exitNodes.transform.childCount; i++)
        // {
        //     Vector3 pos = exitNodes.transform.GetChild(i).transform.position;
        //     AddExitNode(db, "Tower", 6, new Vector3(pos.x, pos.y, pos.z));
        // }

        // for (int i = 0; i < nodes.transform.childCount; i++)
        // {
        //     Vector3 pos = nodes.transform.GetChild(i).transform.position;
        //     AddNode(db, "Tower", 6, new Vector3(pos.x, pos.y, pos.z));
        // }
    }

    public List<Nodes> GetNodes(string buildingName) {
        return db.Table<Nodes>().Where(b => b.BuildingName == buildingName).ToList();
    }

    public List<ExitNodes> GetExitNodes(string buildingName) {
        return db.Table<ExitNodes>().Where(b => b.BuildingName == buildingName).ToList();
    }

    public static void AddNode(SQLiteConnection db, string buildingName, int floor, Vector3 pos) {
        var node = new Nodes() {
            BuildingName = buildingName,
            Floor = floor,
            PosX = pos.x,
            PosY = pos.y,
            PosZ = pos.z,
        };
        db.Insert(node);
    }

    public static void AddExitNode(SQLiteConnection db, string buildingName, int floor, Vector3 pos) {
        var node = new ExitNodes() {
            BuildingName = buildingName,
            Floor = floor,
            PosX = pos.x,
            PosY = pos.y,
            PosZ = pos.z,
        };
        db.Insert(node);
    }
}
