using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Chaos.Effects;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Util
{
    public static class BookUtil
    {
        public static Readable CreateBook()
        {
            GameObject book = GameObject.Instantiate(UkPrefabs.Book.GetObject());
            Readable readable = book.GetComponent<Readable>();
            book.transform.position = CameraController.Instance.transform.position;
            return readable;
        }

        public static Readable IgnoreCantReadEffect(this Readable readable)
        {
            CantRead.IgnoreBookGameObjectHashes.Add(readable.gameObject.GetInstanceID());
            return readable;
        }

        public static Readable SetText(this Readable readable, string text)
        {
            readable.content = text;
            return readable;
        }

        public static Readable SetPosition(this Readable readable, Vector3 position)
        {
            readable.transform.position = position;
            return readable;
        }

    }
}
