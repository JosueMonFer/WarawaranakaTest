using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControladorLobby : MonoBehaviour
{
    [Header("Panel Host")]
    public GameObject panelHost;
    public TextMeshProUGUI textoCodigoSala;
    public TextMeshProUGUI textoEstadoHost;
    public Button botonCancelarHost;

    [Header("Panel Cliente")]
    public GameObject panelCliente;
    public TMP_InputField inputCodigoSala;
    public Button botonUnirse;
    public Button botonAtrasCliente;
    public TextMeshProUGUI textoErrorCliente;

    private bool esperandoCliente = false;
    private MenuMultijugador menuMultijugador;

    void Start()
    {
        // Obtener referencia al MenuMultijugador
        menuMultijugador = FindObjectOfType<MenuMultijugador>();

        // Configurar botones solo si existen
        if (botonCancelarHost != null)
            botonCancelarHost.onClick.AddListener(CancelarHost);

        if (botonAtrasCliente != null)
            botonAtrasCliente.onClick.AddListener(CancelarCliente);

        if (botonUnirse != null)
            botonUnirse.onClick.AddListener(UnirseASala);

        // Determinar qué panel mostrar basado en si es host o cliente
        if (DatosJuego.EsHost())
        {
            MostrarPanelHost();
        }
        else
        {
            MostrarPanelCliente();
        }
    }

    void MostrarPanelHost()
    {
        Debug.Log("Mostrando panel de Host");

        // Activar/desactivar paneles
        if (panelHost != null)
            panelHost.SetActive(true);
        else
            Debug.LogError("panelHost es null! Asigna el panel en el Inspector");

        if (panelCliente != null)
            panelCliente.SetActive(false);

        // Mostrar código de sala
        if (textoCodigoSala != null)
        {
            textoCodigoSala.text = DatosJuego.codigoSala;
        }
        else
        {
            Debug.LogError("textoCodigoSala es null! Asigna el texto en el Inspector");
        }

        // Mostrar estado
        if (textoEstadoHost != null)
        {
            textoEstadoHost.text = "Esperando jugador...";
        }
        else
        {
            Debug.LogError("textoEstadoHost es null! Asigna el texto en el Inspector");
        }

        esperandoCliente = true;

        // Suscribirse al evento de conexión de cliente
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClienteConectado;
        }
        else
        {
            Debug.LogError("NetworkManager.Singleton es null!");
        }
    }

    void MostrarPanelCliente()
    {
        Debug.Log("Mostrando panel de Cliente");

        // Activar/desactivar paneles
        if (panelHost != null)
            panelHost.SetActive(false);

        if (panelCliente != null)
            panelCliente.SetActive(true);
        else
            Debug.LogError("panelCliente es null! Asigna el panel en el Inspector");

        // Limpiar campos
        if (textoErrorCliente != null)
        {
            textoErrorCliente.text = "";
        }
        else
        {
            Debug.LogError("textoErrorCliente es null! Asigna el texto en el Inspector");
        }

        if (inputCodigoSala != null)
        {
            inputCodigoSala.text = "";
        }
        else
        {
            Debug.LogError("inputCodigoSala es null! Asigna el input field en el Inspector");
        }
    }

    void OnClienteConectado(ulong clientId)
    {
        // Si somos el host y se conectó un cliente (no nosotros mismos)
        if (NetworkManager.Singleton.IsHost && clientId != NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log($"Cliente conectado con ID: {clientId}");

            if (textoEstadoHost != null)
            {
                textoEstadoHost.text = "¡Jugador conectado!";
            }

            esperandoCliente = false;

            // Esperar 1 segundo y luego ir a selección de personaje
            Invoke("IrASeleccionPersonaje", 1f);
        }
    }

    void UnirseASala()
    {
        if (inputCodigoSala == null)
        {
            Debug.LogError("inputCodigoSala es null!");
            return;
        }

        string codigo = inputCodigoSala.text.Trim();

        if (string.IsNullOrEmpty(codigo) || codigo.Length != 6)
        {
            if (textoErrorCliente != null)
            {
                textoErrorCliente.text = "Código inválido (debe ser 6 dígitos)";
                textoErrorCliente.color = Color.red;
            }
            return;
        }

        if (textoErrorCliente != null)
        {
            textoErrorCliente.text = "Conectando...";
            textoErrorCliente.color = Color.yellow;
        }

        // Usar MenuMultijugador para unirse
        if (menuMultijugador != null)
        {
            menuMultijugador.UnirseConCodigo(codigo);
        }
        else
        {
            Debug.LogError("No se encontró MenuMultijugador!");
            return;
        }

        // Suscribirse al evento de conexión exitosa
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnConexionExitosa;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnConexionFallida;
        }
    }

    void OnConexionExitosa(ulong clientId)
    {
        // Si somos nosotros los que nos conectamos
        if (clientId == NetworkManager.Singleton.LocalClientId && !NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Conexión exitosa como cliente");

            if (textoErrorCliente != null)
            {
                textoErrorCliente.text = "¡Conectado!";
                textoErrorCliente.color = Color.green;
            }

            // Ir a selección de personaje
            Invoke("IrASeleccionPersonaje", 1f);
        }
    }

    void OnConexionFallida(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId && !NetworkManager.Singleton.IsHost)
        {
            if (textoErrorCliente != null)
            {
                textoErrorCliente.text = "No se pudo conectar a la sala";
                textoErrorCliente.color = Color.red;
            }
        }
    }

    void IrASeleccionPersonaje()
    {
        // Cargar escena de selección de personaje
        SceneManager.LoadScene("SeleccionPersonaje");
    }

    void CancelarHost()
    {
        Debug.Log("=== CancelarHost llamado ===");

        ControladorSonidos.ObtenerInstancia()?.SonidoBotonAtras();

        // Desuscribirse de eventos PRIMERO
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClienteConectado;
            Debug.Log("Eventos desuscritos");
        }

        // Desconectar usando MenuMultijugador
        if (menuMultijugador != null)
        {
            Debug.Log("Desconectando via MenuMultijugador");
            menuMultijugador.Desconectar();
        }
        else if (NetworkManager.Singleton != null)
        {
            Debug.Log("Desconectando via NetworkManager.Shutdown");
            NetworkManager.Singleton.Shutdown();
        }

        RegresarAlMenu();
    }

    void CancelarCliente()
    {
        Debug.Log("=== CancelarCliente llamado ===");

        ControladorSonidos.ObtenerInstancia()?.SonidoBotonAtras();

        // Desuscribirse de eventos
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnConexionExitosa;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnConexionFallida;
            Debug.Log("Eventos de cliente desuscritos");
        }

        // Desconectar usando MenuMultijugador
        if (menuMultijugador != null)
        {
            Debug.Log("Cliente desconectando via MenuMultijugador");
            menuMultijugador.Desconectar();
        }
        else if (NetworkManager.Singleton != null)
        {
            Debug.Log("Cliente desconectando via NetworkManager.Shutdown");
            NetworkManager.Singleton.Shutdown();
        }

        RegresarAlMenu();
    }

    void RegresarAlMenu()
    {
        Debug.Log("=== RegresarAlMenu llamado ===");

        ControladorSonidos.ObtenerInstancia()?.SonidoBotonAtras();

        // NO desconectar aquí, ya se hizo en CancelarHost

        Debug.Log("Cambiando a escena MainMenu");
        SceneManager.LoadScene("MainMenu");
    }

    void OnDestroy()
    {
        // Limpiar suscripciones
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClienteConectado;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnConexionExitosa;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnConexionFallida;
        }
    }
}