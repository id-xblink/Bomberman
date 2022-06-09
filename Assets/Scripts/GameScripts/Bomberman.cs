using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bomberman : NetworkBehaviour
{
    private NetworkManager network; //Нетворк
    private InitializeGame Initialize;

    private bool ActionBlock = false; //Блокировка управления
    public Stats stats; //Статы игрока

    public CharacterController ch_controller; //Контроллер
    public Animator ch_animator; //Аниматор для анимаций

    public Actions actions; //Скрипт с действиями
    
    public Material[] skins = new Material[4]; //Скины персонажа

    public short ColorType = -1;

    [SerializeField]
    private string layerName = "Player";

    private void Start()
    {
        gameObject.name = "Player " + GetComponent<NetworkIdentity>().netId; //Установка название объекта с NetID
        if (!isLocalPlayer)
            gameObject.layer = LayerMask.NameToLayer(layerName); //Установка слоя удалённого игрока
        network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        Initialize = GameObject.Find("GameInitializer").GetComponent<InitializeGame>();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer || ActionBlock || !Initialize.isGameStart)
            return;

        actions.CharacterMove();
        actions.GamingGravity();
    }

    private void Update()
    {
        if (!isLocalPlayer || Initialize.isGameEnd || !Initialize.isGameStart)
            return;

        if (Input.GetKeyUp(KeyCode.Space) && !ActionBlock)
            actions.CmdPlaceBomb();

        if (Input.GetKeyUp(KeyCode.Escape))
            ActionBlock = !ActionBlock;
    }
}