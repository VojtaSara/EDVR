---
description: Dokumentace aktivity Data Labeling
---

# 🏷 Data Labeling - aktivita

<figure><img src=".gitbook/assets/image (1).png" alt=""><figcaption></figcaption></figure>

## 1. Stručný přehled návrhu

V základní verzi se tato aktivita skládá ze tří částí:

* LabelBrush připojený k ovladači - Assets/Scripts/LabelScripts/LabelBrush.cs
* LabelDatabase držící stav vytvořených labelů - Assets/Scripts/LabelScripts/LabelDatabase.cs
* ObjLoader načítající .obj soubory za běhu - Assets/OBJImport/OBJLoader.cs

LabelBrush sleduje oba ovladače a z matice pozice a kvaternionu rotace určuje, kde by se měla nacházet vyznačující krychle. Levý ovladač určuje rotaci, pravý pak pouze jeden roh krychle. Z těchto dvou parametrů lze jednoznačně určit pozici vyznačující krychle. Funkce RenderBox pak předá tyto informace MeshRendereru, který správně vykreslí vyznačující krychli. Vedle toho LabelBrush spravuje paprsky vycházející z ovladačů, které umožňují prodloužit jejich dosah.&#x20;

LabelDatabase obsahuje jednoduché pole GameObjectů typu Label, každý label je krychle umístěná obecně v prostoru společně s popiskem. Vykreslení Labelu probíhá pomocí LineRendereru. Aby popisek vizuálně nepřekážel, tak je zobrazen pouze pomocí wireframu, který je vytvořen jako lomená čára, formát, který využívá vysoce zoptimalizovaného LineRendereru v Unity, i po přidání obrovského množství labelů by se běh neměl zpomalit.&#x20;

ObjLoader načítá modely, ve kterých bude labelováno. Z předem definované složky načte všechny OBJ soubory.

## 2. Diskuze

Tato aktivita bude do budoucna významně rozšířena - bude předmětem bakalářské práce. LabelDatabase bude umět ukládat svůj obsah do souboru. OBJloader bude napojený na přívětivé UI přes které mohu jednotlivými modely snadno brouzdat. Přibyde funkcionalita na manipulaci s modelem - pro maximální kompatibilitu i s usazeným uživatelem bude možné pohybovat s modelem a hotovými labely. Vedle této vizuální transformace bude zachován globální souřadnicový systém, ve kterém se labely logicky pohybovat nebudou. Tato možnost bude podpořena UI elementem pomáhajícím uživateli zorientovat se.
