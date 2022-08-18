using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BzKovSoft.ObjectSlicer;
using System.Diagnostics;
using DG.Tweening;
using MoreMountains.NiceVibrations;

namespace BzKovSoft.ObjectSlicerSamples
{
	/// <summary>
	/// This script will invoke slice method of IBzSliceableAsync interface if knife slices this GameObject.
	/// The script must be attached to a GameObject that have rigidbody on it and
	/// IBzSliceable implementation in one of its parent.
	/// </summary>
	[DisallowMultipleComponent]
	public class FruitNinjaSlice : MonoBehaviour
	{
		[SerializeField]
		public IBzSliceableAsync _sliceableAsync;

		Vector3 startPos;
		Vector3 endPos;

		int _sliceId = 0;

		public Transform enemy;
		Transform sword;

		private GameObject trail;

		AttackMoveController amc;

		public LayerMask mask;
		float timer = 0;
		float sliceDelay = 0.15f;

		public GameObject sliceParticle;

		public bool activateOnlyOnEnemyTouch = false;

		AudioSource sliceSource;

        private void Awake()
        {
			sliceSource = GetComponent<AudioSource>();
			amc = FindObjectOfType<AttackMoveController>();
			trail = FindObjectOfType<TrailRenderer>().gameObject;
			_sliceableAsync = FindObjectOfType<ObjectSlicerSample>();
			//enemy = FindObjectOfType<ObjectSlicerSample>().transform.parent;
			sword = transform.Find("Sword");
		}

        void Start()
		{
			trail.SetActive(false);
			sword.gameObject.SetActive(false);
		}

        private void Update()
        {
			if (GameManager.Instance.State == GameState.Menu || GameManager.Instance.State == GameState.Victory) return;

			if (Input.GetMouseButtonDown(0))
			{
				startPos = FindRelativePos();

			}
			if (Input.GetMouseButton(0))
            {
				timer -= Time.deltaTime;
				trail.SetActive(true);
				endPos = FindRelativePos();
				trail.transform.position = endPos;

				if (!(Vector3.Distance(startPos, FindRelativePos()) > 0.5f) || timer > 0)
					return;
				RaycastHit hit;
				if (activateOnlyOnEnemyTouch && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10))
                {
					if (hit.transform.gameObject.layer != 13)
                    {
						return;
                    }
                }

				timer = sliceDelay;
				SendRay(startPos, endPos);
				startPos = endPos;
            }
			if (Input.GetMouseButtonUp(0))
            {
				trail.SetActive(false);
			}
		}

		private Vector3 FindRelativePos()
        {
			if (amc.enemyToKill == null) return Vector3.zero;

			enemy = amc.enemyToKill.transform;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			var point = ray.origin + (ray.direction * Mathf.Abs(enemy.position.z - Camera.main.transform.position.z));
			return point;
		}

		private void SendRay(Vector3 start, Vector3 end,int timesCalled = 0)
        {
			Vector3 e = end;
			Vector3 s = start;
			if (timesCalled != 2)
            {
				e.z = enemy.position.z;
				s.z = enemy.position.z;
			}

			Ray ray = new Ray(s, e-s);
			UnityEngine.Debug.DrawRay(s, e - s, Color.cyan, 10);
			RaycastHit[] hits = Physics.RaycastAll(ray, 100f, mask);

			++_sliceId;
			
			bool sentSomething = false;

			for (int i = 0; i < hits.Length; i++)
			{
				IBzSliceable sliceable = hits[i].transform.GetComponentInParent<IBzSliceable>();
				IBzSliceableAsync sliceableA = hits[i].transform.GetComponentInParent<IBzSliceableAsync>();

				Vector3 direction = Vector3.Cross(e, s).normalized;
				//UnityEngine.Debug.Log("Slice dir: " + direction);
				direction.z = 0;
				Plane plane = new Plane(direction, ray.origin);
				//UnityEngine.Debug.Log("My clicable plane: " + plane);

				DrawPlaneNearPoint(plane, ray.origin, 100, Color.blue);

				if (sliceable != null)
                {
					var part = Instantiate(sliceParticle, hits[i].point, sliceParticle.transform.rotation);
					Destroy(part, 1);
					sliceable.Slice(plane);
				}

				if (sliceableA != null)
                {
					var part = Instantiate(sliceParticle, hits[i].point, sliceParticle.transform.rotation);
					Destroy(part, 1);
					sliceableA.Slice(plane, _sliceId, null);
				}

				if (sliceable != null || sliceableA != null)
                {
					SwingSword(s,e);
					sentSomething = true;
				}
			}
			if (!sentSomething && timesCalled == 0)
            {
				SendRay(start - (end - start).normalized * 3, e, 1);
			}
			if (!sentSomething && timesCalled == 1)
			{
				start += new Vector3(0, 0, 0.5f);
				end += new Vector3(0, 0, 0.5f);
				SendRay(start - (end - start).normalized * 3, end, 2);
			}

			/*
			if (calledAgain)
				DrawSpheres(s, e, Vector3.Cross(e,s).normalized);
			*/
		}

		private void SwingSword(Vector3 s, Vector3 e)
        {
			var direction = (e - s).normalized;
			MMVibrationManager.Haptic(HapticTypes.Selection);

			sliceSource.Play();
			sword.DOKill();
			sword.transform.position = s - direction * 1;
			//sword.DOLookAt(e, 0.1f);
			sword.gameObject.SetActive(true);
			sword.DOMove(e + direction * 3, 0.45f).OnComplete(()=> sword.gameObject.SetActive(false));
		}

		//DEBUGS

		void DrawSpheres(Vector3 s, Vector3 e, Vector3 n)
        {
			var start = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			start.transform.position = s;
			start.GetComponent<Renderer>().material.color = Color.green;
			start.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			start.name = "start";

			var end = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			end.transform.position = e;
			end.GetComponent<Renderer>().material.color = Color.red;
			end.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			end.name = "end";

			var norm = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			norm.transform.position = n;
			norm.GetComponent<Renderer>().material.color = Color.blue;
			norm.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			norm.name = "norm";
		}


		// Mimics Debug.DrawLine, drawing a plane containing the 3 provided worldspace points,
		// with the visualization centered on the centroid of the triangle they form.
		public static void DrawPlane(Vector3 a, Vector3 b, Vector3 c, float size,
			Color color, float duration = 3f, bool depthTest = true)
		{

			var plane = new Plane(a, b, c);
			var centroid = (a + b + c) / 3f;

			DrawPlaneAtPoint(plane, centroid, size, color, duration, depthTest);
		}

		// Draws the portion of the plane closest to the provided point, 
		// with an altitude line colour-coding whether the point is in front (cyan)
		// or behind (red) the provided plane.
		public static void DrawPlaneNearPoint(Plane plane, Vector3 point, float size, Color color, float duration = 3f, bool depthTest = true)
		{
			var closest = plane.ClosestPointOnPlane(point);
			Color side = plane.GetSide(point) ? Color.cyan : Color.red;
			UnityEngine.Debug.DrawLine(point, closest, side, duration, depthTest);

			DrawPlaneAtPoint(plane, closest, size, color, duration, depthTest);
		}

		// Non-public method to do the heavy lifting of drawing the grid of a given plane segment.
		static void DrawPlaneAtPoint(Plane plane, Vector3 center, float size, Color color, float duration, bool depthTest)
		{
			var basis = Quaternion.LookRotation(plane.normal);
			var scale = Vector3.one * size / 10f;

			var right = Vector3.Scale(basis * Vector3.right, scale);
			var up = Vector3.Scale(basis * Vector3.up, scale);

			for (int i = -5; i <= 5; i++)
			{
				UnityEngine.Debug.DrawLine(center + right * i - up * 5, center + right * i + up * 5, color, duration, depthTest);
				UnityEngine.Debug.DrawLine(center + up * i - right * 5, center + up * i + right * 5, color, duration, depthTest);
			}
		}

	}
}