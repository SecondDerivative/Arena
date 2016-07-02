namespace SFMLApp
{
	public class Inventory {
		private int Mana;
		private int[] nArrows;
		private int currentArrow;
		private int [] inventory;
		public static int totalNumberofItems = 9;//Both starts counting with 0
		public static int totalNumberofArrows = 2;//Starts counting with 0
		public Inventory() {
			Mana = 100;
			nArrows = new int[totalNumberofArrows+1];
			inventory = new int[totalNumberofItems+1];
			currentArrow = 0;//changes from 0 to 2;
        }

		public Item getItem(int i) {
			if (inventory[i] > 0) {
				return Items.allItems[i];
			} else {
				return null;
			}
		}
		public int getArrowsAmount(){return nArrows[currentArrow];}
		public void addArrows(Arrow a, int i){nArrows[a.id] = nArrows[a.id] + i;}
		public Arrow getCurrentArrow() { return (Arrow)Items.allItems[totalNumberofItems+currentArrow]; }
		public void setCurrentArrow(int i){ currentArrow = i; }
		public int getMana(){return Mana;}
		public void addMana(int i){Mana = Mana + i;}
		public void addItem(Item item) {
			inventory[item.id]++;
        }

        public void addItem(Item item, int cnt) {
            inventory[item.id] += cnt;
        }

        public void addItem(int id) {
            inventory[id]++;
        }

        public void addItem(int id, int cnt) {
            inventory[id] += cnt;
        }

        public bool isInStock(Item item) {
			return inventory[item.id] > 0;
        }

		public int howMuchItems(Item item) {
			return inventory[item.id];
        }

		public void clearInventory() {
			for (int i = 1; i < totalNumberofItems; i++) {
				inventory [i] = 0;
			}
		}
    }
}