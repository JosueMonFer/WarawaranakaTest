using UnityEngine;
using UnityEngine.InputSystem; // Para el nuevo Input System
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControladorMenuPausa : MonoBehaviour
{
    [Header("Panel de Pausa")]
    public GameObject panelPausa;

    [Header("Referencias")]
    public ControladorOpciones controladorOpciones;

    [Header("Botones")]
    public Button botonReanudar;
    public Button botonOpciones;
    public Button botonMenuPrincipal;

    private bool estaPausado = false;

    void Start()
    {
        // Ocultar panel de pausa al inicio
        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }

        // Configurar botones
        if (botonReanudar != null)
            botonReanudar.onClick.AddListener(ReanudarJuego);

        if (botonOpciones != null)
            botonOpciones.onClick.AddListener(AbrirOpciones);

        if (botonMenuPrincipal != null)
            botonMenuPrincipal.onClick.AddListener(VolverAlMenuPrincipal);
    }

    void Update()
    {
        // Detectar tecla ESC o P para pausar/reanudar
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (estaPausado)
            {
                ReanudarJuego();
            }
            else
            {
                PausarJuego();
            }
        }
    }

    public void PausarJuego()
    {
        estaPausado = true;
        Time.timeScale = 0f; // Detener el tiempo del juego

        if (panelPausa != null)
        {
            panelPausa.SetActive(true);
        }

        // Pausar música de pelea
        ControladorMusicaPelea musicaPelea = ControladorMusicaPelea.ObtenerInstancia();
        if (musicaPelea != null)
        {
            musicaPelea.PausarMusica();
        }

        Debug.Log("Juego pausado");
    }

    public void ReanudarJuego()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonOpciones();

        estaPausado = false;
        Time.timeScale = 1f; // Reanudar el tiempo del juego

        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }

        // Reanudar música de pelea
        ControladorMusicaPelea musicaPelea = ControladorMusicaPelea.ObtenerInstancia();
        if (musicaPelea != null)
        {
            musicaPelea.ReanudarMusica();
        }

        Debug.Log("Juego reanudado");
    }

    public void AbrirOpciones()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonAjustes();

        if (controladorOpciones != null)
        {
            controladorOpciones.AbrirOpciones();
        }
        else
        {
            Debug.LogError("ControladorOpciones no está asignado en el inspector!");
        }
    }

    public void VolverAlMenuPrincipal()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonAtras();

        // Reanudar el tiempo antes de cambiar de escena
        Time.timeScale = 1f;

        // Destruir la música de pelea
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

        Debug.Log("Volviendo al menú principal...");
        SceneManager.LoadScene("MainMenu");
    }
}