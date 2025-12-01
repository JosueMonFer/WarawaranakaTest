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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // IMPORTANTE: Estas funciones DEBEN ser públicas para aparecer en el Inspector
    public void IniciarComoHost()
    {
        Debug.Log("IniciarComoHost llamado");

        ControladorSonidos.ObtenerInstancia()?.SonidoBotonComenzar();

        // Configurar modo multijugador como host
        DatosJuego.ConfigurarMultijugador(true);

        // Generar código de sala de 6 dígitos
        DatosJuego.codigoSala = Random.Range(100000, 999999).ToString();

        Debug.Log("Código de sala generado: " + DatosJuego.codigoSala);

        // Verificar que NetworkManager existe
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton es null!");
            return;
        }

        // Iniciar como Host
        NetworkManager.Singleton.StartHost();

        // Ir a la escena de lobby
        SceneManager.LoadScene("LobbyMultijugador");
    }

    public void PrepararUnirse()
    {
        Debug.Log("PrepararUnirse llamado");

        ControladorSonidos.ObtenerInstancia()?.SonidoBotonComenzar();

        // Configurar modo multijugador como cliente
        DatosJuego.ConfigurarMultijugador(false);

        // Ir a la escena de lobby para ingresar código
        SceneManager.LoadScene("LobbyMultijugador");
    }

    public void UnirseConCodigo(string codigo)
    {
        Debug.Log("UnirseConCodigo llamado con código: " + codigo);

        DatosJuego.codigoSala = codigo;

        // En una implementación real, aquí convertirías el código en una IP
        // Por ahora usamos localhost

        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton es null!");
            return;
        }

        NetworkManager.Singleton.StartClient();

        Debug.Log("Intentando unirse con código: " + codigo);
    }

    // Función de prueba para verificar que el script funciona
    public void FuncionDePrueba()
    {
        Debug.Log("¡El script funciona! Las funciones deberían aparecer.");
    }
}