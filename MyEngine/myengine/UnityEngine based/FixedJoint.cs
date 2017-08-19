using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyEngine
{
    public class FixedJoint : Joint
    {
        Rigidbody m_connectedTo;
        public Rigidbody connectedTo
        {
            set
            {
                if (physicsJoint!=null)
                {
                    PhysicsUsage.PhysicsManager.instance.Remove(physicsJoint);
                    physicsJoint = null;
                }

                m_connectedTo = value;

                if (value)
                {

                    var a = this.GetComponent<Collider>();
                    if (!a) return;

                    var b = connectedTo.GetComponent<Collider>();
                    if (!b) return;

                    physicsJoint = new BEPUphysics.Constraints.SolverGroups.WeldJoint(a.collisionEntity_generic, b.collisionEntity_generic);
                    physicsJoint.BallSocketJoint.SpringSettings.Stiffness = 10000000;
                    physicsJoint.NoRotationJoint.SpringSettings.Stiffness = physicsJoint.BallSocketJoint.SpringSettings.Stiffness;

                    PhysicsUsage.PhysicsManager.instance.Add(physicsJoint);
                }
            }
            get
            {
                return m_connectedTo;
            }
        }

        BEPUphysics.Constraints.SolverGroups.WeldJoint physicsJoint;


        internal override void OnDestroyed()
        {
            connectedTo = null;
        }
    }
}
