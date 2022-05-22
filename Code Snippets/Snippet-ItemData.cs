[CreateAssetMenu(menuName = "Custom/New Item", fileName = "New Item")]
public class ItemData : ScriptableObject
{
	[HideInInspector]
	public string uuid = Guid.NewGuid().ToString(); //easier to use with networking in a string format then in the GUID format

	//Values that count for all items
	public string displayName;
	public string description;
	public Sprite icon;
	public int stackLimit = 0;
	public int level;
	public float weight;
	public bool canScrap;
	public List<ItemWithAmount> scrap = new List<ItemWithAmount>();
	public ItemQuality quality;
	public ItemType type;

	//Equipment, Consumables and Armor only
	public List<ItemStats> stats = new List<ItemStats>();
	public ItemCategory category;

	//Armor only
	public ArmorSlot armorSlot;
}

public enum ItemType
{
	None,
	Equipment,
	Consumable,
	Armor,
	Resource,
	Junk,
}

public enum ItemQuality
{
	Common,
	Uncommon,
	Rare,
	Unique,
}

public enum ItemCategory
{
	None,
	Light,
	Medium,
	Heavy,
}

public enum ArmorSlot
{
	Head,
	Chest,
	Legs,
	Feet,
	Arms,
	Hands,
}