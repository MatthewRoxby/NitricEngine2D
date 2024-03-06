using ImGuiNET;
using NitricEngine2D.nodes;

namespace NitricEngine2D.debug_windows
{
    public class HierarchyViewer : DebugWindow
    {
        public HierarchyViewer()
        {

        }

        public override void Update(float deltaTime)
        {
            if (ImGui.Begin("Hierarchy"))
            {
                if(GameManager.currentRoot != null) ExposeNode(GameManager.currentRoot);
            }
        }

        private void ExposeNode(Node node)
        {
            if(node.GetChildren().Length > 0)
            {
                bool b = ImGui.TreeNode(node.name + "##" + node.id.ToString());
                ImGui.SameLine();
                if (ImGui.Button("select##" + node.id.ToString()))
                {
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
