using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Input;
using MyEngine;

namespace MyGame
{
    public class CreatePhysicsObject : MonoBehaviour
    {

        public Resource path = "lego/blocks/block_2x3x05.obj";

        static Vector3[] colors = new Vector3[]
        {
            new Vector3(1,1,1),
            new Vector3(1,1,0),
            new Vector3(1,0,1),
            new Vector3(0,1,1),
            new Vector3(1,0,0),
            new Vector3(0,1,0),
            new Vector3(0,0,1),
        };
        int next = 0;
        public override void Update(double deltaTime)
        {
            if(Input.GetKeyDown(Key.Enter))
            {
                /* var go = new GameObject();
                 go.transform.position = this.transform.position;
                 go.transform.rotation = this.transform.rotation;
                 var renderer = go.AddComponent<MeshRenderer>();
                 renderer.mesh = Factory.GetMesh(path);
                 renderer.material.albedo = new Vector4(colors[(next++)%colors.Length]);
                 var rb = go.AddComponent<Rigidbody>();
                 rb.velocity = this.transform.forward*20;
                 go.AddComponent<BoxCollider>();*/

                var go = LegoPiece.Create();

                go.transform.position = this.transform.position;
                go.transform.rotation = this.transform.rotation;

                var rb = go.GetComponent<Rigidbody>();
                if(rb)
                {
                    rb.velocity = this.transform.forward * 20;
                }
            }

        }


        public override void Start()
        {
            var r = new Random();
            int count = 10;
            while (count-- > 0)
            {
                var go=LegoPiece.Create();
                go.transform.position = new Vector3((float)r.NextDouble() * 20 - 10, (float)r.NextDouble() * 5, (float)r.NextDouble() * 20 - 10);
            }
        }
    }
}
