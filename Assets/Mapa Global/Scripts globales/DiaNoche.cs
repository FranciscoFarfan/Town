using UnityEngine;

public class SunLightSim : MonoBehaviour
{
    public Light sunLight;
    public Gradient lightColor;
    public AnimationCurve lightIntensity;
    public AnimationCurve ambientMultiplier;

    public Color ambientBaseColor = Color.white;
    public float ambientBaseMultiplier = 1f;

    public Material skyboxMaterial;
    public Gradient skyTintGradient;
    public AnimationCurve skyExposureCurve;

    string exposureProperty = null;
    float dayIntensity = 2f;

    void Start()
    {
        if (sunLight == null)
        {
            sunLight = GetComponent<Light>();
            if (sunLight == null || sunLight.type != LightType.Directional)
                sunLight = FindObjectOfType<Light>();
        }

        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
            DetectSkyboxExposureProperty();
        }
    }

    void Update()
    {
        // 🔹 Leer hora desde GameManager
        float hourOfDay = GameManager.Instance.hora;

        // convertir a 0-1
        float time01 = Mathf.Clamp01(hourOfDay / 24f);

        // ángulo del sol
        float angle = time01 * 360f - 90f;
        transform.rotation = Quaternion.Euler(angle, 170f, 0f);

        // luz del sol
        float sInt = lightIntensity.Evaluate(time01) * dayIntensity;
        sunLight.color = lightColor.Evaluate(time01);
        sunLight.intensity = sInt;
        sunLight.enabled = angle >= 0f && angle <= 180f;

        // luz ambiental
        float amb = ambientMultiplier.Evaluate(time01) * ambientBaseMultiplier;
        RenderSettings.ambientLight = ambientBaseColor * amb;
        RenderSettings.ambientIntensity = amb;
        DynamicGI.UpdateEnvironment();

        // skybox
        if (skyboxMaterial != null)
        {
            if (skyTintGradient != null && skyboxMaterial.HasProperty("_Tint"))
            {
                Color tint = skyTintGradient.Evaluate(time01);
                skyboxMaterial.SetColor("_Tint", tint);
            }

            if (skyExposureCurve != null && exposureProperty != null)
            {
                float exp = skyExposureCurve.Evaluate(time01);
                skyboxMaterial.SetFloat(exposureProperty, exp);
            }
        }
    }

    void DetectSkyboxExposureProperty()
    {
        if (skyboxMaterial == null) return;

        string[] props = { "_Exposure", "_ExposureBias", "_ExposureScale", "_ExposureCompensation" };
        foreach (string p in props)
        {
            if (skyboxMaterial.HasProperty(p))
            {
                exposureProperty = p;
                break;
            }
        }
    }
}