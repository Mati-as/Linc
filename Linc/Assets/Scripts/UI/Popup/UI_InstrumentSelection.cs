using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UI_InstrumentSelection : UI_Popup
{
    public enum Btns
    {
        Btn_Drum,
        Btn_HandBell,
    }

    public static Action OnInstrumentSelected;

    public override bool Init()
    {   
        
        if (base.Init() == false) return false;
        BindButton(typeof(Btns));
        GetButton((int)Btns.Btn_HandBell).gameObject.BindEvent(OnHandBellBtnClicked);
        GetButton((int)Btns.Btn_Drum).gameObject.BindEvent(OnDrumPlayBtnClicked);
        return base.Init();
    }
    
    private void OnHandBellBtnClicked()
    {
        Managers.ContentInfo.PlayData.CurrentInstrument = (int)Define.Instrument.HandBell;
        Managers.UI.ClosePopupUI(this);
        Managers.UI.SceneUI.GetComponent<UI_PersistentController>().ShowStartBtn();
        OnInstrumentSelected?.Invoke();
    }

    private void OnDrumPlayBtnClicked()
    {
        Managers.ContentInfo.PlayData.CurrentInstrument =  (int)Define.Instrument.Drum;
        Managers.UI.ClosePopupUI(this);
        Managers.UI.SceneUI.GetComponent<UI_PersistentController>().ShowStartBtn();
        OnInstrumentSelected?.Invoke();
    }


  
}
