using UnityEngine;

public class ControladorJugadorMinijuego1 : MonoBehaviour
{
    [Header("Configuración del Jugador")]
    public KeyCode teclaArriba = KeyCode.W;
    public KeyCode teclaIzquierda = KeyCode.A;
    public KeyCode teclaDerecha = KeyCode.D;

    [Header("Configuración de Movimiento")]
    public float fuerzaImpulso = 2.5f;
    public float velocidadRotacion = 200f;
    public float velocidadMaxima = 5f;

    [Header("Visual")]
    public GameObject llamaPropulsor;

    [Header("Referencias")]
    public GameObject efectoExplosion;
    public ControladorJuegoMinijuego1 controladorJuego;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (llamaPropulsor != null)
        {
            llamaPropulsor.SetActive(false);
        }
    }

    void Update()
    {
        ManejarRotacion();
        ManejarVisualPropulsor();
    }

    void FixedUpdate()
    {
        ManejarMovimiento();
    }

    void ManejarRotacion()
    {
        if (Input.GetKey(teclaIzquierda))
        {
            transform.Rotate(0, 0, velocidadRotacion * Time.deltaTime);
        }
        if (Input.GetKey(teclaDerecha))
        {
            transform.Rotate(0, 0, -velocidadRotacion * Time.deltaTime);
        }
    }

    void ManejarMovimiento()
    {
        if (Input.GetKey(teclaArriba))
        {
            Vector2 direccion = transform.up;
            rb.AddForce(direccion * fuerzaImpulso);

            if (rb.linearVelocity.magnitude > velocidadMaxima)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * velocidadMaxima;
            }
        }
    }

    void ManejarVisualPropulsor()
    {
        if (llamaPropulsor != null)
        {
            llamaPropulsor.SetActive(Input.GetKey(teclaArriba));
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstaculo") ||
            collision.gameObject.CompareTag("Pared"))
        {
            if (efectoExplosion != null)
            {
                Instantiate(efectoExplosion, transform.position, transform.rotation);
            }

            if (controladorJuego != null)
            {
                controladorJuego.JugadorChoco(gameObject);
            }

            Destroy(gameObject);
        }
    }
}