using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    
    private Dictionary<int, float> _drumPlayEventMap_indexToTiming = new Dictionary<int, float>()
    {
        {1, 12.5f},
        {2, 20.3f},
        {3, 24.0f},
        {4, 28.5f},
        {5, 34.0f},
        {6, 36.0f},
        {7, 37.0f},
        {8, 43.0f},
        {9, 46.0f},
        {10, 51.0f}
    };
    private Dictionary<int, int> _drumPlayEventDictionary_IndexToCount = new Dictionary<int, int>()
    {
        {1, 1},
        {2, 4},
        {3, 3},
        {4, 3},
        {5, 4},
        {6, 3},
        {7, 2},
        {8, 3},
        {9, 3},
        {10, 3}
    };
    private Dictionary<int, float> _drumPlayEventDictionary_IndexToInterval = new Dictionary<int, float>()
    {
        {1, 0},
        {2, 0.5f},
        {3, 0.5f},
        {4, 4},
        {5, 3},
        {6, 1},
        {7, 2},
        {8, 1},
        {9, 3},
        {10, 3}
    };
    
    private int _currentIndex;
    private float _currentTime;
    private bool _isPlayBtnClicked;
    private float _playInterval = 0.25f;
    private int _indexMax;
    private bool _isMusicOver;
    private WaitForSeconds _waitForInterval;
    private bool _isDrumPlaying; // 중복실행 방지

    private void OnPlayMusic()
    {
        _isPlayBtnClicked = true;
        _isMusicOver = false;
        _currentIndex = 1;
        _currentTime = 0;
    }
    protected override void Init()
    {
        base.Init();
    
        ManageProjectSettings(150,0.08f);
      
        _indexMax = _drumPlayEventDictionary_IndexToCount.Count;
        _set = new GameObject[2];
        _set[(int)Define.Instrument.Drum]
            = transform.GetChild((int)Define.Instrument.Drum).gameObject;
        _set[(int)Define.Instrument.HandBell]
            = transform.GetChild((int)Define.Instrument.HandBell).gameObject;

        // foreach (var obj in _set)
        // {
        //     obj.SetActive(false);
        // }
    }

    

    protected override void BindEvent()
    {
        base.BindEvent();
        UI_Maincontroller_SinglePlay.PlayMusicEvent -= OnPlayMusic;
        UI_Maincontroller_SinglePlay.PlayMusicEvent += OnPlayMusic;
        UI_MainController_NetworkInvolved.PlayMusicEvent -= OnPlayMusic;
        UI_MainController_NetworkInvolved.PlayMusicEvent += OnPlayMusic;
        
        UI_InstrumentSelection.OnInstrumentSelected -= OnGameStart;
        UI_InstrumentSelection.OnInstrumentSelected += OnGameStart;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UI_Maincontroller_SinglePlay.PlayMusicEvent -= OnPlayMusic;
        UI_MainController_NetworkInvolved.PlayMusicEvent -= OnPlayMusic;
        UI_InstrumentSelection.OnInstrumentSelected -= OnGameStart;
    }


    IEnumerator PlayDrumWithMusic(int currentIndex)
    {
        if (_waitForInterval == null)
        {
            _waitForInterval = new WaitForSeconds(_playInterval);
        }

        for (var i = 0; i < _drumPlayEventDictionary_IndexToCount[currentIndex]; i++)
        {
            var randomChar = (char)Random.Range('A', 'B' + 1);
            Managers.Sound.Play(SoundManager.Sound.Effect, "Audio/Effect/Drum" + randomChar);
            if (i % 2 == 0)
                OnLeftDrumClicked?.Invoke();
            else
                OnRightDrumClicked?.Invoke();
            yield return _waitForInterval;
        }

        if (_currentIndex >= _indexMax)
        {
            Debug.Log("drun Auto Event With music is over--------");
            _isMusicOver = true;
        }
        else
        {
         _currentIndex++;
        }

        _isDrumPlaying = false;


    }

 
    private void Update()
    {
        if (_isPlayBtnClicked)
        {
            Debug.Assert(_drumPlayEventMap_indexToTiming.ContainsKey(_currentIndex));

            _currentTime += Time.deltaTime;
            if (_currentTime >= _drumPlayEventMap_indexToTiming[_currentIndex] && Managers.ContentInfo.PlayData.HostInstrument == (int)Define.Instrument.Drum)
            {
                if (_isDrumPlaying || _isMusicOver) return;
                _isDrumPlaying = true;
                StartCoroutine(PlayDrumWithMusic(_currentIndex));
            }
        }
    }

    public override void OnRaySynced()
    {
        Logger.Log("click");
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
        if (Managers.Network == null) return;


        if (Managers.Network.Server.IsHost && Managers.Network .Server.isActiveAndEnabled)
        {
            if (Managers.ContentInfo.PlayData.HostInstrument == (int)Define.Instrument.Drum)
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
        else
        {
            if (Managers.ContentInfo.PlayData.ClientInstrument == (int)Define.Instrument.Drum)
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


        if (Managers.ContentInfo.PlayData.HostInstrument == (int)Define.Instrument.Drum)
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
