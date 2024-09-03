using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Persistent : UI_Scene
{
    private enum Btns
    {
        Btn_Logo_MenuActivation, // 산기대 대학교 로고
        Btn_Main,
        Btn_Help,
        Btn_FullMenu,
        Btn_Setting,
        Btn_Close
    }

    private Animator _activationAnimator;
    private readonly int UI_ON = Animator.StringToHash("UI_On");
    private readonly Text[] hoverTexts = new Text[Enum.GetValues(typeof(Btns)).Length];

    private Canvas _canvas;
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

#if UNITY_EDITOR
        Debug.Log("Main 화면 구성--------------------------");
#endif
        
        gameObject.GetComponent<Canvas>().sortingOrder = 20;
        BindButton(typeof(Btns));
        

        _activationAnimator = GetButton((int)Btns.Btn_Logo_MenuActivation).gameObject.GetComponent<Animator>();
        GetButton((int)Btns.Btn_Logo_MenuActivation).gameObject
            .BindEvent(() =>
            {
                OnMouseEnterActivationBtn();
            }, Define.UIEvent.PointerEnter);
        
        GetButton((int)Btns.Btn_Logo_MenuActivation).gameObject
            .BindEvent(() =>
            {
                OnMouseExitFromActivationBtn();
            }, Define.UIEvent.PointerExit);

        
        SetupButton((int)Btns.Btn_Main, OnMainBtnClicked);
        SetupButton((int)Btns.Btn_Help, OnHelpBtnClicked);
        SetupButton((int)Btns.Btn_FullMenu, OnFullMenuBtnClicked);
        SetupButton((int)Btns.Btn_Setting, OnSettingBtnClicked);
        SetupButton((int)Btns.Btn_Close, OnCloseBtnClicked);

        return true;
    }

    // 버튼 설정을 위한 공통 메서드 입니다.
    private void SetupButton(int btnIndex, Action onClickAction)
    {
        // 클릭 Up 이벤트 설정
        GetButton(btnIndex).gameObject.BindEvent(onClickAction, Define.UIEvent.PointerUp);

        // 텍스트 설정
        hoverTexts[btnIndex] = GetButton(btnIndex).GetComponentInChildren<Text>();
        hoverTexts[btnIndex].gameObject.SetActive(false);

        // 마우스 포인터 이벤트 설정
        GetButton(btnIndex).gameObject.BindEvent(() => { SetHoverText(btnIndex); }, Define.UIEvent.PointerEnter);
        GetButton(btnIndex).gameObject.BindEvent(() => { SetHoverText(btnIndex, false); }, Define.UIEvent.PointerExit);
    }
    #region Mouse Hover Logic

    private void OnMouseEnterActivationBtn()
    {
#if UNITY_EDITOR
        Debug.Log("Hover Animation Activating");
#endif
        _activationAnimator.SetBool(UI_ON, true);
    }

    private void OnMouseExitFromActivationBtn()
    {
        DeactivatePersistentUI();
    }

    #endregion


    #region Btn Methods

    private void OnMainBtnClicked()
    {
        Managers.UI.CloseAllPopupUI();
        
        if (Managers.UI.FindPopup<UI_Main>() == null) Managers.UI.ShowPopupUI<UI_Main>();
        //if (Managers.UI.SceneUI<UI_Persistent>() == null) Managers.UI.ShowSceneUI<UI_Persistent>();
    }

    private void OnHelpBtnClicked()
    {
        Managers.UI.ShowPopupUI<UI_Help>();
    }

    private void OnFullMenuBtnClicked()
    {
        Managers.UI.ShowPopupUI<UI_FullMenu>();
    }

    private void OnSettingBtnClicked()
    {
        Managers.UI.ShowPopupUI<UI_Setting>();
    }


    private void OnCloseBtnClicked()
    {
        DeactivatePersistentUI();
    }

    #endregion

    public void SetHoverText(int idx, bool isOn = true)
    {
        hoverTexts[idx].gameObject.SetActive(isOn);
    }

    private void DeactivatePersistentUI()
    {
#if UNITY_EDITOR
        Debug.Log("Deactivate PersistentUI");
#endif
        _activationAnimator.SetBool(UI_ON, false);
    }
}