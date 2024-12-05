using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;


public class RoomListItem : MonoBehaviour
{
    [SerializeField] Text RoomNameText;
    public RoomInfo info;


    public void SetUp(RoomInfo _info)
    {
        info = _info;
        RoomNameText.text = info.Name;

    }
    public void onClick()
    {
        Launcher.Instance.JoinRoom(info);
    }
}
