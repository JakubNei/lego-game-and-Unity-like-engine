# lego-game-and-Unity-like-engine
Simple lego game made along with simple Unity inspired game engine.

One semester we had a Computer Graphics, everyone did their projects in in C++, but I set out to create it in C# and make the scripting API exactly the same as Unity's scripting API.

If someone ever thinks of creating and switching to their own proprietary engine with same scripting API (for smooth transition), this project might be a good start.

Uses OpenTK as OpenGL wrapper for rendering and C# physics engine BEPUphysics.

It is a simple lego game, with: physics, deferred rendering, directional soft shadows, any number of lights (any type), parallax mapping and very very lame CUDA 'accelerated' particles.

Not everything works properly, still needs plenty of work. It is separated into MyEngine and MyGame projects, in theory MyGame will require minimal effort to port over to Unity (same API, resource management is different).

[Unity forum thread](https://forum.unity3d.com/threads/beginnings-of-clone-of-unity-engine-scripting-api.326255)

![](http://i.imgur.com/O2gSs9E.jpg)

![](http://i.imgur.com/kWN0aXg.png)
