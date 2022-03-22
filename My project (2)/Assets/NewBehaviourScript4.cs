// Fill out your copyright notice in the Description page of Project Settings.


#include "User_Character.h"
#include "User_Character_AnimInstance.h"
#include "Kismet/KismetMathLibrary.h"
// 헤더파일 포함

// Sets default values
AUser_Character::AUser_Character()
//:: class 선언
{
 	// Set this character to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	SpringArm = CreateDefaultSubobject <USpringArmComponent>(TEXT("SPRINGARM"));
	Camera = CreateDefaultSubobject <UCameraComponent>(TEXT("Camera"));
	User_Detect_Capsule = CreateDefaultSubobject <UCapsuleComponent>(TEXT("USER_DETECT_CAPSULE"));
	//컴포넌트 가져오기

	SpringArm->SetupAttachment(GetCapsuleComponent());
	User_Detect_Capsule->SetupAttachment(GetCapsuleComponent());
	Camera->SetupAttachment(SpringArm);

	SpringArm->TargetArmLength = 400.0f;
	SpringArm->SetRelativeRotation(FRotator::ZeroRotator);
	SpringArm->bUsePawnControlRotation = true;
	SpringArm->bInheritPitch = true;
	SpringArm->bInheritRoll = true;
	SpringArm->bInheritYaw = true;
	SpringArm->bDoCollisionTest = true;
	//SpringArm 카메라 3인칭시점으로 사용하기 위해 설정

	this->bUseControllerRotationYaw = false;

	GetCharacterMovement()->bOrientRotationToMovement = true;
	GetCharacterMovement()->RotationRate = FRotator(0.0f, 900.0f, 0.0f);
	GetCharacterMovement()->JumpZVelocity = 800.0f;
	// Capsules Setting
	GetCapsuleComponent()->SetCapsuleSize(30.0f, 90.0f);
	GetCapsuleComponent()->OnComponentHit.AddDynamic(this, &AUser_Character::OnHit);
	//캡슐 컴포넌트 설정

	User_Detect_Capsule->SetCapsuleHalfHeight(95.0f);
	User_Detect_Capsule->SetCapsuleRadius(35.0f);

	GetMesh()->SetRelativeLocation(FVector(0.0f, 0.0f, -90.0f));
	GetMesh()->SetRelativeRotation(FRotator(0.0f, -90.0f, 0.0f));

	//방향, 위치 설정

	//User Normal Var
	PlayerSpeed = 1.0f;
	WallTouch = false;
	Air_Dash_Now = false;
	WallRun_Now = false;
	//플레이어 움직임 및 상태 설정

	//Set Design of Character
	/// File : Game/GhostLady_S4/Meshes/Characters/Combines/SK_GLS4_B.SK_GLS4_B

	static ConstructorHelpers::FObjectFinder<USkeletalMesh> User_Character_SkelMesh_OF(TEXT("/Game/GhostLady_S4/Meshes/Characters/Combines/SK_GLS4_B.SK_GLS4_B"));
	//메쉬설정
	if (User_Character_SkelMesh_OF.Succeeded())
	//만약 캐릭터 메쉬설정이 되었다면
	{
		GetMesh()->SetSkeletalMesh(User_Character_SkelMesh_OF.Object);
		//캐릭터에 적용한 메쉬 설정
	}
	// File : AnimBlueprint'/Game/Self_Anim_BP/User_Anim_BP.User_Anim_BP'
	static ConstructorHelpers::FClassFinder<UAnimInstance> User_Character_AnimInstance_OF(TEXT("/Game/Self_Anim_BP/User_Anim_BP.User_Anim_BP_C"));
	//애니메이션 설정
	if (User_Character_AnimInstance_OF.Succeeded())
	//만약 캐릭터 애니메이션 설정이 되었다면
	{
		GetMesh()->SetAnimInstanceClass(User_Character_AnimInstance_OF.Class);
		//캐릭터에 적용한 애니메이션 메쉬설정
	}
}

// Called when the game starts or when spawned

void AUser_Character::PostInitializeComponents()
{
	Super::PostInitializeComponents();
	//Super:: 자식 클래스를 부모 클래스에서 가져올 때 
	//Link User AnimInstace for Something Call
	User_Character_AnimInstance = Cast<UUser_Character_AnimInstance>(GetMesh()->GetAnimInstance());
	User_Character_AnimInstance->OnMontageEnded.AddDynamic(this, &AUser_Character::End_Montage_Trail);
}
void AUser_Character::BeginPlay()
{
	Super::BeginPlay();
	//Super:: 
	//Play 시작?
}

// Called every frame
void AUser_Character::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	if (GetCharacterMovement()->IsFalling())
	//만약 캐릭터가 떨어지고 있다면
	{
		GetController()->SetIgnoreMoveInput(true);
		//움직임을 무시하는것을 true설정
	}
	else
	{
		GetController()->ResetIgnoreMoveInput();
		//아니라면 움직임 무시하는 설정 리셋 (false로 변경?)
	}
}

// Called to bind functionality to input
void AUser_Character::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);


	PlayerInputComponent->BindAxis(TEXT("CamDis"), this, &AUser_Character::Cam_Dis);
	PlayerInputComponent->BindAction(TEXT("Jump"), EInputEvent::IE_Pressed, this, &AUser_Character::User_Jump);
	//PlayerInputComponent : 함수 바인딩. "CamDis","Jump"함수 바인딩
	// 바인딩 된 함수 호출?

}

void AUser_Character::Cam_Dis(float NewAxisValue)
//카메라 기본거리 설정
{
	if (SpringArm->TargetArmLength <= 520.0f && SpringArm->TargetArmLength >= 220.0f)
	//3인칭 카메라의 기본거리가 520.0f보다 작거나 같고 220.0f보다 크거나 같다면
	{
		SpringArm->TargetArmLength += NewAxisValue;
		//기본거리에서 NewAxisValue추가하기
	}
	else if (SpringArm->TargetArmLength > 520.0f)
	//3인칭 카메라의 기본거리가 520.0f보다 크다면
	{
		SpringArm->TargetArmLength = 520.0f;
		//520.0f로 설정
	}
	else if (SpringArm->TargetArmLength < 220.0f)
	//220.0f보다 작다면
	{
		SpringArm->TargetArmLength = 220.0f;
		//220.0f로 설정
	}
}
void AUser_Character::User_Jump()
//점프 메서드
{
	if (GetCharacterMovement()->IsFalling() == false) //On Grounded - Normal Jump
	//떨어지고 있는 상태가 거짓일때= 떨어지고 있지 않을 때 (바닥에 닿아있을 때 기본점프)
	{
		LaunchCharacter(FVector(0.0f, 0.0f, 800.0f), false, false);
		//위치 변경
	}
	else if (Air_Dash_Now == false && WallTouch == false)// Eccelerate
	//떨어지고 있을 때 air Dash를 하고있지 않고 벽 터치를 하고 있지 않을 때
	{
		GetCharacterMovement()->StopMovementImmediately();
		//캐릭터 움직임을 순간적으로 멈추기
		User_Character_AnimInstance->PlayAirDashMontage();
		//airdash상태로 바꾸기
		LaunchCharacter((GetActorForwardVector() + FVector(0.0f, 0.0f, 0.5f)) * 1500.0f, false, false);
		//위치 변경
		Air_Dash_Now = true;
		//air dash를 true 상태로 바꾸기
		GetController()->SetIgnoreMoveInput(true);
	}
}
void AUser_Character::End_Montage_Trail(UAnimMontage* Montage, bool bInterrupted)
{
	UE_LOG(LogTemp, Error, TEXT("Trail Montage is Ended"));
	//몽타주가 끝났다..?
}

void AUser_Character::NotifyActorBeginOverlap(AActor* Other)
{
	UE_LOG(LogTemp, Warning, TEXT("Actor Overlap Acting : %s"), *Other->GetName());
	// if You Interrupt Something, Stop AirDash
	// 무언가에 방해를 받는다면 AirDash를 멈춘다
	User_Character_AnimInstance->StopAirDashMontage();
	//AirDash 멈춤
	Air_Dash_Now = false;
	//AirDash false로 변경

	if (Other->ActorHasTag("Floor")) // Floor Overlap Setting, OnGrounded
	//만약 ActorHasTag가 Floor이라면
	{
		User_Character_AnimInstance->StopAllMontages(0.35f);
		//0.35초동안 멈추기
	}
	//		GetCharacterMovement()->GravityScale = 0.5f;
	if (Other->ActorHasTag("Wall") && GetCharacterMovement()->IsFalling()) // Wall Launch Details
	//ActroHasTag가 Wall이면서 캐릭터가 떨어지고있다면
	{
		GetCharacterMovement()->GravityScale = 0.5f;
		//중력 변경
		LaunchCharacter((GetActorForwardVector() + FVector(0.0f, 0.0f, 0.2f)) * 500.0f, false, false);
		//위치변경

	}
}
void AUser_Character::NotifyActorEndOverlap(AActor* Other)
{
	UE_LOG(LogTemp, Warning, TEXT("Actor Overlap Ending Target : %s"), *Other->GetName());
	UE_LOG(LogTemp, Warning, TEXT("Notify End Overlap"));

	if (Other->ActorHasTag(TEXT("Wall")))
	//만약 Wall이라는 Tag를 가진것이 닿았다면
	{
		WallTouch = false;
		WallRun_Now = false;
		UE_LOG(LogTemp, Warning, TEXT("Wall Touch : False"));
		User_Character_AnimInstance->StopWallRun_Montage();
		//벽점프 및 벽터치 false
		GetCharacterMovement()->GravityScale = 1.0f;
		//중력 변경
	}
}
void AUser_Character::NotifyHit
(
	class UPrimitiveComponent* MyComp,
	AActor* Other,
	class UPrimitiveComponent* OtherComp,
	//UPrimitiveComponent : 충돌감지
	bool bSelfMoved,
	FVector HitLocation,
	FVector HitNormal,
	FVector NormalImpulse,
	//Hit 포지션
	const FHitResult& Hit
)
{
	//UE_LOG(LogTemp, Error, TEXT("Notify Hit Location : %s"), *(HitLocation-GetActorLocation()).ToString());
	float degree = Cal_Forward_Target_Degree(HitLocation);

	if (Other->ActorHasTag(TEXT("Wall")) && GetCharacterMovement()->IsFalling() &&WallTouch == false)
	//ActorHasTag가 Wall이면서 떨어지고있고 벽에는 닿지 않은 상태라면
	{	
		WallTouch = true;
		//WallTouch true로 변경 
		UE_LOG(LogTemp, Error, TEXT("Wall Dir By Character : %f"), degree);
		GetController()->SetIgnoreMoveInput(true);
		//SetActorRotation((FRotator(0.0f, (-90 - degree), 0.0f)));
		//AddActorLocalRotation((FRotator(0.0f, (90-degree), 0.0f)));
		if (degree > 1.0f)
		{
			if (degree > 90.0f)
			{
				degree -= -(90 - degree);
			}
			UE_LOG(LogTemp, Error, TEXT("Wall On Right"));
		
			User_Character_AnimInstance->PlayWallRun_R_Montage();
			AddActorLocalRotation((FRotator(0.0f, -(90-degree), 0.0f)));
			//SetActorRotation((FRotator(0.0f, -(90 - degree), 0.0f)));
		}
		else if (degree < -1.0f)
		{
			if (degree < -90.0f)
			{
				degree -= -(-90 - degree);
			}
			
			UE_LOG(LogTemp, Error, TEXT("Wall On Left"));
			User_Character_AnimInstance->PlayWallRun_L_Montage();
			AddActorLocalRotation((FRotator(0.0f, -(-90-degree), 0.0f)));
			//SetActorRotation((FRotator(0.0f, -(-90 - degree), 0.0f)));

		}
		else
		{
			UE_LOG(LogTemp, Error, TEXT("Wall On Forward"));
		}
	}
	// 각도에 따라 벽의 왼쪽, 오른쪽, 앞쪽 방향 설정

}
float AUser_Character::Cal_Forward_Target_Degree(FVector TargetLocation)
{
	FVector OwnerLocation = GetActorLocation();
	FVector ToTargetVec = (TargetLocation - OwnerLocation);
	ToTargetVec *= FVector(1.f, 1.f, 0.f);
	ToTargetVec.Normalize();

	//타겟 설정 및 정규화

	FVector OwnerForwardVec = GetActorForwardVector();
	float InnerProduct = FVector::DotProduct(OwnerForwardVec, ToTargetVec);
	float TargetDegree = UKismetMathLibrary::DegAcos(InnerProduct);
	// UKismetMathLibrary : 방향 벡터를 회전값으로 전환
	
	FVector OutterProduct = FVector::CrossProduct(OwnerForwardVec, ToTargetVec);
	float DegSign = UKismetMathLibrary::SignOfFloat(OutterProduct.Z);

	float Result = TargetDegree * DegSign;
	//float degree = (Other->GetActorLocation() - this->GetActorLocation()).Rotation().Yaw;


	return Result;
}
// User Another Function
