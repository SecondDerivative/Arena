namespace SFMLApp
{
	public class Inventory
	{
		private int Mana;
		private int[] nArrows;
		private int currentArrow;
		private int [] inventory;
		private static int totalNumberofItems = 20;
		private static int totalNumberofArrows = 3;
        public Inventory()
        {
			Mana = 100;
			nArrows = new int[totalNumberofArrows];
			inventory = new int[totalNumberofItems];
        }
		public Item getItem(int i){
			if (inventory [i] > 0)
				return Items.allItems[i];
			else
				return null;
		}
		public int getArrowsAmount(){return nArrows[currentArrow];}
		public void addArrows(Arrow a, int i){nArrows[a.id] = nArrows[a.id] + i;}
		public Arrow getCurrentArrow() { return Items.allArrows[currentArrow]; }
		public void setCurrentArrow(int i){ currentArrow = i; }
		public int getMana(){return Mana;}
		public void addMana(int i){Mana = Mana + i;}
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
