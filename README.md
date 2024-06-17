# AI-Powered VR Project: Zombie Defense

## Inleiding

### Zombie defense: a zombie shooter VR application

Zombie defense is een game met als doel zombies neer te schieten. De speler verliest als de zombies hem bereiken. Het spel maakt gebruik van machine learning om de zombies naar de speler te laten gaan. Ze moeten de snelste weg zoeken en obstakels ontwijken.

### Overzicht

Deze tutorial bespreekt de methoden en resultaten.

In de methoden wordt besproken wat je moet installeren, hoe het spel verloopt, welke observaties, acties en beloningen de zombie krijgt en wordt er informatie over de objecten gegeven.

Hierbij zullen we de fouten waaruit we geleerd hebben bespreken, en waarbij we ook hun oplossing bespreken.

## Methoden

### Installatie

- Unity versie: 2022.3.\*
- Anaconda (conda): 24.1.2
- In de anaconda environment:
  - Python versie: 3.9.\*
  - ML-Agents Toolkit versie: 0.30.0
  - PyTorch versie: 1.7.1

### Reproduceren project

Voor het opstarten en downloaden van het project moet je het volgende doen:  
**(!!!!Emir, gelieve dit in een codeblock te steken!!!)**

git clone <https://github.com/AP-IT-GH/eindproject-VR-experience.git>

Trainen kan je doen in het RoanEnvTest scene, de game kan men spelen in de GAME scene.

### Verloop van de simulatie

1. Zombies worden aan het begin van de omgeving gespawned.
2. Aan het einde van deze omgeving bevindt zich de speler.
3. In het begin van het spel moet de speler een wapen van een tafel oppakken om de zombies te kunnen beschieten.
4. De zombies moeten naar de speler toe lopen.
5. Op hun weg naar de speler moeten de zombies rekening houden met verschillende obstakels op het parcours.
6. Om zo efficiënt mogelijk bij de speler te komen, moeten de zombies deze obstakels ontwijken.

### Overzicht van de observaties, mogelijke acties en beloningen

#### Observaties

- Afstand tussen zichzelf en speler
- Positie van de speler
- Positie van zichzelf
- Sensoren die objecten detecteren (gebaseerd op tags)  
    \-> RayPerceptionSensor3D
- Checkpoints zodat de zombie weet dat hij voorwaartse progressie maakt op het parcours
- Afstand tussen ML-Agent(zombie) en target(speler)
- Het huidig object die de ML-agent aanraakt

Code voor de observaties:

![Code voor de observaties](https://i.imgur.com/a/v7nH9Yz)

Zombie met sensoren:

![Zombie met sensoren](https://i.imgur.com/a/BOLOYWo)

#### Mogelijke acties

De acties bestaan uit 3 discrete actions met telkens 2-3 branches.

- Beweeg naar voren en achter
- Draai naar links en rechts
- Springen
- Geen actie (stil staan)

De mogelijke acties zijn beschreven in de TouchObjectType enum.

Beweging code:

![Alt text](https://i.imgur.com/a/d8F5oY3 "Beweging code")

Spring code:

![Alt text](https://i.imgur.com/a/MS7Dqht "Spring code")

#### Beloningen

- Positieve beloningen
  - Avanceren door de checkpoints
  - Target (de speler) aanraken
- Negatieve beloningen
  - Terugtrekken door de checkpoints
  - Muren raken
  - Op een spinnenweb staan
  - Door een gat vallen
  - (Te veel) springen
  - Een straf per gezette stap

ML-Agent punten configuratie:
![ML-Agent punten configuratie](https://i.imgur.com/a/CwU6afK)
Code voor checkpoint beloning:
![Code voor checkpoint beloning](https://i.imgur.com/a/iHSdBp0)

### Beschrijving van de objecten

- Speler: De target waarop zombies afkomen
- Zombie: De ML-Agent die getraind wordt om de speler te vinden
- Checkpoint: Onzichtbaar punt op het parcours
- Pistool: Een wapen dat de speler kan gebruiken om de ML-agent te doden
- Obstakels
  - Barrière: Muur die de zombie tegenhoudt
  - Spinnenweb: Object die ervoor zorgt dat de zombie vertraagd wordt.
  - Gat: Object waardoor de zombie kan vallen.

### Beschrijving van de gedragingen van de objecten

- Speler: Kan rondlopen in het eindstuk van de omgeving en een wapen oppakken om de zombies mee te beschieten
- Zombie: Beweegt door de arena met als doel de speler te bereiken en obstakels te vermijden.
- Checkpoint: Locatie op het parcours die bijhoudt of de zombie voorwaartse of achterwaartse progressie boekt.
- Pistool: Kan gevuurd worden om een zombie mee te verdelgen.
- Obstakels: Staan stil en fungeren als hindernissen voor de zombies

### One-pager

##### Team

- Roan Heylen
- Emirhan Ramazan Sahin
- Inge Leenaerts
- Soukayna Mohammed

##### Titel

Zombie Defense

##### Draaiboek

Als speler, sta je in het midden van een doodlopende straat.  
Er komen zombies op je af.

Je kan een wapen gebruiken om de zombies dood te schieten.  
Maar, elke wave komen er meer en meer zombies op je af.

Je krijgt bepaalde objecten die je kan plaatsen (zoals een barrière), om een zombie te laten vertragen.  
En hiermee moet je er voor zorgen dat je de zombies lang genoeg kan tegenhouden zodat je ze kan doodschieten.

##### Waarom ML?

We hebben ML nodig.  
Als we geen ML gebruiken, dan moeten we programatisch zelf een parkour hard-coden.  
Met ML kunnen we er voor zorgen dat de speler zelf een parkour kan maken, hierdoor leert de ML agent obstakels te vermijden en de snelste weg te vinden.  
Zonder ML, moeten we elke keer als het parkour veranderd, de zombie herprogrameren.

Het type AI is een Single-Agent.

##### Waarom VR?

VR maakt Zombie Defense beter door het niveau van immersie.  
Het zorgt ervoor dat spelers meer genijgt zijn om dit de spelen.  
De immersie zorgt voor extra spanning en angst, dit omdat het realistischer is.

De interactie zorgt er namelijk voor dat de speler realistischer kan schieten en kan interacten met de zombies.

##### Interactie

Het zal een VR zombie schietspel zijn waarbij spelers op zombies schieten met een wapen terwijl ze strategisch barrières en objecten plaatsen om de zombies te vertragen.  
De game bevat een timer die aangeeft wanneer spelers objecten kunnen plaatsen, dit kan alleen maar voor de timer is afgelopen, daarna komen de zombies op je af.  
Ze moeten hierdoor tactisch nadenken over het moment en de locatie van hun acties

### Afwijking van one-pager

Het dynamisch en strategisch plaats systeem van hindernis objecten hebben we niet kunnen integreren in het spel.  
Er is een statische map opgezet waardoor de zombies moeten navigeren.

## Resultaten

### Tensor Boards

### Oude grafiek

![Oude grafiek](https://i.imgur.com/a/T7dXUhB)

De grafiek die we hier terugvinden, is de grafiek van de eerste training.

Hier gebruikte de ML agent nog steeds ContinuousActions en werden rewards pas gegeven op het einde van de episode.

Zoals we kunnen zien, leert de ML agent wel, maar geraakt hij dan vast, we zien na 500k steps, geen evolutie.

Hierbij, zoals hieronder uitgelegd hebben we de BETA hyperparameter (randomness) verhoogt, dit toonde een goede evolutie, maar de ML agent geraakte toch nog vast rondt 2 miljoen steps.

##### Environment/Cumulatieve Reward

De cumulatieve beloning begint laag en vertoont enkele schommelingen. Na ongeveer 1 miljoen stappen begint de beloning te stijgen, wat wijst op verbetering van de prestaties van de agent.  
Na 1 miljoen stappen probeerde we eens de BETA hyperparameter te verhogen, dit gaf een zeer positieve feedback.

Maar daarna, daalt de reward zwaar, de ML agent weet niet meer wat hij moet doen.

##### Environment/Episode Length

Zoals we kunnen zien op de episode length grafiek, was de lengte per episode nog steeds niet zoals het hoorde, de episode length bleef stabiel staan.

Na het verhogen van de BETA hyperparameter toonde dit een goede evolutie.

##### Policy/Beta

De beta parameter hebben we geprobeerd te verhogen, omdat we kunnen zien bij de Entropy of de ML agent zekerder wordt van wat hij aan het doen is.  
Dit was niet het geval, hij bleef in de eerste zone staan, en deed niet veel.

De ML agent wist niet goed wat hij moest doen, door de BETA hyperparameter te verhogen (randomness) begon hij terug te experimenteren.

##### Policy/Entropy

Zoals we kunnen zien aan de Entropy waarde, blijft deze telkens stijgen.

Dit betekend dat de ML agent onzeker is, hij weet niet goed hoe hij het probleem moet oplossen.  
Dit zorgt voor een slechte training.

### Nieuwe grafiek

![Nieuwe grafiek](https://i.imgur.com/a/JedO19D)

##### Environment/Cumulatieve Reward

Zoals we kunnen zien aan de reward grafiek, hebben we steeds een opwaartse evolutie, dit betekend dat de ML agent steeds beter en beter wordt.

De cumulatieve beloning stijgt gestaag naarmate de training vordert, met enkele fluctuaties. Na ongeveer 1.2 miljoen stappen lijkt de beloning zich te stabiliseren rond een waarde van 19.5.

##### Environment/Episode Length

De gemiddelde lengte van de episodes neemt af van ongeveer 110 naar ongeveer 90 stappen naarmate de training vordert. Er zijn nog steeds enkele fluctuaties, maar de algehele trend is dalend.

Kortere episode-lengtes kunnen erop wijzen dat de agent efficiënter zijn doel bereikt of dat hij sneller faalt en opnieuw begint. Dit kan een teken zijn van verbeterde prestaties en snellere besluitvorming.

##### Policy/Entropy

De entropy-waarde begint rond 0.2 en daalt geleidelijk naar bijna 0 na 1.4 miljoen stappen.

We kunnen zien aan de entropy, dat de ML agent zekerder en zekerder wordt over wat hij nu juist aan het doen is.  
Het laat ons zien of dat de ML agent de meest efficiënte manier heeft gevonden om het probleem op te lossen.

### Tensor Boards Conclusie

We hebben vastgesteld dat het model vooruitgang boekt in het begrip en de interactie met de omgeving, zoals blijkt uit de geleidelijke verbetering van de cumulatieve beloning.

Er waren een paar problemen, doordat we die problemen hebben ondervonden, en opgelost, traint te ML agent nu veel efficiënter en sneller.

Ook, zorgt dit ervoor dat de ML agent steeds kan verbeteren, het kan beter worden dan de eerste training ooit zal zijn.

Zo zien we, door de gegevens van het TensorBoard krijgen we een beter begrip op wat de ML agent juist aan het doen is, en of onze methodes wel kloppen.

### Problemen die we ondervonden

##### Probleem 1

Het eerste probleem is het gebruik van Continuous Actions.

Dit houdt in dat de acties van de agent waardes kunnen aannemen tussen –1,00 en 1,00. Doordat tussen deze 2 waardes zeer veel mogelijke waardes zitten voor de agent om uit te kiezen, hebben we de volgende gevolgen:

- Trage en moeilijke training:
  - Door meerdere waardes voor het bewegen van de ML agent, leert hij trager.  
        De bewegingen zijn complexer doordat er meerdere decimale waardes zijn.

##### Probleem 2

De beloningen werden pas aan het einde van elke episode gegeven.

Dit betekent dus dat de agent geen directe feedback kreeg op zijn acties, maar pas aan het einde van een reeks acties een enkele beloning ontving. Hierdoor zijn de volgende problemen ontstaan.

- Vertraagde feedback:
  - De agent had moeite om te begrijpen welke van zijn acties goed of slecht waren omdat de beloning pas later werd gegeven.  
        In reinforcement learning is directe feedback cruciaal omdat het de agent helpt om snel te leren welke acties positieve resultaten opleveren.
- Verwarring en inefficiëntie:
  - De agent werd verward omdat het moeilijk was om de relatie tussen een specifieke actie en de uiteindelijke beloning te leggen.

### Oplossingen

##### Probleem 1

Omdat Continuous Actions problemen geeft, gaan we in plaats daarvan voor Discrete Actions.  
Dit houdt in dat de acties die de agent kan nemen vooraf gedefinieerd zijn en beperkte waarden hebben.

Bijvoorbeeld bij een sprong kan de actie 0 of 1 zijn, zonder tussen liggende waarden. Dit geeft de volgende gevolgen:

- Verminderde complexiteit
  - Doordat er minder mogelijke acties zijn, wordt de zoekruimte aanzienlijk minder voor de agent wat ervoor zorgt dat die eenvoudiger kan leren welke acties effectief zijn.
- Snellere training
  - Met een beperkte set acties kan de agent sneller bepalen welke acties leiden tot een positief resultaat.
- Betere prestaties
  - De agent kan stabieler en consistenter leren omdat hij niet verward raakt door vele mogelijke waarden.

##### Probleem 2

In plaats van de beloningen aan het eind van de episode te geven, wordt er nu een beloning of straf direct gegeven wanneer de agent een actie onderneemt.

Bijvoorbeeld als de agent naar een volgend checkpoint gaat, krijgt hij direct een beloning en als hij naar achter loopt krijgt hij per direct een straf. Dit geeft de volgende gevolgen:

- Snellere feedback
  - De agent ontvangt direct feedback over zijn acties waardoor hij sneller kan leren welke acties goed of slecht zijn.
- Verminderde verwarring
  - De agent kan onmiddellijk de relatie tussen zijn actie en het resultaat zien, waardoor verwarring wordt verminderd omdat de leerervaring wordt verbeterd.
- Efficiënt leren
  - Door directe feedback kan de agent sneller leren van zijn fouten en successen wat resulteert in een snellere en efficiëntere training.

### Opvallende waarneming

Gebaseerd op alles hierboven zijn hier in het kort nog een aantal opvallende bevindingen.

1. **Effectiviteit van Discrete Actions:** De agent vertoont betere trainingsresultaten wanneer gebruik wordt gemaakt van Discrete Actions in plaats van Continuous Actions. Dit komt doordat Discrete Actions de complexiteit verlagen en de agent in staat stellen sneller en consistenter te leren.
2. **Impact van Directe Beloningen en Straffen**: in tegenstelling tot enkel 1 beloning/straf aan het eind van een episode, levert het gebruik van directe beloningen en straffen tijdens de training verbeterde leerprestaties van de agent op . Dit zorgt voor snellere feedback, vermindert verwarring en bevordert efficiënter leren.

## Conclusie

We hebben een AI-gedreven VR Zombie Defense game gemaakt waarbij je jezelf moet verdedigen tegen AI Zombies doormiddel van obstakels en een wapen terwijl de zombies je proberen te pakken.

Door de resultaten te bekijken op het TensorBoard, het te bekijken van rewards, het te testen van de ML agent hebben we enkele problemen ondervonden.

Door die problemen te vinden en te diagnosen hebben we een oplossing kunnen vinden en zo een efficiënte en werkende oplossing kunnen bieden voor het ML agent project.

### Onze persoonlijke visie op dit project

Onze persoonlijke visie?

Een ML agent is niet zoiets als een mens, die kan je zeer complexe dingen aanleren.  
De ML agent, snapt niet goed wat de wereld rond zich betekend.

Dus, door deze resultaten, kunnen de ML agent bijsturen en er voor zorgen dat we een oplossing hebben die werkt.

De resultaten vertellen ons of de ML agent goed bezig is, en of wij het moeten bijsturen of niet.

### Toekomstverbeteringen

- De ML agent langer laten trainen
- De target meer te laten bewegen voor betere training resultaten
- Een moeilijkheidsniveau, gebaseerd op trainingen met meer steps en trainingen met minder steps. (Voor de zombies)
- De mogelijkheid om punten te kunnen scoren om het spel competitiever te maken en de speler gemotiveerd te houden.

## Gebruikte assets

Modern Guns Handgun: <https://assetstore.unity.com/packages/3d/props/guns/modern-guns-handgun-129821>

Training table: <https://assetstore.unity.com/packages/3d/environments/training-table-136070>

Buildings: <https://assetstore.unity.com/packages/3d/environments/free-open-building-112907>

Zombie: <https://assetstore.unity.com/packages/3d/characters/humanoids/zombie-30232>