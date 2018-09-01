using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
   
    public void SetCameraTo(string roomName)
    {
        if (MapData.instance.cameraTransDict.ContainsKey(roomName))
        {
            transform.SetParent(MapData.instance.cameraTransDict[roomName]);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetCameraTo("走廊");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetCameraTo("大厅");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetCameraTo("二楼");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetCameraTo("小巷");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetCameraTo("月之间");
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SetCameraTo("卧室");
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            SetCameraTo("英灵殿");
        }
    }

}
