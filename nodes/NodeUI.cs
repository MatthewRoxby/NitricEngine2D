using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Desktop;
using System.Text.Json;

namespace NitricEngine2D.nodes
{
    public class Sticky
    {
        public enum Anchor
        {
            NW, N, NE, W, C, E, SW, S, SE
        }

        public List<Anchor> anchors = new List<Anchor>();

        Anchor[] dirs = new Anchor[]
        {
            Anchor.NW, Anchor.N, Anchor.NE,
            Anchor.W, Anchor.C, Anchor.E,
            Anchor.SW, Anchor.S, Anchor.SE
        };

        public bool HasAnchor(Anchor anchor)
        {
            foreach(Anchor a in anchors)
            {
                if (a == anchor) return true;
            }

            return false;
        }

        public void ExposeToInspector()
        {
            if (ImGui.TreeNode("Sticky"))
            {
                ImGui.Text("left click to set / unset anchors (max 2)");
                ImDrawListPtr drawList = ImGui.GetWindowDrawList();
                
                System.Numerics.Vector2 boxSize = new System.Numerics.Vector2(90, 90);
                System.Numerics.Vector2 borderSize = new System.Numerics.Vector2(5, 5);
                System.Numerics.Vector2 boxPos = ImGui.GetCursorScreenPos() + borderSize;
                System.Numerics.Vector2 littleSize = boxSize / 3;


                drawList.AddRectFilled(boxPos - borderSize, boxPos + boxSize + borderSize, Helper.Color4ToImguiColour(Color4.DarkSlateGray));

                //mouse hover
                System.Numerics.Vector2 mouse = (ImGui.GetMousePos() - boxPos) / boxSize;

                for (int y = 0; y < 3; y++)
                {
                    for(int x = 0; x < 3; x++)
                    {
                        System.Numerics.Vector2 p = System.Numerics.Vector2.Zero;
                        p.X += littleSize.X * x;
                        p.Y += littleSize.Y * y;

                        Color4 c;
                        
                        
                        if(mouse.X > p.X / boxSize.X && mouse.Y > p.Y / boxSize.Y && mouse.X < (p.X + littleSize.X) / boxSize.X && mouse.Y < (p.Y + littleSize.Y) / boxSize.Y)
                        {
                            c = Color4.Green;

                            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                            {
                                Anchor a = dirs[y * 3 + x];
                                if (HasAnchor(a))
                                {
                                    anchors.Remove(a);
                                }
                                else
                                {
                                    if(anchors.Count < 2)
                                    {
                                        anchors.Add(a);
                                    }
                                }
                            }
                        }
                        else
                        {
                            c = Color4.DarkGreen;
                        }

                        if (HasAnchor(dirs[y * 3 + x]))
                        {
                            c = Color4.LimeGreen;
                        }

                        drawList.AddRectFilled(p + boxPos, p + boxPos + littleSize, Helper.Color4ToImguiColour(c));

                        drawList.AddText(p + boxPos + (littleSize * 0.3f), Helper.Color4ToImguiColour(Color4.Black), dirs[y * 3 + x].ToString());
                    }
                }

                ImGui.SetCursorScreenPos(boxPos + boxSize);
                ImGui.NewLine();
                string s = "";
                foreach (Anchor a in anchors) s += a.ToString() + "\n";
                ImGui.Text("selected anchors:\n" + s);
                

                ImGui.TreePop();
            }
        }
    }


    public class NodeUI : Node
    {
        public Shader? shader = null;

        public Vector2 center;
        public Vector2 size;

        public Sticky sticky;

        public NodeUI(JsonElement data) : base(data)
        {
            this.sticky = new Sticky();
        }

        public override void ExposeToInspector()
        {
            base.ExposeToInspector();
            sticky.ExposeToInspector();
        }
    }
}
