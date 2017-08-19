// https://managedcuda.codeplex.com/documentation




#include <texture_fetch_functions.h>
#include "float.h"
#include <builtin_types.h>

#include <cuda.h>
#include <cuda_runtime.h>
#include <device_launch_parameters.h>
#include <vector_types.h>
#include <vector_functions.h>
#include <math.h>

#include "cutil_math.h"


#define F3ADDASIGNI(A,B,I) F3ADDASIGN(A[I],B[I])
#define F3ADDASIGN(A,B) A.x+=B.x; A.y+=B.y; A.z+=B.z; 

#define F3ADD(A,B) make_float3(A.x+B.x, A.y+B.y, A.z+B.z)

#define F3MULTF(A,B) make_float3(A.x*B, A.y*B, A.z*B)


extern "C"
{


	__constant__ __device__ float randomFloats[1024];


	__global__ void updateParticles(
		int* randomIndex, float deltaTime, int length,
		float3* currentPositionH, float3* currentVelocityH, float3* currentAccelerationH, float* currentLifeTimeH, float4* startColorH, float4* endColorH, float* startSizeH, float* endSizeH, float* startLifeTimeH
	)
	{
		unsigned int tid = threadIdx.x + blockDim.x * blockIdx.x;
		unsigned int tidMax = blockDim.x * gridDim.x;

		while (tid < length) 
		{	
			if (currentLifeTimeH[tid]>=0) {
				currentLifeTimeH[tid] -= deltaTime;
				F3ADDASIGNI(currentVelocityH, currentAccelerationH, tid)
				F3ADDASIGNI(currentPositionH, currentVelocityH, tid)
			}
			tid += tidMax;
		}
	}



	inline __host__ __device__ float getRandomFloat(int* randomIndex) {
		int old = atomicAdd(randomIndex, 1);
		if (old > 1000) {
			*randomIndex = 0;
			old = 0;
		}
		return randomFloats[old];
	}

	__global__ void generateParticles(
		int* randomIndex, int length,
		float3* currentPositionH, float3* currentVelocityH, float3* currentAccelerationH, float* currentLifeTimeH, float4* startColorH, float4* endColorH, float* startSizeH, float* endSizeH, float* startLifeTimeH,
		int* realCount, int* desiredCount, float3 aroundPosition, float4 startColor, float4 endColor, float startSize, float endSize, float startLifeTime
	)
	{
		unsigned int tid = threadIdx.x + blockDim.x * blockIdx.x;
		unsigned int tidMax = blockDim.x * gridDim.x;

		while (tid < length)
		{
			if (currentLifeTimeH[tid] <= 0.0f) 
			{
				int old = *desiredCount;
				if (old>0) {
					old = atomicSub(desiredCount, 1);
					if (old > 0) {
						//atomicAdd(realCount, 1);

#define RF getRandomFloat(randomIndex)

						//float3 direction = make_float3(0,0,0);
						float3 direction = normalize(make_float3(RF - 0.5, RF - 0.5, RF - 0.5));
						float r = RF;
						direction = F3MULTF(direction, r);

						//currentPositionH[tid] = F3ADD(aroundPosition,direction);
						currentPositionH[tid] = aroundPosition;
						currentVelocityH[tid] = direction;
						//currentVelocityH[tid] = make_float3(0, 0, 0);
						//r = -0.5*RF;
						//currentAccelerationH[tid] = F3MULTF(direction, r);
						currentAccelerationH[tid] = make_float3(0, 0, 0);
						currentLifeTimeH[tid] = startLifeTime;
						startColorH[tid] = startColor;
						endColorH[tid] = endColor;
						startSizeH[tid] = startSize;
						endSizeH[tid] = endSize;
						startLifeTimeH[tid] = startLifeTime;
					}
				}
			}
			tid += tidMax;
		}
	}
}


int main()
{
	return 0;
}

