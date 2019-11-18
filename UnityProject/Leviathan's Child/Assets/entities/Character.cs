using System;

public class Character
{
    public Guid id { get; private set; }
    public string name { get; private set; }
    public Guid job { get; private set; }
    public Guid element { get; private set; }
    public Guid amulet { get; private set; }
    public int amuletLevel { get; private set; }
    public int amuletExperience { get; private set; }
    public float hp { get; private set; }
    public float strength { get; private set; }
    public float agility { get; private set; }
    public float intelligence { get; private set; }
    public int battlesNumber { get; private set; }
    public int victorysNumber { get; private set; }
    public int losesNumber { get; private set; }
    public int battleTimeInSeconds { get; private set; }
    public int xp { get; private set; }
    public int xpToUp { get; private set; }
    public int level { get; private set; }

    public Character(string id, string name, string job, string element, string amulet, int amuletLevel, int amuletExperience, float hp, float strength, float agility, float intelligence, int battlesNumber, int victorysNumber, int losesNumber, int battleTimeInSeconds, int xp, int xpToUp, int level)
    {
        this.id = Guid.Parse(id);
        this.name = name;
        this.job = Guid.Parse(job);
        this.element = Guid.Parse(element);
        this.amulet = Guid.Parse(amulet);
        this.amuletLevel = amuletLevel;
        this.amuletExperience = amuletExperience;
        this.hp = hp;
        this.strength = strength;
        this.agility = agility;
        this.intelligence = intelligence;
        this.battlesNumber = battlesNumber;
        this.victorysNumber = victorysNumber;
        this.losesNumber = losesNumber;
        this.battleTimeInSeconds = battleTimeInSeconds;
        this.xp = xp;
        this.xpToUp = xpToUp;
        this.level = level;
    }
}