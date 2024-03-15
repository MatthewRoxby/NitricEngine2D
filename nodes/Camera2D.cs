using ImGuiNET;
using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;
using System.Diagnostics;
using System.Text.Json;

namespace NitricEngine2D.nodes
{
    /// <summary>
    /// A 2d class that defines an orthographic camera
    /// </summary>
    public class Camera2D : Node2D
    {
        const float CAMERA_VISIBILITY_BORDER = 1.1f;
        public Box2 bounds { get; private set; }

        public float zoom = 1f;

        public float near, far;

        public Matrix4 projection, view;

        public int width, height;

        public Camera2D(float near, float far)
        {
            this.near = near;
            this.far = far;
            
        }

        public Camera2D(JsonElement data) : base(data)
        {
            this.near = Helper.JSONGetPropertyFloat(data, "near", 0.1f);
            this.far = Helper.JSONGetPropertyFloat(data, "far", 100f);
            this.zoom = Helper.JSONGetPropertyFloat(data, "zoom", 1f);
            
        }

        public override void Begin()
        {
            base.Begin();
            //question: how to get the parent viewport?
            //solution: check parent recursively until parent is of type Viewport2D
            Node p = parent;
            while(true)
            {
                if(p == null)
                {
                    Debug.WriteLine($"camera {name} did not find a parent viewport.");
                    break;
                }
                else if(p.GetType() == typeof(Viewport2D))
                {
                    Viewport2D v = p as Viewport2D;
                    v.renderCamera = this;
                    break;
                }
                else
                {
                    p = p.parent;
                }
                
            }
        }

        public override void Update(float deltaTime)
        {
            
            AL.Listener(ALListener3f.Position, global_position.X, global_position.Y, 0f);
            //Debug.WriteLine($"{ALC.GetError(ALC.GetContextsDevice(ALC.GetCurrentContext())).ToString()}");

            zoom = MathHelper.Clamp(zoom, 0.001f, 100f);
            Vector2 v = new Vector2(width, height) * zoom;
            projection = Matrix4.CreateOrthographic(v.X, v.Y, near, far);

            bounds = new Box2() { Center = global_position, Size = v * CAMERA_VISIBILITY_BORDER};


            float r = MathHelper.DegreesToRadians(global_rotation);

            Vector3 up = Vector3.UnitY;

            view = Matrix4.LookAt(new Vector3(global_position.X, global_position.Y, 0f), new Vector3(global_position.X, global_position.Y, -1f), Quaternion.FromEulerAngles(0f, 0f, r) * up);

            base.Update(deltaTime);
        }

        public override void ExposeToInspector()
        {
            base.ExposeToInspector();

            ImGui.DragFloat("zoom", ref zoom, 0.1f, 0.1f, 100f);

            ImGui.Text($"Bounds: \n{bounds.ToString()}");
        }
    }
}
