using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MyEngine.ParticleSimulation
{
    internal class ParticleMesh : IUnloadable
    {

 

        bool isOnGPU = false;

        internal void Draw()
        {
            if(!isOnGPU) UploadMeshData();

            GL.Enable(EnableCap.Blend);

            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.ProgramPointSize);
            //GL.PointSize(10);

            GL.BindVertexArray(vertexArrayObjectHandle);
            GL.DrawArrays(PrimitiveType.Points, 0, maxParticles);
            GL.BindVertexArray(0);

            GL.Disable(EnableCap.Blend);
        }

        public void UploadMeshData()
        {
            Unload();
            CreateVAO();
            isOnGPU = true;
        }


        internal uint currentPositionH, currentVelocityH, currentAccelerationH, currentLifeTimeH, startColorH, endColorH, startSizeH, endSizeH, startLifeTimeH;


        public readonly int maxParticles = 1000000;

        internal List<uint> allBufferHandles = new List<uint>();
        int vertexArrayObjectHandle = -1;


        //static uint[] emptyData = new uint[10000000];


        int attribLocation = 0;

        void CreateVAO()
        {
            vertexArrayObjectHandle = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObjectHandle);

            attribLocation = 0;

            currentPositionH = CreateAndBindBuffer(maxParticles, 3, Vector3.SizeInBytes, VertexAttribPointerType.Float);
            currentVelocityH = CreateAndBindBuffer(maxParticles, 3, Vector3.SizeInBytes, VertexAttribPointerType.Float);
            currentAccelerationH = CreateAndBindBuffer(maxParticles, 3, Vector3.SizeInBytes, VertexAttribPointerType.Float);
            currentLifeTimeH = CreateAndBindBuffer(maxParticles, 1, sizeof(float), VertexAttribPointerType.Float);
            startColorH = CreateAndBindBuffer(maxParticles, 4, Vector4.SizeInBytes, VertexAttribPointerType.Float);
            endColorH = CreateAndBindBuffer(maxParticles, 4, Vector4.SizeInBytes, VertexAttribPointerType.Float);
            startSizeH = CreateAndBindBuffer(maxParticles, 1, sizeof(float), VertexAttribPointerType.Float);
            endSizeH = CreateAndBindBuffer(maxParticles, 1, sizeof(float), VertexAttribPointerType.Float);
            startLifeTimeH = CreateAndBindBuffer(maxParticles, 1, sizeof(float), VertexAttribPointerType.Float);


            // set entire currentLifeTimeH to 0
            var zeroFloatArray = new float[maxParticles];
            GL.BindBuffer(BufferTarget.ArrayBuffer, currentLifeTimeH);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)maxParticles, zeroFloatArray, BufferUsageHint.StaticDraw);

            /*
            for (int i = 0; i < zeroFloatArray.Length; i++) zeroFloatArray[i] = -1.0f;
            for (int offset = 0; offset < maxParticles; offset += zeroFloatArray.Length)
            {
                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)offset, (IntPtr)zeroFloatArray.Length, zeroFloatArray);
            }
            */

            GL.BindVertexArray(0);
        }




        uint CreateAndBindBuffer(int countOfAllDataParts, int countOfDataInOnePart, int sizeOfOnePartInBytes, VertexAttribPointerType type)
        {
            uint handle = CreateVBOPart(countOfAllDataParts * sizeOfOnePartInBytes, BufferTarget.ArrayBuffer);

            GL.EnableVertexAttribArray(attribLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            GL.VertexAttribPointer(attribLocation, countOfDataInOnePart, type, false, sizeOfOnePartInBytes, 0);

            attribLocation++;

            return handle;
        }

        uint CreateVBOPart(int size, BufferTarget bt)
        {
            int sizeFromGpu;
            var hande = GLGenBuffer();
            GL.BindBuffer(bt, hande);
            GL.BufferData(bt, (IntPtr)(size), (IntPtr)0, BufferUsageHint.DynamicDraw);
            GL.GetBufferParameter(bt, BufferParameterName.BufferSize, out sizeFromGpu);
            if(size!=sizeFromGpu) Debug.Error("size mismatch size=" + size + " sizeFromGpu=" + sizeFromGpu);
            return hande;
        }




        uint GLGenBuffer()
        {
            uint handle = (uint)GL.GenBuffer();
            allBufferHandles.Add(handle);
            return handle;
        }

        public void Unload()
        {
            if (vertexArrayObjectHandle >= 0)
            {
                GL.DeleteVertexArray(vertexArrayObjectHandle);
                vertexArrayObjectHandle = -1;
            }

            if (allBufferHandles.Count > 0)
            {
                GL.DeleteBuffers(allBufferHandles.Count, allBufferHandles.ToArray());
                allBufferHandles.Clear();
            }
        }

    }
}

