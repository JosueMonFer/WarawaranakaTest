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
    public Color colorAdvertencia = Color.yellow; // Cuando quedan 30 segundos
    public Color colorCritico = Color.red; // Cuando quedan 10 segundos

    private float tiempoRestante;
    private bool temporizadorActivo = true;
    private bool empateDeclarado = false;

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
        if (!temporizadorActivo || empateDeclarado)
            return;

        tiempoRestante -= Time.deltaTime;

        // Actualizar el texto del temporizador
        ActualizarTextoTemporizador();

        // Cambiar color segun el tiempo restante
        ActualizarColorTemporizador();

        // Verificar si el tiempo se acabo
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
            // Parpadeo en rojo cuando quedan 10 segundos
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
        if (empateDeclarado)
            return;

        empateDeclarado = true;
        temporizadorActivo = false;

        Debug.Log("¡EMPATE! El tiempo se ha agotado");

        // Pausar el juego
        Time.timeScale = 0f;

        // Pausar la música de pelea
        ControladorMusicaPelea musicaPelea = ControladorMusicaPelea.ObtenerInstancia();
        if (musicaPelea != null)
        {
            musicaPelea.PausarMusica();
            Debug.Log("Música de pelea pausada");
        }
        else
        {
            Debug.LogWarning("No se encontró ControladorMusicaPelea");
        }

        // Mostrar panel de empate
        if (panelResultadoEmpate != null)
        {
            panelResultadoEmpate.SetActive(true);

            if (textoResultado != null)
            {
                textoResultado.text = "¡EMPATE!\n\nEl tiempo se ha agotado";
            }
        }
    }

    public void DetenerTemporizador()
    {
        temporizadorActivo = false;
    }

    public void ReanudarTemporizador()
    {
        if (!empateDeclarado)
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
        // Restaurar el tiempo normal PRIMERO
        Time.timeScale = 1f;

        // Detener y destruir la música de pelea
        ControladorMusicaPelea musicaPelea = ControladorMusicaPelea.ObtenerInstancia();
        if (musicaPelea != null)
        {
            musicaPelea.DestruirInstancia();
            Debug.Log("Música de pelea destruida");
        }

        // Limpiar datos
        DatosJuego.LimpiarDatos();

        // Reproducir sonido
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonAtras();

        // Volver al menu principal
        SceneManager.LoadScene("MainMenu");
    }
}