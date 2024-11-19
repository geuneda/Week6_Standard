# Week6_Standard
 학습을 위한 repository

# Q1

## OX퀴즈

### Q : 앵커와 피벗은 같은 기능을 한다.
#### A : X

### Q : 피벗을 왼쪽 상단으로 설정하면, UI 요소는 화면의 왼쪽 상단을 기준으로 위치가 고정된다.
#### A : X

### Q : 피벗을 UI 요소의 중심에 설정하면, 회전 시 UI 요소가 중심을 기준으로 회전한다.
#### A : O

## 생각해보기(주관식)

### Q : 게임의 상단바와 같이 화면에 특정 영역에 꽉 차게 구성되는 UI와 화면의 특정 영역에 특정한 크기로 등장하는 UI의 앵커 구성이 어떻게 다른지 설명해보세요.
#### A : 보통 상단바라면 앵커를 상단에 고정하고, 좌우로 stretch를 선택할 것 같습니다. 특정 영역에 특정한 크기라는말이 이해가 잘 안되는데, 우측하단을 기준으로 한다면 앵커를 우측하단으로 설정하고 원하는 만큼 Pos를 조정할 것 같습니다.

### Q : 돌아다니는 몬스터의 HP 바와 늘 고정되어있는 플레이어의 HP바는 Canvas 컴포넌트의 어떤 설정이 달라질 지 생각해보세요
#### A : RenderMode를 몬스터의 HP바는 WorldSpace 플레이어는 ScreenSpace를 기준으로 설정할것 같습니다.

## 확장문제 : GameManager.cs에 구현

# Q2

## OX퀴즈

### Q : 코루틴은 비동기 작업을 처리하기 위해 사용된다.
#### A : X (절대적으로는 동기적이지만 비동기적 방식의 흉내 ?이긴 함)

### Q : yield return new WaitForSeconds(1);는 코루틴을 1초 동안 대기시킨다.
#### A : X (1초가 지날때까지 매 프레임 return을 시키기에 대기라고 보기 애매함)

### Q : 코루틴은 void를 반환하는 메소드의 형태로 구현된다.
#### A : X

## 생각해보기(주관식)

### Q : 코루틴을 이미 실행중이라면 추가로 실행하지 않으려면 어떻게 처리해주면 될까요?
#### A : 코루틴을 변수에 저장하고 메서드로 작동중인지 확인을 하며 사용하거나 bool값을 이용하여 제어하면 됩니다.

### Q : 코루틴 실행 중 게임오브젝트가 파괴되더라도 코루틴의 실행이 정상적으로 지속될까요?
#### A : 안됩니다.

## 확장문제 : GameManager.cs의 155 line 참고
