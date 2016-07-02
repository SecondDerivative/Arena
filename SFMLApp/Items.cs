using System;
using System.Collections.Generic;
using System.IO;

namespace SFMLApp
{
	public abstract class Item
	{
		public string Name { get; protected set; }
		public int id { get; protected set; }
		public void Create(string n, int i)
		{
			Name = n;
			id = i;
		}
	}
	public abstract class Weapon : Item
	{
		public int Damage { get; protected set; }
		public int Range { get; protected set; }
        public int Reloading { get; private set; }
		abstract public int attack();
		abstract public int attack(Inventory i);
		public void Create(int dmg, int ran, string name, int i, int kd)
		{
			Damage = dmg;
			Range = ran;
            Reloading = kd;
			base.Create(name, i);
		}
	}
	public class Arrow : Item
	{
		public int Damage { get; protected set; }
		public Arrow(string n, int d, int i)
		{
			Damage = d;
			Create(n, i);
		}
	}
	class ItemSword : Weapon
	{
		public ItemSword(string n, int dmg, int ran, int id, int kd)
        {
			base.Create (dmg, ran, n, id, kd);
		}
		override public int attack(){return Damage;}
		override public int attack(Inventory i){return Damage;}
	}
	class ItemBow : Weapon
	{
		public ItemBow(string n, int dmg, int ran, int id, int kd) {base.Create (dmg, ran, n, id, kd);}
		override public int attack (Inventory i)
		{
			if (i.getArrowsAmount() > 0)
			{
				i.addArrows(i.getCurrentArrow(),-1);
				return Damage+i.getCurrentArrow().Damage;
			}
			else { return 0; }
		}
		override public int attack(){ return 0;}
	}
	class Magic : Weapon
	{
		private int ManaCost;
		public Magic(string n,int dmg, int ran, int mana, int id, int kd)
		{
			base.Create (dmg, ran, n, id, kd);
			ManaCost = mana;
		}
		override public int attack(Inventory i)
		{
			if (i.getMana() >= ManaCost)
			{
				i.addMana(-ManaCost);
				return Damage;
			}else
			{
				return 0;
			}
		}
		override public int attack(){return 0;}
	}
	class Fist : Weapon{
		public Fist() {base.Create (2, 1, "Fist", 0, 1000);}
		override public int attack(){return Damage;}
		override public int attack(Inventory i){return Damage;}
	}

	public static class Items
	{
		public static List<Item> allItems;
		public static List<Arrow> allArrows;
		public static void getAllItems(){
			allItems = new List<Item>();
			allItems.Add (new Fist());
			int currentIndex=1;
			StreamReader fileReader = new StreamReader("data/Weapons/Weapons.txt");
			fileReader.ReadLine();
			for (int i = currentIndex; i < currentIndex+3; i++) {
				allItems.Add (new ItemSword (fileReader.ReadLine (), Int32.Parse (fileReader.ReadLine ()), Int32.Parse (fileReader.ReadLine ()), i, 1000));
			}
			currentIndex += 3;
			fileReader.ReadLine ();
			for (int i = currentIndex; i < currentIndex + 3; i++) {
				allItems.Add (new ItemBow (fileReader.ReadLine (), Int32.Parse (fileReader.ReadLine ()), Int32.Parse (fileReader.ReadLine ()), i, 1000));
			}
			currentIndex += 3;
			fileReader.ReadLine ();
			for (int i = currentIndex; i < currentIndex + 3; i++) {
				allItems.Add (new Magic (fileReader.ReadLine (), Int32.Parse (fileReader.ReadLine ()), Int32.Parse (fileReader.ReadLine ()), Int32.Parse(fileReader.ReadLine()), i, 1000));
			}
			currentIndex += 3;
			fileReader.ReadLine();
			allArrows = new List<Arrow>();
			allArrows.Add(new Arrow("Wooden Arrow",2, 0));
			currentIndex = 1;
			for (int i = currentIndex; i < currentIndex + 2; i++)
			{
				allArrows.Add(new Arrow(fileReader.ReadLine(), Int32.Parse(fileReader.ReadLine()), i));
			}
		}
	}
}