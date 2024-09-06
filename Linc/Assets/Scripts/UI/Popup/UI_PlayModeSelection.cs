using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayModeSelection : UI_Popup
{
    public enum Btns
    {
        Btn_FreePlayMode,
        Btn_RhythmGameMode
    }

    public override bool Init()
    {
        BindButton(typeof(Btns));
        GetButton((int)Btns.Btn_RhythmGameMode).gameObject.BindEvent(OnRythmGameModeBtnClicked);
        GetButton((int)Btns.Btn_FreePlayMode).gameObject.BindEvent(Btn_FreePlayMode);
        return base.Init();
    }


    public void OnRythmGameModeBtnClicked()
    {
        Managers.ContentInfo.PlayData.CurrentPlayMode = (int)Define.PlayMode.RhythmGame;
        Managers.UI.ClosePopupUI(this);
        Managers.UI.ShowPopupUI<UI_InstrumentSelection>();
    }

    public void Btn_FreePlayMode()
    {
        Managers.ContentInfo.PlayData.CurrentPlayMode = (int)Define.PlayMode.Free;
        Managers.UI.ClosePopupUI(this);
        Managers.UI.ShowPopupUI<UI_InstrumentSelection>();
    }
}
