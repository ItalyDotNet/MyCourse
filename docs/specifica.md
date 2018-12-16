# Specifica
Il committente del progetto è la ditta Formazione srl (fittizia) che opera nell'ambito dell'editoria di corsi di formazione, divulgati attraverso supporti tradizionali come libri e DVD venduti in edicola. Dopo alcuni incontri, è emersa l'esigenza di entrare nel mercato dei corsi on-line. Si è deciso che il mezzo per fare questo è una piattaforma web per la vendita di corsi.

Le indagini di mercato hanno rivelato che la domanda di corsi online è in crescita e perciò si stima un progressivo aumentare del traffico web. Il team di sviluppo ha deciso di usare ASP.NET Core per via della sua snellezza e scalabilità.

Dopo un primo incontro con il committente, nel quale sono stati chiariti gli obiettivi di business, si è prodotta una specifica di una ventina di punti che vengono elencati di seguito.

# Requisiti funzionali
Funzionalità da implementare nell'applicazione, già concordate con il committente.

## Parte pubblica del sito (per visitatori anonimi e studenti)
 1. L'homepage deve presentare una selezioni di contenuti. In particolare deve visualizzare due liste: quella dei corsi aggiunti di recente (i 3 più recenti) e quella dei corsi con valutazione più alta (i 3 con valutazione più alta);
 2. Gli utenti devono poter visualizzare l'intero catalogo dei corsi pubblicati da una pagina di elenco. Per ogni corso in elenco, visualizzare un titolo, un'immagine rappresentativa, l'autore, la valutazione, il prezzo intero e il prezzo corrente di acquisto;
 3. Il prezzo corrente di acquisto deve essere inferiore o uguale al prezzo intero e devono avere entrambi la stessa valuta;
 4. Il catalogo dei corsi deve essere paginato (max 10 risultati per pagina) e deve supportare la ricerca per titolo e l'ordinamento per titolo, valutazione e prezzo corrente;
 5. Cliccando uno dei risultati nell'elenco, si accede alla pagina di dettaglio del corso contenente anche la descrizione completa, oltre agli altri valori già mostrati in elenco;
 6. Nella pagina di dettaglio del corso deve essere visualizzato l'elenco delle lezioni del corso con titolo, descrizione e durata stimata di completamento;
 7. Nella pagina di dettaglio del corso deve trovarsi un link a una pagina di contatto da cui lo studente potrà inviare domande al docente;
 8. Nella pagina di dettaglio del corso deve trovarsi un bottone per far registrare lo studente;
 9. La registazione a un corso ha successo solo dopo che lo studente ha completato il pagamento con Paypal. A quel punto lo studente è autorizzato ad accedere al contenuto delle lezioni;
 10. Lo studente può esprimere un voto da 1 a 5 sul corso, ma solo se è registrato a quel corso;

## Gestione dei corsi (solo per i docenti)
 11. Nella pagina di elenco deve esserci un link alla pagina di creazione di un nuovo corso;
 12. Un docente deve poter creare un nuovo corso fornendo un titolo e l'autore (cioè il suo stesso nome);
 13. Allo stesso modo, nella pagina di dettaglio di un corso deve esserci un link alla pagina di modifica;
 14. La pagina di modifica deve consentire l'aggiornamento del titolo, la descrizione completa, il prezzo intero e attuale e l'immagine rappresentativa. L'autore non può essere modificato perché deve restare di proprietà di chi l'ha creato;
 15. Nella pagina di modifica, installare un editor WYSIWYG per consentire l'inserimento di descrizione formattata in HTML;
 16. La descrizione HTML del corso deve essere sicura, quindi il docente non può inserire script o altri contenuti malevoli. È consentito l'embed tramite iframe ma solo di video YouTube;
 17. Nella pagina di modifica, consentire la gestione delle lezioni del corso, che include inserimento, modifica ed eliminazione. Ciascuna lezione ha un titolo, una durata stimata per il completamento e un corpo HTML sicuro;
 18. Quando un corso viene creato, si trova in stato di "Draft" e non è visibile nella parte pubblica del sito finché il docente non decide di portarlo allo stato "Published". Il docente può riportare lo stato da "Published" a "Draft" ma il corso continuerà ad essere visibile agli studenti che si sono già iscritti;
 19. Per politica aziendale, un corso non può essere eliminato normalmente dal database. Tuttavia, può essere portato dal docente sullo stato "Deleted" che lo renderà di fatto invisibile e immodificabile da chiunque, compreso il docente;
 20. Un corso può essere portato sullo stato "Deleted" solo se non ci sono già iscritti;
 21. L'applicazione deve comunque rispettare la normativa del GDPR e quindi i docenti hanno diritto a visualizzare tutti i dati in possesso dell'applicazione. Inoltre, il docente ha diritto all'oblio e perciò, se decide di eliminare il proprio account, verranno fisicamente eliminati i suoi dati e tutti i corsi da lui creati. Lo staff di MyCourse esaminerà lo storico delle transazioni Paypal per capire a quali studenti andranno erogati i rimborsi;
 22. Le pagine di inserimento, modifica ed eliminazione devono ovviamente essere accessibili solo ai docenti.

## Gestione degli utenti (solo per amministratori)
 23. Un utente amministratore può cambiare il ruolo di un utente registrato (nessun ruolo per gli studenti, ruolo "Teacher" per i docenti e ruolo "Administrator" per altri amministratori). Quindi ogni utente registrato può avere al massimo un solo ruolo.

# Requisiti non funzionali
Come sviluppatore, decidi di realizzare un'applicazione di qualità e quindi vuoi integrare i seguenti accorgimenti nel prodotto finale.
  * a) Hai scelto di usare ASP.NET Core perché è una tecnologia snella e che permetterà un'elevata scalabilità nel cloud per assecondare l'aumentare delle visite all'applicazione. Prima di iniziare, però, devi formarti sull'uso di questa tecnologia;
  * b) Monitorerai le performance dell'applicazione per identificare eventuali disservizi che potrebbero limitare o impedire una corretta fruizione dell'applicazione da parte degli utenti;
  * c) Terrai un log su file per tracciare le operazioni più importanti, come l'esito delle transazioni monetarie;
  * d) Scriverai test unitari e d'integrazione per comprovare il corretto funzionamento dei componenti dell'applicazione.