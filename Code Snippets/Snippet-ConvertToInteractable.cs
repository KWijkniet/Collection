//Menu path of the buttons
private const string MENU_ITEM_PATH = "Tools/Selected/Convert to Interactables/";
//Prevent the functions from being triggered multiple times (an unity issue/feature that ocurs when you select multiple objects)
private static int lastInstanceID;

//Validate the current selection (if atleast 1 or more have been selected)
private static bool Validate()
{
	return Selection.count >= 1 && lastInstanceID != Selection.activeInstanceID;
}

//Create base GameObject
private static GameObject CreateGameObject(Transform source)
{
	//Create instance of the base interactable prefab
	var prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/0_Custom/Resources/SpawnablePrefabs/Interactable.prefab", typeof(GameObject));
	GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

	//Match sources position, rotation and scale
	obj.transform.SetAsLastSibling();
	obj.transform.position = source.position;
	obj.transform.rotation = source.rotation;
	obj.transform.localScale = source.localScale;

	//Lets make the name easy to read and informative :D
	obj.name = source.gameObject.name + $" (x:{source.position.x:0} y:{source.position.y:0} z:{source.position.z:0})";

	//Add required components
	BoxCollider bc = obj.GetComponent<BoxCollider>();
	MeshFilter mf = obj.GetComponent<MeshFilter>();
	MeshRenderer mr = obj.GetComponent<MeshRenderer>();

	//Get mesh filter from GameObject. If its not found check its child GameObjects
	MeshFilter smf = source.GetComponent<MeshFilter>();
	if(smf == null)
	{
		smf = source.GetComponentInChildren<MeshFilter>();
	}

	//Set Mesh
	mf.mesh = smf.sharedMesh;

	//Match bounds for the collider
	bc.center = mf.sharedMesh.bounds.center;
	bc.size = mf.sharedMesh.bounds.size;
	//bc.size = mf.sharedMesh.bounds.size + new Vector3(0.1f, 0.1f, 0.1f);

	//Set correct shadow mode (off)
	mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

	return obj;
}

[MenuItem(MENU_ITEM_PATH + "Container")]
private static void Container()
{
	//validate selected GameObject
	if(!Validate()) { return; }
	//ensure this function is only called once when selecting multiple GameObjects
	lastInstanceID = Selection.activeInstanceID;

	//Loop over all selected GameObjects
	foreach (Transform source in Selection.transforms)
	{
		//Create a copy of the selected GameObject (a version with only the bare minimum info)
		GameObject obj = CreateGameObject(source);
		obj.name = "Container_" + obj.name;

		//Add required components for a container
		InventoryController ic = obj.AddComponent<InventoryController>();
		InteractableContainerController icc = obj.AddComponent<InteractableContainerController>();

		//Set inventory data
		ic.syncInterval = 0f;
		ic.saveToFile = true;
		ic.maxWeight = 50;

		//Set interactable data
		icc.outlineMaterial = Resources.Load<Material>("Materials/OutlineMaterial");
		icc.hoverMessage = "Press [KEY] to open container";
		icc.rend = obj.GetComponent<MeshRenderer>();
		icc.inventory = ic;
		icc.syncInterval = 0.1f;
		icc.destroyOnEmpty = false;
		icc.isBeingInteractedWith = false;
		icc.generatedInventory = false;
	}
}