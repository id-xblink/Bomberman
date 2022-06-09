using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Strike : NetworkBehaviour
{
    private GameObject bomb; //Бомба

    private void OnTriggerEnter(Collider other)
    {
        switch (LayerMask.LayerToName(other.gameObject.layer))
        {
            case "Bomb": //Цепная реакция
                {
                    bomb = other.gameObject;
                    if (isServer)
                        CmdBombChain(bomb);
                    break;
                }
            default:
                {
                    if (other.tag == "Player")
                    {
                        GameObject.Find("NetworkManager").GetComponent<AudioSource>().Play();
                        if (isServer)
                            CmdDenied(other.gameObject);
                    }
                    break;
                }
        }
    }

    [Command]
    private void CmdBombChain(GameObject bomb)
    {
        RpcSetting(bomb);
    }

    [ClientRpc]
    private void RpcSetting(GameObject bomb)
    {
        bomb.GetComponent<Explosion>().CancelInvoke();
        bomb.GetComponent<Explosion>().Invoke("CmdFastBoom", 0f);
    }

    [Command]
    private void CmdDenied(GameObject go)
    {
        RpcDenied(go);
        GameObject.Find("GameInitializer").GetComponent<InitializeGame>().CmdRemovePlayer(go);
    }

    [ClientRpc]
    private void RpcDenied(GameObject go)
    {
        Instantiate(go.transform.GetChild(6).gameObject.GetComponent<ParticleSystem>(), go.transform.position, go.transform.rotation).Play();
        Instantiate(go.transform.GetChild(7).gameObject.GetComponent<ParticleSystem>(), go.transform.position, go.transform.rotation).Play();
        go.SetActive(false);
    }
}