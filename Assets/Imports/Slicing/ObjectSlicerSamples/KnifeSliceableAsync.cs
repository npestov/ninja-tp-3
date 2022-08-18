using UnityEngine;
using System.Collections;
using BzKovSoft.ObjectSlicer;
using System;

namespace BzKovSoft.ObjectSlicerSamples
{
	/// <summary>
	/// This script will invoke slice method of IBzSliceableAsync interface if knife slices this GameObject.
	/// The script must be attached to a GameObject that have rigidbody on it and
	/// IBzSliceable implementation in one of its parent.
	/// </summary>
	[DisallowMultipleComponent]
	public class KnifeSliceableAsync : MonoBehaviour
	{
		[HideInInspector]
		IBzSliceableAsync _sliceableAsync;
		bool slicebilityEnded;
		
		void Start()
		{
			_sliceableAsync = GetComponentInParent<IBzSliceableAsync>();
			//PrefabUtility.SaveAsPrefabAsset(transform.gameObject, "Assets/" + transform.name + ".prefab");
		}

		void OnTriggerEnter(Collider other)
		{
			
			var knife = other.gameObject.GetComponent<BzKnife>();
			var pm = other.gameObject.GetComponent<PlayerMovement>();

			if ((knife == null && pm == null) || transform.parent.name.Equals("BOSS") || !transform.parent.CompareTag("bonus"))
				return;

			if (pm == null)
				StartCoroutine(Slice(knife));
			else
				StartCoroutine(Slice(knife, true));
			
		}

		public void PmCallsSlice()
        {
			StartCoroutine(Slice(null));
		}

        private void OnCollisionEnter(Collision other)
        {
			/*
			var knife = other.gameObject.GetComponent<BzKnife>();
			var pm = other.gameObject.GetComponent<PlayerMovement>();

			if ((knife == null && pm == null) || transform.parent.name.Equals("BOSS"))
				return;

			if (pm == null)
				StartCoroutine(Slice(knife));
			else
				StartCoroutine(Slice(knife, true));
			*/
		}

        private IEnumerator Slice(BzKnife knife, bool forceSlice = false)
		{
			if (!transform.parent.CompareTag("bonus"))
				forceSlice = true;

			if (transform.parent.GetComponentsInChildren<Canvas>().Length == 1)
            {
				Destroy(transform.parent.GetComponentInChildren<Canvas>().gameObject);
            }

			slicebilityEnded = true;
			// The call from OnTriggerEnter, so some object positions are wrong.
			// We have to wait for next frame to work with correct values
			yield return null;
			if (knife == null)
				knife = FindObjectOfType<BzKnife>();
			Plane plane = new Plane();

			if (!forceSlice)
            {

				Vector3 point = GetCollisionPoint(knife);
				Vector3 normal = Vector3.Cross(knife.MoveDirection, knife.BladeDirection);
				plane = new Plane(normal, point);
			}
            else
            {
                try
                {
					Vector3 normal = knife.transform.eulerAngles.normalized;
					plane = new Plane(normal, transform.position + new Vector3(0, 2.2f, 0));
				}
                catch (Exception e)
                {
					//UnityEngine.Debug.Log("errror " + e);
					Vector3 normal = Vector3.up;
					plane = new Plane(normal, transform.position + new Vector3(0, 2.2f, 0));
				}
			}

			if (_sliceableAsync != null)
			{				
				int id = 1;
				//first call
				Debug.Log("SLicing " + transform.parent.name);
				if (forceSlice)
                {
					_sliceableAsync.Slice(plane, id, null);
				}
				else
					_sliceableAsync.Slice(plane, id, null);

				

			}
		}

		private Vector3 GetCollisionPoint(BzKnife knife)
		{
			Vector3 distToObject = transform.position - knife.Origin;
			Vector3 proj = Vector3.Project(distToObject, knife.BladeDirection);

			Vector3 collisionPoint = knife.Origin + proj;

			UnityEngine.Debug.Log("COLLISION POINT: " + collisionPoint);
			

			return collisionPoint;
		}

	}
}