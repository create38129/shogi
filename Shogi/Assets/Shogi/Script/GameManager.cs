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

    public enum State : int{
        Prep = 0,
        WaitOtherPlayer,
        SelectHand,
        Result,

        Max
    };

    private State NowState = State.Prep;

    private string myPlayerName = "";
    static readonly string playerNamePrefKey = "PlayerName";
    private List<Player> players = new List<Player>();
	private Player myPlayer;

    // Use this for initialization
    void Start()
    {
        //前回の名前を利用
        if (this.nameField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
//                this.myPlayerName = PlayerPrefs.GetString(playerNamePrefKey);
//                nameField.text = this.myPlayerName;
            }
        }
        this.buttonPlay.OnClickAsObservable()
            .Subscribe(_ => { this.ChangeState(State.WaitOtherPlayer); })
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
        switch (this.NowState)
        {
			case State.WaitOtherPlayer:
				Wait();
				break;
            case State.SelectHand:
                SelectHand();
                break;
            case State.Result:
                Result();
                break;
        }
    }

    private void ChangeState(State state)
    {
        switch (state)
        {
            case State.Prep:
                this.prepObject.SetActive(true);
                this.waitObject.SetActive(false);
                this.selectObject.SetActive(false);
                this.resultObject.SetActive(false);
                break;
            case State.WaitOtherPlayer:
				this.prepObject.SetActive(false);
				this.waitObject.SetActive(true);
				this.selectObject.SetActive(false);
				this.resultObject.SetActive(false);
				break;
            case State.SelectHand:
				this.prepObject.SetActive(false);
				this.waitObject.SetActive(false);
				this.selectObject.SetActive(true);
				this.resultObject.SetActive(false);
				break;
            case State.Result:
                break;

        }
        this.NowState = state;
    }


	private void Wait()
	{
	}

	private void SelectHand()
    {
        if(this.players.Any(x => x.selectHand != Janken.Hand.None))
        {
            this.ChangeState(State.Result);
        }
    }

    private void Result()
    {

    }


	public void SelectMyHand(Janken.Hand hand)
	{
		this.myPlayer.SetSelectHand(hand);
	}


    private void StartConnect()
    {
        this.networkManager.Connect("1.0", OnJoinRoom);
    }

    private void OnJoinRoom()
	{
		this.myPlayer = PhotonNetwork.Instantiate(photonObject.name, new Vector3(0f, 1f, 0f), Quaternion.identity, 0).GetComponent<Player>();
	}

    /// <summary>
    /// プレイヤー名設定
    /// </summary>
    /// <param name="value"></param>
    public void OnSetPlayerName()
    {
        this.myPlayerName = this.nameField.text;
        PlayerPrefs.SetString(playerNamePrefKey, this.nameField.text);    //今回の名前をセーブ
        Debug.Log("setname:" + this.nameField.text);
    }
}
