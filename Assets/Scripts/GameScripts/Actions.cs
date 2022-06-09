using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Actions : NetworkBehaviour
{
    public Bomberman player; //Скрипт игрока
    private float gravityForce; //Гравитация
    private Vector3 moveVector; //Передвижение
    [SerializeField]
    private GameObject bomb; //Префаб бомбы
    private GameObject DetonateBomb; //Детонируемая бомба
    [SerializeField]
    private LayerMask BombLayer;

    public void CharacterMove()
    {
        if (!GameObject.Find("GameInitializer").GetComponent<InitializeGame>().isGameEnd)
        {
            moveVector = Vector3.zero;
            moveVector.x = Input.GetAxis("Horizontal") * player.stats.SuchSpeed;
            moveVector.z = Input.GetAxis("Vertical") * player.stats.SuchSpeed;
        }
        else
        {
            moveVector.x = 0;
            moveVector.z = 0;
        }
        //Анимация
        if (moveVector.x != 0 || moveVector.z != 0)
            player.ch_animator.SetBool("Move", true);
        else
            player.ch_animator.SetBool("Move", false);

        if (!GameObject.Find("GameInitializer").GetComponent<InitializeGame>().isGameEnd)
            if (Vector3.Angle(Vector3.forward, moveVector) > 1f || Vector3.Angle(Vector3.forward, moveVector) == 0)
            {
                Vector3 direct = Vector3.RotateTowards(transform.forward, moveVector, player.stats.SuchSpeed, 0.0f);
                transform.rotation = Quaternion.LookRotation(direct);
            }
        moveVector.y = gravityForce;
        player.ch_controller.Move(moveVector * Time.deltaTime);
    }

    public void GamingGravity()
    {
        if (!player.ch_controller.isGrounded)
            gravityForce -= 20f * Time.deltaTime;
        else
            gravityForce = -1f;
    }

    [Command]
    public void CmdPlaceBomb()
    {
        if (player.stats.CurrentBombAmount < player.stats.MaxBombAmount)
        {
            Vector3 vector = player.ch_controller.transform.position;
            vector.x = Mathf.Floor(vector.x) + (float)0.5;
            vector.z = Mathf.Floor(vector.z) + (float)0.5;
            vector.y = 0.5f;


            Collider[] hitColliders = Physics.OverlapSphere(vector, 0.45f, BombLayer);
            if (hitColliders.Length == 0 || player.stats.isPlanted)
                if (!player.stats.isDetonate)
                {
                    player.stats.CurrentBombAmount++;
                    GameObject go = Instantiate(bomb, vector, Quaternion.identity);
                    go.GetComponent<Explosion>().player = player;
                    NetworkServer.Spawn(go);
                    RpcSetting(go);
                }
                else
                {
                    if (!player.stats.isPlanted)
                    {
                        player.stats.isPlanted = true;
                        DetonateBomb = Instantiate(bomb, vector, Quaternion.identity);
                        DetonateBomb.GetComponent<Explosion>().player = player;
                        NetworkServer.Spawn(DetonateBomb);
                        RpcSetting(DetonateBomb);
                    }
                    else
                    {
                        if (DetonateBomb.GetComponent<Explosion>().CurrentAudio.clip == null)
                            DetonateBomb.GetComponent<Explosion>().RpcPlaySoundBlowUp(2); //Звук детонируемого взрыва
                        DetonateBomb.GetComponent<Explosion>().Invoke("CmdFastBoom", 0.45f);
                    }
                }
        }    
    }

    [ClientRpc]
    public void RpcSetting(GameObject bomb)
    {
        if (isServer)
            return;

        if (player.stats.isDetonate)
            player.stats.isPlanted = true;
        player.stats.CurrentBombAmount++;
        bomb.GetComponent<Explosion>().player = player;
    }
}