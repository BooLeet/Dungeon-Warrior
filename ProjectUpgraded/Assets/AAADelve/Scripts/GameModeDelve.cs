using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
    private DelveRoom previousDelveRoom;

    [Header("Room Director")]
    [Range(0, 10)]
    public int difficultyEasyCount = 3;
    [Range(0, 10)]
    public int difficultyMediumCount = 2;
    [Range(0, 10)]
    public int difficultyHardCount = 1;

    private int roomCountCyclical = 0;
    public int RoomNumber { get; private set; }
    [Header("Score System")]
    public int scoreRegularMonster = 50;
    public int scoreBigMonster = 100;
    [Space]
    public int scorePotion = 20;
    public int scoreAmmo = 20;
    [Space]
    public int spikeKill = 30;

    public int Score { get; private set; }
    public int ScoreMultiplier { get; private set; }
    private int scoreMaxMultipler = 32;

    [Space]
    public int scoreMultiplierStep = 500;
    private float scoreMultiplierMeter;
    public int scoreMultiplierMeterTickDown = 10;
    public float scoreMultiplierMeterTickDownDelay = 5;
    private float scoreMultiplierMeterTickDownCounter = 0;
    public float ScoreMultiplierMeter { get { return scoreMultiplierMeter / (float)scoreMultiplierStep; } }

    private int currentEnemyLevel = 0;

    [Header("PlayerUpgrades")]
    public MeleeWeapon[] meleeUpgrades;
    private int meleeUpgradeCounter = 0;

    protected override void AwakeFunction()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        ScoreMultiplierTickDown();
    }

    private void Start()
    {
        ScoreMultiplier = 1;

        currentRoom = room1;
        nextRoom = room2;
        InstantiateRoom(room1, roomSet.startRoom);
        Instantiate(playerPrefab, nextSpawnPoint.position, nextSpawnPoint.rotation);
        //LoadNewRoom();
    }

    #region Rooms

    public void NextRoom()
    {
        DestructableObjectPieceRegistry.GetInstance().Clear();
        LoadNewRoom();
        MovePlayerToNextSpawnPoint();
        RoomNumber++;
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
        DelveRoom result;
        int mediumThreshold = difficultyEasyCount;
        int hardThreshold = mediumThreshold + difficultyMediumCount;
        int merchantThreshold = hardThreshold + difficultyHardCount;
        //int nextRoomNumber = (RoomNumber + 1) % (merchantThreshold + 2);


        ++roomCountCyclical;
        if (roomCountCyclical > merchantThreshold + 1)
            roomCountCyclical = 1;
        
        if (roomCountCyclical > merchantThreshold)
        {
            result = roomSet.merchantRoom;
            IncreaceEnemyLevel();
        }
        else if(roomCountCyclical > hardThreshold)
        {
            result = GetRandomRoom(roomSet.hard, previousDelveRoom);
        }
        else if (roomCountCyclical > mediumThreshold)
        {
            result = GetRandomRoom(roomSet.medium, previousDelveRoom);
        }
        else
        {
            result = GetRandomRoom(roomSet.easy, previousDelveRoom);
        }

        previousDelveRoom = result;
        return result;
    }

    private DelveRoom GetRandomRoom(DelveRoom[] rooms,DelveRoom roomToExclude)
    {
        List<DelveRoom> delveRooms = rooms.ToList();
        if(delveRooms.Count > 1)
            delveRooms.Remove(roomToExclude);
        return delveRooms[Random.Range(0, delveRooms.Count)];
    }

    private void FillRooms(ref List<DelveRoom> reciever, DelveRoom[] source,int count)
    {
        for (int i = 0; i < count; ++i)
            reciever.Add(source[Random.Range(0, source.Length)]);
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
    #endregion

    #region Score
    private void AddRawScore(int rawScore, Vector3 position, HUD_ScorePopup.Scale scale)
    {
        GameManager.instance.hud.ScorePopup(rawScore, position, scale);
        Score += rawScore;
    }

    private void AddMultipliedScore(int rawScore, Vector3 position, HUD_ScorePopup.Scale scale)
    {
        int score = rawScore * ScoreMultiplier;
        AddRawScore(score, position, scale);
        scoreMultiplierMeterTickDownCounter = scoreMultiplierMeterTickDownDelay;
        ScoreMultiplierIncreace(rawScore);
    }

    private void ScoreMultiplierIncreace(int rawScore)
    {
        scoreMultiplierMeter += rawScore;
        if (scoreMultiplierMeter >= scoreMultiplierStep)
        {
            int scoreStepReachCount = (int)scoreMultiplierMeter / scoreMultiplierStep;
            int nextScoreMultiplier = ScoreMultiplier * (int)Mathf.Pow(2, scoreStepReachCount);
            if (nextScoreMultiplier > scoreMaxMultipler)
                scoreMultiplierMeter = scoreMultiplierStep;
            else
            {
                ScoreMultiplier = nextScoreMultiplier;
                scoreMultiplierMeter %= scoreMultiplierStep;
            }
        }
    }

    private void ScoreMultiplierTickDown()
    {
        int merchantRoomNumber = difficultyEasyCount + difficultyMediumCount + difficultyHardCount + 1;

        if (roomCountCyclical == merchantRoomNumber)
            return;

        if (scoreMultiplierMeterTickDownCounter <= 0)
        {
            scoreMultiplierMeterTickDownCounter = 0;
            scoreMultiplierMeter -= Time.deltaTime * scoreMultiplierMeterTickDown;
            if(scoreMultiplierMeter < 0)
            {
                if (ScoreMultiplier > 1)
                {
                    ScoreMultiplier /= 2;
                    scoreMultiplierMeter = scoreMultiplierStep + scoreMultiplierMeter;
                }
                else
                    scoreMultiplierMeter = 0;
            }
        }
        
        scoreMultiplierMeterTickDownCounter -= Time.deltaTime;
    }

    #endregion

    #region Game Over
    private int GetRecordRoomNumber()
    {
        return 1337;
    }


    #endregion

    #region Player Upgrades

    public MeleeWeapon GetMeleeUpgrade()
    {
        if (meleeUpgradeCounter >= meleeUpgrades.Length)
            return null;
        return meleeUpgrades[meleeUpgradeCounter];
    }

    public int GetMeleeUpgradeCost()
    {
        return 1337;
    }

    public void OnMeleeUpgrade(Vector3 popupPosition)
    {
        AddRawScore(-GetMeleeUpgradeCost(), popupPosition, HUD_ScorePopup.Scale.Mid);
        ++meleeUpgradeCounter;
    }

    #endregion

    private void IncreaceEnemyLevel()
    {
        ++currentEnemyLevel;
        Debug.Log("Current enemy level: " + currentEnemyLevel);
    }

    #region Overrides

    public override void OnAIKilled(AICharacter character)
    {
        int scoreToAdd = scoreRegularMonster;
        AIDirector.TokenType tokenType = character.aiStats.attackTokenType;
        if (tokenType == AIDirector.TokenType.HeavyMelee || tokenType == AIDirector.TokenType.HeavyRange)
            scoreToAdd = scoreBigMonster;
        AddMultipliedScore(scoreToAdd, character.Position, HUD_ScorePopup.Scale.Big);
    }

    public override void OnInteraction(Interactable interactable)
    {
        int scoreToAdd = -1;
        if (interactable as HealthPotion)
            scoreToAdd = scorePotion;
        else if (interactable as ResourceLoot)
            scoreToAdd = scoreAmmo;

        if (scoreToAdd < 0)
            return;
        AddMultipliedScore(scoreToAdd, interactable.ButtonPosition, HUD_ScorePopup.Scale.Small);
    }

    public override void OnPlayerDamaged(float rawDamage)
    {
        //if (ScoreMultiplier > 1)
        //    ScoreMultiplier /= 2;
        scoreMultiplierMeter = 0;
    }

    public override void OnPlayerDeath()
    {

    }

    public override string GetGameOverText()
    {
        LanguagePack languagePack = GameManager.instance.languagePack;
        return languagePack.GetString("delveRoomsReached") + ": " + RoomNumber.ToString() + "\n" +
               languagePack.GetString("record") + ": " + GetRecordRoomNumber().ToString();
    }

    public override void OnSpikesKill(DeadlySpikes spikes)
    {
        AddMultipliedScore(spikeKill, spikes.Position + Vector3.up, HUD_ScorePopup.Scale.Mid);
    }

    public override int GetEnemyLevel()
    {
        return currentEnemyLevel;
    }

    #endregion
}
