using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

public class InitializeGame : NetworkBehaviour
{
    [SerializeField]
    private string GameTime;

    public bool isGameReady = false; //Игра готова
    public bool isGameStart = false; //Игра началась
    public bool isGameEnd = false; //Игра закончилась

    [SerializeField]
    private CustomNetworkManager Manager; //Нетворк
    public Text timer; //Текстовой таймер
    [SerializeField]
    private GameObject TriumphStand; //Стенд для победиля
    public GameObject RightPanel; //Правая панель
    public GameObject MainCamera; //Главная камера
    public GameObject WinCamera; //Победная камера
    public GameObject WinFirework; //Победный фейерверк
    public GameObject DrawFirework; //Фейерверк ничьи
    public List<GameObject> Players = new List<GameObject>(); //Лист игроков

    private void FixedUpdate()
    {
        if (isServer)
        {
            if (Manager.CurrentPlayerCount == Manager.MaxPlayerCount && !isGameReady) //Когда все игроки в сборе
            {
                Players = Manager.Players;
                CmdStartgame();
            }

            if (isGameStart && !isGameEnd) //Если игра в процессе
            {
                if (Players.Count < 2)
                    CmdShowWinner(Players.Count);
            }
        }
    }

    [Command]
    private void CmdStopGame()
    {
        RpcStopGame();
    }

    [ClientRpc]
    private void RpcStopGame()
    {
        Manager.StopHost();
    }

    [Command]
    private void CmdShowWinner(int Alive)
    {
        isGameEnd = true;
        RpcShowWinner(Alive);
    }

    [ClientRpc]
    private void RpcShowWinner(int Alive)
    {
        if (isServer)
            CancelInvoke("CmdTimer");
        if (!isServer)
            isGameEnd = true;
        if (GetComponents<AudioSource>()[0].isPlaying)
            GetComponents<AudioSource>()[0].Stop();
        if (Players.Count != 0)
            GetComponents<AudioSource>()[1].Play();
        WinCamera.SetActive(true);
        MainCamera.SetActive(false);
        RightPanel.SetActive(true);
        if (Alive == 1)
        {
            WinFirework.SetActive(true);
            GameObject Winner = Players[0];
            Quaternion rot = new Quaternion();
            rot.x = 0f;
            rot.y = 180f;
            rot.z = 0f;
            Winner.transform.rotation = rot;

            Vector3 pos = TriumphStand.transform.position;
            pos.y = 2.5f;
            Winner.GetComponent<CharacterController>().enabled = false;
            Winner.transform.position = pos;
            Winner.GetComponent<CharacterController>().enabled = true;
        }
        else
        {
            DrawFirework.SetActive(true);
        }
    }

    [Command]
    public void CmdTimer()
    {
        RpcTimer();
    }
    
    [ClientRpc]
    public void RpcTimer()
    {
        if (timer.text != "0")
            timer.text = (Convert.ToInt16(timer.text) - 1).ToString();

        if (timer.text == "0")
            CmdTimerIsUp();
    }

    [Command]
    public void CmdTimerIsUp()
    {
        RpcTimerIsUp();
    }

    [ClientRpc]
    public void RpcTimerIsUp()
    {
        foreach (GameObject player in Players)
            player.SetActive(false);
        Players.Clear();
    }


    [Command]
    public void CmdStartgame()
    {
        isGameReady = true;
        Manager.GenerateBoxes();
        RpcStartgame(Players.ToArray());
    }

    [ClientRpc]
    private void RpcStartgame(GameObject[] RpcPlayers)
    {
        if (!isServer)
            isGameReady = true;

        Players.Clear(); //Очистка листа
        for (short j = 0; j < RpcPlayers.Length; j++)
        {
            Players.Add(RpcPlayers[j]); //Запись в лист
            for (int i = 1; i < RpcPlayers[j].transform.childCount - 2; i++) //Раскраска
                RpcPlayers[j].transform.GetChild(i).gameObject.GetComponent<SkinnedMeshRenderer>().material = RpcPlayers[j].GetComponent<Bomberman>().skins[j];
            RpcPlayers[j].GetComponent<Bomberman>().ColorType = j;
        }

        GetComponents<AudioSource>()[0].Play();
        isGameStart = true; //"Начать игру"
        timer.text = GameTime;
        if (isServer)
            InvokeRepeating("CmdTimer", 0f, 1f); //Запустить таймер
    }

    [Command]
    public void CmdRemovePlayer(GameObject go)
    {
        RpcRemovePlayer(go);
    }

    [ClientRpc]
    private void RpcRemovePlayer(GameObject go)
    {
        Players.Remove(go);
    }
}