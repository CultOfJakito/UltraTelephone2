using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos;

internal abstract class ChaosEffect : MonoBehaviour, IChaosEffect, IDisposable {
	public abstract void BeginEffect(System.Random random);

	public virtual void Dispose() {
		Destroy(this);
	}
}
