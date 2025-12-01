using TMPro;
using Unity.Netcode;
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

    [Header("UI Multijugador")]
    public TextMeshProUGUI textoRol; // Muestra "Eres el HOST" o "Esperando al host..."
    public GameObject panelBloqueoCliente; // Panel que bloquea la selección del cliente

    private TarjetaMapaSimple mapaSeleccionado;
    private GestorSeleccionRed gestorRed;
    private bool esHost;

    void Start()
    {
        if (tarjetasMapas == null || tarjetasMapas.Length == 0)
        {
            Debug.LogError("No hay tarjetas de mapas asignadas en el inspector!");
            return;
        }

        // Determinar si somos host o cliente
        if (DatosJuego.EsModoMultijugador() && NetworkManager.Singleton != null)
        {
            esHost = NetworkManager.Singleton.IsHost;

            // Intentar obtener el gestor
            StartCoroutine(EsperarGestorRed());
        }
        else
        {
            // Modo single player - funcionalidad normal
            esHost = true; // Tratamos al jugador único como host

            if (panelBloqueoCliente != null)
                panelBloqueoCliente.SetActive(false);
        }

        // Inicializar tarjetas
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

        Debug.Log("GestorSeleccionRed encontrado en SeleccionMapa");

        // Configurar UI según el rol
        ConfigurarUISegunRol();
    }

    void ConfigurarUISegunRol()
    {
        if (esHost)
        {
            // El host puede seleccionar
            if (textoRol != null)
                textoRol.text = "Eres el HOST - Selecciona el mapa";

            if (panelBloqueoCliente != null)
                panelBloqueoCliente.SetActive(false);

            // Suscribirse al cambio de mapa
            if (gestorRed != null)
            {
                gestorRed.mapaSeleccionado.OnValueChanged += OnMapaCambiadoPorHost;
            }
        }
        else
        {
            // El cliente solo espera
            if (textoRol != null)
                textoRol.text = "Esperando a que el HOST seleccione el mapa...";

            if (panelBloqueoCliente != null)
                panelBloqueoCliente.SetActive(true);

            // Deshabilitar todas las tarjetas para el cliente
            foreach (var tarjeta in tarjetasMapas)
            {
                if (tarjeta != null && tarjeta.botonSeleccionar != null)
                {
                    tarjeta.botonSeleccionar.interactable = false;
                }
            }

            // Suscribirse al cambio de mapa
            if (gestorRed != null)
            {
                gestorRed.mapaSeleccionado.OnValueChanged += OnMapaCambiadoPorHost;
            }
        }
    }

    void OnDestroy()
    {
        // Desuscribirse de eventos
        if (gestorRed != null)
        {
            gestorRed.mapaSeleccionado.OnValueChanged -= OnMapaCambiadoPorHost;
        }
    }

    void OnMapaCambiadoPorHost(int valorAnterior, int valorNuevo)
    {
        if (valorNuevo >= 0 && valorNuevo < tarjetasMapas.Length)
        {
            Debug.Log($"Mapa sincronizado del host: índice {valorNuevo}");

            // Seleccionar automáticamente el mapa que eligió el host
            SeleccionarMapa(tarjetasMapas[valorNuevo]);

            // Si somos el cliente, actualizar el texto
            if (!esHost && textoRol != null)
            {
                textoRol.text = $"El host seleccionó: {tarjetasMapas[valorNuevo].ObtenerNombre()}";
            }
        }
    }

    public void SeleccionarMapa(TarjetaMapaSimple tarjeta)
    {
        // En multijugador, solo el host puede seleccionar manualmente
        if (DatosJuego.EsModoMultijugador() && !esHost)
        {
            // El cliente no puede seleccionar, solo recibe la selección del host
            return;
        }

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

        // Si somos el host en multijugador, sincronizar la selección
        if (DatosJuego.EsModoMultijugador() && esHost && gestorRed != null)
        {
            gestorRed.EstablecerMapaServerRpc(tarjeta.ObtenerIndiceMapa());
        }
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