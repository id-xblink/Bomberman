using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class LoadSettings : MonoBehaviour
{
    [SerializeField] //Полноэкранный режим
    private Toggle toggleFullScreen;
    [SerializeField] //Качество
    private Dropdown dropdownQuality;
    [SerializeField] //Разрешение
    private Dropdown dropdownResolution;
    [SerializeField] //Микшер настройки музыки и звуков
    private AudioMixer audioMixer;
    [SerializeField] //Общая громкость
    private Slider masterSlider;
    [SerializeField] //Громкость музыки
    private Slider musicSlider;
    [SerializeField] //Громкость звуков
    private Slider soundSlider;

    public void Awake()
    {
        dropdownResolution.ClearOptions(); //Очистка выпадающего списка
        List<string> ListResolutions = new List<string>(); //Объявление листа строк разрешений экрана
        foreach (Resolution resol in Screen.resolutions) //Цикл по всем доступным разрешениям
            ListResolutions.Add(resol.width + "x" + resol.height); //Запись разрешения в лист
        dropdownResolution.AddOptions(ListResolutions); //Запись листа в выпадающий список
    }

    private void Start() //Для загрузки настроек
    {
        //Загрузка полноэкранного режима
        if (PlayerPrefs.HasKey("FullScreen"))
            toggleFullScreen.isOn = Convert.ToBoolean(PlayerPrefs.GetString("FullScreen"));
        else
            Screen.fullScreen = false;
        //Загрузка качества
        if (PlayerPrefs.HasKey("Quality"))
            dropdownQuality.value = PlayerPrefs.GetInt("Quality");
        else
            dropdownQuality.value = 3;
        //Загрузка разрешения
        if (PlayerPrefs.HasKey("Resolution"))
            dropdownResolution.value = PlayerPrefs.GetInt("Resolution");
        else
            dropdownResolution.value = dropdownResolution.options.Count - 1;
        //Загрузка настроек громкости

        //Общая громкость
        if (PlayerPrefs.HasKey("MasterVolume"))
            masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        else
            masterSlider.value = 0;
        //Громкость музыки
        if (PlayerPrefs.HasKey("MusicVolume"))
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        else
            musicSlider.value = 0;
        //Громкость звуков
        if (PlayerPrefs.HasKey("SoundVolume"))
            soundSlider.value = PlayerPrefs.GetFloat("SoundVolume");
        else
            soundSlider.value = 0;
    }
}