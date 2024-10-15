using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este atributo permite crear un nuevo asset de tipo "Pattern" desde el menú de Unity.
// Esto es útil para definir diferentes patrones de celdas vivas que puedes reutilizar en el juego.
[CreateAssetMenu(menuName = "Game of Life/Pattern")]
public class Pattern : ScriptableObject  // Esta clase extiende ScriptableObject, lo que significa que se puede usar para almacenar datos en Unity.
{
    // Un array que almacena las posiciones de las celdas vivas. 
    // Cada celda se representa con un Vector2Int, que tiene dos componentes enteros: X e Y.
    public Vector2Int[] cells;

    // Este método devuelve el "centro" del patrón de celdas vivas.
    // Se usa para alinear el patrón en el tablero de juego.
    public Vector2Int GetCenter()
    {
        // Si el array de celdas está vacío o no ha sido inicializado, el centro es el origen (0,0).
        if (cells == null || cells.Length == 0)
        {
            return Vector2Int.zero;  // Devuelve (0, 0) como el centro por defecto.
        }

        // Variables que almacenarán los valores mínimos y máximos de las posiciones X e Y de las celdas.
        // Inicialmente se establecen en (0,0).
        Vector2Int min = Vector2Int.zero;
        Vector2Int max = Vector2Int.zero;

        // Recorremos todas las celdas vivas para encontrar las posiciones mínimas y máximas.
        for (int i = 0; i < cells.Length; i++)
        {
            // Se obtiene la posición de la celda actual.
            Vector2Int cell = cells[i];

            // Actualizamos las coordenadas mínimas: buscamos las celdas más a la izquierda (menor X)
            // y más abajo (menor Y) del patrón.
            min.x = Mathf.Min(min.x, cell.x);  // Encuentra el menor valor en X.
            min.y = Mathf.Min(min.y, cell.y); // Encuentra el menor valor en Y.

            // Actualizamos las coordenadas máximas: buscamos las celdas más a la derecha (mayor X)
            // y más arriba (mayor Y) del patrón.
            max.x = Mathf.Max(max.x, cell.x);  // Encuentra el mayor valor en X.
            max.y = Mathf.Max(max.y, cell.y);  // Encuentra el mayor valor en Y.
        }

        // Calculamos el centro del patrón tomando el punto medio entre los límites
        // más a la izquierda/abajo (min) y los límites más a la derecha/arriba (max).
        return (min + max) / 2;  // Devuelve el centro como el promedio de las posiciones mínimas y máximas.
    }
}
