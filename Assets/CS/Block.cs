using UnityEngine;

public class Block: MonoBehaviour {
    private static readonly Vector3[] direction = new Vector3[]{
        Vector3.right,
        Vector3.forward,
        Vector3.left,
        Vector3.back
    };  // �����̵� ���� [x��ȭ��, z��ȭ��]
    private readonly double[] moveTime = new double[] { 0, 0, 0 };     // �̵� ���� �ӵ������� �ð�, [��, ��, ��]
    private readonly bool[] pushed = new bool[] { true, true, true, true };     // �ݺ��Է� ������ ���ۿ� bool, [��, ��, ���, ȸ��]
    public bool isPlace = false;

    private void Update() {
        if(isPlace) {
            if(transform.childCount == 0) {
                Destroy(gameObject);
            }
            return;
        }

        bool drop = Input.GetKey(KeyCode.Space);
        bool rotate = Input.GetKey(KeyCode.UpArrow);
        bool left = Input.GetKey(KeyCode.LeftArrow);
        bool right = Input.GetKey(KeyCode.RightArrow);

        if(Time.time - moveTime[1] > (Input.GetKey(KeyCode.DownArrow) ? 1 / GameManager.speed / 12 : 1 / GameManager.speed)) {
            moveTime[1] = Time.time;
            MoveDown();
        }
        else if(left && (!pushed[0] || Time.time - moveTime[0] > 0.2)) {
            moveTime[0] = Time.time;
            MoveLeft();
        }
        else if(right && (!pushed[1] || Time.time - moveTime[2] > 0.2)) {
            moveTime[2] = Time.time;
            MoveRight();
        }

        else if(drop && !pushed[2]) {
            while(!isPlace) {
                MoveDown();
            }
        }
        else if(rotate && !pushed[3]) {
            Rotation();
        }

        //  �ȴ����� ��� Ÿ�̸� �ʱ�ȭ (����� ��Ÿ ���)
        if(!left) {
            moveTime[0] = 0;
        }
        if(!right) {
            moveTime[2] = 0;
        }

        // ���°� ����
        pushed[0] = left;
        pushed[1] = right;
        pushed[2] = drop;
        pushed[3] = rotate;
    }

    #region �� ���� �Լ�

    private void MoveDown() {
        transform.position += new Vector3(0, -1, 0);
        if(!VaildMove()) {
            transform.position += new Vector3(0, 1, 0);
            PlaceBlock();
            ClearLine();
        }
        moveTime[1] = Time.time;
    }
    private void MoveLeft() {
        Vector3 delta = direction[GameManager.rotate];
        transform.position -= delta;
        if(!VaildMove()) {
            transform.position += delta;
        }
        transform.position = Vector3Int.RoundToInt(transform.position);
    }
    private void MoveRight() {
        Vector3 delta = direction[GameManager.rotate];
        transform.position += delta;
        if(!VaildMove()) {
            transform.position -= delta;
        }
        transform.position = Vector3Int.RoundToInt(transform.position);
    }
    private void Rotation() {
        // ���� ��ǥ���� ȸ���� �Ұ����� �� -> ȸ���� ���¿��� �������� �̵��� �������� üũ �� �Ұ����ϸ� ȸ�� ����,
        transform.Rotate(0, 0, 90);
        if(!VaildMove()) {
            for(int i = 0; i < 2; i++) {
                if(i == 1)
                    transform.position += new Vector3(0, 1, 0);
                Vector3 prev_vector = transform.position;

                MoveLeft();
                if(prev_vector != transform.position)
                    return;
                MoveRight();
                if(prev_vector != transform.position)
                    return;
            }
            transform.position -= new Vector3(0, 1, 0);
            transform.Rotate(0, 0, -90);
        }
    }

    #endregion


    #region �� �ý��� �Լ�
    private bool VaildMove() {  // ���� ��ǥ�� �̵��ص� �̻��� ������ üũ�ϴ� �Լ�
        foreach(Transform child in transform) {
            int x = Mathf.RoundToInt(child.position.x);
            int y = Mathf.RoundToInt(child.position.y);
            int z = Mathf.RoundToInt(child.position.z);
            if(x < 0 || x >= GameManager.width || z < 0 || z >= GameManager.width) {
                return false;
            }
            if(y < 0 || y >= GameManager.height) {
                return false;
            }
            if(GameManager.board[x, y, z] != null) {
                return false;
            }
        }
        return true;
    }

    private void PlaceBlock() {     // �ٴڿ� ���� ���� ���忡 ��ġ�ϴ� �Լ�
        foreach(Transform child in transform) {
            int x = Mathf.RoundToInt(child.position.x);
            int y = Mathf.RoundToInt(child.position.y);
            int z = Mathf.RoundToInt(child.position.z);
            GameManager.board[x, y, z] = child;
            if(y > 22) {
                GameManager.GameOver();
            }
        }
        isPlace = true;
    }

    private void ClearLine() {  // ���� �ִ� ������ ����� ����
        foreach(Transform child in transform) {
            int y = Mathf.RoundToInt(child.position.y);
            if(!IsEmptyDirection(y)) {
                if(!IsEmpty(y)) {
                    DeleteFloor(y);
                    FloorDown(y);
                    return;
                }
                else {
                    Spawner.Rotate();
                    return;
                }
            }
        }
    }
    private bool IsEmpty(int y) {   // �ش� Y��ǥ�� ���� ������ �Ǻ�
        /// cases       rotate      direction
        /// n,  y, 0     0            1,  0
        /// n,  y, 10    2           -1,  0
        /// 10, y, n     1            0,  1
        /// 0,  y, n     3            0, -1
        for(int p = 0; p < GameManager.width - 1; p++) {
            if(GameManager.board[p, y, 0] == null) {
                return true;
            }
            if(GameManager.board[p, y, GameManager.width - 1] == null) {
                return true;
            }
            if(GameManager.board[0, y, p] == null) {
                return true;
            }
            if(GameManager.board[GameManager.width - 1, y, p] == null) {
                return true;
            }
        }
        return false;
    }
    private bool IsEmptyDirection(int y) {  // �ش� Y��ǥ���� �ش� ���⿡ ���� ������
        for(int p = 0; p < GameManager.width; p++) {
            if(GameManager.rotate % 2 == 0) {   // x�࿡ ���� ���ΰ�
                if(GameManager.board[p, y, GameManager.rotate == 0 ? 0 : GameManager.width - 1] == null) {
                    return true;
                }
            }
            else {      // y�࿡ ���� ���ΰ�
                if(GameManager.board[GameManager.rotate == 1 ? GameManager.width - 1 : 0, y, p] == null) {
                    return true;
                }
            }
        }
        return false;
    }
    private void DeleteFloor(int y) {    // ���忡�� ���� ����
        GameManager.score += 10 * Mathf.Pow(GameManager.width, 3);
        for(int p1 = 0; p1 < GameManager.width; p1 += GameManager.width - 1) {
            for(int p2 = 0; p2 < GameManager.width; p2++) {
                if(GameManager.board[p1, y, p2]) {
                    Destroy(GameManager.board[p1, y, p2].gameObject);
                    GameManager.board[p1, y, p2] = null;
                }
                if(GameManager.board[p2, y, p1]) {
                    Destroy(GameManager.board[p2, y, p1].gameObject);
                    GameManager.board[p2, y, p1] = null;
                }
            }
        }
    }
    private void FloorDown(int y) {   // ������ �� ���� ������ �Ʒ��� ������ �Լ�
        for(int d_y = y; d_y < GameManager.height; d_y++) {
            for(int p1 = 0; p1 < GameManager.width; p1 += GameManager.width - 1) {
                for(int p2 = 0; p2 < GameManager.width; p2++) {
                    if(GameManager.board[p1, d_y, p2]) {
                        GameManager.board[p1, d_y - 1, p2] = GameManager.board[p1, d_y, p2];
                        GameManager.board[p1, d_y, p2] = null;
                        GameManager.board[p1, d_y - 1, p2].transform.position += new Vector3(0, -1, 0);
                    }
                    if(GameManager.board[p2, d_y, p1]) {
                        GameManager.board[p2, d_y - 1, p1] = GameManager.board[p2, d_y, p1];
                        GameManager.board[p2, d_y, p1] = null;
                        GameManager.board[p2, d_y - 1, p1].transform.position += new Vector3(0, -1, 0);
                    }

                }
            }
        }
    }
    #endregion
}
