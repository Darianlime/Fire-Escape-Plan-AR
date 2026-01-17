using UnityEngine;

public class SimulatePhoneRotation : MonoBehaviour
{
    public float sensitivity = 2f;

    float yaw;
    float pitch;

    void LateUpdate()
    {
    #if UNITY_EDITOR
        if (Input.GetMouseButton(1)) 
        {
            yaw += Input.GetAxis("Mouse X") * sensitivity;
            pitch -= Input.GetAxis("Mouse Y") * sensitivity;
            pitch = Mathf.Clamp(pitch, -85f, 85f);

            transform.localRotation = Quaternion.Euler(pitch, yaw, 0);
        }
        Debug.Log("Mouse X: " + Input.GetAxis("Mouse X") + " | Mouse Y: " + Input.GetAxis("Mouse Y"));
    #endif
    }
}