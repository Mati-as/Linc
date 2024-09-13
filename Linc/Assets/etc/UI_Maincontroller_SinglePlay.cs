using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Maincontroller_SinglePlay : UI_Scene
{
    public enum Btns
    {
        Btn_StartGame,
        Btn_HideMenu,
        Btn_Play,
        Btn_Replay,
        Btn_Quit,
        Btn_ApplicationQuit,
        Btn_Setting
        
    }
    
    
    public enum UIObjs
    {
        Btn_Menus,
        Image_Background,
        UI_Setting
    }
    
        
    
    private Canvas _canvas;
    private Animator _menuAnimator;
    private int UI_ON = Animator.StringToHash("On");
    private bool _isUiOn = false;
    public static event Action OnStartBtnClickedAction;
    private Image _bg;
    private Color _defaultColor;

    //음악

    
        public override bool Init()
    {
        
       
        
        BindObject(typeof(UIObjs));
        BindButton(typeof(Btns));
        
        _menuAnimator = GetObject((int)UIObjs.Btn_Menus).gameObject.GetComponent<Animator>();
        GetButton((int)Btns.Btn_HideMenu).gameObject.BindEvent(ToggleAnimation);
        
        _bg = GetObject((int)UIObjs.Image_Background).gameObject.GetComponent<Image>();
        _defaultColor = _bg.color;
        GetButton((int)Btns.Btn_StartGame).gameObject.BindEvent(OnStartBtnClicked);
        GetButton((int)Btns.Btn_StartGame).gameObject.SetActive(false);
        GetButton((int)Btns.Btn_StartGame).gameObject.GetComponent<Image>().DOFade(0, 0.01f);
        GetButton((int)Btns.Btn_StartGame).gameObject.GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 0.01f);
        _canvas = GetComponent<Canvas>();
        _canvas.sortingOrder = -321; // 항상플레이 화면에만 있을수 있도록

        
        
        //생성후 프리팹을 파괴하지않고 값을 보존
        GetObject((int)UIObjs.UI_Setting).gameObject.SetActive(false);
        
        GetButton((int)Btns.Btn_Setting).gameObject.BindEvent(() =>
        {
           
                GetObject((int)UIObjs.UI_Setting).gameObject.SetActive(true);
        });
        
        SetInGameUIs(false);
        GetButton((int)Btns.Btn_ApplicationQuit).gameObject.SetActive(true);
        GetButton((int)Btns.Btn_Setting).gameObject.SetActive(true);
        GetButton((int)Btns.Btn_Quit).gameObject.BindEvent(OnQuitBtnClicked);
        
        GetButton((int)Btns.Btn_Play).gameObject.BindEvent(OnPlayBtnClicked);
        GetButton((int)Btns.Btn_Replay).gameObject.BindEvent(OnReplayBtnClicked);
        
        GetButton((int)Btns.Btn_ApplicationQuit).gameObject.BindEvent(()=>
        {
            OnApplicationQuitClicked();
        });
        
        GetObject((int)UIObjs.Btn_Menus).gameObject.SetActive(false);
        return base.Init();
    }
     
    private void SetInGameUIs(bool value)
    {
        GetButton((int)Btns.Btn_Quit).gameObject.SetActive(value);
        GetButton((int)Btns.Btn_Setting).gameObject.SetActive(value);
        GetButton((int)Btns.Btn_ApplicationQuit).gameObject.SetActive(value);
    }

    public void ShowStartBtn()
    {
        GetButton((int)Btns.Btn_StartGame).gameObject.SetActive(true);
        Managers.Sound.Play(SoundManager.Sound.Effect, "Audio/Common/UI_Message_Button", 0.3f);
        GameObject.FindWithTag("GameManager").GetComponent<Solo_BeadsDrum_GameManager>().isStartButtonClicked = true;
        
        GetButton((int)Btns.Btn_StartGame).gameObject.GetComponent<Image>().DOFade(1, 0.5f).SetDelay(1.5f);
        GetButton((int)Btns.Btn_StartGame).gameObject.GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.5f).SetDelay(1.5f);

    }

    private void OnQuitBtnClicked()
    {
        Managers.RestartSceneWithRemoveDontDestroy();
    }
    private void OnApplicationQuitClicked()
    {
        Managers.RestartSceneWithRemoveDontDestroy();
        Application.Quit();
    }

    public static event Action<bool> PlayMusicEvent; 
    private void OnPlayBtnClicked()
    {
        if (!Managers.Sound.audioSources[(int)SoundManager.Sound.Narration].isPlaying)
        {
            Managers.Sound.Stop(SoundManager.Sound.Bgm);
            Managers.Sound.Play(SoundManager.Sound.Bgm, "Audio/Narration/Carrot",Managers.Data.Preference[(int)Define.Preferences.BgmVol]);
            var isReplayBtn = false; // 드럼초기화로직 구분
            PlayMusicEvent?.Invoke(isReplayBtn);
            
        }
    }
    
    private void OnReplayBtnClicked()
    {
        
        Managers.Sound.Stop(SoundManager.Sound.Narration);
        Managers.Sound.Play(SoundManager.Sound.Narration, "Audio/Narration/Carrot",Managers.Data.Preference[(int)Define.Preferences.BgmVol]);
        var isReplayBtn = true; // 드럼초기화로직 구분
        PlayMusicEvent?.Invoke(true);
    }

    private void ToggleAnimation()
    {
        _isUiOn = !_isUiOn;   
        _menuAnimator.SetBool(UI_ON,_isUiOn);
    }


    public static bool IsStartBtnClicked { get; private set; } 
    
    private void OnStartBtnClicked()
    {
     
        Debug.Log("StartBtn Clicked");
        _bg.DOFade(0, 1f);
       
        GetButton((int)Btns.Btn_StartGame).gameObject.GetComponent<Image>().DOFade(0, 0.5f);
        GetButton((int)Btns.Btn_StartGame).gameObject.GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 0.5f).OnComplete(
            () =>
            {
                GetButton((int)Btns.Btn_StartGame).gameObject.SetActive(false);
            });
        SetInGameUIs(true);
        IsStartBtnClicked = true;
        OnStartBtnClickedAction?.Invoke();
        GetObject((int)UIObjs.Btn_Menus).gameObject.SetActive(true);
        _menuAnimator.SetBool(UI_ON,true);
    }

        
}