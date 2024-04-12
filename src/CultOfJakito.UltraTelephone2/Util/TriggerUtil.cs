using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Util
{
    public static class TriggerUtil
    {
        public static void MakeTrigger(Action onEnter, Vector3 pos, Vector3 size, Quaternion? rot = null)
        {
            ObjectActivator obac = NewTrigger();
            obac.transform.position = pos;
            obac.transform.localScale = size;
            if (rot.HasValue)
                obac.transform.rotation = rot.Value;

            obac.oneTime = true;
            obac.events.onActivate.AddListener(onEnter.Invoke);
        }

        public static ObjectActivator NewTrigger()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().enabled = false;
            cube.GetComponent<BoxCollider>().isTrigger = true;
            cube.layer = LayerMask.NameToLayer("Invisible");
            return cube.AddComponent<ObjectActivator>();
        }
    }
}
