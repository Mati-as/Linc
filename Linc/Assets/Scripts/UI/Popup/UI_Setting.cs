using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Setting : UI_Popup
{
    private enum Btns
    {
        Btn_Close,
        Btn_SensorRefresh
    }

    public enum Sliders
    {
        Slider_Main,
        Slider_Narration,
        Slider_Effect,
        Slider_Bgm,
        Slider_HapticStick_Integration
        // -----------------
       //Slider_ScreenMode,
        //Slider_Language,
       // Slider_ControlGuideOn
    }

    public enum Toggles
    {
        Toggle_Main_Mute,
        Toggle_Narration_Mute,
        Toggle_Effect_Mute,
        Toggle_Bgm_Mute,
        Toggle_HaptickStick
      
        // Toggle_GraphicQuality_Low,
        // Toggle_GraphicQuality_Mid,
        // Toggle_GraphicQuality_High,
        // Toggle_GraphicQuality_Auto,
        // Toggle_Resolution_1280x720,
        // Toggle_Resolution_1920x1080,
        // Toggle_Resolution_2560x1440
    }

    //sensor-related part.-----------------------------------
    public static event Action OnRefreshEvent;
    public static event Action<string, DateTime> OnSceneQuit;
    public static event Action<string, DateTime> OnAppQuit;

    public readonly int OFF = 0;
    public readonly int ON = 1;

    private Button[] _btns;
    private Slider[] _volumeSliders;

    private Text _text_hapticstick;
    // scene-related part -----------------------------------
    private Sequence _connectBlinkAnimSeq;
    private bool _isSequencePlaying; // 중복 연결시도 방지.

    private void OnDestroy()
    {
        
        Stick_DataController.OnConnectedEvent -= OnBlueToothConnected;
        Stick_DataController.OnConnectionFailedEvent -= OnBlueToothDisConnected;
    }

    // Start is called before the first frame update
    public override bool Init()
    {
        if (base.Init() == false)return false;

        
        
        Stick_DataController.OnConnectedEvent -= OnBlueToothConnected;
        Stick_DataController.OnConnectionFailedEvent -= OnBlueToothDisConnected;
        
        Stick_DataController.OnConnectedEvent += OnBlueToothConnected;
        Stick_DataController.OnConnectionFailedEvent += OnBlueToothDisConnected;
        
        
#if UNITY_EDITOR
        Debug.Log("UI_SETTING INIT------------------------------------------------------");
#endif
        Managers.Data.LoadSettingParams();

        // BindObject(typeof(GameObj));
        BindButton(typeof(Btns));
        BindToggle(typeof(Toggles));
        BindSlider(typeof(Sliders));

        GetToggle((int)Toggles.Toggle_Main_Mute).onValueChanged.AddListener(isOn =>
        {
            if (GetToggle((int)Toggles.Toggle_Main_Mute).isOn)
            {
                Managers.Sound.SetMute(SoundManager.Sound.Main);
                Managers.Sound.SetMute(SoundManager.Sound.Narration);
                Managers.Sound.SetMute(SoundManager.Sound.Effect);
                Managers.Sound.SetMute(SoundManager.Sound.Bgm);
                GetToggle((int)Toggles.Toggle_Main_Mute).isOn = true;
                GetToggle((int)Toggles.Toggle_Narration_Mute).isOn = true;
                GetToggle((int)Toggles.Toggle_Effect_Mute).isOn = true;
                GetToggle((int)Toggles.Toggle_Bgm_Mute).isOn = true;
                Managers.Data.Preference[(int)Define.Preferences.Mute_Bgm] = 1;
                Managers.Data.Preference[(int)Define.Preferences.Mute_Effect] = 1;
                Managers.Data.Preference[(int)Define.Preferences.Mute_Main] = 1;
                Managers.Data.Preference[(int)Define.Preferences.Mute_Narration] = 1;
            }
            else
            {
                Managers.Sound.SetMute(SoundManager.Sound.Main, false);
                Managers.Sound.SetMute(SoundManager.Sound.Narration, false);
                Managers.Sound.SetMute(SoundManager.Sound.Effect, false);
                Managers.Sound.SetMute(SoundManager.Sound.Bgm, false);
                GetToggle((int)Toggles.Toggle_Main_Mute).isOn = false;
                GetToggle((int)Toggles.Toggle_Narration_Mute).isOn = false;
                GetToggle((int)Toggles.Toggle_Effect_Mute).isOn = false;
                GetToggle((int)Toggles.Toggle_Bgm_Mute).isOn = false;

                Managers.Data.Preference[(int)Define.Preferences.Mute_Bgm] = 0;
                Managers.Data.Preference[(int)Define.Preferences.Mute_Effect] = 0;
                Managers.Data.Preference[(int)Define.Preferences.Mute_Main] = 0;
                Managers.Data.Preference[(int)Define.Preferences.Mute_Narration] = 0;
            }

            Managers.Data.Preference[(int)Define.Preferences.Mute_Main] =
                GetToggle((int)Toggles.Toggle_Effect_Mute).isOn ? 1 : 0;
            Managers.Data.Preference[(int)Define.Preferences.Mute_Narration] =
                GetToggle((int)Toggles.Toggle_Narration_Mute).isOn ? 1 : 0;
            Managers.Data.Preference[(int)Define.Preferences.Mute_Effect] =
                GetToggle((int)Toggles.Toggle_Effect_Mute).isOn ? 1 : 0;
            Managers.Data.Preference[(int)Define.Preferences.Mute_Bgm] =
                GetToggle((int)Toggles.Toggle_Bgm_Mute).isOn ? 1 : 0;
        });

        GetToggle((int)Toggles.Toggle_Narration_Mute).onValueChanged.AddListener(isOn =>
        {
            if (GetToggle((int)Toggles.Toggle_Narration_Mute).isOn)
                Managers.Sound.SetMute(SoundManager.Sound.Narration);
            else
                Managers.Sound.SetMute(SoundManager.Sound.Narration, false);


            Managers.Data.Preference[(int)Define.Preferences.Mute_Narration] =
                GetToggle((int)Toggles.Toggle_Narration_Mute).isOn ? 1 : 0;
        });

        GetToggle((int)Toggles.Toggle_Effect_Mute).onValueChanged.AddListener(isOn =>
        {
            if (GetToggle((int)Toggles.Toggle_Effect_Mute).isOn)
                Managers.Sound.SetMute(SoundManager.Sound.Effect);
            else
                Managers.Sound.SetMute(SoundManager.Sound.Effect, false);
            Managers.Data.Preference[(int)Define.Preferences.Mute_Effect] =
                GetToggle((int)Toggles.Toggle_Effect_Mute).isOn ? 1 : 0;
        });

        GetToggle((int)Toggles.Toggle_Bgm_Mute).onValueChanged.AddListener(isOn =>
        {
            if (GetToggle((int)Toggles.Toggle_Bgm_Mute).isOn)
                Managers.Sound.SetMute(SoundManager.Sound.Bgm);
            else
                Managers.Sound.SetMute(SoundManager.Sound.Bgm, false);
            Managers.Data.Preference[(int)Define.Preferences.Mute_Bgm] =
                GetToggle((int)Toggles.Toggle_Bgm_Mute).isOn ? 1 : 0;
        });

        
        
        
        _text_hapticstick = GetToggle((int)Toggles.Toggle_HaptickStick).transform.GetChild(1).GetComponentInChildren<Text>();
        GetToggle((int)Toggles.Toggle_HaptickStick).onValueChanged.AddListener(_=>
        {

            if (_isSequencePlaying) return;
            _isSequencePlaying = true;
            
            _connectBlinkAnimSeq = DOTween.Sequence();
            
            _text_hapticstick.text = "연결 중...";
            Managers.DeviceManager.OnBtnClick();
            
            
              _connectBlinkAnimSeq.AppendCallback(() =>
              {
                  GetToggle((int)Toggles.Toggle_HaptickStick).isOn = true;
                  _text_hapticstick.text = "연결 중...";
              });
              _connectBlinkAnimSeq.AppendInterval(0.75f);
              _connectBlinkAnimSeq.AppendCallback(() =>
              {
                  GetToggle((int)Toggles.Toggle_HaptickStick).isOn = false;
                  _text_hapticstick.text = "연결 중..";
              });
              _connectBlinkAnimSeq.AppendInterval(0.75f);
              
              _connectBlinkAnimSeq.SetLoops(3, LoopType.Restart);
              _connectBlinkAnimSeq.Play();

        });
        
        

        GetButton((int)Btns.Btn_Close).gameObject.BindEvent(() =>
        {

            Managers.Data.SaveCurrentSetting();
            Managers.UI.ClosePopupUI(this);
        });

        SetVolumeSlider();

        InitialSetting();
        

        return true;
    }


 
    private void OnBlueToothConnected()
    {
        _connectBlinkAnimSeq.Kill();
        _connectBlinkAnimSeq = null;
        _isSequencePlaying = true; 
        Logger.Log("Hapticstick (BlueTooth) is Connected-----------------");
        GetToggle((int)Toggles.Toggle_HaptickStick).isOn = true;
        _text_hapticstick.text = "연결 됨";
    }

    private void OnBlueToothDisConnected()
    {
        _connectBlinkAnimSeq.Kill();
        _connectBlinkAnimSeq = null;
        _isSequencePlaying = false;
        Logger.Log("Hapticstick (BlueTooth) is Disconnected or Failed to Connect-----------------");
        GetToggle((int)Toggles.Toggle_HaptickStick).isOn = false;
        _text_hapticstick.text =  "연결 안 됨";
    }

    /// <summary>
    ///     볼륨,
    /// </summary>
    public void InitialSetting()
    {
        
        _volumeSliders[(int)SoundManager.Sound.Main].value = Managers.Data.Preference[(int)Define.Preferences.MainVol];

        _volumeSliders[(int)SoundManager.Sound.Narration].value =
            Managers.Data.Preference[(int)Define.Preferences.NarrationVol];

        _volumeSliders[(int)SoundManager.Sound.Effect].value =
            Managers.Data.Preference[(int)Define.Preferences.EffectVol];

        _volumeSliders[(int)SoundManager.Sound.Bgm].value = Managers.Data.Preference[(int)Define.Preferences.BgmVol];

        GetToggle((int)Toggles.Toggle_Main_Mute).isOn =
            (int)Managers.Data.Preference[(int)Define.Preferences.Mute_Main] == Define.OFF ? false : true;
        GetToggle((int)Toggles.Toggle_Narration_Mute).isOn =
            (int)Managers.Data.Preference[(int)Define.Preferences.Mute_Effect] == Define.OFF ? false : true;
        GetToggle((int)Toggles.Toggle_Effect_Mute).isOn =
            (int)Managers.Data.Preference[(int)Define.Preferences.Mute_Effect] == Define.OFF ? false : true;
        GetToggle((int)Toggles.Toggle_Bgm_Mute).isOn =
            (int)Managers.Data.Preference[(int)Define.Preferences.Mute_Bgm] == Define.OFF ? false : true;
        

        var count = 0;
        foreach (var val in Managers.Data.Preference)
        {
            Debug.Log($"Load: {(Define.Preferences)count} is {Managers.Data.Preference[count]}");
            count++;
        }
        
    }


    private void SetVolumeSlider()
    {
        _volumeSliders = new Slider[(int)SoundManager.Sound.Max];
        for (var sliderIndex = 0; sliderIndex < (int)SoundManager.Sound.Max; sliderIndex++)
            _volumeSliders[sliderIndex] = GetSlider(sliderIndex);


        for (var i = 0; i < Managers.Sound.volumes.Length; i++)
        {
            _volumeSliders[i].maxValue = Managers.Sound.VOLUME_MAX[i];
            _volumeSliders[i].value = Managers.Sound.volumes[i];

#if UNITY_EDITOR
            if (i == (int)Sliders.Slider_Main) Debug.Log($" 메인 볼륨 {Managers.Sound.volumes[i]}");
#endif

            var audioIndex = i;
            _volumeSliders[i].onValueChanged.AddListener(_ =>
            {
                Managers.Sound.volumes[audioIndex] = _volumeSliders[audioIndex].value;

                if (audioIndex == (int)SoundManager.Sound.Effect)
                    if (Managers.Sound.audioSources[(int)SoundManager.Sound.Effect] &&
                        !Managers.Sound.audioSources[audioIndex].isPlaying)
                        Managers.Sound.Play(SoundManager.Sound.Effect, "Audio/Test_Effect");


                if (audioIndex == (int)Sliders.Slider_Narration && !Managers.Sound.audioSources[audioIndex].isPlaying)
                    Managers.Sound.Play(SoundManager.Sound.Narration, "Audio/Test_Narration");

                Managers.Sound.audioSources[audioIndex].volume =
                    Mathf.Lerp(0, Managers.Sound.VOLUME_MAX[audioIndex],
                        Managers.Sound.volumes[(int)SoundManager.Sound.Main] *
                        _volumeSliders[audioIndex].value);

                UpdateLinkedVolumes();
            });
        }
    }

    private void UpdateLinkedVolumes()
    {
        UpdateVolume((int)SoundManager.Sound.Main);
        UpdateVolume((int)SoundManager.Sound.Bgm);
        UpdateVolume((int)SoundManager.Sound.Effect);
        UpdateVolume((int)SoundManager.Sound.Narration);
    }


    private readonly float FLOAT_TOLERANCE = 0.005f;

    private void UpdateVolume(int index)
    {
        Managers.Sound.volumes[index] = _volumeSliders[index].value;
        Managers.Sound.audioSources[index].volume =
            Mathf.Lerp(0, Managers.Sound.VOLUME_MAX[index],
                Managers.Sound.volumes[(int)SoundManager.Sound.Main] *
                _volumeSliders[index].value);


        if (Math.Abs(Managers.Data.Preference[index] - Managers.Sound.audioSources[index].volume) <
            FLOAT_TOLERANCE) return;
        Managers.Data.Preference[index] = GetSlider(index).value;

        Debug.Log("Volume Updated (Not saved Yet) \n " +
                  $"Preference: {(Define.Preferences)index} is " +
                  $"Volume Name:{(SoundManager.Sound)index} {Managers.Sound.audioSources[index].volume} -----------");
    }


    private void OnResolutionChanged(int width, int height)
    {
        Debug.Assert(width == 1920 || width == 1280 || width == 2560);
#if UNITY_EDITOR
        Debug.Log($"Resolution Change => {width} x {height} ");
# endif

        Managers.UI.SetResolution(width, height, Managers.UI.isFullScreen);
        Managers.Data.Preference[(int)Define.Preferences.Resolution] = width;

    }

    private void OnGraphicQualityChanged(Define.QaulityLevel qaulityLevel)
    {
        Debug.Assert((int)qaulityLevel < 6 || (int)qaulityLevel > 0);
#if UNITY_EDITOR
        Debug.Log($"Graphic Quality {qaulityLevel}");
# endif
        Managers.UI.SetGraphicQuality(qaulityLevel);

        Managers.Data.Preference[(int)Define.Preferences.GraphicQuality] = (int)qaulityLevel;
    }


    #region Archive

    private void OnQuit()
    {
        StartCoroutine(QuitApplicationCo());
    }

    private IEnumerator QuitApplicationCo()
    {
#if UNITY_EDITOR
        Debug.Log("App Quit ");
# endif

        OnAppQuit?.Invoke(SceneManager.GetActiveScene().name, DateTime.Now);
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }


    /// <summary>
    ///     씬이동 초기화 수행 전, 다양한 초기화를 진행합니다.
    /// </summary>
    private void TerminateProcess()
    {
    }

//     public void SetLanguageWithConfirmation()
//     {
//         Managers.UI.languageSetting = (int)GetSlider((int)Sliders.Slider_Language).value;
//
//         if (GetSlider((int)Sliders.Slider_Language).value >= 1)
//         {
//             Managers.Data.Preference[(int)Define.Preferences.EngMode] = 1;
//             Managers.UI.SetEngMode(UIManager.ENG);
// #if UNITY_EDITOR
//             Debug.Log("Preference Changed --------------------------EngMode On");
// #endif
//         }
//
//         else
//         {
// #if UNITY_EDITOR
//             Debug.Log("Preference Changed --------------------------EngMode off");
// #endif
//             Managers.Data.Preference[(int)Define.Preferences.EngMode] = 0;
//             Managers.UI.SetEngMode(UIManager.KOR);
//         }
//     }

    private const int REFRESH_INTERIM_MIN = 10;
    private readonly WaitForSeconds _wait = new(REFRESH_INTERIM_MIN);
    private IEnumerator ResetSensorRefreshableCo()
    {
        _isSensorRefreshable = false;
        yield return _wait;
        _isSensorRefreshable = true;
    }
    private bool _isSensorRefreshable = true;
    private void RefreshSensor()
    {
        if (!_isSensorRefreshable) return;

        StartCoroutine(ResetSensorRefreshableCo());
        OnRefreshEvent?.Invoke();
    }



#endregion
}