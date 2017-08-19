using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using MyEngine;

namespace MyGame
{
    class SSAO : MonoBehaviour
    {
        public override void Start()
        {
            //var shader = Factory.GetShader("postProcessEffects/SSAO.shader");
            
            //shader.SetUniform("testColor", new Vector3(0, 1, 0));

            //Camera.main.AddPostProcessEffect(shader);
        }

        public override void Update(double deltaTime)
        {
            
        }
    }
}
