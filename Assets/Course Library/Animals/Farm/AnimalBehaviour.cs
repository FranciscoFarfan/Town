using UnityEngine;

public class AnimalBehaviour : MonoBehaviour
{
    public float radioMovimiento = 10f;
    public float velocidad = 1f;
    public float tiempoCambioMin = 3f;
    public float tiempoCambioMax = 7f;

    private Animator animator;
    private Vector3 destino;
    private bool estaComiendo = false;
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
            estaComiendo = !estaComiendo; // alterna entre comer y caminar
            tiempoAccion = Random.Range(tiempoCambioMin, tiempoCambioMax);

            if (!estaComiendo) SetNuevoDestino();
        }

        if (estaComiendo)
        {
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