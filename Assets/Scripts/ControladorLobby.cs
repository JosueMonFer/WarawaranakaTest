using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControladorLobby : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelHost;
    public GameObject panelCliente;

    [Header("UI Host")]
    public TextMeshProUGUI textoCodigoValor;
    public Button botonCancelarHost;

    [Header("UI Cliente")]
    public TMP_InputField inputFieldCodigo;
    public Button botonUnirse;
    public Button botonAtrasCliente;

    [Header("Estado")]
    public TextMeshProUGUI textoEstado;

    void Start()
    {
        // Configurar según rol
        if (DatosJuego.esHost)
        {
            ConfigurarHost();
        }
        else
        {
            ConfigurarCliente();
        }

        // Configurar botones
        botonCancelarHost?.onClick.AddListener(CancelarYRegresar);
        botonUnirse?.onClick.AddListener(IntentarUnirse);
        botonAtrasCliente?.onClick.AddListener(RegresarAlMenu);

        // Suscribirse a eventos de red
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    void ConfigurarHost()
    {
        panelHost.SetActive(true);
        panelCliente.SetActive(false);

        // Mostrar código
        textoCodigoValor.text = DatosJuego.codigoSala;
        textoEstado.text = "Esperando que se una otro jugador...";

        Debug.Log("Lobby Host - Código: " + DatosJuego.codigoSala);
    }

    void ConfigurarCliente()
    {
        panelHost.SetActive(false);
        panelCliente.SetActive(true);

        textoEstado.text = "Ingresa el código de sala";

        // Enfocar el input field
        inputFieldCodigo.Select();
    }

    void IntentarUnirse()
    {
        string codigo = inputFieldCodigo.text.Trim();

        if (string.IsNullOrEmpty(codigo))
        {
            textoEstado.text = "¡Debes ingresar un código!";
            textoEstado.color = Color.red;
            return;
        }

        if (codigo.Length != 6)
        {
            textoEstado.text = "El código debe tener 6 dígitos";
            textoEstado.color = Color.red;
            return;
        }

        textoEstado.text = "Conectando...";
        textoEstado.color = Color.yellow;

        // Intentar unirse
        MenuMultijugador menu = FindObjectOfType<MenuMultijugador>();
        if (menu != null)
        {
            menu.UnirseConCodigo(codigo);
        }

        // Deshabilitar input mientras conecta
        inputFieldCodigo.interactable = false;
        botonUnirse.interactable = false;
    }

    void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Cliente conectado: {clientId}");

        if (DatosJuego.esHost)
        {
            textoEstado.text = "¡Jugador conectado!";
            textoEstado.color = Color.green;

            // Esperar 2 segundos y pasar a selección de personaje
            Invoke(nameof(IrASeleccionPersonaje), 2f);
        }
        else
        {
            textoEstado.text = "¡Conectado! Esperando al host...";
            textoEstado.color = Color.green;
        }
    }

    void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Cliente desconectado: {clientId}");

        textoEstado.text = "Conexión perdida";
        textoEstado.color = Color.red;

        Invoke(nameof(RegresarAlMenu), 2f);
    }

    void IrASeleccionPersonaje()
    {
        if (DatosJuego.esHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("SeleccionPersonaje", LoadSceneMode.Single);
        }
    }

    void CancelarYRegresar()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        RegresarAlMenu();
    }

    void RegresarAlMenu()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonAtras();

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }

        DatosJuego.LimpiarDatos();
        SceneManager.LoadScene("MainMenu");
    }

    void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
}