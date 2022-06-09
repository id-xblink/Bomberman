using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using System;

public class CustomNetworkHUD : MonoBehaviour
{
    [SerializeField]
    private NetworkManager manager;

    [SerializeField]
    private Canvas MainMenu;
    [SerializeField]
    private InputField Address;
    [SerializeField]
    private InputField Port;

    public void JoinNetworkGame()
    {
        manager.StartMatchMaker();
        manager.matchMaker.ListMatches(0, 20, "", false, 0, 0, manager.OnMatchList);
        InvokeRepeating("CheckCreatedMatch", 0f, 1f);
    }

    private void CheckCreatedMatch()
    {
        if (manager.matches != null)
        {
            if (MainMenu.GetComponent<AudioSource>().isPlaying)
                MainMenu.GetComponent<AudioSource>().Stop();
            CancelInvoke("CheckCreatedMatch");
            if (manager.matches.Count == 0)
            {
                manager.matches = null;
                manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "", "", "", 0, 0, manager.OnMatchCreate); //Создание матча
            }
            else
            {
                manager.matchMaker.JoinMatch(manager.matches[0].networkId, "", "", "", 0, 0, manager.OnMatchJoined); //Подключение к матчу
            }
        }
    }

    public void DisconnectFromGame()
    {
        manager.StopHost();
    }

    public void CreateLocalGame()
    {
        ManagerSettings();
        manager.StartHost();
    }

    public void ConnectLocalGame()
    {
        ManagerSettings();
        manager.StartClient();
    }

    public void ManagerSettings()
    {
        if (Address.text != "")
            manager.networkAddress = Address.text;
        if (Port.text != "")
            manager.networkPort = Convert.ToInt32(Port.text);
    }
}