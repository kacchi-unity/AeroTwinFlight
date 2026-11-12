using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Audio UI")]
    [SerializeField] private Toggle audioToggle;
    [SerializeField] private Slider audioSlider;

    public static AudioManager Instance;

    private List<AudioClip> clips = new List<AudioClip>();
    private bool isAudioLoaded = false;

    private void OnEnable()
    {
        if (audioToggle != null)
        {
            audioToggle.onValueChanged.AddListener(ToggleRandomAudio);
        }

        if (audioSlider != null)
        {
            audioSlider.onValueChanged.AddListener(SetSliderVolume);
        }
    }

    private void OnDisable()
    {
        if (audioToggle != null)
        {
            audioToggle.onValueChanged.RemoveListener(ToggleRandomAudio);
        }

        if (audioSlider != null)
        {
            audioSlider.onValueChanged.RemoveListener(SetSliderVolume);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        StartCoroutine(LoadAudioFromStreamingAssets());

        if (audioToggle != null)
        {
            ToggleRandomAudio(audioToggle.isOn);
        }
    }

    private IEnumerator LoadAudioFromStreamingAssets()
    {
        string folderPath = Path.Combine(Application.streamingAssetsPath, "Audio");

        if (Directory.Exists(folderPath))
        {
            string[] allFiles = Directory.GetFiles(folderPath);
            foreach (string filePath in allFiles)
            {
                if (!filePath.EndsWith(".mp3") && !filePath.EndsWith(".ogg") && !filePath.EndsWith(".wav"))
                    continue;

                string url = filePath.Contains("://") ? filePath : "file://" + filePath;

                AudioType audioType = AudioType.MPEG;
                if (filePath.EndsWith(".ogg")) audioType = AudioType.OGGVORBIS;
                else if (filePath.EndsWith(".wav")) audioType = AudioType.WAV;

                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
                {
                    yield return www.SendWebRequest();

                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                        if (clip != null)
                        {
                            clips.Add(clip);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("StreamingAssets/Audio 폴더가 없습니다: " + folderPath);
        }

        isAudioLoaded = true;

        if (audioToggle != null && audioToggle.isOn)
        {
            ToggleRandomAudio(true);
        }
    }

    private void ToggleRandomAudio(bool toggleState)
    {
        if (!isAudioLoaded || clips.Count == 0 || audioSource == null)
        {
            return;
        }

        if (toggleState)
        {
            int randomIndex = Random.Range(0, clips.Count);
            AudioClip selectedClip = clips[randomIndex];

            audioSource.clip = selectedClip;
            audioSource.Play();
            audioSource.volume = audioSlider.value;
        }

        else
        {
            audioSource.Stop();
        }
    }

    private void SetSliderVolume(float sliderValue)
    {
        if (audioSource != null)
        {
            audioSource.volume = sliderValue;
        }
    }
}
