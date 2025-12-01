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

        Debug.Log("¡Juego iniciado!");
        SceneManager.LoadScene("SeleccionPersonaje");
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