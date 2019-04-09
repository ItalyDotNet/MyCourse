//Per fare il debug di questi esempi, devi prima installare il global tool dotnet-script con questo comando:
//dotnet tool install -g dotnet-script
//Trovi altre istruzioni nel file /scripts/readme.md


//ESEMPIO #1: Definisco una lambda che accetta un parametro DateTime e restituisce un bool, e l'assegno alla variabile canDrive
Func<DateTime, bool> canDrive = dob => {
    return dob.AddYears(18) <= DateTime.Today;
};

//Eseguo la lambda passandole il parametro DateTime
DateTime dob = new DateTime(2002, 12, 25);
bool result = canDrive(dob);
//Poi stampo il risultato bool che ha restituito
Console.WriteLine(result);

//ESEMPIO #2: Stavolta definisco una lambda che accetta un parametro DateTime ma non restituisce nulla
Action<DateTime> printDate = date => Console.WriteLine(date);

//La invoco passandole l'argomento DateTime
DateTime date = DateTime.Today;
printDate(date);

/*** ESERCIZI! ***/

// ESERCIZIO #1: Scrivi una lambda che prende due parametri stringa (nome e cognome) e restituisce la loro concatenazione
// Func<...> concatFirstAndLastName = ...;
// Qui invoca la lambda

// ESERCIZIO #2: Una lambda che prende tre parametri interi (tre numeri) e restituisce il maggiore dei tre
// Func<...> getMaximum = ...;
// Qui invoca la lambda

// ESERCIZIO #3: Una lambda che prende due parametri DateTime e non restituisce nulla, ma stampa la minore delle due date in console con un Console.WriteLine
// Action<...> printLowerDate = ...;
// Qui invoca la lambda