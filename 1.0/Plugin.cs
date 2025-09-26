using BepInEx;
using BepInEx.Logging;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using InscryptionAPI.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BackToBeastSigils
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("cyantist.inscryption.api", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "back.to.beast.sigils";
        private const string PluginName = "Back To Beast Sigils";
        private const string PluginVersion = "1.0.0";

        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;

            Log.LogInfo($"Plugin {PluginName} is loaded!");

            // Ajouter les sigils Hand of the Beast et Hand of Nature
            AddHandSigils();
        }

        private void AddHandSigils()
        {
            // Créer le sigil Hand of the Beast
            AbilityInfo handOfBeastInfo = AbilityManager.New(
                PluginGuid,
                "HandOfBeast", 
                "Au début de votre tour, si cette carte est en vie, piochez une carte supplémentaire.",
                typeof(HandOfBeastAbility),
                TextureHelper.GetImageAsTexture("hand_of_the_beast.png")
            )
            .SetDefaultPart1Ability()
            .SetCanStack(false);

            handOfBeastInfo.powerLevel = 4;
            HandOfBeastAbility.ability = handOfBeastInfo.ability;

            Log.LogInfo("Hand of the Beast sigil has been added!");

            // Créer le sigil Hand of Nature
            AbilityInfo handOfNatureInfo = AbilityManager.New(
                PluginGuid,
                "HandOfNature", 
                "Au début de votre tour, si cette carte est en vie, obtenez un écureuil dans votre main.",
                typeof(HandOfNatureAbility),
                TextureHelper.GetImageAsTexture("hand_of_nature.png")
            )
            .SetDefaultPart1Ability()
            .SetCanStack(false);

            handOfNatureInfo.powerLevel = 3;
            HandOfNatureAbility.ability = handOfNatureInfo.ability;

            Log.LogInfo("Hand of Nature sigil has been added!");

            // Créer le sigil Strange Evolution
            AbilityInfo strangeEvolutionInfo = AbilityManager.New(
                PluginGuid,
                "StrangeEvolution", 
                "À la fin du tour, évolue aléatoirement si deux formes sont définies, sinon gagne +2 ATK ou +4 PV. Une seule fois par combat.",
                typeof(StrangeEvolutionAbility),
                TextureHelper.GetImageAsTexture("strange_evolution.png")
            )
            .SetDefaultPart1Ability()
            .SetCanStack(false);

            strangeEvolutionInfo.powerLevel = 3;
            StrangeEvolutionAbility.ability = strangeEvolutionInfo.ability;

            Log.LogInfo("Strange Evolution sigil has been added!");

            // Créer le sigil Tribe Life
            AbilityInfo tribeLifeInfo = AbilityManager.New(
                PluginGuid,
                "TribeLife", 
                "Quand cette carte meurt, une carte de la même tribu prend sa place sur le plateau.",
                typeof(TribeLifeAbility),
                TextureHelper.GetImageAsTexture("tribe_life.png")
            )
            .SetDefaultPart1Ability()
            .SetCanStack(false);

            tribeLifeInfo.powerLevel = 4;
            TribeLifeAbility.ability = tribeLifeInfo.ability;

            Log.LogInfo("Tribe Life sigil has been added!");
        }
    }

    // Classe qui définit le comportement du sigil Hand of the Beast
    public class HandOfBeastAbility : AbilityBehaviour
    {
        public static Ability ability;

        public override Ability Ability
        {
            get
            {
                return ability;
            }
        }

        // Se déclenche au début de chaque tour
        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            // Se déclenche seulement au début du tour du joueur si la carte est sur le plateau du joueur
            return playerUpkeep && base.Card.OnBoard && !base.Card.OpponentCard && !base.Card.Dead;
        }

        // L'effet : piocher une carte supplémentaire du deck principal
        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            yield return base.PreSuccessfulTriggerSequence();

            Plugin.Log.LogInfo("Hand of the Beast: Triggering extra draw!");

            // Attendre un peu pour que la pioche normale se termine
            yield return new WaitForSeconds(0.5f);

            // Piocher une carte directement du deck
            if (Singleton<CardDrawPiles>.Instance.Deck.Cards.Count > 0)
            {
                Plugin.Log.LogInfo("Drawing extra card for Hand of the Beast!");
                yield return Singleton<CardDrawPiles>.Instance.DrawCardFromDeck();
            }

            yield return base.LearnAbility(0.1f);
            yield break;
        }
    }

    // Classe qui définit le comportement du sigil Hand of Nature
    public class HandOfNatureAbility : AbilityBehaviour
    {
        public static Ability ability;

        public override Ability Ability
        {
            get
            {
                return ability;
            }
        }

        // Se déclenche au début de chaque tour
        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            // Se déclenche seulement au début du tour du joueur si la carte est sur le plateau du joueur
            return playerUpkeep && base.Card.OnBoard && !base.Card.OpponentCard && !base.Card.Dead;
        }

        // L'effet : obtenir un écureuil dans la main
        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            yield return base.PreSuccessfulTriggerSequence();

            Plugin.Log.LogInfo("Hand of Nature: Adding squirrel to hand!");

            // Attendre un peu pour que la pioche normale se termine
            yield return new WaitForSeconds(0.3f);

            // Créer un écureuil directement (ou utiliser la carte de la pile d'écureuil si modifiée)
            CardInfo squirrelCard = CardLoader.GetCardByName("Squirrel");
            if (squirrelCard != null)
            {
                Plugin.Log.LogInfo("Creating squirrel card for Hand of Nature!");
                PlayableCard playableSquirrel = CardSpawner.SpawnPlayableCard(squirrelCard);
                yield return Singleton<PlayerHand>.Instance.AddCardToHand(playableSquirrel, new Vector3(0f, 0f, 0f), 0f);
            }

            yield return base.LearnAbility(0.1f);
            yield break;
        }
    }

    // Classe qui définit le comportement du sigil Strange Evolution
    public class StrangeEvolutionAbility : AbilityBehaviour
    {
        public static Ability ability;
        private bool hasActivated = false; // Pour s'assurer qu'il ne s'active qu'une fois

        public override Ability Ability
        {
            get
            {
                return ability;
            }
        }

        // Se déclenche à la fin du tour (comme l'évolution normale)
        public override bool RespondsToTurnEnd(bool playerTurnEnd)
        {
            // Se déclenche seulement à la fin du tour du joueur si la carte est sur le plateau et n'a pas encore été activé
            return playerTurnEnd && base.Card.OnBoard && !base.Card.OpponentCard && !base.Card.Dead && !hasActivated;
        }

        // L'effet : évolution aléatoire ou buff stats
        public override IEnumerator OnTurnEnd(bool playerTurnEnd)
        {
            yield return base.PreSuccessfulTriggerSequence();

            Plugin.Log.LogInfo("Strange Evolution: Triggering effect!");
            hasActivated = true; // Marquer comme activé

            string evolution1 = null;
            string evolution2 = null;

            // Parser les évolutions depuis la description
            string description = base.Card.Info.description;
            
            if (!string.IsNullOrEmpty(description))
            {
                System.Text.RegularExpressions.Regex regex1 = new System.Text.RegularExpressions.Regex(@"\[evolution1:([^\]]+)\]");
                System.Text.RegularExpressions.Regex regex2 = new System.Text.RegularExpressions.Regex(@"\[evolution2:([^\]]+)\]");
                
                var match1 = regex1.Match(description);
                var match2 = regex2.Match(description);
                
                if (match1.Success) evolution1 = match1.Groups[1].Value;
                if (match2.Success) evolution2 = match2.Groups[1].Value;
                
                Plugin.Log.LogInfo($"Strange Evolution: Parsed from description - evolution1: {evolution1}, evolution2: {evolution2}");
            }

            // Vérifier si on a les deux évolutions définies
            if (!string.IsNullOrEmpty(evolution1) && !string.IsNullOrEmpty(evolution2))
            {
                // Mode évolution : choisir entre les deux cartes
                Plugin.Log.LogInfo("Strange Evolution: Evolution mode activated!");
                
                bool evolveToFirst = UnityEngine.Random.Range(0f, 1f) < 0.5f;
                string chosenEvolution = evolveToFirst ? evolution1 : evolution2;
                
                Plugin.Log.LogInfo($"Strange Evolution: Chosen evolution {chosenEvolution}");

                CardInfo evolutionCard = CardLoader.GetCardByName(chosenEvolution);
                
                if (evolutionCard != null)
                {
                    Plugin.Log.LogInfo($"Strange Evolution: Evolving into {evolutionCard.name}");

                    CardSlot currentSlot = base.Card.Slot;
                    yield return base.Card.Die(false, null, true);
                    yield return Singleton<BoardManager>.Instance.CreateCardInSlot(evolutionCard, currentSlot, 0.15f, true);

                    Plugin.Log.LogInfo("Strange Evolution: Evolution completed!");
                }
                else
                {
                    Plugin.Log.LogError($"Strange Evolution: Evolution card '{chosenEvolution}' not found!");
                }
            }
            else
            {
                // Mode buff : 50% +2 attaque ou 50% +4 vie
                Plugin.Log.LogInfo("Strange Evolution: Buff mode activated (no valid evolutions found)!");
                
                bool buffAttack = UnityEngine.Random.Range(0f, 1f) < 0.5f;
                
                if (buffAttack)
                {
                    Plugin.Log.LogInfo("Strange Evolution: Adding +2 attack!");
                    base.Card.AddTemporaryMod(new CardModificationInfo(2, 0));
                    
                    // Effet visuel pour le buff d'attaque
                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    Plugin.Log.LogInfo("Strange Evolution: Adding +4 health!");
                    base.Card.AddTemporaryMod(new CardModificationInfo(0, 4));
                    
                    // Effet visuel pour le buff de vie
                    yield return new WaitForSeconds(0.2f);
                }
                
                Plugin.Log.LogInfo("Strange Evolution: Buff applied!");
            }

            yield return base.LearnAbility(0.1f);
            yield break;
        }

        // Reset le flag quand la carte est jouée
        public override bool RespondsToResolveOnBoard()
        {
            return true;
        }

        public override IEnumerator OnResolveOnBoard()
        {
            // Reset le flag quand la carte est jouée pour la première fois
            if (hasActivated)
            {
                hasActivated = false;
                Plugin.Log.LogInfo("Strange Evolution: Reset activation flag on board placement!");
            }
            yield break;
        }
    }

    // Classe qui définit le comportement du sigil Tribe Life
    public class TribeLifeAbility : AbilityBehaviour
    {
        public static Ability ability;

        public override Ability Ability
        {
            get
            {
                return ability;
            }
        }

        // Se déclenche quand la carte meurt
        public override bool RespondsToPreDeathAnimation(bool wasSacrifice)
        {
            return base.Card.OnBoard && !base.Card.OpponentCard;
        }

        // L'effet : remplacer par une carte de la même tribu
        public override IEnumerator OnPreDeathAnimation(bool wasSacrifice)
        {
            yield return base.PreSuccessfulTriggerSequence();

            Plugin.Log.LogInfo("Tribe Life: Card is dying, looking for replacement!");

            // Sauvegarder le slot actuel avant que la carte meure
            CardSlot currentSlot = base.Card.Slot;
            
            // Obtenir la tribu de la carte mourante
            Tribe cardTribe = base.Card.Info.tribes.Count > 0 ? base.Card.Info.tribes[0] : Tribe.None;
            
            Plugin.Log.LogInfo($"Tribe Life: Card tribe is {cardTribe}");

            // Attendre que l'animation de mort commence
            yield return new WaitForSeconds(0.2f);

            // Chercher une carte de remplacement
            CardInfo replacementCard = FindReplacementCard(cardTribe);

            if (replacementCard != null)
            {
                Plugin.Log.LogInfo($"Tribe Life: Found replacement card {replacementCard.name}");

                // Attendre un peu puis placer la nouvelle carte
                yield return new WaitForSeconds(0.5f);
                
                // Créer la nouvelle carte dans le slot
                yield return Singleton<BoardManager>.Instance.CreateCardInSlot(replacementCard, currentSlot, 0.15f, true);
                
                Plugin.Log.LogInfo("Tribe Life: Replacement card placed successfully!");
            }
            else
            {
                Plugin.Log.LogWarning("Tribe Life: No replacement card found!");
            }

            yield return base.LearnAbility(0.1f);
            yield break;
        }

        private CardInfo FindReplacementCard(Tribe tribe)
        {
            Plugin.Log.LogInfo($"Tribe Life: Searching for replacement with tribe {tribe}");

            // Méthode 1 : Chercher dans le deck du joueur (et retirer la carte du deck)
            CardInfo deckCard = FindCardInDeck(tribe);
            if (deckCard != null)
            {
                Plugin.Log.LogInfo($"Tribe Life: Found card in deck: {deckCard.name} (removed from deck to avoid duplicates)");
                return deckCard;
            }

            // Méthode 2 : Chercher dans toutes les cartes du jeu (pas besoin de retirer)
            CardInfo gameCard = FindCardInAllCards(tribe);
            if (gameCard != null)
            {
                Plugin.Log.LogInfo($"Tribe Life: Found card in game collection: {gameCard.name} (no deck removal needed)");
                return gameCard;
            }

            Plugin.Log.LogWarning("Tribe Life: No replacement card found anywhere!");
            return null;
        }

        private CardInfo FindCardInDeck(Tribe tribe)
        {
            var deck = Singleton<CardDrawPiles>.Instance.Deck;
            if (deck != null && deck.Cards != null && deck.Cards.Count > 0)
            {
                // Créer une liste des cartes à parcourir pour éviter la modification pendant l'itération
                var deckCards = new System.Collections.Generic.List<CardInfo>(deck.Cards);
                
                foreach (var card in deckCards)
                {
                    if (tribe == Tribe.None)
                    {
                        // Pour les cartes sans tribu, chercher d'autres cartes sans tribu
                        if (card.tribes == null || card.tribes.Count == 0)
                        {
                            // IMPORTANT: Retirer la carte du deck pour éviter les doublons
                            deck.Cards.Remove(card);
                            Plugin.Log.LogInfo($"Tribe Life: Removed {card.name} from deck to avoid duplicates");
                            return card;
                        }
                    }
                    else
                    {
                        // Pour les cartes avec tribu, chercher la même tribu
                        if (card.tribes != null && card.tribes.Contains(tribe))
                        {
                            // IMPORTANT: Retirer la carte du deck pour éviter les doublons
                            deck.Cards.Remove(card);
                            Plugin.Log.LogInfo($"Tribe Life: Removed {card.name} from deck to avoid duplicates");
                            return card;
                        }
                    }
                }
            }
            return null;
        }

        private CardInfo FindCardInAllCards(Tribe tribe)
        {
            // Obtenir toutes les cartes disponibles dans le jeu
            var allCards = CardLoader.AllData;
            var validCards = new System.Collections.Generic.List<CardInfo>();

            foreach (var card in allCards)
            {
                // Ignorer les cartes spéciales, rares ou problématiques
                if (card == null || string.IsNullOrEmpty(card.name) || 
                    card.name.Contains("Empty") || card.name.Contains("Rare") ||
                    card.name.Contains("!") || card.name.Contains("CHOICE"))
                {
                    continue;
                }

                if (tribe == Tribe.None)
                {
                    // Chercher des cartes sans tribu
                    if (card.tribes == null || card.tribes.Count == 0)
                    {
                        validCards.Add(card);
                    }
                }
                else
                {
                    // Chercher des cartes de la même tribu
                    if (card.tribes != null && card.tribes.Contains(tribe))
                    {
                        validCards.Add(card);
                    }
                }
            }

            // Retourner une carte aléatoire parmi les valides
            if (validCards.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, validCards.Count);
                return validCards[randomIndex];
            }

            return null;
        }
    }
}