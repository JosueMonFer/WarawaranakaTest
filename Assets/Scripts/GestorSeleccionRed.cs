using Unity.Netcode;
using UnityEngine;

public class GestorSeleccionRed : NetworkBehaviour
{
    private static GestorSeleccionRed instancia;

    // Variables de red sincronizadas
    public NetworkVariable<int> personajeHost = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> personajeCliente = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> hostListo = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> clienteListo = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> mapaSeleccionado = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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

    // El cliente llama esto para informar su selección al servidor
    [ServerRpc(RequireOwnership = false)]
    public void EstablecerPersonajeClienteServerRpc(int indicePersonaje, ServerRpcParams rpcParams = default)
    {
        personajeCliente.Value = indicePersonaje;
        Debug.Log($"Cliente seleccionó personaje índice: {indicePersonaje}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void EstablecerPersonajeHostServerRpc(int indicePersonaje)
    {
        personajeHost.Value = indicePersonaje;
        Debug.Log($"Host seleccionó personaje índice: {indicePersonaje}");
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
    public void EstablecerMapaServerRpc(int indiceMapa)
    {
        mapaSeleccionado.Value = indiceMapa;
        Debug.Log($"Host seleccionó mapa índice: {indiceMapa}");
    }

    public bool AmbosJugadoresListos()
    {
        return hostListo.Value && clienteListo.Value;
    }

    public void LimpiarDatos()
    {
        if (IsServer)
        {
            personajeHost.Value = -1;
            personajeCliente.Value = -1;
            hostListo.Value = false;
            clienteListo.Value = false;
            mapaSeleccionado.Value = -1;
        }
    }
}