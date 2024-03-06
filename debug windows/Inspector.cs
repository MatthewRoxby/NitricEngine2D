using ImGuiNET;
using NitricEngine2D.nodes;

namespace NitricEngine2D.debug_windows
{
    public class Inspector : DebugWindow
    {
        public Inspector()
        {

        }

        private Node? selected = null;

        public void SelectNode(Node node)
        {
            selected = node;
        }

        public override void Update(float deltaTime)
        {
            if (ImGui.Begin("Inspector"))
            {
                selected?.ExposeToInspector();
            }
            
        }
    }
}
