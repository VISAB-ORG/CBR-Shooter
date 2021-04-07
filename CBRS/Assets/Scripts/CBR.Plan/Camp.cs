using System.Runtime.Serialization;
namespace Assets.Scripts.CBR.Plan
{
    /**
     *  Class to display the Camp action.
     *  
     * @author Chris Wieczorek
     *
     */
    [DataContract]
    public class Camp : Action
    {
        /**
         * Default-Konstruktor.
         */
        public Camp() : this(false) { }

        /**
         * Konstruktor, der die Angabe erwartet, ob die Aktion sequentiell oder parallel durchgeführt werden kann.
         */
        public Camp(bool sequentiel) : base("Camp", sequentiel)
        {
        }
    }
}
