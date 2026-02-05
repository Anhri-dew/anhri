using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    [Header("Particle References")]
    public ParticleSystem footDust;    // Пыль под ногами при беге
    public ParticleSystem jumpPuff;    // Эффект прыжка

    [Header("Trail")]
    public TrailRenderer speedTrail;   // След при быстром движении

    [Header("Settings")]
    public float dustMinSpeed = 1f;    // Минимальная скорость для пыли
    public float trailMinSpeed = 4f;   // Минимальная скорость для следа

    private Rigidbody rb;
    private bool isGrounded = true;
    private ParticleSystem.EmissionModule dustEmission;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Настраиваем эмиссию пыли
        if (footDust != null)
        {
            dustEmission = footDust.emission;
            dustEmission.rateOverTime = 0f;
            dustEmission.rateOverDistance = 0f; // Начинаем с выключенной
            footDust.Stop();
        }

        // Настраиваем след
        if (speedTrail != null)
        {
            speedTrail.emitting = false;
        }
    }

    void Update()
    {
        CheckGroundStatus();
        UpdateDustEffect();
        UpdateTrailEffect();
    }

    void CheckGroundStatus()
    {
        // Простая проверка на землю
        bool newGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            1.1f
        );

        // Если только что приземлились
        if (!isGrounded && newGrounded)
        {
            PlayLandingEffect();
        }

        // Если только что оторвались от земли
        if (isGrounded && !newGrounded)
        {
            PlayJumpEffect();
        }

        isGrounded = newGrounded;
    }

    void UpdateDustEffect()
    {
        if (footDust == null) return;

        // Вычисляем горизонтальную скорость
        Vector3 horizontalVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float speed = horizontalVel.magnitude;

        // Включаем/выключаем пыль
        if (isGrounded && speed > dustMinSpeed)
        {
            // Бежим по земле - включаем пыль
            dustEmission.rateOverDistance = 25f;

            if (!footDust.isPlaying)
            {
                footDust.Play();
            }
        }
        else
        {
            // Стоим или в воздухе - выключаем пыль
            dustEmission.rateOverDistance = 0f;

            if (footDust.isPlaying)
            {
                footDust.Stop();
            }
        }
    }

    void UpdateTrailEffect()
    {
        if (speedTrail == null) return;

        float speed = rb.velocity.magnitude;
        bool shouldEmitTrail = speed > trailMinSpeed || !isGrounded;

        speedTrail.emitting = shouldEmitTrail;

        // Меняем цвет следа в зависимости от скорости
        if (shouldEmitTrail)
        {
            UpdateTrailColor(speed);
        }
    }

    void UpdateTrailColor(float speed)
    {
        Gradient gradient = new Gradient();

        // Цвет от синего к красному
        float t = Mathf.Clamp01((speed - trailMinSpeed) / 10f);
        Color trailColor = Color.Lerp(
            new Color(0.2f, 0.5f, 1f, 0.8f), // Синий
            new Color(1f, 0.3f, 0.2f, 0.8f), // Красный
            t
        );

        gradient.colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(trailColor, 0f),
            new GradientColorKey(trailColor, 1f)
        };

        gradient.alphaKeys = new GradientAlphaKey[]
        {
            new GradientAlphaKey(0.8f, 0f),
            new GradientAlphaKey(0f, 1f)
        };

        speedTrail.colorGradient = gradient;
    }

    void PlayJumpEffect()
    {
        if (jumpPuff != null)
        {
            jumpPuff.Play();
        }
    }

    void PlayLandingEffect()
    {
        // Можно воспроизвести jumpPuff или создать отдельный эффект
        if (jumpPuff != null)
        {
            jumpPuff.Play();
        }
    }

    // Методы для других скриптов
    public void PlayDamageEffect()
    {
        // Красная вспышка
        Debug.Log("Damage VFX");
    }

    public void PlayDeathEffect()
    {
        // Останавливаем все эффекты
        if (footDust != null && footDust.isPlaying)
        {
            footDust.Stop();
        }

        if (speedTrail != null)
        {
            speedTrail.emitting = false;
        }

        Debug.Log("Death VFX");
    }
}