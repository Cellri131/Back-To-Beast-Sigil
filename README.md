# Back To Beast Sigils

Un mod Inscryption qui ajoute quatre nouveaux sigils puissants pour enrichir votre expérience de jeu.

## 📦 Installation

1. Assurez-vous d'avoir **BepInEx** et **InscryptionAPI** installés
2. Téléchargez la dernière version depuis les [Releases](https://github.com/Cellri131/Back-To-Beast-Sigil/releases)
3. Extrayez le contenu dans votre dossier `BepInEx/plugins/`
4. Lancez Inscryption !

## ⚡ Sigils Inclus

### 🦾 Hand of the Beast
- **ID**: `back.to.beast.sigils.HandOfBeast`
- **Description**: Au début de votre tour, si cette carte est en vie, piochez une carte supplémentaire.
- **Utilisation**: Parfait pour maintenir une main pleine de cartes
- **Niveau de puissance**: 4

### 🌿 Hand of Nature  
- **ID**: `back.to.beast.sigils.HandOfNature`
- **Description**: Au début de votre tour, si cette carte est en vie, obtenez un écureuil dans votre main.
- **Utilisation**: Source constante d'écureuils pour les sacrifices
- **Niveau de puissance**: 3

### 🎲 Strange Evolution
- **ID**: `back.to.beast.sigils.StrangeEvolution` 
- **Description**: À la fin du tour, évolue aléatoirement si deux formes sont définies, sinon gagne +2 ATK ou +4 PV. Une seule fois par combat.
- **Utilisation**: Évolution imprévisible ou amélioration des statistiques
- **Niveau de puissance**: 3

### 🏛️ Tribe Life
- **ID**: `back.to.beast.sigils.TribeLife`
- **Description**: Quand cette carte meurt, une carte de la même tribu prend sa place sur le plateau.
- **Utilisation**: Continuité tribale et remplacement automatique
- **Niveau de puissance**: 4

## 🎯 Utilisation dans vos cartes

### Utilisation basique
```json
{
  "name": "MaCarte",
  "abilities": ["back.to.beast.sigils.HandOfBeast"]
}
```

### Strange Evolution avec évolutions personnalisées
```json
{
  "name": "IGCC_MysteriousEgg",
  "description": "Un œuf étrange [evolution1:IGCC_Dragon][evolution2:IGCC_Phoenix] qui éclot différemment.",
  "abilities": ["back.to.beast.sigils.StrangeEvolution"],
  "baseAttack": 0,
  "baseHealth": 2
}
```

### Strange Evolution sans évolutions (mode buff)
```json
{
  "name": "IGCC_WildBeast", 
  "description": "Une créature sauvage qui s'adapte au combat.",
  "abilities": ["back.to.beast.sigils.StrangeEvolution"],
  "baseAttack": 2,
  "baseHealth": 2
}
```

### Tribe Life - Continuité tribale
```json
{
  "name": "IGCC_AlphaWolf",
  "description": "Le chef de meute veille sur sa famille.",
  "abilities": ["back.to.beast.sigils.TribeLife"],
  "tribes": ["Canine"],
  "baseAttack": 3,
  "baseHealth": 2
}
```

## 🔧 Fonctionnalités techniques

### Strange Evolution - Modes de fonctionnement

**Mode Évolution** (si `[evolution1:Carte1][evolution2:Carte2]` dans la description):
- 50% de chance d'évoluer vers `Carte1`
- 50% de chance d'évoluer vers `Carte2`

**Mode Buff** (si pas d'évolutions définies):
- 50% de chance de gagner +2 Attaque
- 50% de chance de gagner +4 Vie

**Limitations**:
- Ne s'active qu'une seule fois par combat par carte
- Se déclenche à la fin de votre tour
- Reset quand la carte est rejouée

### Tribe Life - Système de remplacement

**Ordre de priorité pour le remplacement**:
1. **Deck du joueur** → Cherche une carte de la même tribu dans votre deck
2. **Cartes du jeu** → Si aucune dans le deck, prend une carte aléatoire de la même tribu du jeu de base
3. **Sans tribu** → Si la carte morte n'a pas de tribu, cherche une autre carte sans tribu

**Mécaniques**:
- Se déclenche avant l'animation de mort
- Placement automatique dans le même slot
- Compatible avec toutes les tribus (Canine, Bird, Insect, etc.)
- Filtre les cartes problématiques ou spéciales

## 🛠️ Développement

### Prérequis
- .NET SDK
- Visual Studio Code ou Visual Studio
- Inscryption + BepInEx + InscryptionAPI

### Compilation
```bash
cd "1.0"
dotnet build
```

### Structure du projet
```
1.0/
├── Plugin.cs              # Code principal du mod
├── BackToBeastSigils.csproj # Configuration du projet
├── manifest.json          # Métadonnées Thunderstore
├── README.md              # Documentation
├── *.png                  # Images des sigils
└── bin/                   # Fichiers compilés
```

## 📋 Dépendances

- **BepInEx**: 5.4.2100+
- **InscryptionAPI**: 2.20.1+

## 🤝 Contribution

Les contributions sont les bienvenues ! N'hésitez pas à :
- Ouvrir des issues pour signaler des bugs
- Proposer de nouvelles fonctionnalités
- Soumettre des pull requests

## 📄 Licence

Ce projet est sous licence MIT. Voir le fichier [LICENSE](LICENSE) pour plus de détails.

## 🙏 Remerciements

- L'équipe d'**InscryptionAPI** pour leurs outils exceptionnels
- La communauté **Inscryption modding** pour leur support
- **Daniel Mullins Games** pour le jeu original

---

*Créé avec ❤️ pour la communauté Inscryption*
