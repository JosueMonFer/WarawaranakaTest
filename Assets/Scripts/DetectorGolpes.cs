using UnityEngine;

/// <summary>
/// Este script se coloca en un objeto hijo del personaje para detectar golpes
/// Crea un área de ataque que detecta enemigos
/// </summary>
public class DetectorGolpes : MonoBehaviour
{
    [Header("Configuración")]
    public float rangoAtaque = 1.5f; // Qué tan lejos alcanza el golpe
    public LayerMask capaEnemigos; // Qué objetos pueden recibir daño

    [Header("Offset de Ataque")]
    public Vector2 offsetAtaque = new Vector2(0.8f, 0); // Dónde aparece el área de ataque

    [Header("Debug Visual")]
    public bool mostrarGizmos = true;
    public Color colorGizmo = Color.red;

    private ControladorPersonajePelea controladorPersonaje;

    void Start()
    {
        controladorPersonaje = GetComponentInParent<ControladorPersonajePelea>();

        if (controladorPersonaje == null)
        {
            Debug.LogError("DetectorGolpes debe ser hijo de un objeto con ControladorPersonajePelea");
        }
    }

    /// <summary>
    /// Detectar y golpear enemigos en rango
    /// </summary>
    public void DetectarGolpe(float daño)
    {
        // Calcular posición del ataque
        Vector2 posicionAtaque = (Vector2)transform.position + offsetAtaque;

        // Voltear el offset si el personaje mira a la izquierda
        if (controladorPersonaje != null && !controladorPersonaje.EstaMirandoDerecha())
        {
            posicionAtaque = (Vector2)transform.position + new Vector2(-offsetAtaque.x, offsetAtaque.y);
        }

        // Detectar todos los objetos en rango
        Collider2D[] objetosGolpeados = Physics2D.OverlapCircleAll(posicionAtaque, rangoAtaque, capaEnemigos);

        // Aplicar daño a cada objeto golpeado
        foreach (Collider2D objetoGolpeado in objetosGolpeados)
        {
            // Intentar obtener el sistema de vida del enemigo
            SistemaVidaPersonaje vidaEnemigo = objetoGolpeado.GetComponent<SistemaVidaPersonaje>();

            if (vidaEnemigo != null)
            {
                vidaEnemigo.RecibirDano(daño);
                Debug.Log($"¡Golpe acertado! Daño: {daño} a {objetoGolpeado.name}");
            }

            // O si tienes otro sistema de vida para enemigos:
            // SistemaVidaEnemigo vidaEnemigo = objetoGolpeado.GetComponent<SistemaVidaEnemigo>();
            // if (vidaEnemigo != null) { vidaEnemigo.RecibirDano(daño); }
        }

        if (objetosGolpeados.Length > 0)
        {
            Debug.Log($"Se golpearon {objetosGolpeados.Length} objeto(s)");
        }
    }

    /// <summary>
    /// Método para llamar desde el ControladorPersonajePelea cuando ataca
    /// </summary>
    public void EjecutarAtaque(float daño)
    {
        DetectarGolpe(daño);
    }

    // Visualizar el área de ataque en el editor
    void OnDrawGizmos()
    {
        if (!mostrarGizmos) return;

        Gizmos.color = colorGizmo;

        // Dibujar el rango de ataque
        Vector2 posicionAtaque = (Vector2)transform.position + offsetAtaque;
        Gizmos.DrawWireSphere(posicionAtaque, rangoAtaque);

        // Dibujar también del otro lado (cuando mira a la izquierda)
        Gizmos.color = new Color(colorGizmo.r, colorGizmo.g, colorGizmo.b, 0.3f);
        Vector2 posicionAtaqueIzquierda = (Vector2)transform.position + new Vector2(-offsetAtaque.x, offsetAtaque.y);
        Gizmos.DrawWireSphere(posicionAtaqueIzquierda, rangoAtaque);
    }
}