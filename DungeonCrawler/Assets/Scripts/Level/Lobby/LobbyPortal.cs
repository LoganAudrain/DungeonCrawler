using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LobbyPortal : MonoBehaviour
{
    [SerializeField] float Timer = 5;
    [SerializeField] float ShakeDuration = 0.01f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(ScreenShake(Timer, ShakeDuration));
        StartCoroutine(LoadLevel());

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        StopAllCoroutines();
    }

    private IEnumerator LoadLevel()
    {
        yield return new WaitForSeconds(Timer);
        SceneManager.LoadScene(2);
    }

    private IEnumerator ScreenShake(float duration, float magnitude)
    {
        Transform camTransform = Camera.main.transform;
        Vector3 originalPos = camTransform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            camTransform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        camTransform.localPosition = originalPos;
    }
}
