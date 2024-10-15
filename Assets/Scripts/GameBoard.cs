using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private Tilemap currentState;  // Tilemap para el estado actual del tablero.
    [SerializeField] private Tilemap nextState;     // Tilemap para el siguiente estado del tablero.
    [SerializeField] private Tile aliveTile;        // Tile que representa una celda viva.
    [SerializeField] private Tile deadTile;         // Tile que representa una celda muerta.
    [SerializeField] private float updateInterval = 0.05f;  // Intervalo entre actualizaciones de la simulaci�n.

    private HashSet<Vector3Int> aliveCells;         // Conjunto de celdas vivas.
    private HashSet<Vector3Int> cellsToCheck;       // Conjunto de celdas que deben ser revisadas.

    public int population { get; private set; }     // Poblaci�n de celdas vivas.
    public int iterations { get; private set; }     // N�mero de iteraciones de la simulaci�n.
    public float time { get; private set; }         // Tiempo transcurrido de la simulaci�n.

    private void Awake()
    {
        aliveCells = new HashSet<Vector3Int>();     // Inicializa el conjunto de celdas vivas.
        cellsToCheck = new HashSet<Vector3Int>();   // Inicializa el conjunto de celdas a revisar.
    }

    private void Start()
    {
        // Ajusta la c�mara para que enfoque el �rea donde se encuentran las celdas.
        Camera.main.transform.position = new Vector3(0, 0, -10);  // Ajusta las coordenadas seg�n la escala de tu tablero.
    }

    private void Update()
    {
        HandleInput();  // Permite que el jugador pinte celdas en el tablero.

        // Inicia la simulaci�n cuando el jugador presiona la tecla "S".
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(Simulate());  // Comienza la simulaci�n.
        }
    }

    // M�todo para manejar la interacci�n del jugador (pintar celdas).
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))  // Detecta clic izquierdo del rat�n.
        {
            // Obtiene la posici�n del rat�n en el mundo.
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Convierte la posici�n del mundo a una posici�n de celda en el Tilemap.
            Vector3Int cellPosition = currentState.WorldToCell(mouseWorldPos);

            // Si la celda est� viva, la cambia a muerta, si est� muerta, la cambia a viva.
            if (IsAlive(cellPosition))
            {
                currentState.SetTile(cellPosition, deadTile);  // Cambia la celda a muerta.
                aliveCells.Remove(cellPosition);  // Elimina la celda del conjunto de celdas vivas.
            }
            else
            {
                currentState.SetTile(cellPosition, aliveTile);  // Cambia la celda a viva.
                aliveCells.Add(cellPosition);  // A�ade la celda al conjunto de celdas vivas.
            }

            // Actualiza la poblaci�n de celdas vivas.
            population = aliveCells.Count;
        }
    }

    // Limpia el tablero y reinicia las variables.
    private void Clear()
    {
        currentState.ClearAllTiles();  // Borra todas las celdas del Tilemap actual.
        nextState.ClearAllTiles();  // Borra todas las celdas del Tilemap siguiente.
        aliveCells.Clear();  // Limpia el conjunto de celdas vivas.
        cellsToCheck.Clear();  // Limpia el conjunto de celdas a revisar.
        population = 0;  // Reinicia la poblaci�n.
        iterations = 0;  // Reinicia las iteraciones.
        time = 0;  // Reinicia el tiempo.
    }

    // Coroutine que controla la simulaci�n.
    private IEnumerator Simulate()
    {
        var interval = new WaitForSeconds(updateInterval);  // Define el intervalo de tiempo.
        yield return interval;  // Espera antes de la primera actualizaci�n.

        // Mientras el objeto est� habilitado, contin�a la simulaci�n.
        while (enabled)
        {
            UpdateState();  // Actualiza el estado del tablero.

            population = aliveCells.Count;  // Actualiza el n�mero de celdas vivas.
            iterations++;  // Incrementa el n�mero de generaciones.
            time += updateInterval;  // Aumenta el tiempo de la simulaci�n.

            yield return interval;  // Espera el intervalo antes de la siguiente actualizaci�n.
        }
    }

    // M�todo que actualiza el estado de todas las celdas en el tablero.
    private void UpdateState()
    {
        cellsToCheck.Clear();  // Limpia las celdas a revisar.

        // Recolecta las celdas vivas y sus vecinas para revisarlas.
        foreach (Vector3Int cell in aliveCells)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    cellsToCheck.Add(cell + new Vector3Int(x, y, 0));  // A�ade las vecinas a revisar.
                }
            }
        }

        // Transiciona las celdas al siguiente estado seg�n las reglas del "Game of Life".
        foreach (Vector3Int cell in cellsToCheck)
        {
            int neighbors = CountNeighbors(cell);  // Cuenta los vecinos vivos de la celda.
            bool alive = IsAlive(cell);  // Verifica si la celda est� viva.

            if (!alive && neighbors == 3)
            {
                nextState.SetTile(cell, aliveTile);  // Si la celda est� muerta y tiene 3 vecinos vivos, nace.
                aliveCells.Add(cell);  // A�ade la celda a las vivas.
            }
            else if (alive && (neighbors < 2 || neighbors > 3))
            {
                nextState.SetTile(cell, deadTile);  // Si la celda est� viva y tiene menos de 2 o m�s de 3 vecinos vivos, muere.
                aliveCells.Remove(cell);  // Elimina la celda del conjunto de celdas vivas.
            }
            else
            {
                // Si no hay cambios, mantiene el estado actual.
                nextState.SetTile(cell, aliveTile);
            }
        }

        // Intercambia los estados: el siguiente estado pasa a ser el actual y viceversa.
        currentState.ClearAllTiles();  // Borra el Tilemap actual para actualizarlo con el nuevo estado.
        foreach (Vector3Int cell in aliveCells)
        {
            currentState.SetTile(cell, aliveTile);  // Actualiza el estado del Tilemap actual.
        }
    }

    // M�todo que cuenta el n�mero de vecinos vivos alrededor de una celda.
    private int CountNeighbors(Vector3Int cell)
    {
        int count = 0;  // Inicializa el contador de vecinos.

        // Recorre todas las vecinas de la celda actual.
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int neighbor = cell + new Vector3Int(x, y, 0);  // Calcula la posici�n de la vecina.

                if (x == 0 && y == 0)
                {
                    continue;  // Ignora la celda actual.
                }
                else if (IsAlive(neighbor))
                {
                    count++;  // Incrementa el contador si el vecino est� vivo.
                }
            }
        }

        return count;  // Devuelve el n�mero de vecinos vivos.
    }

    // M�todo que verifica si una celda est� viva.
    private bool IsAlive(Vector3Int cell)
    {
        return currentState.GetTile(cell) == aliveTile;  // Devuelve true si la celda est� viva.
    }
}
