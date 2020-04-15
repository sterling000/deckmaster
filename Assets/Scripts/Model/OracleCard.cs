using deckmaster;
using System;
using System.Runtime.Serialization;
using Color = deckmaster.Color;

[Serializable]
public class OracleCard
{
    public string[] colorIdentity;
    public string name;
    public string[] superTypes;
    public string[] types;

    [IgnoreDataMember]
    public Color ColorIdentity
    {
        get
        {
            Color result = Color.Colorless;
            foreach (string colorString in colorIdentity)
            {
                Color temp;
                if (Enum.TryParse(colorString, out temp))
                {
                    result |= temp;
                    result &= ~Color.Colorless;
                }

            }

            return result;
        }
    }

    [IgnoreDataMember]
    public SuperTypes SuperTypes
    {
        get
        {
            SuperTypes result = SuperTypes.Undefined;
            foreach (string superType in superTypes)
            {
                SuperTypes temp;
                if (Enum.TryParse(superType, out temp))
                {
                    result |= temp;
                    result &= ~SuperTypes.Undefined;
                }
            }

            return result;
        }
    }

    [IgnoreDataMember]
    public CardTypes CardTypes
    {
        get
        {
            CardTypes result = CardTypes.Undefined;
            foreach (string cardType in types)
            {
                CardTypes temp;
                if (Enum.TryParse(cardType, out temp))
                {
                    result |= temp;
                    result &= ~CardTypes.Undefined;
                }
            }

            return result;
        }
    }
}
