using System;
using System.Collections.Generic;
using CameraDoorScript;
using Unity.VisualScripting;
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
    
    // tuple of slope and intercept
    List<Tuple<float, float>> slopeInterPath;
    List<Vector3> exitNodesPos;

    float slope;
    float yInter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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

        exitNodesPos = new List<Vector3>();

        QrCodeReader.qrScannedEvent.AddListener(ScannedQRPath);
    }

    // Update is called once per frame
    void Update()
    { 
        if (path != null) {
            UpdateSplineRealtime();
            if (currentPhone.position.z > slope * currentPhone.position.x + yInter)
            {
                Debug.Log("true");
            }
        }
    }

    public void ScannedQRPath()
    {
        buildingName = QrCodeReader.roomData.building_name;

        FillExitNodesPositions();

        SetSplinePath();

        Vector3 vec = path[1] - path[0];
        Vector3 vec1 = path[2] - path[1];
        Vector3 vec2 = path[3] - path[2];
        Vector3 vec3 = path[4] - path[3];

        float angle = Vector3.SignedAngle(vec, vec1, Vector3.up)/2;
        float angle1 = Vector3.SignedAngle(vec1, vec2, Vector3.up)/2;
        float angle2 = Vector3.SignedAngle(vec2, vec3, Vector3.up)/2;
        Debug.Log("path angle: " + angle);
        Debug.Log("path angle 1: " + angle1);
        Debug.Log("path angle 2: " + angle2);

        Quaternion rot = Quaternion.AngleAxis(angle1, Vector3.up);
        Quaternion rot1 = Quaternion.AngleAxis(angle2, Vector3.up);

        Vector3 cords = rot * vec2;
        Vector3 cords1 = rot1 * vec3;

        Debug.Log("path coords 1: " + cords.ToString());
        Debug.Log("path coords 2: " + cords1.ToString());

        Vector3 pointOfSlope = path[2] + cords;
        Vector3 pointOfSlope1 = path[3] + cords1;
        Debug.Log("point of slope: " + pointOfSlope.ToString());
        Debug.Log("point of slope: " + pointOfSlope1.ToString());

        slope = (path[2].z - pointOfSlope.z) / (path[2].x - pointOfSlope.x);
        float slope1 = (path[3].z - pointOfSlope1.z) / (path[3].x - pointOfSlope1.x);
        Debug.Log("slope: " + slope);
        Debug.Log("slope: " + slope1);

        yInter = (slope * (0.0f - path[2].x)) + path[2].z;
        float yInter1 = (slope1 * (0.0f - path[3].x)) + path[3].z;

        Debug.Log("y - intercept: " + yInter);
        Debug.Log("y - intercept1: " + yInter1);


        Debug.Log("path angle: " + Vector3.SignedAngle(vec, vec1, Vector3.up)/2);

        // for (int i = 0; i < path.Count; i++)
        // {
        //     path[0];
        // }

        Debug.Log("path: " + path[0].ToString());
        Debug.Log("path: " + path[1].ToString());
        Debug.Log("path: " + path[2].ToString());
        Debug.Log("path: " + path[3].ToString());
    }

    public void CalcPathSlopeIntercepts()
    {
        for (int i = 2; i < path.Count; i++)
        {
            Vector3 intial = path[i-1] - path[i-2];
            Vector3 next = path[i] - path[i-1];

            // angle of two vectors
            float angle = Vector3.SignedAngle(intial, next, Vector3.up)/2;
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);

            // unit coordinates for slope 
            Vector3 coords = rot * next;

            // coordinates for slope at path
            Vector3 pointOfSlope = path[i-1] + coords;
            float slope = (path[i-1].z - pointOfSlope.z) / (path[i-1].x - pointOfSlope.x);
            float yInter = (slope * (0.0f - path[i-1].x)) + path[i-1].z;

            slopeInterPath.Add(new Tuple<float, float>(slope, yInter));
        }
    }

    public void UpdateSplineRealtime()
    {
        // Vector Math EX:
        // Vector1 point a - b
        // Vector2 point a - b
        // Get Angle
        // Get cords

        Vector3 currentPosition = new Vector3(currentPhone.position.x, 0.0f, currentPhone.position.z);
        spline[0].SetKnot(0, new BezierKnot(currentPosition));
        // for (int i = 0; i < path.Count; i++)
        // {
        //     spline[0].Add(new BezierKnot(path[i]));
        // }
    }

    public void SetSplinePath()
    {
        path = FindPathLinear(buildingName);
        spline[0].Clear();
        for (int i = 0; i < path.Count; i++)
        {
            spline[0].Add(new BezierKnot(path[i]));
        }
    }

    public void FillExitNodesPositions()
    {
        List<ExitNodes> exitNodes = db.GetExitNodes(buildingName);
        for (int i = 0; i < exitNodes.Count; i++)
        {
            exitNodesPos.Add(new Vector3(exitNodes[i].PosX, exitNodes[i].PosY, exitNodes[i].PosZ));
        }
    }

    public int FindNearestExitIndex(Vector3 startNode)
    {
        if (exitNodesPos.Count == 0)
        {
            FillExitNodesPositions();
        }

        int i = 0;
        float distance = Vector3.Distance(startNode, exitNodesPos[0]);
        for (int j = 1; j < exitNodesPos.Count; j++)
        {
            float nearest = Vector3.Distance(startNode, exitNodesPos[i]);
            if (distance > nearest)
            {
                i = j;
                distance = nearest;
            }
        }
        return i;
    }

    public List<Vector3> FindPathLinear(string buildingName)
    {     
        Room qrData = QrCodeReader.roomData;
        Vector3 qrPos = new Vector3(qrData.pos_x, 0.0f, qrData.pos_z);
        float z = db.GetNodes(buildingName)[0].PosZ;
        Vector3 startNodePos = new Vector3(qrData.pos_x, 0.0f, z);
        
        int index = FindNearestExitIndex(qrPos);
        Debug.Log("index of nearest: " + index);
        Vector3 exitNodePos = exitNodesPos[index];
        exitNodesPos.RemoveAt(index);

        Vector3 endNodePos = new Vector3(exitNodePos.x, 0.0f, z);
        
        return ToRealWorldPoints(new List<Vector3>(){qrPos, startNodePos, endNodePos, exitNodePos});
    }   

    public List<Vector3> ToRealWorldPoints(List<Vector3> editorCords)
    {
        Vector3 currentPosition = new Vector3(currentPhone.position.x, 0.0f, currentPhone.position.z);

        Transform qrTrans = CameraOpenDoor.qr.transform;
        Vector3 qrPos = new Vector3(qrTrans.position.x, 0.0f, qrTrans.position.z);
        Debug.Log("qr world transform: " + qrPos.x + " " + qrPos.y + " " + qrPos.z + " ");  

        List<Vector3> nodes = new List<Vector3>(){qrPos};
        for (int i = 1; i < editorCords.Count; i++)
        {
            Vector3 pos = editorCords[i] - editorCords[i - 1];
            Vector3 newPos = nodes[i - 1] + pos;
            nodes.Add(newPos);
        }
        nodes.Insert(0, currentPosition);
        return nodes;
    }

    public void RerouteExit()
    {
        SetSplinePath();
    }   
}
