using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MyEngine
{
    public class Component
    {

     

        public Transform transform
        {
            get
            {
                return gameObject.transform;
            }
        }
        public GameObject gameObject
        {
            get;
            internal set;
        }
        public T GetComponent<T>() where T : Component
        {
            return gameObject.GetComponent<T>();
        }

        public T[] GetComponents<T>() where T : Component
        {
            return gameObject.GetComponents<T>();
        }
        internal Component()
        {
        }
        internal virtual void OnCreated()
        { 
        }
        internal virtual void OnDestroyed() {

        }

     

        public static implicit operator bool (Component c)
        {
            return c != null;
        }
    }
}
