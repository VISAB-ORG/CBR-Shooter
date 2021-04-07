using System.Runtime.Serialization;
namespace Assets.Scripts.CBR.Plan
{
    /**
     * Klasse zur Darstellung der konkreten Aktion, dass der Spieler sich bewegen
     * soll. Diese Klasse erbt somit von der abstrakten Klasse Action.
     * 
     * @author Jannis Hillmann
     *
     */
    [DataContract]
    public class MoveToEnemy : Action
    {
        /**
         * Default-Konstruktor.
         */
        public MoveToEnemy() : this(false) { }

        /**
         * Konstruktor, der die Angabe erwartet, ob die Aktion sequentiell oder parallel durchgeführt werden kann.
         */
        public MoveToEnemy(bool sequentiel) : base("MoveToEnemy", sequentiel)
        {
        }
    }
}
