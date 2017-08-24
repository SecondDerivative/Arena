using System.Diagnostics;
using System.Text;

namespace SFMLApp
{
	public class Player
	{
        private const int MAX_HP = 150;
		public int Health { get; private set; }
		public Inventory inventory{ get; private set; }
		public int leftHand{ get; private set; }
		public int rightHand{ get; private set; }
        private Stopwatch LeftReloadTimer, RightReloadTimer;
		public Player()
		{
			inventory = new Inventory();
			inventory.addItem(Items.allItems[0]);
			//setting Health to 100
			Health = 100;
			//setting fists as a weapon
			leftHand = 0;
			rightHand = 0;
            LeftReloadTimer = new Stopwatch();
            LeftReloadTimer.Start();
            RightReloadTimer = new Stopwatch();
            RightReloadTimer.Start();
		}
        public int attack() {
            int total = 0;
            if (LeftReloadTimer.ElapsedMilliseconds >= ((Weapon)Items.allItems[leftHand]).Reloading)
            {
                LeftReloadTimer.Restart();
                if (inventory.getItem(leftHand).GetType() == typeof(ItemBow))
                    total = total + ((ItemBow)inventory.getItem(leftHand)).attack(inventory);
                else if (inventory.getItem(leftHand).GetType() == typeof(Magic))
                    total = total + ((Magic)inventory.getItem(leftHand)).attack(inventory);
                else if (inventory.getItem(leftHand).GetType() == typeof(ItemSword))
                    total = total + ((ItemSword)inventory.getItem(leftHand)).attack();
            }
            if (RightReloadTimer.ElapsedMilliseconds >= ((Weapon)Items.allItems[rightHand]).Reloading)
            {
                RightReloadTimer.Restart();
                if (inventory.getItem(rightHand).GetType() == typeof(ItemBow))
                    total = total + ((ItemBow)inventory.getItem(rightHand)).attack(inventory);
                else if (inventory.getItem(rightHand).GetType() == typeof(Magic))
                        total = total + ((Magic)inventory.getItem(rightHand)).attack(inventory);
                else if(inventory.getItem(leftHand).GetType() == typeof(ItemSword))
                    total = total + ((ItemSword)inventory.getItem(rightHand)).attack();
            }
        	return total;
		}
		public void recieveDamage(int dmg){
			if (Health > 0 && Health < dmg)
				Health = 0;
			else
				Health = Health - dmg;
		}
		public bool isDead(){
			return Health <= 0;
		}
		public void takeItemLeft(Item i){
			if (inventory.isInStock(i))
				leftHand = i.id;
		}
		public void takeItemRight(Item i){
			if (inventory.isInStock (i))
				rightHand = i.id;
		}
		public Item getItemLeft(){
			return inventory.getItem(leftHand);
		}
		public Item getItemRight(){
			return inventory.getItem(rightHand);
		}
		public void pickUpArrow(Arrow arrowType, int nArrowsPickedUp){
			inventory.addArrows(arrowType, nArrowsPickedUp);
		}
		public void addedMana(int nManaAdded){
			inventory.addMana(nManaAdded);
		}
        public void pickedUpItem(int id, int cnt)
        {
            if (Items.allItems[id] is ManaBottle)
                ((ManaBottle)Items.allItems[id]).Consume(this);
            else if (Items.allItems[id] is HPBottle)
                ((HPBottle)Items.allItems[id]).Consume(this);
            else
                inventory.addItem(id, cnt);
        }
        public void pickedUpItem(Item i)
        {
            pickedUpItem(i.id, 1);
		}
		public void pickedUpItem(int id)
        {
            pickedUpItem(id, 1);
        }
        public void pickedUpItem(Item i, int cnt)
        {
            pickedUpItem(i.id, cnt);
        }
        public void selectArrow(Arrow i)
        {
            inventory.setCurrentArrow(i.id);
        }
        public void respawn(){
			Health = 100;
			inventory.clearInventory ();
			rightHand = 0;
			leftHand = 0;
            LeftReloadTimer.Restart();
            RightReloadTimer.Restart();
            rightHand = 1;
            //start kit
            inventory.addItem(1);
            inventory.addItem(7, 10);
            inventory.setCurrentArrow(7);
        }
        public double Speed()
        {
            //need change
            return 0.05;
        }
        public double ArrowSpeed()
        {
            if (Items.allItems[rightHand] is Magic)
                return ((Magic)Items.allItems[rightHand]).speed();
            return ((Arrow)Items.allItems[inventory.getCurrentArrow().id]).speed();
        }
        public void HealHP(int HPHealed)
        {
            if (Health + HPHealed >= MAX_HP)
            {
                Health = MAX_HP;
            }else
            {
                Health += HPHealed;
            }
        }
        public void NextItem()
        {
            int yk = rightHand + 1;
            int cntItem = Inventory.totalNumberofItems;
            while (!inventory.isInStock(yk % cntItem) || yk % cntItem == 0
                || Items.allItems[yk % cntItem] is Arrow)
                ++yk;
            rightHand = yk % cntItem;
        }
        public void PrevItem()
        {
            int yk = rightHand - 1;
            int cntItem = Inventory.totalNumberofItems;
            while (!inventory.isInStock((yk + cntItem) % cntItem) || (yk + cntItem) % cntItem == 0
                || Items.allItems[(yk + cntItem) % cntItem] is Arrow)
                --yk;
            rightHand = (yk + cntItem) % cntItem;
        }
        public void NextArrow()
        {
            int yk = inventory.getCurrentArrow().id + 1;
            int cntItem = Inventory.totalNumberofItems;
            while (!inventory.isInStock(yk % cntItem) || !(Items.allItems[yk % cntItem] is Arrow))
                ++yk;
            inventory.setCurrentArrow(yk % cntItem);
        }
        public void PrevArrow()
        {
            int yk = inventory.getCurrentArrow().id - 1;
            int cntItem = Inventory.totalNumberofItems;
            while (!inventory.isInStock((yk + cntItem) % cntItem) || !(Items.allItems[(yk + cntItem) % cntItem] is Arrow))
                --yk;
            inventory.setCurrentArrow(yk % cntItem);
        }
        public string LargeString()
        {
            StringBuilder ans = new StringBuilder();
            ans.AppendFormat("{0} {1} {2},", Health, rightHand, RightReloadTimer.ElapsedMilliseconds);
            ans.Append(inventory.LargeString());
            return ans.ToString();
        }
        public string SmallString()
        {
            StringBuilder ans = new StringBuilder();
            ans.AppendFormat("{0} {1},", Health, rightHand);
            ans.Append(inventory.SmallString());
            return ans.ToString();
        }
    }
}
