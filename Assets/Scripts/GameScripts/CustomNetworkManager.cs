using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField]
    private GameObject PreGame;
    [SerializeField]
    private GameObject RightPanel;
    [SerializeField]
    private GameObject InGame;

    public List<GameObject> Players = new List<GameObject>(); //Лист игроков
    private List<GameObject> Boxes = new List<GameObject>(); //Лист коробок для удаления

    [SerializeField]
    public int MaxPlayerCount; //Количество слотов на игру
    public int CurrentPlayerCount = 0; //Количество подключенных игроков

    [SerializeField]
    private InitializeGame Initialize;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = Instantiate(playerPrefab, startPositions[CurrentPlayerCount].position, startPositions[CurrentPlayerCount].rotation); //Создание игрока
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId); //Заспавнить (чтобы видели все)
        Players.Add(player); //Добавить в лист
        CurrentPlayerCount++; //+1 подключенный игрок
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        CurrentPlayerCount = 0;
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        CurrentPlayerCount--; //-1 подключенный игрок
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        if (GameObject.Find("Canvas").GetComponent<AudioSource>().isPlaying)
            GameObject.Find("Canvas").GetComponent<AudioSource>().Stop();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        for (int i = 0; i < 4; i++) //Настройка бонусов по умолчанию
        {
            Text text = GameObject.Find($"PanelBonusItem ({i})").transform.GetChild(1).gameObject.GetComponent<Text>();
            if (i == 3)
                text.text = "X";
            else
                text.text = "x01";
        }
        Initialize.timer.text = "---";
        CurrentPlayerCount = 0;
        Players.Clear();
        DeleteBoxes();
        Initialize.isGameReady = false; //Игра готова
        Initialize.isGameStart = false; //Игра началась
        Initialize.isGameEnd = false; //Игра закончилась
        Initialize.MainCamera.SetActive(true);
        Initialize.WinCamera.SetActive(false);
        Initialize.WinFirework.SetActive(false);
        Initialize.DrawFirework.SetActive(false);
        PreGame.SetActive(true);
        RightPanel.SetActive(false);
        InGame.SetActive(false);
        GameObject.Find("Canvas").GetComponent<AudioSource>().Play();
    }
    
    public void GenerateBoxes()
    {
        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 17; j++)
            {
                if (j > -1 && j < 17)
                {
                    if (i % 2 == 0 || j % 2 == 0)
                    {
                        if (AcceptPlaceBox())
                        {
                            Vector3 vec;
                            vec.x = (float)(i + 2.5);
                            vec.z = (float)(j + 0.5);
                            vec.y = (float)0.5;
                            SpawnBox(vec);
                        }
                    }
                }
            }
        }
        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 17; j++)
            {
                if (j > -1 && j < 2 || j > 14 && j < 17)
                {
                    if (i % 2 == 0 || j % 2 == 0)
                    {
                        if (AcceptPlaceBox())
                        {
                            Vector3 vec;
                            vec.z = (float)(i + 2.5);
                            vec.x = (float)(j + 0.5);
                            vec.y = (float)0.5;
                            SpawnBox(vec);
                        }
                    }
                }
            }
        }
    }

    private bool AcceptPlaceBox()
    {
        int result = Random.Range(1, 101);
        if (result >= 1 && result <= 5)
            return false;
        else
            return true;
    }

    private void SpawnBox(Vector3 pos)
    {
        GameObject BoxObject = Instantiate(spawnPrefabs[2], pos, Quaternion.identity);
        Boxes.Add(BoxObject);
        NetworkServer.Spawn(BoxObject);
    }

    public void DeleteBoxes()
    {
        if (Boxes.Count != 0)
        {
            foreach (GameObject Box in Boxes)
                Destroy(Box);
            Boxes.Clear();
        }
    }
}