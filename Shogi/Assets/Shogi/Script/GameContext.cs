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

	public State NowState = State.Prep;

	public string myPlayerName = "";
	public static readonly string playerNamePrefKey = "PlayerName";
	public List<GamePlayer> players = new List<GamePlayer>();
	public GamePlayer myPlayer;
}