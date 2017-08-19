using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MyEngine
{
    public class GameObject : Component
    {

        public Transform transform { get; private set; }

        public GameObject(string name = "")
        {
            Factory.allGameObjects.Add(this);
            transform = this.AddComponent<Transform>();
        }

        public List<Component> components = new List<Component>();
        public T GetComponent<T>() where T : Component
        {
            foreach (var c in components)
            {
                if (c is T)
                {
                    return c as T;
                }
            }
            return null;
        }

        public T[] GetComponents<T>() where T : Component
        {
            List<T> ret = new List<T>();
            foreach (var c in components)
            {
                if (c is T)
                {
                    ret.Add(c as T);
                }
            }
            return ret.ToArray();
        }

        public T AddComponent<T>() where T : Component, new()
        {
            T c = new T();
            c.gameObject = this;
            c.OnCreated();
            components.Add(c);
            return c;
        }

        public void DestroyComponent<T>(T component) where T : Component {
            if (component) component.OnDestroyed();
        }


        public Action<ChangedFlags> OnChanged;

        internal void RaiseOnChanged(ChangedFlags flags)
        {
            if (OnChanged != null) OnChanged(flags);
        }
    }

    public enum ChangedFlags
    {
        Position = 1<<0,
        Roltation = 1 << 1,
        Scale = 1 << 2,
        Bounds = 1 << 3,
        VisualRepresentation = 1 << 4,
        PhysicsSettings = 1 << 5,
        PhysicalShape = 1 << 6,
        All = 0xffff
    }
}
