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
        NetObj_Mockup_Drum,
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

        GetObject((int)Objs.Drum).SetActive(false);
        GetObject((int)Objs.NetObj_Mockup_Drum).SetActive(false);
        GetObject((int)Objs.Handbell).SetActive(false);
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

        if (Managers.DeviceManager.IsConnected)
        {
            Logger.Log($"HapticStick_On : isConnectd{Managers.DeviceManager.IsConnected}");
            Managers.DeviceManager.SetHapticStick(true,false);
        }
        else
        {
            Logger.Log($"hapticStick In Not connected..... isConnected{Managers.DeviceManager.IsConnected})");
        }
        
        
        var hostInstrument = 0;
        if (Managers.ContentInfo.PlayData.HostInstrument == (int)Define.Instrument.Drum)
        {
            if (Managers.Network.Server.IsHost)
            {
                hostInstrument = (int)Define.Instrument.Drum;
                GetObject((int)Objs.Drum).SetActive(true);
                GetObject((int)Objs.NetObj_Mockup_Handbell).SetActive(true);
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

        var Drum = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(x =>
        {
            Logger.Log($"Client: {x.gameObject.name} -> NetID {x.NetId}");
            // x가 UI_WaitForHost에 해당하는 NetworkIdentity와 같은지 비교
            return x.gameObject.name == "Drum";
        });

        var Handbell = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(x =>
        {
            Logger.Log($"Client: {x.gameObject.name} -> NetID {x.NetId}");
            // x가 UI_WaitForHost에 해당하는 NetworkIdentity와 같은지 비교
            return x.gameObject.name == "Handbell";
        });
        var Handbell_Left = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(x =>
        {
            Logger.Log($"Client: {x.gameObject.name} -> NetID {x.NetId}");
            // x가 UI_WaitForHost에 해당하는 NetworkIdentity와 같은지 비교
            return x.gameObject.name == "Handbell_Left";
        });
        
        var Handbell_Right = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(x =>
        {
            Logger.Log($"Client: {x.gameObject.name} -> NetID {x.NetId}");
            // x가 UI_WaitForHost에 해당하는 NetworkIdentity와 같은지 비교
            return x.gameObject.name == "Handbell_Right";
        });


        var NetObj_Mockup_Drum = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(x =>
        {
            Logger.Log($"Client: {x.gameObject.name} -> NetID {x.NetId}");
            // x가 UI_WaitForHost에 해당하는 NetworkIdentity와 같은지 비교
            return x.gameObject.name == "NetObj_Mockup_Drum";
        });

        var NetObj_Mockup_Handbell = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(x =>
        {
            Logger.Log($"Client: {x.gameObject.name} -> NetID {x.NetId}");
            // x가 UI_WaitForHost에 해당하는 NetworkIdentity와 같은지 비교
            return x.gameObject.name == "NetObj_Mockup_Handbell";
        });

        var NetObj_Mockup_Handbell_Left = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(x =>
        {
            Logger.Log($"Client: {x.gameObject.name} -> NetID {x.NetId}");
            // x가 UI_WaitForHost에 해당하는 NetworkIdentity와 같은지 비교
            return x.gameObject.name == "NetObj_Mockup_Handbell_Left";
        });

        var NetObj_Mockup_Handbell_Right = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(x =>
        {
            Logger.Log($"Client: {x.gameObject.name} -> NetID {x.NetId}");
            // x가 UI_WaitForHost에 해당하는 NetworkIdentity와 같은지 비교
            return x.gameObject.name == "NetObj_Mockup_Handbell_Right";
        });

        if (hostInstrument == (int)Define.Instrument.Drum)
        {
            Handbell.gameObject.SetActive(true);
            Handbell_Left.gameObject.SetActive(true);
            Handbell_Right.gameObject.SetActive(true);
            NetObj_Mockup_Drum.gameObject.SetActive(true);
            
            
            Drum.gameObject.SetActive(false);
            NetObj_Mockup_Handbell.gameObject.SetActive(false);
            NetObj_Mockup_Handbell_Left.gameObject.SetActive(false);
            NetObj_Mockup_Handbell_Right.gameObject.SetActive(false);
            
            
            Logger.Log("HandBell On(상대방 드럼) -----------RPC");
        }
        else if (hostInstrument == (int)Define.Instrument.HandBell)
        {
            Logger.Log("Drum On (상대방 핸드벨) -----------RPC");
            Drum.gameObject.SetActive(true);
            NetObj_Mockup_Handbell.gameObject.SetActive(true);
            NetObj_Mockup_Handbell_Left.gameObject.SetActive(true);
            NetObj_Mockup_Handbell_Right.gameObject.SetActive(true);
            
            Handbell.gameObject.SetActive(false);
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