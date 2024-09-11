using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class Solo_GameObjectController : MonoBehaviour
{
    public enum Objs
    {
        Drum,
        BeadsDrumLeft,
        BeadsDrumRight,
        Drumsticks,
        Xylophone,
        Handbell,
        Handbell_Left,
        Handbell_Right
    }


    public bool Init()
    {
        if (_init) return false;
        
        Debug.Assert(Managers.Network == null);
        
        BindObject(typeof(Objs));

        GetObject((int)Objs.Drum).SetActive(true);

        GetObject((int)Objs.Drumsticks).SetActive(true);

        GetObject((int)Objs.Xylophone).SetActive(false);

        GetObject((int)Objs.BeadsDrumLeft).SetActive(false);
        GetObject((int)Objs.BeadsDrumRight).SetActive(false);

        GetObject((int)Objs.Handbell).SetActive(true);
        GetObject((int)Objs.Handbell_Left).SetActive(false);
        GetObject((int)Objs.Handbell_Right).SetActive(false);


        UI_Maincontroller_SinglePlay.OnStartBtnClickedAction -= OnStartBtnClicked;
        UI_Maincontroller_SinglePlay.OnStartBtnClickedAction += OnStartBtnClicked;

        return _init = true;
    }

    private void OnDestroy()
    {
        UI_Maincontroller_SinglePlay.OnStartBtnClickedAction -= OnStartBtnClicked;
    }

    private void OnStartBtnClicked()
    {

        if (Managers.DeviceManager.IsConnected)
            Logger.Log($"HapticStick_On : isConnectd{Managers.DeviceManager.IsConnected}");
        else
            Logger.Log($"hapticStick In Not connected..... isConnected{Managers.DeviceManager.IsConnected})");
        
        if (Managers.ContentInfo.PlayData.HostInstrument == (int)Define.Instrument.Drum)
        {
            GetObject((int)Objs.Drum).SetActive(true);
            GetObject((int)Objs.BeadsDrumLeft).SetActive(true);
            GetObject((int)Objs.BeadsDrumRight).SetActive(true);

            GetObject((int)Objs.Handbell).SetActive(false);
            GetObject((int)Objs.Handbell_Left).SetActive(false);
            GetObject((int)Objs.Handbell_Right).SetActive(false);
        }
        else if (Managers.ContentInfo.PlayData.HostInstrument == (int)Define.Instrument.HandBell)
        {
            GetObject((int)Objs.Handbell).SetActive(true);
            GetObject((int)Objs.Handbell_Left).SetActive(true);
            GetObject((int)Objs.Handbell_Right).SetActive(true);

            GetObject((int)Objs.Drum).SetActive(false);
            GetObject((int)Objs.BeadsDrumLeft).SetActive(false);
            GetObject((int)Objs.BeadsDrumRight).SetActive(false);
        }
        else
        {
            Logger.LogError("Wrong Value in HostInstrument Data");
        }


        Logger.Log("Instrument RPC sent from the server.");
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