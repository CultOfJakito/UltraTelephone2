using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Fun.Transmissions
{
    [ConfigureSingleton(SingletonFlags.NoAutoInstance)]
    public class TransmissionManager : MonoSingleton<TransmissionManager>
    {
        [SerializeField] private Sprite anonymousIcon;

        public void SendTransmission(Transmission transmission)
        {

        }

        public void SendAnonymousTransmission(string content)
        {

        }

    }
}
