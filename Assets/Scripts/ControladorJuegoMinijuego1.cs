using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ControladorJuegoMinijuego1 : MonoBehaviour
{
    [Header("Configuracion")]
    public float velocidadMovimiento = 10f;
    public float limiteIzquierdo = -8f;
    public float limiteDerecho = 8f;

    [Header("UI")]
    public TextMeshProUGUI textoPuntuacion;
    public TextMeshProUGUI textoTiempo;
    public GameObject panelGameOver;
    public TextMeshProUGUI textoPuntuacionFinal;
    public Button botonReiniciar;
    public Button botonMenu;

    [Header("Sonidos (Opcional)")]
    public AudioClip sonidoAtrapar;
    private AudioSource audioSource;

    private int puntuacion = 0;
    private float tiempoRestante = 30f;
    private bool juegoActivo = true;
    private Camera camaraPrincipal;

    void Start()
    {
        camaraPrincipal = Camera.main;
        audioSource = gameObject.AddComponent<AudioSource>();

        if (panelGameOver != null)
            panelGameOver.SetActive(false);

        if (botonReiniciar != null)
            botonReiniciar.onClick.AddListener(Reiniciar);

        if (botonMenu != null)
            botonMenu.onClick.AddListener(VolverAlMenu);

        ActualizarUI();
    }

    void Update()
    {
        if (!juegoActivo) return;

        // Mover el jugador con el mouse
        MoverConMouse();

        // Actualizar temporizador
        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0)
        {
            FinDelJuego();
        }

        ActualizarUI();
    }

    void MoverConMouse()
    {
        // Obtener posición del mouse en el mundo
        Vector3 posicionMouse = camaraPrincipal.ScreenToWorldPoint(Input.mousePosition);
        posicionMouse.z = 0;

        // Mover suavemente hacia el mouse (solo en X)
        Vector3 posicionObjetivo = new Vector3(posicionMouse.x, transform.position.y, 0);
        transform.position = Vector3.Lerp(transform.position, posicionObjetivo, velocidadMovimiento * Time.deltaTime);

        // Limitar movimiento horizontal
        float x = Mathf.Clamp(transform.position.x, limiteIzquierdo, limiteDerecho);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Estrella"))
        {
            // Aumentar puntuación
            puntuacion += 10;

            // Reproducir sonido
            if (sonidoAtrapar != null)
                audioSource.PlayOneShot(sonidoAtrapar);

            // Destruir la estrella
            Destroy(other.gameObject);

            // Notificar al generador que aumente la velocidad
            GeneradorEstrellasMinijuego1 generador = FindFirstObjectByType<GeneradorEstrellasMinijuego1>();
            if (generador != null)
                generador.AumentarDificultad();
        }
        else if (other.CompareTag("Bomba"))
        {
            // Perder puntos
            puntuacion = Mathf.Max(0, puntuacion - 20);
            Destroy(other.gameObject);
        }
    }

    void ActualizarUI()
    {
        if (textoPuntuacion != null)
            textoPuntuacion.text = "Puntuación: " + puntuacion;

        if (textoTiempo != null)
            textoTiempo.text = "Tiempo: " + Mathf.CeilToInt(tiempoRestante) + "s";
    }

    void FinDelJuego()
    {
        juegoActivo = false;

        if (panelGameOver != null)
            panelGameOver.SetActive(true);

        if (textoPuntuacionFinal != null)
            textoPuntuacionFinal.text = "¡Puntuación Final: " + puntuacion + "!";

        // Detener el generador
        GeneradorEstrellasMinijuego1 generador = FindFirstObjectByType<GeneradorEstrellasMinijuego1>();
        if (generador != null)
            generador.DetenerGeneracion();
    }

    void Reiniciar()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonComenzar();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void VolverAlMenu()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonAtras();
        SceneManager.LoadScene("MainMenu");
    }

    public bool EstaJuegoActivo()
    {
        return juegoActivo;
    }
}