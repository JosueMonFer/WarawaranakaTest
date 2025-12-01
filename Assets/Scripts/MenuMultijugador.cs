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

    public void IniciarComoHost()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonComenzar();

        DatosJuego.esHost = true;

        // Generar código de sala de 6 dígitos
        DatosJuego.codigoSala = Random.Range(100000, 999999).ToString();

        Debug.Log("Código de sala generado: " + DatosJuego.codigoSala);

        // Iniciar como Host
        NetworkManager.Singleton.StartHost();

        // Ir a la escena de lobby
        SceneManager.LoadScene("LobbyMultijugador");
    }

    public void PrepararUnirse()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonComenzar();

        DatosJuego.esHost = false;

        // Ir a la escena de lobby para ingresar código
        SceneManager.LoadScene("LobbyMultijugador");
    }

    public void UnirseConCodigo(string codigo)
    {
        DatosJuego.codigoSala = codigo;

        // En una implementación real, aquí convertirías el código en una IP
        // Por ahora usamos localhost

        NetworkManager.Singleton.StartClient();

        Debug.Log("Intentando unirse con código: " + codigo);
    }
}