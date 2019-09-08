using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityChan;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject photonObject;

    /// <summary>
    /// 入室完了コールバック
    /// </summary>
    public System.Action onJoinRoomAction;
	private GameContext context;


    void Start()
    {/*
        this.Connect("1.0", ()=> {
            PhotonNetwork.Instantiate(
                photonObject.name,
                new Vector3(0f, 1f, 0f),
                Quaternion.identity, 0
            );

            GameObject mainCamera =
                GameObject.FindWithTag("MainCamera");
            mainCamera.GetComponent<ThirdPersonCamera>().enabled = true;

        });*/
    }

    /// <summary>
    /// Photonに接続する
    /// </summary>
    public void Connect(string gameVersion, System.Action joinRoom, GameContext context)
    {
		this.context = context;
        if (PhotonNetwork.IsConnected == false)
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
            this.onJoinRoomAction = joinRoom;
        }
        else if(joinRoom != null)
        {
            joinRoom();
			PhotonNetwork.LocalPlayer.NickName = context.myPlayerName;
        }
    }

    /// <summary>
    /// ルーム作成
    /// </summary>
    private void CreateRoom()
    {
        
        // ルームオプションの基本設定
        RoomOptions roomOptions = new RoomOptions
        {
            // 部屋の最大人数
            MaxPlayers = (byte)Janken.MaxRoomNum,
 
            // 公開
            IsVisible = true,
 
            // 入室可
            IsOpen = true
        };


        // 部屋を作成して入室する
        PhotonNetwork.CreateRoom("test", roomOptions);
    }


	/**
     * RPCで遠隔の関数を呼ぶ.
     */
	/*
		private void SendRPC(string str)
		{
			if (m_photonView != null)
			{
				m_photonView.RPC("SendMessageTest", RpcTarget.All, str);
			}
		}
		public void GetCustmProperties<T>(string key, out T value)
		{
			if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Score", out object scoreObject))
			{
				value = (T)scoreObject;
			}
		}
	*/






	/// <summary>
	/// photonへの接続完了
	/// </summary>
	public override void OnConnectedToMaster()
	{
		PhotonNetwork.LocalPlayer.NickName = context.myPlayerName;
		PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// 入室失敗
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        this.CreateRoom();
    }


    /// <summary>
    /// 入室成功
    /// </summary>
    public override void OnJoinedRoom()
    {
		this.UpdateMemberList();

		if (this.onJoinRoomAction != null)
        {
            this.onJoinRoomAction();
        }
    }


	[SerializeField]
	Text joinedMembersText;

	// <summary>
	// リモートプレイヤーが入室した際にコールされる
	// </summary>
	public override void OnPlayerEnteredRoom(Player player)
	{
		Debug.Log(player.NickName + " is joined.");
		UpdateMemberList();
	}

	// <summary>
	// リモートプレイヤーが退室した際にコールされる
	// </summary>
	public override void OnPlayerLeftRoom(Player player)
	{
		Debug.Log(player.NickName + " is left.");
		UpdateMemberList();
	}

	public void UpdateMemberList()
	{
		joinedMembersText.text = "";
		foreach (var p in PhotonNetwork.PlayerList)
		{
			joinedMembersText.text += p.NickName + "\n";
		}
	}
}