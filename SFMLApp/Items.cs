using System;
using System.Collections.Generic;
using System.IO;

namespace SFMLApp
{
	public abstract class Item {
		public int Damage { get; protected set; }
		public int Range { get; protected set; }
		public string Name { get; protected set; }
		public int id { get; protected set; }

		abstract public int attack ();
		abstract public int attack (Inventory i);

		public int getRange() {
			return Range;
		}

		public void Create(int dmg, int ran, string name, int i)
		{
			Damage = dmg;
			Range = ran;
			Name = name;
			id = i;
		}
	}

	class ItemSword : Item {
		public ItemSword(string n, int dmg, int ran, int id)
		{
			base.Create (dmg, ran, n, id);
		}

		override public int attack() {
			return Damage;
		}

		override public int attack(Inventory i) {
			return 0;
		}
	}

	class ItemBow : Item {
		public ItemBow(string n, int dmg, int ran, int id) {
			base.Create (dmg, ran, n, id);
		}

		override public int attack (Inventory i) {
			if (i.getArrows() > 0) {
				i.addArrows(-1);
				return Damage;
			} else {
				return 0;
			}
		}

		override public int attack() {
			return 0;
		}
	}

	class Magic : Item {
		private int ManaCost;

		public Magic(string n,int dmg, int ran, int mana, int id) {
			base.Create (dmg, ran, n, id);
			ManaCost = mana;
		}

		override public int attack(Inventory i)	{
			if (i.getMana() >= ManaCost) {
				i.addMana(-ManaCost);
				return Damage;
			} else {
				return 0;
			}
		}

		override public int attack(){return 0;}
	}

	class Fist : Item {

		public Fist() {
			base.Create (2, 1, "Fist", 0);
		}

		override public int attack() {
			return Damage;
		}

		override public int attack(Inventory i) {
			return 0;
		}
	}

	public static class Items {

		public static List<Item> allItems;

		public static void getAllItems(){
			allItems = new List<Item>();
			allItems.Add (new Fist());
			StreamReader fileReader = new StreamReader("Weapons.txt");
			fileReader.ReadLine();
			for (int i = 1; i < 4; i++) {
				allItems.Add (new ItemSword (fileReader.ReadLine (), Int32.Parse (fileReader.ReadLine ()), Int32.Parse (fileReader.ReadLine ()), i));
			}
			fileReader.ReadLine ();
			for (int i = 4; i < 7; i++) {
				allItems.Add (new ItemBow (fileReader.ReadLine (), Int32.Parse (fileReader.ReadLine ()), Int32.Parse (fileReader.ReadLine ()), i));
			}
			fileReader.ReadLine ();
			for (int i = 7; i < 10; i++) {
				allItems.Add (new Magic (fileReader.ReadLine (), Int32.Parse (fileReader.ReadLine ()), Int32.Parse (fileReader.ReadLine ()), Int32.Parse(fileReader.ReadLine()), i));
			}
		}
	}
}
