using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este atributo permite crear un nuevo asset de tipo "Pattern" desde el men� de Unity.
// Esto es �til para definir diferentes patrones de celdas vivas que puedes reutilizar en el juego.
[CreateAssetMenu(menuName = "Game of Life/Pattern")]
public class Pattern : ScriptableObject  // Esta clase extiende ScriptableObject, lo que significa que se puede usar para almacenar datos en Unity.
{
    // Un array que almacena las posiciones de las celdas vivas. 
    // Cada celda se representa con un Vector2Int, que tiene dos componentes enteros: X e Y.
    public Vector2Int[] cells;

    // Este m�todo devuelve el "centro" del patr�n de celdas vivas.
    // Se usa para alinear el patr�n en el tablero de juego.
    public Vector2Int GetCenter()
    {
        // Si el array de celdas est� vac�o o no ha sido inicializado, el centro es el origen (0,0).
        if (cells == null || cells.Length == 0)
        {
            return Vector2Int.zero;  // Devuelve (0, 0) como el centro por defecto.
        }

        // Variables que almacenar�n los valores m�nimos y m�ximos de las posiciones X e Y de las celdas.
        // Inicialmente se establecen en (0,0).
        Vector2Int min = Vector2Int.zero;
        Vector2Int max = Vector2Int.zero;

        // Recorremos todas las celdas vivas para encontrar las posiciones m�nimas y m�ximas.
        for (int i = 0; i < cells.Length; i++)
        {
            // Se obtiene la posici�n de la celda actual.
            Vector2Int cell = cells[i];

            // Actualizamos las coordenadas m�nimas: buscamos las celdas m�s a la izquierda (menor X)
            // y m�s abajo (menor Y) del patr�n.
            min.x = Mathf.Min(min.x, cell.x);  // Encuentra el menor valor en X.
            min.y = Mathf.Min(min.y, cell.y); // Encuentra el menor valor en Y.

            // Actualizamos las coordenadas m�ximas: buscamos las celdas m�s a la derecha (mayor X)
            // y m�s arriba (mayor Y) del patr�n.
            max.x = Mathf.Max(max.x, cell.x);  // Encuentra el mayor valor en X.
            max.y = Mathf.Max(max.y, cell.y);  // Encuentra el mayor valor en Y.
        }

        // Calculamos el centro del patr�n tomando el punto medio entre los l�mites
        // m�s a la izquierda/abajo (min) y los l�mites m�s a la derecha/arriba (max).
        return (min + max) / 2;  // Devuelve el centro como el promedio de las posiciones m�nimas y m�ximas.
    }
}
