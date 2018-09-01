using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    public static MapData instance = null;
    private MapData() { }
    private void Awake()
    {
        instance = this;
    }
    public CameraManager cm;
    public int roomCount = 0;
    public Dictionary<int, string> roomNameDict = new Dictionary<int, string>();
    public Dictionary<string, Transform> cameraTransDict = new Dictionary<string, Transform>();
    public Dictionary<string, PlayerCell[]> playerPositionDict = new Dictionary<string, PlayerCell[]>();
    public Dictionary<string, Vector3> centerPointDict = new Dictionary<string, Vector3>();
    private void Start()
    {
        cm = Camera.main.GetComponent<CameraManager>();
        Transform map = transform;
        for (int i = 0; i < map.childCount; i++)
        {
            Transform temp = map.GetChild(i);
            cameraTransDict.Add(temp.name, temp.Find("CameraPosition"));
            PlayerCell[] playerCells = temp.GetComponentsInChildren<PlayerCell>();
            playerPositionDict.Add(temp.name, playerCells);
            roomNameDict.Add(i, temp.name);
            centerPointDict.Add(temp.name, temp.Find("CenterPoint").position);
        }
        roomCount = roomNameDict.Count;
    }
    public void SetPlayerToRoom(int netIndex, Transform player, string room)
    {
        cm.SetCameraTo(room);
        Transform cell = playerPositionDict[room][netIndex].transform;
        player.transform.SetParent(cell);
        player.localPosition = Vector3.zero;
        player.localRotation = Quaternion.identity;
        player.transform.SetParent(null);
    }
    public void SetCenterPoint(Transform obj,string room)
    {
        obj.position = centerPointDict[room];
    }
}
