using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEngine.ParticleSimulation
{
    public class ParticleMeshRenderer : Renderer
    {

        override internal bool shouldRenderGeometry { get { return base.shouldRenderGeometry && material.gBufferShader != null; } }
        override internal bool castsShadows { get { return base.castsShadows && material.depthGrabShader != null; } }

        MaterialPBR _material;
        public new MaterialPBR material
        {
            set
            {
                base.material = value;
                _material = value;
            }

            get
            {
                return _material;
            }
        }

        internal ParticleMesh particleMesh;

        public ParticleMeshRenderer()
        {
            material = new MaterialPBR()
            {
                gBufferShader = Factory.GetShader("internal/deferred.gBuffer.particle.shader"),
                depthGrabShader = null,
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
            particleMesh.Draw();            
        }

    }
}
