using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

public class PointDefense : MonoBehaviour
{
    private NativeArray<Vector3> EnemyPositions;
    private NativeArray<Vector3> GunPositions;
    private NativeArray<Vector3> ClosestTarget;
    private NativeArray<Vector3> DirectionToClosestTarget;
    public NativeArray<Vector3> GunOrientations;
    private NativeArray<bool> TargetLocked;
    bool JobVarsCreated = false;

    //List<Asteroid> AsteroidsInRange;
    // private void OnTriggerEnter(Collider other)
    // {
    //     AsteroidsInRange.Add(other.GetComponent<Asteroid>());
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     AsteroidsInRange.Remove(other.GetComponent<Asteroid>());
    // }
    [BurstCompile(CompileSynchronously = true)]


    struct PontDefenseJob : IJobParallelFor
    {
        // Jobs declare all data that will be accessed in the job
        // By declaring it as read only, multiple jobs are allowed to access the data in parallel
        [ReadOnly]
        public NativeArray<Vector3> EnemyPositions;
        [ReadOnly]
        public NativeArray<Vector3> GunPositions;
        [ReadOnly]
        public NativeArray<Vector3> GunOrientations;

        // By default containers are assumed to be read & write
        public NativeArray<Vector3> ClosestTarget;
        public NativeArray<Vector3> DirectionToClosestTarget;
        public NativeArray<bool> TargetLocked;

        // Delta time must be copied to the job since jobs generally don't have concept of a frame.
        // The main thread waits for the job same frame or next frame, but the job should do work deterministically
        // independent on when the job happens to run on the worker threads.
        public float deltaTime;
        [ReadOnly]
        internal int EnemyCount;



        // The code actually running on the job
        public void Execute(int i)
        {
            // Move the positions based on delta time and velocity
            //ClosestTarget[i] = ClosestTarget[i] + EnemyPositions[i] * deltaTime;

            Vector3 bestTarget = ClosestTarget[i];
            float closestDistanceSqr = 900;
            Vector3 currentPosition = GunPositions[i];
            TargetLocked[i] = false;
            for (int j = 0; j < EnemyCount; j++)
            {
                Vector3 directionToTarget = EnemyPositions[j] - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr && Vector3.Dot(directionToTarget.normalized, GunOrientations[i].normalized) > .7f)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = EnemyPositions[j];
                    TargetLocked[i] = true;
                }
            }
            // foreach (Vector3 potentialTarget in EnemyPositions)
            // {

            // }
            ClosestTarget[i] = bestTarget;
            DirectionToClosestTarget[i] = ClosestTarget[i] - GunPositions[i];
        }
    }

    public void Update()
    {
        if (!JobVarsCreated)
        {
            EnemyPositions = new NativeArray<Vector3>(AsteroidField.Instance.Asteroids.Count, Allocator.Persistent);

            GunPositions = new NativeArray<Vector3>(SpaceShipManager.Instance.FocusedSpaceship.Weapons.Count, Allocator.Persistent);
            ClosestTarget = new NativeArray<Vector3>(SpaceShipManager.Instance.FocusedSpaceship.Weapons.Count, Allocator.Persistent);
            DirectionToClosestTarget = new NativeArray<Vector3>(SpaceShipManager.Instance.FocusedSpaceship.Weapons.Count, Allocator.Persistent);
            GunOrientations = new NativeArray<Vector3>(SpaceShipManager.Instance.FocusedSpaceship.Weapons.Count, Allocator.Persistent);
            TargetLocked = new NativeArray<bool>(SpaceShipManager.Instance.FocusedSpaceship.Weapons.Count, Allocator.Persistent);
            JobVarsCreated = true;
        }

        for (var i = 0; i < GunPositions.Length; i++)
        {
            GunPositions[i] = SpaceShipManager.Instance.FocusedSpaceship.Weapons[i].Gun.transform.position;
            ClosestTarget[i] = SpaceShipManager.Instance.FocusedSpaceship.Weapons[i].Gun.transform.position + (SpaceShipManager.Instance.FocusedSpaceship.Weapons[i].Orientation.transform.position - SpaceShipManager.Instance.FocusedSpaceship.Weapons[i].Gun.transform.position);
            GunOrientations[i] = SpaceShipManager.Instance.FocusedSpaceship.Weapons[i].Orientation.transform.position - SpaceShipManager.Instance.FocusedSpaceship.Weapons[i].Gun.transform.position;
        }
        for (var i = 0; i < AsteroidField.Instance.Asteroids.Count; i++)
            EnemyPositions[i] = AsteroidField.Instance.Asteroids[i].transform.position;
        var EnemyCount = AsteroidField.Instance.Asteroids.Count;
        // Initialize the job data
        var job = new PontDefenseJob()
        {
            GunPositions = GunPositions,
            GunOrientations = GunOrientations,
            EnemyPositions = EnemyPositions,
            ClosestTarget = ClosestTarget,
            DirectionToClosestTarget = DirectionToClosestTarget,
            EnemyCount = EnemyCount,
            TargetLocked = TargetLocked
        };

        // Schedule a parallel-for job. First parameter is how many for-each iterations to perform.
        // The second parameter is the batch size,
        // essentially the no-overhead innerloop that just invokes Execute(i) in a loop.
        // When there is a lot of work in each iteration then a value of 1 can be sensible.
        // When there is very little work values of 32 or 64 can make sense.
        JobHandle jobHandle = job.Schedule(GunPositions.Length, 1);


        // Ensure the job has completed.
        // It is not recommended to Complete a job immediately,
        // since that reduces the chance of having other jobs run in parallel with this one.
        // You optimally want to schedule a job early in a frame and then wait for it later in the frame.
        jobHandle.Complete();
        for (var i = 0; i < GunPositions.Length; i++)
        {
            Vector3 refCurr = Vector3.zero;
            SpaceShipManager.Instance.FocusedSpaceship.Weapons[i].LocalTarget.position = Vector3.SmoothDamp(SpaceShipManager.Instance.FocusedSpaceship.Weapons[i].LocalTarget.position, job.ClosestTarget[i], ref refCurr, Time.deltaTime, 50);
            SpaceShipManager.Instance.FocusedSpaceship.Weapons[i].TargetLocked = job.TargetLocked[i];
        }
        // Debug.Log(job.ClosestTarget[0]);

    }

    private void OnDestroy()
    {
        // Native arrays must be disposed manually.
        EnemyPositions.Dispose();
        ClosestTarget.Dispose();
        GunPositions.Dispose();
        DirectionToClosestTarget.Dispose();
        GunOrientations.Dispose();
        TargetLocked.Dispose();
    }
}