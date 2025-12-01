using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMultijugador : MonoBehaviour
{
    private static MenuMultijugador instancia;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);

            // Hacer que NetworkManager persista
            if (NetworkManager.Singleton != null)
            {
                DontDestroyOnLoad(NetworkManager.Singleton.gameObject);
            }

            // Limpiar EventSystems duplicados
            LimpiarEventSystems();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LimpiarEventSystems()
    {
        UnityEngine.EventSystems.EventSystem[] eventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();

        if (eventSystems.Length > 1)
        {
            Debug.LogWarning($"Detectados {eventSystems.Length} EventSystems. Limpiando duplicados...");

            // Mantener solo el primero
            for (int i = 1; i < eventSystems.Length; i++)
            {
                Destroy(eventSystems[i].gameObject);
            }
        }
    }

    // IMPORTANTE: Estas funciones DEBEN ser públicas para aparecer en el Inspector
    public void IniciarComoHost()
    {
        Debug.Log("IniciarComoHost llamado");

        ControladorSonidos.ObtenerInstancia()?.SonidoBotonComenzar();

        // Configurar modo multijugador como host (CORREGIDO - ahora funciona)
        DatosJuego.ConfigurarMultijugador(true);

        // Generar código de sala de 6 dígitos (CORREGIDO - acceso directo)
        DatosJuego.codigoSala = Random.Range(100000, 999999).ToString();

        Debug.Log("Código de sala generado: " + DatosJuego.codigoSala);

        // Verificar que NetworkManager existe
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton es null! Asegúrate de tener un NetworkManager en la escena.");
            return;
        }

        // Iniciar como Host
        bool exito = NetworkManager.Singleton.StartHost();

        if (exito)
        {
            Debug.Log("Host iniciado exitosamente");
            // Ir a la escena de lobby
            SceneManager.LoadScene("Lobby");
        }
        else
        {
            Debug.LogError("Error al iniciar como Host");
        }
    }

    public void PrepararUnirse()
    {
        Debug.Log("PrepararUnirse llamado");

        ControladorSonidos.ObtenerInstancia()?.SonidoBotonComenzar();

        // Configurar modo multijugador como cliente (CORREGIDO - ahora funciona)
        DatosJuego.ConfigurarMultijugador(false);

        // Ir a la escena de lobby para ingresar código
        SceneManager.LoadScene("Lobby");
    }

    public void UnirseConCodigo(string codigo)
    {
        Debug.Log("UnirseConCodigo llamado con código: " + codigo);

        // Validar código
        if (string.IsNullOrEmpty(codigo) || codigo.Length != 6)
        {
            Debug.LogError("Código inválido. Debe tener 6 dígitos.");
            return;
        }

        // Guardar código (CORREGIDO - acceso directo)
        DatosJuego.codigoSala = codigo;

        // Verificar que NetworkManager existe
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton es null! Asegúrate de tener un NetworkManager en la escena.");
            return;
        }

        // En una implementación real, aquí convertirías el código en una IP
        // Por ahora usamos localhost para pruebas locales
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null)
        {
            transport.ConnectionData.Address = "127.0.0.1"; // localhost para pruebas
            transport.ConnectionData.Port = 7777;
            Debug.Log("Configurado para conectar a: 127.0.0.1:7777");
        }

        // Iniciar como Cliente
        bool exito = NetworkManager.Singleton.StartClient();

        if (exito)
        {
            Debug.Log("Intentando unirse con código: " + codigo);
        }
        else
        {
            Debug.LogError("Error al iniciar como Cliente");
        }
    }

    // Función para desconectar
    public void Desconectar()
    {
        Debug.Log("=== MenuMultijugador.Desconectar llamado ===");

        if (NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.Shutdown();
                Debug.Log("Host desconectado");
            }
            else if (NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.Shutdown();
                Debug.Log("Cliente desconectado");
            }

            // Destruir NetworkManager
            Destroy(NetworkManager.Singleton.gameObject);
            Debug.Log("NetworkManager destruido");
        }

        // Limpiar datos
        DatosJuego.LimpiarDatos();

        // CRÍTICO: Destruir este MenuMultijugador para que se recree al volver al menú
        if (instancia == this)
        {
            instancia = null;
            Destroy(gameObject);
            Debug.Log("MenuMultijugador destruido");
        }
    }

    // Función de prueba para verificar que el script funciona
    public void FuncionDePrueba()
    {
        Debug.Log("¡El script funciona! Las funciones deberían aparecer.");
    }

    void OnDestroy()
    {
        // Limpiar al destruir
        if (instancia == this)
        {
            instancia = null;
        }
    }
}