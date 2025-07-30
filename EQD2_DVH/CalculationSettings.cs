namespace EQD2_DVH
{
    /// <summary>
    /// Määrittelee käytettävissä olevat laskentatavat EQD2 D.mean -arvolle.
    /// </summary>
    public enum EQD2MeanCalculationMethod
    {
        /// <summary>
        /// Nopea arvio: Muuntaa suoraan fyysisen keskiarvoannoksen.
        /// </summary>
        Simple,

        /// <summary>
        /// Tarkempi: Laskee painotetun keskiarvon DVH-käyrän datasta.
        /// </summary>
        Differential
    }
}