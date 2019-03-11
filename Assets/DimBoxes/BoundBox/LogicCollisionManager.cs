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
