using System;
using System.Linq;
using Mirage;
using UnityEngine;
using UnityEngine.Serialization;

public class UI_InstrumentSelection : UI_Popup
{
    public enum Btns
    {
        Btn_Drum,
        Btn_HandBell,
        Btn_BackToPlayModeSelection
    }

    public static Action OnInstrumentSelected;
    public static Action OnHostSettingFinished;

    public override bool Init()
    {
        if (!base.Init()) return false;

        
        BindButton(typeof(Btns));
        GetButton((int)Btns.Btn_HandBell).gameObject
            .BindEvent(()=>
            {
                OnInstrumentBtnClicked(Define.Instrument.HandBell, Define.Instrument.Drum);
            });
       
        GetButton((int)Btns.Btn_Drum).gameObject
            .BindEvent(()=>
            {
                OnInstrumentBtnClicked(Define.Instrument.Drum, Define.Instrument.HandBell);
            });

        GetButton((int)Btns.Btn_BackToPlayModeSelection).gameObject
            .BindEvent(() =>
            {
               
                if (Managers.Network == null)
                {
                    Managers.UI.ClosePopupUI(this);
                    Managers.UI.ShowPopupUI<UI_PlayModeSelection>();
                    return;
                }
        
                if (Managers.Network.Server.IsHost)
                {
                    Managers.NetworkObjs[(int)Define.NetworkObjs.UI_PlayModeSelection].SetActive(true);
                    gameObject.SetActive(false);
                }
                
            });
        
        if (Managers.Network == null && Managers.ContentInfo.PlayData.isMultiMode == false) return true;
        
        //mutliplay인 경우만
        gameObject.SetActive(false);

        return true;
    }

    // Host clicks HandBell button
    private void OnInstrumentBtnClicked(Define.Instrument instrumentA, Define.Instrument instrumentB)
    {
        
        
        //Managers.UI.ClosePopupUI(this);
        OnInstrumentSelected?.Invoke();
        OnHostSettingFinished?.Invoke();
        
      
        if (Managers.Network == null)
        {
            Managers.ContentInfo.PlayData.HostInstrument = (int)instrumentA;
            Managers.ContentInfo.PlayData.ClientInstrument = (int)instrumentB;
            Managers.UI.ClosePopupUI(this);
            Managers.UI.SceneUI.GetComponent<UI_Maincontroller_SinglePlay>().ShowStartBtn();
            
            return;
        }
        
        
        if (Managers.Network.Server.IsHost)
        {
            Managers.ContentInfo.PlayData.HostInstrument = (int)instrumentA;
            Managers.ContentInfo.PlayData.ClientInstrument = (int)instrumentB;
            
        }



        if (Managers.Network.Server.IsHost)
        {
            ClientRPC_OnInstrumentSelected();
            Logger.Log("Instrument RPC sent from the server.");
        }
        else
        {
          //  Managers.UI.SceneUI.transform.Find("UI_WaitForHost(Clone)").gameObject.SetActive(false);
          //   Managers.UI.SceneUI.transform.Find("UI_Lobby").gameObject.SetActive(false);
        }
     
    
        
        gameObject.SetActive(false);
    }

    [ClientRpc]
    private void ClientRPC_OnInstrumentSelected()
    {
        Logger.Log("Client RPC Get from the server----------------------");
        Logger.Log($"클라이언트 NetworkID Dict Element Count: {Managers.NetworkObjNetworkIds.Count}");
        foreach (var key in Managers.Network.Client.World.SpawnedIdentities.ToList())
        {
         Logger.Log($"Spawned 객체: {key.gameObject.name}, ID: {key.NetId}");
        }

        if (!Managers.Network.Server.IsHost)
        {
            var targetIdentity = Managers.Network.Client.World.SpawnedIdentities.ToList().FindAll(x =>
            {
                Logger.Log($"Client: {x.gameObject.name} -> NetID {x.NetId}");
                // x가 UI_WaitForHost에 해당하는 NetworkIdentity와 같은지 비교
                return x.gameObject.name.Contains("Wait");
            });
        
            if (targetIdentity != null)
            {
                foreach (var target in targetIdentity)
                {
                    target.gameObject.SetActive(false);
                    Logger.Log($"객체 {target.name}를 비활성화했습니다.");
                }
               
            }
            else
            {
                Logger.LogError("해당하는 NetworkIdentity 객체를 찾을 수 없습니다.");
            }
            
            Scene_MultiMode.InGame_MultiMode.GetComponentInChildren<UI_MainController_NetworkInvolved>().UI_Lobby.SetActive(false);
        }
        
       Scene_MultiMode.InGame_MultiMode.GetComponentInChildren<UI_MainController_NetworkInvolved>().ShowStartBtn();
        
        
    }
    




  


}