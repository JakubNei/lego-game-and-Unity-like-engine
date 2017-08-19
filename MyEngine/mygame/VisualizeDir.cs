using System;
using System.Collections.Generic;
using System.Text;

using MyEngine;
using OpenTK;

namespace MyGame
{
    class VisualizeDir : MonoBehaviour
    {

        public Vector3 offset = new Vector3(0, 10, 0);

        GameObject dirVisualize;

        public override void Start()
        {
            var go = new GameObject();
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.mesh = Factory.GetMesh("sphere.obj");
            renderer.material.albedo = new Vector4(0, 0, 1, 1);
            go.transform.scale *= 0.5f;

            dirVisualize = go;
        }

        public override void Update(double deltaTime)
        {

            dirVisualize.transform.position = this.gameObject.transform.position + this.gameObject.transform.forward*2;

        }
    }
}
