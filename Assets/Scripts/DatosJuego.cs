using UnityEngine;

public static class DatosJuego
{
    // Datos de personaje
    public static string personajeSeleccionado = "";
    public static int indicePersonajeSeleccionado = 0;

    // Datos de mapa
    public static string mapaSeleccionado = "";
    public static int indiceMapaSeleccionado = 0;

    // Modo de juego
    public static bool esModoMultijugador = false;
    public static bool esHost = false;

    // Datos de personajes en multijugador
    public static string personajeJugador1 = "";
    public static int indicePersonajeJugador1 = 0;

    public static string personajeJugador2 = "";
    public static int indicePersonajeJugador2 = 0;

    // Código de sala para multijugador (PÚBLICO para acceso directo)
    public static string codigoSala = "";

    // ============================================
    // MÉTODOS CON SOBRECARGA (para compatibilidad)
    // ============================================

    // Versión con parámetro booleano (usado por MenuMultijugador)
    public static void ConfigurarMultijugador(bool comoHost)
    {
        esModoMultijugador = true;
        esHost = comoHost;
        Debug.Log($"Modo configurado: Multiplayer - {(comoHost ? "HOST" : "CLIENTE")}");
    }

    // Versión sin parámetros (usado por otros scripts)
    public static void ConfigurarMultijugador()
    {
        esModoMultijugador = true;
        Debug.Log("Modo configurado: Multiplayer");
    }

    // ============================================
    // MÉTODOS ADICIONALES
    // ============================================

    public static void ConfigurarSinglePlayer()
    {
        esModoMultijugador = false;
        esHost = false;
        Debug.Log("Modo configurado: Single Player");
    }

    public static void ConfigurarMultiplayer()
    {
        esModoMultijugador = true;
        Debug.Log("Modo configurado: Multiplayer");
    }

    public static bool EsModoMultijugador()
    {
        return esModoMultijugador;
    }

    public static bool EsHost()
    {
        return esHost;
    }

    public static void EstablecerCodigoSala(string codigo)
    {
        codigoSala = codigo;
        Debug.Log($"Código de sala establecido: {codigo}");
    }

    public static string ObtenerCodigoSala()
    {
        return codigoSala;
    }

    public static void LimpiarDatos()
    {
        personajeSeleccionado = "";
        indicePersonajeSeleccionado = 0;
        mapaSeleccionado = "";
        indiceMapaSeleccionado = 0;
        esModoMultijugador = false;
        esHost = false;
        personajeJugador1 = "";
        indicePersonajeJugador1 = 0;
        personajeJugador2 = "";
        indicePersonajeJugador2 = 0;
        codigoSala = "";
        Debug.Log("Datos del juego limpiados");
    }
}