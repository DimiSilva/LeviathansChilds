using System;

public class Job
{
    public Guid id { get; private set; }
    public string name { get; private set; }
    public string description { get; private set; }
    public int baseHp { get; private set; }
    public int baseStrength { get; private set; }
    public int baseAgility { get; private set; }
    public int baseIntelligence { get; private set; }
    public int baseXpToUp { get; private set; }
    public float xpToUpMultiplier { get; private set; }
    public float hpMultiplier { get; private set; }
    public float strengthMultiplier { get; private set; }
    public float agilityMultiplier { get; private set; }
    public float intelligenceMultiplier { get; private set; }
    public int levelToBlessing { get; private set; }

    public Job(string id, string name, string description, int baseHp, int baseStrength, int baseAgility, int baseIntelligence, int baseXpToUp, float xpToUpMultiplier, float hpMultiplier, float strengthMultiplier, float agilityMultiplier, float intelligenceMultiplier, int levelToBlessing)
    {
        this.id = Guid.Parse(id);
        this.name = name;
        this.description = description;
        this.baseHp = baseHp;
        this.baseStrength = baseStrength;
        this.baseAgility = baseAgility;
        this.baseIntelligence = baseIntelligence;
        this.baseXpToUp = baseXpToUp;
        this.xpToUpMultiplier = xpToUpMultiplier;
        this.hpMultiplier = hpMultiplier;
        this.strengthMultiplier = strengthMultiplier;
        this.agilityMultiplier = agilityMultiplier;
        this.intelligenceMultiplier = intelligenceMultiplier;
        this.levelToBlessing = levelToBlessing;
    }
}