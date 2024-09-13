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

    
// 타이밍 딕셔너리
    private Dictionary<int, float> _drumPlayEventMap_indexToTiming = new Dictionary<int, float>()
    {
        {1,  8.7f},  //'길'쭉길쭉 당근을 뽑아봐요
        {2,  12.4f}, //'쏙' 
        {3,  13.1f}, //'세'모세모 당근을 뽑아봐요
        {4,  16.8f},  //'쏙'
        {5,  19.52f},//익힌 당근을 자르면 '쓰윽쓰윽'
        {6,  24.02f}, // 한입 베어물면 '물'컹 '물'컹 '무'울컹
        {7,  28.3f},  //딱딱한 당근을 자르면 '통통통통'
        {8,  32.7f},  // 한입 베이물면 '아'삭'아'삭' '아'사삭
        {9,  36.0f},  //아삭아삭 '당'근도 
        {10, 37.75f},  // 물컹물컹 '당'근도 
        {11, 40.0f},  // 모두 '정'말 '정'말
        {12, 43.0f},  //(맛이 있어요) '예'
        {13, 46.3f},  // 나는 당근이 정말 '좋''아''요'
        {14, 51.2f},  //나는 당근이 정말 '좋아요'
        {15, 0.0f}
    };

// 두드림 횟수 딕셔너리
    private Dictionary<int, int> _drumPlayEventDictionary_IndexToCount = new Dictionary<int, int>()
    {
        {1,  1},//'길'쭉길쭉 당근을 뽑아봐요
        {2,  1},//'쏙' 
        {3,  1},//'세'모세모 당근을 뽑아봐요
        {4,  1}, //'쏙'
        {5,  4},//익힌 당근을 자르면 '쓰윽쓰윽'
        {6,  3},// 한입 베어물면 '물'컹 '물'컹 '무'울컹
        {7,  4}, //딱딱한 당근을 자르면 '통통통통'
        {8,  3}, // 한입 베이물면 '아'삭'아'삭' '아'사삭
        {9,  1}, //아삭아삭 '당'근도 
        {10, 1}, // 물컹물컹 '당'근도 
        {11, 2}, // 모두 '정'말 '정'말
        {12, 1}, //(맛이 있어요) '예'
        {13, 3}, // 나는 당근이 정말 '좋''아''요'
        {14, 3}, //나는 당근이 정말 '좋아요'
        {15, 0}
    };

// 간격 딕셔너리
    private Dictionary<int, float> _drumPlayEventDictionary_IndexToInterval = new Dictionary<int, float>()
    {
        {1,  0.0f},//'길'쭉길쭉 당근을 뽑아봐요
        {2,  0f},//'쏙' 
        {3,  0f},//'세'모세모 당근을 뽑아봐요
        {4,  0f}, //'쏙'
        {5,  0.55f},//익힌 당근을 자르면 '쓰윽쓰윽'
        {6,  0.55f},// 한입 베어물면 '물'컹 '물'컹 '무'울컹
        {7,  0.55f}, //딱딱한 당근을 자르면 '통통통통'
        {8,  0.55f}, // 한입 베이물면 '아'삭'아'삭' '아'사삭
        {9,  0.0f}, //아삭아삭 '당'근도 
        {10, 0.0f}, // 물컹물컹 '당'근도 
        {11, 0.55f}, // 모두 '정'말 '정'말
        {12, 0.0f}, //(맛이 있어요) '예'
        {13, 0.245f}, // 나는 당근이 정말 '좋''아''요'
        {14, 0.245f}, //나는 당근이 정말 '좋아요'
        {15, 0.0f}
    };

    private int _currentIndex;
    private float _currentTime;
    private bool _isPlayBtnClicked;
    private float _playInterval = 0.5f;
    private int _indexMax;
    private bool _isMusicOver;
    private WaitForSeconds _waitForInterval;
    private bool _isDrumPlaying; // 중복실행 방지

    private void OnPlayMusic(bool isReplay)
    {
        if (_isPlayBtnClicked && !isReplay) return;
        
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
       
        _waitForInterval = new WaitForSeconds(_drumPlayEventDictionary_IndexToInterval[currentIndex]);
        

        for (var i = 0; i < _drumPlayEventDictionary_IndexToCount[currentIndex]; i++)
        {
            var randomChar = (char)Random.Range('A', 'B' + 1);
            Managers.Sound.Play(SoundManager.Sound.Effect, "Audio/Effect/Drum" + randomChar);
            {
                if (i % 2 == 0)
                    OnLeftDrumClicked?.Invoke();
                else
                    OnRightDrumClicked?.Invoke();
                yield return _waitForInterval;
            }
           
        }

        if (_currentIndex >= _indexMax)
        {
            Debug.Log("drun Auto Event With music is over--------");
            _isMusicOver = true;
            _isPlayBtnClicked = false;
        }
        else
        {
         _currentIndex++;
        }

        _isDrumPlaying = false;


    }

 
    private void Update()
    {
        if (_isPlayBtnClicked
            && Managers.ContentInfo.PlayData.HostInstrument == (int)Define.Instrument.Drum
            && Managers.ContentInfo.PlayData.CurrentPlayMode == (int)Define.PlayMode.RhythmGame)
        {
            _currentTime += Time.deltaTime;
            Debug.Assert(_drumPlayEventMap_indexToTiming.ContainsKey(_currentIndex));

         
            if (_currentTime >= _drumPlayEventMap_indexToTiming[_currentIndex])
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
        if (!PreCheckOnRaySync())
        {
            return;
        }
   
        
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
