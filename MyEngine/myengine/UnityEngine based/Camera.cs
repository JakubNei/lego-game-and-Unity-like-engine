using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MyEngine
{
    public class Camera : Component
    {

        public static Camera main { get; internal set; }

        public float aspect = 0.8f;
        public float fieldOfView = 45.0f;
        public float nearClipPlane = 0.1f;
        public float farClipPlane = 5000;
        public bool orthographic = false;
        public float orthographicSize = 5;
        public int pixelWidth;
        public int pixelHeight;
        Vector2 screenSize = Vector2.Zero;

        internal List<Shader> postProcessEffects = new List<Shader>();

        public void SetSize(int w, int h) {
            this.pixelHeight = h;
            this.pixelWidth = w;
            screenSize = new Vector2(w, h);
            aspect = screenSize.X / screenSize.Y;
        }

        public Matrix4 GetProjectionMat()
        {
            if (orthographic) return Matrix4.CreateOrthographic(orthographicSize * 2, orthographicSize * 2 / aspect, nearClipPlane, farClipPlane);
            return Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 180.0f * fieldOfView, aspect, nearClipPlane, farClipPlane);
        }

        public Matrix4 GetViewMat()
        {
            return
                Matrix4.CreateTranslation(-gameObject.transform.position) *
                Matrix4.CreateFromQuaternion(Quaternion.Invert(gameObject.transform.rotation));
        }

        internal void UploadDataToUBO(UniformBlock ubo)
        {
            ubo.engine.viewMatrix = GetViewMat();
            ubo.engine.projectionMatrix = GetProjectionMat();
            ubo.engine.viewProjectionMatrix = ubo.engine.viewMatrix * ubo.engine.projectionMatrix;
            ubo.engine.cameraPosition = this.gameObject.transform.position;
            ubo.engine.screenSize = this.screenSize;
            ubo.engine.nearClipPlane = this.nearClipPlane;
            ubo.engine.farClipPlane = this.farClipPlane;
            GL.Viewport(0, 0, pixelWidth, pixelHeight);
            ubo.engineUBO.UploadData();
        }

        public void AddPostProcessEffect(Shader shader)
        {
            postProcessEffects.Add(shader);
        }
    }
}
