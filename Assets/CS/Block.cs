using UnityEngine;

public class Block: MonoBehaviour {
    private static readonly Vector3[] direction = new Vector3[]{
        Vector3.right,
        Vector3.forward,
        Vector3.left,
        Vector3.back
    };  // 우측이동 기준 [x변화량, z변화량]
    private readonly double[] moveTime = new double[] { 0, 0, 0 };     // 이동 조작 속도조절용 시간, [좌, 하, 우]
    private readonly bool[] pushed = new bool[] { true, true, true, true };     // 반복입력 방지할 조작용 bool, [좌, 우, 드랍, 회전]
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

        //  안눌렀을 경우 타이머 초기화 (사용자 연타 허용)
        if(!left) {
            moveTime[0] = 0;
        }
        if(!right) {
            moveTime[2] = 0;
        }

        // 상태값 갱신
        pushed[0] = left;
        pushed[1] = right;
        pushed[2] = drop;
        pushed[3] = rotate;
    }

    #region 블럭 조작 함수

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
        // 현재 좌표에서 회전이 불가능할 때 -> 회전한 상태에서 양쪽으로 이동이 가능한지 체크 후 불가능하면 회전 안함,
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


    #region 블럭 시스템 함수
    private bool VaildMove() {  // 현재 좌표에 이동해도 이상이 없는지 체크하는 함수
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

    private void PlaceBlock() {     // 바닥에 닿은 블럭을 보드에 설치하는 함수
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

    private void ClearLine() {  // 블럭이 있는 라인을 지우는 로직
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
    private bool IsEmpty(int y) {   // 해당 Y좌표에 블럭이 없는지 판별
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
    private bool IsEmptyDirection(int y) {  // 해당 Y좌표에서 해당 방향에 블럭이 없는지
        for(int p = 0; p < GameManager.width; p++) {
            if(GameManager.rotate % 2 == 0) {   // x축에 대한 면인가
                if(GameManager.board[p, y, GameManager.rotate == 0 ? 0 : GameManager.width - 1] == null) {
                    return true;
                }
            }
            else {      // y축에 관한 면인가
                if(GameManager.board[GameManager.rotate == 1 ? GameManager.width - 1 : 0, y, p] == null) {
                    return true;
                }
            }
        }
        return false;
    }
    private void DeleteFloor(int y) {    // 보드에서 블럭을 제거
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
    private void FloorDown(int y) {   // 삭제된 줄 위의 블럭들을 아래로 내리는 함수
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
