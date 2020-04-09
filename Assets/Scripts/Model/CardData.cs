using System;

namespace deckmaster
{
    [Serializable]
    public class CardData
    {
        public int id;
        public OracleCard oracleCard;
    }

    [Flags]
    public enum Color
    {
        Colorless = (1 << 0),
        White = (1 << 1),
        Blue = (1 << 2),
        Black = (1 << 3),
        Red = (1 << 4),
        Green = (1 << 5)
    }

    public enum Category
    {
        Undefined,
        Land,
        Creature,
        Instant,
        Sorcery,
        Artifact,
        Enchantment,
        Planeswalker,
        Commander,
        Draw,
        Ramp,
        Interaction,
        Maybeboard,
        Sideboard
    }

    public enum CardTypes
    {
        Undefined,
        Land,
        Creature,
        Instant,
        Sorcery,
        Artifact,
        Enchantment,
        Planeswalker
    }

    public enum SubTypes
    {
        Undefined
    }

    [Flags]
    public enum SuperTypes
    {
        Undefined = (1 << 0),
        Basic = (1 << 1),
        Tribal = (1 << 2),
        Legendary = (1 << 3)
    }
}
