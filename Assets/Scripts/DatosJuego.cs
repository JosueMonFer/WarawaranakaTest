using UnityEngine;

public static class DatosJuego
{
    // Datos Jugador 1 (Host)
    public static string personajeSeleccionadoJ1;
    public static int indicePersonajeSeleccionadoJ1;

    // Datos Jugador 2 (Cliente)
    public static string personajeSeleccionadoJ2;
    public static int indicePersonajeSeleccionadoJ2;

    // Mapa (solo el Host lo selecciona)
    public static string mapaSeleccionado;
    public static int indiceMapaSeleccionado;

    // Control de red
    public static bool esHost;
    public static bool jugador1Listo = false;
    public static bool jugador2Listo = false;

    // Código de sala (generado por el host)
    public static string codigoSala = "";

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
    }
}