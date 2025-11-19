using UnityEngine;

public class SunLightSim : MonoBehaviour
{
    public Light sunLight;
    public float rotationSpeed = 1f;
    public float dayIntensity = 2f;
    public bool isDayTime = true;

    public Gradient lightColor; // amanecer, día, atardecer, noche
    public AnimationCurve lightIntensity; // 0 (noche) a 1 (día)

    public float timeOfDay = 0f; // 0 a 1 representa el ciclo completo

    void Start() {
        if (sunLight == null) {
            sunLight = GetComponent<Light>();
            if (sunLight == null || sunLight.type != LightType.Directional) {
                sunLight = FindObjectOfType<Light>();
            }
        }
    }

    void Update() {
        // Avanza el tiempo del día
        timeOfDay += Time.deltaTime * rotationSpeed * 0.01f;
        if (timeOfDay > 1f) timeOfDay = 0f;

        // Calcular ángulo solar
        float angle = timeOfDay * 360f - 90f;
        transform.rotation = Quaternion.Euler(angle, 170f, 0f);

        // Actualizar color e intensidad
        float intensity = lightIntensity.Evaluate(timeOfDay) * dayIntensity;
        sunLight.color = lightColor.Evaluate(timeOfDay);
        sunLight.intensity = intensity;

        // Actualizar estado de día
        isDayTime = angle >= 0f && angle <= 180f;
    }


}
