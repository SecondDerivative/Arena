using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

namespace SFMLApp
{
	public abstract class Item
	{
		protected int Damage;
		protected int Range;
		protected string Name;
		abstract public int attack ();
		abstract public int attack (Inventory i);
		public int getRange() { return Range; }
		public void Create(int dmg, int ran, string name)
		{
			Damage = dmg;
			Range = ran;
			Name = name;
		}
	}
	class ItemSword : Item
	{
		public ItemSword(string n, int dmg, int ran)
		{
			base.Create (dmg, ran, n);
		}
		override public int attack(){return Damage;}
		override public int attack(Inventory i){return 0;}
	}
	class ItemBow : Item
	{
		public ItemBow(string n, int dmg, int ran){base.Create (dmg, ran, n);}
		override public int attack (Inventory i)
		{
			if (i.getArrows() > 0)
			{
				i.addArrows(-1);
				return Damage;
			}
			else { return 0; }
		}
		override public int attack(){ return 0;}
	}
	class Magic : Item
	{
		private int ManaCost;
		public Magic(string n,int dmg, int ran, int mana)
		{
			base.Create (dmg, ran, n);
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
	class Fist : Item{
		public Fist(){base.Create (2, 1, "Fist");}
		override public int attack(){return Damage;}
		override public int attack(Inventory i){return 0;}
	}

	public static class Items
	{
		public static List<Item> allItems;
		public static void getAllItems(){
			allItems = new List<Item>();
			allItems.Add (new Fist());
			StreamReader fileReader = new StreamReader("Weapons.txt");
			fileReader.ReadLine();
			for (int i = 0; i < 3; i++) {
				allItems.Add (new ItemSword (fileReader.ReadLine (), Int32.Parse (fileReader.ReadLine ()), Int32.Parse (fileReader.ReadLine ())));
			}
			fileReader.ReadLine ();
			for (int i = 0; i < 3; i++) {
				allItems.Add (new ItemBow (fileReader.ReadLine (), Int32.Parse (fileReader.ReadLine ()), Int32.Parse (fileReader.ReadLine ())));
			}
			fileReader.ReadLine ();
			for (int i = 0; i < 3; i++) {
				allItems.Add (new Magic (fileReader.ReadLine (), Int32.Parse (fileReader.ReadLine ()), Int32.Parse (fileReader.ReadLine ()), Int32.Parse(fileReader.ReadLine())));
			}
		}
	}
}

