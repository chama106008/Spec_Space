using UnityEngine;

public class StageClear : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject clearUI;   // Canvas（暗転+文字）などを丸ごと
    [SerializeField] private float timeScaleOnClear = 0f; // 0で停止、0.2でスロー等

    private bool cleared = false; //確認用

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (clearUI != null)
        {
            clearUI.SetActive(false); // 起動時は非表示
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clear(GameObject player)
    {
        if(!cleared)
        {
            cleared = true;
        }
        // 1) プレイヤー操作停止（代表的な止め方）
        // ここはあなたの操作スクリプト名に合わせる
        var controller = player.GetComponent<PlayerControl>();
        if (controller != null) controller.enabled = false;

        // 2) 速度も止める（物理で滑って動くのを防止）
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false; // 物理演算そのものを止める（必要なら）
        }

        // 3) UI表示（暗転 + Stage Clear!!）
        if (clearUI != null)
            clearUI.SetActive(true);

        // 4) 時間停止（演出方針次第）
        Time.timeScale = timeScaleOnClear;
    }
}
