using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using MyEngine;

namespace MyGame
{
    public class DebugShowPlanes : MonoBehaviour
    {
        List<GameObject> gos = new List<GameObject>();
        public override void Start()
        {
            for (int i = 0; i < 6; i++)
            {
                var go = new GameObject();
                gos.Add(go);
                var r = go.AddComponent<MeshRenderer>();
                r.mesh = Factory.GetMesh("internal/cube.obj");
                go.transform.scale = new Vector3(10, 10, 1);
            }
        }
        public override void Update(double deltaTime)
        {
            var p = GeometryUtility.CalculateFrustumPlanes(GetComponent<Camera>());

            for (int i = 0; i < 6; i++)
            {
                // is broken maybe, furstum culling works but this doesnt make much sense
                //gos[i].transform.position = p[i].normal * p[i].distance;
                gos[i].transform.rotation = QuaternionUtility.LookRotation(p[i].normal);
            }
        }
    }
}
