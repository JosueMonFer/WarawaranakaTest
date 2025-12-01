using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GestorFinJuego : MonoBehaviour
{
    [Header("Panel de Fin de Juego")]
    public GameObject panelFinJuego;
    public TextMeshProUGUI textoResultado;
    public TextMeshProUGUI textoDetalles;

    [Header("Botones")]
    public Button botonReintentar;
    public Button botonMenuPrincipal;

    [Header("Configuración")]
    public float tiempoAntesDeDetener = 1f; // Segundos antes de pausar el juego

    [Header("Audio de Fin de Juego")]
    public AudioClip musicaDerrota; // Música que suena al perder
    public AudioClip musicaVictoria; // Música que suena al ganar (opcional)
    [Range(0f, 1f)]
    public float volumenMusicaFinJuego = 0.5f;

    private bool juegoTerminado = false;
    private AudioSource audioSourceFinJuego;

    void Start()
    {
        // Asegurarse de que el panel esté oculto al inicio
        if (panelFinJuego != null)
        {
            panelFinJuego.SetActive(false);
        }

        // Configurar botones
        if (botonReintentar != null)
        {
            botonReintentar.onClick.AddListener(Reintentar);
        }

        if (botonMenuPrincipal != null)
        {
            botonMenuPrincipal.onClick.AddListener(VolverAlMenu);
        }

        // Crear AudioSource para la música de fin de juego
        audioSourceFinJuego = gameObject.AddComponent<AudioSource>();
        audioSourceFinJuego.loop = true;
        audioSourceFinJuego.playOnAwake = false;
        audioSourceFinJuego.volume = volumenMusicaFinJuego;
    }

    public void PersonajeDerrotado(string nombrePersonaje)
    {
        if (juegoTerminado) return;

        juegoTerminado = true;

        Debug.Log($"¡Fin del juego! {nombrePersonaje} ha sido derrotado.");

        // Detener música de pelea
        ControladorMusicaPelea musicaPelea = ControladorMusicaPelea.ObtenerInstancia();
        if (musicaPelea != null)
        {
            musicaPelea.DetenerMusica();
            Debug.Log("Música de pelea detenida");
        }

        // Reproducir música de derrota
        if (musicaDerrota != null && audioSourceFinJuego != null)
        {
            audioSourceFinJuego.clip = musicaDerrota;
            audioSourceFinJuego.Play();
            Debug.Log("Reproduciendo música de derrota");
        }
        else
        {
            Debug.LogWarning("No hay música de derrota asignada o AudioSource no disponible");
        }

        // Reproducir sonido de derrota (opcional - efecto corto)
        // ControladorSonidos.ObtenerInstancia()?.SonidoDerrota();

        // Mostrar panel de fin de juego después de un breve delay
        StartCoroutine(MostrarPanelFinJuego(nombrePersonaje));
    }

    System.Collections.IEnumerator MostrarPanelFinJuego(string nombrePersonaje)
    {
        // Esperar un momento antes de mostrar el panel
        yield return new WaitForSeconds(tiempoAntesDeDetener);

        // Pausar el juego
        Time.timeScale = 0f;

        // Configurar textos
        if (textoResultado != null)
        {
            textoResultado.text = "FIN DEL JUEGO";
        }

        if (textoDetalles != null)
        {
            textoDetalles.text = $"{nombrePersonaje} ha sido derrotado";
        }

        // Mostrar el panel
        if (panelFinJuego != null)
        {
            panelFinJuego.SetActive(true);
        }
    }

    void Reintentar()
    {
        // Reproducir sonido
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonOpciones();

        // Detener música de fin de juego
        if (audioSourceFinJuego != null && audioSourceFinJuego.isPlaying)
        {
            audioSourceFinJuego.Stop();
            Debug.Log("Música de fin de juego detenida");
        }

        // Reanudar el tiempo
        Time.timeScale = 1f;

        // IMPORTANTE: Destruir la instancia actual de música de pelea para que se cree una nueva
        ControladorMusicaPelea musicaPeleaActual = ControladorMusicaPelea.ObtenerInstancia();
        if (musicaPeleaActual != null)
        {
            musicaPeleaActual.DestruirInstancia();
            Debug.Log("Instancia de ControladorMusicaPelea destruida para reiniciar");
        }

        // Recargar la escena actual (esto recreará todo, incluyendo la música del mapa)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void VolverAlMenu()
    {
        // Reproducir sonido
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonAtras();

        // Detener música de fin de juego
        if (audioSourceFinJuego != null && audioSourceFinJuego.isPlaying)
        {
            audioSourceFinJuego.Stop();
        }

        // Reanudar el tiempo
        Time.timeScale = 1f;

        // Destruir música de pelea
        ControladorMusicaPelea musicaPelea = ControladorMusicaPelea.ObtenerInstancia();
        if (musicaPelea != null)
        {
            musicaPelea.DestruirInstancia();
        }

        // Reanudar música del menú
        ControladorMusica musicaMenu = ControladorMusica.ObtenerInstancia();
        if (musicaMenu != null)
        {
            musicaMenu.ReanudarMusica();
        }

        // Limpiar datos del juego
        DatosJuego.LimpiarDatos();

        // Volver al menú principal
        SceneManager.LoadScene("MainMenu");
    }

    // Método opcional para victoria (si agregas enemigos en el futuro)
    public void PersonajeVictorioso(string nombrePersonaje)
    {
        if (juegoTerminado) return;

        juegoTerminado = true;

        Debug.Log($"¡Victoria! {nombrePersonaje} ha ganado.");

        // Detener música de pelea
        ControladorMusicaPelea musicaPelea = ControladorMusicaPelea.ObtenerInstancia();
        if (musicaPelea != null)
        {
            musicaPelea.DetenerMusica();
        }

        // Reproducir música de victoria (si está asignada)
        if (musicaVictoria != null && audioSourceFinJuego != null)
        {
            audioSourceFinJuego.clip = musicaVictoria;
            audioSourceFinJuego.Play();
            Debug.Log("Reproduciendo música de victoria");
        }

        // Reproducir sonido de victoria (opcional)
        // ControladorSonidos.ObtenerInstancia()?.SonidoVictoria();

        StartCoroutine(MostrarPanelVictoria(nombrePersonaje));
    }

    System.Collections.IEnumerator MostrarPanelVictoria(string nombrePersonaje)
    {
        yield return new WaitForSeconds(tiempoAntesDeDetener);

        Time.timeScale = 0f;

        if (textoResultado != null)
        {
            textoResultado.text = "¡VICTORIA!";
        }

        if (textoDetalles != null)
        {
            textoDetalles.text = $"{nombrePersonaje} ha ganado la batalla";
        }

        if (panelFinJuego != null)
        {
            panelFinJuego.SetActive(true);
        }
    }
}