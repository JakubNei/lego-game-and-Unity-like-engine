using System;
using System.Collections.Generic;
using System.Text;

using MyEngine;
using OpenTK;

namespace MyGame
{
    class VisualizePosition : MonoBehaviour
    {


        GameObject target;
        Vector3 targetsLocalPosition;

        public static void Create(GameObject target, Vector3 targetsLocalPosition)
        {
            var go = new GameObject();
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.mesh = Factory.GetMesh("sphere.obj");
            renderer.material.albedo = new Vector4(0, 0, 1, 1);
            go.transform.scale *= 0.5f;

            var vp=go.AddComponent<VisualizePosition>();
            vp.target = target;
            vp.targetsLocalPosition = targetsLocalPosition;

        }

        public override void Update(double deltaTime)
        {
            transform.position = target.transform.position + targetsLocalPosition.RotateBy(target.transform.rotation);
        }
    }
}
