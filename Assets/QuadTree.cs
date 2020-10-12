using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree<T>
{
    Rectangle boundary;
    int capacity;
    bool divided = false;
    List<(Vector3 position, T data)> positions;

    QuadTree<T>[] treeBranchesNwNeSwSe = new QuadTree<T>[0];


    public QuadTree(Rectangle bounds, int number)
    {
        boundary = bounds;
        capacity = number;
        positions = new List<(Vector3 position, T data)>();
    }
    public void Insert(Vector3 point, T t)
    {
        if (boundary.Contains(point.x, point.z))
        {
            if (positions.Count < capacity)
            {
                positions.Add((point, t));
            }
            else
            {
                if (!divided)
                {
                    Subdivide();
                }
                foreach (QuadTree<T> branch in treeBranchesNwNeSwSe)
                {
                    branch.Insert(point, t);
                }
            }
        }
    }
    public List<(Vector3 position, T data)> GetPositionsInRange(Vector3 point, float range, ref List<(Vector3 position, T data)> positionList)
    {
        if (!Overlaps(point, range))
        {
            return positionList;
        }
        else
        {
            foreach ((Vector3 position, T data) pos in positions)
            {
                if (Vector3.Distance(point, pos.position) < range)
                    positionList.Add(pos);
            }
            foreach (QuadTree<T> branch in treeBranchesNwNeSwSe)
            {
                branch.GetPositionsInRange(point, range, ref positionList);
            }
            return positionList;
        }
    }
    bool Overlaps(Vector3 point, float range)
    {
        return boundary.Overlaps(point.x - range, point.z - range, range * 2, range * 2);
    }



    void Subdivide()
    {
        float halfWidth = boundary.width * 0.5f;
        float halfHeight = boundary.height * 0.5f;

        Rectangle nwQuad = new Rectangle(boundary.x, boundary.y + halfHeight, halfWidth, halfHeight);
        Rectangle neQuad = new Rectangle(boundary.x + halfWidth, boundary.y + halfHeight, halfWidth, halfHeight);
        Rectangle swQuad = new Rectangle(boundary.x, boundary.y, halfWidth, halfHeight);
        Rectangle seQuad = new Rectangle(boundary.x + halfWidth, boundary.y, halfWidth, halfHeight);

        treeBranchesNwNeSwSe = new QuadTree<T>[] {
            new QuadTree<T>(nwQuad, capacity),
            new QuadTree<T>(neQuad, capacity),
            new QuadTree<T>(swQuad, capacity),
            new QuadTree<T>(seQuad, capacity)
    };
        divided = true;
    }
    public void PrintTree()
    {
        Debug.Log(boundary);
        foreach ((Vector3 position, T data) point in positions)
        {
            Debug.Log(point);
        }
        Debug.Log("----");

        foreach (QuadTree<T> branch in treeBranchesNwNeSwSe)
        {
            branch.PrintTree();
        }
    }

    public List<Rectangle> GetTree(ref List<Rectangle> rectList)
    {
        rectList.Add(this.boundary);
        foreach (QuadTree<T> branch in treeBranchesNwNeSwSe)
        {
            branch.GetTree(ref rectList);
        }
        return rectList;

    }


    public Rectangle GetBoundary => boundary;
}

public struct Rectangle
{
    public float x, y, width, height;

    public Rectangle(float x, float y, float width, float height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }
    public bool Contains(float X, float Y)
    {
        if (X < x || X > x + width)
        {
            return false;
        }
        if (Y < y || Y > y + height)
        {
            return false;
        }
        return true;
    }
    public bool Overlaps(Rectangle box)
    {
        if (x >= box.x + box.width
            || box.x >= x + width)
        {
            return false;
        }

        // If one rectangle is above other  
        if (y >= box.y + box.height
           || box.y >= y + height)
        {
            return false;
        }
        return true;
    }
    public bool Overlaps(float X, float Y, float Width, float Height)
    {
        if (x >= X + Width
            || X >= x + width)
        {
            return false;
        }

        // If one rectangle is above other  
        if (y >= Y + Height
           || Y >= y + height)
        {
            return false;
        }
        return true;
    }
}