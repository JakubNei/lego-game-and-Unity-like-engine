using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MyEngine
{
    public enum Space
    {
        Self,
        World,
    }
    public partial class Transform : Component
    {
        internal Vector3 m_position = Vector3.Zero;
        internal Vector3 m_scale = Vector3.One;
        internal Quaternion m_rotation = Quaternion.Identity;


        public Vector3 position
        {
            set
            {
                m_position = value;
                gameObject.RaiseOnChanged(ChangedFlags.Position);
            }
            get
            {
                return m_position;
            }
        }
        public Vector3 scale
        {
            set
            {
                m_scale = value;
                gameObject.RaiseOnChanged(ChangedFlags.Scale);
            }
            get
            {
                return m_scale;
            }
        }
        public Quaternion rotation
        {
            set
            {
                m_rotation = value;
                gameObject.RaiseOnChanged(ChangedFlags.Roltation);
            }
            get
            {
                return m_rotation;
            }
        }




        public Vector3 right
        {
            get
            {
                return (-Vector3.UnitX).RotateBy(rotation);
            }
        }
        public Vector3 up
        {
            get
            {
                return (-Vector3.UnitY).RotateBy(rotation);
            }
        }

        public Vector3 forward
        {
            get
            {
                return (-Vector3.UnitZ).RotateBy(rotation);
            }
        }

        private Matrix4 GetScalePosRotMatrix()
        {
            return
                Matrix4.CreateScale(scale) *
                Matrix4.CreateFromQuaternion(rotation) *
                Matrix4.CreateTranslation(position);
        }


        public Matrix4 localToWorldMatrix
        {
            get
            {
                return GetScalePosRotMatrix();
            }
        }
        public Matrix4 worldToLocalMatrix
        {
            get
            {
                return Matrix4.Invert(localToWorldMatrix);
            }
        }

        public void Translate(Vector3 translation, Space relativeTo = Space.Self)
        {
            if (relativeTo == Space.Self)
            {
                var m = Matrix4.CreateTranslation(translation) * Matrix4.CreateFromQuaternion(rotation);
                this.position += m.ExtractTranslation();
            }
            else if (relativeTo == Space.World)
            {
                this.position += translation;
            }
        }

        
        /// <summary>
        /// Transforms position from local space to world space.
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
        public Vector3 TransformPoint(Vector3 local)
        {
            Vector3 world;
            var mat = localToWorldMatrix;
            Vector3.TransformPosition(ref local, ref mat, out world);
            return world;
        }


        /// <summary>
        /// Transforms position from world space to local space. The opposite of Transform.TransformPoint.
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        public Vector3 InverseTransformPoint(Vector3 world)
        {
            Vector3 local;
            var mat = worldToLocalMatrix;
            Vector3.TransformPosition(ref world, ref mat, out local);
            return local;
        }

        public void LookAt(Vector3 worldPosition, Vector3 worldUp)
        {            
            this.rotation = Matrix4.LookAt(this.position, -worldPosition, worldUp).ExtractRotation();
        }
        public void LookAt(Vector3 worldPosition)
        {
            LookAt(worldPosition, Vector3.UnitZ);
        }

        //public Matrix4 GetScalePosRotMatrix()
        //{
        //    return
        //        Matrix4.CreateScale(scale) *
        //        Matrix4.CreateTranslation(position) *
        //        Matrix4.CreateFromQuaternion(rotation);
        //}
        //public Matrix4 GetPosRotMatrix()
        //{
        //    return
        //        Matrix4.CreateTranslation(position) *
        //        Matrix4.CreateFromQuaternion(rotation);
        //}

    }
}
