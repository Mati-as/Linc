using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayModeSelection : UI_Popup
{
    public enum Btns
    {
        Btn_MultiPlay,
        Btn_SinglePlay
    }

    public override bool Init()
    {
        BindObject(typeof(Btns));
        GetButton((int)Btns.Btn_SinglePlay).gameObject.BindEvent(OnSinglePlayBtnClicked);
        GetButton((int)Btns.Btn_MultiPlay).gameObject.BindEvent(OnMultiPlayBtnClicked);
        return base.Init();
    }


    public void OnSinglePlayBtnClicked()
    {
        
    }

    public void OnMultiPlayBtnClicked()
    {
        
    }
}
