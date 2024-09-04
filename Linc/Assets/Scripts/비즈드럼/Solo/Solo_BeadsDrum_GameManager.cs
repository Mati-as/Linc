using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Mirage;
using UnityEngine;
using Random = UnityEngine.Random;

public class Solo_BeadsDrum_GameManager : Base_NetworkGameManager
{
    
    private GameObject[] _set;
    
    
    [SerializeField]
    [Range(0,10)]
    public float clickRadius;

    public static event Action OnLeftDrumClicked;
    public static event Action OnRightDrumClicked;

    protected override void Init()
    {
        base.Init();
        ManageProjectSettings(150,0.08f);
        _set = new GameObject[2];
        _set[(int)Define.Instrument.Drum]
            = transform.GetChild((int)Define.Instrument.Drum).gameObject;
        _set[(int)Define.Instrument.HandBell]
            = transform.GetChild((int)Define.Instrument.HandBell).gameObject;

        foreach (var obj in _set)
        {
            obj.SetActive(false);
        }
    }

    protected override void BindEvent()
    {
        base.BindEvent();
        UI_InstrumentSelection.OnInstrumentSelected -= OnGameStart;
        UI_InstrumentSelection.OnInstrumentSelected += OnGameStart;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UI_InstrumentSelection.OnInstrumentSelected -= OnGameStart;
    }



    public override void OnRaySynced()
    {
        
        if (!PreCheckOnRaySync()) return;
   
        
        foreach (var hit in GameManager_Hits)
        {
            if(hit.transform.gameObject.name.Contains("Wall")) continue; // WallCollider는 비즈랑만 상호작용
            Collider[] hitColliders = Physics.OverlapSphere(hit.point, clickRadius);
            foreach (var hitCollider in hitColliders)
            {
                IBeadOnClicked clickable = hitCollider.GetComponent<IBeadOnClicked>();
                if (clickable != null)
                {
                    clickable.OnClicked();
                }
            }
        }
            
        if (GameManager_Hits[0].transform.gameObject.name.Contains("Background"))
        {
            var randomChar = (char)Random.Range('A', 'D' + 1);
            Managers.Sound.Play(SoundManager.Sound.Effect, "Audio/Effect/Bubble"+randomChar,0.25f);
                  
        }
    
        foreach (var hit in GameManager_Hits)
        {
            if (hit.transform.gameObject.name.Contains("DrumLeft"))
            {
                var randomChar = (char)Random.Range('A', 'B' + 1);
                Managers.Sound.Play(SoundManager.Sound.Effect, "Audio/Effect/Drum" + randomChar);
                OnLeftDrumClicked?.Invoke();
                return;
            }

            if (hit.transform.gameObject.name.Contains("DrumRight"))
            {
                var randomChar = (char)Random.Range('A', 'B' + 1);
                Managers.Sound.Play(SoundManager.Sound.Effect, "Audio/Effect/Drum" + randomChar);
                OnRightDrumClicked?.Invoke();
                return;
            }

            if (hit.transform.gameObject.name.Contains("Frame"))
            {
                var randomChar = (char)Random.Range('A', 'C' + 1);
                Managers.Sound.Play(SoundManager.Sound.Effect, "Audio/Effect/Plastic" + randomChar);
                return;
            }
        }

    }
    
    private void OnGameStart()
    {
        if (Managers.ContentInfo.PlayData.CurrentInstrument == (int)Define.Instrument.Drum)
        {
            _set[(int)Define.Instrument.Drum].SetActive(true);
            _set[(int)Define.Instrument.HandBell].SetActive(false);
        }
        else
        {
            _set[(int)Define.Instrument.HandBell].SetActive(true);
            _set[(int)Define.Instrument.Drum].SetActive(false);
        }
    }
    

  
}
