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

    [Flags]
    public enum Category
    {
        Undefined = (1 << 0),
        Land = (1 << 1),
        Draw = (1 << 2),
        Ramp = (1 << 3),
        Interaction = (1 << 4),
        Tutor = (1 << 5),
        Boardwipe = (1 << 6),
        Wincon = (1 << 7),
        Creature = (1 << 8),
        Instant = (1 << 9),
        Sorcery = (1 << 10),
        Artifact = (1 << 11),
        Enchantment = (1 << 12),
        Planeswalker = (1 << 13),
        Commander = (1 << 14),
        Maybeboard = (1 << 15),
        Sideboard = (1 << 16)
    }

    [Flags]
    public enum CardTypes
    {
        Undefined = (1 << 0),
        Land = (1 << 1),
        Creature = (1 << 2),
        Instant = (1 << 3),
        Sorcery = (1 << 4),
        Artifact = (1 << 5),
        Enchantment = (1 << 6),
        Planeswalker = (1 << 7)
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
