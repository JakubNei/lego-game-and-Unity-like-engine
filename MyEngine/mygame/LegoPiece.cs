using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MyEngine;
using OpenTK;

namespace MyGame
{
    public class LegoPiece : MonoBehaviour
    {

        public LegoPiece attachedTo;

        const float T = 0.8f;
        const float F = 0.3f;

        public static Vector4[] colors = new Vector4[]
        {
            new Vector4(T,T,T,1),
            new Vector4(T,T,F,1),
            new Vector4(T,F,T,1),
            new Vector4(F,T,T,1),
            new Vector4(T,F,F,1),
            new Vector4(F,T,F,1),
            new Vector4(F,F,T,1),
        };
        static int nextColor = 0;

        static ResourceFolder pathToPieces = "lego/blocks";

        public Vector4 myColor;


        List<Vector3> slots = new List<Vector3>();
        List<Vector3> points = new List<Vector3>();


        LegoPiece connectedTo;
        List<LegoPiece> connectedToMe = new List<LegoPiece>();
        static Random random = new Random();
        public static GameObject Create()
        {
        
            DirectoryInfo di = new DirectoryInfo(pathToPieces);
            FileInfo[] files = di.GetFiles("*.obj");

            
            var file = files[random.Next() % files.Length];

            var go = new GameObject();
            
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.mesh = Factory.GetMesh(Resource.WithAllPathsAs(file.FullName));
            var c = new Vector4(colors[(nextColor++) % colors.Length]);
            renderer.material.albedo = c;

            /*var b = renderer.mesh.bounds;
            b.max = b.max - new Vector3(0, 0.35f, 0);
            renderer.mesh.bounds = b;*/


            var rb = go.AddComponent<Rigidbody>();
            go.AddComponent<BoxCollider>();
            var lb=go.AddComponent<LegoPiece>();
            lb.myColor = renderer.material.albedo;
            lb.myColor.W = 1;
            //lb.myColor = new Vector4(1, 1, 1, 1);

            return go;
        }

        /*void OnChanged(ChangedFlags flags)
        {
            if (flags.HasFlag(ChangedFlags.Position))
            {
                PositionWarped();
            }
        }*/

        public override void Start()
        {

            var m = GetComponent<MeshRenderer>();
            if (m)
            {
                var b=m.mesh.bounds;                

                const float halfSize = 1.139775f;

                int xCount = (int)Math.Round( b.extents.X / halfSize );
                int zCount = (int)Math.Round( b.extents.Z / halfSize );

                for (int x = 0; x < xCount; x++)
                {
                    for (int z = 0; z < zCount; z++)
                    {
                        var slot = b.min + new Vector3(halfSize + x * halfSize * 2, 0, halfSize + z * halfSize * 2);
                        slots.Add(slot);                        

                        var point = slot;
                        point.Y = b.max.Y;
                        point.Y -= 0.45f;
                        points.Add(point);

                        //VisualizePosition.Create(gameObject, point); VisualizePosition.Create(gameObject, slot);
                    }
                }

            }
            
            
        }


        FixedJoint connectionJoint;

        public Vector3 FindClosestSlot(Vector3 localPosition)
        {
            var closestPos = slots[0];
            var closestDistance = closestPos.Distance(localPosition);
            foreach (var s in slots)
            {
                var d = s.Distance(localPosition);
                if (d < closestDistance)
                {
                    closestDistance = d;
                    closestPos = s;
                }
            }
            return closestPos;
        }

        public Vector3 FindClosestPoint(Vector3 localPosition)
        {
            var closestPos = points[0];
            var closestDistance = closestPos.Distance(localPosition);
            foreach (var s in points)
            {
                var d = s.Distance(localPosition);
                if (d < closestDistance)
                {
                    closestDistance = d;
                    closestPos = s;
                }
            }
            return closestPos;
        }

        public void EndVisualise()
        {
            gameObject.GetComponent<BoxCollider>().enabled = true;
            //gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
        public void StartVisualise()
        {
            gameObject.GetComponent<BoxCollider>().enabled = false;
            //gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }


        LegoPiece connectedToPoint;
        Vector3 connecedToLocalPoint;
        Vector3 myLocalSlot;
        public void VizualizeConnectionTo(LegoPiece other, Vector3 targetPos)
        {
            this.transform.rotation = other.transform.rotation;
            connectedToPoint = other;
            connecedToLocalPoint = other.FindClosestPoint(other.transform.InverseTransformPoint(targetPos));
            Vector3 worldPoint = other.transform.TransformPoint(connecedToLocalPoint);
            myLocalSlot = this.FindClosestSlot(this.transform.InverseTransformPoint(worldPoint));
            SetPositionMySlotPosition();
        }
        public void SetPositionMySlotPosition()
        {
            Vector3 worldPoint = connectedToPoint.transform.TransformPoint(connecedToLocalPoint);
            Vector3 worldSlot = this.transform.TransformPoint(myLocalSlot);
            this.transform.position += worldPoint - worldSlot;
            this.transform.rotation = connectedToPoint.transform.rotation;
        }

        public void ConnectTo(LegoPiece other)
        {
            var p = transform.position;

            Disconnect();

            connectedTo = other;

            GetComponent<BoxCollider>().enabled = true;
            Physics.IgnoreCollision(this.GetComponent<Collider>(), connectedTo.GetComponent<Collider>(), true);

            transform.position = p;

            connectionJoint = gameObject.AddComponent<FixedJoint>();
            connectionJoint.connectedTo = connectedTo.GetComponent<Rigidbody>();

            transform.position = p;
            
            connectedTo.connectedToMe.Add(this);

            PositionWarped();
        }

        public void PositionWarped()
        {
            foreach (var lp in connectedToMe)
            {
                lp.SetPositionMySlotPosition();
                lp.PositionWarped();
            }
        }


        public bool IsConnected()
        {
            return connectedTo != null;
        }
        public void Disconnect()
        {
            if (!IsConnected()) return;

            gameObject.DestroyComponent(connectionJoint);

            Physics.IgnoreCollision(GetComponent<Collider>(), connectedTo.GetComponent<Collider>(), false);

            connectedTo.connectedToMe.Remove(this);
            connectedTo = null;
        }



        public override void OnCollisionEnter(Collision collision)
        {
            //GetComponent<MeshRenderer>().material.albedo = new Vector4(0, 0, 0, 0)
            //var r = collision.gameObject.GetComponent<MeshRenderer>(); if (r) r.material.albedo = new Vector4(0, 0, 0, 0);

            foreach(var cp in collision.contacts) {
                //MyEngine.ParticleSimulation.Manager.instance.GenerateParticles(100, cp.point, myColor, new Vector4(1,1,1,1), 1000,1, 1);
            }
        }
    }
}
