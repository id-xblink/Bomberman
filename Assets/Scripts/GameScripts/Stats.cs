using System;

[Serializable]
public class Stats //Статы игрока
{
    public float SuchSpeed = 2; //Скорость

    public int MaxBombAmount = 1; //Количество доступных бомб
    public int CurrentBombAmount = 0; //Поставлено бомб
    public int BombPower = 1; //Радиус взрыва бомб
    public bool isDetonate = false; //Детанируемая бомба

    public bool isPlanted = false; //Поставлена ли детанируемая бомба
}