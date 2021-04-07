using System.Runtime.Serialization;


[DataContract]
public class StatisticsForPathViewer
{

    [DataMember]
    public string coordinatesCBRBot { get; set; }

    [DataMember]
    public string coordinatesScriptBot { get; set; }

    [DataMember]
    public string healthCBRBot { get; set; }

    [DataMember]
    public string healthScriptBot { get; set; }

    [DataMember]
    public string weaponCBRBot { get; set; }

    [DataMember]
    public string weaponScriptBot { get; set; }

    [DataMember]
    public string weaponMagAmmuCBRBot { get; set; }

    [DataMember]
    public string weaponMagAmmuScriptBot { get; set; }

    [DataMember]
    public string statisticCBRBot { get; set; }

    [DataMember]
    public string statisticScriptBot { get; set; }

    [DataMember]
    public string nameCBRBot { get; set; }

    [DataMember]
    public string nameScriptBot { get; set; }

    [DataMember]
    public string planCBRBot { get; set; }

    [DataMember]
    public string planScriptBot { get; set; }

    [DataMember]
    public string healthPosition { get; set; }

    [DataMember]
    public string weaponPosition { get; set; }

    [DataMember]
    public string ammuPosition { get; set; }

    [DataMember]
    public string roundCounter { get; set; }

    /**
         * Default-Konstruktor
         */
    public StatisticsForPathViewer() : this("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "")
    {

    }

    /**
     * Konstruktor, der sämtliche Daten der Statistik erwartet.
     */
    public StatisticsForPathViewer(string coordinatesCBRBot, string coordinatesScriptBot, string healthCBRBot, string healthScriptBot, string weaponCBRBot, string weaponScriptBot, string statisticCBRBot, string statisticScriptBot, string nameScriptBot, string nameCBRBot, string planCBRBot, string weaponMagAmmuCBRBot, string weaponMagAmmuScriptBot, string healthPosition, string ammuPosition, string weaponPosition, string roundCounter, string planScriptBot)
    {
        this.coordinatesCBRBot = coordinatesCBRBot;
        this.coordinatesScriptBot = coordinatesScriptBot;
        this.healthCBRBot = healthCBRBot;
        this.healthScriptBot = healthScriptBot;
        this.weaponScriptBot = weaponScriptBot;
        this.weaponCBRBot = weaponCBRBot;
        this.statisticCBRBot = statisticCBRBot;
        this.statisticScriptBot = statisticScriptBot;
        this.nameCBRBot = nameCBRBot;
        this.nameScriptBot = nameScriptBot;
        this.planCBRBot = planCBRBot;
        this.weaponMagAmmuCBRBot = weaponMagAmmuCBRBot;
        this.weaponMagAmmuScriptBot = weaponMagAmmuScriptBot;
        this.healthPosition = healthPosition;
        this.ammuPosition = ammuPosition;
        this.weaponPosition = weaponPosition;
        this.roundCounter = roundCounter;
        this.planScriptBot = planScriptBot;
    }
    /**
     * ToString Methode der Klasse StatisticsForPathViewer
     */ 
    public override string ToString()
    {
        return "StatisticsForPathViewer [coordinatesCBRBot=" + coordinatesCBRBot + "]"
            + "[coordinatesScriptBot = " + coordinatesScriptBot + "]"
            + "[healthScriptBot = " + healthScriptBot + "]" + "[healthCBRBot = " + healthCBRBot + "]"
            + "[weaponScriptBot = " + weaponScriptBot + "]" + "[weaponCBRBot = " + weaponCBRBot + "]"
            + "[statisticScriptBot = " + statisticScriptBot + "]" + "[statisticCBRBot = " + statisticCBRBot + "]"
            + "[nameScriptBot = " + nameScriptBot + "]" + "[nameCBRBot = " + nameCBRBot + "]"
            + "[planCBRBot = " + planCBRBot + "]"
            + "[weaponMagAmmuCBRBot = " + weaponMagAmmuCBRBot + "]" + "[weaponMagAmmuScriptBot = " + weaponMagAmmuScriptBot + "]"
            + "[healthPosition = " + healthPosition + "]" + "[weaponPosition = " + weaponPosition + "]" + "[ammuPosition = " + ammuPosition + "]"
            + "[roundCounter = " + roundCounter + "]" + "[planScriptBot = " + planScriptBot + "]";
    }

}
