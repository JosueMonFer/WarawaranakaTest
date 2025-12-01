using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorMenuPrincipal : MonoBehaviour
{
    [Header("Referencias")]
    public ControladorOpciones controladorOpciones;

    public void IniciarJuego()
    {
        // Reproducir sonido específico de Comenzar
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonComenzar();

        // IMPORTANTE: Configurar modo Single Player
        DatosJuego.ConfigurarSinglePlayer();

        Debug.Log("¡Juego iniciado en modo Single Player!");
        SceneManager.LoadScene("SeleccionPersonaje");
    }

    // NUEVO: Crear partida (Host)
    public void CrearPartida()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonComenzar();

        // Configurar modo multijugador
        DatosJuego.ConfigurarMultiplayer();

        // Configurar como host
        SistemaSalas sistemaSalas = SistemaSalas.ObtenerInstancia();
        if (sistemaSalas != null)
        {
            sistemaSalas.EstablecerComoHost();
        }

        Debug.Log("Creando partida multijugador (Host)");
        SceneManager.LoadScene("Lobby");
    }

    // NUEVO: Unirse a partida (Cliente)
    public void UnirseAPartida()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonComenzar();

        // Configurar modo multijugador
        DatosJuego.ConfigurarMultiplayer();

        // Configurar como cliente
        SistemaSalas sistemaSalas = SistemaSalas.ObtenerInstancia();
        if (sistemaSalas != null)
        {
            sistemaSalas.EstablecerComoCliente();
        }

        Debug.Log("Uniéndose a partida multijugador (Cliente)");
        SceneManager.LoadScene("Lobby");
    }

    public void AbrirAjustes()
    {
        // Reproducir sonido específico de Ajustes
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonAjustes();

        Debug.Log("Abriendo ajustes...");

        // Abrir panel de opciones
        if (controladorOpciones != null)
        {
            controladorOpciones.AbrirOpciones();
        }
        else
        {
            Debug.LogError("ControladorOpciones no está asignado en el inspector!");
        }
    }

    public void SalirDelJuego()
    {
        // Reproducir sonido específico de Salir
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonSalir();

        Debug.Log("Saliendo del juego...");

        // Cerrar el juego completamente
        Application.Quit();

#if UNITY_EDITOR
        // En el editor de Unity, detener el modo Play
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}