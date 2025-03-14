using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{

    [Header("SFX StepSound")]
    [Tooltip("Suara StepSound AudioClip")]
    [SerializeField] AudioSource _FootStepSfx;



    [Header("SFX StepSound")]
    [Tooltip("Suara StepSound AudioClip")]
    [SerializeField] AudioSource _GlideSfx;



    [Header("SFX StepSound")]
    [Tooltip("Suara StepSound AudioClip")]
    [SerializeField] AudioSource _LandingSfx;



    [Header("SFX StepSound")]
    [Tooltip("Suara StepSound AudioClip")]
    [SerializeField] AudioSource _PunchSfx;

    void Start()
    {
    }


    public void PlayGlideSfx()
    {
        _GlideSfx.Play();
    }

    public void StopGlideSfx()
    {
        _GlideSfx.Stop();

    }

    public void PlayLandingSfx()
    {

        _LandingSfx.Play();
    }

    public void StopLandingSfx()
    {
        _LandingSfx.Stop();
    }

    public void PlayPunchSfx()
    {
        _PunchSfx.volume = Random.Range(0.7f, 1f);
        _PunchSfx.pitch = Random.Range(0.5f, 2.5f);

        _PunchSfx.Play();

    }
    private void PlayFootstepSfx()
    {

        _FootStepSfx.volume = Random.Range(0.7f, 1f);
        _FootStepSfx.pitch = Random.Range(0.5f, 2.5f);

        _FootStepSfx.Play();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
