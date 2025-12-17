# EQD2_DVH

Skripti laskee ja visualisoi 2 Gy:n ekvivalenttiannoksen (EQD2) annostilavuushistogrammeja (DVH).

## Ominaisuudet

* **DVH-käyrien visualisointi:** Piirtää sekä alkuperäisen annoksen DVH-käyrän että lasketun EQD2 DVH-käyrän samaan kuvaajaan.
* **Muokattavat α/β-arvot:** Käyttäjä voi asettaa ja muokata kunkin rakenteen alfa/beeta-suhdetta. Oletusarvot asetetaan automaattisesti yleisimpien kudostyyppien mukaan.
* **Yksityiskohtainen yhteenveto:** Tarjoaa taulukon, jossa näkyvät tärkeimmät DVH-parametrit (Dmax, Dmean, Dmin, tilavuus) sekä alkuperäiselle että EQD2-annokselle.
* **Kaksi laskentatapaa D.mean-arvolle:**
    * **Yksinkertainen (Oletus):** Muuntaa alkuperäisen D.mean annoksen suoraaan EQD2-arvoksi.
    * **Differentiaali-DVH:** EQD2 lasketaan jakamalla alkuperäinen DVH "bineihin", joista lasketaan painotettu keskiarvo. Hyötyä eniten epätasaisella annosjakaumalla.
* **CSV-vienti:** Kaikki yhteenvetotiedot voidaan viedä CSV-tiedostoon.

## Käyttöohjeet

### Asennus

Tämän skriptin käyttöönotto vaatii vain **yhden tiedoston** kopioimista.

1.  Lataa uusin `EQD2_DVH.esapi.dll` -tiedosto tämän projektin "Releases"-sivulta.
2.  Kopioi **vain tämä yksi tiedosto** Eclipsen skriptihakemistoon.
    * *Kaikki tarvittavat kirjastot (kuten OxyPlot) on paketoitu tiedoston sisään.*
    * Voit löytää oikean skriptihakemiston Eclipsestä valitsemalla **Tools > Scripts** ja klikkaamalla **Change Folder...** -painiketta.

### Suorittaminen

1.  **Avaa potilas:** Käynnistä Eclipse ja avaa potilas, jolla on vähintään yksi laskettu hoitosuunnitelma.
2.  **Käynnistä skripti:** Mene Eclipsessä kohtaan **Tools > Scripts** ja valitse `EQD2_DVH.esapi.dll`-tiedosto listasta käynnistääksesi skriptin.
3.  **Valitse suunnitelma ja rakenteet:**
    * Skriptin käynnistyttyä avautuu valintaikkuna.
    * Valitse pudotusvalikosta hoitosuunnitelma.
    * Valitse alla olevasta listasta ne rakenteet, jotka haluat analysoida.
    * Paina **"Piirrä DVH"** -painiketta.
4.  **Analysoi tulokset:**
    * Pääikkuna avautuu, ja näet valittujen rakenteiden DVH-käyrät sekä yhteenvedon niiden arvoista.
    * Määritä **Alpha/Beta Asetukset** -laatikossa kullekin rakenteelle sopiva **α/β-arvo**.
    * Paina **"Laske EQD2"** -painiketta. Tämä lisää kuvaajaan EQD2-käyrät katkoviivalla ja päivittää yhteenvetotaulukon uusilla arvoilla.
5.  **Lisätoiminnot:**
    * **Kuvaajan asetukset:** Voit piilottaa tai näyttää alkuperäiset ja EQD2-käyrät valintaruuduilla.
    * **Asetukset:** Voit vaihtaa EQD2 D.mean -arvon laskentatapaa.
    * **Vie yhteenveto CSV:** Tallentaa kaikki yhteenvetotaulukon tiedot CSV-tiedostoon.
    * **Tyhjennä ja aloita alusta:** Nollaa koko näkymän ja avaa uudelleen suunnitelman ja rakenteiden valintaikkunan.

## Kehittäjälle

Projekti on konfiguroitu niin, että se paketoi kaikki riippuvuudet (kuten OxyPlot) automaattisesti yhdeksi `EQD2_DVH.esapi.dll`-tiedostoksi käyttäen `Costura.Fody`-pakettia.

Jotta voit kääntää projektin, sinun on hankittava Varianin ESAPI-kirjastot:

1.  **Luo kansio:** Luo projektin `EQD2_DVH`-kansion sisään (samalle tasolle kuin `EQD2_DVH.sln`-tiedosto) kansio nimeltä `ESAPI_LIBS`.
2.  **Kopioi kirjastot:** Kopioi omalta Eclipse-työasemaltasi seuraavat tiedostot tähän `ESAPI_LIBS`-kansioon:
    * `VMS.TPS.Common.Model.API.dll`
    * `VMS.TPS.Common.Model.Types.dll`
3.  **Käännä:** Avaa `EQD2_DVH.sln` Visual Studiossa. NuGet-pakettien (kuten OxyPlot ja Costura.Fody) pitäisi palautua automaattisesti. Käännä "solution" (**Build > Build Solution**) `Release | x64` -asetuksilla.
4.  Valmis `.dll`-tiedosto löytyy kansiosta `bin/x64/Release/`.

## Tekninen toteutus

* **Kieli ja alusta:** C# ja WPF (Windows Presentation Foundation) .NET Framework 4.7.2 -ympäristössä.
* **Kirjastot:**
    * **Varian ESAPI (Eclipse Scripting API):** Rajapinta, jonka kautta skripti kommunikoi Eclipse-tietokannan kanssa.
    * **OxyPlot:** Avoimen lähdekoodin kirjasto, jota käytetään DVH-käyrien piirtämiseen.
    * **Costura.Fody:** Työkalu, joka paketoi riippuvuuskirjastot (kuten OxyPlotin) suoraan lopullisen `.dll`-tiedoston sisään.

## Tekijät:

* Juho Ala-Myllymäki ja Risto Hirvilammi Vaasan keskussairaala

## Lisenssi

Tämä projekti on lisensoitu **MIT-lisenssillä**. Katso `LICENSE.txt`-tiedosto lisätietoja varten.
