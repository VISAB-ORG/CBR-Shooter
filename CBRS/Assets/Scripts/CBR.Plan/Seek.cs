using System.Runtime.Serialization;
namespace Assets.Scripts.CBR.Plan
{
    /**
     *  Class to display the seek action.
     *  
     * @author Chris Wieczorek
     *
     */
    [DataContract]
    public class Seek : Action
    {
        /**
         * Default-Konstruktor.
         */
        public Seek() : this(false) { }

        /**
         * Konstruktor, der die Angabe erwartet, ob die Aktion sequentiell oder parallel durchgeführt werden kann.
         */
        public Seek(bool sequentiel) : base("Seek", sequentiel)
        {
        }
    }
}
