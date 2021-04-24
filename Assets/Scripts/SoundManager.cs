using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioSource _soundSuccess;

    public void OnSuccess() 
    {
        _soundSuccess.pitch += 0.1f;
        _soundSuccess.Play();
    } 

    public void ResetPitch() 
    {
        _soundSuccess.Stop();
        _soundSuccess.pitch = 0.9f;
    }

    private void Awake()
    {
        instance = this;
    }
}
