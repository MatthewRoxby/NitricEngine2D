using NitricEngine2D.nodes;
using System.Text.Json;

namespace NitricEngine2D.loaders
{
    /// <summary>
    /// a static loader to load JSON scene files as a hierarchy of nodes
    /// </summary>
    public static class SceneLoader
    {
        /// <summary>
        /// Loads a JSON file as a hierarchy of nodes
        /// </summary>
        /// <param name="scenePath">path to the scene file</param>
        /// <returns>the root node of the scene</returns>
        public static Node LoadScene(string scenePath)
        {
            JsonDocument doc = JsonDocument.Parse(File.OpenRead(scenePath), Helper.DefaultJSONOptions);

            //call the recursive function on the root node
            Node result = LoadNode(doc.RootElement.GetProperty("root"));
            

            return result;
        }

        /// <summary>
        /// Recursive function to load a single node
        /// </summary>
        /// <param name="data">the current JSON element</param>
        /// <returns>the loaded node (if the load is valid)</returns>
        private static Node? LoadNode(JsonElement data)
        {
            //get the node type
            string type = data.GetProperty("type").GetString();

            Type t = null;

            //search possible node namespaces for the given type
            foreach(string name in GameManager.NODE_NAMESPACES)
            {
                Type nodeType = Type.GetType(name + "." + type);
                if( nodeType != null)
                {
                    t = nodeType;
                    break;
                }
            }

            if (t == null) return null;

            //create an instance of the type if found
            Node result = (Node)Activator.CreateInstance(t, new object[] { data });

            //repeat the function on all children
            JsonElement children;
            if (data.TryGetProperty("children", out children))
            {
                foreach (JsonElement child in children.EnumerateArray())
                {
                    result.AddChild(LoadNode(child));
                }
            }
            

            return result;

        }
    }
}
