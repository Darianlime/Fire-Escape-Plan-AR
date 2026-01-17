using System.Collections.Generic;
using CameraDoorScript;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class PathFinding : MonoBehaviour
{
    [SerializeField] Transform currentPhone;

    DatabaseManager db;
    SplineContainer spline;
    SplineInstantiate splineInit;
    Room qrData;
    string buildingName;

    List<Vector3> path;
    List<ExitNodes> exitNodes;
    List<Vector3> exitNodesPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        db = GetComponent<DatabaseManager>();
        spline = GetComponent<SplineContainer>();
        splineInit = GetComponent<SplineInstantiate>();
        splineInit.Clear();
        foreach (Spline s in spline.Splines) {
            spline.RemoveSpline(s);
        }
        spline.AddSpline();
        spline[0].Clear();
        spline[0].Add(new BezierKnot(new Vector3(0.0f, 0.0f, 0.0f)));
    }

    // Update is called once per frame
    void Update()
    {
        if (QrCodeReader.qrScanned)
        {
            buildingName = QrCodeReader.roomData.building_name;
            exitNodes = db.GetExitNodes(buildingName);
            for (int i = 0; i < exitNodes.Count; i++)
            {
                exitNodesPos.Add(new Vector3(exitNodes[i].PosX, exitNodes[i].PosY, exitNodes[i].PosZ));
            }
            path = FindPathLinear(buildingName);
            spline[0].Clear();
            for (int i = 0; i < path.Count; i++)
            {
                spline[0].Add(new BezierKnot(path[i]));
            }
            Debug.Log("path: " + path[0].ToString());
            Debug.Log("path: " + path[1].ToString());
            Debug.Log("path: " + path[2].ToString());
            Debug.Log("path: " + path[3].ToString());
            QrCodeReader.qrScanned = false;
        }   
    }

    public int FindNearestExitIndex(Vector3 startNode)
    {
        int i = 0;
        float distance = Vector3.Distance(startNode, exitNodesPos[0]);
        for (int j = 1; j < exitNodes.Count; j++)
        {
            float nearest = Vector3.Distance(startNode, exitNodesPos[i]);
            if (distance > nearest)
            {
                i = j;
                distance = nearest;
            }
        }
        exitNodes.RemoveAt(i);
        return i;
    }

    public List<Vector3> FindPathLinear(string buildingName)
    {     
        Vector3 currentPosition = new Vector3(currentPhone.position.x, 0.0f, currentPhone.position.z);

        Room qrData = QrCodeReader.roomData;
        Vector3 qrPos = new Vector3(qrData.pos_x, 0.0f, qrData.pos_z);
        float z = db.GetNodes(buildingName)[0].PosZ;
        Vector3 startNodePos = new Vector3(qrData.pos_x, 0.0f, z);

        Vector3 exitNodePos = exitNodesPos[FindNearestExitIndex(qrPos)];
        Vector3 endNodePos = new Vector3(exitNodePos.x, 0.0f, z);
        
        return ToRealWorldPoints(new List<Vector3>(){currentPosition, startNodePos, endNodePos, exitNodePos});
    }   

    public List<Vector3> ToRealWorldPoints(List<Vector3> editorCords)
    {
        Transform qrTrans = CameraOpenDoor.qr.transform;
        Vector3 qrPos = new Vector3(qrTrans.position.x, 0.0f, qrTrans.position.z);
        Debug.Log("qr world transform: " + qrPos.x + " " + qrPos.y + " " + qrPos.z + " ");  

        List<Vector3> nodes = new List<Vector3>(){editorCords[0], qrPos};
        for (int i = 2; i < editorCords.Count; i++)
        {
            Vector3 pos = editorCords[i] - editorCords[i - 1];
            Vector3 newPos = nodes[i - 1] + pos;
            nodes.Add(newPos);
        }
        return nodes;
    }

    public void RerouteExit()
    {
        List<Vector3> path = FindPathLinear(buildingName);
        
    }   
}
