using System;
using System.Collections.Generic;
using System.Text;

namespace MyEngine
{
    public abstract class MonoBehaviour : Component
    {
        public int updatePerFrame = 1;
        public virtual void Start()
        {
            //Debug.Info(this.GetType());
        }
        public virtual void Update(double deltaTime)
        {
            //Debug.Info(this.GetType());
        }

        public virtual void OnCollisionEnter(Collision collision)
        {
        }
        public virtual void OnCollisionExit(Collision collision)
        {
        }

        private bool shouldRunStart = true;
        internal void Update_Internal(double deltaTime)
        {
            if(shouldRunStart)
            {
                shouldRunStart = false;
                //Debug.Info("start "+this.GetType());
                Start();
            }
            Update(deltaTime);
        }

    }
}
