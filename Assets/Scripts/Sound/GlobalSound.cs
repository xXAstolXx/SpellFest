using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSound : MonoBehaviour
{
    #region singleton
    static GlobalSound instance;
    public static GlobalSound Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    [SerializeField]
    SillyManSound sillyManSound;
    [SerializeField]
    SlimeSounds slimeSound;
    [SerializeField]
    ArcherSound archerSound;
    [SerializeField]
    WendigoSound wendigoSound;
    [SerializeField]
    PlayerSound playerSound;
    [SerializeField]
    NPCSound gorbatSound;
    [SerializeField]
    NPCSound wispsTalking;
    [SerializeField]
    ItemSounds itemSound;
    [SerializeField]
    GameSound gameSound;

    public PlayerSound PlayerSound{ get => playerSound; }
    public NPCSound GorbatSound { get => gorbatSound; }
    public NPCSound WispsSound { get => wispsTalking; }
    public ItemSounds ItemSound { get => itemSound; }
    public SlimeSounds SlimeSound { get => slimeSound; }
    public ArcherSound ArcherSound { get => archerSound; }
    public SillyManSound SillyManSound { get => sillyManSound; }
    public GameSound GameSound { get => gameSound; }
    public WendigoSound WendigoSound { get => wendigoSound; }

    private void Awake()
    {
        instance = this;
    }
}
