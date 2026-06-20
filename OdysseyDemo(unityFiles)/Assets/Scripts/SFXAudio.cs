using UnityEngine;

public class SFXAudio : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip healthSound;
    public AudioClip lifeSound;
    public AudioClip coinSound;
    public AudioClip damageSound;
    public AudioClip enemyDamageSound;
    public AudioClip jumpSound;
    public AudioClip collectibleSound;
    public AudioClip sirenShootSound;
    public AudioClip swordSlashSound;
    public AudioClip puzzleCompleteSound;
    public AudioClip arrowSound;
    public AudioClip deathSound;
    public AudioClip clockSound;
    public AudioClip boulderMovingSound;

    public AudioClip pigSound;
    public AudioClip crouchSound;
    public AudioClip switchSound;
    public AudioClip checkpointSound;

    public void PlayHealth() => audioSource.PlayOneShot(healthSound);
    public void PlayLife() => audioSource.PlayOneShot(lifeSound);
    public void PlayCoin() => audioSource.PlayOneShot(coinSound);
    public void PlayDamage() => audioSource.PlayOneShot(damageSound);
    public void PlayEnemyDamage() => audioSource.PlayOneShot(enemyDamageSound);
    public void PlayJump() => audioSource.PlayOneShot(jumpSound);
    public void PlayCollectible() => audioSource.PlayOneShot(collectibleSound);
    public void PlaySirenShoot() => audioSource.PlayOneShot(sirenShootSound);
    public void PlaySwordSlash() => audioSource.PlayOneShot(swordSlashSound);
    public void PlayPuzzleComplete() => audioSource.PlayOneShot(puzzleCompleteSound);
    public void PlayArrow() => audioSource.PlayOneShot(arrowSound);
    public void PlayDeath() => audioSource.PlayOneShot(deathSound);
    public void PlayClock() => audioSource.PlayOneShot(clockSound);
    public void PlayBoulderMoving() => audioSource.PlayOneShot(boulderMovingSound);

    public void PlayPig() => audioSource.PlayOneShot(pigSound);
    public void PlayCrouch() => audioSource.PlayOneShot(crouchSound);
    public void PlaySwitch() => audioSource.PlayOneShot(switchSound);
    public void PlayCheckpoint() => audioSource.PlayOneShot(checkpointSound);
}