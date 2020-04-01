using System;
using UnityEngine;
using UnityEditor;

namespace deckmaster
{
    [Serializable]
    public class CardData
    {
        public int id;
        public OracleCard oracleCard;
    }

    public enum Color
    {
        Colorless,
        White,
        Blue,
        Black,
        Red,
        Green
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
}
