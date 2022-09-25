using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : Singleton<AudioController>
{
    /// <summary>
    /// �p�`���Ƒł�
    /// </summary>
    public AudioClip soundPutStone;
    /// <summary>
    /// �΂���������̉�
    /// </summary>
    public AudioClip soundRemoveStone;
    /// <summary>
    /// �΂�u���Ȃ����̉�
    /// �J�[�\���𓮂������Ƃ��̉�
    /// </summary>
    public AudioClip soundError;

    /// <summary>
    /// ���̃V�[���̃I�[�f�B�I�\�[�X
    /// </summary>
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }
    /// <summary>
    /// �����o���i�G�C���A�X�j
    /// </summary>
    /// <param name="clip"></param>
    public void Play(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    /// <summary>
    /// �΂�ł�
    /// </summary>
    public void PlayPutStone()
    {
        Play(soundPutStone);
    }
    /// <summary>
    /// �΂���鉹
    /// </summary>
    public void PlayRemoveStone()
    {
        Play(soundRemoveStone);
    }
    /// <summary>
    /// �G���[���o�����̉�
    /// </summary>
    public void PlayError()
    {
        Play(soundError);
    }
}