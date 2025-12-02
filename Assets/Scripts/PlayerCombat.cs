using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Referencias")]
    public Transform puntoDeAtaque; // Arrastra aquí el objeto hijo (Hitbox)
    public LayerMask capaEnemigos;  // Define qué es un enemigo (Layers)

    [Header("Estadísticas")]
    public float rangoAtaque = 10f;
    public float daño = 20f;
    public float fuerzaEmpuje = 100f;

    [Header("Cooldown")]
    public float tiempoEntreAtaques = 0.5f;
    private float tiempoSiguienteAtaque = 0f;

    [Header("Controles")]
    // Aquí definimos que la tecla por defecto sea la 'E'
    private KeyCode teclaAtacar = KeyCode.E;

    void Update()
    {
        // Comprobamos si ya pasó el tiempo de espera (Cooldown)
        if (Time.time >= tiempoSiguienteAtaque)
        {
            // Detectamos si presiona la tecla configurada (E)
            if (Input.GetKeyDown(teclaAtacar))
            {
                Atacar();
                Debug.Log("Esta atacando");
                tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques; // Reseteamos el reloj
            }
        }
    }

    void Atacar()
    {
        // 1. Animación (Descomenta cuando tengas animaciones)
        // Animator anim = GetComponent<Animator>();
        // if(anim != null) anim.SetTrigger("Atacar");

        Debug.Log("¡Ataque con tecla E!");

        // 2. Detectar enemigos
        if (puntoDeAtaque == null) return;

        Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(puntoDeAtaque.position, rangoAtaque, capaEnemigos);

        // 3. Aplicar Daño y Empuje
        foreach (Collider2D enemigo in enemigosGolpeados)
        {
            Debug.Log("Golpeaste a: " + enemigo.name);

            // A. Empuje (Knockback)
            Rigidbody2D rbEnemigo = enemigo.GetComponent<Rigidbody2D>();
            if (rbEnemigo != null)
            {
                Vector2 direccion = (enemigo.transform.position - transform.position).normalized;

                // Si usas Unity 6 cambia 'velocity' por 'linearVelocity'
                rbEnemigo.linearVelocity = Vector2.zero;
                rbEnemigo.AddForce(direccion * fuerzaEmpuje, ForceMode2D.Impulse);
            }

            // B. Aquí iría el código para bajar vida al script del enemigo
        }
    }

    void OnDrawGizmosSelected()
    {
        if (puntoDeAtaque == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(puntoDeAtaque.position, rangoAtaque);
    }
}