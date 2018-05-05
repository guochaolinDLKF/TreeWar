using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioKind
{
    Alert,
    ArrowShoot,
    Bg_fast,
    Bg_moderate,
    ButtonClick,
    Miss,
    ShootPerson,
    Timer,

}
public class AudioManager : BaseManager
{
    public AudioManager(GameFacade facede) : base(facede)
    {

    }

    private Dictionary<AudioKind, string> m_AudioPth = new Dictionary<AudioKind, string>()
    {
        {AudioKind.Alert, "Sounds/Alert"},
        {AudioKind.ArrowShoot, "Sounds/ArrowShoot" },
        {AudioKind.Bg_fast, "Sounds/Bg(fast)" },
        {AudioKind.Bg_moderate, "Sounds/Bg(moderate)" },
        {AudioKind.ButtonClick, "Sounds/ButtonClick" },
        {AudioKind.Miss, "Sounds/Miss" },
        {AudioKind.ShootPerson, "Sounds/ShootPerson" },
        {AudioKind.Timer, "Sounds/Timer" },
    };

    private AudioSource m_BgAudio;
    private AudioSource m_NormalAudio;
    private bool isBG = true;
    public override void Init()
    {
        GameObject audioSourceGo = new GameObject("AudioSources");
        GameObject audioListener = new GameObject("SoundListener");
        audioListener.transform.SetParent(audioSourceGo.transform);
        audioListener.AddComponent<AudioListener>();
        m_BgAudio = audioSourceGo.AddComponent<AudioSource>();
        m_NormalAudio = audioSourceGo.AddComponent<AudioSource>();

    }

    public void PlayBgAudio(AudioKind soundName)
    {
        PlaySound(soundName, m_BgAudio, LoadSound(m_AudioPth.TryGet(soundName)), 0.3f, true);
    }
    public void PlayNormalAudio(AudioKind soundName)
    {
        PlaySound(soundName, m_NormalAudio, LoadSound(m_AudioPth.TryGet(soundName)));
    }
    void PlaySound(AudioKind soundName, AudioSource source, AudioClip clip, float volume = 1, bool isLoop = false)
    {
        source.clip = clip;
        source.loop = isLoop;
        source.volume = volume;
        source.Play();

    }
    AudioClip LoadSound(string soundPath)
    {
        return Resources.Load<AudioClip>(soundPath);
    }
}
