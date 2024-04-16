using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Util
{
    /// <summary>
    /// Utility class for locating objects in the _scene because frankly, Unity sucks at it.
    /// </summary>
    public static class Locator
    {
        /// <summary>
        /// Returns the first object of type T in the _scene that matches the given parental path. If no object is found, returns null.
        /// </summary>
        /// <typeparam name="T">Type of the component</typeparam>
        /// <param name="path">Parental path of the object for matching accuracy</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T LocateComponent<T>(string path) where T : Component
        {
            return LocateComponentsOfType<T>().FirstOrDefault(x => x.transform.HasParentalPath(path));
        }

        /// <summary>
        /// Locates all objects of type T in the _scene including inactive objects
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns></returns>
        public static T[] LocateComponentsOfType<T>() where T : Component
        {
            return Resources.FindObjectsOfTypeAll<T>().Where(x => x.hideFlags != HideFlags.NotEditable && x.hideFlags != HideFlags.HideAndDontSave).ToArray();
        }

        /// <summary>
        /// Returns true if the parental names in hierarchy of the transform match the array of names in reverse order. So the last name in the array is the object, and sencond to last is it's parent, then on until a match is made or rejected.
        /// </summary>
        /// <param name="tf">Object to check</param>
        /// <param name="parentNames">Parental path to validate</param>
        /// <returns></returns>
        public static bool HasParentalPath(this Transform tf, string[] parentNames)
        {
            Transform current = tf;

            for (int i = parentNames.Length - 1; i >= 0; i--)
            {
                if (current == null || current.name != parentNames[i])
                    return false;

                current = current.parent;
            }

            return true;
        }

        /// <summary>
        /// Checks if object has the given parental path in the hierarchy. Starts from last name in the path and goes up the hierarchy. Path separator should be / ALWAYS
        /// </summary>
        /// <param name="tf">object to check</param>
        /// <param name="path">Path separated by /</param>
        /// <returns></returns>
        public static bool HasParentalPath(this Transform tf, string path)
        {
            return HasParentalPath(tf, path.Split('/'));
        }

        /// <summary>
        /// Locates a given object type with a matching name in the children of the given transform, including inactive. If no object is found, returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tf"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T LocateComponentInChildren<T>(this Transform tf, string name) where T : Component
        {
            return tf.GetComponentsInChildren<T>(true).FirstOrDefault(x => x.name == name);
        }
    }
}
