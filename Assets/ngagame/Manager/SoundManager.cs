using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    private AudioSource sfxAS;

    [SerializeField]
    private string resourceFolder = "Sounds";
    [SerializeField]
    private List<string> list;

    private Dictionary<string, AudioClip> dict = null;
    private Dictionary<string, AudioClip> Dict
    {
        get
        {
            if (dict == null)
            {
                Serialize();
            }
            return dict;
        }
    }

    public void PlaySFX(string key, float volume = 1)
    {
        if(!Application.isPlaying)
		{
            return;
		}
        if (!UserSettings.Instance.SoundOn)
        {
            return;
        }

        if (Dict.ContainsKey(key))
        {
            sfxAS.PlayOneShot(Dict[key], volume);
        }
        else
        {
            AudioClip audioClip = Resources.Load<AudioClip>(resourceFolder + "/" + key);
            if (audioClip == null)
            {
                Debug.LogError(resourceFolder + "/" + key + " not exist");
                return;
            }
            Dict.Add(key, audioClip);
            sfxAS.PlayOneShot(Dict[key], volume);
        }
    }

    public void Serialize()
    {
        if (dict == null)
        {
            dict = new Dictionary<string, AudioClip>();
        }
    }

    private void Awake()
    {
        if(sfxAS == null)
        {
            sfxAS = GetComponent<AudioSource>();
            if(sfxAS == null)
            {
                gameObject.AddComponent<AudioSource>();
                sfxAS = GetComponent<AudioSource>();
            }
        }
    }
}
