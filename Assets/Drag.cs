using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Drag : MonoBehaviour
{
    public GameObject another;
    Vector3[] custom = new Vector3[8]
    {
        new Vector3(-4,0,-4),
        new Vector3(-4,0,4), 
         new Vector3(-4,4,4), 
          new Vector3(-4,4,-4), 
          new Vector3(-5,0,-5),
        new Vector3(-5,0,5), 
         new Vector3(-5,5,5), 
          new Vector3(-5,5,-5), 
    };

    void Reset()
    {
        setLines();
    }
    // Use this for initialization
    void Start()
    {
        LogicCollisionManager.Instance.AddParticipant(this.gameObject);
        LogicCollisionManager.Instance.AddParticipant(another);
        LogicCollisionManager.Instance.AddCustomizeParticipant(new GameObject("walll"), custom);
        setLines();
    }
#if UNITY_EDITOR
    void OnValidate()
    {
        if (EditorApplication.isPlaying) return;
        setLines();
    }


#endif
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == "Quad")
                {
                    this.gameObject.transform.position = LogicCollisionManager.Instance.GetSimulatePosititon(
                   this.gameObject, new Vector3(hit.point.x, this.transform.position.y, hit.point.z));
                    List<GameObject> triggers;
                    if (LogicCollisionManager.Instance.CollisionDetection(this.gameObject))
                    {
                        Debug.Log(this.gameObject.name + " detect collision true ");
                    }
                    else
                    {
                        // Debug.Log(this.gameObject.name + " detect collision false ");
                    }
                }
               
            }
        }

    }
    private Vector3[,] lines;
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
            _line = new Vector3[] { custom[2 * i], custom[2 * i + 1] };
            _lines.Add(_line);
            //height
            _line = new Vector3[] { custom[i], custom[i + 4] };
            _lines.Add(_line);
            //depth
            _line = new Vector3[] { custom[2 * i], custom[2 * i + 3 - 4 * (i % 2)] };
            _lines.Add(_line);

        }
        lines = new Vector3[_lines.Count, 2];
        for (int j = 0; j < _lines.Count; j++)
        {
            lines[j, 0] = _lines[j][0];
            lines[j, 1] = _lines[j][1];
        }
    }
    void OnDrawGizmos()
    {

        //Gizmos.color = Color.cyan;
        //for (int i = 0; i < lines.GetLength(0); i++)
        //{
        //    Gizmos.DrawLine(lines[i, 0], lines[i, 1]);
        //}
        Gizmos.color = Color.red;
        for (int i = 0; i < custom.Length; i++)
        {
            Gizmos.DrawSphere(custom[i], 0.1f);
        }

    }
}
