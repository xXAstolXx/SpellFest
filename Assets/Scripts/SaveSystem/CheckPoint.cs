using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField]
    int checkPointNumber = 0;
    [SerializeField]
    GameObject playerSpawn;

    [SerializeField]
    float healValue = 50f;
    bool hasTriggered = false;

    [SerializeField]
    Transform petLeftPosition;
    [SerializeField]
    Transform petRightPosition;
    Player player;

    Animator animator;

    AudioSource audioSource;

    InteractScreen interactScreen;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        interactScreen = GetComponentInChildren<InteractScreen>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("#####################triggered");
        if(hasTriggered == false)
        {
            hasTriggered = true;
            Game.Instance.globalData.currentCheckPoint = checkPointNumber;
            Game.Instance.globalData.spawnPositionX = playerSpawn.transform.position.x;
            Game.Instance.globalData.spawnPositionY = playerSpawn.transform.position.y;
            GetComponentInChildren<ParticleSystem>().Play();
            animator.SetBool("isActive", true);
            PlayActivatedSound();
            player = collision.transform.parent.GetComponent<Player>();
            Debug.Log(player);
            player.Heal(healValue);
        }
        interactScreen.PopUp();
        player.isInCheckpointRange = true;
        player.OnPetCat.AddListener(PetCat);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        interactScreen.ClosePopUp();
        player.isInCheckpointRange = false;
        player.OnPetCat.RemoveListener(PetCat);
    }


    private void PlayActivatedSound()
    {
        audioSource.clip = GlobalSound.Instance.GameSound.CheckPointActivated;
        audioSource.Play();
    }

    public void PetCat()
    {
        interactScreen.ClosePopUp();
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        player.GetComponent<PlayerInput>().DisableInput();
        movement.OnTargetReached.AddListener(StartPeting);


        if (transform.position.x < player.transform.position.x)
        {
            movement.StartAutoMove(petRightPosition.position);
        }
        else
        {
            movement.StartAutoMove(petLeftPosition.position);
        }
    }

    public void StartPeting()
    {
        Debug.Log("is peting");
        player.graphic.gameObject.SetActive(false);
        if (transform.position.x < player.transform.position.x)
        {
            animator.SetBool("isPetingLeft", false);
            animator.SetBool("isPetingRight", true);
        }
        else
        {
            animator.SetBool("isPetingLeft", true);
            animator.SetBool("isPetingRight", false);
        }

        if(player.activeSpell == SpellInput.FIRE)
        {
            animator.SetBool("isRed", true);
            animator.SetBool("isBlue", false);
        }
        else
        {
            animator.SetBool("isRed", false);
            animator.SetBool("isBlue", true);
        }

    }

    public void FinishPeting()
    {
        player.graphic.gameObject.SetActive(true);
        Animator playerAnimator;
        if (transform.position.x < player.transform.position.x)
        {
            player.graphic.SetGraphic(2);
            playerAnimator = player.graphic.graphicObjects[2].GetComponent<Animator>();

        }
        else
        {
            player.graphic.SetGraphic(1);
            playerAnimator = player.graphic.graphicObjects[1].GetComponent<Animator>();
        }
        if (player.activeSpell == SpellInput.FIRE)
        {
            //Fire Spell
            playerAnimator.SetBool("Fire", true);
            playerAnimator.SetBool("Ice", false);
        }
        else
        {
            //Ice Spell
            playerAnimator.SetBool("Fire", false);
            playerAnimator.SetBool("Ice", true);
        }

        player.GetComponent<PlayerInput>().EnableInput();
        animator.SetBool("isPetingLeft", false);
        animator.SetBool("isPetingRight", false);
        animator.SetBool("isRed", false);
        animator.SetBool("isBlue", false);

        interactScreen.PopUp();
    }
}
