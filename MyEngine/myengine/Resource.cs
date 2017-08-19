using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace MyEngine
{
    public class Resource
    {
        public string originalPath;
        public string realPath;

        private Resource()
        {

        }
               
        public static implicit operator string(Resource r)
        {
            return r.realPath;
        }
        public static implicit operator Resource(string originalPath)
        {
            return MakeResource(originalPath);
        }

        public static Resource WithAllPathsAs(string path)
        {
            var r=new Resource();
            r.originalPath = UseCorrectDirectorySeparator(path);
            r.realPath = UseCorrectDirectorySeparator(path);
            return r;
        }

        public static string UseCorrectDirectorySeparator(string path)
        {
            path = path.Replace('/', System.IO.Path.DirectorySeparatorChar);
            path = path.Replace('\\', System.IO.Path.DirectorySeparatorChar);
            return path;
        }

        public static bool ResourceInFolderExists(Resource folder, string childName)
        {
            var lastSlash = folder.originalPath.LastIndexOf("/");
            if (lastSlash == -1) lastSlash = 0;
            var originalPath = folder.originalPath.Substring(0, lastSlash) + childName;

            return File.Exists(MakeRealPath(originalPath));
        }



        public static Resource GetResourceInFolder(Resource folder, string childName)
        {            
            var lastSlash = folder.originalPath.LastIndexOf("/");
            if (lastSlash == -1) lastSlash = 0;
            var originalPath = folder.originalPath.Substring(0, lastSlash) + childName;

            return MakeResource(originalPath);
        }

        private static string MakeRealPath(string originalPath)
        {
            return UseCorrectDirectorySeparator("../../../Resources/" + originalPath);
        }
        
        private static Resource MakeResource(string originalPath)
        {
            var realPath = MakeRealPath(originalPath);
            if (File.Exists(realPath))
            {
                return new Resource() { originalPath = originalPath, realPath = realPath };
            }
            else
            {
                Debug.Error("File "+originalPath + " doesnt exits");
                Debug.Pause();
                return null;
            }
        }

    }
}
