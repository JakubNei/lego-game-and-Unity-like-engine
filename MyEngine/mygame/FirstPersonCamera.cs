using System;
using System.IO;
using System.Drawing;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


using MyEngine;

namespace MyGame
{
    public class FirstPersonCamera : MonoBehaviour
    {

        public float velocityChangeSpeed = 10.0f;

        private Vector3 up = Vector3.UnitY;
        private float pitch = 0.0f;
        private float facing = 0.0f;

        public bool disabledInput = false;

        float speedModifier = 1.0f;
        Point lastMousePos;
        int scrollWheelValue;
        Vector3 currentVelocity;

        public override void Start()
        {
            Screen.lockCursor = disabledInput;
        }

        public override void Update(double deltaTime)
        {

            
 

            var mouse = Mouse.GetState();

            var mouseDelta = new Point(mouse.X - lastMousePos.X, mouse.Y - lastMousePos.Y);
            lastMousePos = new Point(mouse.X, mouse.Y);

            int scrollWheelDelta = mouse.ScrollWheelValue - scrollWheelValue;
            scrollWheelValue = mouse.ScrollWheelValue;


            if (Input.GetKeyDown(Key.Tab))
            {
                disabledInput = !disabledInput;
                Screen.lockCursor = disabledInput;
            }
            
            if (disabledInput) return;

            speedModifier += scrollWheelDelta;
            speedModifier = Math.Max(1.0f, speedModifier);
          
            
            /*
            var p = System.Windows.Forms.Cursor.Position;
            p.X -= mouseDelta.X;
            p.Y -= mouseDelta.Y;
            System.Windows.Forms.Cursor.Position = p;*/


            float c = 1f * (float)deltaTime;
            facing += mouseDelta.X * c;
            pitch += mouseDelta.Y * c;

            const float m = (float)Math.PI / 180.0f * 80.0f;
            if (pitch > m) pitch = m;
            if (pitch < -m) pitch = -m;

            var rot = Matrix4.CreateFromQuaternion(                
                Quaternion.FromAxisAngle(Vector3.UnitY, -facing) *
                Quaternion.FromAxisAngle(Vector3.UnitX, -pitch)
            );
                

            gameObject.transform.rotation = rot.ExtractRotation();


            float d = speedModifier * (float)deltaTime;

            if (Input.GetKey(Key.ShiftLeft)) d *= 5;

            var targetVelocity = Vector3.Zero;
            if (Input.GetKey(Key.W)) targetVelocity.Z -= d;
            if (Input.GetKey(Key.S)) targetVelocity.Z += d;
            if (Input.GetKey(Key.D)) targetVelocity.X += d;
            if (Input.GetKey(Key.A)) targetVelocity.X -= d;
            if (Input.GetKey(Key.Space)) targetVelocity.Y += d;
            if (Input.GetKey(Key.ControlLeft)) targetVelocity.Y -= d;

            //var pos = Matrix4.CreateTranslation(targetVelocity);

            targetVelocity = Vector3.TransformPosition(targetVelocity, rot);


            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, velocityChangeSpeed * (float)deltaTime);

            gameObject.transform.position += currentVelocity;

            //Debug.Info(gameObject.transform.position);

            
        }

    }
}
