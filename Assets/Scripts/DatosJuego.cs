using UnityEngine;

public static class DatosJuego
{
    //esto lo dejo para que no se rompa el proyecto pero sera sustituido
    public static string personajeSeleccionado;
    public static int indicePersonajeSeleccionado;

    // Datos del personaje Uno
    public static string personajeSeleccionadoUno;
    public static int indicePersonajeSeleccionadoUno;


    //Datos del personaje Dos
    public static string personajeSeleccionadoDos;
    public static int indicePersonajeSeleccionadoDos;


    public static string mapaSeleccionado;
    public static int indiceMapaSeleccionado;

    public static void LimpiarDatos()
    {
        personajeSeleccionado = null;
        indicePersonajeSeleccionado = -1;

        mapaSeleccionado = null;
        indiceMapaSeleccionado = -1;

        personajeSeleccionadoUno = null;
        indicePersonajeSeleccionadoUno = -1;


        personajeSeleccionadoDos = null;
        indicePersonajeSeleccionadoDos = -1;
    }
}