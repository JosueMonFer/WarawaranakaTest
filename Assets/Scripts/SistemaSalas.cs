using Unity.Netcode;
using UnityEngine;

public class SistemaSalas : MonoBehaviour
{
    private static SistemaSalas instancia;

    [Header("Configuración")]
    public string codigoSalaActual = "";
    public bool esHost = false;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static SistemaSalas ObtenerInstancia()
    {
        return instancia;
    }

    public string GenerarCodigoSala()
    {
        // Generar código aleatorio de 6 dígitos
        codigoSalaActual = Random.Range(100000, 999999).ToString();
        return codigoSalaActual;
    }

    public void EstablecerCodigoSala(string codigo)
    {
        codigoSalaActual = codigo;
    }

    public string ObtenerCodigoSala()
    {
        return codigoSalaActual;
    }

    public void EstablecerComoHost()
    {
        esHost = true;
        Debug.Log("Configurado como HOST");
    }

    public void EstablecerComoCliente()
    {
        esHost = false;
        Debug.Log("Configurado como CLIENTE");
    }

    public bool EsHost()
    {
        return esHost;
    }

    public void IniciarComoHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log($"Host iniciado con código de sala: {codigoSalaActual}");
    }

    public void ConectarComoCliente()
    {
        // Aquí podrías configurar la IP si usas conexión por red local
        NetworkManager.Singleton.StartClient();
        Debug.Log($"Intentando conectar a sala: {codigoSalaActual}");
    }

    public void Desconectar()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Desconectado del servidor");
        }
    }
}