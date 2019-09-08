using UnityEngine;
using UnityEditor;
using Photon.Pun;


public class Player : Photon.Pun.MonoBehaviourPun , IPunObservable
{
    public Janken.Hand selectHand { get; private set; } = Janken.Hand.None;
	private bool isSyncing = true; // 同期フラグ


	public void SetSelectHand(Janken.Hand hand)
    {
        selectHand = hand;
		isSyncing = true;
	}



	// データを送受信するメソッド
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			if (isSyncing)
			{
				// 自身側が生成したオブジェクトの場合送信
				stream.SendNext(selectHand);
				isSyncing = false;
			}
		}
		else
		{
			// 他プレイヤー側が生成したオブジェクトの場合は受け取ったデータを適応
			SetSelectHand((Janken.Hand)stream.ReceiveNext());
		}
	}
}