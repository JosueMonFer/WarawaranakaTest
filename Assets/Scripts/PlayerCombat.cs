using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    public bool usarWASD;

    [Header("Referencias")]
    public Transform puntoDeAtaque; // Arrastra aquí el objeto hijo (Hitbox)
    public LayerMask capaEnemigos;  // Define qué es un enemigo (Layers)

    [Header("Estadísticas")]
    public float rangoAtaque = 5f;
    public float daño = 20f;
    public float fuerzaEmpuje = 100f;

    [Header("Cooldown")]
    public float tiempoEntreAtaques = 0.5f;
    private float tiempoSiguienteAtaque = 0f;

    [Header("Controles")]
    // Aquí definimos que la tecla por defecto sea la 'E'
    private KeyCode teclaAtacarManoIzquierda = KeyCode.E;
    private KeyCode teclaAtacarPatadaIzquierda = KeyCode.R;

    private KeyCode teclaAtacarManoDerecha = KeyCode.M;
    private KeyCode teclaAtacarPatadaDerecha = KeyCode.N;


    void Update()
    {
        // Comprobamos si ya pasó el tiempo de espera (Cooldown)
        if (Time.time >= tiempoSiguienteAtaque)
        {
            // Detectamos si presiona la tecla configurada (E)

            if (usarWASD)//jugador del lado izquierdo
            {
                if (Input.GetKeyDown(teclaAtacarManoIzquierda))//ataque de puño
                {
                    Atacar(30);
                    Debug.Log("Esta atacando");
                    tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques; // Reseteamos el reloj
                }
                if(Input.GetKeyDown(teclaAtacarPatadaIzquierda)) //patada
                {
                    Atacar(40);
                    Debug.Log("Esta atacando");
                    tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques; // Reseteamos el reloj
                }
            }
            else//jugador del lado derecho
            {
                if (Input.GetKeyDown(teclaAtacarManoDerecha))//ataque de puño
                {
                    Atacar(30);
                    Debug.Log("Esta atacando");
                    tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques; // Reseteamos el reloj
                }
                if (Input.GetKeyDown(teclaAtacarPatadaDerecha)) //patada
                {
                    //
                    Atacar(30);
                    Debug.Log("Esta atacando");
                    tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques; // Reseteamos el reloj                    Atacar(30);
                    Debug.Log("Esta atacando");
                    tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques; // Reseteamos el reloj
                }

            }

           
        }
    }

    void Atacar(float dano)
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
            ControlPlayer ScriptVida = rbEnemigo.GetComponentInParent<ControlPlayer>();
            ScriptVida.RecibirDano(dano);
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