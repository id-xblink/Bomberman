using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Explosion : NetworkBehaviour
{
    public Bomberman player; //Скрипт игрока
    public LayerMask layerMask; //Маска поиска объектов, которые попали на взрыв бомбы
    public bool isExploded = false; //Взорвана ли бомба
    bool isDetonate = false; //Тип бомбы
    public int power = 0; //Сила взрыва
    private short Waves = 0; //Отвечает за взрывы по всем 4-м сторонам
    public AudioSource CurrentAudio; //Проигрываемый звук
    [SerializeField]
    private AudioClip[] clips = new AudioClip[4]; //Массив с различными звуками взрывов бомбы
    [SerializeField]
    private GameObject[] ExplosionsObjects = new GameObject[7]; //Массив со взрывами

    private void Start()
    {
        power = player.stats.BombPower;
        isDetonate = player.stats.isDetonate;
        if (!isDetonate)
        {
            Invoke("CmdFastBoom", 3f);
            Invoke("RpcPreBoom", 1.589f);
        }
    }

    void FixedUpdate() //Проверка на взрыв бомбы
    {
        if (Waves == 4 && !CurrentAudio.isPlaying)
            Destroy(gameObject);
    }

    [ClientRpc]
    public void RpcPreBoom() //Звук обычной мины
    {
        CurrentAudio.clip = clips[0];
        CurrentAudio.Play();
    }

    [ClientRpc]
    public void RpcPlaySoundBlowUp(int sound) //Выбрать звук взрыва
    {
        //0 - Превзрыв; 1 - Обычный взрыв; 2 - Детонирумый взрыв; 3 - Спровоцированный детонируемый взрыв
        CurrentAudio.clip = clips[sound];
        CurrentAudio.Play();
    }

    [ClientRpc]
    private void RpcFadeBomb() //Очистка бомбы с поля
    {
        isExploded = true; //Бомба взорвана
        gameObject.GetComponent<MeshRenderer>().enabled = false; //Сделать "невидимым"
        Destroy(gameObject.GetComponent<CharacterController>()); //Убрать твёрдое тело
    }

    [ClientRpc]
    private void RpcSetting()
    {
        if (isServer)
            return;

        if (!isDetonate)
            player.stats.CurrentBombAmount--; //Уменьшить количество поставленных бомб
        else
            player.stats.isPlanted = false; //Детонируемая бомба взорвалась
    }

    [Command]
    public void CmdFastBoom() //Взрыв бомбы
    {
        if (!isServer) //Проверка на сервер
            return;

        if (!isExploded) //Если бомба не взорвана
        {
            RpcFadeBomb(); //Скрытие бомбы

            if (!isDetonate)
            {
                RpcPlaySoundBlowUp(1); //Звук обычного взрыва
                player.stats.CurrentBombAmount--; //Уменьшить количество поставленных бомб
            }
            else
            {
                if (CurrentAudio.clip == null) //??? Если бомбу взорвал кто-то другой
                {
                    RpcPlaySoundBlowUp(3); //Звук детонируемого взрыва (спровоцированный)
                }
                player.stats.isPlanted = false; //Детонируемая бомба взорвалась
            }
            RpcSetting(); //Настройки для клиентов

            Vector3 basic = transform.position;
            
            //Выбор типа взрыва
            int result = Random.Range(1, 101);
            short ExplosionType = 0;
            if (result >= 1 && result <= 1) //Black/White
                ExplosionType = (short)Random.Range(4, 6);
            if (result >= 2 && result <= 6) //Color
                ExplosionType = player.ColorType;
            if (result >= 7 && result <= 100) //Default
                ExplosionType = 6;

            BlowUp(basic, 0, ExplosionType); //Первоначальный взрыв
            //Начать распространение взрывов
            StartCoroutine(TrackBoom(basic, Vector3.forward, ExplosionType)); //Вперёд
            StartCoroutine(TrackBoom(basic, Vector3.back, ExplosionType)); //Назад
            StartCoroutine(TrackBoom(basic, Vector3.left, ExplosionType)); //Влево
            StartCoroutine(TrackBoom(basic, Vector3.right, ExplosionType)); //Вправо
        }
    }

    private void BlowUp(Vector3 pos, int i, short type) //Взрыв
    {
        GameObject boom = Instantiate(ExplosionsObjects[type], pos, Quaternion.identity); //Создать взрыв локально
        NetworkServer.Spawn(boom); //Отправить взрыв каждому игроку
        Destroy(boom, 0.5f);
    }

    private IEnumerator TrackBoom(Vector3 start, Vector3 direction, short type)
    {
        for (int i = 1; i <= power; i++)
        {
            Physics.Raycast(start + (i - 1) * direction, direction, out RaycastHit hit, 1, layerMask); //Проверка лучом местности

            if (!hit.collider) //Если не встретил препятствие
            {
                BlowUp(start + (i * direction), i, type);
            }
            else //Если встретил препятствие
            {
                if (LayerMask.LayerToName(hit.collider.gameObject.layer) == "Brick")
                {
                    BlowUp(start + (i * direction), i, type);
                    hit.collider.gameObject.GetComponent<DropBonus>().CmdDestroyBrick();
                }
                break;
            }
            yield return new WaitForSeconds(0.025f);
        }
        Waves++;
    }
}