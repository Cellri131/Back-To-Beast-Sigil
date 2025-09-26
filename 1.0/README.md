# Back To Beast Sigils - Ambidextrous

Ce mod ajoute le sigil "Ambidextre" (Ambidextrous) à Inscryption.

## Description

Le sigil **Ambidextre** permet à la carte qui le possède de piocher 2 cartes lorsqu'elle est posée sur le plateau. C'est similaire à la bénédiction que l'on peut obtenir à la fin de l'Acte 1, mais uniquement quand la carte est jouée.

## Fonctionnalités

- **Nom du sigil** : Ambidextrous (Ambidextre)
- **Description** : "When [creature] is played, draw 2 cards."
- **Déclencheur** : Quand la carte est posée sur le plateau
- **Effet** : Pioche 2 cartes du deck
- **Icône** : Utilise l'image `boonicon_doubledraw.png` fournie

## Installation

1. Assurez-vous d'avoir BepInEx installé pour Inscryption
2. Installez l'API Inscryption (InscryptionAPI)
3. Placez ce dossier dans votre répertoire `BepInEx/plugins/`
4. Lancez le jeu

## Dépendances

- BepInEx 5.4.21+
- InscryptionAPI 2.20.1+

## Développement

Le sigil est créé en utilisant l'API Inscryption officielle et suit les bonnes pratiques de modding d'Inscryption.

### Structure du code

- `Plugin.cs` : Classe principale du plugin qui initialise le sigil
- `AmbidextrousAbility` : Classe qui définit le comportement du sigil
- Le sigil hérite de `AbilityBehaviour` et implémente :
  - `RespondsToResolveOnBoard()` : Se déclenche quand posé sur le plateau
  - `OnResolveOnBoard()` : Exécute l'effet de pioche de 2 cartes

### Niveau de puissance

Le sigil a un niveau de puissance de 4, ce qui reflète sa capacité utile de pioche de cartes.

## Version

1.0.0 - Version initiale avec le sigil Ambidextrous