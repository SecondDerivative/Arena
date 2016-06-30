namespace SFMLApp
{
	public class Inventory
	{
		private int Mana;
		private int nArrows;
		private int [] inventory;
		public static int totalNumberofItems = 20;

		public Inventory() {
			Mana = 100;
			nArrows = 0;
			inventory = new int[totalNumberofItems];
        }

		public Item getItem(int i) {
			if (inventory[i] > 0) {
				return Items.allItems[i];
			} else {
				return null;
			}
		}

		public int getArrows() {
			return nArrows;
		}

		public void addArrows(int i) {
			nArrows = nArrows + i;
		}

		public int getMana() {
			return Mana;
		}

		public void addMana(int i) {
			Mana = Mana + i;
		}

		public void addItem(Item item)
        {
			inventory[item.id]++;
        }

		public bool isInStock(Item item)
        {
			return inventory[item.id] > 0;
        }

		public int howMuchItems(Item item)
        {
			return inventory[item.id];
        }

		public void clearInventory(){
			for (int i = 1; i < totalNumberofItems; i++) {
				inventory [i] = 0;
			}
		}
    }
}
