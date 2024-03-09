using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Vector2 = System.Numerics.Vector2;
using System.Text.Json;

namespace NitricEngine2D
{ 

    public class BezierCurve
    {
        public BezierCurveCubic curve;
        public float magnitude = 1f;

        public enum EDITOR_SELECTIONS
        {
            START_ANCHOR,
            END_ANCHOR,
            CP1,
            CP2,
            NOTHING
        }

        public EDITOR_SELECTIONS selection = EDITOR_SELECTIONS.NOTHING;

        public static BezierCurve GetDefault()
        {
            BezierCurve c = new BezierCurve() { curve = new BezierCurveCubic() };
            c.curve.StartAnchor = new OpenTK.Mathematics.Vector2(0f, 0f);
            c.curve.EndAnchor = new OpenTK.Mathematics.Vector2(1f, 0f);
            c.curve.FirstControlPoint = new OpenTK.Mathematics.Vector2(0f, 1f);
            c.curve.SecondControlPoint = new OpenTK.Mathematics.Vector2(1f, 1f);
            return c;
        }

        public BezierCurve()
        {

        }

        public BezierCurve(JsonElement data)
        {
            this.magnitude = Helper.JSONGetPropertyFloat(data, "magnitude", 1f);
            this.curve = new BezierCurveCubic();
            this.curve.StartAnchor.X = 0f;
            this.curve.StartAnchor.Y = MathHelper.Clamp(Helper.JSONGetPropertyFloat(data, "start", 0f), 0f, 1f);
            this.curve.EndAnchor.X = 1f;
            this.curve.EndAnchor.Y = MathHelper.Clamp(Helper.JSONGetPropertyFloat(data, "end", 0f), 0f, 1f);
            this.curve.FirstControlPoint = OpenTK.Mathematics.Vector2.Clamp(Helper.JSONGetPropertyVec2(data, "cp1", new OpenTK.Mathematics.Vector2(0f, 1f)), OpenTK.Mathematics.Vector2.Zero, OpenTK.Mathematics.Vector2.One);
            this.curve.SecondControlPoint = OpenTK.Mathematics.Vector2.Clamp(Helper.JSONGetPropertyVec2(data, "cp2", new OpenTK.Mathematics.Vector2(1f, 1f)), OpenTK.Mathematics.Vector2.Zero, OpenTK.Mathematics.Vector2.One);
        }

        public float SampleAt(float x)
        {
            return curve.CalculatePoint(x).Y * magnitude;
        }

        public Vector2 pointInScreenSpace(OpenTK.Mathematics.Vector2 p, Vector2 box_pos, Vector2 box_size)
        {
            Vector2 ps = Helper.FromOpentkVec2(new OpenTK.Mathematics.Vector2(p.X, 1f - p.Y));
            return box_pos + (ps * box_size);
        }

        private void EditorLeftClick(Vector2 box_pos, Vector2 box_size)
        {
            Vector2 mouse = ImGui.GetMousePos();
            Vector2 mouse_in_range = (mouse - box_pos) / box_size;
            mouse_in_range.Y = 1.0f - mouse_in_range.Y;
            if (mouse_in_range.X < -0.1f || mouse_in_range.X > 1.1f || mouse_in_range.Y < -0.1f || mouse_in_range.Y > 1.1f) return;

            float distStart = (Helper.FromOpentkVec2(curve.StartAnchor) - mouse_in_range).Length();
            float distEnd = (Helper.FromOpentkVec2(curve.EndAnchor) - mouse_in_range).Length();
            float distCP1 = (Helper.FromOpentkVec2(curve.FirstControlPoint) - mouse_in_range).Length();
            float distCP2 = (Helper.FromOpentkVec2(curve.SecondControlPoint) - mouse_in_range).Length();

            if (distStart < 0.05f) selection = EDITOR_SELECTIONS.START_ANCHOR;
            else if (distEnd < 0.05f) selection = EDITOR_SELECTIONS.END_ANCHOR;
            else if (distCP1 < 0.05f) selection = EDITOR_SELECTIONS.CP1;
            else if (distCP2 < 0.05f) selection = EDITOR_SELECTIONS.CP2;
            else selection = EDITOR_SELECTIONS.NOTHING;


        }

        private void EditorRightClick(Vector2 box_pos, Vector2 box_size)
        {
            Vector2 mouse = ImGui.GetMousePos();
            Vector2 mouse_in_range = (mouse - box_pos) / box_size;
            mouse_in_range.Y = 1.0f - mouse_in_range.Y;
            if (mouse_in_range.X < -0.1f || mouse_in_range.X > 1.1f || mouse_in_range.Y < -0.1f || mouse_in_range.Y > 1.1f) return;

            mouse_in_range.X = MathHelper.Clamp(mouse_in_range.X, 0f, 1f);
            mouse_in_range.Y = MathHelper.Clamp(mouse_in_range.Y, 0f, 1f);

            if (selection == EDITOR_SELECTIONS.START_ANCHOR) curve.StartAnchor = new OpenTK.Mathematics.Vector2(0f, mouse_in_range.Y);
            else if (selection == EDITOR_SELECTIONS.END_ANCHOR) curve.EndAnchor = new OpenTK.Mathematics.Vector2(1f, mouse_in_range.Y);
            else if (selection == EDITOR_SELECTIONS.CP1) curve.FirstControlPoint = Helper.FromNumericsVec2(mouse_in_range);
            else if (selection == EDITOR_SELECTIONS.CP2) curve.SecondControlPoint = Helper.FromNumericsVec2(mouse_in_range);

            
        }

        public void ExposeToInspector()
        {
            ImGui.Text("left click to select points\nholdright click to move points");
            ImDrawListPtr drawList = ImGui.GetWindowDrawList();
            Vector2 box_pos = ImGui.GetCursorScreenPos();
            Vector2 box_size = new Vector2(150, 150);

            

            drawList.AddBezierCubic(pointInScreenSpace(curve.StartAnchor, box_pos, box_size), pointInScreenSpace(curve.FirstControlPoint, box_pos, box_size), pointInScreenSpace(curve.SecondControlPoint, box_pos, box_size), pointInScreenSpace(curve.EndAnchor, box_pos, box_size), Helper.Color4ToImguiColour(Color4.Red), 3f);
            drawList.AddRect(box_pos, box_pos + box_size, Helper.Color4ToImguiColour(Color4.LightGray));

            //draw markers
            

            //line from start to control point 1
            drawList.AddLine(pointInScreenSpace(curve.StartAnchor, box_pos, box_size), pointInScreenSpace(curve.FirstControlPoint, box_pos, box_size), Helper.Color4ToImguiColour((selection == EDITOR_SELECTIONS.CP1) ? Color4.Green : Color4.Blue), 2f);

            //line from end to control point 2
            drawList.AddLine(pointInScreenSpace(curve.EndAnchor, box_pos, box_size), pointInScreenSpace(curve.SecondControlPoint, box_pos, box_size), Helper.Color4ToImguiColour((selection == EDITOR_SELECTIONS.CP2) ? Color4.Green : Color4.Blue), 2f);

            //start anchor
            drawList.AddCircleFilled(pointInScreenSpace(curve.StartAnchor, box_pos, box_size), 10f, Helper.Color4ToImguiColour((selection == EDITOR_SELECTIONS.START_ANCHOR)? Color4.Green : Color4.White));

            

            //end anchor
            drawList.AddCircleFilled(pointInScreenSpace(curve.EndAnchor, box_pos, box_size), 10f, Helper.Color4ToImguiColour((selection == EDITOR_SELECTIONS.END_ANCHOR) ? Color4.Green : Color4.White));

            //control point 1
            drawList.AddCircleFilled(pointInScreenSpace(curve.FirstControlPoint, box_pos, box_size), 5f, Helper.Color4ToImguiColour((selection == EDITOR_SELECTIONS.CP1) ? Color4.Green : Color4.Blue));

            //control point 2
            drawList.AddCircleFilled(pointInScreenSpace(curve.SecondControlPoint, box_pos, box_size), 5f, Helper.Color4ToImguiColour((selection == EDITOR_SELECTIONS.CP2) ? Color4.Green : Color4.Blue));

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                EditorLeftClick(box_pos, box_size);
            }

            if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                EditorRightClick(box_pos, box_size);
            }

            ImGui.SetCursorScreenPos(box_pos + box_size);
            ImGui.NewLine();
            ImGui.DragFloat("magnitude", ref magnitude, 0.1f, 0f, 100f);
        }
    }
}
