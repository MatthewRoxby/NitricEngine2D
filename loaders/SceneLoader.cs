using NitricEngine2D.nodes;
using System.Text.Json;

namespace NitricEngine2D.loaders
{
    public static class SceneLoader
    {
        public static Node LoadScene(string scenePath)
        {
            JsonDocument doc = JsonDocument.Parse(File.OpenRead(scenePath), Helper.DefaultJSONOptions);

            Node result = LoadNode(doc.RootElement.GetProperty("root"));
            

            return result;
        }

        private static Node? LoadNode(JsonElement data)
        {
            
            string type = data.GetProperty("type").GetString();

            Type t = null;

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

            Node result = (Node)Activator.CreateInstance(t, new object[] { data });

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
