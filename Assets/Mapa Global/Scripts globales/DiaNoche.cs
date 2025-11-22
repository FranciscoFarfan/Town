using UnityEngine;

public class SunLightSim : MonoBehaviour
{
    public Light sunLight;
    public float hourOfDay = 12f;   // 0 a 24
    public float velocidadDIa = 0.1f;

    public Gradient lightColor;
    public AnimationCurve lightIntensity;
    public AnimationCurve ambientMultiplier;

    public Color ambientBaseColor = Color.white;
    public float ambientBaseMultiplier = 1f;

    public float time01 = 0f;
    float dayIntensity = 2f;

    // --- NUEVO: skybox ---
    public Material skyboxMaterial;
    public Gradient skyTintGradient;
    public AnimationCurve skyExposureCurve;

    string exposureProperty = null;

    void Start()
    {
        // detectar sunLight
        if (sunLight == null)
        {
            sunLight = GetComponent<Light>();
            if (sunLight == null || sunLight.type != LightType.Directional)
                sunLight = FindObjectOfType<Light>();
        }

        // asignar skybox si existe
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
            DetectSkyboxExposureProperty();
        }
    }

    void Update()
    {
        // avanzar hora
        hourOfDay += Time.deltaTime * velocidadDIa;
        if (hourOfDay >= 24f) hourOfDay = 0f;

        // 0-1
        time01 = Mathf.Clamp01(hourOfDay / 24f);

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

        // --- ACTUALIZAR SKYBOX ---
        if (skyboxMaterial != null)
        {
            // tinte del skybox panorámico
            if (skyTintGradient != null && skyboxMaterial.HasProperty("_Tint"))
            {
                Color tint = skyTintGradient.Evaluate(time01);
                skyboxMaterial.SetColor("_Tint", tint);
            }

            // exposición del skybox panorámico
            if (skyExposureCurve != null && exposureProperty != null)
            {
                float exp = skyExposureCurve.Evaluate(time01);
                skyboxMaterial.SetFloat(exposureProperty, exp);
            }
        }
    }

    // Detectar qué propiedad usa el shader Panoramic
    void DetectSkyboxExposureProperty()
    {
        if (skyboxMaterial == null) return;

        string[] props =
        {
            "_Exposure",
            "_ExposureBias",
            "_ExposureScale",
            "_ExposureCompensation"
        };

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
