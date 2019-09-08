using UnityEngine;
using UnityEditor;

public static class Janken
{
    public enum Hand : int
    {
        None = -1,
        Rock,
        Scissors,
		Paper,

		Max
    };

    /// <summary>
    /// ルーム内の最大人数
    /// </summary>
    public static int MaxRoomNum = 2;

}