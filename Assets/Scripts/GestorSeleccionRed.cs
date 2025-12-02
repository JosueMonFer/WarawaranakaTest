using Unity.Netcode;
using UnityEngine;

public class GestorSeleccionRed : NetworkBehaviour
{
    private static GestorSeleccionRed instancia;

    public NetworkVariable<bool> hostListo = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> clienteListo = new NetworkVariable<bool>(false);
    public NetworkVariable<int> personajeHost = new NetworkVariable<int>(-1);
    public NetworkVariable<int> personajeCliente = new NetworkVariable<int>(-1);
    public NetworkVariable<int> mapaSeleccionado = new NetworkVariable<int>(-1);

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

    public static GestorSeleccionRed ObtenerInstancia()
    {
        return instancia;
    }

    [ServerRpc(RequireOwnership = false)]
    public void MarcarHostListoServerRpc()
    {
        hostListo.Value = true;
        Debug.Log("Host marcado como listo");
    }

    [ServerRpc(RequireOwnership = false)]
    public void MarcarClienteListoServerRpc()
    {
        clienteListo.Value = true;
        Debug.Log("Cliente marcado como listo");
    }

    [ServerRpc(RequireOwnership = false)]
    public void EstablecerPersonajeHostServerRpc(int indice)
    {
        personajeHost.Value = indice;
        Debug.Log($"Personaje del host establecido: {indice}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void EstablecerPersonajeClienteServerRpc(int indice)
    {
        personajeCliente.Value = indice;
        Debug.Log($"Personaje del cliente establecido: {indice}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void EstablecerMapaServerRpc(int indice)
    {
        mapaSeleccionado.Value = indice;
        Debug.Log($"Mapa seleccionado por host: {indice}");
    }

    public bool AmbosJugadoresListos()
    {
        return hostListo.Value && clienteListo.Value;
    }

    public void RestablecerEstados()
    {
        if (IsServer)  // Solo el servidor puede modificar NetworkVariables
        {
            hostListo.Value = false;
            clienteListo.Value = false;
            Debug.Log("Estados de jugadores restablecidos por el servidor");
        }
    }
}