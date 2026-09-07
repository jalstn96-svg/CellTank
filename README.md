# Cell Tank

Unity와 C#으로 제작한 2D Top View 모듈형 탱크 슈팅 게임입니다.

적을 처치하고 획득한 Cell을 Core에 직접 부착하여 전차의 공격, 방어, 이동 성능을 변경하는 구조를 구현했습니다.

## Project

- 개발 기간: 2026.07.13 ~ 2026.07.31
- 개발 인원: 1인
- Engine: Unity
- Language: C#

## 주요 구현

### Cell System

`TankCell` 추상 클래스를 기반으로 Armor, Engine, Turret 등의 Cell을 상속 구조로 구성했습니다.

Cell의 배치는 다음 구조를 이용해서 관리했습니다.

- `Dictionary<Vector2Int, TankCell>`: Grid 위치와 장착된 Cell 관리
- `HashSet<Vector2Int>`: 장착 가능한 위치 및 연결 검사 관리

Cell 제거 이후에는 Core를 기준으로 상하좌우 방향을 재귀 검색하여 연결이 끊어진 Cell을 판별합니다.

### Projectile Interaction

포탄은 `Physics2D.Raycast`를 이용해 충돌을 검사하고, 충돌 대상은 `Ihittable` 인터페이스를 통해 처리합니다.

```text
Bullet
  ↓
Physics2D.Raycast
  ↓
Ihittable.Hit()
  ↓
ProjectileHitResult
```

충돌 결과를 Passed, Hitted, Penetrated, Ricochet, Immuned 상태로 구분하여 포탄의 후속 동작을 결정합니다.

### Armor Penetration
Armor Cell에는 포탄의 진행 방향과 충돌면의 Normal Vector를 이용한 도탄 및 관통 판정을 적용했습니다.

- Dot Product 기반 입사각 계산
- 입사각에 따른 실질 장갑 두께 계산
- 관통 후 남은 Penetration / Power 계산
- 일정 각도 이상에서 강제 도탄 처리

단순한 고정 피해가 아닌 포탄의 방향과 Armor 배치가 전투 결과에 영향을 주도록 구현했습니다.

### Object Pooling
Enemy와 Bullet의 반복적인 생성 및 제거 비용을 줄이기 위해 Object Pooling을 적용했습니다.

`Dictionary<string, Queue<GameObject>>`

Pooling 적용 이후 Enemy를 재사용할 때 이전 HP, Cell 상태, Rigidbody 값이 남는 문제가 발생하여 OnEnable() 및 별도의 Reset 로직에서 런타임 상태를 초기화하도록 수정했습니다.

### Enemy Management

Enemy Editor에서 Cell을 직접 배치한 Enemy를 Prefab으로 저장하여 사용합니다.

```text
Enemy Editor
    ↓
EnemyPrefabSaver
    ↓
Enemy_Vx_y Prefab
    ↓
EnemyPool
    ↓
MobSpawner
```

Alert 단계와 Variation에 따라 필요한 Enemy Prefab을 Pool에서 호출하도록 구성했습니다.

### Game State

게임 흐름은 GameState enum을 기준으로 관리합니다.

```text
MainMenu
Playing
MaintenanceCall
Maintenance
Pause
GameOver
GameClear
```

현재는 GameManager에서 상태 전이를 직접 관리하는 구조이며, 프로젝트를 회고하며 상태 규모가 커질 경우 FSM 형태로 분리할 수 있는 부분을 검토했습니다.

### 기술 요소
- Unity / C#
- Abstract Class / Interface
- Interface
- Dictionary / HashSet / Queue
- Physics2D.Raycast
- Dot Product
- Object Pooling

### Note

본 저장소에는 프로젝트 실행 및 확인을 위한 빌드 파일이 포함되어 있습니다.

게임 내 사운드 리소스는 외부 에셋을 사용했습니다.




