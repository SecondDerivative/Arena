using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFMLApp
{
	class Player
	{
		private int Health;
		private Inventory inventory;
		private int leftHand;
		private int rightHand;
		public Player()
		{
			inventory = new Inventory();
			Health = 100;
			leftHand = 0;
			rightHand = 0;
		}
		public int attack(){
			int total=0;
			if (inventory.getItem(leftHand).GetType() == typeof(ItemBow)) {
				total = total + inventory.getItem(leftHand).attack(inventory);
			} else {
				if (inventory.getItem(leftHand).GetType () == typeof(Magic)) {
					total = total + inventory.getItem(leftHand).attack (inventory);
				} else {
					total = total + inventory.getItem(leftHand).attack ();
				}
			}
			if (inventory.getItem(rightHand).GetType () == typeof(ItemBow)) {
				total = total + inventory.getItem(rightHand).attack (inventory);
			} else {
				if (rightHand.GetType () == typeof(Magic)) {
					total = total + inventory.getItem(rightHand).attack (inventory);
				} else {
					total = total + inventory.getItem(rightHand).attack ();
				}
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
			for (int j = 0; j < Items.allItems.Capacity; j++) {
				if (Items.allItems [j].Equals (i)) {
					leftHand = j;
					break;
				}
			}
		}
		public void takeItemRight(Item i){
			for (int j = 0; j < Items.allItems.Capacity; j++) {
				if (Items.allItems[j].Equals (i)) {
					leftHand = j;
					break;
				}
			}
		}
		public void pickUpArrow(int nArrowsPickedUp){
			inventory.addArrows(nArrowsPickedUp);
		}
		public void addedMana(int nManaAdded){
			inventory.addMana(nManaAdded);
		}
		public Item getItemLeft(){
			return inventory.getItem(leftHand);
		}
		public Item getItemRight(){
			return inventory.getItem(rightHand);
		}
	}
}
