using UnityEngine;

public static class DatosJuego
{
    // ========================================
    // MODO SINGLE PLAYER (Original)
    // ========================================
    public static string personajeSeleccionado
    {
        get
        {
            // En single player, usar J1
            return personajeSeleccionadoJ1;
        }
        set
        {
            personajeSeleccionadoJ1 = value;
        }
    }

    public static int indicePersonajeSeleccionado
    {
        get
        {
            // En single player, usar J1
            return indicePersonajeSeleccionadoJ1;
        }
        set
        {
            indicePersonajeSeleccionadoJ1 = value;
        }
    }

    // ========================================
    // MODO MULTIJUGADOR
    // ========================================

    // Datos Jugador 1 (Host)
    public static string personajeSeleccionadoJ1;
    public static int indicePersonajeSeleccionadoJ1;

    // Datos Jugador 2 (Cliente)
    public static string personajeSeleccionadoJ2;
    public static int indicePersonajeSeleccionadoJ2;

    // ========================================
    // MAPA (compartido)
    // ========================================
    public static string mapaSeleccionado;
    public static int indiceMapaSeleccionado;

    // ========================================
    // CONTROL DE RED
    // ========================================
    public static bool esMultijugador = false; // Indica si estamos en modo multijugador
    public static bool esHost = false;
    public static bool jugador1Listo = false;
    public static bool jugador2Listo = false;

    // Código de sala (generado por el host)
    public static string codigoSala = "";

    // ========================================
    // MÉTODOS ÚTILES
    // ========================================

    public static void LimpiarDatos()
    {
        personajeSeleccionadoJ1 = null;
        indicePersonajeSeleccionadoJ1 = -1;
        personajeSeleccionadoJ2 = null;
        indicePersonajeSeleccionadoJ2 = -1;
        mapaSeleccionado = null;
        indiceMapaSeleccionado = -1;
        jugador1Listo = false;
        jugador2Listo = false;
        codigoSala = "";
        esMultijugador = false;
        esHost = false;
    }

    // Método para configurar modo single player
    public static void ConfigurarSinglePlayer()
    {
        esMultijugador = false;
        esHost = false;
        Debug.Log("Modo configurado: Single Player");
    }

    // Método para configurar modo multijugador
    public static void ConfigurarMultijugador(bool comoHost)
    {
        esMultijugador = true;
        esHost = comoHost;
        Debug.Log($"Modo configurado: Multijugador ({(comoHost ? "Host" : "Cliente")})");
    }

    // Método para debug
    public static void MostrarEstado()
    {
        Debug.Log("=== ESTADO DE DATOSJUEGO ===");
        Debug.Log($"Modo Multijugador: {esMultijugador}");
        Debug.Log($"Es Host: {esHost}");
        Debug.Log($"Personaje J1: {personajeSeleccionadoJ1} (índice: {indicePersonajeSeleccionadoJ1})");
        Debug.Log($"Personaje J2: {personajeSeleccionadoJ2} (índice: {indicePersonajeSeleccionadoJ2})");
        Debug.Log($"Mapa: {mapaSeleccionado} (índice: {indiceMapaSeleccionado})");
        Debug.Log($"Código Sala: {codigoSala}");
        Debug.Log("============================");
    }
}