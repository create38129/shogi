using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;

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
        if (this.nameField != null)
        {
            if (PlayerPrefs.HasKey(GameContext.playerNamePrefKey))
            {
//                this.myPlayerName = PlayerPrefs.GetString(playerNamePrefKey);
//                nameField.text = this.myPlayerName;
            }
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
				break;
            case GameContext.State.SelectHand:
				this.prepObject.SetActive(false);
				this.waitObject.SetActive(false);
				this.selectObject.SetActive(true);
				this.resultObject.SetActive(false);
				break;
            case GameContext.State.Result:
                break;

        }
        this.context.NowState = state;
    }


	private void Wait()
	{
	}

	private void SelectHand()
    {
        if(this.context.players.Any(x => x.selectHand != Janken.Hand.None))
        {
            this.ChangeState(GameContext.State.Result);
        }
    }

    private void Result()
    {

    }


	public void SelectMyHand(Janken.Hand hand)
	{
		this.context.myPlayer.SetSelectHand(hand);
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
