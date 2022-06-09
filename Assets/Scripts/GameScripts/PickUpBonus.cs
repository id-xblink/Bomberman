using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class PickUpBonus : NetworkBehaviour
{
    public short BonusType = 10;
    private bool canRemove = false;

    [SerializeField]
    private Material[] Materials = new Material[5];
    
    private void Awake()
    {
        GetComponents<AudioSource>()[0].Play();
    }

    private void Start()
    {
        Material[] ChangeMaterials = new Material[2];
        ChangeMaterials[0] = Materials[4];
        ChangeMaterials[1] = Materials[BonusType];
        gameObject.GetComponent<MeshRenderer>().materials = ChangeMaterials;
    }

    private void FixedUpdate()
    {
        if (!GetComponents<AudioSource>()[0].isPlaying && !GetComponents<AudioSource>()[1].isPlaying && canRemove)
            Destroy(gameObject);
    }

    [ClientRpc]
    private void RpcFadeBonus()
    {
        Destroy(gameObject.GetComponent<BoxCollider>()); //Убрать тригер
        gameObject.GetComponent<MeshRenderer>().enabled = false; //Сделать "невидимым"
        canRemove = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        GetComponents<AudioSource>()[1].Play();
        if (!other.gameObject.GetComponent<Bomberman>().isLocalPlayer)
            GetComponents<AudioSource>()[1].mute = true;

        GameObject Hero = other.gameObject;
        
        if (isServer)
            CmdChangeStats(Hero);
    }

    [Client]
    private void UpgradeUIStat()
    {
        Text text = GameObject.Find($"PanelBonusItem ({BonusType})").transform.GetChild(1).gameObject.GetComponent<Text>();
        if (text.text == "X")
            text.text = "V";
        else
        {
            if (text.text == "x09")
                text.text = "x10";
            else
                text.text = "x0" + (Convert.ToInt16(text.text.Substring(1, 2)) + 1);
        }
    }

    [Command]
    private void CmdChangeStats(GameObject Hero)
    {
        RpcUpgradeStat(Hero);
    }

    [ClientRpc]
    private void RpcUpgradeStat(GameObject Hero)
    {
        switch (BonusType)
        {
            case 0: //+1
                {
                    if (Hero.GetComponent<Bomberman>().stats.MaxBombAmount < 10)
                    {
                        Hero.GetComponent<Bomberman>().stats.MaxBombAmount++;
                        if (Hero.GetComponent<Bomberman>().isLocalPlayer)
                            UpgradeUIStat();
                    }
                        
                    break;
                }
            case 1: //Fire
                {
                    if (Hero.GetComponent<Bomberman>().stats.BombPower < 10)
                    {
                        Hero.GetComponent<Bomberman>().stats.BombPower++;
                        if (Hero.GetComponent<Bomberman>().isLocalPlayer)
                            UpgradeUIStat();
                    }
                        
                    break;
                }
            case 2: //Speed
                {
                    if (Hero.GetComponent<Bomberman>().stats.SuchSpeed < 5)
                    {
                        Hero.GetComponent<Bomberman>().stats.SuchSpeed += 0.3F;
                        if (Hero.GetComponent<Bomberman>().isLocalPlayer)
                            UpgradeUIStat();
                    }
                        
                    break;
                }
            case 3: //Remote
                {
                    if (!Hero.GetComponent<Bomberman>().stats.isDetonate)
                    {
                        Hero.GetComponent<Bomberman>().stats.isDetonate = true;
                        if (Hero.GetComponent<Bomberman>().isLocalPlayer)
                            UpgradeUIStat();
                    }
                        
                    break;
                }
        }
        
        if (isServer)
            CmdDestroyBonus();
    }

    [Command]
    private void CmdDestroyBonus()
    {
        RpcFadeBonus();
    }
}