using UnityEngine;

public class UI_Main : UI_Popup
{
    private enum Btns
    {
        Btn_Start
    }

    private UI_Popup main;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        main = this;


        BindButton(typeof(Btns));

        GetButton((int)Btns.Btn_Start).gameObject.BindEvent(OnMainStartBtnClicked);
        Debug.Log("Main UI Init");
        return true;
    }

    private void OnMainStartBtnClicked()
    {
#if UNITY_EDITOR
        Debug.Log("Main Content Start");
#endif
        Managers.UI.ClosePopupUI(main);
        Managers.UI.ShowPopupUI<UI_DepthSelection>();
    }
}