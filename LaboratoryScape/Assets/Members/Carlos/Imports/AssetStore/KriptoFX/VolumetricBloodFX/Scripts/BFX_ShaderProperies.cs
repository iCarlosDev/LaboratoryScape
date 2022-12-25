using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Rendering.HighDefinition;

public class BFX_ShaderProperies : MonoBehaviour {

    public BFX_BloodSettings BloodSettings;

    public AnimationCurve FloatCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float GraphTimeMultiplier = 1, GraphIntensityMultiplier = 1;
    public float TimeDelay = 0;

    private bool canUpdate;
    bool isFrized;
    private float startTime;

    private int cutoutPropertyID;
    int forwardDirPropertyID;
    float timeLapsed;

    private DecalProjector decal;
    private Material decalMat;

    public event Action OnAnimationFinished;

    private void Awake()
    {
        decal = GetComponent<DecalProjector>();
        decalMat = new Material(decal.material);
        decal.material = decalMat;
        cutoutPropertyID = Shader.PropertyToID("_Cutout");
        forwardDirPropertyID = Shader.PropertyToID("_DecalForwardDir");

        OnEnable();
    }

    private void OnEnable()
    {
        startTime = Time.time + TimeDelay;
        canUpdate = true;

        GetComponent<DecalProjector>().enabled = true;

        var eval = FloatCurve.Evaluate(0) * GraphIntensityMultiplier;
        decalMat.SetFloat(cutoutPropertyID, eval);
        decalMat.SetVector(forwardDirPropertyID, transform.up);

    }

    private void OnDisable()
    {

        var eval = FloatCurve.Evaluate(0) * GraphIntensityMultiplier;
        decalMat.SetFloat(cutoutPropertyID, eval);

        timeLapsed = 0;
    }



    private void Update()
    {
        if (!canUpdate) return;


        var deltaTime = BloodSettings == null ? Time.deltaTime : Time.deltaTime * BloodSettings.AnimationSpeed;
        if (BloodSettings != null && BloodSettings.FreezeDecalDisappearance && (timeLapsed / GraphTimeMultiplier) > 0.3f) { }
        else timeLapsed += deltaTime;

        var eval = FloatCurve.Evaluate(timeLapsed / GraphTimeMultiplier) * GraphIntensityMultiplier;
        decalMat.SetFloat(cutoutPropertyID, eval);

        if (BloodSettings != null) decalMat.SetFloat("_LightIntencity", Mathf.Clamp(BloodSettings.LightIntensityMultiplier, 0.01f, 1f));

        if (timeLapsed >= GraphTimeMultiplier)
        {
            canUpdate = false;
            OnAnimationFinished?.Invoke();

        }
        decalMat.SetVector(forwardDirPropertyID, transform.up);
    }

}
