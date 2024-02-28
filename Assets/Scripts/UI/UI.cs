using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    static UI instance;
    public static UI Instance
    {
        get { return instance; }
    }

    [HideInInspector]
    public PlayerHealthBar playerHealthBar;
    [HideInInspector]
    public SpellUI spellUI;
    [HideInInspector]
    public Manabar manaBar;
    [HideInInspector]
    public DeathScreen deathScreen;
    [HideInInspector]
    public DialogueScreen dialogueScreen;
    [HideInInspector]
    public PauseMenu pauseMenu;
    [HideInInspector]
    public HurtScreen hurtScreen;


    [SerializeField]
    private Image overlayImage;

    private void Awake()
    {
        instance = this;
        playerHealthBar = GetComponentInChildren<PlayerHealthBar>();
        spellUI = GetComponentInChildren<SpellUI>();
        manaBar = GetComponentInChildren<Manabar>();
        deathScreen = GetComponentInChildren<DeathScreen>();
        dialogueScreen = GetComponentInChildren<DialogueScreen>();
        pauseMenu = GetComponentInChildren<PauseMenu>();
        hurtScreen = GetComponentInChildren<HurtScreen>();
    }

    public void ActivateOverlay()
    {
        overlayImage.enabled = true;
    }

    public void DeActivateOverlay()
    {
        overlayImage.enabled = false;
    }
}
