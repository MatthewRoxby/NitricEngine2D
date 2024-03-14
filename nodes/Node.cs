using ImGuiNET;
using System.Text.Json;

namespace NitricEngine2D.nodes
{
    /// <summary>
    /// base node class
    /// </summary>
    public class Node
    {
        //unique identifier for each node
        public Guid id { get; private set; }

        public string name;

        public Node? parent { get; private set; } = null;

        //the children attached to the node
        private List<Node> children = new List<Node>();

        //children yet to be added
        private List<Node> pendingChildren = new List<Node>();

        //the possible states of the node
        public enum STATE
        {
            BORN,
            RUNNING,
            PAUSED,
            DEAD
        }

        public STATE state { get; private set; }

        /// <summary>
        /// creates a node instance
        /// </summary>
        /// <param name="name">the name of the node</param>
        public Node(string name = "Node")
        {
            this.name = name;
            //create a new ID for the node
            this.id= Guid.NewGuid();
            //set state to born
            this.state = STATE.BORN;
        }

        /// <summary>
        /// creates a node instance from JSON data
        /// </summary>
        /// <param name="data">the JSON element</param>
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

        /// <summary>
        /// starts the node and all children
        /// </summary>
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

        /// <summary>
        /// updates the node by a delta time
        /// </summary>
        /// <param name="deltaTime">time since last update</param>
        public virtual void Update(float deltaTime)
        {
            

            foreach(Node child in children)
            {
                if(child.state == STATE.DEAD)
                {
                    pendingChildren.Add(child);
                    continue;
                }

                if(child.state != STATE.PAUSED) child.Update(deltaTime);

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

        /// <summary>
        /// Render the node to the screen
        /// </summary>
        /// <param name="deltaTime">time since last render</param>
        public virtual void Render(float deltaTime)
        {
            foreach(Node child in children)
            {
                child.Render(deltaTime);
            }
        }

        /// <summary>
        /// frees all resources the node created and deletes all children
        /// </summary>
        public virtual void End()
        {
            foreach(Node child in children)
            {
                child.End();
            }

            state = STATE.DEAD;
        }

        /// <summary>
        /// Get first child of a given type
        /// </summary>
        /// <typeparam name="T">the type to search for</typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// get child by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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

        /// <summary>
        /// get all children of the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// exposes the node to the inspector debug window
        /// </summary>
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
