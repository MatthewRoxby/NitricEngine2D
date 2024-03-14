using ImGuiNET;
using NitricEngine2D.nodes;

namespace NitricEngine2D.debug_windows
{
    /// <summary>
    /// debug window that allows the editing of certain aspects of a selected node
    /// </summary>
    public class Inspector : DebugWindow
    {
        public Inspector()
        {

        }

        private Node? selected = null;

        /// <summary>
        /// selects the given node
        /// </summary>
        /// <param name="node">node to be selected</param>
        public void SelectNode(Node node)
        {
            selected = node;
        }

        public override void Update(float deltaTime)
        {
            
            if (ImGui.Begin("Inspector"))
            {
                //call the expose function on the selected node if it exists
                selected?.ExposeToInspector();
            }
            
        }
    }
}
