using ImGuiNET;
using NitricEngine2D.nodes;

namespace NitricEngine2D.debug_windows
{
    /// <summary>
    /// Debug window that displays the node hierarchy
    /// </summary>
    public class HierarchyViewer : DebugWindow
    {
        public HierarchyViewer()
        {

        }

        public override void Update(float deltaTime)
        {
            if (ImGui.Begin("Hierarchy"))
            {
                //call the recursive function, starting with the scene root
                if(GameManager.currentRoot != null) ExposeNode(GameManager.currentRoot);
            }
        }

        /// <summary>
        /// recursive function that goes through every node and exposes the hierarchy
        /// </summary>
        /// <param name="node">starting node</param>
        private void ExposeNode(Node node)
        {
            //if node has no children, don't bother with a tree node
            if(node.GetChildren().Length > 0)
            {
                //id is used as an invisible marker that ensures that two nodes with the same node don't conflict
                bool b = ImGui.TreeNode(node.name + "##" + node.id.ToString());
                ImGui.SameLine();
                if (ImGui.Button("select##" + node.id.ToString()))
                {
                    //if the inspector exists, select the current node
                    Inspector ins = DebugWindowManager.GetWindow<Inspector>();
                    if (ins != null) ins.SelectNode(node);
                }
                if (b)
                {
                    foreach (Node child in node.GetChildren())
                    {
                        ExposeNode(child);
                    }
                    ImGui.TreePop();
                }
            }
            else
            {
                //adds a tab for better formatting
                ImGui.Text("   " + node.name);
                ImGui.SameLine();
                if (ImGui.Button("select##" + node.id.ToString()))
                {
                    Inspector ins = DebugWindowManager.GetWindow<Inspector>();
                    if (ins != null) ins.SelectNode(node);
                }
            }
            
        }
    }
}
