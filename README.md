# InkassoMaeglerenAPIDemo
Inkasso Mægleren ApS API

Der er 2 projekter i den Visual Studio Solution man henter ned. Begge projekter kan fuldkommen det samme, de er bare udviklet i 2 forskellige teknologier. Den ene i .NET Framework 4.8, som er den sidste version af det gamle framework, før det blev "smeltet sammen" med .NET Core i .NET 5. 
.NET 5 er en videreførsel af .NET Core, men man kalder det bare ikke Core længere. I stedet hedder det bare .NET.
Det andet projekt er i .NET 6 som er efterfølgeren til .NET 5.
Begge projekter er lavet i GUI-frameworket WPF.

Selve API´et er et Window Communication Foundation (WCF) -projekt, udviklet i Visual Studio.

WCF bruges primært til SOAP. Der kan dog laves et json endpoint, hvilket jo ikke gør det til et REST api, da REST jo er en arkitektur, men i "hverdagsforstand" kalder man det REST når det er et web-api som kan tilgås via https og med objekter i json-format. API´et benytter kun GET og POST, da det er normalen for SOAP.

Hvorfor SOAP i 2022? Det er et godt spørgsmål. Det korte svar er: Fordi det er det nemmeste for os. Det går lynhurtigt at lave ændringer, da koden hurtigt bliver spejlet via Service referencen (en Visual Studio -feature). Desuden er der stadig ting, som SOAP er bedst til, f.eks. transaktioner på tværs af kald.

I øvrigt er det da også rart at kunne tilbyde begge ting, om end den såkaldte "REST"-del ikke er så gennemført som et ægte REST-api. Vi har dog ikke oplevet nogen problemer og derfor har det endnu ikke stået øverst på ønskelisten at lave det om.

Det primære API-kode findes i MainWindow.xaml.cs
