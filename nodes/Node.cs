using ImGuiNET;
using System.Text.Json;

namespace NitricEngine2D.nodes
{
    public class Node
    {
        public Guid id { get; private set; }

        public string name;

        public Node? parent { get; private set; } = null;

        private List<Node> children = new List<Node>();

        private List<Node> pendingChildren = new List<Node>();

        public enum STATE
        {
            BORN,
            RUNNING,
            PAUSED,
            DEAD
        }

        public STATE state { get; private set; }

        public Node(string name = "Node")
        {
            this.name = name;
            this.id= Guid.NewGuid();
            this.state = STATE.BORN;
        }

        public Node(JsonElement data)
        {
            this.name = Helper.JSONGetPropertyString(data, "name", "Node");
            this.id = Guid.NewGuid();
            this.state = STATE.BORN;
        }

        public void SetParent(Node newParent)
        {
            if(parent != null)
            {
                parent.RemoveChild(this);
            }
            parent = newParent;
            newParent.AddChild(this);
        }

        public void RemoveChild(Node child)
        {
            if (children.Contains(child))
            {
                children.Remove(child);
            }
        }

        public void AddChild(Node child)
        {
            if(state == STATE.BORN)
            {
                if (!children.Contains(child)) children.Add(child);
            }
            else
            {
                pendingChildren.Add(child);
            }
        }

        public virtual void Begin()
        {
            foreach(Node child in children)
            {
                child.SetParent(this);
                child.Begin();
            }

            state = STATE.RUNNING;
        }

        public void SetPaused(bool pause)
        {
            if(state == STATE.RUNNING || state == STATE.PAUSED)
            {
                state = pause ? STATE.PAUSED : STATE.RUNNING;
            }
        }

        public virtual void Update(float deltaTime)
        {
            if (state == STATE.PAUSED) return;

            foreach(Node child in children)
            {
                if(child.state == STATE.DEAD)
                {
                    pendingChildren.Add(child);
                    continue;
                }

                child.Update(deltaTime);
            }

            foreach(Node n in pendingChildren)
            {
                if(n.state == STATE.DEAD)
                {
                    children.Remove(n);
                }
                else if(n.state == STATE.BORN)
                {
                    if (!children.Contains(n)) children.Add(n);
                    n.Begin();
                }
                else
                {
                    if (!children.Contains(n)) children.Add(n);
                }
            }

            pendingChildren.Clear();
        }

        public virtual void Render(float deltaTime)
        {
            foreach(Node child in children)
            {
                child.Render(deltaTime);
            }
        }

        public virtual void End()
        {
            foreach(Node child in children)
            {
                child.End();
            }

            state = STATE.DEAD;
        }

        public T GetChildOfType<T>() where T : Node
        {
            foreach(Node child in children)
            {
                if(child is T)
                {
                    return (T)child;
                }
            }

            return null;
        }

        public Node GetNode(string name)
        {
            foreach(Node node in children)
            {
                if(node.name == name) return node;
            }
            return null;
        }

        public Node[] GetChildren()
        {
            return children.ToArray();
        }

        public T[] GetChildrenOfType<T>() where T : Node
        {
            List<T> result = new List<T>();

            foreach(Node child in children)
            {
                if (child is T)
                {
                    result.Add((T)child);
                }
            }

            return result.ToArray();
        }

        public virtual void ExposeToInspector()
        {
            ImGui.Text("Node type: " + this.GetType().Name);

            ImGui.Text("Name: " + name);

            ImGui.Text("ID: " + id.ToString());

            bool paused = state == STATE.PAUSED;

            ImGui.Checkbox("paused", ref paused);

            SetPaused(paused);
        }
    }
}
