using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControladorJuegoMinijuego1 : MonoBehaviour
{
    [Header("Configuración del Jugador")]
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

    [Header("Audio")]
    public AudioClip sonidoAtrapar;

    private int puntuacion = 0;
    private float tiempoRestante = 30f;
    private bool juegoActivo = true;
    private Camera camaraJuego;
    private AudioSource audioSource;

    void Start()
    {
        camaraJuego = Camera.main;
        audioSource = gameObject.AddComponent<AudioSource>();

        ActualizarUI();
        panelGameOver.SetActive(false);

        // Configurar botones
        botonReiniciar.onClick.AddListener(ReiniciarJuego);
        botonMenu.onClick.AddListener(VolverAlMenu);

        // Ocultar el cursor y bloquearlo en el centro (opcional)
        Cursor.visible = true;
    }

    void Update()
    {
        if (juegoActivo)
        {
            MoverJugador();
            ActualizarTiempo();
        }
    }

    void MoverJugador()
    {
        // Obtener la posición del mouse en el mundo
        Vector3 posicionMouse = camaraJuego.ScreenToWorldPoint(Input.mousePosition);

        // Mantener la posición Y del jugador y solo cambiar X
        float nuevaX = Mathf.Clamp(posicionMouse.x, limiteIzquierdo, limiteDerecho);
        transform.position = new Vector3(nuevaX, transform.position.y, transform.position.z);
    }

    void ActualizarTiempo()
    {
        tiempoRestante -= Time.deltaTime;
        textoTiempo.text = "Tiempo: " + Mathf.CeilToInt(tiempoRestante).ToString() + "s";

        if (tiempoRestante <= 0)
        {
            TerminarJuego();
        }
    }

    void OnTriggerEnter2D(Collider2D colision)
    {
        if (juegoActivo && colision.CompareTag("Estrella"))
        {
            puntuacion += 10;
            ActualizarUI();

            if (sonidoAtrapar != null)
            {
                audioSource.PlayOneShot(sonidoAtrapar);
            }

            Destroy(colision.gameObject);
        }
    }

    void ActualizarUI()
    {
        textoPuntuacion.text = "Puntuación: " + puntuacion.ToString();
    }

    void TerminarJuego()
    {
        juegoActivo = false;
        panelGameOver.SetActive(true);
        textoPuntuacionFinal.text = "¡Juego Terminado!\nPuntuación Final: " + puntuacion.ToString();
        Time.timeScale = 0f; // Pausar el juego
    }

    void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void VolverAlMenu()
    {
        Time.timeScale = 1f;
        // Cambiar "MenuPrincipal" por el nombre de tu escena de menú
        SceneManager.LoadScene("MainMenu");
    }
}