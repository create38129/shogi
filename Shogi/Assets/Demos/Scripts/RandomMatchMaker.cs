using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityChan;

public class RandomMatchMaker : MonoBehaviourPunCallbacks
{
    public GameObject photonObject;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate(
            photonObject.name,
            new Vector3(0f, 1f, 0f),
            Quaternion.identity, 0
        );

        GameObject mainCamera =
            GameObject.FindWithTag("MainCamera");
        mainCamera.GetComponent<ThirdPersonCamera>().enabled = true;
    }

}