using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    public static GlobalSettings Instance { get; private set; } = null;


    public ElementSettings elementSettings { get; private set; }

    [SerializeField]
    private EnemySettings enemySettings;
    public EnemySettings EnemySettings => enemySettings;

    [SerializeField]
    float unitSpeedModifierMin;
    public float UnitSpeedModifierMin => unitSpeedModifierMin;

    [SerializeField]
    float uiDamageIncreaseMultiplier;
    public float UiDamageIncreaseMultiplier => uiDamageIncreaseMultiplier;

    [SerializeField]
    float screenFreezeDurationEnemyAttack;
    public float ScreenFreezeDurationEnemyAttack => screenFreezeDurationEnemyAttack;

    [SerializeField]
    private int fps;

    public float pixelWorldUnitRatio { get; private set; }


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found another GlobalSettings on " + gameObject.name);
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        GetPixelWorldUnitRatio();

        elementSettings = new ElementSettings();

        Application.targetFrameRate = fps;
    }

    private void GetPixelWorldUnitRatio()
    {
        int resoX = Screen.width;
        Vector3 v = Camera.main.ScreenToWorldPoint(new Vector3(resoX,0,0)) - Camera.main.ScreenToWorldPoint(Vector3.zero);
        float worldUnitScreenwidth = Mathf.Abs(v.x);
        pixelWorldUnitRatio = resoX / worldUnitScreenwidth;
    }
}
