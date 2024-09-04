using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PersistentController : UI_Scene
{

    public enum Btns
    {
        Btn_Start,
        Btn_HideMenu,
        Btn_Play,
        Btn_Pause,
        Btn_Replay,
        Btn_Home
        
    }

    public enum UIObjs
    {
        Btn_Menus,
        Image_Background
    }
    
    
    private Canvas _canvas;
    private Animator _menuAnimator;
    private int UI_ON = Animator.StringToHash("On");
    private bool _isUiOn = false;

    private Image _bg;
    private Color _defaultColor;
 
    public static event Action OnStartBtnClickedAction;
    public override bool Init()
    {
       
        BindObject(typeof(UIObjs));
        BindButton(typeof(Btns));
        
        _menuAnimator = GetObject((int)UIObjs.Btn_Menus).gameObject.GetComponent<Animator>();
        GetButton((int)Btns.Btn_HideMenu).gameObject.BindEvent(ToggleAnimation);
        
        _bg = GetObject((int)UIObjs.Image_Background).gameObject.GetComponent<Image>();
        _defaultColor = _bg.color;
        GetButton((int)Btns.Btn_Start).gameObject.BindEvent(OnStartBtnClicked);
        GetButton((int)Btns.Btn_Start).gameObject.SetActive(false);
        GetButton((int)Btns.Btn_Start).gameObject.GetComponent<Image>().DOFade(0, 0.01f);
        GetButton((int)Btns.Btn_Start).gameObject.GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 0.01f);
        _canvas = GetComponent<Canvas>();
        _canvas.sortingOrder = -321; // 항상플레이 화면에만 있을수 있도록
        
        
        GetButton((int)Btns.Btn_Home).gameObject.BindEvent(OnHomeBtnClicked);
        return base.Init();
    }

    public void ShowStartBtn()
    {
        GetButton((int)Btns.Btn_Start).gameObject.SetActive(true);
        Managers.Sound.Play(SoundManager.Sound.Effect, "Audio/Common/UI_Message_Button", 0.3f);
        
        GetButton((int)Btns.Btn_Start).gameObject.GetComponent<Image>().DOFade(1, 0.5f).SetDelay(1.5f);
        GetButton((int)Btns.Btn_Start).gameObject.GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.5f).SetDelay(1.5f);

    }

    private void OnHomeBtnClicked()
    {
        Managers.UI.CloseAllPopupUI();
        Managers.Scene.ChangeScene(Define.Scene.linc_main_solo);
    }
    private void OnStartBtnClicked()
    {
        Debug.Log("Clicked");
        _bg.DOFade(0, 1f);
        OnStartBtnClickedAction?.Invoke();
        GetButton((int)Btns.Btn_Start).gameObject.GetComponent<Image>().DOFade(0, 0.5f);
        GetButton((int)Btns.Btn_Start).gameObject.GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 0.5f);
    }

    private void ToggleAnimation()
    {
        _isUiOn = !_isUiOn;   
        _menuAnimator.SetBool(UI_ON,_isUiOn);
    }
}
