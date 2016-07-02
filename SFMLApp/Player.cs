using System.Diagnostics;

namespace SFMLApp
{
	public class Player
	{
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
                    total = total + ((SFMLApp.Weapon)inventory.getItem(leftHand)).attack(inventory);
                else if (inventory.getItem(leftHand).GetType() == typeof(Magic))
                    total = total + ((SFMLApp.Weapon)inventory.getItem(leftHand)).attack(inventory);
                else 
                    total = total + ((SFMLApp.Weapon)inventory.getItem(leftHand)).attack();
            }
            if (RightReloadTimer.ElapsedMilliseconds >= ((Weapon)Items.allItems[rightHand]).Reloading)
            {
                RightReloadTimer.Restart();
                if (inventory.getItem(rightHand).GetType() == typeof(ItemBow))
                    total = total + ((SFMLApp.Weapon)inventory.getItem(leftHand)).attack(inventory);
                else if (rightHand.GetType() == typeof(Magic))
                        total = total + ((SFMLApp.Weapon)inventory.getItem(leftHand)).attack(inventory);
                else
                    total = total + ((SFMLApp.Weapon)inventory.getItem(leftHand)).attack();
            }
        	return total;
		}
		public void recieveDamage(int dmg){
			if (Health > 0 & Health < dmg)
				Health = 0;
			else
				Health = Health - dmg;
		}
		public bool isDead(){
			return Health > 0;
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
		public void pickUpArrow(Arrow arrowType,int nArrowsPickedUp){
			inventory.addArrows(arrowType, nArrowsPickedUp);
		}
		public void addedMana(int nManaAdded){
			inventory.addMana(nManaAdded);
		}
		public void pickedUpItem(Item i){
			inventory.addItem(i);
		}
		public void selectArrow(Arrow i) { inventory.setCurrentArrow(i.id); }
        public void pickedUpItem(int id)
        {
            inventory.addItem(id);
        }
        public void pickedUpItem(Item i, int cnt)
        {
            inventory.addItem(i, cnt);
        }
        public void pickedUpItem(int id, int cnt)
        {
            inventory.addItem(id, cnt);
        }
        public void respawn(){
			Health = 100;
			inventory.clearInventory ();
			rightHand = 0;
			leftHand = 0;
            LeftReloadTimer.Restart();
            RightReloadTimer.Restart();
        }
    }
}
