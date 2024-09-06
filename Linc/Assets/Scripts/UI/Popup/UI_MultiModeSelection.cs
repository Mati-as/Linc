using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MultiModeSelection : UI_Popup
{

    public enum Btns
    {
        Btn_MultiPlay,
        Btn_SinglePlay,
    }

    public override bool Init()
    {
        if (base.Init() == false) return false;
        
        BindButton(typeof(Btns));
        GetButton((int)Btns.Btn_SinglePlay).gameObject.BindEvent(OnSinglePlayBtnClicked);
        GetButton((int)Btns.Btn_MultiPlay).gameObject.BindEvent(OnMultipPlayBtnClicked);
        return base.Init();
    }

    private void OnSinglePlayBtnClicked()
    {
        Managers.UI.ClosePopupUI(this);
        Managers.UI.ShowPopupUI<UI_PlayModeSelection>();
    }

    private void OnMultipPlayBtnClicked()
    {
        Managers.UI.ClosePopupUI(this);
        Managers.UI.SceneUI.GetComponent<UI_MainController_NetworkInvolved>().UI_Lobby.SetActive(true);

        // Managers.Scene.ChangeScene(Define.Scene.linc_multimode);
    }
}
