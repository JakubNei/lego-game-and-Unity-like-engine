using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MyEngine;
using OpenTK;
using OpenTK.Input;

namespace MyGame
{
    public class DragLegoPieces : MonoBehaviour
    {
        public GameObject visualiseHitTarget;



        LegoPiece clickedLegoPiece;


        LegoPiece lastLegoPiece;
        Vector4 lastAlbedo;


        public override void Update(double deltaTime)
        {

           

            var ray = new Ray(this.transform.position, this.transform.forward);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit))
            {
                visualiseHitTarget.transform.position = raycastHit.point;
                visualiseHitTarget.transform.rotation = QuaternionUtility.LookRotation(raycastHit.normal);
                var legoPiece = raycastHit.collider.gameObject.GetComponent<LegoPiece>();
                if (legoPiece)
                {
                    if (legoPiece != lastLegoPiece)
                    {
                        if(lastLegoPiece) lastLegoPiece.GetComponent<MeshRenderer>().material.albedo = lastAlbedo;
                        lastLegoPiece = legoPiece;

                        var m = lastLegoPiece.GetComponent<MeshRenderer>().material;
                        lastAlbedo = m.albedo;
                        const float T = 1.0f;
                        const float F = 0.3f;
                        if (clickedLegoPiece && clickedLegoPiece != lastLegoPiece) m.albedo = new Vector4(F,T,F,1);
                        else m.albedo = new Vector4(T, F,F,1);
                    }

                    if (clickedLegoPiece && lastLegoPiece && clickedLegoPiece != lastLegoPiece)
                    {
                        clickedLegoPiece.VizualizeConnectionTo(lastLegoPiece, visualiseHitTarget.transform.position);
                    }
                }
                else
                {
                    if (lastLegoPiece) lastLegoPiece.GetComponent<MeshRenderer>().material.albedo = lastAlbedo;
                    lastLegoPiece = null;
                }
            }
            else
            {
                if (lastLegoPiece) lastLegoPiece.GetComponent<MeshRenderer>().material.albedo = lastAlbedo;
                lastLegoPiece = null;
            }


            if (Input.GeMouseButtonDown(MouseButton.Left))
            {
                if (lastLegoPiece == null)
                {
                    if (clickedLegoPiece != null)
                    {
                        clickedLegoPiece.EndVisualise();
                        var p = visualiseHitTarget.transform.position - clickedLegoPiece.GetComponent<MeshRenderer>().mesh.bounds.center;
                        p.Y += 0.5f;
                        clickedLegoPiece.transform.position = p;
                        clickedLegoPiece.transform.rotation = visualiseHitTarget.transform.rotation;
                        clickedLegoPiece = null;
                    }
                }
                else
                {
                    if (clickedLegoPiece != null)
                    {
                        if (clickedLegoPiece != lastLegoPiece)
                        {
                            clickedLegoPiece.VizualizeConnectionTo(lastLegoPiece, visualiseHitTarget.transform.position);
                            clickedLegoPiece.ConnectTo(lastLegoPiece);
                            clickedLegoPiece = null;
                        }
                    }
                    else
                    {
                        clickedLegoPiece = lastLegoPiece;
                        clickedLegoPiece.StartVisualise();
                    }
                }
            }


            if (Input.GeMouseButton(MouseButton.Right))
            {
                if (lastLegoPiece)
                {
                    var rb = lastLegoPiece.GetComponent<Rigidbody>();
                    if (rb)
                    {
                        rb.AddForceAtPosition(transform.forward*100, transform.position);
                    }
                }
            }

            if (Input.GetKeyDown(Key.L))
            {
                Debug.Info("a");
                Physics.IgnoreCollision(lastLegoPiece.GetComponent<Collider>(), clickedLegoPiece.GetComponent<Collider>(), true);
            }



            if (Input.GetKey(Key.AltLeft))
            {
                MyEngine.ParticleSimulation.Manager.instance.GenerateParticles(1000, visualiseHitTarget.transform.position, new Vector4(1,1,1,1), new Vector4(1,1,1,1), 1000,1, 1);
            }

        }
    }
}
