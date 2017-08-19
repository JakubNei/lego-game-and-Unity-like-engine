using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MyEngine;

namespace MyGame
{
    public class DrawBounds : MonoBehaviour
    {
        public Transform drawForThisTransform;

        public static void ForGameObject(GameObject go)
        {
            var bb = new GameObject();
            var ft = bb.AddComponent<DrawBounds>();
            ft.drawForThisTransform = go.transform;
            var mr = bb.AddComponent<MeshRenderer>();
            mr.mesh = Factory.GetMesh("internal/cube.obj");
        }
        public override void Update(double deltaTime)
        {
            var r = drawForThisTransform.GetComponent<MeshRenderer>();
            if (r)
            {
                this.transform.scale = r.mesh.bounds.extents;

                this.transform.rotation = drawForThisTransform.rotation;
                this.transform.position = drawForThisTransform.position + r.mesh.bounds.center.RotateBy(transform.rotation);                
            }
            else
            {
                this.transform.position = drawForThisTransform.position;
                this.transform.rotation = drawForThisTransform.rotation;
            }
        }

    }
}
