## 1. 핵심 흐름
```
MirrorPlacer(입력) -> MirrorController / Mirror (거울 추가, 이동, 회전)

Laser -> LaserPath.Compute() -> IReflective -> Mirror / Receiver

Receiver (OnActivationChanged) -> SuccessHit

Laser, MirrorPlacer -> UiManager -> Canvas Text
```

## 2. 클래스별 역할
- **Laser**: `_muzzle` 위치/방향을 매 프레임 `LaserPath.Compute()`에 넘기고, 반환된 점 목록을 `LineRenderer`로 그림. 경로는 매 프레임 처음부터 다시 계산, 반사 경로가 실시간으로 반영되며 Receiver의 자동 복귀도 성립
- **LaserPath**: `Physics.Raycast`로 충돌 판정, 충돌 대상이 `IReflective`면 `OnLaserHit()`을 호출해 반사. 10회 반사 시 반사종료
- **IReflective / LaserHitResult**: `Mirror`, `Receiver`가 각각 구현, 새로운 상호작용 오브젝트를 추가해도 `Laser`/`LaserPath` 코드는 수정할 필요 없음
- **Mirror**: 정반사 계산(`Vector3.Reflect`)
- **MirrorController**: 개별 거울 인스턴스의 Transform 조작 (`Select`/`Deselect`/`MoveTo`/`Spin`/`Tilt`)
- **MirrorPlacer**: 배치/선택/드래그 상태 머신, 그라운드 Raycast, 회전 단축키
- **Receiver**: `LateUpdate`에서 현재 프레임에서 충돌 여부와 직전 프레임 비교해, 상태가 바뀐 순간에만 `OnActivationChanged` 이벤트 발생
- **SuccessHit**: 리시버에 레이저 닿았을때 리시버와 레이저 색상 변경
- **UIManager**: 반사 횟수, 미러 갯수 값이 바뀐 경우에만 Canvas 텍스트 갱신

---

## 3. 조작 방법

| 동작 | 방법                                   |
|---|--------------------------------------|
| 거울 생성 | [Add Mirror] 버튼 클릭 -> 바닥 클릭 (최대 12개) |
| 거울 선택 | 생성된 거울을 클릭                           |
| 거울 이동 | 선택된 거울을 다시 클릭한 채로 드래그                |
| 거울 회전 | `Q` / `W`                            |
| 거울 기울이기 | `E` / `R`                            |
| 선택한 거울 삭제 | [Remove Mirror] 버튼                   |
| 전체 거울 삭제 | [Reset] 버튼                           |

거울은 클릭한 지점의 그라운드위에 생성
(레이저 쪽 그라운드에 놓으면 수평, 리시버 쪽의 기울어진 그라운드에 놓으면 수직으로 생성)

화면 상단에는 현재 반사 횟수(`반사횟수: n/10회`)와 배치된 거울 개수(`미러: n/12개`) 표시

---

## 4. 기능 구현 이유

- **설치 방식**: "버튼 -> 배치 대기 -> 클릭" 선택. 직관적이고 실제 게임 씬에서 바로 확인 가능
- **회전 조작**: 이동은 드래그, 회전/기울임은 키보드(Q/W/E/R)로 분리. 이동까지 키로 처리하면 위치 지정이 번거로워지고 회전이 모호해짐
- **거울 프리팹 구조**: 콜라이더 있는 실제 메시는 자식에, 배치/회전 피벗은 부모에 배치됨. 피벗을 거울의 "접지면"으로 고정해두면 표면 법선에 맞춰 회전 계산(`Quaternion.FromToRotation`) 성립
- **Spin/Tilt가 로컬 축 기준인 이유**: 씬이 완전한 평면이 아니라 약 70도 기울어진 그라운드로 나뉨. 기울어진 그라운드 위 거울을 월드 Y축 기준으로 돌리면 피벗은 고정된 채 몸체가 월드 수직축을 중심으로 원을 그리며 돌아 표면을 파고드는 문제 발생, 확인 후 로컬 축(Space.Self) 기준으로 수정
- **성공 연출 방식**: 리시버는 색상 그대로 변경, 레이저 LineRenderer는 Emission 색의 Hue만 바꾸고 채도/밝기 유지. 원래 색의 발광 강도까지 덮어쓰면 레이저가 발광하지 않아 잘 보이지 않음
- **거울 최대 개수 제한**: 반사 자체가 최대 10회로 제한되어 있어 거울이 그 이상이어도 반사하지 못하므로 여유만 두고 의미 없이 늘어나는 것을 막음 -> 짧은 시간에 대량 생성/파괴되는 상황이 아니라 풀링
- **UIManager 값이 바뀔 때만 갱신**: 값이 변경되지 않았으면 변경하지 않음

---

## 5. 요구사항 확인

- [x] 레이저가 첫 충돌 지점까지 그려짐
- [x] Mirror에 닿으면 정반사
- [x] 반사 최대 10회 제한
- [x] Receiver 상태 변화 + 닿지 않으면 자동 복귀
- [x] Mirror 플레이타임 중 설치 가능 (UI 버튼 + 클릭)
- [x] Mirror Position/Rotation 조작 가능 (드래그 + Q/W/E/R)

---

## 6. 테스트

코드용 asmdef(`Assets/Scripts/PreTest.Scripts.asmdef`), 테스트용 asmdef(`Assets/Tests/Tests.asmdef`, Editor 전용) 생성

- **대상**: `LaserPath.Compute()` -> 씬 구성 없이 콜라이더로 검증 가능
- **파일**: `Assets/Tests/LaserPathTest.cs`
- **검증 항목**
  - 충돌 없을 때 레이저 최대거리까지 그려지고 반사 없음
  - 미러 아닌 오브젝트에 맞으면 그 자리에서 레이저 종료
  - 미러 충돌시 레이저 반사 카운트 1 증가
  - 반사 10회 초과 시 반사 종료
- **실행**: `Window > General > Test Runner` > `EditMode` 탭 > `Run All`