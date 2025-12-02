using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControladorSeleccionMapa : MonoBehaviour
{
    [Header("Tarjetas de Mapas")]
    public TarjetaMapaSimple[] tarjetasMapas;

    [Header("Boton Comenzar Pelea")]
    public Button botonComenzarPelea;

    [Header("Boton Atras")]
    public Button botonAtras;

    private TarjetaMapaSimple mapaSeleccionado;

    void Start()
    {
        if (tarjetasMapas == null || tarjetasMapas.Length == 0)
        {
            Debug.LogError("No hay tarjetas de mapas asignadas en el inspector!");
            return;
        }

        foreach (var tarjeta in tarjetasMapas)
        {
            if (tarjeta != null)
            {
                tarjeta.InicializarTarjeta(this);
            }
            else
            {
                Debug.LogWarning("Hay una tarjeta nula en el array de tarjetasMapas");
            }
        }

        if (botonComenzarPelea != null)
        {
            botonComenzarPelea.interactable = false;
            botonComenzarPelea.onClick.RemoveAllListeners();
            botonComenzarPelea.onClick.AddListener(ComenzarPelea);
        }
        else
        {
            Debug.LogError("El boton 'botonComenzarPelea' no esta asignado!");
        }

        if (botonAtras != null)
        {
            botonAtras.onClick.AddListener(RegresarASeleccionPersonaje);
        }
    }

    public void SeleccionarMapa(TarjetaMapaSimple tarjeta)
    {
        if (mapaSeleccionado != null)
        {
            mapaSeleccionado.DesactivarSeleccion();
        }

        mapaSeleccionado = tarjeta;
        mapaSeleccionado.ActivarSeleccion();

        if (botonComenzarPelea != null)
        {
            botonComenzarPelea.interactable = true;
        }

        Debug.Log("Mapa seleccionado: " + tarjeta.ObtenerNombre());
    }

    public void ComenzarPelea()
    {
        if (mapaSeleccionado != null)
        {
            // Reproducir sonido
            ControladorSonidos.ObtenerInstancia()?.SonidoComenzarMapa(mapaSeleccionado.ObtenerIndiceMapa());

            Debug.Log("Comenzando pelea en: " + mapaSeleccionado.ObtenerNombre() + "!");

            // Detener música del menú
            ControladorMusica musicaMenu = ControladorMusica.ObtenerInstancia();
            if (musicaMenu != null)
            {
                Debug.Log("Deteniendo musica del menu...");
                musicaMenu.DetenerMusica();
            }
            else
            {
                Debug.LogWarning("No se encontro el ControladorMusica del menu!");
            }

            // Guardar datos del mapa seleccionado
            DatosJuego.mapaSeleccionado = mapaSeleccionado.ObtenerNombre();
            DatosJuego.indiceMapaSeleccionado = mapaSeleccionado.ObtenerIndiceMapa();

            // Cargar pantalla de carga (que luego cargará la escena de pelea)
            SceneManager.LoadScene("PantallaCarga");
        }
        else
        {
            Debug.LogWarning("No hay mapa seleccionado!");
        }
    }

    public void RegresarASeleccionPersonaje()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonAtras();
        SceneManager.LoadScene("SeleccionPersonaje");
    }
}