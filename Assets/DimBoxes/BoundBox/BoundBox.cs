using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DimBoxes
{
    [ExecuteInEditMode]
    public class BoundBox : MonoBehaviour
    {

        public bool colliderBased = false;
        public bool permanent = false; //permanent//onMouseDown

        public Color lineColor = new Color(0f, 1f, 0.4f, 0.74f);

        public Bounds bound;
        private Vector3 boundOffset;
        [HideInInspector]
        public Bounds colliderBound;
        [HideInInspector]
        public Vector3 colliderBoundOffset;
        [HideInInspector]
        public Bounds meshBound;
        [HideInInspector]
        public Vector3 meshBoundOffset;

        public bool setupOnAwake = false;

        private Vector3[] corners;

        private Vector3[,] lines;

        private Quaternion quat;
        public float FixedLineWidth = 0.02f;

        public float DefalutDistance = 1f;
        private DimBoxes.DrawLines cameralines;

        private MeshFilter[] meshes;

        private Vector3 topFrontLeft;
        private Vector3 topFrontRight;
        private Vector3 topBackLeft;
        private Vector3 topBackRight;
        private Vector3 bottomFrontLeft;
        private Vector3 bottomFrontRight;
        private Vector3 bottomBackLeft;
        private Vector3 bottomBackRight;

        [HideInInspector]
        public Vector3 startingScale;
        private Vector3 previousScale;

        private Vector3 previousPosition;
        private Quaternion previousRotation;

        public Vector3[] OBBCorners;
        private List<LineRenderer> lineRenders;
        public void Reset()
        {
            meshes = GetComponentsInChildren<MeshFilter>();
            calculateBounds();
            Start();
        }

        void Awake()
        {
            if (setupOnAwake)
            {
                meshes = GetComponentsInChildren<MeshFilter>();
                calculateBounds();
            }
        }

        void Start()
        {
            cameralines = FindObjectOfType(typeof(DimBoxes.DrawLines)) as DimBoxes.DrawLines;

            if (!cameralines)
            {
                Debug.LogError("DimBoxes: no camera with DimBoxes.DrawLines in the scene", gameObject);
                return;
            }


            previousPosition = transform.position;
            previousRotation = transform.rotation;
            startingScale = transform.localScale;
            previousScale = startingScale;
            init();
            lineRenders = this.GetComponentsInChildren<LineRenderer>().ToList();
        }

        public void init()
        {
            setPoints();
            setLines();
            CalculateOBBCorners();
        }

        void LateUpdate()
        {
            Reset();
            if (transform.localScale != previousScale)
            {
                scaleBounds();
                setPoints();
            }
            if (transform.position != previousPosition || transform.rotation != previousRotation || transform.localScale != previousScale)
            {
                setLines();
                previousRotation = transform.rotation;
                previousPosition = transform.position;
                previousScale = transform.localScale;
            }
            cameralines.setOutlines(lines, lineColor, new Vector3[0, 0]);
        }

        public void scaleBounds()
        {
            //bound.size = new Vector3(startingBoundSize.x * transform.localScale.x / startingScale.x, startingBoundSize.y * transform.localScale.y / startingScale.y, startingBoundSize.z * transform.localScale.z / startingScale.z);
            //bound.center = transform.TransformPoint(startingBoundCenterLocal);
        }

        void calculateBounds()
        {
            quat = transform.rotation;//object axis AABB
            meshBound = new Bounds();
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            for (int i = 0; i < meshes.Length; i++)
            {
                Mesh ms = meshes[i].sharedMesh;
                int vc = ms.vertexCount;
                for (int j = 0; j < vc; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        meshBound = new Bounds(meshes[i].transform.TransformPoint(ms.vertices[j]), Vector3.zero);
                    }
                    else
                    {
                        meshBound.Encapsulate(meshes[i].transform.TransformPoint(ms.vertices[j]));
                    }
                }
            }
            transform.rotation = quat;
            meshBoundOffset = meshBound.center - transform.position;
        }

        void setPoints()
        {

            if (colliderBased)
            {
                if (colliderBound == null)
                {
                    Debug.LogError("no collider - add collider to " + gameObject.name + " gameObject");
                    return;

                }
                bound = colliderBound;
                boundOffset = colliderBoundOffset;
            }

            else
            {
                bound = meshBound;
                boundOffset = meshBoundOffset;
            }

            bound.size = new Vector3(bound.size.x * transform.localScale.x / startingScale.x, bound.size.y * transform.localScale.y / startingScale.y, bound.size.z * transform.localScale.z / startingScale.z);
            boundOffset = new Vector3(boundOffset.x * transform.localScale.x / startingScale.x, boundOffset.y * transform.localScale.y / startingScale.y, boundOffset.z * transform.localScale.z / startingScale.z);


            topFrontRight = boundOffset + Vector3.Scale(bound.extents, new Vector3(1, 1, 1));
            topFrontLeft = boundOffset + Vector3.Scale(bound.extents, new Vector3(-1, 1, 1));
            topBackLeft = boundOffset + Vector3.Scale(bound.extents, new Vector3(-1, 1, -1));
            topBackRight = boundOffset + Vector3.Scale(bound.extents, new Vector3(1, 1, -1));
            bottomFrontRight = boundOffset + Vector3.Scale(bound.extents, new Vector3(1, -1, 1));
            bottomFrontLeft = boundOffset + Vector3.Scale(bound.extents, new Vector3(-1, -1, 1));
            bottomBackLeft = boundOffset + Vector3.Scale(bound.extents, new Vector3(-1, -1, -1));
            bottomBackRight = boundOffset + Vector3.Scale(bound.extents, new Vector3(1, -1, -1));

            corners = new Vector3[] { topFrontRight, topFrontLeft, topBackLeft, topBackRight, bottomFrontRight, bottomFrontLeft, bottomBackLeft, bottomBackRight };

        }

        void setLines()
        {

            Quaternion rot = transform.rotation;
            Vector3 pos = transform.position;

            List<Vector3[]> _lines = new List<Vector3[]>();
            //int linesCount = 12;

            Vector3[] _line;
            for (int i = 0; i < 4; i++)
            {
                //width
                _line = new Vector3[] { rot * corners[2 * i] + pos, rot * corners[2 * i + 1] + pos };
                _lines.Add(_line);
                //height
                _line = new Vector3[] { rot * corners[i] + pos, rot * corners[i + 4] + pos };
                _lines.Add(_line);
                //depth
                _line = new Vector3[] { rot * corners[2 * i] + pos, rot * corners[2 * i + 3 - 4 * (i % 2)] + pos };
                _lines.Add(_line);

            }
            lines = new Vector3[_lines.Count, 2];
            for (int j = 0; j < _lines.Count; j++)
            {
                lines[j, 0] = _lines[j][0];
                lines[j, 1] = _lines[j][1];
            }
        }

        void OnMouseDown()
        {
            if (permanent) return;
            enabled = !enabled;
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (EditorApplication.isPlaying) return;
            init();
        }


#endif

        void CalculateOBBCorners()
        {
            OBBCorners = new Vector3[8];
            for (int i = 0; i < corners.Length; i++)
            {
                OBBCorners[i] = (transform.rotation * corners[i] + transform.position);
            }
        }
        void OnDrawGizmos()
        {

            Gizmos.color = lineColor;
            for (int i = 0; i < lines.GetLength(0); i++)
            {
                //Gizmos.DrawLine(lines[i, 0], lines[i, 1]);
            }

            Gizmos.color = Color.red;
            for (int i = 0; i < OBBCorners.Length; i++)
            {
                // Gizmos.DrawSphere(OBBCorners[i], 0.1f);
                Debug.Log(OBBCorners[i] + i.ToString());
            }
            //if (line != null)
            //{
            //    line.SetPosition(0, OBBCorners[5]);//前左下
            //    line.SetPosition(1, OBBCorners[4]);//前右下
            //    line.SetPosition(2, OBBCorners[7]);//后右下
            //    line.SetPosition(3, OBBCorners[6]);//后左下
            //    line.SetPosition(4, OBBCorners[5]);
            //    line.SetPosition(5, OBBCorners[1]);//前左上
            //    line.SetPosition(6, OBBCorners[0]);//前右上
            //    line.SetPosition(7, OBBCorners[3]);//后右上
            //    line.SetPosition(8, OBBCorners[2]);//后左上

            //    line.SetPosition(9, OBBCorners[1]);
            //    line.SetPosition(10, OBBCorners[2]);
            //    line.SetPosition(11, OBBCorners[6]);
            //    line.SetPosition(12, OBBCorners[7]);
            //    line.SetPosition(13, OBBCorners[3]);
            //    line.SetPosition(14, OBBCorners[0]);
            //    line.SetPosition(15, OBBCorners[4]);
            //}
            if (lineRenders.Count > 0)
            {
                for (int i = 0; i < lineRenders.Count; i++)
                {

                    lineRenders[i].startWidth = FixedLineWidth * Vector3.Distance(Camera.main.transform.position, lines[i, 0]) / DefalutDistance;
                    lineRenders[i].endWidth = FixedLineWidth * Vector3.Distance(Camera.main.transform.position, lines[i, 1]) / DefalutDistance;
                    lineRenders[i].SetPosition(0, lines[i, 0]);
                    lineRenders[i].SetPosition(1, lines[i, 1]);
                }
            }
        }

        public LineRenderer line;
        void Update()
        {


        }
    }
}
