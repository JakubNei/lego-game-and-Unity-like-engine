using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyEngine
{
    public enum ShadowCastingMode
    {
        Off, //No shadows are cast from this object.
        On, //Shadows are cast from this object.
        TwoSided, //Shadows are cast from this object, treating it as two-sided.
        ShadowsOnly, //Object casts shadows, but is otherwise invisible in the scene.
    };
    public abstract class Renderer : Component
    {
        public ShadowCastingMode shadowCastingMode = ShadowCastingMode.On;

        virtual internal bool shouldRenderGeometry { get { return shadowCastingMode != ShadowCastingMode.ShadowsOnly; } }
        virtual internal bool castsShadows { get { return shadowCastingMode != ShadowCastingMode.Off; } }

        public Material material;
        public virtual Bounds bounds { set; get; }
        public bool canBeFrustumCulled = true;
        virtual internal void UploadUBOandDraw(Camera camera, UniformBlock ubo)
        {
        }
    }
}
