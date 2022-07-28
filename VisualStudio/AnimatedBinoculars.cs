extern alias Hinterland;
using Hinterland;
using ModComponent.Utils;
using UnityEngine;

namespace Binoculars
{
	public class AnimatedBinoculars
	{
		private float originalFOV;
		private UITexture? texture;
		private GameObject? arms;
		private GameObject? binoculars;
		private GameObject? strap;
		Animator? anim;
		float a = 0.5f;
		float f = 0.0f;
		int i = 0;
		bool runIdleCoroutine = true;
		private static readonly string[] states = new string[] { "offscreen", "bring", "putDown", "idleRare", "idleTree", "ready", "lensZoom", "lensZoom_back", "ready_back" };
		bool isZoomed = false;
		ModComponent.API.Components.ModBaseEquippableComponent? EquippableModComponent;

		public void OnPrimaryAction()
		{
			if(anim != null)
            {
				anim.SetTrigger("idle_twiddle");
			}
		}

		public void OnSecondaryAction()
		{
			if (isZoomed)
			{
				if (anim != null)
				{
					anim.SetBool("active", false);
				}
				isZoomed = false;
			}
			else
			{
				if (anim != null)
				{
					anim.SetBool("active", true);
				}
				isZoomed = true;
			}
		}

		public void OnEquipped()
		{
			runIdleCoroutine = true;
			anim = EquippableModComponent?.EquippedModel?.GetComponent<Animator>();
			InitializeGameObjects();
			ShowButtonPopups();
			if (anim != null)
			{
				anim.SetTrigger("bring");
			}

			MelonLoader.MelonCoroutines.Start(InvokeRepeating(IdleFluc, 0.0f, 0.1f));
		}

		public void OnUnequipped()
		{
			runIdleCoroutine = false;
			isZoomed = false;
			EndZoom();
			if (anim != null)
			{
				anim.SetTrigger("put_down");
			}
		}

		public void OnControlModeChangedWhileEquipped()
		{
			runIdleCoroutine = false;
			isZoomed = false;
			EndZoom();
			if (anim != null)
			{
				anim.SetTrigger("put_down");
			}
		}

		System.Collections.IEnumerator InvokeRepeating(System.Action action, float delay, float interval)
		{
			yield return new WaitForSecondsRealtime(delay);
			while (runIdleCoroutine)
			{
				action.Invoke();
				yield return new WaitForSecondsRealtime(interval);
			}
			yield return null;
		}

		void IdleFluc()
		{
			if (anim == null)
			{
				return;
			}

			if (f < 1.0f)
			{
				a += i == 0 ? 0.03f : -0.03f; // add if i is 0, otherwise substract

				if (a > 0.9f)
				{
					i = 1;
				}

				if (a < 0.1f)
				{
					i = 0;
				}

				anim.SetFloat("idle_random", a);

				f += 0.1f;
			}
			else
			{
				i = UnityEngine.Random.Range(0, 2);
				f = 0;
			}
		}

		void OnUpdate()
		{
			try
			{
				if (anim == null || anim.runtimeAnimatorController == null)
				{
					return;
				}

				anim.GetAnimatorStateInfo(0, StateInfoIndex.CurrentState, out AnimatorStateInfo info);

				isZoomed = info.IsName("lensZoom") && runIdleCoroutine;

				if (isZoomed)
				{
					StartZoom();
				}
				else
				{
					EndZoom();
				}
			}
			catch { }
		}

		private void InitializeGameObjects()
		{
			if (EquippableModComponent == null || EquippableModComponent.EquippedModel == null)
			{
				return;
			}

			switch (PlayerUtils.GetPlayerGender())
			{
				case PlayerGender.Female:
					arms = EquippableModComponent.EquippedModel.transform.FindChild("readingArmsF")?.gameObject;
					EquippableModComponent.EquippedModel.transform.FindChild("readingArmsM")?.gameObject.SetActive(false);
					break;
				case PlayerGender.Male:
					EquippableModComponent.EquippedModel.transform.FindChild("readingArmsF")?.gameObject.SetActive(false);
					arms = EquippableModComponent.EquippedModel.transform.FindChild("readingArmsM")?.gameObject;
					break;
			}
			binoculars = EquippableModComponent.EquippedModel.transform.FindChild("dev_binoculars_hudShape_1")?.gameObject;
			strap = EquippableModComponent.EquippedModel.transform.FindChild("strap")?.gameObject;
		}

		private void SetVisibility(bool visible)
		{
			if (binoculars != null && arms != null && strap != null)
			{
				binoculars.SetActive(visible);
				arms.SetActive(visible);
				strap.SetActive(visible);
			}
		}

		private static void ShowButtonPopups()
		{
			EquipItemPopupUtils.ShowItemPopups("Idle", Localization.Get("GAMEPLAY_Use"), false, false, true);
		}

		private void StartZoom()
		{
			if (!GameManager.GetVpFPSCamera().IsZoomed)
			{
				PlayerUtils.FreezePlayer();
				ZoomCamera();
				ShowOverlay();
				SetVisibility(false);
			}
		}

		private void EndZoom()
		{
			if (GameManager.GetVpFPSCamera().IsZoomed)
			{
				PlayerUtils.UnfreezePlayer();
				RestoreCamera();
				HideOverlay();
				SetVisibility(true);
			}
		}

		private void HideOverlay()
		{
			if (texture != null)
			{
				Object.Destroy(texture);
				texture = null;
			}
		}

		private void RestoreCamera()
		{
			var camera = GameManager.GetVpFPSCamera();
			camera.ToggleZoom(false);
			camera.SetFOVFromOptions(originalFOV);
		}

		private void ShowOverlay()
		{
			texture = UIUtils.CreateOverlay(Resources.Load("Binoculars_Overlay")?.Cast<Texture2D>());
		}

		private void ZoomCamera()
		{
			vp_FPSCamera camera = GameManager.GetVpFPSCamera();
			originalFOV = camera.m_RenderingFieldOfView;
			camera.ToggleZoom(true);
			camera.SetFOVFromOptions(originalFOV * BinocularsSettings.Instance.GetFovScalar());
		}
	}
}
