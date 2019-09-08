using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;

    [SerializeField] Button buttonPlay;
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

    public State NowState = State.Prep;

    private string myPlayerName = "";
    static readonly string playerNamePrefKey = "PlayerName";
    private List<Player> players = new List<Player>();

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
            .Subscribe(_ => { })
            .AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        switch (this.NowState)
        {
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
                break;
            case State.SelectHand:
                break;
            case State.Result:
                break;

        }
        this.NowState = state;
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


    private void StartConnect()
    {
        this.networkManager.Connect("1.0", OnJoinRoom);
    }

    private void OnJoinRoom()
    {

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
