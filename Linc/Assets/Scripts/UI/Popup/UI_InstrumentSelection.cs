using System;
using Mirage;
using UnityEngine;
using UnityEngine.Serialization;

public class UI_InstrumentSelection : UI_Popup
{
    public enum Btns
    {
        Btn_Drum,
        Btn_HandBell,
    }

    public NetworkManager networkManager;
    public static Action OnInstrumentSelected;
    public static Action OnHostSettingFinished;

    public override bool Init()
    {
        if (!base.Init()) return false;
        if (networkManager == null)
        {
            networkManager = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>();
        }
        
        
        BindButton(typeof(Btns));
        GetButton((int)Btns.Btn_HandBell).gameObject
            .BindEvent(()=>
            {
                OnInstrumentBtnClicked(Define.Instrument.HandBell, Define.Instrument.Drum);
            });
       
        GetButton((int)Btns.Btn_Drum).gameObject
            .BindEvent(()=>
            {
                OnInstrumentBtnClicked(Define.Instrument.HandBell, Define.Instrument.Drum);
            });
        
      

        return true;
    }

    // Host clicks HandBell button
    private void OnInstrumentBtnClicked(Define.Instrument instrumentA, Define.Instrument instrumentB)
    {
        if (!GetComponent<NetworkIdentity>().isActiveAndEnabled) Logger.LogError("identity is not enabled or active");
        // Call the RPC to send the instrument selection to all clients
        if (networkManager.Server.IsHost)
        {
            Managers.ContentInfo.PlayData.HostInstrument = (int)instrumentA;
            Managers.ContentInfo.PlayData.ClientInstrument = (int)instrumentB;

            Logger.Log("Instrument RPC sent from the server.");
        }

        Managers.UI.ClosePopupUI(this);
        Managers.UI.SceneUI.GetComponent<UI_MainController_NetworkInvolved>().ShowStartBtn();
        OnInstrumentSelected?.Invoke();
        OnHostSettingFinished?.Invoke();
    }




  


}