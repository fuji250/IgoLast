using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

public AudioClip sound1;
public AudioClip sound2;
    AudioSource audioSource;

 void Start () {
   //Component??????
   audioSource = GetComponent<AudioSource>();
 }

 void Update () {
   // ??
  if (Input.GetKey (KeyCode.LeftArrow)) {
   //??(sound1)????????
    audioSource.PlayOneShot(sound1);
   }
 }
}