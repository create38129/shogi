using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;

	[SerializeField] GameObject photonObject;

	[SerializeField] Button buttonPlay;
	[SerializeField] Button buttonRock;
	[SerializeField] Button buttonPaper;
	[SerializeField] Button buttonScissors;
	[SerializeField] InputField nameField;

    [SerializeField] GameObject prepObject;
    [SerializeField] GameObject waitObject;
    [SerializeField] GameObject selectObject;
    [SerializeField] GameObject resultObject;

	private GameContext context = new GameContext();

    // Use this for initialization
    void Start()
    {
		//前回の名前を利用
		if (PlayerPrefs.HasKey(GameContext.playerNamePrefKey))
		{
			this.context.myPlayerName = PlayerPrefs.GetString(GameContext.playerNamePrefKey);
			nameField.text = this.context.myPlayerName;
		}
		this.buttonPlay.OnClickAsObservable()
            .Subscribe(_ => { this.ChangeState(GameContext.State.WaitOtherPlayer); })
            .AddTo(this);
		this.buttonRock.OnClickAsObservable()
			.Subscribe(_ => { this.SelectMyHand(Janken.Hand.Rock); })
			.AddTo(this);
		this.buttonPaper.OnClickAsObservable()
			.Subscribe(_ => { this.SelectMyHand(Janken.Hand.Paper); })
			.AddTo(this);
		this.buttonScissors.OnClickAsObservable()
			.Subscribe(_ => { this.SelectMyHand(Janken.Hand.Scissors); })
			.AddTo(this);
	}

    // Update is called once per frame
    void Update()
    {
        switch (this.context.NowState)
        {
			case GameContext.State.WaitOtherPlayer:
				Wait();
				break;
            case GameContext.State.SelectHand:
                SelectHand();
                break;
            case GameContext.State.Result:
                Result();
                break;
        }
    }

    private void ChangeState(GameContext.State state)
    {
        switch (state)
        {
            case GameContext.State.Prep:
                this.prepObject.SetActive(true);
                this.waitObject.SetActive(false);
                this.selectObject.SetActive(false);
                this.resultObject.SetActive(false);
                break;
            case GameContext.State.WaitOtherPlayer:
				this.prepObject.SetActive(false);
				this.waitObject.SetActive(true);
				this.selectObject.SetActive(false);
				this.resultObject.SetActive(false);

				StartConnect();
				break;
            case GameContext.State.SelectHand:
				this.prepObject.SetActive(false);
				this.waitObject.SetActive(false);
				this.selectObject.SetActive(true);
				this.resultObject.SetActive(false);
				break;
            case GameContext.State.Result:
				this.prepObject.SetActive(false);
				this.waitObject.SetActive(false);
				this.selectObject.SetActive(false);
				this.resultObject.SetActive(true);
				break;

        }
        this.context.NowState = state;
    }


	private void Wait()
	{
		if(PhotonNetwork.PlayerListOthers.Length > 0)
		{
			this.ChangeState(GameContext.State.SelectHand);
		}
	}

	private List<Janken.Hand> handList = new List<Janken.Hand>();
	private void SelectHand()
	{
		if (this.context.myPlayer.selectHand == Janken.Hand.None) return;
		handList.Clear();
		foreach (var p in PhotonNetwork.PlayerList) {
			if (!p.CustomProperties.ContainsKey("hand") || !p.CustomProperties.ContainsKey("aikoCount"))return;
			if ((int)p.CustomProperties["aikoCount"] != this.context.aikoCount)return;

			Janken.Hand hand = (Janken.Hand)p.CustomProperties["hand"];
			if (hand == Janken.Hand.None) return;

			handList.Add(hand);
		}

		this.ChangeState(GameContext.State.Result);
	}

    private void Result()
    {

    }

	private void CheckPlayersHand()
	{
		bool[] isSelect = new bool[(int)Janken.Hand.Max];
		bool isAll = true;
		bool isOnly = true;
		for(int i = 0; i < (int)Janken.Hand.Max; i++)
		{
			isSelect[i] = handList.Any(x => x == (Janken.Hand)i);
			if(i != (int)this.context.myPlayer.selectHand)
			{
				isAll &= isSelect[i];
				isOnly &= !isSelect[i];
			}
		}
		if(isAll || isOnly)
		{
			//全種 or 同じ時
			this.Aiko();
		}
		else
		{
			//ぐーちょきぱーの順で定義してるので選択した手の次があれば勝ち
			this.context.isWin = isSelect[((int)this.context.myPlayer.selectHand + 1) % 3];
		}
	}

	/// <summary>
	/// あいこのときの処理
	/// </summary>
	private void Aiko()
	{
		this.context.aikoCount++;
		this.context.myPlayer.SetSelectHand(Janken.Hand.None);
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable() {{ "aikoCount", this.context.aikoCount }};
		PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
		ChangeState(GameContext.State.SelectHand);
	}


	public void SelectMyHand(Janken.Hand hand)
	{
		this.context.myPlayer.SetSelectHand(hand);
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable() { { "hand", hand }, { "aikoCount", this.context.aikoCount } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
	}


    private void StartConnect()
    {
        this.networkManager.Connect("1.0", OnJoinRoom, context);
    }

    private void OnJoinRoom()
	{
		this.context.myPlayer = PhotonNetwork.Instantiate(photonObject.name, new Vector3(0f, 1f, 0f), Quaternion.identity, 0).GetComponent<GamePlayer>();
	}

    /// <summary>
    /// プレイヤー名設定
    /// </summary>
    /// <param name="value"></param>
    public void OnSetPlayerName()
    {
        this.context.myPlayerName = this.nameField.text;
        PlayerPrefs.SetString(GameContext.playerNamePrefKey, this.nameField.text);    //今回の名前をセーブ
        Debug.Log("setname:" + this.nameField.text);
    }
}
