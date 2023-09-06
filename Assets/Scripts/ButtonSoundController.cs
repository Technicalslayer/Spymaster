using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSoundController : MonoBehaviour
{
    public AudioClip clickSound1;
    public AudioClip clickSound2;
    private AudioSource soundSource;

    private void Awake() {
        soundSource = GetComponent<AudioSource>();
    }

    public void playSound1() {
        soundSource.clip = clickSound1;
        soundSource.Play();
    }

    public void playSound2() { 
        soundSource.clip = clickSound2;
        soundSource.Play();
    }
}
