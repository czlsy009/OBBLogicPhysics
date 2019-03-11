using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DimBoxes;
using UnityEngine;

public class LogicCollisionManager : MonoBehaviour
{


    private static LogicCollisionManager _instance = null;
    public static LogicCollisionManager Instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    private Dictionary<GameObject, Vector3[]> CustomizeCollisionParticipants = null;
    private Dictionary<GameObject, BoundBox> CollisionParticipants = null;
    public static float simulateStep = 0.01f;
    void Awake()
    {
        _instance = this;
        CollisionParticipants = new Dictionary<GameObject, BoundBox>();
        CustomizeCollisionParticipants = new Dictionary<GameObject, Vector3[]>();
    }

    public void AddParticipant(GameObject participantObject)
    {
        if (!CollisionParticipants.ContainsKey(participantObject))
        {
            CollisionParticipants.Add(participantObject, participantObject.GetComponent<BoundBox>());
        }
        else
        {
            CollisionParticipants[participantObject] = participantObject.GetComponent<BoundBox>();
        }
        ResetAllBoundBoxes();
    }
    public void AddCustomizeParticipant(GameObject participantObject, Vector3[] customizeBoundCorners)
    {
        if (!CustomizeCollisionParticipants.ContainsKey(participantObject))
        {
            CustomizeCollisionParticipants.Add(participantObject, customizeBoundCorners);
        }
        else
        {
            CustomizeCollisionParticipants[participantObject] = customizeBoundCorners;
        }
        ResetAllBoundBoxes();
    }

    void RecalculateOBBCorners()
    {
        for (int i = 0; i < CollisionParticipants.Values.Count; i++)
        {
            CollisionParticipants.Values.ToArray()[i].Reset();
        }
    }

    public void ResetAllBoundBoxes()
    {
        for (int i = 0; i < CollisionParticipants.Values.Count; i++)
        {
            CollisionParticipants.Values.ToArray()[i].Reset();
        }
    }

    public bool CollisionDetection(GameObject kinematicParticipantObject)
    {
        ResetAllBoundBoxes();
        if (CollisionParticipants.ContainsKey(kinematicParticipantObject))
        {
            foreach (KeyValuePair<GameObject, BoundBox> collisionParticipant in CollisionParticipants)
            {
                if (collisionParticipant.Key != kinematicParticipantObject)
                {
                    return IntersectBoxBox(collisionParticipant.Value.bound, collisionParticipant.Key.transform.position,
                        collisionParticipant.Key.transform.rotation,
                        CollisionParticipants[kinematicParticipantObject].bound,
                        kinematicParticipantObject.transform.position, kinematicParticipantObject.transform.rotation);
                }
            }
        }
        return false;
    }

    public Vector3 GetSimulatePosititon(GameObject kinematicParticipantObject, Vector3 tarPos)
    {
        //if (CollisionParticipants.ContainsKey(kinematicParticipantObject))
        //{
        //    foreach (KeyValuePair<GameObject, BoundBox> collisionParticipant in CollisionParticipants)
        //    {
        //        if (collisionParticipant.Key != kinematicParticipantObject)
        //        {
        //            if (!IntersectBoxBox(collisionParticipant.Value.bound, collisionParticipant.Key.transform.position,
        //                collisionParticipant.Key.transform.rotation,
        //                CollisionParticipants[kinematicParticipantObject].bound,
        //              tarPos, kinematicParticipantObject.transform.rotation))
        //            {
        //                return tarPos;
        //            }
        //        }
        //    }
        //}
        Vector3 expectOffset = tarPos - kinematicParticipantObject.transform.position;
        float offsetX = 0f;
        float offsetY = 0f;
        float offsetZ = 0f;
        Debug.Log(expectOffset);
        for (int i = 0; i < Mathf.Abs(expectOffset.x / simulateStep); i++)
        {
            //ResetAllBoundBoxes();
            if (CollisionParticipants.ContainsKey(kinematicParticipantObject))
            {
                foreach (KeyValuePair<GameObject, BoundBox> collisionParticipant in CollisionParticipants)
                {
                    if (collisionParticipant.Key != kinematicParticipantObject)
                    {
                        if (
                            !IntersectBoxBox(collisionParticipant.Value.bound,
                                collisionParticipant.Key.transform.position,
                                collisionParticipant.Key.transform.rotation,
                                CollisionParticipants[kinematicParticipantObject].bound,
                                kinematicParticipantObject.transform.position + new Vector3(expectOffset.x * simulateStep * i, 0, 0),
                                kinematicParticipantObject.transform.rotation))
                        {
                            offsetX = expectOffset.x * simulateStep * i;
                        }
                        else
                        {
                            break;
                            break;
                            break;
                        }
                    }
                }
            }

        }
        for (int i = 0; i < Mathf.Abs(expectOffset.y / simulateStep); i++)
        {
            //ResetAllBoundBoxes();
            if (CollisionParticipants.ContainsKey(kinematicParticipantObject))
            {
                foreach (KeyValuePair<GameObject, BoundBox> collisionParticipant in CollisionParticipants)
                {
                    if (collisionParticipant.Key != kinematicParticipantObject)
                    {
                        if (
                            !IntersectBoxBox(collisionParticipant.Value.bound,
                                collisionParticipant.Key.transform.position,
                                collisionParticipant.Key.transform.rotation,
                                CollisionParticipants[kinematicParticipantObject].bound,
                                kinematicParticipantObject.transform.position + new Vector3(0, expectOffset.y * simulateStep * i, 0),
                                kinematicParticipantObject.transform.rotation))
                        {
                            offsetY = expectOffset.y * simulateStep * i;
                        }
                        else
                        {
                            break;
                            break;
                            break;
                        }
                    }
                }
            }

        }
        for (int i = 0; i < Mathf.Abs(expectOffset.z / simulateStep); i++)
        {
            //ResetAllBoundBoxes();
            if (CollisionParticipants.ContainsKey(kinematicParticipantObject))
            {
                foreach (KeyValuePair<GameObject, BoundBox> collisionParticipant in CollisionParticipants)
                {
                    if (collisionParticipant.Key != kinematicParticipantObject)
                    {
                        if (
                            !IntersectBoxBox(collisionParticipant.Value.bound,
                                collisionParticipant.Key.transform.position,
                                collisionParticipant.Key.transform.rotation,
                                CollisionParticipants[kinematicParticipantObject].bound,
                                kinematicParticipantObject.transform.position + new Vector3(0, 0, expectOffset.z * simulateStep * i),
                                kinematicParticipantObject.transform.rotation))
                        {
                            offsetZ = expectOffset.z * simulateStep * i;
                        }
                        else
                        {
                            break;
                            break;
                            break;
                        }
                    }
                }
            }

        }
        Debug.Log("offset : " + new Vector3(offsetX, offsetY, offsetZ));
        return kinematicParticipantObject.transform.position + new Vector3(offsetX, offsetY, offsetZ);
    }
    public bool CollisionDetection(GameObject kinematicParticipantObject, bool detectHeight = true)
    {
        ResetAllBoundBoxes();
        bool collisionTrigger = false;
        if (CollisionParticipants.ContainsKey(kinematicParticipantObject))
        {
            //Debug.Log(SATAxisDetection(SatAxis.X, CollisionParticipants[kinematicParticipantObject].OBBCorners) &&
            //    SATAxisDetection(SatAxis.Z, CollisionParticipants[kinematicParticipantObject].OBBCorners));
            List<GameObject> collisionObjects = SingleAxisDetection(SatAxis.X, CollisionParticipants[kinematicParticipantObject].OBBCorners).Intersect(SingleAxisDetection(SatAxis.Z, CollisionParticipants[kinematicParticipantObject].OBBCorners)).ToList();

            if (detectHeight)
            {
                collisionObjects = collisionObjects.Intersect(SingleAxisDetection(SatAxis.Y, CollisionParticipants[kinematicParticipantObject].OBBCorners)).ToList();
            }

            collisionTrigger = collisionObjects.Count > 0;
            //collisionTrigger = detectHeight && collisionTrigger &&
            //    SATAxisDetection(SatAxis.X, CollisionParticipants[kinematicParticipantObject].OBBCorners) &&
            //    SATAxisDetection(SatAxis.Y, CollisionParticipants[kinematicParticipantObject].OBBCorners);
            return collisionTrigger;
        }
        else
        {
            Debug.LogError("The Kinematic Participant Object is not contained in the collision system.");
            return false;
        }
    }
    public bool CollisionDetection(GameObject kinematicParticipantObject, out List<GameObject> triggerObjects, bool detectHeight = true)
    {
        ResetAllBoundBoxes();
        bool collisionTrigger = false;
        if (CollisionParticipants.ContainsKey(kinematicParticipantObject))
        {
            //Debug.Log(SATAxisDetection(SatAxis.X, CollisionParticipants[kinematicParticipantObject].OBBCorners) &&
            //    SATAxisDetection(SatAxis.Z, CollisionParticipants[kinematicParticipantObject].OBBCorners));
            List<GameObject> collisionObjects = SingleAxisDetection(SatAxis.X, CollisionParticipants[kinematicParticipantObject].OBBCorners).Intersect(SingleAxisDetection(SatAxis.Z, CollisionParticipants[kinematicParticipantObject].OBBCorners)).ToList();

            if (detectHeight)
            {
                collisionObjects = collisionObjects.Intersect(SingleAxisDetection(SatAxis.Y, CollisionParticipants[kinematicParticipantObject].OBBCorners)).ToList();
            }

            collisionTrigger = collisionObjects.Count > 0;
            //collisionTrigger = detectHeight && collisionTrigger &&
            //    SATAxisDetection(SatAxis.X, CollisionParticipants[kinematicParticipantObject].OBBCorners) &&
            //    SATAxisDetection(SatAxis.Y, CollisionParticipants[kinematicParticipantObject].OBBCorners);
            triggerObjects = collisionObjects;
            return collisionTrigger;
        }
        else
        {
            Debug.LogError("The Kinematic Participant Object is not contained in the collision system.");
            triggerObjects = null;
            return false;
        }
    }

    List<GameObject> SingleAxisDetection(SatAxis satAxis, Vector3[] kpOBBCorners)
    {
        List<GameObject> singleAxisCollisionObjects = new List<GameObject>();
        for (int i = 0; i < CollisionParticipants.Count; i++)
        {
            Vector3[] envOBBCorners = CollisionParticipants.Values.ToArray()[i].OBBCorners;
            if (!envOBBCorners.Equals(kpOBBCorners))
            {
                Vector2 envAxisRnage = OffsetAxis(satAxis, envOBBCorners);
                Vector2 kpAxisRnage = OffsetAxis(satAxis, kpOBBCorners);
                float envCenter = (envAxisRnage.x + envAxisRnage.y) / 2f;
                float kpCenter = (kpAxisRnage.x + kpAxisRnage.y) / 2f;
                float range = (Mathf.Abs(envAxisRnage.y - envAxisRnage.x) + Mathf.Abs(kpAxisRnage.y - kpAxisRnage.x)) / 2f;
                float distance = Mathf.Abs(kpCenter - envCenter);
                if (distance < range)
                {
                    singleAxisCollisionObjects.Add(CollisionParticipants.Keys.ToArray()[i]);
                    //Debug.Log(CollisionParticipants.Keys.ToArray()[i].name);
                }
            }
        }
        for (int i = 0; i < CustomizeCollisionParticipants.Count; i++)
        {
            Vector3[] envOBBCorners = CustomizeCollisionParticipants.Values.ToArray()[i];
            if (!envOBBCorners.Equals(kpOBBCorners))
            {
                Vector2 envAxisRnage = OffsetAxis(satAxis, envOBBCorners);
                Vector2 kpAxisRnage = OffsetAxis(satAxis, kpOBBCorners);
                float envCenter = (envAxisRnage.x + envAxisRnage.y) / 2f;
                float kpCenter = (kpAxisRnage.x + kpAxisRnage.y) / 2f;
                float range = (Mathf.Abs(envAxisRnage.y - envAxisRnage.x) + Mathf.Abs(kpAxisRnage.y - kpAxisRnage.x)) / 2f;
                float distance = Mathf.Abs(kpCenter - envCenter);
                if (distance < range)
                {
                    //Debug.Log(CustomizeCollisionParticipants.Keys.ToArray()[i].name);
                    singleAxisCollisionObjects.Add(CollisionParticipants.Keys.ToArray()[i]);
                }
            }
        }
        return singleAxisCollisionObjects;
    }

    Vector2 OffsetAxis(SatAxis satAxis, Vector3[] corners)
    {
        float min = 0f;
        float max = 0f;
        for (int i = 0; i < corners.Length; i++)
        {

            float axisValue;
            switch (satAxis)
            {
                case SatAxis.X:
                    axisValue = corners[i].x;
                    break;
                case SatAxis.Y:
                    axisValue = corners[i].y;
                    break;
                case SatAxis.Z:
                    axisValue = corners[i].z;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("satAxis", satAxis, null);
            }
            if (i == 0)
            {
                min = axisValue;
                max = axisValue;
            }

            min = axisValue < min ? axisValue : min;
            max = axisValue > max ? axisValue : max;
        }
        return new Vector2(min, max);
    }

    public static bool IntersectBoxBox(Bounds box0, Vector3 pos0, Quaternion rot0, Bounds box1, Vector3 pos1, Quaternion rot1)
    {
        Vector3 v = pos1 - pos0;

        //Compute A's basis
        Vector3 VAx = rot0 * new Vector3(1, 0, 0);
        Vector3 VAy = rot0 * new Vector3(0, 1, 0);
        Vector3 VAz = rot0 * new Vector3(0, 0, 1);

        Vector3[] VA = new Vector3[3];
        VA[0] = VAx;
        VA[1] = VAy;
        VA[2] = VAz;

        //Compute B's basis
        Vector3 VBx = rot1 * new Vector3(1, 0, 0);
        Vector3 VBy = rot1 * new Vector3(0, 1, 0);
        Vector3 VBz = rot1 * new Vector3(0, 0, 1);

        Vector3[] VB = new Vector3[3];
        VB[0] = VBx;
        VB[1] = VBy;
        VB[2] = VBz;

        Vector3 T = new Vector3(Vector3.Dot(v, VAx), Vector3.Dot(v, VAy), Vector3.Dot(v, VAz));

        float[,] R = new float[3, 3];
        float[,] FR = new float[3, 3];
        float ra, rb, t;

        for (int i = 0; i < 3; i++)
        {
            for (int k = 0; k < 3; k++)
            {
                R[i, k] = Vector3.Dot(VA[i], VB[k]);
                FR[i, k] = 1e-6f + Mathf.Abs(R[i, k]);
            }
        }

        // A's basis vectors
        for (int i = 0; i < 3; i++)
        {
            ra = box0.extents[i];
            rb = box1.extents[0] * FR[i, 0] + box1.extents[1] * FR[i, 1] + box1.extents[2] * FR[i, 2];
            t = Mathf.Abs(T[i]);
            if (t > ra + rb) return false;
        }

        // B's basis vectors
        for (int k = 0; k < 3; k++)
        {
            ra = box0.extents[0] * FR[0, k] + box0.extents[1] * FR[1, k] + box0.extents[2] * FR[2, k];
            rb = box1.extents[k];
            t = Mathf.Abs(T[0] * R[0, k] + T[1] * R[1, k] + T[2] * R[2, k]);
            if (t > ra + rb) return false;
        }

        //9 cross products

        //L = A0 x B0
        ra = box0.extents[1] * FR[2, 0] + box0.extents[2] * FR[1, 0];
        rb = box1.extents[1] * FR[0, 2] + box1.extents[2] * FR[0, 1];
        t = Mathf.Abs(T[2] * R[1, 0] - T[1] * R[2, 0]);
        if (t > ra + rb) return false;

        //L = A0 x B1
        ra = box0.extents[1] * FR[2, 1] + box0.extents[2] * FR[1, 1];
        rb = box1.extents[0] * FR[0, 2] + box1.extents[2] * FR[0, 0];
        t = Mathf.Abs(T[2] * R[1, 1] - T[1] * R[2, 1]);
        if (t > ra + rb) return false;

        //L = A0 x B2
        ra = box0.extents[1] * FR[2, 2] + box0.extents[2] * FR[1, 2];
        rb = box1.extents[0] * FR[0, 1] + box1.extents[1] * FR[0, 0];
        t = Mathf.Abs(T[2] * R[1, 2] - T[1] * R[2, 2]);
        if (t > ra + rb) return false;

        //L = A1 x B0
        ra = box0.extents[0] * FR[2, 0] + box0.extents[2] * FR[0, 0];
        rb = box1.extents[1] * FR[1, 2] + box1.extents[2] * FR[1, 1];
        t = Mathf.Abs(T[0] * R[2, 0] - T[2] * R[0, 0]);
        if (t > ra + rb) return false;

        //L = A1 x B1
        ra = box0.extents[0] * FR[2, 1] + box0.extents[2] * FR[0, 1];
        rb = box1.extents[0] * FR[1, 2] + box1.extents[2] * FR[1, 0];
        t = Mathf.Abs(T[0] * R[2, 1] - T[2] * R[0, 1]);
        if (t > ra + rb) return false;

        //L = A1 x B2
        ra = box0.extents[0] * FR[2, 2] + box0.extents[2] * FR[0, 2];
        rb = box1.extents[0] * FR[1, 1] + box1.extents[1] * FR[1, 0];
        t = Mathf.Abs(T[0] * R[2, 2] - T[2] * R[0, 2]);
        if (t > ra + rb) return false;

        //L = A2 x B0
        ra = box0.extents[0] * FR[1, 0] + box0.extents[1] * FR[0, 0];
        rb = box1.extents[1] * FR[2, 2] + box1.extents[2] * FR[2, 1];
        t = Mathf.Abs(T[1] * R[0, 0] - T[0] * R[1, 0]);
        if (t > ra + rb) return false;

        //L = A2 x B1
        ra = box0.extents[0] * FR[1, 1] + box0.extents[1] * FR[0, 1];
        rb = box1.extents[0] * FR[2, 2] + box1.extents[2] * FR[2, 0];
        t = Mathf.Abs(T[1] * R[0, 1] - T[0] * R[1, 1]);
        if (t > ra + rb) return false;

        //L = A2 x B2
        ra = box0.extents[0] * FR[1, 2] + box0.extents[1] * FR[0, 2];
        rb = box1.extents[0] * FR[2, 1] + box1.extents[1] * FR[2, 0];
        t = Mathf.Abs(T[1] * R[0, 2] - T[0] * R[1, 2]);
        if (t > ra + rb) return false;

        return true;
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    enum SatAxis
    {
        X, Y, Z
    }
}
