using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;
        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5, 9); // We are going to have a minimum of 5 walls per level and a maximum of 9.
    public Count foodCount = new Count(1, 5); // We are going to have a minimum of 1 food pieces per level and a maximum of 5.
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder; // Used in order to keep the hierarchy clean.
    private List<Vector3> gridPositions = new List<Vector3>(); // Used to track all of the different possible positions in our GameBoard.

    void InitialiseList()
    {
        gridPositions.Clear();

        // "for" loop along the "x" and "y" axis.
        for (int x=1; x<columns-1; x++)
        {
            for (int y=1; y<rows-1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for(int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range (0, outerWallTiles.Length)];
                }
                GameObject instance = Instantiate(toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition() // New function which returns a "Vector3", which is a random position that will be used for a certain object.
    {
        int randomIndex = Random.Range(0, gridPositions.Count); // Generamos un número entre 0 y el número de posiciones de nuestra grid.
        Vector3 randomPosition = gridPositions[randomIndex]; // Definimos esta posición random a partir del número aleatorio que nos haya salido.
        gridPositions.RemoveAt(randomIndex); // Borramos esta posición (para que no aparezcan dos objetos en una misma casilla).
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i=0; i<objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level) // Es la única función pública que tenemos (así pues, la que va a ser llamada por el GameManager).
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f); // Difficulty scaling up as the player ascends in level: 1 enemy at level 2, 2 enemies at level 4, 3 enemies at level 8, ...
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0F), Quaternion.identity); // The exit will always be in the top right corner of the board.
    }
}
