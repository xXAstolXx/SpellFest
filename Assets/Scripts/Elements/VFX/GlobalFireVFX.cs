using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;
using Color = UnityEngine.Color;

public class GlobalFireVFX : MonoBehaviour
{
    [HideInInspector]
    public static GlobalFireVFX Instance;


    [SerializeField]
    int resoX;
    [SerializeField]
    int resoY;

    [SerializeField]
    Color innerColor;
    [SerializeField]
    Color middleColor;
    [SerializeField]
    Color outerColor;

    [SerializeField]
    AnimationCurve innerBaseSpawnProb;
    [SerializeField]
    AnimationCurve innerFlameExpandProb;
    [SerializeField]
    AnimationCurve innerBaseDeathProb;

    [SerializeField]
    AnimationCurve middleBaseSpawnProb;
    [SerializeField]
    AnimationCurve middleFlameExpandProb;
    [SerializeField]
    AnimationCurve middleBaseDeathProb;

    [SerializeField]
    AnimationCurve outerBaseSpawnProb;
    [SerializeField]
    AnimationCurve outerFlameExpandProb;
    [SerializeField]
    AnimationCurve outerBaseDeathProb;

    [SerializeField]
    float expandProb;
    [SerializeField]
    float flameSpawnProb;
    [SerializeField]
    float adjustBaseSpawn;


    float[] innerBaseSpawnProbArray;
    float[] innerFlameExpandProbArray;
    float[] innerBaseDeathProbArray;

    float[] middleBaseSpawnProbArray;
    float[] middleFlameExpandProbArray;
    float[] middleBaseDeathProbArray;

    float[] outerBaseSpawnProbArray;
    float[] outerFlameExpandProbArray;
    float[] outerBaseDeathProbArray;

    Float4S[] baseSpawnProbData;
    Float4S[] flameExpandProbData;
    Float4S[] baseDeathProbData;

    int UpdateBaseKernel;
    int UpdateFlameKernel;
    int DrawKernel;
    int DedrawKernel;

    int threadGroupsX;
    int threadGroupsY;

    ComputeShader computeShader;

    ComputeBuffer baseSpawnProbBuffer;
    ComputeBuffer flameExpandProbBuffer;
    ComputeBuffer baseDeathProbBuffer;
    ComputeBuffer baseBuffer;
    ComputeBuffer flameHeightBuffer;

    public RenderTexture renderTexture { get; private set; }
    MaterialPropertyBlock materialPropertyBlock;

    ComputeBuffer test;
    Float4S[] t;

    bool executeFrame;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        UpdateShader();
        //materialPropertyBlock.SetTexture("_FireTex", renderTexture);
        //GetComponent<SpriteRenderer>().SetPropertyBlock(materialPropertyBlock);
        //test.GetData(t);
        //Debug.Log("spawnLimit: " + t[0].innerValue + ". on cpu: " + spawnLimit);
        //Debug.Log("length prop: " + t[0].middleValue + ". on cpu: " + lengthData[7].middleValue);
        //Debug.Log("stay alive prob: " + t[0].outerValue + ". on cpu: " + thicknessDespawnData[7].outerValue);
    }

    private void Initialize()
    {
        executeFrame = true;

        threadGroupsX = resoX / 8;
        threadGroupsY = resoY / 8;

        renderTexture = new RenderTexture(resoX, resoY, 0);
        renderTexture.enableRandomWrite = true;
        renderTexture.format = RenderTextureFormat.ARGBFloat;
        renderTexture.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat;
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.Create();

        //materialPropertyBlock = new MaterialPropertyBlock();
        //materialPropertyBlock.SetTexture("_FireTex", renderTexture);
        //GetComponent<SpriteRenderer>().SetPropertyBlock(materialPropertyBlock);
        //GetComponent<SpriteRenderer>().sharedMaterial.SetTexture("_FireTex", renderTexture);

        innerBaseSpawnProbArray = CurveToWidthArray(innerBaseSpawnProb, resoX);
        middleBaseSpawnProbArray = CurveToWidthArray(middleBaseSpawnProb, resoX);
        outerBaseSpawnProbArray = CurveToWidthArray(outerBaseSpawnProb, resoX);
        InitializeBaseSpawnData();

        innerFlameExpandProbArray = CurveToHeightArray(innerFlameExpandProb, resoX);
        middleFlameExpandProbArray = CurveToHeightArray(middleFlameExpandProb, resoX);
        outerFlameExpandProbArray = CurveToHeightArray(outerFlameExpandProb, resoX);
        InitializeFlameExpandData();

        innerBaseDeathProbArray = CurveToWidthArray(innerBaseDeathProb, resoX);
        middleBaseDeathProbArray = CurveToWidthArray(middleBaseDeathProb, resoX);
        outerBaseDeathProbArray = CurveToWidthArray(outerBaseDeathProb, resoX);
        InitializeBaseDeathData();

        int size = resoX * resoX;
        int[] baseData = new int[size];
        int[] flameHeightData = new int[size];
        for (int i = 0; i < size; i++)
        {
            baseData[i] = 0;
            flameHeightData[i] = 0;
        }

        computeShader = Resources.Load<ComputeShader>("Shader/VFX/FireVFXCompute");

        UpdateBaseKernel = computeShader.FindKernel("CSUpdateBase");
        UpdateFlameKernel = computeShader.FindKernel("CSUpdateFlame");
        DrawKernel = computeShader.FindKernel("CSDraw");
        DedrawKernel = computeShader.FindKernel("CSDedraw");

        baseBuffer = new ComputeBuffer(resoX * resoX, sizeof(int));
        flameHeightBuffer = new ComputeBuffer(resoX * resoX, sizeof(int));
        baseSpawnProbBuffer = new ComputeBuffer(resoY, sizeof(float) * 4);
        baseDeathProbBuffer = new ComputeBuffer(resoX, sizeof(float) * 4);
        flameExpandProbBuffer = new ComputeBuffer(resoX, sizeof(float) * 4);

        baseBuffer.SetData(baseData);
        flameHeightBuffer.SetData(flameHeightData);

        baseSpawnProbBuffer.SetData(baseSpawnProbData);
        baseDeathProbBuffer.SetData(baseDeathProbData);
        flameExpandProbBuffer.SetData(flameExpandProbData);

        computeShader.SetBuffer(UpdateBaseKernel, "baseBuffer", baseBuffer);
        computeShader.SetBuffer(UpdateBaseKernel, "flameHeightBuffer", flameHeightBuffer);
        computeShader.SetBuffer(UpdateBaseKernel, "baseSpawnProbBuffer", baseSpawnProbBuffer);
        computeShader.SetBuffer(UpdateBaseKernel, "baseDeathProbBuffer", baseDeathProbBuffer);

        computeShader.SetBuffer(UpdateFlameKernel, "baseBuffer", baseBuffer);
        computeShader.SetBuffer(UpdateFlameKernel, "flameHeightBuffer", flameHeightBuffer);
        computeShader.SetBuffer(UpdateFlameKernel, "flameExpandProbBuffer", flameExpandProbBuffer);

        computeShader.SetTexture(DedrawKernel, "renderTexture", renderTexture);

        computeShader.SetTexture(DrawKernel, "renderTexture", renderTexture);
        computeShader.SetBuffer(DrawKernel, "baseBuffer", baseBuffer);
        computeShader.SetBuffer(DrawKernel, "flameHeightBuffer", flameHeightBuffer);

        computeShader.SetFloat("time", Time.time);
        computeShader.SetFloat("flameSpawnProb", flameSpawnProb);

        computeShader.SetVector("innerColor", innerColor);
        computeShader.SetVector("middleColor", middleColor);
        computeShader.SetVector("outerColor", outerColor);

        computeShader.SetInt("resoX", resoX);
        computeShader.SetInt("resoY", resoY);

        //test = new ComputeBuffer(1, sizeof(float) * 4);
        //t = new Float4S[1];
        //t[0] = new Float4S(0, 0, 0);
        //test.SetData(t);
        //computeShader.SetBuffer(SetKernel, "test", test);

        //computeShader.Dispatch(InitializeKernel, threadGroupsX, threadGroupsY, 1);
    }

    private void UpdateShader()
    {
        if (executeFrame)
        {
            executeFrame = true;
            computeShader.SetFloat("time", Time.time);
            computeShader.Dispatch(DedrawKernel, threadGroupsX, threadGroupsY, 1);
            computeShader.Dispatch(UpdateBaseKernel, threadGroupsX, threadGroupsY / 2, 1);
            computeShader.Dispatch(UpdateFlameKernel, threadGroupsX, threadGroupsY / 2, 1);
            int[] flameHeightData = new int[resoX * resoX];
            flameHeightBuffer.GetData(flameHeightData);
            computeShader.Dispatch(DrawKernel, threadGroupsX, threadGroupsY, 1);

        }
    }

    private float[] CurveToHeightArray(AnimationCurve curve, int arrayLength)
    {
        float[] result = new float[arrayLength];

        float stepSize = 1f / arrayLength;
        for (int i = 0; i < arrayLength; i++)
        {
            result[i] = Mathf.Clamp01(curve.Evaluate(i * stepSize));
        }

        return result;
    }

    private float[] CurveToWidthArray(AnimationCurve curve, int arrayLength)
    {
        float[] result = new float[arrayLength];

        int half = arrayLength / 2;
        float stepSize = 1f / (half - 1);

        for (int i = 0; i < half; i++)
        {
            Debug.Log("--YYYY " + (i * stepSize));
            float value = Mathf.Clamp01(curve.Evaluate((i) * stepSize));
            result[half - 1 - i] = value;
            result[half + i] = value;
        }
        return result;
    }

    private void InitializeBaseSpawnData()
    {
        baseSpawnProbData = new Float4S[resoX];

        for (int i = 0; i < resoX; i++)
        {
            float tempAddA = innerBaseSpawnProbArray[i] * adjustBaseSpawn;
            float tempAddB = tempAddA + middleBaseSpawnProbArray[i] * adjustBaseSpawn;
            float tempAddC = tempAddB + outerBaseSpawnProbArray[i] * adjustBaseSpawn;
            baseSpawnProbData[i] = new Float4S(tempAddA, tempAddB, tempAddC);
        }
    }

    private void InitializeBaseDeathData()
    {
        baseDeathProbData = new Float4S[resoX];

        for (int i = 0; i < resoX; i++)
        {
            baseDeathProbData[i] = new Float4S(1 - (1 - innerBaseDeathProbArray[i]) * 0.1f, 1 - (1 - middleBaseDeathProbArray[i]) * 0.1f, 1 - (1 - outerBaseDeathProbArray[i]) * 0.1f);
        }
    }

    private void InitializeFlameExpandData()
    {
        flameExpandProbData = new Float4S[resoX];
        for (int i = 0; i < resoX; i++)
        {
            flameExpandProbData[i] = new Float4S(innerFlameExpandProbArray[i], middleFlameExpandProbArray[i], outerFlameExpandProbArray[i]);
        }
    }

    private void OnDestroy()
    {
        baseBuffer.Release();
        flameHeightBuffer.Release();
        baseSpawnProbBuffer.Release();
        baseDeathProbBuffer.Release();
        flameExpandProbBuffer.Release();
        //test.Release();
        renderTexture.Release();
    }
}

public struct Float4S
{
    public float innerValue;
    public float middleValue;
    public float outerValue;
    public float paddingDummy;

    public Float4S(float innerValue, float middleValue, float outerValue)
    {
        this.innerValue = innerValue;
        this.middleValue = middleValue;
        this.outerValue = outerValue;
        paddingDummy = -1;
    }
}

