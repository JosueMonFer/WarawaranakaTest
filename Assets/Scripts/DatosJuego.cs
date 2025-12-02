using UnityEngine;

public static class DatosJuego
{
    public static string personajeSeleccionado;
    public static int indicePersonajeSeleccionado;

    public static string mapaSeleccionado;
    public static int indiceMapaSeleccionado;

    public static void LimpiarDatos()
    {
        personajeSeleccionado = null;
        indicePersonajeSeleccionado = -1;
        mapaSeleccionado = null;
        indiceMapaSeleccionado = -1;
    }
}