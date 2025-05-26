using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BGMType
{ 
    MainBGM,
    BattleBGM
}

public enum SFXType
{ 
    StartButtonSound,
    GetWisdomSound,
    UpgradeSound,
    UpgradeNegativeSound,
    LostWisdomSound,
    ClearSound,
    DefeatSound,
    UIClickSound,
    CountDownSound,
    PlayerDamagedSound,
    BossSound,
    UpgradeSkillSound,
    PlayerAttackSound,
    PlayerAttackEffectSound,
    CastSound,
    Meteor,
    FireExplosion,
    FireWall,
    LightningAura,
    LightningSlash,
    ElectroSpark,
    SlientWaterSlash,
    HydroFrozen,
    DevilRainFog
    
}

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public Dictionary<BGMType, AudioClip> bgmDic = new Dictionary<BGMType, AudioClip>();
    public Dictionary<SFXType, AudioClip> sfxDic = new Dictionary<SFXType, AudioClip>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitSoundManager()
    {
        GameObject obj = new GameObject("SoundController");
        Instance = obj.AddComponent<SoundController>();
        DontDestroyOnLoad(obj);
        
        GameObject bgmObj = new GameObject("BGM");
        SoundController.Instance.bgmSource = bgmObj.AddComponent<AudioSource>();
        bgmObj.transform.SetParent(obj.transform);
        SoundController.Instance.bgmSource.loop = true;
        SoundController.Instance.bgmSource.volume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        
        GameObject sfxObj = new GameObject("SFX");
        SoundController.Instance.sfxSource = sfxObj.AddComponent<AudioSource>();
        SoundController.Instance.sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        sfxObj.transform.SetParent(obj.transform);
        
        AudioClip[] bgmClips = Resources.LoadAll<AudioClip>("Sound/BGM");
        
        foreach (var clip in bgmClips)
        {
            try
            {
                BGMType type = (BGMType)Enum.Parse(typeof(BGMType), clip.name);
                SoundController.Instance.bgmDic.Add(type, clip);
            }
            catch
            {
                Debug.LogWarning("BGM enum 필요" + clip.name);
            }

        }

        AudioClip[] sfxClips = Resources.LoadAll<AudioClip>("Sound/SFX");

        foreach (var clip in sfxClips)
        {
            try
            {
                SFXType type = (SFXType)Enum.Parse(typeof(SFXType), clip.name);
                SoundController.Instance.sfxDic.Add(type, clip);
            }
            catch
            {
                Debug.LogWarning("SFX enum 필요" + clip.name);
            }
        }
    }


    public void PlaySFX(SFXType type)
    {
        sfxSource.PlayOneShot(sfxDic[type]);
    }

    public void PlaySkillSFX(string skillName)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Sound/SFX/{skillName}");
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Logger.Warning($"SFX {skillName} not found!");
        }
    }

    public void PlayBGM(BGMType type, float fadeTime = 0)
    {
        if (bgmSource.clip != null)
        {
            if (bgmSource.clip.name == type.ToString())
            {
                return;
            }

            if (fadeTime == 0)
            {
                bgmSource.clip = bgmDic[type];
                bgmSource.Play();
                bgmSource.volume = 0.3f;
            }
            else
            {
                StartCoroutine(FadeOutBGM(() =>
                {
                    bgmSource.clip = bgmDic[type];
                    bgmSource.Play();
                    StartCoroutine(FadeInBGM(fadeTime));
                },fadeTime));                
            }

        }
        else
        {
            if (fadeTime == 0)
            {
                bgmSource.clip = bgmDic[type];
                bgmSource.Play();
                bgmSource.volume = 0.3f;
            }
            else
            {
                bgmSource.volume = 0;
                bgmSource.clip = bgmDic[type];
                bgmSource.Play();
                StartCoroutine(FadeInBGM(fadeTime));
            }

        }
        
    }

    private IEnumerator FadeOutBGM(Action onComplete, float duration)
    { 
        float startVolume = bgmSource.volume;
        float time = 0;

        while (time < duration)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0F, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        bgmSource.volume = 0f;
        onComplete?.Invoke();
    }

    private IEnumerator FadeInBGM(float duration = 1.0f)
    {
        float targetVolume = PlayerPrefs.GetFloat("BGMVolume", 0.3f);
        float time = 0f;

        while (time < duration)
        {
            bgmSource.volume = Mathf.Lerp(0f, targetVolume, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        bgmSource.volume = targetVolume;
    }
    
    public void SetBGMVolume(float volume)
    { 
        bgmSource.volume = volume;
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
    
    public void StopBGMWithFade(float fadeDuration = 1.0f)
    {
        StartCoroutine(FadeOutBGM(() =>
        {
            bgmSource.Stop();
            bgmSource.clip = null;
        }, fadeDuration));
    }
}
