using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{

    [SerializeField]
    float dyingDuration;

    void OnTriggerEnter(Collider other)
    {
        var shape = other.GetComponent<Shape>();
        if (shape)
        {
            if (dyingDuration <= 0f)
                shape.Die();
            else if (!shape.IsMarkAsDying)
            {
                shape.AddBehavior<DyingShapeBehavior>().Initialize(shape, dyingDuration);
            }
            else
                shape.AddBehavior<DyingShapeBehavior>().Initialize(shape, dyingDuration);
        }
    }

	void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		
		var c = GetComponent<Collider>();
		var b = c as BoxCollider;
		if(b != null)
		{
			Gizmos.matrix = Matrix4x4.TRS(transform.position,transform.rotation,transform.lossyScale);

			Gizmos.DrawWireCube(b.center,b.size);
			return;
		}
		var s = c as SphereCollider;
		if(s != null)
		{
			Vector3 sacle = transform.lossyScale;
			sacle = Vector3.one * Mathf.Max(Mathf.Abs(sacle.x),Mathf.Abs(sacle.y),Mathf.Abs(sacle.z));
			Gizmos.matrix = Matrix4x4.TRS(transform.position,transform.rotation,sacle);
			Gizmos.DrawWireSphere(s.center,s.radius);
			return;
		}
	}
}
