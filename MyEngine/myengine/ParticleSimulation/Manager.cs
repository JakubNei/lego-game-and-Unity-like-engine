using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

using ManagedCuda;
using ManagedCuda.BasicTypes;
using ManagedCuda.VectorTypes;

namespace MyEngine.ParticleSimulation
{
    public class Manager
    {
        public static Manager instance;

        CudaKernel updateParticles;
        CudaKernel generateParticles;

        List<CudaOpenGLBufferInteropResource> resources = new List<CudaOpenGLBufferInteropResource>();

        ParticleMeshRenderer renderer;
        public Manager()
        {

            var go = new GameObject();
            renderer = go.AddComponent<ParticleMeshRenderer>();

            renderer.particleMesh = new ParticleMesh();
            renderer.particleMesh.UploadMeshData();
            renderer.material.albedoTexture = Factory.GetTexture2D("internal/Cloud-particle.png");
            renderer.canBeFrustumCulled = false;

            InitKernels();

            instance = this;
        }

        void InitKernels()
        {

            var path = @"..\..\..\CudaParticleSimulation\kernel.ptx";
            if (!System.IO.File.Exists(path))
            {
                Debug.Error(path + " doesnt exists");
                return;
            }

            var cntxt = new CudaContext();

            uint deviceCount = 1;
            var devices = new CUdevice[50];
            OpenGLNativeMethods.CUDA3.cuGLGetDevices(ref deviceCount, devices, 50, CUGLDeviceList.All);

            var context = cntxt.Context;
            OpenGLNativeMethods.CUDA3.cuGLCtxCreate(ref context, CUCtxFlags.BlockingSync, devices[0]);

            Debug.Info("Found " + deviceCount + " OpenGL devices associated with current context");
            

            CUmodule cumodule = cntxt.LoadModule(path);

            updateParticles = new CudaKernel("updateParticles", cumodule, cntxt);           
            updateParticles.BlockDimensions = new dim3(16*16,1,1);
            updateParticles.GridDimensions = new dim3(16*16,1,1);


            generateParticles = new CudaKernel("generateParticles", cumodule, cntxt);
            generateParticles.BlockDimensions = updateParticles.BlockDimensions;
            generateParticles.GridDimensions = updateParticles.GridDimensions;

            var random = new Random();
            var randomFloats = new float[1000];
            for (int i = 0; i < randomFloats.Length; i++) randomFloats[i] = (float)random.NextDouble();

            generateParticles.SetConstantVariable("randomFloats", randomFloats);
            
            // CudaGraphicsInteropResourceCollection

            resources.Clear();
            foreach (var h in renderer.particleMesh.allBufferHandles)
            {
                var resoure = new CudaOpenGLBufferInteropResource(h, CUGraphicsRegisterFlags.None, CUGraphicsMapResourceFlags.None);
                resources.Add(resoure);
            }


            randomIndex_D = 0;
            randomIndex_D.CopyToDevice(0);
            
        }

        CudaDeviceVariable<int> randomIndex_D;

        public void Update(double deltaTime)
        {

            List<object> parameters = new List<object>();

            parameters.Add(randomIndex_D.DevicePointer);
            parameters.Add((float)deltaTime);
            parameters.Add(renderer.particleMesh.maxParticles);

            foreach (var r in resources)
            {
                r.Map();
                parameters.Add(r.GetMappedPointer());
            }

            object[] arrParams = parameters.ToArray();
            updateParticles.Run(arrParams);

            foreach (var r in resources)
            {
                r.UnMap();
            }
            
        }


        float3 Conv(Vector3 A)
        {
            return new float3(A.X, A.Y, A.Z);
        }
        float4 Conv(Vector4 A)
        {
            return new float4(A.X, A.Y, A.Z, A.W);
        }

        /*
        int length, 
		float3* currentPositionH, float3* currentVelocityH, float3* currentAccelerationH, float* currentLifeTimeH, float4* startColorH, float4* endColorH, float* startSizeH, float* endSizeH, float* startLifeTimeH,
		int* realCount, int* desiredCount, float3 aroundPosition, float4 startColor, float4 endColor, float startSize, float endSize, float startLifeTime
         */
        public void GenerateParticles(int desiredCount, Vector3 aroundPosition, Vector4 startColor, Vector4 endColor, float startSize, float endSize, float startLifeTime)
        {

            List<object> parameters = new List<object>();

            parameters.Add(randomIndex_D.DevicePointer);
            parameters.Add(renderer.particleMesh.maxParticles);

            foreach (var r in resources)
            {
                r.Map();
                parameters.Add(r.GetMappedPointer());
            }

            CudaDeviceVariable<int> realCount_D = 0;
            parameters.Add(realCount_D.DevicePointer);
            CudaDeviceVariable<int> desiredCount_D = desiredCount;
            parameters.Add(desiredCount_D.DevicePointer);
            parameters.Add(Conv(aroundPosition));
            parameters.Add(Conv(startColor));
            parameters.Add(Conv(endColor));
            parameters.Add(startSize);
            parameters.Add(endSize);
            parameters.Add(startLifeTime);

            object[] arrParams = parameters.ToArray();
            generateParticles.Run(arrParams);

            foreach (var r in resources)
            {
                r.UnMap();
            }

            int realCount=0;
            realCount_D.CopyToHost(ref realCount);
            //Debug.Info("desiredCount=" + desiredCount + " realCount=" + realCount);
        }

    }
}

