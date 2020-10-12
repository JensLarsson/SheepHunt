using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePartitioning
{
    List<int>[,] worldSpaceIndexes;
    int worldWidth;
    int worldHeight;
    int ammount;
    int[] objects;
    int[] startArray;
    int[] stopArray;

    public SpacePartitioning(int objectAmount, int width, int height)
    {
        worldSpaceIndexes = new List<int>[width, height];
        ammount = objectAmount;
        worldWidth = width;
        worldHeight = height;
    }

    public (int[] indexes, int[] start, int[] stop) GetNew1DArray(Vector3[] positions)
    {
        objects = new int[ammount];
        startArray = new int[worldHeight * worldWidth];
        stopArray = new int[worldHeight * worldWidth];
        //Reset lists
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                worldSpaceIndexes[x, y] = new List<int>();
            }
        }
        //Add indexes to list
        for (int i = 0; i < positions.Length; i++)
        {
            worldSpaceIndexes[(int)positions[i].x + worldWidth / 2, (int)positions[i].z + worldHeight / 2].Add(i);
        }
        //Add indexes from lists to Array
        int index = 0;
        for (int y = 0; y < worldHeight; y++)
        {
            for (int x = 0; x < worldWidth; x++)
            {
                startArray[x + worldWidth * y] = index;
                stopArray[x + worldWidth * y] = index + worldSpaceIndexes[x, y].Count;
                foreach (int objectIndex in worldSpaceIndexes[x, y])
                {
                    objects[index] = objectIndex;
                    index++;
                }
            }
        }
        return (objects, startArray, stopArray);
    }

}
