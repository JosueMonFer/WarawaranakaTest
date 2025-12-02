using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControladorSeleccion : MonoBehaviour
{
    [Header("Tarjetas")]
    public TarjetaPersonaje[] tarjetas;

    [Header("Boton Listo")]
    public Button botonListo;

    [Header("Boton Atras")]
    public Button botonAtras;

    private TarjetaPersonaje tarjetaSeleccionada;

    void Start()
    {
        // Inicializar todas las tarjetas
        foreach (var tarjeta in tarjetas)
        {
            tarjeta.InicializarTarjeta(this);
        }

        // Configurar botón listo
        botonListo.interactable = false;
        botonListo.onClick.AddListener(ConfirmarSeleccion);

        // Configurar botón atrás (opcional)
        if (botonAtras != null)
        {
            botonAtras.onClick.AddListener(RegresarAlMenu);
        }
    }

    public void SeleccionarPersonaje(TarjetaPersonaje tarjeta)
    {
        // El sonido se reproduce en la tarjeta misma

        // Desactivar la tarjeta anterior
        if (tarjetaSeleccionada != null)
        {
            tarjetaSeleccionada.DesactivarSeleccion();
        }

        // Activar la nueva tarjeta
        tarjetaSeleccionada = tarjeta;
        tarjetaSeleccionada.ActivarSeleccion();

        // Activar botón listo
        botonListo.interactable = true;

        Debug.Log("Personaje seleccionado: " + tarjeta.ObtenerNombre());
    }

    public void ConfirmarSeleccion()
    {
        if (tarjetaSeleccionada != null)
        {
            // Reproducir sonido específico del botón Listo
            ControladorSonidos.ObtenerInstancia()?.SonidoBotonListo();

            DatosJuego.personajeSeleccionado = tarjetaSeleccionada.ObtenerNombre();
            DatosJuego.indicePersonajeSeleccionado = tarjetaSeleccionada.indicePersonaje;

            Debug.Log("¡Confirmado! Personaje: " + tarjetaSeleccionada.ObtenerNombre());

            // Cargar escena de selección de mapa
            SceneManager.LoadScene("SeleccionMapa");
        }
    }

    public void RegresarAlMenu()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonAtras();
        SceneManager.LoadScene("MainMenu");
    }
}