using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFMLApp
{
	class Item
	{
		protected int Damage;
		protected int Range;
		public int attack(){return Damage;}
		public int getRange() { return Range; }
	}
    class ItemSword : Item
    {
        public ItemSword(int dmg, int ran)
        {
             Damage = dmg;
             Range = ran;
        }
     }
        class ItemBow : Item
        {
            private int nArrows;
            public ItemBow(int dmg, int ran, int arr)
            {
                Damage = dmg;
                Range = ran;
                nArrows = arr;
            }
            public int attack()
            {
                if (nArrows > 0)
                {
                    nArrows--;
                    return Damage;
                }
                else { return 0; }
            }
        }
        /*class Magic : Item
        {
            private int ManaCost;
            public Magic(int dmg, int ran, int mana)
            {
                Damage = dmg;
                Range = ran;
                ManaCost = mana;
            }
            public int attack(ref int mana)
            {
                if (mana >= ManaCost)
                {
                    mana = mana-ManaCost;
                    return Damage;
                }else
                {
                    return 0;
                }
            }
        }*/
		class Fist : Item{
			public Fist(){
				Damage = 5;
				Range = 1;
			}
		}
	class Inventory
	{
		List<Item> inventory;
        public Inventory()
        {
			inventory = new List<Item>(20);
        }
        public void addItem(Item item)
        {
            if (inventory.Count < 20)
            {
				inventory.Add(item);
            }
        }
        public bool isInStock(Item item)
        {
            return inventory.Contains(item);
        }
        public int howMuchItems(Item item)
        {
            int total=0;
            for(int i = 0; i < inventory.Count; i++)
            {
				if (inventory[i].Equals(item)) { total++; }
            }
            return total;
        }
    }
}
