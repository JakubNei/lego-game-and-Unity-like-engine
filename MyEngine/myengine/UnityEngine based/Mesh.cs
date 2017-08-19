﻿using System;
using System.Collections.Generic;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MyEngine
{
    public class Mesh : IUnloadable
    {

        public Vector3[] vertices { set { isOnGPU = false; m_vertices = value; } get { return m_vertices; } }
        public Vector3[] normals { set { isOnGPU = false; m_normals = value; } get { return m_normals; } }
        public Vector3[] tangents { set { isOnGPU = false; m_tangents = value; } get { return m_tangents; } }
        public Vector2[] uvs { set { isOnGPU = false; m_uvs = value; } get { return m_uvs; } }
        public int[] triangleIndicies { set { isOnGPU = false; m_triangleIndicies = value; } get { return m_triangleIndicies; } }


        internal Action<ChangedFlags> OnChanged;
        internal void RaiseOnChanged(ChangedFlags flags)
        {
            if (OnChanged != null) OnChanged(flags);
        }

        Vector3[] m_vertices;
        Vector3[] m_normals;
        Vector3[] m_tangents;
        Vector2[] m_uvs;
        int[] m_triangleIndicies;

        bool recalculateBounds = true;
        public Bounds bounds
        {
            get
            {
                if (recalculateBounds)
                {
                    recalculateBounds = false;
                    _bounds = new Bounds(vertices[0], Vector3.Zero);
                    foreach (var point in vertices)
                    {
                        _bounds.Encapsulate(point);
                    }
                }

                return _bounds;
            }
            set
            {
                RaiseOnChanged(ChangedFlags.Bounds);
                _bounds = value;
            }

        }
        Bounds _bounds;
    

        bool isOnGPU = false;
        internal Resource resource;

        internal void Draw()
        {
            if(!isOnGPU) UploadMeshData();
            GL.BindVertexArray(vertexArrayObjectHandle);
            GL.DrawElements(PrimitiveType.Triangles, triangleIndicies.Length,
                DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.BindVertexArray(0);
        }

        public void UploadMeshData()
        {
            Unload();
            CreateVBOs();
            CreateVAO();
            isOnGPU = true;
            RaiseOnChanged(ChangedFlags.VisualRepresentation);
        }

        internal uint positionVboHandle;
        uint normalVboHandle;
        uint tangentsVboHandle;
        uint uvVboHandle;
        uint elementArrayBuffeHandle;

        List<uint> allBufferHandles = new List<uint>();
        int vertexArrayObjectHandle = -1;


        public bool HasTangents()
        {
            return tangents != null;
        }

        
        
        public void RecalculateNormals()
        {
            if (normals == null) normals = new Vector3[vertices.Length];

            int verticesNum = vertices.Length;
            int indiciesNum = triangleIndicies.Length;


            int[] counts = new int[verticesNum];

            for (int i = 0; i < verticesNum; i++)
            {
                counts[i] = 0;
            }

            for (int i = 0; i < indiciesNum - 3; i += 3)
            {

                int ai = triangleIndicies[i];
                int bi = triangleIndicies[i + 1];
                int ci = triangleIndicies[i + 2];

                if (ai < verticesNum && bi < verticesNum && ci < verticesNum)
                {
                    Vector3 av = vertices[ai];
                    Vector3 n = Vector3.Normalize(Vector3 .Cross(
                        vertices[bi] - av,
                        vertices[ci] - av
                    ));

                    normals[ai] += n;
                    normals[bi] += n;
                    normals[ci] += n;

                    counts[ai]++;
                    counts[bi]++;
                    counts[ci]++;
                }
            }

            for (int i = 0; i < verticesNum; i++)
            {
                normals[i] /= counts[i];
            }



        }
        
        public void RecalculateTangents()
        {
            if (tangents == null) tangents = new Vector3[vertices.Length];

            //partialy stolen from http://www.opengl-tutorial.org/intermediate-tutorials/tutorial-13-normal-mapping/

            int verticesNum = vertices.Length;
            int indiciesNum = triangleIndicies.Length;

            int[] counts = new int[verticesNum];

            for (int i = 0; i < verticesNum; i++)
            {
                counts[i] = 0;
            }

            for (int i = 0; i < indiciesNum - 3; i += 3)
            {

                int ai = triangleIndicies[i];
                int bi = triangleIndicies[i + 1];
                int ci = triangleIndicies[i + 2];

                if (ai < verticesNum && bi < verticesNum && ci < verticesNum)
                {
                    Vector3 av = vertices[ai];
                    Vector3 deltaPos1 = vertices[bi] - av;
                    Vector3 deltaPos2 = vertices[ci] - av;

                    Vector2 auv = uvs[ai];
                    Vector2 deltaUV1 = uvs[bi] - auv;
                    Vector2 deltaUV2 = uvs[ci] - auv;

                    float r = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X);
                    Vector3 t = (deltaPos1 * deltaUV2.Y - deltaPos2 * deltaUV1.Y) * r;


                    tangents[ai] += t;
                    tangents[bi] += t;
                    tangents[ci] += t;

                    counts[ai]++;
                    counts[bi]++;
                    counts[ci]++;
                }
            }

            for (int i = 0; i < verticesNum; i++)
            {
                tangents[i] /= counts[i];
            }


        }

   
        void CreateVBOs()
        {
            

            if (!HasTangents()) RecalculateTangents();

            positionVboHandle = CreateVBOPart(vertices, vertices.Length * 3 * sizeof(float), BufferTarget.ArrayBuffer);
            normalVboHandle = CreateVBOPart(normals, normals.Length * 3 * sizeof(float), BufferTarget.ArrayBuffer);
            tangentsVboHandle = CreateVBOPart(tangents, tangents.Length * 3 * sizeof(float), BufferTarget.ArrayBuffer);
            uvVboHandle = CreateVBOPart(uvs, uvs.Length * 2 * sizeof(float), BufferTarget.ArrayBuffer);

            int size;
            int sizeFromGpu;

            size = triangleIndicies.Length * sizeof(Int32);
            elementArrayBuffeHandle = GLGenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementArrayBuffeHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(size), triangleIndicies, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out sizeFromGpu);
            if (size != sizeFromGpu) Debug.Error("size mismatch size=" + size + " sizeFromGpu=" + sizeFromGpu);            

            //GL.GetActiveAtomicCounterBuffer(0, 0, AtomicCounterBufferParameter.)
        }

        uint CreateVBOPart<T>(T[] data, int size, BufferTarget bt) where T : struct
        {
            int sizeFromGpu;
            var hande = GLGenBuffer();
            GL.BindBuffer(bt, hande);
            GL.BufferData(bt, (IntPtr)(size), data, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(bt, BufferParameterName.BufferSize, out sizeFromGpu);
            if (size != sizeFromGpu) Debug.Error("size mismatch size=" + size + " sizeFromGpu=" + sizeFromGpu);
            return hande;
        }

        
        void CreateVAO()
        {
            vertexArrayObjectHandle = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObjectHandle);

            GL.EnableVertexAttribArray(Shader.positionLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, positionVboHandle);
            GL.VertexAttribPointer(Shader.positionLocation, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.EnableVertexAttribArray(Shader.normalLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, normalVboHandle);
            GL.VertexAttribPointer(Shader.normalLocation, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.EnableVertexAttribArray(Shader.tangentLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, tangentsVboHandle);
            GL.VertexAttribPointer(Shader.tangentLocation, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.EnableVertexAttribArray(Shader.uvLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, uvVboHandle);
            GL.VertexAttribPointer(Shader.uvLocation, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementArrayBuffeHandle);

            GL.BindVertexArray(0);
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

