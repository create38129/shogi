using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;

    [SerializeField] Button buttonPlay;
    [SerializeField] InputField nameField;

    public enum State {
        Prep,
        WaitOtherPlayer,
        Play,
        Result
    };

    public State NowState = State.Prep;

    private string myPlayerName = "";
    static string playerNamePrefKey = "PlayerName";

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
    }

    // Update is called once per frame
    void Update()
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
