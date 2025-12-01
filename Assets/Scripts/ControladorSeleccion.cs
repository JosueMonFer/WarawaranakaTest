using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControladorSeleccion : MonoBehaviour
{
    [Header("Tarjetas")]
    public TarjetaPersonaje[] tarjetas;

    [Header("Boton Listo")]
    public Button botonListo;

    [Header("Boton Atras")]
    public Button botonAtras;

    [Header("UI Multijugador")]
    public TextMeshProUGUI textoEstado; // Para mostrar "Esperando al otro jugador..."
    public GameObject panelEspera; // Panel que se muestra mientras se espera

    private TarjetaPersonaje tarjetaSeleccionada;
    private GestorSeleccionRed gestorRed;
    private bool yoEstoyListo = false;

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

        // Configurar botón atrás
        if (botonAtras != null)
        {
            botonAtras.onClick.AddListener(RegresarAlMenu);
        }

        // Ocultar panel de espera al inicio
        if (panelEspera != null)
            panelEspera.SetActive(false);

        // Si es multijugador, obtener gestor de red
        if (DatosJuego.EsModoMultijugador())
        {
            // Intentar obtener el gestor, si no existe, esperar
            StartCoroutine(EsperarGestorRed());
        }
    }

    System.Collections.IEnumerator EsperarGestorRed()
    {
        // Esperar hasta que el GestorSeleccionRed esté disponible
        float tiempoEspera = 0f;
        while (gestorRed == null && tiempoEspera < 5f)
        {
            gestorRed = GestorSeleccionRed.ObtenerInstancia();

            if (gestorRed == null)
            {
                yield return new UnityEngine.WaitForSeconds(0.1f);
                tiempoEspera += 0.1f;
            }
        }

        if (gestorRed == null)
        {
            Debug.LogError("No se pudo encontrar GestorSeleccionRed después de esperar!");
            yield break;
        }

        Debug.Log("GestorSeleccionRed encontrado correctamente");

        // Suscribirse a cambios en las variables de red
        gestorRed.hostListo.OnValueChanged += OnCambioEstadoJugadores;
        gestorRed.clienteListo.OnValueChanged += OnCambioEstadoJugadores;
    }

    void OnDestroy()
    {
        // Desuscribirse de eventos
        if (gestorRed != null)
        {
            gestorRed.hostListo.OnValueChanged -= OnCambioEstadoJugadores;
            gestorRed.clienteListo.OnValueChanged -= OnCambioEstadoJugadores;
        }
    }

    void OnCambioEstadoJugadores(bool valorAnterior, bool valorNuevo)
    {
        VerificarSiAmbosListos();
    }

    void VerificarSiAmbosListos()
    {
        if (gestorRed != null && gestorRed.AmbosJugadoresListos() && yoEstoyListo)
        {
            Debug.Log("¡Ambos jugadores listos! Avanzando a selección de mapa...");

            if (textoEstado != null)
                textoEstado.text = "¡Ambos listos!";

            // Esperar un momento antes de cambiar de escena
            Invoke("IrASeleccionMapa", 1f);
        }
    }

    public void SeleccionarPersonaje(TarjetaPersonaje tarjeta)
    {
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
        if (tarjetaSeleccionada == null) return;

        // Reproducir sonido específico del botón Listo
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonListo();

        // Verificar si es modo multijugador
        if (DatosJuego.EsModoMultijugador() && NetworkManager.Singleton != null)
        {
            yoEstoyListo = true;

            // Deshabilitar botón para que no se pueda presionar de nuevo
            botonListo.interactable = false;

            // Mostrar panel de espera
            if (panelEspera != null)
                panelEspera.SetActive(true);

            if (NetworkManager.Singleton.IsHost)
            {
                // Host guarda su personaje
                DatosJuego.personajeJugador1 = tarjetaSeleccionada.ObtenerNombre();
                DatosJuego.indicePersonajeJugador1 = tarjetaSeleccionada.indicePersonaje;

                if (textoEstado != null)
                    textoEstado.text = "Esperando al otro jugador...";

                Debug.Log("Host confirmó: " + tarjetaSeleccionada.ObtenerNombre());

                // Notificar al servidor (el host es el servidor)
                if (gestorRed != null)
                {
                    gestorRed.EstablecerPersonajeHostServerRpc(tarjetaSeleccionada.indicePersonaje);
                    gestorRed.MarcarHostListoServerRpc();
                }
            }
            else if (NetworkManager.Singleton.IsClient)
            {
                // Cliente guarda su personaje
                DatosJuego.personajeJugador2 = tarjetaSeleccionada.ObtenerNombre();
                DatosJuego.indicePersonajeJugador2 = tarjetaSeleccionada.indicePersonaje;

                if (textoEstado != null)
                    textoEstado.text = "Esperando al otro jugador...";

                Debug.Log("Cliente confirmó: " + tarjetaSeleccionada.ObtenerNombre());

                // Notificar al servidor
                if (gestorRed != null)
                {
                    gestorRed.EstablecerPersonajeClienteServerRpc(tarjetaSeleccionada.indicePersonaje);
                    gestorRed.MarcarClienteListoServerRpc();
                }
            }

            // Verificar si ambos ya están listos
            VerificarSiAmbosListos();
        }
        else
        {
            // Modo single player (código original)
            DatosJuego.personajeSeleccionado = tarjetaSeleccionada.ObtenerNombre();
            DatosJuego.indicePersonajeSeleccionado = tarjetaSeleccionada.indicePersonaje;

            Debug.Log("¡Confirmado! Personaje: " + tarjetaSeleccionada.ObtenerNombre());

            // Cargar escena de selección de mapa directamente
            SceneManager.LoadScene("SeleccionMapa");
        }
    }

    void IrASeleccionMapa()
    {
        SceneManager.LoadScene("SeleccionMapa");
    }

    public void RegresarAlMenu()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonAtras();

        // Si estamos en multijugador, desconectar
        if (DatosJuego.EsModoMultijugador())
        {
            MenuMultijugador menuMulti = FindObjectOfType<MenuMultijugador>();
            if (menuMulti != null)
            {
                menuMulti.Desconectar();
            }
        }

        SceneManager.LoadScene("MainMenu");
    }
}