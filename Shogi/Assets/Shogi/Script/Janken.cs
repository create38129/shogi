using UnityEngine;
using UnityEditor;

public static class Janken
{
    public enum Hand
    {
        None,
        Rock,
        Paper,
        Scissors,

        Max
    };

    /// <summary>
    /// ルーム内の最大人数
    /// </summary>
    public static int MaxRoomNum = 2;

}