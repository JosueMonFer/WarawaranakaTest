using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TemporizadorPelea : MonoBehaviour
{
    [Header("Configuracion del Temporizador")]
    [Tooltip("Duracion de la pelea en segundos (180 = 3 minutos)")]
    public float duracionPelea = 180f;

    [Header("Referencias UI")]
    public TextMeshProUGUI textoTemporizador;
    public GameObject panelResultadoEmpate;
    public TextMeshProUGUI textoResultado;
    public Button botonVolverMenu;

    [Header("Colores del Temporizador")]
    public Color colorNormal = Color.white;
    public Color colorAdvertencia = Color.yellow;
    public Color colorCritico = Color.red;

    [Header("Nombres de Jugadores")]
    public string nombreJugador1 = "JUGADOR 1";
    public string nombreJugador2 = "JUGADOR 2";

    private float tiempoRestante;
    private bool temporizadorActivo = true;
    private bool peleaTerminada = false;

    // Instancia estática para acceso global
    private static TemporizadorPelea instancia;

    void Awake()
    {
        // Configurar instancia estática
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Debug.LogWarning("Ya existe una instancia de TemporizadorPelea");
        }
    }

    void Start()
    {
        tiempoRestante = duracionPelea;

        if (textoTemporizador == null)
        {
            Debug.LogError("TemporizadorPelea: No se asigno el texto del temporizador!");
        }

        if (panelResultadoEmpate != null)
        {
            panelResultadoEmpate.SetActive(false);
        }

        if (botonVolverMenu != null)
        {
            botonVolverMenu.onClick.AddListener(VolverAlMenu);
        }
    }

    void Update()
    {
        if (!temporizadorActivo || peleaTerminada)
            return;

        tiempoRestante -= Time.deltaTime;

        ActualizarTextoTemporizador();
        ActualizarColorTemporizador();

        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            DeclararEmpate();
        }
    }

    void ActualizarTextoTemporizador()
    {
        if (textoTemporizador == null)
            return;

        int minutos = Mathf.FloorToInt(tiempoRestante / 60f);
        int segundos = Mathf.FloorToInt(tiempoRestante % 60f);

        textoTemporizador.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    void ActualizarColorTemporizador()
    {
        if (textoTemporizador == null)
            return;

        if (tiempoRestante <= 10f)
        {
            textoTemporizador.color = Mathf.PingPong(Time.time * 2, 1) > 0.5f ? colorCritico : colorNormal;
        }
        else if (tiempoRestante <= 30f)
        {
            textoTemporizador.color = colorAdvertencia;
        }
        else
        {
            textoTemporizador.color = colorNormal;
        }
    }

    void DeclararEmpate()
    {
        if (peleaTerminada)
            return;

        peleaTerminada = true;
        temporizadorActivo = false;

        Debug.Log("¡EMPATE! El tiempo se ha agotado");

        Time.timeScale = 0f;

        ControladorMusicaPelea musicaPelea = ControladorMusicaPelea.ObtenerInstancia();
        if (musicaPelea != null)
        {
            musicaPelea.PausarMusica();
            Debug.Log("Música de pelea pausada");
        }

        if (panelResultadoEmpate != null)
        {
            panelResultadoEmpate.SetActive(true);

            if (textoResultado != null)
            {
                textoResultado.text = "¡EMPATE!\n\nEl tiempo se ha agotado";
            }
        }
    }

    /// <summary>
    /// Declara al ganador de la pelea. Método estático para llamar desde cualquier script.
    /// </summary>
    /// <param name="numeroJugador">1 para Jugador 1, 2 para Jugador 2</param>
    public static void DeclararGanador(int numeroJugador)
    {
        if (instancia == null)
        {
            Debug.LogError("No hay instancia de TemporizadorPelea en la escena!");
            return;
        }

        instancia.MostrarVictoria(numeroJugador);
    }

    /// <summary>
    /// Método interno que maneja la lógica de mostrar la victoria
    /// </summary>
    private void MostrarVictoria(int numeroJugador)
    {
        if (peleaTerminada)
        {
            Debug.LogWarning("La pelea ya había terminado");
            return;
        }

        peleaTerminada = true;
        temporizadorActivo = false;

        string nombreGanador = numeroJugador == 1 ? nombreJugador1 : nombreJugador2;
        Debug.Log($"¡{nombreGanador} ha ganado la pelea!");

        // Pausar el juego
        Time.timeScale = 0f;

        // Pausar la música de pelea
        ControladorMusicaPelea musicaPelea = ControladorMusicaPelea.ObtenerInstancia();
        if (musicaPelea != null)
        {
            musicaPelea.PausarMusica();
            Debug.Log("Música de pelea pausada");
        }

        // Mostrar panel de resultado (reutilizamos el panel de empate)
        if (panelResultadoEmpate != null)
        {
            panelResultadoEmpate.SetActive(true);

            if (textoResultado != null)
            {
                textoResultado.text = $"¡{nombreGanador}\nHA GANADO!\n\n🏆 VICTORIA 🏆";
            }
        }
        else
        {
            Debug.LogError("No se encontró el panel de resultado");
        }
    }

    public void DetenerTemporizador()
    {
        temporizadorActivo = false;
    }

    public void ReanudarTemporizador()
    {
        if (!peleaTerminada)
        {
            temporizadorActivo = true;
        }
    }

    public float ObtenerTiempoRestante()
    {
        return tiempoRestante;
    }

    public bool TiempoAgotado()
    {
        return tiempoRestante <= 0;
    }

    void VolverAlMenu()
    {
        Time.timeScale = 1f;

        ControladorMusicaPelea musicaPelea = ControladorMusicaPelea.ObtenerInstancia();
        if (musicaPelea != null)
        {
            musicaPelea.DestruirInstancia();
            Debug.Log("Música de pelea destruida");
        }

        DatosJuego.LimpiarDatos();

        ControladorSonidos.ObtenerInstancia()?.SonidoBotonAtras();

        SceneManager.LoadScene("MainMenu");
    }

    void OnDestroy()
    {
        // Limpiar la instancia cuando se destruye el objeto
        if (instancia == this)
        {
            instancia = null;
        }
    }
}