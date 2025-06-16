# Lion-Survive
+ 몰려오는 몬스터들을 처치하고 성장하여 오랜 시간 버텨나가는 뱀서 라이크 장르의 게임입니다.

# 개발 기간
+ 25/04/28 ~ 25/06/15

# 팀원
+ 박종한, 윤시환, 조준형, 이규대, 김도윤, 김종현, 장윤상

# 담당 업무
1. 비동기적 길 찾기 시스템   
  BFS, A* 알고리즘과 UniTask, Semaphore를 이용한 비동기식 길 찾기 시스템 구현

2. Observer Pattern을 활용한 중앙 데이터 관리 구조
  정의된 게임 데이터들을 중앙에서 Observer Pattern으로 관리해 UI, 게임 로직 등 객체들을 분리

3. 행동 트리 기반의 몬스터 구조  
  행동 트리를 활용해 매 프레임 어떤 행동을 취할지 평가

# 비동기적 길 찾기 시스템
### 코드 위치 : [비동기 PathFinding 관련 클래스](https://github.com/MunHwaDong/Lion-Survive/tree/main/%5BNew%5D%20Enemy/PathFinding/AsyncPathFinding)
### 코드 위치 : [PathFinding을 요청하는 Action Node](https://github.com/MunHwaDong/Lion-Survive/blob/main/%5BNew%5D%20Enemy/BehaviourTree/PathFindingMovementActionNode.cs)

# Observer Pattern을 활용한 중앙 데이터 관리 구조
### 코드 위치 : [DataController.cs 및 Container들](https://github.com/MunHwaDong/Lion-Survive/tree/main/Data)

# 행동 트리 기반의 몬스터 구조
### 코드 위치 : [Selector, Sequence, Action 노드들](https://github.com/MunHwaDong/Lion-Survive/tree/main/%5BNew%5D%20Enemy/BehaviourTree)
