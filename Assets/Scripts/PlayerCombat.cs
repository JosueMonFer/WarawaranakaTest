using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public bool usarWASD;

    [Header("Referencias")]
    public Transform puntoDeAtaque;
    public LayerMask capaEnemigos;
    private Animator anim;

    [Header("Estadísticas")]
    public float rangoAtaque = 5f;
    public float daño = 20f;
    public float fuerzaEmpuje = 100f;

    [Header("Cooldown")]
    public float tiempoEntreAtaques = 0.5f;
    private float tiempoSiguienteAtaque = 0f;

    [Header("Controles")]
    // Jugador izquierdo
    private KeyCode golpeIzq = KeyCode.E;
    private KeyCode patadaIzq = KeyCode.R;
    private KeyCode especialIzq = KeyCode.T;
    private KeyCode ultiIzq = KeyCode.G;
    private KeyCode comboIzq = KeyCode.F;   // 🔥 COMBO IZQUIERDO (nuevo)

    // Jugador derecho
    private KeyCode golpeDer = KeyCode.M;
    private KeyCode patadaDer = KeyCode.N;
    private KeyCode especialDer = KeyCode.K;
    private KeyCode ultiDer = KeyCode.L;
    private KeyCode comboDer = KeyCode.J;   // 🔥 COMBO DERECHO (nuevo)

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time >= tiempoSiguienteAtaque)
        {
            if (usarWASD)   // ------------------ JUGADOR IZQUIERDO ------------------
            {
                // --- GOLPE ---
                if (Input.GetKeyDown(golpeIzq))
                {
                    anim.SetTrigger("Punch");
                    Atacar(30);
                    tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques;
                }

                // --- PATADA ---
                if (Input.GetKeyDown(patadaIzq))
                {
                    anim.SetTrigger("Kick");
                    Atacar(40);
                    tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques;
                }

                // --- COMBO ---
                if (Input.GetKeyDown(comboIzq))
                {
                    anim.SetTrigger("Combo");
                    Atacar(60);
                    tiempoSiguienteAtaque = Time.time + 1f;
                }

                // --- ESPECIAL ---
                if (Input.GetKeyDown(especialIzq))
                {
                    anim.SetTrigger("Special");
                    Atacar(80);
                    tiempoSiguienteAtaque = Time.time + 1.2f;
                }

                // --- ULTI ---
                if (Input.GetKeyDown(ultiIzq))
                {
                    anim.SetTrigger("Ulti");
                    Atacar(150);
                    tiempoSiguienteAtaque = Time.time + 2f;
                }
            }
            else    // ------------------ JUGADOR DERECHO ------------------
            {
                // --- GOLPE ---
                if (Input.GetKeyDown(golpeDer))
                {
                    anim.SetTrigger("Punch");
                    Atacar(30);
                    tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques;
                }

                // --- PATADA ---
                if (Input.GetKeyDown(patadaDer))
                {
                    anim.SetTrigger("Kick");
                    Atacar(40);
                    tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques;
                }

                // --- COMBO ---
                if (Input.GetKeyDown(comboDer))
                {
                    anim.SetTrigger("Combo");
                    Atacar(60);
                    tiempoSiguienteAtaque = Time.time + 1f;
                }

                // --- ESPECIAL ---
                if (Input.GetKeyDown(especialDer))
                {
                    anim.SetTrigger("Special");
                    Atacar(80);
                    tiempoSiguienteAtaque = Time.time + 1.2f;
                }

                // --- ULTI ---
                if (Input.GetKeyDown(ultiDer))
                {
                    anim.SetTrigger("Ulti");
                    Atacar(150);
                    tiempoSiguienteAtaque = Time.time + 2f;
                }
            }
        }
    }

    void Atacar(float dano)
    {
        if (puntoDeAtaque == null) return;

        Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(
            puntoDeAtaque.position, rangoAtaque, capaEnemigos
        );

        foreach (Collider2D enemigo in enemigosGolpeados)
        {
            Rigidbody2D rbEnemigo = enemigo.GetComponent<Rigidbody2D>();
            ControlPlayer ScriptVida = rbEnemigo.GetComponentInParent<ControlPlayer>();
            ScriptVida.RecibirDano(dano);

            if (rbEnemigo != null)
            {
                Vector2 direccion = (enemigo.transform.position - transform.position).normalized;
                rbEnemigo.linearVelocity = Vector2.zero;
                rbEnemigo.AddForce(direccion * fuerzaEmpuje, ForceMode2D.Impulse);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (puntoDeAtaque == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(puntoDeAtaque.position, rangoAtaque);
    }
}
