using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Lobby : UI_Popup
{
    public enum Btns
    {
        Btn_Start,
        Btn_Back
    }
    public enum UIs
    {
        MyPlayer,
        Opponent,
        TryingConnection
    }
    public override bool Init()
    {
        if (base.Init() == false) return false;
        

        BindObject(typeof(UIs));
        BindButton(typeof(Btns));
  
        return true;
    }

    private void OnConnected()
    {
        _isGamePlayable = true;
    }

    private void OnWaitForConnection()
    {
        
    }


    private bool _isGamePlayable;
    private void OnStart()
    {
     
    }

}
