using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

namespace MyEngine
{
    public abstract class Collider : Component
    {
        public virtual bool enabled { set; get; }
        public Rigidbody attachedRigidbody;
        public bool isTrigger;
        public float contactOffset;
        //public PhysicMaterial material;
        //public PhysicMaterial sharedMaterial;
        //public Vector3 ClosestPointOnBounds(Vector3 position);

        //internal BEPUphysics.Entities.Entity collisionEntityBase;
        /*
        public bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
        {
            hitInfo = new RaycastHit();
            return false;
        }
        */
        internal BEPUphysics.Entities.Entity collisionEntity_generic;

        internal void CollisionEntityCreated(BEPUphysics.Entities.Entity collisionEntity)
        {
            if (collisionEntity == null) return;

            collisionEntity.Tag = this;
            PhysicsUsage.PhysicsManager.instance.Add(collisionEntity);

            collisionEntity.CollisionInformation.Events.ContactCreated += Events_ContactCreated;
            collisionEntity.CollisionInformation.Events.ContactRemoved += Events_ContactRemoved;

            collisionEntity_generic = collisionEntity;
        }


        private Collision GenerateCollision(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair, BEPUphysics.CollisionTests.ContactData contact)
        {
            var otherEntity = other as BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable;
            var otherCollider = otherEntity.Entity.Tag as Collider;

            var collision = new Collision()
            {
                gameObject = otherCollider.gameObject,
                contacts = new Collision.ContactPoint[1],
            };
            collision.contacts[0] = new Collision.ContactPoint()
            {
                normal = contact.Normal,
                point = contact.Position,
                otherCollider = otherCollider,
                thisCollider = this.gameObject.GetComponent<Collider>(),
            };
            return collision;
        }

        internal void Events_ContactCreated(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair, BEPUphysics.CollisionTests.ContactData contact)
        {
            var collision = GenerateCollision(sender, other, pair, contact);
            foreach (var m in this.gameObject.GetComponents<MonoBehaviour>())
            {
                m.OnCollisionEnter(collision);
            }
        }

        internal void Events_ContactRemoved(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair, BEPUphysics.CollisionTests.ContactData contact)
        {
            var collision = GenerateCollision(sender, other, pair, contact);
            foreach (var m in this.gameObject.GetComponents<MonoBehaviour>())
            {
                m.OnCollisionExit(collision);
            }
        }


 
    
    }
}
