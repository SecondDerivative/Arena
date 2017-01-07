using System.Text;

namespace SFMLApp
{
    public class Inventory
    {
        private int Mana;
        private int currentArrow;
        private int[] inventory;
        public static int totalNumberofItems = 15;//Both starts counting with 0
        public Inventory()
        {
            Mana = 100;
            inventory = new int[totalNumberofItems + 1];
            currentArrow = 0;//changes from 0 to 2;
        }

        public Item getItem(int i)
        {
            if (inventory[i] > 0)
            {
                return Items.allItems[i];
            }
            else
            {
                return null;
            }
        }
        public int getArrowsAmount()
        {
            return inventory[currentArrow];
        }
        public void addArrows(Arrow a, int i)
        {
            inventory[a.id] += i;
        }
        public Arrow getCurrentArrow()
        {
            return (Arrow)Items.allItems[currentArrow];
        }
        public void setCurrentArrow(int i)
        {
            currentArrow = i;
        }
        public int getMana()
        {
            return Mana;
        }
        public void addMana(int i)
        {
            Mana += i;
        }

        public void addItem(int id, int cnt)
        {
            if (Items.allItems[id] is ManaBottle)
                ((ManaBottle)Items.allItems[id]).Consume(this);
            else if (Items.allItems[id] is HPBottle)
                ((HPBottle)Items.allItems[id]).Consume(this);
            else
                inventory[id] += cnt;
        }

        public void addItem(Item item)
        {
            addItem(item.id, 1);
        }

        public void addItem(Item item, int cnt)
        {
            addItem(item.id, cnt);
        }

        public void addItem(int id)
        {
            addItem(id, 1);
        }

        public bool isInStock(int id)
        {
            return inventory[id] > 0;
        }

        public bool isInStock(Item item)
        {
            return inventory[item.id] > 0;
        }

        public int howMuchItems(int id)
        {
            return inventory[id];
        }

        public int howMuchItems(Item item)
        {
            return inventory[item.id];
        }

        public void clearInventory()
        {
            for (int i = 1; i < totalNumberofItems; i++)
            {
                inventory[i] = 0;
            }
        }

        public string LargeString()
        {
            StringBuilder ans = new StringBuilder();
            ans.Append(Mana);
            ans.Append(" ");
            ans.Append(currentArrow);
            ans.Append(" ");
            ans.Append(totalNumberofItems);
            for (int i = 0; i < totalNumberofItems; i++)
            {
                ans.Append(" ");
                ans.Append(inventory[i]);
            }
            return ans.ToString();
        }
        public string SmallString()
        {
            return currentArrow.ToString();
        }
    }
}