using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Evaluation : UI_Popup
{
    private enum Btns
    {
        Btn_Close
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;


        // BindObject(typeof(GameObj));
        BindButton(typeof(Btns));

        GetButton((int)Btns.Btn_Close).gameObject.BindEvent(() => { Managers.UI.ClosePopupUI(this); });

        return true;
    }
}
