using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Color = UnityEngine.Color;

public class PullPath : MonoBehaviour
{
    [SerializeField, Range(0,1)]
    float width = 0.5f;
    //minWidth is percentage of width
    [SerializeField, Range(0,1)]
    float minWidth = 0.1f;
    [SerializeField, Range(1, 15)]
    float maxPullDistance;
    [SerializeField, Range(0.5f, 5)]
    float maxChargeTime;

    [SerializeField]
    Color colorInvalid;
    [SerializeField, Range(0f,1f)]
    float backgroundTransparency;

    Vector2 startPosition;
    float pullStepDistance;
    float pullDistance;

    float chargeTimePosition;

    public float travelDistance { get; private set; }
    public Vector2 pullDirection { get; private set; }
    int step;

    public bool isPathValid { get; private set; }

    List<float> maxTravelDistance;

    LineRenderer lineRenderer;
    Arrow arrow;

    [SerializeField]
    GameObject collisionCheckerTemplate;
    GameObject collisionChecker;
    [SerializeField]
    GameObject attackChargeVFXTemplate;
    GameObject attackChargeVFX;

    private AudioSource audioSource;
    public UnityEvent<int> OnSpellChargeChanged {  get; private set; } = new UnityEvent<int>();
    public UnityEvent OnSpellValid { get; private set; } = new UnityEvent();


    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        arrow = GetComponentInChildren<Arrow>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Initialize()
    {
        pullStepDistance = maxPullDistance / 3;

        InitializeValues();

        lineRenderer.widthMultiplier = width;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        lineRenderer.material.SetColor("_ColorInvalid", colorInvalid);
        lineRenderer.material.SetFloat("_BackgroundTransparency", backgroundTransparency);
        lineRenderer.material.SetFloat("_WidthSmall", minWidth*(1/width));
    }

    private void InitializeValues()
    {
        pullDirection = Vector2.zero;
        pullDistance = 0;
        chargeTimePosition = 0;
        step = 0;
        lineRenderer.material.SetInt("_IsValid", 0);
        isPathValid = false;
    }

    private void Update()
    {
        if (lineRenderer.enabled)
        {
            Vector2 pullVector = (Vector2)(Input.mousePosition) / GlobalSettings.Instance.pixelWorldUnitRatio - startPosition / GlobalSettings.Instance.pixelWorldUnitRatio;
            if(pullVector.magnitude > 0 )
            {
                pullDirection = pullVector.normalized;

                if (pullVector.magnitude > maxPullDistance)
                {
                    pullVector = pullDirection * maxPullDistance;
                }
                pullDistance = pullVector.magnitude;
                
                float positionChange = (Time.deltaTime/maxChargeTime) * (maxPullDistance / maxChargeTime);

                if (chargeTimePosition < pullDistance)
                {
                    chargeTimePosition += positionChange;
                    chargeTimePosition = Mathf.Clamp(chargeTimePosition, chargeTimePosition, pullDistance);
                }
                else if (chargeTimePosition > pullDistance)
                {
                    chargeTimePosition = pullDistance;
                }
                Vector2 chargeTimeVector = pullDirection * chargeTimePosition;
                lineRenderer.material.SetFloat("_CurrentChargeLength", 1 - (chargeTimePosition / maxPullDistance));
                lineRenderer.material.SetFloat("_CurrentLength", 1 - (pullDistance / maxPullDistance));

                if (chargeTimePosition / maxPullDistance >= 1 / 6f)
                {
                    if(isPathValid == false)
                    {
                        lineRenderer.material.SetInt("_IsValid", 1);
                        isPathValid = true;
                        OnSpellChargeChanged.Invoke(1);

                        Destroy(attackChargeVFX);
                        attackChargeVFX = Instantiate(attackChargeVFXTemplate, transform);
                        attackChargeVFX.GetComponent<ParticleSystem>().Play();
                        OnSpellValid.Invoke();
                    }

                    var main = attackChargeVFX.GetComponent<ParticleSystem>().main;
                    var emission = attackChargeVFX.GetComponent<ParticleSystem>().emission;
                    //main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 0.5f + (chargeTimePosition / maxPullDistance) * 3.5f);
                    main.startLifetime = new ParticleSystem.MinMaxCurve(0.2f, 0.2f + (chargeTimePosition / maxPullDistance) * 0.6f);
                    emission.rateOverTime = new ParticleSystem.MinMaxCurve((chargeTimePosition / maxPullDistance) * 500f,(chargeTimePosition / maxPullDistance) * 500f);
                }
                else
                {
                    if(isPathValid == true)
                    {
                        lineRenderer.material.SetInt("_IsValid", 0);
                        isPathValid = false;
                        OnSpellChargeChanged.Invoke(0);

                        Destroy(attackChargeVFX);
                    }
                }

                collisionChecker.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                collisionChecker.transform.rotation = Quaternion.Euler(0, 0, TransformHelper.DirectionToAngle(chargeTimeVector * -1));

                int tempStep = Mathf.Clamp(Mathf.FloorToInt(chargeTimePosition / pullStepDistance), 0, maxTravelDistance.Count - 1);
                if (step != tempStep)
                {
                    if (tempStep > step)
                    {
                        if(tempStep == 1) 
                        { 
                            audioSource.clip = GlobalSound.Instance.PlayerSound.StartChargeSound; 
                            audioSource.Play(); 
                        }
                        else
                        {
                            audioSource.clip = GlobalSound.Instance.PlayerSound.MediumChargeSound;
                            audioSource.Play();
                        }
                    }
                    else
                    {
                        audioSource.clip = GlobalSound.Instance.PlayerSound.DecreaseChargeSound;
                        audioSource.Play();
                    }
                    step = tempStep;
                    OnSpellChargeChanged.Invoke(step+1);
                }

                //lineRenderer.SetPosition(0, (Vector2)Camera.main.ScreenToWorldPoint(startPosition) + pullDirection * maxPullDistance);
                //lineRenderer.SetPosition(1, (Vector2)Camera.main.ScreenToWorldPoint(startPosition));
                lineRenderer.SetPosition(0, (Vector2)transform.position + pullDirection * 0.5f + pullDirection * maxPullDistance);
                lineRenderer.SetPosition(1, (Vector2)transform.position + pullDirection * 0.5f);



                //if (isPathValid)
                //{
                travelDistance = 0;
                if (step > 0)
                {
                    travelDistance += maxTravelDistance[step - 1];
                    travelDistance += ((chargeTimePosition - (step * pullStepDistance)) / pullStepDistance) * (maxTravelDistance[step] - maxTravelDistance[step - 1]);
                }
                else
                {
                    travelDistance = (chargeTimePosition/ pullStepDistance) * maxTravelDistance[step];
                }
                if (isPathValid)
                {
                    arrow.RenderPath(pullDirection * -1, travelDistance);
                }
                else
                {
                    arrow.ClearPath();
                }
                //}
            }
        }
    }

    public void StartPull(List<float> maxTravelDistance, GameObject projectile)
    {
        ElementProjectile elementProjectile = projectile.GetComponent<ElementProjectile>();

        collisionChecker = Instantiate(collisionCheckerTemplate);
        CopyHelper.CopyComponent(projectile.GetComponent<CapsuleCollider2D>(), collisionChecker);
        collisionChecker.GetComponent<CapsuleCollider2D>().isTrigger = true;
        collisionChecker.transform.localScale = projectile.transform.localScale;
        collisionChecker.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //collisionChecker.GetComponent<CollisionChecker>().OnCollisionEnter.AddListener(OnPathInvalid);
        //collisionChecker.GetComponent<CollisionChecker>().OnCollisionExit.AddListener(OnPathValid);
        //OnPathValid();

        lineRenderer.material.SetColor("_Color1", elementProjectile.ColorSteps[0]);
        lineRenderer.material.SetColor("_Color2", elementProjectile.ColorSteps[1]);
        lineRenderer.material.SetColor("_Color3", elementProjectile.ColorSteps[2]);

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
        
        InitializeValues();

        attackChargeVFXTemplate = elementProjectile.chargeVFXObject;

        startPosition = (Vector2)Input.mousePosition;
        this.maxTravelDistance = maxTravelDistance;
    }

    public void FinishPull()
    {
        lineRenderer.enabled = false;
        arrow.ClearPath();
        OnSpellChargeChanged.Invoke(0);
        Destroy(attackChargeVFX);
        Destroy(collisionChecker);
    }

    public int GetCurrentPullStep()
    {
        return step;
    }

    public void UpdateDirection(Vector2 moveVector)
    {
        if (attackChargeVFX)
        {
            if (moveVector.magnitude == 0)
            {
                return;
            }
            else if (moveVector.y < 0)
            {
                attackChargeVFX.GetComponent<ParticleSystemRenderer>().sortingLayerName = "PlayerVFX";
            }
            else if (moveVector.x < 0 && moveVector.y == 0)
            {
                attackChargeVFX.GetComponent<ParticleSystemRenderer>().sortingLayerName = "PlayerVFX";

            }
            else if (moveVector.x > 0 && moveVector.y == 0)
            {
                attackChargeVFX.GetComponent<ParticleSystemRenderer>().sortingLayerName = "PlayerVFX";
            }
            else
            {
                attackChargeVFX.GetComponent<ParticleSystemRenderer>().sortingLayerName = "EnemyVFX";
            }
        }
    }
}
