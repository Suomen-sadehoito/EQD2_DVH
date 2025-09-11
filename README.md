# EQD2_DVH

Tämä on Varian Eclipse -hoitosuunnittelujärjestelmään (TPS) tarkoitettu ESAPI-skripti, joka laskee ja visualisoi ekvivalenttiannoksen 2 Gy:n fraktioissa (EQD2) annostilavuushistogrammeja (DVH).

## Ominaisuudet

* **DVH-käyrien visualisointi:** Piirtää sekä alkuperäisen annoksen DVH-käyrän että lasketun EQD2 DVH-käyrän samaan kuvaajaan.
* **Muokattavat α/β-arvot:** Käyttäjä voi asettaa ja muokata kunkin rakenteen alfa/beeta-suhdetta. Oletusarvot asetetaan automaattisesti yleisimpien kudostyyppien mukaan.
* **Yksityiskohtainen yhteenveto:** Tarjoaa taulukon, jossa näkyvät tärkeimmät DVH-parametrit (Dmax, Dmean, Dmin, tilavuus) sekä alkuperäiselle että EQD2-annokselle.
* **Kaksi laskentatapaa D.mean-arvolle:** Oletuksena käytössä on tarkempi Differentiaali-DVH -menetelmä.
    * **Yksinkertainen:** Muuntaa suoraan fyysisen keskiarvoannoksen EQD2-arvoksi.
    * **Differentiaali-DVH (Oletus):** Laskee painotetun keskiarvon differentiaalisesta DVH-käyrästä, mikä antaa tarkemman keskiarvoannoksen.
* **CSV-vienti:** Kaikki yhteenvetotiedot voidaan viedä CSV-tiedostoon.

## Käyttöohjeet

### Asennus

1.  **Käännä projekti:** Käännä ratkaisu Visual Studiossa (**Build > Build Solution**). Varmista, että projektin asetuksissa **Platform target** on asetettu `x64`-arkkitehtuurille.
2.  **Kopioi tiedostot:** Kääntämisen jälkeen kopioi seuraavat tiedostot projektin `bin/x64/Debug` (tai `Release`) -kansiosta Eclipsen skriptihakemistoon:
    * `EQD2_DVH.esapi`
    * `OxyPlot.dll`
    * `OxyPlot.Wpf.dll`
    * `OxyPlot.Wpf.Shared.dll`

    Voit löytää oikean skriptihakemiston Eclipsestä valitsemalla **Tools > Scripts** ja klikkaamalla **Open Folder** -painiketta.

### Suorittaminen

1.  **Avaa potilas:** Käynnistä Eclipse ja avaa potilas, jolla on vähintään yksi laskettu hoitosuunnitelma.
2.  **Käynnistä skripti:** Mene Eclipsessä kohtaan **Tools > Scripts** ja valitse `EQD2_DVH.esapi`-tiedosto listasta käynnistääksesi skriptin.
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

## Tunnetut ongelmat

* **Ikkunan sulkemisvirhe:** Kun käyttäjä sulkee skripti-ikkunan, skripti antaa seuraavan virheilmoituksen:
    `There was a problem while executing the script 'EQD2_DVH.esapi.dll'. Cannot set Visiblity or call Show, Showdialog, or WindowInteropHelper.EnsureHandle after a Window was closed.` Tämä ei kuitenkaan haittaa käyttöä.
  
## Tekninen toteutus

* **Kieli ja alusta:** C# ja WPF (Windows Presentation Foundation) .NET Framework 4.7.2 -ympäristössä.
* **Kirjastot:**
    * **Varian ESAPI (Eclipse Scripting API):** Rajapinta, jonka kautta skripti kommunikoi Eclipse-tietokannan kanssa hakeakseen potilas-, suunnitelma- ja rakennetietoja.
    * **OxyPlot:** Avoimen lähdekoodin kirjasto, jota käytetään DVH-käyrien piirtämiseen ja visualisointiin.
* **Arkkitehtuuri:** Sovellus noudattaa MVVM (Model-View-ViewModel) -suunnittelumallia, mikä erottaa käyttöliittymän (View) sovelluslogiikasta (ViewModel).
    * `MainViewModel.cs`: Sisältää pääikkunan toiminnallisuuden, kuten datan hallinnan ja komentojen toteutuksen.
    * `DVHCalculator.cs`: Sisältää staattiset metodit EQD2-arvojen laskemiseksi sekä pistekohtaisesti että koko DVH-käyrälle.

## Lisenssi

Tämä projekti on lisensoitu **MIT-lisenssillä**. Katso `LICENSE.txt`-tiedosto lisätietoja varten.
