using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Archer_Sound", menuName = "Enemies/Sound/Archer")]
public class ArcherSound : EnemySound
{
    [Header("Attack Sounds")]
    [SerializeField]
    private AudioClip chargeCrossbowSound;
    [SerializeField]
    private AudioClip crossbowShoot;
    [Header("Walking Sounds")]
    [SerializeField]
    private AudioClip walkingSound;

    public AudioClip ChargeCrossbowSound { get => chargeCrossbowSound; }
    public AudioClip CrossbowShoot { get => crossbowShoot; }
    public AudioClip WalkingSound { get => walkingSound; }
}
