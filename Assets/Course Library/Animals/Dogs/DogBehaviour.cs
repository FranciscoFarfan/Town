using UnityEngine;

public class DogBehaviour : MonoBehaviour
{
    [Header("Configuración de movimiento")]
    public float radioMovimiento = .5f;
    public float velocidad = 1f;
    public float tiempoCambioMin = 6f;
    public float tiempoCambioMax = 15f;

    private Animator animator;
    private Vector3 destino;
    private bool estaComiendo = false;
    private bool estaSentado = false;
    private float tiempoAccion;

    void Start()
    {
        animator = GetComponent<Animator>();
        SetNuevoDestino();
        tiempoAccion = Random.Range(tiempoCambioMin, tiempoCambioMax);
    }

    void Update()
    {
        tiempoAccion -= Time.deltaTime;

        if (tiempoAccion <= 0f)
        {
            // Alternar entre estados
            if (estaSentado)
            {
                // Si estaba sentado, se levanta
                estaSentado = false;
                estaComiendo = false;
                SetNuevoDestino();
            }
            else
            {
                // Decide aleatoriamente entre sentarse o comer
                int decision = Random.Range(0, 3); // 0 = sentarse, 1 = comer, 2 = caminar
                if (decision == 0)
                {
                    estaSentado = true;
                    estaComiendo = false;
                }
                else if (decision == 1)
                {
                    estaComiendo = true;
                    estaSentado = false;
                }
                else
                {
                    estaComiendo = false;
                    estaSentado = false;
                    SetNuevoDestino();
                }
            }

            tiempoAccion = Random.Range(tiempoCambioMin, tiempoCambioMax);
        }

        // Aplicar animaciones según estado
        if (estaSentado)
        {
            animator.SetBool("Sit_b", true);
            animator.SetBool("Eat_b", false);
            animator.SetFloat("Speed_f", 0f);
        }
        else if (estaComiendo)
        {
            animator.SetBool("Sit_b", false);
            animator.SetBool("Eat_b", true);
            animator.SetFloat("Speed_f", 0f);
        }
        else
        {
            Mover();
        }
    }

    void Mover()
    {
        Vector3 direccion = destino - transform.position;
        direccion.y = 0;

        if (direccion.magnitude > 0.2f)
        {
            transform.position += direccion.normalized * velocidad * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direccion);

            animator.SetBool("Sit_b", false);
            animator.SetBool("Eat_b", false);
            animator.SetFloat("Speed_f", velocidad);
        }
        else
        {
            animator.SetFloat("Speed_f", 0f);
        }
    }

    void SetNuevoDestino()
    {
        Vector2 randomCircle = Random.insideUnitCircle * radioMovimiento;
        destino = new Vector3(transform.position.x + randomCircle.x, transform.position.y, transform.position.z + randomCircle.y);
    }
}