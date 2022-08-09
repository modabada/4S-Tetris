using UnityEngine;

public class Spawner: MonoBehaviour {
    private static Spawner s;
    private Block target;
    private Transform player;
    private Transform board;
    private static float rotationTime = 10;
    private static float targetAngle;

    private void Start() {
        s = GetComponent<Spawner>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        board = GameObject.FindGameObjectWithTag("Board").transform;
        Spawn();
    }
    private void FixedUpdate() {
        if(rotationTime < 2) {
            player.RotateAround(board.transform.position, Vector3.up, -90 * Time.deltaTime / 2);
            transform.Rotate(0, -90 * Time.deltaTime / 2, 0);
        }
        else {
            if(player.rotation.eulerAngles[1] % 90 != 0 && false) {
                player.RotateAround(board.transform.position, Vector3.up, targetAngle - player.eulerAngles[1]);
                transform.Rotate(
                    0,
                    targetAngle - transform.eulerAngles[1],
                    0);
            }
            if(target.isPlace) {
                Spawn();
            }
        }
        rotationTime += Time.deltaTime;
    }
    private void Spawn() {
        GameObject spawnTarget = transform.GetChild(Random.Range(0, 7)).gameObject;
        Vector3 spawnPosition = Vector3Int.RoundToInt(spawnTarget.transform.position);
        Quaternion spawnQuat = transform.rotation;
        GameObject targetObj = Instantiate(spawnTarget, spawnPosition, spawnQuat, board);
        target = targetObj.GetComponent<Block>();
        targetObj.SetActive(true);
    }
    public static void Rotate() {
        GameManager.rotate = (GameManager.rotate + 1) % 4;
        Debug.Log(GameManager.rotate);
        rotationTime = 0;
        targetAngle = s.transform.rotation.eulerAngles[1] - 90;
        // targetAngle = s.transform.rotation * Quaternion.Euler(0, -90, 0);
    }
}
