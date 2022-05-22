public class LootTable : ScriptableObject
{
	public string displayName = "";
	public Vector2Int minMaxRolls = new Vector2Int(1,1);
	public List<LootTableEntry> entries = new List<LootTableEntry>();

	//Generate a random list of items
	public List<ItemWithAmount> Generate()
	{
		List<ItemWithAmount> items = new List<ItemWithAmount>();
		//Rolls determine how many times each entry is allowed to generate items
		int rolls = Random.Range(minMaxRolls.x, minMaxRolls.y + 1);
		Debug.Log($"Generating with {rolls} rolls");

		//Order all entries by weight
		List<LootTableEntry> tmp = entries.OrderBy(x => x.weight).Reverse().ToList();
		
		for (int i = 0; i < rolls; i++)
		{
			//Calculate the max weight in the entries
			int totalWeight = 0;
			for (int r = 0; r < entries.Count; r++)
			{
				totalWeight += entries[r].weight;
			}

			//Select a random value between 0 and total weight (inclusive)
			int selectedWeight = Random.Range(0, totalWeight + 1);
			int currentWeight = 0;

			//Find the selected weight value
			for (int r = 0; r < tmp.Count; r++)
			{
				currentWeight += tmp[r].weight;
				
				if(currentWeight >= selectedWeight)
				{
					//If it has been found then let the entry generate item(s) and add these to the list
					items.AddRange(tmp[r].GetItems());
					break;
				}
			}
		}

		return items;
	}
}

[System.Serializable]
public class LootTableEntry
{
	public LootTableEntryType type;
	public int weight = 1;
	public Vector2Int minMaxAmount = new Vector2Int(1, 1);

	public string itemUuid = "";
	//OR
	public LootTable lootTable;

	//Get items based on the settings
	public List<ItemWithAmount> GetItems()
	{
		//Get a random amount between the set min and max (inclusive)
		int rand = Random.Range(minMaxAmount.x, minMaxAmount.y + 1);
		//If its 0 then return an empty list
		if(rand <= 0) { return new List<ItemWithAmount>(); }

		//Based on the type add either the item, return the results of another loot table or return an empty list.
		switch (type)
		{
			case LootTableEntryType.Item:
				return new List<ItemWithAmount>() { new ItemWithAmount() { amount = rand, itemUuid = itemUuid } };
			case LootTableEntryType.LootTable:
				return lootTable.Generate();
			default:
				return new List<ItemWithAmount>();
		}
	}
}

public enum LootTableEntryType
{
	Item,
	LootTable,
}