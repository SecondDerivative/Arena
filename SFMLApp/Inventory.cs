using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFMLApp
{
	public class Inventory
	{
		private int Mana;
		private int nArrows;
		List<int> inventory;
        public Inventory()
        {
			Mana = 100;
			nArrows = 0;
			inventory = new List<int>(20);
        }
		public Item getItem(int i){
			return Items.allItems[i];
		}
		public int getArrows(){return nArrows;}
		public void addArrows(int i){nArrows = nArrows + i;}
		public int getMana(){return Mana;}
		public void addMana(int i){Mana = Mana + i;}
        public void addItem(Item item)
        {
			for(int i = 0; i < Items.allItems.Capacity; i++) {
				if (Items.allItems[i].Equals(item))
					inventory.Add(i);
			}
        }
        public bool isInStock(Item item)
        {
			bool Contains=false;
			foreach(int i in inventory){
				if (Items.allItems[i].Equals (item)) {
					Contains = true;
					break;
				}
			}
			return Contains;
        }
        public int howMuchItems(Item item)
        {
            int total=0;
			foreach(int i in inventory){
				if (Items.allItems[i].Equals (item)) { total++; }
			}
            return total;
        }
    }
}
