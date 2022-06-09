using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DropBonus : NetworkBehaviour
{
    [SerializeField]
    private GameObject BonusObject; //Префаб бонуса

    private bool isBreak = false;

    private void FixedUpdate()
    {
        if (isBreak)
        {
            if (!gameObject.GetComponent<AudioSource>().isPlaying && !gameObject.GetComponent<ParticleSystem>().isPlaying)
                Destroy(gameObject);
        }
    }

    [ClientRpc]
    public void RpcFadeBrick() //Очистка кирпича с поля
    {
        gameObject.GetComponent<ParticleSystem>().Play();
        gameObject.GetComponent<AudioSource>().Play();
        isBreak = true;
        Destroy(gameObject.GetComponent<BoxCollider>()); //Убрать твёрдое тело
        gameObject.GetComponent<MeshRenderer>().enabled = false; //Сделать "невидимым"
    }

    [Command]
    public void CmdDestroyBrick()
    {
        if (isBreak)
            return;
        RpcFadeBrick();

        Vector3 BoxPosition = gameObject.transform.position;
        BoxPosition.y = 1;
        
        int result = Random.Range(1, 101);
        if (result >= 1 && result <= 25)
        {
            GameObject BuffObject = Instantiate(BonusObject, BoxPosition, Quaternion.identity);

            result = Random.Range(1, 101);

            if (result >= 11 && result <= 40) //+1
                BuffObject.GetComponent<PickUpBonus>().BonusType = 0;

            if (result >= 41 && result <= 70) //Fire
                BuffObject.GetComponent<PickUpBonus>().BonusType = 1;

            if (result >= 71 && result <= 100) //Speed
                BuffObject.GetComponent<PickUpBonus>().BonusType = 2;

            if (result >= 1 && result <= 10) //Detonator
                BuffObject.GetComponent<PickUpBonus>().BonusType = 3;

            NetworkServer.Spawn(BuffObject);
            RpcSetting(BuffObject, BuffObject.GetComponent<PickUpBonus>().BonusType);
        }

    }

    [ClientRpc]
    private void RpcSetting(GameObject BuffObject, short result)
    {
        BuffObject.GetComponent<PickUpBonus>().BonusType = result;
    }
}