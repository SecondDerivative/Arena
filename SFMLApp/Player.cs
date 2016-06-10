using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFMLApp
{
    class Player
    {
        private int Health;
        //private int Mana;
        private Inventory inventory;
		private Item leftHand;
        private Item rightHand;
        public Player()
        {
            inventory = new Inventory();
            Health = 100;
            //Mana = 100;
			leftHand = new Fist ();
			rightHand = new Fist ();
        }
		public int attack(){
			int total = rightHand.attack();
			total = total + leftHand.attack();
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
		public void takeItemLeft(Item item){
			leftHand = item;
		}
		public void takeItemRight(Item item){
			rightHand = item;
		}
	}
}
