using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MyEngine
{
    public class MeshRenderer : Renderer
    {
        override internal bool shouldRenderGeometry { get { return base.shouldRenderGeometry && material.gBufferShader != null; } }
        override internal bool castsShadows { get { return base.castsShadows && material.depthGrabShader != null; } }


        Mesh _mesh;
        public Mesh mesh
        {
            set
            {
                _mesh = value;
                _mesh.OnChanged += OnMeshHasChanges;
                gameObject.RaiseOnChanged(ChangedFlags.VisualRepresentation);
            }
            get
            {
                _mesh.OnChanged -= OnMeshHasChanges;
                return _mesh;
            }
        }
        MaterialPBR _material;
        public new MaterialPBR material
        {
            set
            {
                base.material = value;
                _material = value;
                gameObject.RaiseOnChanged(ChangedFlags.VisualRepresentation);
            }

            get
            {
                return _material;
            }
        }


        static readonly Vector3[] extentsTransformsToEdges = {
                                                                 new Vector3( 1, 1, 1),
                                                                 new Vector3( 1, 1,-1),
                                                                 new Vector3( 1,-1, 1),
                                                                 new Vector3( 1,-1,-1),
                                                                 new Vector3(-1, 1, 1),
                                                                 new Vector3(-1, 1,-1),
                                                                 new Vector3(-1,-1, 1),
                                                                 new Vector3(-1,-1,-1),
                                                             };
        public override Bounds bounds
        {
            get
            {
                var bounds = new Bounds(this.transform.position, Vector3.Zero);
                var b = this.transform.position + (mesh.bounds.center * this.transform.scale).RotateBy(this.transform.rotation);
                var e = (mesh.bounds.extents * this.transform.scale).RotateBy(this.transform.rotation);
                for (int i = 0; i < 8; i++)
                {
                    bounds.Encapsulate(b + e.CompomentWiseMult(extentsTransformsToEdges[i]));
                }
                return bounds;

                /*var bounds = mesh.bounds;
                bounds.center = bounds.center * transform.scale + transform.position;
                bounds.extents *= transform.scale;
                return bounds;*/
            }
        }


        internal override void OnCreated()
        {
            material = new MaterialPBR()
            {
                gBufferShader = Factory.GetShader("internal/deferred.gBuffer.standart.shader"),
                depthGrabShader = Factory.GetShader("internal/depthGrab.standart.shader"),
            };
        }

        override internal void UploadUBOandDraw(Camera camera, UniformBlock ubo)
        {
            var modelMat = this.gameObject.transform.localToWorldMatrix;
            var modelViewMat = modelMat * camera.GetViewMat();
            ubo.model.modelMatrix = modelMat;
            ubo.model.modelViewMatrix = modelViewMat;
            ubo.model.modelViewProjectionMatrix = modelViewMat * camera.GetProjectionMat();
            ubo.modelUBO.UploadData();
            mesh.Draw();
        }

        void OnMeshHasChanges(ChangedFlags flags)
        {
            gameObject.RaiseOnChanged(flags);
        }


    }
}