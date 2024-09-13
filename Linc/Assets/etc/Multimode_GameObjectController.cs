using System;
using System.Collections.Generic;
using System.Linq;
using Mirage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class Multimode_GameObjectController : NetworkBehaviour
{
    public enum Objs
    {
        Drum,
        BeadsDrumLeft,
        BeadsDrumRight,
        Drumsticks,
        NetObj_Drumsticks,
        NetObj_Mockup_Drum,
        Xylophone,
        Netobj_Xylophone,
        Handbell,
        Handbell_Left,
        Handbell_Right,
        NetObj_Mockup_Handbell,
        NetObj_Mockup_Handbell_Left,
        NetObj_Mockup_Handbell_Right
    }


    public bool Init()
    {
        if (_init) return false;

        BindObject(typeof(Objs));

        GetObject((int)Objs.Drum).SetActive(true);
        GetObject((int)Objs.NetObj_Mockup_Drum).SetActive(false);
        
        GetObject((int)Objs.Drumsticks).SetActive(true);
        GetObject((int)Objs.NetObj_Drumsticks).SetActive(true);
        
        GetObject((int)Objs.Xylophone).SetActive(false);
        GetObject((int)Objs.Netobj_Xylophone).SetActive(false);
        
        GetObject((int)Objs.BeadsDrumLeft).SetActive(false);
        GetObject((int)Objs.BeadsDrumRight).SetActive(false);
        
        GetObject((int)Objs.Handbell).SetActive(true);
        GetObject((int)Objs.Handbell_Left).SetActive(false);
        GetObject((int)Objs.Handbell_Right).SetActive(false);
        
        GetObject((int)Objs.NetObj_Mockup_Handbell).SetActive(false);
        GetObject((int)Objs.NetObj_Mockup_Handbell_Left).SetActive(false);
        GetObject((int)Objs.NetObj_Mockup_Handbell_Right).SetActive(false);

        UI_MainController_NetworkInvolved.OnStartBtnClickedAction -= IncludeRPC_OnStartHandler;
        UI_MainController_NetworkInvolved.OnStartBtnClickedAction += IncludeRPC_OnStartHandler;

        return _init = true;
    }

    private void OnDestroy()
    {
        UI_MainController_NetworkInvolved.OnStartBtnClickedAction -= IncludeRPC_OnStartHandler;
    }

    private void IncludeRPC_OnStartHandler()
    {
        GetObject((int)Objs.Drum).SetActive(true);
        GetObject((int)Objs.Handbell).SetActive(true);
        if (Managers.DeviceManager.IsConnected)
            Logger.Log($"HapticStick_On : isConnectd{Managers.DeviceManager.IsConnected}");
        else
            Logger.Log($"hapticStick In Not connected..... isConnected{Managers.DeviceManager.IsConnected})");


        var hostInstrument = 0;
        if (Managers.ContentInfo.PlayData.HostInstrument == (int)Define.Instrument.Drum)
        {
            if (Managers.Network.Server.IsHost)
            {
                hostInstrument = (int)Define.Instrument.Drum;
                GetObject((int)Objs.Drum).SetActive(true);
                GetObject((int)Objs.BeadsDrumLeft).SetActive(true);
                GetObject((int)Objs.BeadsDrumRight).SetActive(true);
                GetObject((int)Objs.NetObj_Mockup_Handbell).SetActive(true);
                GetObject((int)Objs.NetObj_Mockup_Handbell_Left).SetActive(true);
                GetObject((int)Objs.NetObj_Mockup_Handbell_Right).SetActive(true);
                Logger.Log("Drum On (상대방 핸드벨) -----------RPC");
            }
        }
        else if (Managers.ContentInfo.PlayData.HostInstrument == (int)Define.Instrument.HandBell)
        {
            if (Managers.Network.Server.IsHost)
            {
                hostInstrument = (int)Define.Instrument.HandBell;
                GetObject((int)Objs.Handbell).SetActive(true);
                GetObject((int)Objs.Handbell_Left).SetActive(true);
                GetObject((int)Objs.Handbell_Right).SetActive(true);
                GetObject((int)Objs.NetObj_Mockup_Drum).SetActive(true);
                Logger.Log("HandBell On(상대방 드럼) -----------RPC");
            }
        }
        else
        {
            Logger.LogError("Wrong Value in HostInstrument Data");
        }


        if (Managers.Network.Server.IsHost)
        {
            Logger.Log("GameStart RPC Invoked-----------------------------------");
            RPC_OnStartBtnClicked(hostInstrument);
        }

        Logger.Log("Instrument RPC sent from the server.");
    }


    [ClientRpc]
    private void RPC_OnStartBtnClicked(int hostInstrument)
    {
        if (!Managers.Network.Server.IsHost)
        {
            Logger.Log(
                $"Get RPC_OnStartBtnClicked From Server ----------------------------HostInstument is {(Define.Instrument)hostInstrument}");
            
            var Drum = FindNetworkIdentityByName("Drum");
            var Drumsticks = FindNetworkIdentityByName("Drumsticks");
            var BeadsDrumLeft = FindNetworkIdentityByName("BeadsDrumLeft");
            var BeadsDrumRight = FindNetworkIdentityByName("BeadsDrumRight");
            var NetObj_Mockup_Drum = FindNetworkIdentityByName("NetObj_Mockup_Drum");
           
            var NetObj_Drumsticks = FindNetworkIdentityByName("NetObj_Drumsticks");
            var Handbell = FindNetworkIdentityByName("Handbell");
            var Handbell_Left = FindNetworkIdentityByName("Handbell_Left");
            var Handbell_Right = FindNetworkIdentityByName("Handbell_Right");
            var NetObj_Mockup_Handbell = FindNetworkIdentityByName("NetObj_Mockup_Handbell");
            var NetObj_Mockup_Handbell_Left = FindNetworkIdentityByName("NetObj_Mockup_Handbell_Left");
            var NetObj_Mockup_Handbell_Right = FindNetworkIdentityByName("NetObj_Mockup_Handbell_Right");
            
          

            if (hostInstrument == (int)Define.Instrument.Drum)
            {
                Drum.gameObject.SetActive(true); // 드럼 부모객체는 켜놓아야 스크립트 활성화 및 동기화 가능.
                
                
                Handbell.gameObject.SetActive(true);
                Handbell_Left.gameObject.SetActive(true);
                Handbell_Right.gameObject.SetActive(true);
                NetObj_Mockup_Drum.gameObject.SetActive(true);

                
                BeadsDrumLeft.gameObject.SetActive(false);
                BeadsDrumRight.gameObject.SetActive(false);
                Drumsticks.gameObject.SetActive(false);
                NetObj_Mockup_Handbell.gameObject.SetActive(false);
                NetObj_Mockup_Handbell_Left.gameObject.SetActive(false);
                NetObj_Mockup_Handbell_Right.gameObject.SetActive(false);


                Logger.Log("HandBell On(상대방 드럼) -----------RPC");
            }
            else if (hostInstrument == (int)Define.Instrument.HandBell)
            {
                Logger.Log("Drum On (상대방 핸드벨) -----------RPC");

                Drum.gameObject.SetActive(true);  // 드럼 부모객체는 켜놓아야 스크립트 활성화 및 동기화 가능.
                BeadsDrumLeft.gameObject.SetActive(true);
                BeadsDrumRight.gameObject.SetActive(true);
                
                NetObj_Mockup_Handbell.gameObject.SetActive(true);
                NetObj_Mockup_Handbell_Left.gameObject.SetActive(true);
                NetObj_Mockup_Handbell_Right.gameObject.SetActive(true);

                
                Handbell.gameObject.SetActive(true);
                NetObj_Drumsticks.gameObject.SetActive(false);
                Handbell_Left.gameObject.SetActive(false);
                Handbell_Right.gameObject.SetActive(false);
                NetObj_Mockup_Drum.gameObject.SetActive(false);
            }
            else
            {
                Logger.LogError("Wrong Value in HostInstrument Data");
            }
        }
    }


    
    
    // 공통 함수: 객체 이름으로 NetworkIdentity를 찾는 함수
    private NetworkIdentity FindNetworkIdentityByName(string objectName)
    {
        var identity = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(x =>
        {
            Logger.Log($"Client: {x.gameObject.name} -> NetID {x.NetId}");
            return x.gameObject.name == objectName;
        });

        return identity;
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    #region BindSection

    protected Dictionary<Type, Object[]> _objects = new();

    protected bool _init;


    private void Start()
    {
        Init();
    }

    protected void Bind<T>(Type type) where T : Object
    {
        var names = Enum.GetNames(type);
        var objects = new Object[names.Length];
        _objects.Add(typeof(T), objects);

#if UNITY_EDITOR
//s		Debug.Log($"object counts to bind {names.Length}");
#endif
        for (var i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Utils.FindChild(gameObject, names[i], true);
            else
                objects[i] = Utils.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.Log($"Failed to bind({names[i]})");
        }
    }


    protected void BindObject(Type type)
    {
        Bind<GameObject>(type);
    }

    protected void BindImage(Type type)
    {
        Bind<Image>(type);
    }

    protected void BindTMP(Type type)
    {
        Bind<TextMeshProUGUI>(type);
    }

    protected void BindText(Type type)
    {
        Bind<Text>(type);
    }

    protected void BindButton(Type type)
    {
        Bind<Button>(type);
    }


    protected void BindToggle(Type type)
    {
        Bind<Toggle>(type);
    }

    protected void BindSlider(Type type)
    {
        Bind<Slider>(type);
    }


    protected T Get<T>(int idx) where T : Object
    {
        Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[idx] as T;
    }

    protected GameObject GetObject(int idx)
    {
        return Get<GameObject>(idx);
    }

    protected Text GetText(int idx)
    {
        return Get<Text>(idx);
    }

    protected TextMeshProUGUI GetTMP(int idx)
    {
        return Get<TextMeshProUGUI>(idx);
    }

    protected Button GetButton(int idx)
    {
        return Get<Button>(idx);
    }

    protected Slider GetSlider(int idx)
    {
        return Get<Slider>(idx);
    }

    protected Toggle GetToggle(int idx)
    {
        return Get<Toggle>(idx);
    }

    protected Image GetImage(int idx)
    {
        return Get<Image>(idx);
    }

    public static void BindEvent(GameObject go, Action action, Define.UIEvent type = Define.UIEvent.Click)
    {
        var evt = Utils.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Pressed:
                evt.OnPressedHandler -= action;
                evt.OnPressedHandler += action;
                break;
            case Define.UIEvent.PointerDown:
                evt.OnPointerDownHandler -= action;
                evt.OnPointerDownHandler += action;
                break;
            case Define.UIEvent.PointerUp:
                evt.OnPointerUpHandler -= action;
                evt.OnPointerUpHandler += action;
                break;
            case Define.UIEvent.PointerEnter:
                evt.OnPointerEnterHander -= action;
                evt.OnPointerEnterHander += action;
                break;
            case Define.UIEvent.PointerExit:
                evt.OnPointerExitHandler -= action;
                evt.OnPointerExitHandler += action;
                break;
        }
    }

    #endregion
}