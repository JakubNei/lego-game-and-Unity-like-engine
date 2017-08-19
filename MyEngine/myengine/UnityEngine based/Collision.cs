using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

namespace MyEngine
{
    // http://docs.unity3d.com/ScriptReference/Collision.html
    public class Collision
    {
        

        public class ContactPoint {
            public Vector3 normal;
            public Collider otherCollider;
            public Vector3 point;
            public Collider thisCollider;

        }
        public ContactPoint[] contacts;
        public GameObject gameObject;

        public Vector3 relativeVelocity;


        public Collider collider { get { return gameObject.GetComponent<Collider>(); } }
        public Rigidbody rigidbody { get { return gameObject.GetComponent<Rigidbody>(); } }
        public Transform transform { get { return gameObject.transform; } }

    }
}
