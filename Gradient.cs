using ImGuiNET;
using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NitricEngine2D
{
    public class GradientStep
    {
        public Color4 colour;
        public float position;

        public void SetPosition(float x)
        {
            position = MathHelper.Clamp(x, 0f, 1f);
        }
    }

    public class Gradient
    {
        public List<GradientStep> steps;

        GradientStep editor_selection;

        public enum INTERPOLATION_TYPE
        {
            LINEAR,
            CONSTANT
        }

        public INTERPOLATION_TYPE interpolation;

        public Gradient()
        {
            steps = new List<GradientStep>();
        }

        public Gradient(JsonElement data)
        {
            string interp = Helper.JSONGetPropertyString(data, "interpolation", "linear");
            interpolation = (interp == "linear")? INTERPOLATION_TYPE.LINEAR : INTERPOLATION_TYPE.CONSTANT;

            JsonElement stepArr;
            steps = new List<GradientStep>();
            if(data.TryGetProperty("steps", out stepArr))
            {
                foreach(JsonElement step in stepArr.EnumerateArray())
                {
                    steps.Add(new GradientStep() { colour = Helper.JSONGetPropertyColour(step, "col", Color4.Black), position = Helper.JSONGetPropertyFloat(step, "pos", 0f) });
                }
            }
        }

        public static Gradient GetDefault()
        {
            Gradient g = new Gradient();
            g.interpolation = INTERPOLATION_TYPE.LINEAR;
            g.steps.Add(new GradientStep() { colour = Color4.White, position = 0f });
            g.steps.Add(new GradientStep() { colour = Color4.White, position = 1f });
            return g;
        }

        public Color4 SampleAt(float x)
        {
            if (x < 0f || x > 1f) return Color4.Black;
            List<GradientStep> sortedSteps = steps.OrderBy(o => o.position).ToList();
            GradientStep l = sortedSteps[0], r = sortedSteps[sortedSteps.Count - 1];
            for(int i = 0; i < sortedSteps.Count; i++)
            {
                if (sortedSteps[i].position <= x)
                {
                    l = sortedSteps[i];
                    if(i + 1 < sortedSteps.Count)
                    {
                        r = sortedSteps[i + 1];
                    }
                    else
                    {
                        r = sortedSteps[i];
                    }
                }
            }

            

            if(interpolation == INTERPOLATION_TYPE.LINEAR)
            {
                if (l == null || r == null) return Color4.Black;
                return Helper.LerpColor(l.colour, r.colour, (x - l.position) / (r.position - l.position));
            }
            else
            {
                if (l == null) return Color4.Black;
                return l.colour;
            }
        }

        public void ExposeToInspector()
        {
            ImGui.Text("interpolation: ");
            ImGui.SameLine();
            if(ImGui.Button(interpolation == INTERPOLATION_TYPE.LINEAR ? "linear" : "constant"))
            {
                interpolation = (interpolation == INTERPOLATION_TYPE.LINEAR) ? INTERPOLATION_TYPE.CONSTANT : INTERPOLATION_TYPE.LINEAR;
            }

            ImGui.Text("Left click to select steps\nright click and drag to move");

            ImDrawListPtr drawList = ImGui.GetWindowDrawList();
            System.Numerics.Vector2 bar_pos = ImGui.GetCursorScreenPos();
            System.Numerics.Vector2 bar_size = new System.Numerics.Vector2(200, 50);

            List<GradientStep> sortedSteps = steps.OrderBy(o => o.position).ToList();

            for (int i = 0; i < sortedSteps.Count - 1; i++)
            {
                GradientStep step = sortedSteps[i];
                GradientStep next = sortedSteps[i + 1];
                System.Numerics.Vector2 start = new System.Numerics.Vector2(bar_pos.X + bar_size.X * step.position, bar_pos.Y);
                System.Numerics.Vector2 end = new System.Numerics.Vector2(bar_pos.X + bar_size.X * next.position, bar_pos.Y + bar_size.Y);
                uint col1 = Helper.Color4ToImguiColour(step.colour);
                uint col2 = Helper.Color4ToImguiColour(next.colour);
                if(interpolation == INTERPOLATION_TYPE.LINEAR)
                {
                    drawList.AddRectFilledMultiColor(start, end, col1, col2, col2, col1);
                }
                else
                {
                    drawList.AddRectFilledMultiColor(start, end, col1, col1, col1, col1);
                }
            }

            drawList.AddRect(bar_pos, bar_pos + bar_size, Helper.Color4ToImguiColour(Color4.LightGray));
            //draw markers
            for(int i = 0; i < sortedSteps.Count; i++)
            {
                GradientStep step = sortedSteps[i];
                System.Numerics.Vector2 circlePos = bar_pos + new System.Numerics.Vector2(bar_size.X * step.position, bar_size.Y);
                drawList.AddCircleFilled(circlePos, 10f, Helper.Color4ToImguiColour(step.colour));
                drawList.AddCircle(circlePos, 10f, Helper.Color4ToImguiColour((step == editor_selection)? Color4.White: Color4.Black));

            }

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                
                float x = (ImGui.GetMousePos().X - bar_pos.X) / bar_size.X;
                if (x >= 0f && x <= 1f && ImGui.GetMousePos().Y > bar_pos.Y && ImGui.GetMousePos().Y < bar_pos.Y + bar_size.Y + 10f)
                {
                    editor_selection = null;
                    foreach (GradientStep step in steps)
                    {
                        if (MathF.Abs(step.position - x) < 0.05f)
                        {
                            editor_selection = step;
                            break;
                        }
                    }

                    if (editor_selection == null)
                    {
                        GradientStep newStep = new GradientStep() { colour = SampleAt(x), position = x };
                        steps.Add(newStep);
                        editor_selection = newStep;
                    } 
                }
            }

            if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                float x = (ImGui.GetMousePos().X - bar_pos.X) / bar_size.X;
                if (x >= 0f && x <= 1f && ImGui.GetMousePos().Y > bar_pos.Y && ImGui.GetMousePos().Y < bar_pos.Y + bar_size.Y + 10f)
                {
                    if(editor_selection != null)
                    {
                        editor_selection.position = x;
                    }
                }
            }



            ImGui.SetCursorScreenPos(bar_pos + bar_size);
            ImGui.NewLine();

            


            if (editor_selection == null)
            {
                ImGui.Text("No step selected!");
            }
            else
            {
                editor_selection.colour = Helper.ImguiColourEdit4("step colour", editor_selection.colour);
                if(ImGui.Button("delete step") && steps.Count > 1)
                {
                    steps.Remove(editor_selection);
                    editor_selection = null;
                }
            }
            

            

        }
    }
}
