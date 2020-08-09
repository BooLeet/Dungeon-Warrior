using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameModeDelve : GameMode
{
    public static GameModeDelve instance;

    public Transform room1, room2;
    public GameObject playerPrefab;
    private Transform currentRoom;
    private Transform nextRoom;
    private Transform nextSpawnPoint;

    public NavMeshSurface navMeshSurface;

    public DelveRoomSet roomSet;

    [Header("Room Director")]
    [Range(0,1)]
    public float roomDirectorLowHealthDecrement = 0.3f;
    public float roomDirectorBonusDamageIncrementMultiplier = 0.4f;
    [Space]
    public float playerSuccessMedium = 10;
    public float playerSuccessHard = 20;
    [Space]
    public float playerSuccessMin = -4;
    public float playerSuccessMax = 30;
    public float playerSuccessMeter = 1;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if(Input.GetKeyDown("k"))
            navMeshSurface.RemoveData();
    }

    private void Start()
    {
        currentRoom = room1;
        nextRoom = room2;
        InstantiateRoom(room1, roomSet.startRoom);
        Instantiate(playerPrefab, nextSpawnPoint.position, nextSpawnPoint.rotation);
        //LoadNewRoom();
    }

    public void NextRoom()
    {
        PlayerSuccessMeterHandling();
        DestructableObjectPieceRegistry.GetInstance().Clear();
        LoadNewRoom();
        MovePlayerToNextSpawnPoint();
    }

    private void LoadNewRoom()
    {
        ClearRoom(currentRoom);
        InstantiateRoom(nextRoom, SelectNextRoom());
        Transform temp = currentRoom;
        currentRoom = nextRoom;
        nextRoom = temp;
    }

    private DelveRoom SelectNextRoom()
    {
        List<DelveRoom> delveRooms = new List<DelveRoom>();
        if(playerSuccessMeter >= playerSuccessHard)
        {
            FillRooms(ref delveRooms, roomSet.hard, 3);
            FillRooms(ref delveRooms, roomSet.medium, 2);
            FillRooms(ref delveRooms, roomSet.bigLoot, 1);
        }
        else if(playerSuccessMeter >= playerSuccessMedium)
        {
            FillRooms(ref delveRooms, roomSet.medium, 4);
            FillRooms(ref delveRooms, roomSet.easy, 2);
            FillRooms(ref delveRooms, roomSet.smallLoot, 1);
            FillRooms(ref delveRooms, roomSet.bigLoot, 1);
        }
        else
        {
            FillRooms(ref delveRooms, roomSet.easy, 3);
            FillRooms(ref delveRooms, roomSet.smallLoot, 1);
        }

        return delveRooms[Random.Range(0, delveRooms.Count)];
    }

    private void FillRooms(ref List<DelveRoom> reciever, DelveRoom[] source,int count)
    {
        for (int i = 0; i < count; ++i)
            reciever.Add(source[Random.Range(0, source.Length)]);
    }

    private void PlayerSuccessMeterHandling()
    {
        PlayerCharacter player = PlayerCharacter.instance;
        float increment = 1;

        if (player.CurrentHealth / player.maxHealth < 0.4)
            increment -= roomDirectorLowHealthDecrement;

        increment += player.GetDamageBonusMultiplier() * roomDirectorBonusDamageIncrementMultiplier;

        playerSuccessMeter += increment;
        playerSuccessMeter = Mathf.Clamp(playerSuccessMeter, playerSuccessMin, playerSuccessMax);
    }

    private void ClearRoom(Transform transform)
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; ++i)
            Destroy(transform.GetChild(0).gameObject);
    }

    private void InstantiateRoom(Transform transform,DelveRoom room)
    {
        ClearRoom(transform);

        nextSpawnPoint = Instantiate(room.room, transform).GetComponent<DelveRoomController>().playerSpawnPoint;
        navMeshSurface.RemoveData();
        navMeshSurface.BuildNavMesh();
    }

    private void MovePlayerToNextSpawnPoint()
    {
        PlayerCharacter player = PlayerCharacter.instance;
        player.Warp(nextSpawnPoint.position);
        player.transform.rotation = nextSpawnPoint.rotation;
    }
}
