using ImGuiNET;
using OpenTK.Mathematics;
using System.Text.Json;

namespace NitricEngine2D.nodes
{
    public class Node2D : Node
    {
        public Vector2 position = Vector2.Zero;
        public Vector2 scale = Vector2.One;
        public float rotation_degrees = 0f;
        public int z_index = 0;

        public Matrix4 local_transformation { get; private set; }
        public Matrix4 global_transformation { get; private set; }

        public Vector2 global_position { get; private set; }

        public Vector2 global_scale { get; private set; }

        public float global_rotation {  get; private set; }

        public Node2D(string name = "Node2D") : base(name)
        {

        }

        public Node2D(JsonElement data) : base(data)
        {
            this.position = Helper.JSONGetPropertyVec2(data, "position", Vector2.Zero);
            this.scale = Helper.JSONGetPropertyVec2(data, "scale", Vector2.One);
            this.rotation_degrees = Helper.JSONGetPropertyFloat(data, "rotation", 0f);

            this.z_index = Helper.JSONGetPropertyInt(data, "z_index", 0);
        }

        public override void Update(float deltaTime)
        {
            local_transformation = Helper.createTransformation(position, scale, rotation_degrees, z_index);

            //Debug.WriteLine($"Node {name} has parent: {parent?.name}");
            if (parent is Node2D)
            {
                global_transformation = local_transformation * ((Node2D)parent).global_transformation;
                //global_transformation = local_transformation;
            }
            else
            {
                global_transformation = local_transformation;
            }

            Vector3 t = global_transformation.ExtractTranslation();
            Vector3 s = global_transformation.ExtractScale();
            Vector3 r;
            global_transformation.ExtractRotation().ToEulerAngles(out r);

            global_position = new Vector2(t.X, t.Y);
            global_scale = new Vector2(s.X, s.Y);
            global_rotation = MathHelper.RadiansToDegrees(r.Z);

            base.Update(deltaTime);
        }

        public override void ExposeToInspector()
        {
            base.ExposeToInspector();

            position = Helper.ImguiDragFloat2("position", 0.1f, position);

            scale = Helper.ImguiDragFloat2("scale", 0.1f, scale);

            ImGui.DragFloat("rotation", ref rotation_degrees, 1.0f, -360f, 360f);

            ImGui.DragInt("z_index", ref z_index, 1, 0, 99);

            ImGui.Text("global position: " + global_position.ToString());
            ImGui.Text("global scale: " + global_scale.ToString());
            ImGui.Text("global rotation: " + global_rotation.ToString());
        }

    }
}
