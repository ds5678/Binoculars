using ModComponentMapper;
using UnityEngine;

namespace Binoculars
{
    public class AnimatedBinoculars
    {
        private float originalFOV;
        private UITexture texture;
        private GameObject arms;
        private GameObject binoculars;
        private GameObject strap;
        Animator anim;
        float a = 0.5f;
        float f = 0.0f;
        int i = 0;
        bool showingModel = true;
        bool runIdleCoroutine = true;
        private static readonly string[] states = new string[] { "offscreen", "bring", "putDown", "idleRare", "idleTree", "ready", "lensZoom", "lensZoom_back", "ready_back" };
        bool isZoomed = false;
		ModComponentAPI.EquippableModComponent EquippableModComponent;

        public void OnPrimaryAction()
		{
            anim?.SetTrigger("idle_twiddle");

        }

        public void OnSecondaryAction()
        {
            if (isZoomed)
			{
                anim?.SetBool("active", false);
                isZoomed = false;
            }
            else
			{
                anim?.SetBool("active", true);
                isZoomed = true;
            }
        }

        public void OnEquipped()
        {
            runIdleCoroutine = true;
            anim = EquippableModComponent?.EquippedModel?.GetComponent<Animator>();
            InitializeGameObjects();
            ShowButtonPopups();
            anim?.SetTrigger("bring");
            
            MelonLoader.MelonCoroutines.Start(InvokeRepeating(IdleFluc, 0.0f, 0.1f));
        }

        public void OnUnequipped()
        {
            runIdleCoroutine = false;
            isZoomed = false;
            EndZoom();
            anim?.SetTrigger("put_down");
        }

        public void OnControlModeChangedWhileEquipped()
        {
            runIdleCoroutine = false;
            isZoomed = false;
            EndZoom();
            anim?.SetTrigger("put_down");
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
            if (anim is null) return;

            if (f < 1.0f)
            {
                a += i == 0 ? 0.03f : -0.03f; // add if i is 0, otherwise substract

                if (a > 0.9f) i = 1;
                if (a < 0.1f) i = 0;

                anim?.SetFloat("idle_random", a);

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
                if (anim?.runtimeAnimatorController is null) return;
                anim.GetAnimatorStateInfo(0, StateInfoIndex.CurrentState, out AnimatorStateInfo info);

                if (info.IsName("lensZoom") && runIdleCoroutine) isZoomed = true;
                else isZoomed = false;

                if (isZoomed) StartZoom();
                else EndZoom();
            }
			catch { return; }
        }

        private void InitializeGameObjects()
		{
            if (EquippableModComponent?.EquippedModel is null) return;
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
            if (binoculars is null || arms is null || strap is null) return;
            binoculars.SetActive(visible);
            arms.SetActive(visible);
            strap.SetActive(visible);
            showingModel = visible;
        }

        private static void ShowButtonPopups()
        {
            EquipItemPopupUtils.ShowItemPopups("Idle", Localization.Get("GAMEPLAY_Use"), false, false, true);
        }

        private void StartZoom()
        {
            if (GameManager.GetVpFPSCamera().IsZoomed) return;

            PlayerUtils.FreezePlayer();
            ZoomCamera();
            ShowOverlay();
            SetVisibility(false);
        }

        private void EndZoom()
        {
            if (!GameManager.GetVpFPSCamera().IsZoomed) return;
            //MelonLoader.MelonLogger.Log("Actually end zoom");
            PlayerUtils.UnfreezePlayer();
            RestoreCamera();
            HideOverlay();
            SetVisibility(true);
            //MelonLoader.MelonLogger.Log("Finished ending zoom");
        }

        private void HideOverlay()
        {
            if (texture == null) return;

            Object.Destroy(texture);
            texture = null;
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
            camera.SetFOVFromOptions(originalFOV * 0.1f);
        }
    }
}
