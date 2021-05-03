# MyCourse
In questo repository si trova il codice del progetto `MyCourse`, che viene presentato nel corso `ASP.NET Core per tutti` su Udemy.

[https://www.udemy.com/aspnetcore-per-tutti/](https://www.udemy.com/aspnetcore-per-tutti/)

## Panoramica
Il progetto è un'applicazione ASP.NET Core 5 dimostrativa per la pubblicazione e fruizione di corsi on-line. L'applicazione è pubblicata all'indirizzo:
[https://my-course.azurewebsites.net](https://my-course.azurewebsites.net)

![Screenshot dell'applicazione MyCourse](docs/images/my-course.png)


## Ottenere il codice del progetto
Per chi non è abituato a usare GitHub, ecco spiegate le due opzioni per ottenere il codice di questo progetto:

 1. Per prima cosa [installare GIT](https://git-scm.com/book/it/v1/Per-Iniziare-Installare-Git#Installare-su-Linux) sul proprio PC o Mac. Poi creare una nuova cartella vuota, aprire il prompt dei comandi (anche chiamato _Terminale_) e posizionarsi nella directory appena creata digitando:
    ```
    cd percorsoDirectoryCreata
    ```
    a questo punto clonare il repository GIT digitando il comando:
    ```
    git clone https://github.com/ItalyDotNet/MyCourse.git .
    ```
 2. La seconda alternativa consiste semplicemente nello scaricare un file ZIP del progetto cliccando il bottone verde "Clone or download" che si trova in questa pagina e poi "Download ZIP".
    
    Prima di scaricare lo ZIP, selezionare il ramo che si intende ottenere cliccando il bottone grigio "Branch: master" che si trova in questa pagina.
	
## Elenco degli utenti
Il database è popolato di utenti con ruoli diversi in modo da sperimentare l'accesso alle varie pagine di cui è composta l'applicazione.

### Utente con ruolo Administrator
Può accedere a /Admin/Users per gestire gli utenti

|Nome completo    |Email            |Password       |
|-----------------|-----------------|---------------|
|Severino Padovano|admin@example.com|Administrator1!|

### Utenti con ruolo Teacher
Possono creare corsi, modificarli e pubblicarli.

|Nome completo    |Email            |Password       |
|-----------------|-----------------|---------------|
|Severino Padovano|severino.padovano@example.com|Teacher1!|
|Filiberta Castiglione|filiberta.castiglione@example.com|Teacher1!|
|Guglielma Rivo|guglielma.rivo@example.com|Teacher1!|
|Elvira Piccio|elvira.piccio@example.com|Teacher1!|
|Sandro Longo|sandro.longo@example.com|Teacher1!|
|Salvatore Marino|salvatore.marino@example.com|Teacher1!|
|Ottavio Manna|ottavio.manna@example.com|Teacher1!|
|Furio Bianchi|furio.bianchi@example.com|Teacher1!|
|Crescenzo Cattaneo|crescenzo.cattaneo@example.com|Teacher1!|
|Enea Padovano|enea.padovano@example.com|Teacher1!|
|Nella Pisano|nella.pisano@example.com|Teacher1!|
|Calogero Trevisano|calogero.trevisano@example.com|Teacher1!|
|Fausto Toscani|fausto.toscani@example.com|Teacher1!|
|Ambrosino Nucci|ambrosino.nucci@example.com|Teacher1!|
|Antonella Zetticci|antonella.zetticci@example.com|Teacher1!|
|Miranda Milanesi|miranda.milanesi@example.com|Teacher1!|
|Eric Mazzanti|eric.mazzanti@example.com|Teacher1!|
|Adelinda Greco|adelinda.greco@example.com|Teacher1!|
|Serena Pagnotto|serena.pagnotto@example.com|Teacher1!|
|Penelope Lucchesi|penelope.lucchesi@example.com|Teacher1!|
|Alfredo Genovesi|alfredo.genovesi@example.com|Teacher1!|
|Rodrigo Bellucci|rodrigo.bellucci@example.com|Teacher1!|
|Rosetta Monaldo|rosetta.monaldo@example.com|Teacher1!|
|Azzurra Ricci|azzurra.ricci@example.com|Teacher1!|
|Marcella Barese|marcella.barese@example.com|Teacher1!|
|Duilio Ferri|duilio.ferri@example.com|Teacher1!|
|Davide Fosca|davide.fosca@example.com|Teacher1!|
|Mariano Cocci|mariano.cocci@example.com|Teacher1!|
|Vanna Palermo|vanna.palermo@example.com|Teacher1!|
|Angelo Endrizzi|angelo.endrizzi@example.com|Teacher1!|
|Anastasia Trentini|anastasia.trentini@example.com|Teacher1!|
|Elsa De Luca|elsa.de.luca@example.com|Teacher1!|

### Utenti privi di ruolo
Possono iscriversi ai corsi e inviare domande ai docenti.

|Nome completo    |Email            |Password       |
|-----------------|-----------------|---------------|
|Mario Rossi      |mario@example.com|Student1!      |

> ...e ogni nuovo utente registrato sarà privo di ruolo.