using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameMode/Delve/RoomSet")]
public class DelveRoomSet : ScriptableObject
{
    public DelveRoom startRoom;
    public DelveRoom[] easy;
    public DelveRoom[] medium;
    public DelveRoom[] hard;
    [Space]
    public DelveRoom[] smallLoot;
    public DelveRoom[] bigLoot;
}
