using UnityEngine;
using UnityEditor;
using Photon.Pun;


public class Player : Photon.Pun.MonoBehaviourPun , IPunObservable
{
    public Janken.Hand selectHand = Janken.Hand.None;
    private Janken.Hand sendedHand = Janken.Hand.None;	//送信済み
	private bool isSyncing = true; // 同期フラグ


	public void SetSelectHand(Janken.Hand hand)
    {
        selectHand = hand;
	}



	// データを送受信するメソッド
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			if (selectHand != sendedHand)
			{
				// 自身側が生成したオブジェクトの場合送信
				stream.SendNext(selectHand);
				sendedHand = selectHand;
			}
		}
		else
		{
			// 他プレイヤー側が生成したオブジェクトの場合は受け取ったデータを適応
			SetSelectHand((Janken.Hand)stream.ReceiveNext());
		}
	}
}