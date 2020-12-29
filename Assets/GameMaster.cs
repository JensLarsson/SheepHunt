
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    [SerializeField] Unit unitPrefab;
    [SerializeField] ComputeShader computeShader;
    [Header("Spawn")]
    [SerializeField] [Range(10, 5000)] int spawnWidth = 200;
    [SerializeField] int ammount = 50;
    [Header("Speeds")]
    [SerializeField] float maxSpeed = 2;
    [SerializeField] float minSpeed = 0.2f;
    [Header("Cohesion")]
    [SerializeField] float cohesionAwareness = 2;
    [SerializeField] float cohesionStrenght = 0.5f;
    [Header("Allignment")]
    [SerializeField] float allignAwareness = 2;
    [SerializeField] float allignStrengh = 0.5f;
    [Header("Separation")]
    [SerializeField] float separationAwareness = 2;
    [SerializeField] float separationStrenght = 0.5f;
    [Header("Target")]
    [SerializeField] float targetFollowStrenght = 0.5f;
    [Header("Flee")]
    [SerializeField] float panicAwareness = 1.0f;
    [SerializeField] float panicStrenght = 1.0f;
    [Header("Walls")]
    [SerializeField] float wallAvoidanceStrenght = 40f;


    ComputeBuffer positionBuffer;
    ComputeBuffer velocityBuffer;
    ComputeBuffer DistanceFronPlayerBuffer;
    ComputeBuffer targetBuffer;
    ComputeBuffer targetMultiplierBuffer;
    ComputeBuffer circularObstacleBuffer;
    ComputeBuffer boxObstaclePositionBuffer;
    ComputeBuffer boxObstacleHalfDimetionBuffer;



    //ComputeBuffer partitionedIndexBuffer;
    //ComputeBuffer partitionedIndexOffsetBufferStart;
    //ComputeBuffer partitionedIndexOffsetBufferStop;

    Camera camera;


    Boid[] flock;
    Vector3 playerPosition;

    Vector3[] unitPositions;        //position of the units themselves
    Vector3[] unitTargetPosition;   //positions of the targets of each unit for various uses
    Vector3[] velocities;           //current speed of each unit
    //int[] pinned;                   
    float[] targetMovementMultiplier; //used to controll wether a unit is moving towards a target or not
    float[] unitDistanceFromPlayer;

    Vector4[] circularObstaclePositionsAndRadius;    //xyz and w for radius

    Vector3[] cubeObstaclePositions;
    Vector3[] cubeObstacleHalfWidths;

    int quadMaxUnits = 4;
    QuadTree<int> sheepPositionQuadTree;
    Rectangle spawnRect;


    SpacePartitioning unitSpacePartitioner;

    public static GameMaster Instance { get; private set; }

    private void Start()
    {
        circularObstaclePositionsAndRadius = new Vector4[Obstacle_Cylinder.obstacles.Count];
        for (int i = 0; i < Obstacle_Cylinder.obstacles.Count; i++)
        {
            circularObstaclePositionsAndRadius[i] = Obstacle_Cylinder.obstacles[i].GetPositionAndRadious();
        }

        computeShader.SetInt("circularObstaclesCount", circularObstaclePositionsAndRadius.Length);
        circularObstacleBuffer = new ComputeBuffer(circularObstaclePositionsAndRadius.Length, sizeof(float) * 4);
        circularObstacleBuffer.SetData(circularObstaclePositionsAndRadius);
        computeShader.SetBuffer(0, "circularObstacles", circularObstacleBuffer);

        cubeObstaclePositions = new Vector3[Obstacle_Box.obstacles.Count];
        cubeObstacleHalfWidths = new Vector3[Obstacle_Box.obstacles.Count];
        for (int i = 0; i < Obstacle_Box.obstacles.Count; i++)
        {
            cubeObstaclePositions[i] = Obstacle_Box.obstacles[i].GetPosition;
            cubeObstacleHalfWidths[i] = Obstacle_Box.obstacles[i].GetCenterWidth;
        }

        Debug.Log(cubeObstaclePositions.Length);
        computeShader.SetInt("boxObstacleCount", cubeObstaclePositions.Length);
        //Box Positions
        boxObstaclePositionBuffer = new ComputeBuffer(cubeObstaclePositions.Length, sizeof(float) * 3);
        boxObstaclePositionBuffer.SetData(cubeObstaclePositions);
        computeShader.SetBuffer(0, "boxObstaclePositions", boxObstaclePositionBuffer);
        //Box Widths
        boxObstacleHalfDimetionBuffer = new ComputeBuffer(cubeObstacleHalfWidths.Length, sizeof(float) * 3);
        boxObstacleHalfDimetionBuffer.SetData(cubeObstacleHalfWidths);
        computeShader.SetBuffer(0, "boxObstacleHalfDimentions", boxObstacleHalfDimetionBuffer);


        //////////////////////////
        //unitSpacePartitioner = new SpacePartitioning(ammount, spawnWidth, spawnWidth);
        //var particionedSpace = unitSpacePartitioner.GetNew1DArray(unitPositions);

        //partitionedIndexBuffer = new ComputeBuffer(ammount, sizeof(int));
        //partitionedIndexBuffer.SetData(particionedSpace.indexes);
        //computeShader.SetBuffer(0, "partitionedIndex", partitionedIndexBuffer);

        //partitionedIndexOffsetBufferStart = new ComputeBuffer(spawnWidth * spawnWidth, sizeof(int));
        //partitionedIndexOffsetBufferStart.SetData(particionedSpace.start);
        //computeShader.SetBuffer(0, "partitionedInxesOffsetStart", partitionedIndexOffsetBufferStart);

        //partitionedIndexOffsetBufferStop = new ComputeBuffer(spawnWidth * spawnWidth, sizeof(int));
        //partitionedIndexOffsetBufferStop.SetData(particionedSpace.stop);
        //computeShader.SetBuffer(0, "partitionedInxesOffsetStop", partitionedIndexOffsetBufferStop);

        //computeShader.SetInt("spaceWidth", spawnWidth);




        computeShader.Dispatch(0, ammount, 1, 1);
    }
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        Instance = this;
        //instantiate arrays
        flock = new Boid[ammount];
        unitPositions = new Vector3[ammount];
        velocities = new Vector3[ammount];
        unitTargetPosition = new Vector3[ammount];
        unitDistanceFromPlayer = new float[ammount];
        //pinned = new int[ammount];
        targetMovementMultiplier = new float[ammount];


        camera = Camera.main;
        spawnRect = new Rectangle(-spawnWidth * 0.5f, -spawnWidth * 0.5f, spawnWidth, spawnWidth);
        sheepPositionQuadTree = new QuadTree<int>(spawnRect, quadMaxUnits);


        for (int i = 0; i < ammount; i++)
        {
            unitPositions[i] =
                new Vector3(
                    Random.Range(-spawnWidth, spawnWidth) * 0.5f, 0,
                    Random.Range(-spawnWidth, spawnWidth) * 0.5f
                    );
            sheepPositionQuadTree.Insert(unitPositions[i], i);
            unitDistanceFromPlayer[i] = 10000;
            flock[i].unit = Instantiate(unitPrefab, unitPositions[i], Quaternion.identity);
            flock[i].unit.SetIndex(i);
            targetMovementMultiplier[i] = 0;
            //pinned[i] = 1;
            velocities[i] = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * maxSpeed;
        }


        computeShader.SetInt("ammount", ammount);
        computeShader.SetFloat("arenaWidth", spawnWidth);
        computeShader.SetFloat("wallAwareness", 10);


        ComputeBuffer pinnedBuffer = new ComputeBuffer(ammount, sizeof(int));
        targetBuffer = new ComputeBuffer(ammount, sizeof(float) * 3);
        positionBuffer = new ComputeBuffer(ammount, sizeof(float) * 3);
        velocityBuffer = new ComputeBuffer(ammount, sizeof(float) * 3);
        targetMultiplierBuffer = new ComputeBuffer(ammount, sizeof(float));
        DistanceFronPlayerBuffer = new ComputeBuffer(ammount, sizeof(float));
        //pinnedBuffer.SetData(pinned);
        targetBuffer.SetData(unitTargetPosition);
        positionBuffer.SetData(unitPositions);
        velocityBuffer.SetData(velocities);
        targetMultiplierBuffer.SetData(targetMovementMultiplier);
        computeShader.SetBuffer(0, "pinned", pinnedBuffer);
        computeShader.SetBuffer(0, "target", targetBuffer);
        computeShader.SetBuffer(0, "Positions", positionBuffer);
        computeShader.SetBuffer(0, "Velocities", velocityBuffer);
        computeShader.SetBuffer(0, "targetMultiplier", targetMultiplierBuffer);
        computeShader.SetBuffer(0, "distanceFromPlayer", DistanceFronPlayerBuffer);
        SetComputeValues();

    }

    // Update is called once per frame
    void Update()
    {
        //Flock();
        FlockGPU();
    }


    void FlockGPU()
    {
        //var particionedSpace = unitSpacePartitioner.GetNew1DArray(unitPositions);
        //partitionedIndexBuffer.SetData(particionedSpace.indexes);
        //partitionedIndexOffsetBufferStart.SetData(particionedSpace.start);
        //partitionedIndexOffsetBufferStop.SetData(particionedSpace.stop);

        //targetMultiplierBuffer.GetData(targetMovementMultiplier);
        SetComputeValues();

        positionBuffer.GetData(unitPositions);
        velocityBuffer.GetData(velocities);
        DistanceFronPlayerBuffer.GetData(unitDistanceFromPlayer);


        computeShader.Dispatch(0, ammount, 1, 1);

        for (int i = 0; i < ammount; i++)
        {
            flock[i].unit.transform.position = unitPositions[i];
            flock[i].unit.transform.eulerAngles = new Vector3(0, Mathf.Atan2(velocities[i].x, velocities[i].z) * Mathf.Rad2Deg, 0);
        }
    }


    void Flock()
    {
        QuadTree<int> newQuadTree = new QuadTree<int>(spawnRect, quadMaxUnits);
        for (int currentBoid = 0; currentBoid < flock.Length; currentBoid++)
        {

            Vector3 allignment = Vector3.zero;
            Vector3 coheshionDirection = new Vector3();
            Vector3 seperationDirection = new Vector3();
            int count = 0;
            float maxRange = Mathf.Max(cohesionAwareness, allignAwareness);
            List<(Vector3 position, int index)> pointList = new List<(Vector3 position, int index)>();
            sheepPositionQuadTree.GetPositionsInRange(unitPositions[currentBoid], maxRange, ref pointList);
            //Parallel.For(0, pointList.Count, (otherBoid) =>
            //{

            for (int otherBoid = 0; otherBoid < pointList.Count; otherBoid++)
            {
                //if (otherBoid != currentBoid)
                //{
                float dist = Vector3.Distance(unitPositions[currentBoid], unitPositions[pointList[otherBoid].index]);
                count++;
                if (dist < allignAwareness)
                {
                    allignment += flock[pointList[otherBoid].index].velocity;
                }
                if (dist < cohesionAwareness)
                {
                    coheshionDirection += unitPositions[pointList[otherBoid].index];
                }
                if (dist < separationAwareness)
                {
                    Vector3 direction = unitPositions[currentBoid] - unitPositions[pointList[otherBoid].index];
                    direction /= dist * dist;
                    seperationDirection += direction;
                }
                //}
            }
            if (count > 0)
            {
                allignment /= count;
                allignment = allignment.normalized * maxSpeed;
                allignment -= flock[currentBoid].velocity;
                allignment = allignment.magnitude > maxSpeed ? allignment.normalized * maxSpeed : allignment;

                coheshionDirection /= count;
                coheshionDirection -= unitPositions[currentBoid];
                coheshionDirection = coheshionDirection.normalized * maxSpeed;
                coheshionDirection -= new Vector3(flock[currentBoid].velocity.x, 0, flock[currentBoid].velocity.y);
                coheshionDirection = coheshionDirection.normalized * maxSpeed;

                seperationDirection /= count;
                seperationDirection = seperationDirection.normalized * maxSpeed;
                seperationDirection -= flock[currentBoid].velocity;
                seperationDirection = seperationDirection.magnitude > maxSpeed ? seperationDirection.normalized * maxSpeed : seperationDirection;

            }





            flock[currentBoid].acceleration += allignment * allignStrengh * Time.deltaTime;
            flock[currentBoid].acceleration += coheshionDirection * cohesionStrenght * Time.deltaTime;
            flock[currentBoid].acceleration += seperationDirection * separationStrenght * Time.deltaTime;
            flock[currentBoid].velocity = flock[currentBoid].velocity + flock[currentBoid].acceleration;

            float magnitude = flock[currentBoid].velocity.magnitude;
            flock[currentBoid].velocity = magnitude <= maxSpeed ? flock[currentBoid].velocity : flock[currentBoid].velocity.normalized * maxSpeed;
            unitPositions[currentBoid] = unitPositions[currentBoid] + flock[currentBoid].velocity * Time.deltaTime;
            unitPositions[currentBoid].y = 0;
            flock[currentBoid].unit.transform.position = unitPositions[currentBoid];
            flock[currentBoid].acceleration *= 0;

            flock[currentBoid].unit.transform.eulerAngles = new Vector3(0, Mathf.Atan2(velocities[currentBoid].x, velocities[currentBoid].z) * Mathf.Rad2Deg, 0);

            newQuadTree.Insert(unitPositions[currentBoid], currentBoid);
        }
        sheepPositionQuadTree = newQuadTree;
    }

    void SetComputeValues()
    {
        //computeShader.SetVector("target", camera.ScreenToWorldPoint(Input.mousePosition));
        computeShader.SetFloat("deltaTime", Time.deltaTime);
        computeShader.SetFloat("maxSpeed", maxSpeed);
        computeShader.SetFloat("minSpeed", minSpeed);
        computeShader.SetFloat("wallAvoidanceStrenght", wallAvoidanceStrenght);
        computeShader.SetFloat("targetFollowStrenght", targetFollowStrenght);
        computeShader.SetFloat("separationAwareness", separationAwareness);
        computeShader.SetFloat("separationStrenght", separationStrenght);
        computeShader.SetFloat("cohesionAwareness", cohesionAwareness);
        computeShader.SetFloat("cohesionStrenght", cohesionStrenght);
        computeShader.SetFloat("allignAwareness", allignAwareness);
        computeShader.SetFloat("allignStrengh", allignStrengh);
        computeShader.SetFloat("panicAwareness", panicAwareness);
        computeShader.SetFloat("panicStrenght", panicStrenght);
        computeShader.SetVector("playerPosition", playerPosition);

        targetBuffer.SetData(unitTargetPosition);
        computeShader.SetBuffer(0, "target", targetBuffer);
        targetMultiplierBuffer.SetData(targetMovementMultiplier);
        computeShader.SetBuffer(0, "targetMultiplier", targetMultiplierBuffer);
    }


    public void DestroyUnit(Unit unit)
    {
        unit.gameObject.SetActive(false);
    }


    Vector2 PlayspaceWrap(Vector2 pos, float[] boundingBox)
    {
        if (pos.x < boundingBox[0])
        {
            pos.x = boundingBox[1];
        }
        else if (pos.x > boundingBox[1])
        {
            pos.x = boundingBox[0];
        }
        if (pos.y > boundingBox[2])
        {
            pos.y = boundingBox[3];
        }
        else if (pos.y < boundingBox[3])
        {
            pos.y = boundingBox[2];
        }
        return pos;
    }


    public void SetTargetPosition(List<Unit> units, Vector3 position)
    {
        Debug.Log($"Sending {units.Count} units to {position}");
        for (int i = 0; i < units.Count; i++)
        {
            unitTargetPosition[units[i].unitIndex] = position;
            targetMovementMultiplier[units[i].unitIndex] = 1;
        }
        targetMultiplierBuffer.SetData(targetMovementMultiplier);
    }
    public void SetTargetsPlayer(int unitIndex)
    {
        unitTargetPosition[unitIndex] = playerPosition;
        targetMovementMultiplier[unitIndex] = 1;
        targetMultiplierBuffer.SetData(targetMovementMultiplier);
    }
    public void SetStopChase(int unitIndex)
    {
        targetMovementMultiplier[unitIndex] = 0;
        targetMultiplierBuffer.SetData(targetMovementMultiplier);
    }

    public void SetPlayerPosition(Vector3 pos) => playerPosition = pos;


    public Boid[] GetBoidArray() => flock;
    public Vector3[] GetBoisPositionArray() => unitPositions;
    public float GetDistanceFromPlayer(int unitIndex) => unitDistanceFromPlayer[unitIndex];
    public Vector3 GetPlayerPosition() => playerPosition;

}
public struct Boid
{
    public Unit unit;
    public Vector3 velocity;
    public Vector3 acceleration;
    //public int index;
}

public struct Vector6
{
    public Vector3 positions, widths;
    public Vector6(Vector3 pos, Vector3 width)
    {
        positions = pos;
        widths = width;
        //x = pos.x;
        //y = pos.y;
        //z = pos.z;
        //a = width.x;
        //b = width.y;
        //c = width.z;
    }
}