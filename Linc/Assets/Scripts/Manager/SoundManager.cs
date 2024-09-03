using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;


/// <summary>
/// 사운드 재생과 사운드관련 파라미터를 관리합니다.
/// 설정창의 UI가 참조합니다. 
/// </summary>
public class SoundManager : MonoBehaviour
{
    public enum Sound
    {
        Main,
        Narration,
        Effect,
        Bgm,
        Max
    }

    
    private bool[] _isMute = new bool[(int)Sound.Max];
    private float[] _volumes = new float[(int)Sound.Max];
    public float[] VOLUME_MAX = new float[(int)Sound.Max];
    
    private readonly float VOLUME_MAX_MAIN = 1f;
    private readonly float VOLUME_MAX_BGM =0.5f;
    private readonly float VOLUME_MAX_EFFECT = 1f;
    private readonly float VOLUME_MAX_NARRATION = 1f;
    
    
      
    public static readonly float VOLUME_INITVALUE_MAIN      = 0.5f;
    public static readonly float VOLUME_INITVALUE_BGM       = 0.3f;
    public static readonly float VOLUME_INITVALUE_EFFECT    = 0.5f;
    public static readonly float VOLUME_INITVALUE_NARRATION = 0.5f;
    
    
    

    public float[] volumes
    {
        get => _volumes;
        set
        {
            if (_volumes == null || _volumes.Length != value.Length)
            {
                _volumes = new float[value.Length];
            }
            
        }
    }


    [FormerlySerializedAs("_audioSources")]
    public AudioSource[] audioSources;
    private  Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    private GameObject _soundRoot;
    public void Init()
    {
        volumes = new float[(int)Sound.Max];
        
        if (_soundRoot == null)
        {
            _soundRoot = GameObject.Find("@SoundRoot");
            if (_soundRoot == null)
            {
                audioSources = new AudioSource[(int)Sound.Max];
                VOLUME_MAX = new float[(int)Sound.Max];
                _soundRoot = new GameObject { name = "@SoundRoot" };
                DontDestroyOnLoad(_soundRoot);

                var soundTypeNames = Enum.GetNames(typeof(Sound));
                for (var count = 0; count < soundTypeNames.Length - 1; count++)
                {
                    var go = new GameObject { name = soundTypeNames[count] };
                    audioSources[count] = go.AddComponent<AudioSource>();
                    go.transform.parent = _soundRoot.transform;
                }

                audioSources[(int)Sound.Bgm].loop = true;

                volumes = new float[(int)Sound.Max];
                
                Debug.Assert(Managers.Data!=null);
                
                for (int i = 0; i < (int)Sound.Max; i++)
                {
                    volumes[(int)Sound.Main] =     Managers.Data.Preference[(int)Define.Preferences.MainVol];
                    volumes[(int)Sound.Narration] =Managers.Data.Preference[(int)Define.Preferences.NarrationVol];
                    volumes[(int)Sound.Effect] =   Managers.Data.Preference[(int)Define.Preferences.EffectVol];
                    volumes[(int)Sound.Bgm] =      Managers.Data.Preference[(int)Define.Preferences.BgmVol];
                }
                for (int i = 0; i < (int)Sound.Max; i++)
                {
                    VOLUME_MAX[(int)Sound.Main] = VOLUME_MAX_MAIN;
                    VOLUME_MAX[(int)Sound.Bgm] = VOLUME_MAX_BGM;
                    VOLUME_MAX[(int)Sound.Effect] = VOLUME_MAX_EFFECT;
                    VOLUME_MAX[(int)Sound.Narration] = VOLUME_MAX_NARRATION;
                }
            }
            
        }
        
    }

    public void Clear()
    {
        foreach (var audioSource in audioSources)
            audioSource.Stop();
        _audioClips.Clear();
    }

    public void SetPitch(Sound type, float pitch = 1.0f)
    {
        var audioSource = audioSources[(int)type];
        if (audioSource == null)
            return;

        audioSource.pitch = pitch;
    }

    public bool Play(Sound type, string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        var audioSource = audioSources[(int)type];
        if (path.Contains("Audio/") == false)
            path = string.Format("Audio/{0}", path);

        if(audioSource==null) Debug.LogError("audiosource null exception");
    
        if (type == Sound.Bgm)
        {
            if (_isMute[(int)Sound.Main] ||_isMute[(int)Sound.Bgm])
            {
#if UNITY_EDITOR
                Debug.Log("Currently Bgm is on Mute");
#endif
                return false;
            }
            
            var audioClip = Resources.Load<AudioClip>(path);
            if (audioClip == null)
                return false;

            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.volume = volumes[(int)Sound.Bgm];
            audioSource.clip = audioClip;
           
            audioSource.Play();
            return true;
        }

        if (type == Sound.Effect)
        {
            if (_isMute[(int)Sound.Main] ||_isMute[(int)Sound.Effect])
            {
#if UNITY_EDITOR
                Debug.Log("Currently Bgm is on Mute");
#endif
                return false;
            }
            var audioClip = GetAudioClip(path);
            if (audioClip == null)
                return false;

            audioSource.volume = volumes[(int)Sound.Effect];
            
            audioSource.PlayOneShot(audioClip);
            return true;
        }

        if (type == Sound.Narration)
        {
            if (_isMute[(int)Sound.Main] ||_isMute[(int)Sound.Narration])
            {
#if UNITY_EDITOR
                Debug.Log("Currently Bgm is on Mute");
#endif
                return false;
            }
            var audioClip = GetAudioClip(path);
            if (audioClip == null)
            {
#if UNITY_EDITOR
                Debug.Log($"narration clip is null{path}");
#endif
                return false;
                
            }
            

            audioSource.volume = volumes[(int)Sound.Narration];
            audioSource.clip = audioClip;
          
            audioSource.PlayOneShot(audioClip);
            return true;
        }

        return false;
    }
    
    
    public bool Play(Sound type, string path, float volume = 1.0f, float pitch = 1.0f)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        var audioSource = audioSources[(int)type];
        if (path.Contains("Audio/") == false)
            path = string.Format("Audio/{0}", path);

        if(audioSource==null) Debug.LogError("audiosource null exception");


      
        
        if (type == Sound.Bgm)
        {
            var audioClip = Resources.Load<AudioClip>(path);
            if (audioClip == null)
                return false;

            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.volume = volume * volumes[(int)Sound.Bgm];
            audioSource.clip = audioClip;
            audioSource.pitch = pitch;
            audioSource.Play();
            return true;
        }

        if (type == Sound.Effect)
        {
            var audioClip = GetAudioClip(path);
            if (audioClip == null)
                return false;

            audioSource.volume = volume * volumes[(int)Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
            return true;
        }

        if (type == Sound.Narration)
        {
            audioSource.volume = volume * volumes[(int)Sound.Narration];
            var audioClip = GetAudioClip(path);
            if (audioClip == null)
            {
#if UNITY_EDITOR
                Debug.Log($"narration clip is null{path}");
#endif
                return false;
                
            }
            

        
            audioSource.clip = audioClip;
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
            return true;
        }

        return false;
    }

    public void Stop(Sound type)
    {
        var audioSource = audioSources[(int)type];
        audioSource.Stop();
    }
    
    public void Pause(Sound type)
    {
        var audioSource = audioSources[(int)type];
        audioSource.Pause();
    }

    public float GetAudioClipLength(string path)
    {
        var audioClip = GetAudioClip(path);
        if (audioClip == null)
            return 0.0f;
        return audioClip.length;
    }

    private AudioClip GetAudioClip(string path)
    {
        AudioClip audioClip = null;
        if (_audioClips.TryGetValue(path, out audioClip))
            return audioClip;

        audioClip = Resources.Load<AudioClip>(path);
        _audioClips.Add(path, audioClip);
        return audioClip;
    }


    public void SetMute(Sound sound, bool isMute = true)
    {
        var kindOfSoundMute = (Define.Preferences)(-1);

        if (isMute)
        {
            Pause(sound);
        }

        else
        {
            if (sound == Sound.Main) audioSources[(int)sound].Play();

            if (sound == Sound.Bgm) audioSources[(int)sound].Play();
            if (sound == Sound.Effect) Play(Sound.Effect, "Audio/Test_Effect");
            if (sound == Sound.Narration) Play(Sound.Narration, "Audio/Test_Narration");
        }

        if (sound == Sound.Main) kindOfSoundMute = Define.Preferences.Mute_Main;

        if (sound == Sound.Bgm) kindOfSoundMute = Define.Preferences.Mute_Bgm;
        if (sound == Sound.Effect) kindOfSoundMute = Define.Preferences.Mute_Bgm;
        if (sound == Sound.Narration) kindOfSoundMute = Define.Preferences.Mute_Bgm;

        _isMute[(int)sound] = isMute;


        if (isMute)
            Managers.Data.Preference[(int)kindOfSoundMute] = 0;
        else
            Managers.Data.Preference[(int)kindOfSoundMute] = 1;


#if UNITY_EDITOR
        Debug.Log($"{sound} is muted");
#endif
    }
}