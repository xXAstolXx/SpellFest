using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wendigo_Sound", menuName = "Enemies/Sound/Wendigo")]
public class WendigoSound : EnemySound
{
    [SerializeField]
    private AudioClip switchPhaseSound;
    [SerializeField]
    private AudioClip hyperDeathBeamDeluxe;
    [SerializeField]
    private AudioClip dashAttack;
    [SerializeField]
    private AudioClip arenaSound;
    

    public AudioClip SwitchPhaseSound { get => switchPhaseSound; }
    public AudioClip HyperDeathBeamDeluxe { get => hyperDeathBeamDeluxe; }
    public AudioClip DashAttack { get => dashAttack; }
    public AudioClip ArenaSound { get => arenaSound; }
}
