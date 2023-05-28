---
description: Dokumentace aktivity stavění z cihel
---

# 💡 BrickBuilder - aktivita

<figure><img src=".gitbook/assets/image (2).png" alt=""><figcaption></figcaption></figure>

## 1. Stručný přehled návrhu

Tato aktivita využívá zabudovaných funkcí knihovna XR Interaction toolkit. XR Ray interactor na ovladači umí vzít do ruky cihlu s componentem XR Grab Interactable. Tím cihla přejde do vlastnictví XR Direct Interactoru, který jí poutá k ruce uživatele. Pokud se uživatel s cihlou přiblíží k místu, kam lze umístit, uktivuje se XR Socket Interactor, který po zobrazení poloprůhledného obrysu může cihlu přijmout na své místo po puštění z vlastnictví XR Direct Interactoru.&#x20;

## 2. Diskuze

Bylo by dobré do budoucna dodat zvuky a přidat další situace (roh, dveře)
