using System.Collections;
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
    public TextMeshProUGUI textoEstado;
    public GameObject panelEspera;

    private TarjetaPersonaje tarjetaSeleccionada;
    private GestorSeleccionRed gestorRed;
    private bool yoEstoyListo = false;
    private bool esModoMultijugador;
    private bool yaVerifiqueTransicion = false; // NUEVO: Evita verificaciones múltiples

    void Start()
    {
        // Determinar si estamos en modo multijugador
        esModoMultijugador = DatosJuego.EsModoMultijugador();

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
        if (esModoMultijugador)
        {
            StartCoroutine(EsperarGestorRed());
        }
    }

    IEnumerator EsperarGestorRed()
    {
        float tiempoEspera = 0f;
        while (gestorRed == null && tiempoEspera < 5f)
        {
            gestorRed = GestorSeleccionRed.ObtenerInstancia();

            if (gestorRed == null)
            {
                yield return new WaitForSeconds(0.1f);
                tiempoEspera += 0.1f;
            }
        }

        if (gestorRed == null)
        {
            Debug.LogError("No se pudo encontrar GestorSeleccionRed después de esperar!");
            yield break;
        }

        Debug.Log("GestorSeleccionRed encontrado correctamente");

        // Restablecer estados al iniciar
        if (NetworkManager.Singleton.IsServer)
        {
            gestorRed.RestablecerEstados();
        }

        // Esperar un frame para asegurar que las variables de red estén inicializadas
        yield return null;

        // Suscribirse a cambios en las variables de red
        gestorRed.hostListo.OnValueChanged += OnCambioEstadoJugadores;
        gestorRed.clienteListo.OnValueChanged += OnCambioEstadoJugadores;

        // Verificar estado inicial (por si ya hay datos)
        VerificarSiAmbosListos();
    }

    void OnDestroy()
    {
        if (gestorRed != null)
        {
            gestorRed.hostListo.OnValueChanged -= OnCambioEstadoJugadores;
            gestorRed.clienteListo.OnValueChanged -= OnCambioEstadoJugadores;
        }
    }

    void OnCambioEstadoJugadores(bool valorAnterior, bool valorNuevo)
    {
        Debug.Log($"Cambio de estado detectado - Host: {gestorRed.hostListo.Value}, Cliente: {gestorRed.clienteListo.Value}");
        VerificarSiAmbosListos();
    }

    void VerificarSiAmbosListos()
    {
        // Evitar verificaciones múltiples
        if (yaVerifiqueTransicion)
            return;

        if (gestorRed != null && gestorRed.AmbosJugadoresListos())
        {
            Debug.Log("¡Ambos jugadores listos! Avanzando a selección de mapa...");
            yaVerifiqueTransicion = true; // IMPORTANTE: Marcar que ya verificamos

            if (textoEstado != null)
                textoEstado.text = "¡Ambos listos! Cargando...";

            // Esperar un momento antes de cambiar de escena
            Invoke(nameof(IrASeleccionMapa), 1f);
        }
        else if (yoEstoyListo)
        {
            // Actualizar texto mientras esperamos
            if (textoEstado != null)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    textoEstado.text = gestorRed.clienteListo.Value ?
                        "¡Ambos listos!" : "Esperando al cliente...";
                }
                else
                {
                    textoEstado.text = gestorRed.hostListo.Value ?
                        "¡Ambos listos!" : "Esperando al host...";
                }
            }
        }
    }

    public void SeleccionarPersonaje(TarjetaPersonaje tarjeta)
    {
        // Solo permitir selección si no estamos listos
        if (yoEstoyListo)
            return;

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
        if (yoEstoyListo) return; // Evitar confirmar dos veces

        // Reproducir sonido
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonListo();

        yoEstoyListo = true;
        botonListo.interactable = false;

        if (esModoMultijugador && NetworkManager.Singleton != null)
        {
            // Mostrar panel de espera
            if (panelEspera != null)
                panelEspera.SetActive(true);

            if (NetworkManager.Singleton.IsHost)
            {
                // Host guarda su personaje
                DatosJuego.personajeJugador1 = tarjetaSeleccionada.ObtenerNombre();
                DatosJuego.indicePersonajeJugador1 = tarjetaSeleccionada.indicePersonaje;

                if (textoEstado != null)
                    textoEstado.text = "Esperando al cliente...";

                Debug.Log($"Host confirmó: {tarjetaSeleccionada.ObtenerNombre()}");

                // Notificar al servidor
                if (gestorRed != null)
                {
                    gestorRed.EstablecerPersonajeHostServerRpc(tarjetaSeleccionada.indicePersonaje);
                    gestorRed.MarcarHostListoServerRpc();

                    // IMPORTANTE: Verificar inmediatamente después de marcar
                    StartCoroutine(VerificarDespuesDeMoment());
                }
            }
            else if (NetworkManager.Singleton.IsClient)
            {
                // Cliente guarda su personaje
                DatosJuego.personajeJugador2 = tarjetaSeleccionada.ObtenerNombre();
                DatosJuego.indicePersonajeJugador2 = tarjetaSeleccionada.indicePersonaje;

                if (textoEstado != null)
                    textoEstado.text = "Esperando al host...";

                Debug.Log($"Cliente confirmó: {tarjetaSeleccionada.ObtenerNombre()}");

                // Notificar al servidor
                if (gestorRed != null)
                {
                    gestorRed.EstablecerPersonajeClienteServerRpc(tarjetaSeleccionada.indicePersonaje);
                    gestorRed.MarcarClienteListoServerRpc();

                    // IMPORTANTE: Verificar inmediatamente después de marcar
                    StartCoroutine(VerificarDespuesDeMoment());
                }
            }
        }
        else
        {
            // Modo single player
            DatosJuego.personajeSeleccionado = tarjetaSeleccionada.ObtenerNombre();
            DatosJuego.indicePersonajeSeleccionado = tarjetaSeleccionada.indicePersonaje;

            Debug.Log($"¡Confirmado! Personaje: {tarjetaSeleccionada.ObtenerNombre()}");
            SceneManager.LoadScene("SeleccionMapa");
        }
    }

    // NUEVO: Verificar después de un breve momento para dar tiempo a la sincronización
    IEnumerator VerificarDespuesDeMoment()
    {
        yield return new WaitForSeconds(0.2f);
        VerificarSiAmbosListos();
    }

    void IrASeleccionMapa()
    {
        Debug.Log("Cambiando a escena de selección de mapa...");

        // Solo el host cambia la escena en multijugador
        if (esModoMultijugador && NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("SeleccionMapa", LoadSceneMode.Single);
            }
            // El cliente NO hace nada, la transición la maneja el host
        }
        else
        {
            SceneManager.LoadScene("SeleccionMapa");
        }
    }

    public void RegresarAlMenu()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonAtras();

        if (esModoMultijugador)
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