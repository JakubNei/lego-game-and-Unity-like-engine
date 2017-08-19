using System;
using System.Collections.Generic;
using System.Text;



using OpenTK;

using MyEngine;

namespace MyGame
{
    class Program
    {
        [STAThread]
        public static void Main()
        {
            //string[] args = System.Environment.GetCommandLineArgs();
            using (var engine = new EngineMain())
            {

                {

                    var go = new GameObject();
                    go.AddComponent<FirstPersonCamera>();
                    go.AddComponent<CreatePhysicsObject>();

                    engine.camera = go.AddComponent<Camera>();
                    //var c=go.AddComponent<BoxCollider>(); c.size = Vector3.One*5;
                    string skyboxName = "skybox/sunny/";
                    engine.skyboxCubeMap = Factory.GetCubeMap(new Resource[] {
                        skyboxName +"left.jpg",
                        skyboxName + "right.jpg",
                        skyboxName + "top.jpg",
                        skyboxName + "bottom.jpg",
                        skyboxName + "front.jpg",
                        skyboxName + "back.jpg"
                    });

                    go.transform.position = (new Vector3(1, 1, 1)) * 100;
                    go.transform.LookAt(new Vector3(0, 0, 100));


                    var drag = go.AddComponent<DragLegoPieces>();
                    var t = new GameObject();
                    var renderer = t.AddComponent<MeshRenderer>();
                    renderer.mesh = Factory.GetMesh("sphere.obj");
                    drag.visualiseHitTarget = t;

                    //engine.camera.gameObject.AddComponent<SSAO>();
                }

                {
                    var go = new GameObject();
                    //go.AddComponent<Rigidbody>();
                    var renderer = go.AddComponent<MeshRenderer>();
                    renderer.mesh = Factory.GetMesh("internal/cube_bigUVs.obj");

                    renderer.material.albedoTexture = Factory.GetTexture2D("textures/grassDIFFUSE.jpg");
                    renderer.material.normalMap = Factory.GetTexture2D("textures/grassNORMAL.jpg");
                    renderer.material.depthMap = Factory.GetTexture2D("textures/grassDISP.jpg");

                    //renderer.material.albedoTexture = Factory.GetTexture2D("textures/cobblestonesDiffuse.bmp");
                    //renderer.material.normalMap = Factory.GetTexture2D("textures/cobblestonesNormal.bmp");
                    //renderer.material.depthMap = Factory.GetTexture2D("textures/cobblestonesDepth.bmp");

                    //renderer.material.albedoTexture = Factory.GetTexture2D("textures/stonewallDiffuse.bmp");
                    //renderer.material.normalMap = Factory.GetTexture2D("textures/stonewallNormal.bmp");
                    //renderer.material.depthMap = Factory.GetTexture2D("textures/stonewallDepth.bmp");


                    go.transform.Translate(new Vector3(0, -10, 0));
                    go.transform.scale = new Vector3(1000, 1, 1000);
                    go.AddComponent<BoxCollider>();

                }
                
                {
                    var go = new GameObject();
                    var renderer = go.AddComponent<MeshRenderer>();
                    renderer.mesh = Factory.GetMesh("lego/block_20x20x05.obj");
                    renderer.material.albedo = LegoPiece.colors[5];
                    go.transform.Translate(new Vector3(30, 1, 20));
                    go.AddComponent<Rigidbody>();
                    var c=go.AddComponent<BoxCollider>();
                    go.AddComponent<LegoPiece>();
                }


                /*{
                    var go = new GameObject();
                    //go.AddComponent<Rigidbody>();
                    var renderer = go.AddComponent<MeshRenderer>();
                    renderer.mesh = Factory.GetMesh("internal/cube.obj");
                    go.transform.Translate(new Vector3(0, 0, 2));
                    go.AddComponent<BoxCollider>();
                    renderer.material.albedoTexture = Factory.GetTexture2D("textures/cobblestonesDiffuse.bmp");
                    renderer.material.albedo = Vector4.One;
                    renderer.material.normalMap = Factory.GetTexture2D("textures/cobblestonesNormal.bmp");
                    renderer.material.depthMap = Factory.GetTexture2D("textures/cobblestonesDepth.bmp");
                }*/

                {
                    var go = new GameObject();
                    var renderer = go.AddComponent<MeshRenderer>();
                    renderer.mesh = Factory.GetMesh("dragon.obj");
                    go.transform.position += new Vector3(-30,0,0);
                    go.GetComponent<MeshRenderer>().material.smoothness = 0.8f;
                    go.AddComponent<Rigidbody>();
                    go.AddComponent<BoxCollider>();
                }


                {
                    var go = new GameObject();
                    go.transform.position = new Vector3(-10, 10, -10);
                    go.transform.LookAt(Vector3.Zero);
                    var light = go.AddComponent<Light>();
                    light.type = LightType.Directional;
                    light.color = Vector3.One * 0.7f;
                    light.shadows = LightShadows.Soft;

                    var renderer = go.AddComponent<MeshRenderer>();
                    renderer.mesh = Factory.GetMesh("internal/cube.obj");
                    go.AddComponent<BoxCollider>();
                    go.AddComponent<MoveWithArrowKeys>();
                    go.AddComponent<VisualizeDir>();
                    //go.AddComponent<LightMovement>();
                    
                }

                {
                    var go = new GameObject();
                    go.transform.position = new Vector3(10, 10, 10);
                    go.transform.LookAt(Vector3.Zero);
                    var light = go.AddComponent<Light>();
                    light.type = LightType.Directional;
                    light.color = Vector3.One * 0.7f;
                    light.shadows = LightShadows.Soft;

                    var renderer = go.AddComponent<MeshRenderer>();
                    renderer.mesh = Factory.GetMesh("internal/cube.obj");
                    go.AddComponent<VisualizeDir>();
                    //go.AddComponent<LightMovement>();
                }

                {
                    var go = new GameObject();
                    go.transform.position = new Vector3(0, 10, 0);
                    go.transform.LookAt(Vector3.Zero);
                    var light = go.AddComponent<Light>();
                    light.type = LightType.Point;
                    light.color = Vector3.One * 0.3f;
                    light.shadows = LightShadows.Soft;

                    var renderer = go.AddComponent<MeshRenderer>();
                    renderer.mesh = Factory.GetMesh("internal/cube.obj");
                    go.AddComponent<VisualizeDir>();

                }
               
               
                engine.Run(30);
            }
        }
    }   
}
