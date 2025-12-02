using System.Data.Common;
using UnityEngine;

public class ControlPlayer : MonoBehaviour
{
    [Header("Configuraci�n de Movimiento")]
    public Rigidbody2D rb;
    public float velocidadMovimiento = 5f;
    public float fuerzaSalto = 7f; // Fuerza para el salto

    [Header("Configuraci�n de Controles")]
    [Tooltip("Si es True usa A,W,D. Si es False usa Flechas.")]
    public bool usarControlesWASD = true;

    [Header("Deteccion de Suelo")]
    public Transform pies; // Arrastra aqu� un objeto vac�o ubicado en los pies del jugador
    public float radioDeteccionSuelo = 0.2f;
    public LayerMask capaSuelo; // Define qu� es suelo en el editor
    private bool enElSuelo;

    // Variables de C�mara y L�mites
    private float limiteIzquierdo, limiteDerecho, limiteInferior, limiteSuperior;
    private Camera camaraPrincipal;

    [Header("L�mites de Pantalla")]
    public bool restringirAPantalla = true;
    public float margenLimite = 0.5f;
    
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        // Si no asignaste el RB manualmente, lo buscamos
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        camaraPrincipal = Camera.main;
        CalcularLimitesPantalla();
    }

    void Update()
    {
        MoverJugador();
        RestringirDentroDePantalla(); // Aplicamos los l�mites calculados
    }

    void MoverJugador()
    {
        Debug.Log($"Si me ejecutan");
        float inputHorizontal = 0f;
        bool inputSalto = false;

        // --- L�GICA DE SELECCI�N DE CONTROLES ---
        if (usarControlesWASD)
        {
            // Opci�n 1: Teclas A, W, D
            if (Input.GetKey(KeyCode.A)) { 
            inputHorizontal = -1f;
                Debug.Log("me presionaron");
            }
            if (Input.GetKey(KeyCode.D)) inputHorizontal = 1f;
            if (Input.GetKeyDown(KeyCode.W)) inputSalto = true;
            Debug.Log($"valor del vector con AWD {inputHorizontal}");
        }
        else
        {
            // Opci�n 2: Flechas del teclado
            if (Input.GetKey(KeyCode.LeftArrow)) inputHorizontal = -1f;
            if (Input.GetKey(KeyCode.RightArrow)) inputHorizontal = 1f;
            if (Input.GetKeyDown(KeyCode.UpArrow)) inputSalto = true;
            Debug.Log($"valor del vector con felchas {inputHorizontal}");

        }

        //esta línea es la que hace el movimiento
        rb.linearVelocity = new Vector2(inputHorizontal * velocidadMovimiento, rb.linearVelocity.y);

        // ----- Animación de caminar -----
bool estaCaminando = Mathf.Abs(inputHorizontal) > 0.1f;
anim.SetBool("isWalking", estaCaminando);

// Flip del sprite
if (inputHorizontal > 0) transform.localScale = new Vector3(1, 1, 1);
if (inputHorizontal < 0) transform.localScale = new Vector3(-1, 1, 1);

        // 2. Detectar si estamos tocando el suelo (para no saltar infinito)
        // Crea un peque�o c�rculo en los pies para ver si toca la capa "Suelo"
        if (pies != null)
        {
            enElSuelo = Physics2D.OverlapCircle(pies.position, radioDeteccionSuelo, capaSuelo);
        }
        else
        {
            // Fallback por si olvidaste asignar el objeto 'pies', permite saltar siempre (cuidado)
            enElSuelo = true;
        }

        // 3. Aplicar Salto
        if (inputSalto && enElSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Reseteamos velocidad vertical para salto consistente
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
        }
    }

    // Mantiene al jugador dentro de la c�mara
    void RestringirDentroDePantalla()
    {
        if (!restringirAPantalla || camaraPrincipal == null) return;

        // Mathf.Clamp obliga al valor a quedarse entre un m�nimo y un m�ximo
        float xClamped = Mathf.Clamp(transform.position.x, limiteIzquierdo, limiteDerecho);
        float yClamped = Mathf.Clamp(transform.position.y, limiteInferior, limiteSuperior);

        transform.position = new Vector3(xClamped, yClamped, transform.position.z);
    }

    void CalcularLimitesPantalla()
    {
        if (camaraPrincipal == null) return;

        float alturaCamara = camaraPrincipal.orthographicSize;
        float anchoCamara = alturaCamara * camaraPrincipal.aspect;
        Vector3 posCam = camaraPrincipal.transform.position;

        limiteIzquierdo = posCam.x - anchoCamara + margenLimite;
        limiteDerecho = posCam.x + anchoCamara - margenLimite;
        limiteInferior = posCam.y - alturaCamara + margenLimite;
        limiteSuperior = posCam.y + alturaCamara - margenLimite;
    }

    // Dibuja el c�rculo de detecci�n de suelo en el editor para que puedas verlo
    private void OnDrawGizmosSelected()
    {
        if (pies != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pies.position, radioDeteccionSuelo);
        }
    }
}