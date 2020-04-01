using System;

namespace deckmaster
{
    [Serializable]
    public class CardModel
    {

        public CardData card;
        public int quantity;
        public Category category;

    }
}

/*
 * {
            "card": {
                "id": 82765,
                "edition": {
                    "editioncode": "mb1",
                    "editionname": "Mystery Booster",
                    "mtgoCode": null
                },
                "artist": "Chase Stone",
                "rarity": "rare",
                "options": [
                    "Normal"
                ],
                "multiverseid": 0,
                "flavor": "",
                "prices": {
                    "ck": 27.99,
                    "ckfoil": 0,
                    "tcg": 26.35,
                    "tcgfoil": 0,
                    "mtgo": 0,
                    "mtgofoil": 0,
                    "cm": 29.99,
                    "cmfoil": 0
                },
                "uid": "2271862c-b1df-45f8-bb19-ae4b8ab0739e",
                "tcgProductId": 203369,
                "ckNormalId": 229714,
                "ckFoilId": 0,
                "mtgoNormalId": 0,
                "mtgoFoilId": 0,
                "oracleCard": {
                    "colors": [
                        "White"
                    ],
                    "colorIdentity": [
                        "White"
                    ],
                    "types": [
                        "Instant"
                    ],
                    "subTypes": [],
                    "superTypes": [],
                    "legalities": {
                        "penny": "not_legal",
                        "pauper": "not_legal",
                        "standard": "not_legal",
                        "modern": "not_legal",
                        "1v1": "legal",
                        "vintage": "legal",
                        "duel": "legal",
                        "legacy": "legal",
                        "commander": "legal",
                        "future": "not_legal",
                        "brawl": "not_legal",
                        "oldschool": "not_legal",
                        "oathbreaker": "legal",
                        "historic": "not_legal",
                        "pioneer": "not_legal"
                    },
                    "name": "Teferi's Protection",
                    "manaCost": "{2}{W}",
                    "cmc": 3,
                    "text": "Until your next turn, your life total can't change and you gain protection from everything. All permanents you control phase out. (While they're phased out, they're treated as though they don't exist. They phase in before you untap during your untap step.)\nExile Teferi's Protection.",
                    "power": "",
                    "toughness": "",
                    "layout": "normal",
                    "faces": [],
                    "manaProduction": {}
                },
                "owned": 0,
                "collectorNumber": "256",
                "games": [
                    1
                ],
                "cmEd": "mystery booster"
            },
            "quantity": 1,
            "modifier": "Normal",
            "category": "Interaction",
            "label": ",#656565"
        }
*/