using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlay : MonoBehaviour
{
    public AudioSource Button1;
    public AudioSource Button2;
    public AudioSource Button3;
    public AudioSource Button4;
    public AudioSource Button5;
    public AudioSource Button6;
    public AudioSource Laser;

    public void PlayButton1()
    {
        Button1.Play();
    }

    public void PlayButton2()
    {
        Button2.Play();
    }

    public void PlayButton3()
    {
        Button3.Play();
    }

    public void PlayButton4()
    {
        Button4.Play();
    }

    public void PlayButton5()
    {
        Button5.Play();
    }

    public void PlayButton6()
    {
        Button6.Play();
    }

    public void PlayLaser()
    {
        Laser.Play();
    }
}

