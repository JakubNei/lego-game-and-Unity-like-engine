using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

namespace MyEngine
{
    public struct RaycastHit
    {
        internal Vector3 m_Point;
        internal Vector3 m_Normal;
        internal int m_FaceID;
        internal float m_Distance;
        internal Vector2 m_UV;
        internal Collider m_Collider;
        public Vector3 point
        {
            get
            {
                return this.m_Point;
            }
        }
        public Vector3 normal
        {
            get
            {
                return this.m_Normal;
            }
        }
       /* public Vector3 barycentricCoordinate
        {
            get
            {
                return new Vector3(1f - (this.m_UV.Y + this.m_UV.X), this.m_UV.X, this.m_UV.Y);
            }
            set
            {
                this.m_UV = value.Xy;
            }
        }*/
        public float distance
        {
            get
            {
                return this.m_Distance;
            }
            set
            {
                this.m_Distance = value;
            }
        }
       /* public int triangleIndex
        {
            get
            {
                return this.m_FaceID;
            }
        }*/
        public Collider collider
        {
            get
            {
                return this.m_Collider;
            }
        }
        public Rigidbody rigidbody
        {
            get
            {
                return (!(this.collider != null)) ? null : this.collider.attachedRigidbody;
            }
        }
        public Transform transform
        {
            get
            {
                Rigidbody rigidbody = this.rigidbody;
                if (rigidbody != null)
                {
                    return rigidbody.transform;
                }
                if (this.collider != null)
                {
                    return this.collider.transform;
                }
                return null;
            }
        }

     
    }
}
