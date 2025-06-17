using System.Collections;
using UnityEngine;

public class OctopusLegController : MonoBehaviour
{
    public void Init(float speed, Vector3 direction)
    {
        StartCoroutine(MoveLeg(speed, direction));
    }

    IEnumerator MoveLeg(float speed, Vector3 direction)
    {
        float duration = 1f;
        float time = 0f;

        while (time < duration)
        {
            transform.position += direction * speed * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CollisionComponent collisionComponent = collision.transform.GetComponentInParent<CollisionComponent>();

        if (!collisionComponent) return;

        if (collisionComponent.Contains(CollisionType.IsPlayer))
        {
            SoundManager.Instance.PlaySE("HitOctopusLeg");
            LogManager.Instance.Register(LogType.Octpus);

            // ランダムな足画像を表示
            OctopusUIManager.Instance.ShowRandomLegImage();
        }

        if (collisionComponent.Contains(CollisionType.DestroyEnemyBullet)) Destroy(gameObject);
    }
}
