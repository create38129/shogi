using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GameContext
{

	public enum State : int
	{
		Prep = 0,
		WaitOtherPlayer,
		SelectHand,
		Result,

		Max
	};

	public enum ResultState
	{
		Win,
		Lose,
		Aiko
	};

	public State NowState = State.Prep;

	public string myPlayerName = "";
	public static readonly string playerNamePrefKey = "PlayerName";
	public List<GamePlayer> players = new List<GamePlayer>();
	public GamePlayer myPlayer;

	public int aikoCount = 0;
	public ResultState resultState = ResultState.Win;
}