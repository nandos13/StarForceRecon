using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RootMotion.FinalIK;
using StarForceRecon;

public class CharacterOnDeath : MonoBehaviour
{
    private const float deathDuration = 3.0f;
    private const float preFailScreenTime = 1.0f;
    private const float failScreenTime = 1.2f;

    [SerializeField]
    private Sprite vignetteImage = null;

    private static Coroutine deathCoroutine = null;

    Health health;

    public Canvas DeathCanvasPrefab;

    private bool IsDead = false;

    void Awake()
    {
        Health h = GetComponentInParent<Health>();
        if (!h) h = GetComponentInChildren<Health>();

        if (h)
            h.OnDeath += OnDeath;

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            if (rb.gameObject == this.gameObject)
                continue;

            rb.isKinematic = true;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            if (col.gameObject == this.gameObject)
                col.enabled = true;
            else
                col.enabled = false;
        }
    }

    private void OnDeath(Health sender, float damageValue)
    {
        IsDead = true;

        ThirdPersonController tpc = GetComponent<ThirdPersonController>();
        tpc.enabled = false;

        AimIK aimIK = GetComponent<AimIK>();
        aimIK.enabled = false;

        GunCreator gc = GetComponent<GunCreator>();
        gc.enabled = false;

        AimHandler aimH = GetComponent<AimHandler>();
        aimH.enabled = false;

        Health hp = GetComponent<Health>();
        hp.enabled = false;

        Equipment eq = GetComponent<Equipment>();
        eq.enabled = false;

        Animator anim = GetComponentInChildren<Animator>();
        anim.enabled = false;

        PlayerController pc = GetComponent<PlayerController>();
        pc.enabled = false;

        SquadFollow sf = GetComponent<SquadFollow>();
        sf.enabled = false;

        SquadShooterAI ssAI = GetComponent<SquadShooterAI>();
        ssAI.enabled = false;
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (var col in colliders)
        {
            if (col.gameObject == this.gameObject)
                col.enabled = false;
            else
                col.enabled = true;
        }

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            if (rb.gameObject == this.gameObject)
                rb.isKinematic = true;
            else
                rb.isKinematic = false;
        }

       

        CameraController cameraController = FindObjectOfType<CameraController>();
        cameraController.enabled = false;

        SquadManager.CanSwitchSquadMembers = false;

        if (deathCoroutine == null)
            deathCoroutine = StartCoroutine(deathTimer());
    }

    private IEnumerator deathTimer()
    {
        // Stop being able to pause the game
        K_Pause pauser = FindObjectOfType<K_Pause>();
        if (pauser)
            Destroy(pauser);

        // Clone the vignette image
        UnityEngine.UI.Image vignette = null;
        vignette = CreateVignette();

        // Wait for time and slow down timescale
        float elapsed = 0;
        while (elapsed < deathDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float normalized = Mathf.Clamp01(elapsed / deathDuration);
            Time.timeScale = 1.0f - normalized;

            if (vignette != null)
            {
                Color vignetteColor = vignette.color;
                vignetteColor.a = normalized;
                vignette.color = vignetteColor;
            }

            yield return null;
        }

        yield return new WaitForSecondsRealtime(preFailScreenTime);

        GameOver();

        yield return new WaitForSecondsRealtime(failScreenTime);

        deathCoroutine = null;

        SceneManager.LoadScene("Menu");
    }

    private UnityEngine.UI.Image CreateVignette()
    {
        if (vignetteImage == null)
            return null;

        GameObject canvasObject = new GameObject("Vignette");
        canvasObject.hideFlags = HideFlags.HideAndDontSave;

        GameObject vignetteObject = new GameObject("Sprite");
        vignetteObject.transform.parent = canvasObject.transform;

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        UnityEngine.UI.CanvasScaler scaler = canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        UnityEngine.UI.Image image = vignetteObject.AddComponent<UnityEngine.UI.Image>();
        image.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        image.rectTransform.anchorMin = new Vector2(0, 0);
        image.rectTransform.anchorMax = new Vector2(1, 1);

        image.sprite = vignetteImage;

        return image;
    }

    void GameOver()
    {
        Canvas canvas = GameObject.Instantiate(DeathCanvasPrefab);
        canvas.enabled = true;
        canvas.gameObject.SetActive(true);

        StarForceRecon.PlayerController.DisableCursor();
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }
}
