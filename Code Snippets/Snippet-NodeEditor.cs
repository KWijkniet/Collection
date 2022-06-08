using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;

namespace NodeEditor
{
    /// <summary>
    /// The NodeEditor can be overwriten to create your own custom node editor.
    /// Extend your own class with this one. Doing so requires a custom node class which must extend the class "Node".
    /// To display the window you must create it yourself (see the example class).
    /// </summary>
    /// <typeparam name="T">Any class extending Node</typeparam>
    public class NodeEditor<T> : EditorWindow where T : Node, new()
    {
        #region Variables
        protected static NodeEditor<T> window;  //Keeps track of the active window

        //Events
        public static Action<T> OnCreate;       //An event that is triggered when a node is created
        public static Action<T> OnDelete;       //An event that is triggered when a node is deleted
        public static Action<T, T> OnBind;      //An event that is triggered when binding a node
        public static Action<int> OnNode;       //An event that is triggered when selecting a node

        //Data
        protected SerializedList<T> nodes;      //List of all nodes
        protected string targetFile;            //Path to file (for saving and loading the nodes)

        //Node area
        protected Rect graphPosition;           //Store the node area to detect out of bound nodes

        //ID tracking
        protected int masterNodeID;             //The first node
        protected int? focusedNodeID;           //The currently focused node
        protected int? prevFocusedNodeID;       //To detect changes when selecting a different node

        //Parent Control
        protected int? bindID;                  //Node id which is to be the child of the next selected node
        #endregion

        #region Initializing
        //Called when the GUI is created (not updated)
        public virtual void CreateGUI()
        {
            //Make sure we have a reference to the window
            if (window == null) { window = GetWindow<NodeEditor<T>>(); }

            //Initialize variables
            nodes = new SerializedList<T>();
            targetFile = "/-NodeEditorSaves/Nodes.dat";
            //Create main node
            CreateNode("Master Node", null);
        }
        #endregion

        #region Visuals
        //Called on GUI update
        public virtual void OnGUI()
        {
            //Make sure we have a reference to the window
            if (window == null) { window = GetWindow<NodeEditor<T>>(); }

            //Draw grid
            graphPosition = new Rect(250f, 25f, position.width - 250f, position.height - 25f);
            GraphBackground.DrawGraphBackground(graphPosition, graphPosition);
            
            //Draw inspector pannel
            DrawHeader();
            DrawInspector();
            DrawNodes();
        }

        //Visualise nodes
        protected virtual void DrawNodes()
        {
            //Show nodes
            BeginWindows();
            foreach (T dw in nodes)
            {
                //Create node window
                dw.rect = GUI.Window(dw.id, dw.rect, DrawNodeWindow, dw.title);
                //Draw line to each child node
                foreach (int childID in dw.childIDs)
                {
                    T child = GetById(childID);
                    if (child == null) { continue; }

                    if (childID == focusedNodeID)
                    {
                        DrawLine(dw.rect, child.rect, Color.blue, 8f);
                    }
                    else
                    {
                        DrawLine(dw.rect, child.rect, Color.white);
                    }
                }
            }
            EndWindows();
        }

        //Display the inspector panel (left side panel)
        protected virtual void DrawInspector()
        {
            Rect panel = new Rect(5, 30, 240f, window.position.height - 35f);
            GUILayout.BeginArea(panel);
            GUILayout.Label("Override the\n 'DrawInspector'\n method to display content here");
            GUILayout.EndArea();
        }

        //Display the header panel (top bar panel)
        protected virtual void DrawHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save"))
            {
                Save();
            }
            if (GUILayout.Button("Load"))
            {
                Load();
            }
            GUILayout.EndHorizontal();
        }

        //Draws the node
        protected virtual void DrawNodeWindow(int id)
        {
            T node = GetById(id);

            //Check if it is focussed
            Event E = Event.current;
            if ((E.type == EventType.MouseDown ||
                E.type == EventType.MouseUp ||
                E.type == EventType.MouseDrag ||
                E.type == EventType.MouseMove) || E.GetTypeForControl(id) == EventType.Used)
            {
                GUI.FocusWindow(id);
                if (focusedNodeID != id)
                {
                    OnNode?.Invoke(id);
                }
                focusedNodeID = id;
            }

            //Check if its inside the accepted bounds
            if (!IsInBounds(node.rect))
            {
                if (node.rect.position.x < graphPosition.position.x)
                {
                    node.rect.position = new Vector2(graphPosition.position.x, node.rect.position.y);
                }
                else if (node.rect.position.x + node.rect.width > graphPosition.position.x + graphPosition.width)
                {
                    node.rect.position = new Vector2(graphPosition.position.x + graphPosition.width - node.rect.width, node.rect.position.y);
                }

                if (node.rect.position.y < graphPosition.position.y)
                {
                    node.rect.position = new Vector2(node.rect.position.x, graphPosition.position.y);
                }
                else if (node.rect.position.y + node.rect.height > graphPosition.position.y + graphPosition.height)
                {
                    node.rect.position = new Vector2(node.rect.position.x, graphPosition.position.y + graphPosition.height - node.rect.height);
                }
            }

            GUILayout.BeginHorizontal();
            if((masterNodeID != id || (masterNodeID == id && bindID != null)) && bindID != id)
            {
                if (GUILayout.Button("Bind"))
                {
                    Bind(id);
                }
            }
            else if(bindID == id)
            {
                if (GUILayout.Button("Cancel"))
                {
                    bindID = null;
                }
            }

            if(bindID == null)
            {
                if (GUILayout.Button("+"))
                {
                    CreateNode("Child of " + node.title, id);
                }
                if (id != masterNodeID && GUILayout.Button("-"))
                {
                    DestroyNode(id);
                }
            }
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }
        #endregion

        #region Node Management
        //Draws the line that connects the nodes
        protected virtual Vector3[] DrawLine(Rect start, Rect end, Color color, float thickness = 5f)
        {
            Handles.BeginGUI();
            Handles.color = Color.white;
            Handles.DrawBezier(start.center, end.center, new Vector2(start.xMax - 95f, start.center.y), new Vector2(end.xMin + 95f, end.center.y), color, null, thickness);
            Vector3[] points = Handles.MakeBezierPoints(start.center, end.center, new Vector2(start.xMax - 95f, start.center.y), new Vector2(end.xMin + 95f, end.center.y), 20);
            Handles.EndGUI();

            return points;
        }

        //Generate rect at the correct position
        protected virtual Rect GenerateRect(int? parentID)
        {
            Vector2 center = new Vector2(window.position.width / 2, window.position.height / 2);
            Rect rect = new Rect(center.x, center.y, 200, 50);
            if (parentID == null)
            {
                rect.position = new Vector2(window.position.width / 2, 75);
                return rect;
            }

            T parent = GetById((int)parentID);

            if (parent.childIDs.Count > 0)
            {
                T child = GetById(parent.childIDs[parent.childIDs.Count - 1]);
                rect.position = new Vector2(child.rect.position.x + 400, child.rect.position.y);
            }
            else
            {
                rect.position = new Vector2(parent.rect.position.x, parent.rect.position.y + 100);
            }

            if (!IsInBounds(rect))
            {
                rect.position = center;
            }

            return rect;
        }

        //Check if the given node is inside the window
        protected virtual bool IsInBounds(Rect rect)
        {
            if (rect.position.x < graphPosition.position.x || rect.position.x + rect.width > window.position.width) { return false; }
            if (rect.position.y < graphPosition.position.y || rect.position.y + rect.height > window.position.height) { return false; }
            return true;
        }

        //Bind first selected as child of second selected
        protected virtual void Bind(int id)
        {
            //If its the first bind then set the bindID
            if (bindID == null)
            {
                bindID = id;
                OnBind?.Invoke(GetById((int)bindID), null);
                return;
            }

            //If its the second bind then (un)bind the 2 nodes
            if (bindID != null)
            {
                T node = GetById(id);
                //If they are already bound then remove it. If not then add it.
                if (node.childIDs.Contains((int)bindID))
                {
                    node.childIDs.Remove((int)bindID);
                    GetById((int)bindID).parentIDs.Remove(id);
                }
                else
                {
                    node.childIDs.Add((int)bindID);
                    GetById((int)bindID).parentIDs.Add(id);
                }
                OnBind?.Invoke(GetById((int)bindID), GetById(id));
                bindID = null;
            }
        }

        //Creates a new node
        protected virtual T CreateNode(string name, int? parentID)
        {
            //Create new instance of T
            T node = (T)Activator.CreateInstance(typeof(T), name, GenerateRect(parentID));

            //if no parentID has been set then set it as the master node
            if (parentID == null)
            {
                masterNodeID = node.id;
            }

            //Focus the new node
            GUI.FocusWindow(node.id);
            focusedNodeID = node.id;

            //Add the new node
            nodes.Add(node);
            OnCreate?.Invoke(node);
            OnNode?.Invoke(node.id);

            if (parentID != null)
            {
                Bind(node.id);
                Bind((int)parentID);
            }
            return node;
        }

        //Destroys a node and all of its connected child nodes (recursive)
        protected virtual void DestroyNode(int id, int? parentID = null)
        {
            T data = GetById(id);
            //If it has more then 1 parent
            if(data.parentIDs.Count > 1)
            {
                //If a parent id was given (current node was NOT deleted by the user but via the loop)
                if (parentID != null)
                {
                    //Unbind this node from its parent
                    Bind(id);
                    Bind((int)parentID);

                    //Dont execute the rest of this function
                    return;
                }
            }

            //Call DestroyNode on all child nodes
            for (int i = data.childIDs.Count - 1; i >= 0; i--)
            {
                DestroyNode(data.childIDs[i], data.id);
            }

            //If parentID is null (current node was deleted by the user)
            if (parentID == null)
            {
                //Delete all binds to the parents
                for(int i = data.parentIDs.Count - 1; i >= 0; i--)
                {
                    Bind(id);
                    Bind(data.parentIDs[i]);
                }
                data.parentIDs.Clear();
            }
            //If parentID is not null (current node was deleted via the loop)
            else
            {
                //Delete binds to the parent
                Bind(id);
                Bind((int)parentID);
            }

            //If this node has no parents left
            if (data.parentIDs.Count <= 0)
            {
                //Destroy this node
                nodes.Remove(data);
                //If it was currently selected then unselect this node
                if(focusedNodeID == id)
                {
                    focusedNodeID = null;
                }
                OnDelete?.Invoke(data);
            }
        }

        //Does a window with this ID exist
        protected virtual bool Exists(int id)
        {
            return GetById(id) != null;
        }

        //Search for the correct window by the given ID
        protected virtual T GetById(int id)
        {
            //Loop through all nodes
            foreach (T node in nodes)
            {
                //If id's match
                if (node.id == id)
                {
                    //return found node
                    return node;
                }
            }
            //No match found. returning null
            return null;
        }
        #endregion

        #region File Management
        //Save the current node array to a file
        protected virtual void Save()
        {
            //Fix file path
            FixPath();
            //Generate file path if it doesn't yet exist
            GeneratePath(targetFile);

            //If the file doesn't exist then create it
            if (!File.Exists(Application.dataPath + targetFile))
            {
                FileStream file = File.Create(Application.dataPath + targetFile);
                file.Close();
            }
            //Save data to the file
            string json = JsonUtility.ToJson(nodes);
            File.WriteAllText(Application.dataPath + targetFile, json);
        }

        //Load node array frome file
        protected virtual void Load()
        {
            //Fix file path
            FixPath();

            //If the file doesn't exist
            if (!File.Exists(Application.dataPath + targetFile))
            {
                nodes = new SerializedList<T>();
                Debug.LogWarning("Node Editor: File not found. Cannot load");
                return;
            }

            //Load data from the file
            string json = File.ReadAllText(Application.dataPath + targetFile);
            nodes = JsonUtility.FromJson<SerializedList<T>>(json);
        }

        //Fixes the targetFile path to prevent errors
        protected virtual void FixPath()
        {
            //Remove the dataPath from the path
            targetFile = targetFile.Replace(Application.dataPath, "");
            //Ensure the path starts with a /
            if (!targetFile.StartsWith("/"))
            {
                targetFile = "/" + targetFile;
            }
        }

        //Generate the targetFile path to prevent errors
        protected virtual void GeneratePath(string target)
        {
            //Split the path into parts
            string[] parts = target.Split('/');
            //Remove the file + extension from the parts
            parts[parts.Length - 1] = "";

            //Create base path
            string path = Application.dataPath;
            //Loop over each part
            foreach (string part in parts)
            {
                //If the part is empty then ignore it
                if(part.Length <= 0) { continue; }

                //Create path from parts
                path += "/" + part;
                //If the directory doesn't exist
                if (!Directory.Exists(path))
                {
                    //Then create the directory
                    Directory.CreateDirectory(path);
                }
            }
        }
        #endregion
    }

    //A class required to serialize the data so it can be saved to and loaded from files.
    [System.Serializable]
    public class SerializedList<T>
    {
        //Create list of T
        public List<T> nodes = new List<T>();

        //Return node count
        public int Count { get { return nodes.Count; } }

        //Add new node to list
        public void Add(T node)
        {
            nodes.Add(node);
        }

        //Remove node from list
        public void Remove(T node)
        {
            nodes.Remove(node);
        }

        //Allow looping over the list (example: foreach)
        public IEnumerator<T> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        //Allow indexing on the list (example: nodes[1])
        public T this[int index]
        {
            get => nodes[index];
            set => nodes[index] = value;
        }
    }
}