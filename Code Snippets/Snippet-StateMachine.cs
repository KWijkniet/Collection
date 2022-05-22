private BaseWindow activeWindow;
private int activeIndex = -1;
private Vector2 scrollPos;

//Create a list of all windows to display
private static BaseWindow[] windows = new BaseWindow[]
{
    new DefaultPlayerDataWindow(),
    new ItemsWindow(),
    new SkillsWindow(),
    new LootTablesWindow(),
};

private void OnGUI()
{
    SetStyles();
    //If no window is selected then select the first window by default
    if (activeWindow == null) { SwitchWindow(0); }

    //Create a horizontal area in the layout
    GUILayout.BeginHorizontal();
    //Show all windows
    for (int i = 0; i < windows.Length; i++)
    {
        //Show button with style based on if its selected or not
        if (GUILayout.Button(windows[i].GetTitle(), activeIndex == i ? buttonActive : buttonNormal))
        {
            //Clear the focus
            GUI.FocusControl("");
            //Select the window
            SwitchWindow(i);
        }
    }
    //End horizontal layout
    GUILayout.EndHorizontal();

    //Start scrollable area
    scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
    //Start horizontal layout and add a space of 10 pixels from the left
    GUILayout.BeginHorizontal();
    GUILayout.Space(10);
    //Show the content of the selected window
    activeWindow?.OnGUI();
    //Add a space of 10 pixels to the right and end the horizontal layout
    GUILayout.Space(10);
    GUILayout.EndHorizontal();
    //End scrollable area
    EditorGUILayout.EndScrollView();
}

private void SwitchWindow(int index)
{
    //Stop users from opening the same window again
    if (activeIndex == index && activeWindow != null) { return; }

    //If there is already a selected window
    if (activeWindow != null)
    {
        //Then stop this window
        activeWindow.OnEnd();
    }

    //Switch the window
    activeIndex = index;
    activeWindow = windows[index];

    //Call the start method on the newly selected window
    activeWindow.OnStart();
}


//BaseWindow
public abstract class BaseWindow
{
    public abstract string GetTitle();
    public abstract void OnStart();
    public abstract void OnGUI();
    public abstract void OnEnd();
}