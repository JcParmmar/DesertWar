using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    //Set Some Sounds
    [Header("Audio Clips")]
    [SerializeField] AudioClip[] FootStep;
    [SerializeField] AudioClip JumpSound;
    [SerializeField] AudioClip FallSound;

    //AudioSource is Use for Play Sound
    [SerializeField] AudioSource audioSource;

    private void Awake()
    {
        //set Audio Source
        audioSource = GetComponentInParent<AudioSource>();
    }

    //Step Method Call From Animation
    private void Step()
    {
        //Get Randome Clip From All Foot Step Sounds
        AudioClip clip = FootStep[UnityEngine.Random.Range(0,FootStep.Length)];

        //Play Sound
        audioSource.PlayOneShot(clip);
    }

    //Jump Method Call From Movement Scrpit
    public void Jump()
    {
        audioSource.PlayOneShot(JumpSound);
    }
}
