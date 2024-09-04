using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Lobby : UI_Popup
{
    public enum Btns
    {
        Btn_Start,
        Btn_Back,
        Btn_StartHostBtn,
        Btn_StartClientBtn
    }
    public enum UIs
    {
        MyPlayer,
        Opponent,
        TryingConnection
    }
    
    
    private bool _isGamePlayable;
    private TextMeshProUGUI _tmp;
    public override bool Init()
    {
        if (base.Init() == false) return false;
        

        BindObject(typeof(UIs));
        BindButton(typeof(Btns));
  
        GetObject((int)UIs.Opponent).SetActive(false);
        _tmp = GetObject((int)UIs.TryingConnection).GetComponent<TextMeshProUGUI>();
        GetObject((int)UIs.TryingConnection).SetActive(false);
        
        GetButton((int)Btns.Btn_StartHostBtn).gameObject.BindEvent(OnHostBtnClicked);
        GetButton((int)Btns.Btn_StartHostBtn).gameObject.BindEvent(OnClientBtnClicked);
        return true;
    }

    private void OnHostBtnClicked()
    {
        _tmp.text = "방 만들기 완료, 다른 플레이어 입장 대기 중...";
        GetObject((int)UIs.TryingConnection).SetActive(true);
    }

    private void OnClientBtnClicked()
    {
        _tmp.text = "방 참여 시도 중...";
        GetObject((int)UIs.TryingConnection).SetActive(true);
    }
    private void OnConnected()
    {
        GetObject((int)UIs.Opponent).SetActive(true);
        _isGamePlayable = true;
    }

    private void OnConnectFailed()
    {
        _tmp.text = "방 찾기를 실패했어요. 다시 방을 만들거나 방 참여를 시도 해주세요.";
        GetObject((int)UIs.Opponent).SetActive(false);
    }
    private void OnWaitForConnection()
    {
        
    }

    


}
