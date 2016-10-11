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
    public abstract class Bottle : Item
    {
        public int restore { get; protected set; }
        public void Create(string n, int i, int r)
        {
            restore = r;
            base.Create(n, i);
        }
        public abstract void Consume(Inventory i);
        public abstract void Consume(Player p);
    }
    public class ManaBottle : Bottle
    {
        public override void Consume(Inventory i)
        {
            i.addMana(restore);
        }
        public ManaBottle(string n, int i, int r)
        {
            Create(n, i, r);
        }
        public override void Consume(Player p)
        {
            p.addedMana(restore);
        }
    }
    public class HPBottle : Bottle
    {
        public override void Consume(Player p)
        {
            p.HealHP(restore);
        }
        public override void Consume(Inventory ignored)
        {
        }
        public HPBottle(string n, int i, int r)
        {
            Create(n, i, r);
        }
    }
    public abstract class Weapon : Item
    {
        public int Damage { get; protected set; }
        public int Range { get; protected set; }
        public int Reloading { get; private set; }
        abstract public int attack();
        abstract public int attack(Inventory i);
        public void Create(int dmg, int ran, string name, int i, int RelodTime)
        {
            Damage = dmg;
            Range = ran;
            Reloading = RelodTime;
            base.Create(name, i);
        }
    }
    public class Arrow : Item
    {
        public int Damage { get; protected set; }
        private double Speed;
        public Arrow(string n, int d, int i, double speed)
        {
            Damage = d;
            Create(n, i);
            Speed = speed;
        }
        public double speed()
        {
            return Speed;
        }
    }
    class ItemSword : Weapon
    {
        public ItemSword(string n, int dmg, int ran, int id, int kd)
        {
            base.Create(dmg, ran, n, id, kd);
        }
        override public int attack() { return Damage; }
        override public int attack(Inventory i) { return Damage; }
    }
    class ItemBow : Weapon
    {
        public ItemBow(string n, int dmg, int ran, int id, int kd) { base.Create(dmg, ran, n, id, kd); }
        override public int attack(Inventory i)
        {
            if (i.getArrowsAmount() > 0)
            {
                i.addArrows(i.getCurrentArrow(), -1);
                return Damage + i.getCurrentArrow().Damage;
            }
            else { return 0; }
        }
        override public int attack() { return 0; }
    }
    class Magic : Weapon
    {
        private int ManaCost;
        private double Speed;

        public Magic(string n, int dmg, int ran, int mana, int id, int kd, double speed)
        {
            base.Create(dmg, ran, n, id, kd);
            ManaCost = mana;
            Speed = speed;
        }
        override public int attack(Inventory i)
        {
            if (i.getMana() >= ManaCost)
            {
                i.addMana(-ManaCost);
                return Damage;
            }
            else
            {
                return 0;
            }
        }
        override public int attack()
        {
            return 0;
        }
        public double speed()
        {
            return Speed;
        }
    }
    class Fist : Weapon
    {
        public Fist() { base.Create(2, 1, "Fist", 0, 1000); }
        override public int attack() { return Damage; }
        override public int attack(Inventory i) { return Damage; }
    }

    public static class Items
    {
        private static bool IsInit = false;
        private static List<Item> ArrayItems;
        public static List<Item> allItems
        {
            get
            {
                if (!IsInit)
                {
                    getAllItems();
                    IsInit = true;
                }
                return ArrayItems;
            }
            private set { ArrayItems = value; }
        }

        public static void getAllItems()
        {
            ArrayItems = new List<Item>();
            ArrayItems.Add(new Fist());
            int currentIndex = 1;
            StreamReader fileReader = new StreamReader("data/Weapons/Weapons.txt");
            fileReader.ReadLine();
            for (int i = currentIndex; i < currentIndex + 3; i++)
            {
                ArrayItems.Add(new ItemSword(fileReader.ReadLine(), Int32.Parse(fileReader.ReadLine()),
                    Int32.Parse(fileReader.ReadLine()), i, 1000));
            }
            currentIndex += 3;
            fileReader.ReadLine();
            for (int i = currentIndex; i < currentIndex + 3; i++)
            {
                ArrayItems.Add(new ItemBow(fileReader.ReadLine(), Int32.Parse(fileReader.ReadLine()),
                    Int32.Parse(fileReader.ReadLine()), i, int.Parse(fileReader.ReadLine())));
            }
            currentIndex += 3;
            fileReader.ReadLine();
            for (int i = currentIndex; i < currentIndex + 3; i++)
            {
                ArrayItems.Add(new Magic(fileReader.ReadLine(), Int32.Parse(fileReader.ReadLine()),
                    Int32.Parse(fileReader.ReadLine()), Int32.Parse(fileReader.ReadLine()), i,
                    int.Parse(fileReader.ReadLine()), double.Parse(fileReader.ReadLine())));
            }
            currentIndex += 3;
            fileReader.ReadLine();
            ArrayItems.Add(new Arrow("Wooden Arrow", 2, currentIndex, 0.5));
            currentIndex++;// = 1;
            for (int i = currentIndex; i < currentIndex + 2; i++)
            {
                ArrayItems.Add(new Arrow(fileReader.ReadLine(), Int32.Parse(fileReader.ReadLine()),
                    i, double.Parse(fileReader.ReadLine())));
            }
            fileReader.ReadLine();
            currentIndex += 2;
            for (int i = currentIndex; i < currentIndex + 1; i++)
            {
                ArrayItems.Add(new HPBottle(fileReader.ReadLine(), i, Int32.Parse(fileReader.ReadLine())));
            }
            currentIndex += 1;
            for (int i = currentIndex; i < currentIndex + 1; i++)
            {
                ArrayItems.Add(new ManaBottle(fileReader.ReadLine(), i, Int32.Parse(fileReader.ReadLine())));
            }
        }
    }
}