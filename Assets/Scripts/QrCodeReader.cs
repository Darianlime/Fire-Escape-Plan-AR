using System.Collections;
using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEngine;
using ZXing;

[System.Serializable]
public class Room
{
    public string building_name;
    public string type;
    public int floor;
    public int room_number;
    public float pos_x;
    public float pos_y;
    public float pos_z;
}

public class QrCodeReader : MonoBehaviour
{
    //private IBarcodeReader barcodeReader;
    public Camera scanCamera;
    public Texture2D qrImage;
    public GameObject scanObject;
    public string qrResult;
    public static bool qrScanned;

    public static Room roomData = null;

    private RectTransform scanZone;
    private int texWidth;
    private int texHeight;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scanCamera = GetComponent<Camera>();
        scanZone = scanObject.GetComponent<RectTransform>();
        texWidth = (int)scanZone.rect.width;
        texHeight = (int)scanZone.rect.height;
        qrResult = "";
        //barcodeReader = new BarcodeReader();
    }

    // Update is called once per frame
    void Update()
    {
        if (!qrScanned)
        {
            Decode();
        }
        // if (qrScanned)
        // {
        //     ReadJson();
        // }
    }

    void Decode()
    {
        // creates new render in memory for camera to save its texture too
        RenderTexture rt = new RenderTexture(texWidth, texHeight, 24);
        // render to render texture and not to screen
        scanCamera.targetTexture = rt;
        // draws current camera pixels texture to render texture "rt"
        scanCamera.Render();

        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(texWidth, texHeight, TextureFormat.RGB24, false);

        //reads from current RenderTexture Target "rt"
        tex.ReadPixels(new Rect(0, 0, texWidth, texHeight), 0, 0);
        tex.Apply();

        scanCamera.targetTexture = null;
        RenderTexture.active = null;
        try {
            var barcodeReader = new BarcodeReader();
            var result = barcodeReader.Decode(tex.GetPixels32(), tex.width, tex.height);
            if (result != null) {
                qrResult = result.Text;
                roomData = JsonUtility.FromJson<Room>(qrResult);
                //roomData.building_name = newRoom.building_name;
                // roomData.floor = newRoom.floor;
                // roomData.type = newRoom.type;
                // roomData.room_number = newRoom.room_number;
                // roomData.pos_x = newRoom.pos_x;
                // roomData.pos_y = newRoom.pos_y;
                // roomData.pos_z = newRoom.pos_z;
                // Debug.Log("Simulated QR Code: " + roomData.building_name);
                // Debug.Log("Simulated QR Code: " + roomData.pos_x);
                // Debug.Log("Simulated QR Code: " + roomData.pos_y);
                qrScanned = true;
            }
        } catch {
            //Debug.Log("No QR detected.");
        }
        Destroy(rt);
        Destroy(tex);
    }

    // void ReadJson()
    // {
    //     Room newRoom = JsonUtility.FromJson<Room>(qrResult);
    //     Debug.Log("Simulated QR Code: " + newRoom.building_name);
    //     Debug.Log("Simulated QR Code: " + newRoom.type);
    //     Debug.Log("Simulated QR Code: " + newRoom.floor);
    //     Debug.Log("Simulated QR Code: " + newRoom.room_number);
    //     Debug.Log("Simulated QR Code: " + newRoom.pos_x);
    //     Debug.Log("Simulated QR Code: " + newRoom.pos_y);
    //     Debug.Log("Simulated QR Code: " + newRoom.pos_z);
    // }
}
