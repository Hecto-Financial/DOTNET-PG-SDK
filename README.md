# DOTNET-PG-SDK

헥토파이낸셜 PG 연동을 위한 .NET 샘플 코드입니다.

## 파일 구조

```
/(Project Root)
│
│  index.html			<--- index페이지
│
│  packages.config		<--- .NET 패키지 설정 파일
│  Web.config			<--- .NET 웹 설정 파일
│
│  pay_form.aspx			<--- 결제시 메인 폼
│  pay_encryptParams.aspx		<--- 결제시 파라미터 암호화 및 해쉬 처리 페이지
│  pay_encryptParams.aspx.cs	<--- 결제시 파라미터 암호화 및 해쉬 처리 페이지(코드페이지)
│  pay_autoPayResult.aspx		<--- 휴대폰 자동결제 결과 페이지
│  pay_autoPayResult.aspx.cs	<--- 휴대폰 자동결제 결과 페이지(코드페이지)
│  pay_receiveResult.aspx		<--- 결제 완료 후 응답파라미터 수신페이지
│  pay_showResult.aspx		<--- 자식페이지에서 전달된 응답파라미터 출력
│
│  cardReg_form.aspx		<--- 신용카드 0원 등록(빌키발급) 결제창 호출 폼
│  billkey_pay_form.aspx		<--- 신용카드 빌키 결제(2회차) 요청 폼
│  billkey_pay_showResult.aspx	<--- 신용카드 빌키 결제 처리 및 결과 화면
│  billkey_pay_showResult.aspx.cs	<--- 신용카드 빌키 결제 처리 및 결과 화면(코드페이지)
│
│  cancel_form.aspx		<--- 취소 메인 폼
│  cancel_showResult.aspx		<--- 취소 처리 및 결과 화면
│  cancel_showResult.aspx.cs	<--- 취소 처리 및 결과 화면(코드페이지)
│
│  receiveNoti.aspx		<--- 결제 완료 후 노티 수신 페이지			
│  receiveNoti.aspx.cs		<--- 결제 완료 후 노티 수신 페이지(코드페이지)
│
├─App_Code
│      SettleUtil.cs			<--- 헥토파이낸셜 유틸리티 클래스
│
└─Bin				<--- 의존성 패키지 위치(JSON)
        Newtonsoft.Json.dll
        Newtonsoft.Json.dll.refresh
        Newtonsoft.Json.xml
```

## 📄 파일 설명

### 🔧 공통 페이지
- **index.html**: 인덱스 페이지입니다.
- **SettleUtil.cs**: 상점아이디, 암복호화키 등을 설정하며, 유틸성 함수가 들어있는 유틸 클래스입니다.
- **receiveNoti.aspx**: 결제 또는 취소 처리가 완료된 후, 헥토파이낸셜에서 가맹점으로 전달하는 노티(결과통보)를 수신하는 페이지입니다. 전달받은 노티를 활용하여 가맹점의 실제 내부데이터, DB를 처리하시면 됩니다.

### 💳 결제 관련 페이지
- **pay_form.aspx**: 결제 요청 시 사용자로부터 정보를 입력받는 Form 페이지입니다. 결제는 Form POST 방식으로 처리됩니다.
- **pay_encryptParams.aspx**: pay_form.aspx에서 암호화가 필요한 파라미터들을 AJAX 통신으로 암호화하는 페이지입니다. 또한 SHA256 해시 처리도 수행합니다.
- **pay_receiveResult.aspx**: 결제창에서 결제가 완료된 이후 닫기 버튼을 누를 때, 헥토파이낸셜로부터 응답 파라미터를 수신하는 페이지입니다.
- **pay_showResult.aspx**: pay_receiveResult.aspx에서 받은 파라미터를 부모창으로 전송할 수 있는데, 이때 전송된 파라미터들을 수신하여 출력하는 페이지입니다.
- **pay_autoPayResult.aspx**: 휴대폰 자동연장결제 시 사용되는 결제 및 결과 페이지입니다.

### 🔁 신용카드 빌키(정기결제) 관련 페이지
- **cardReg_form.aspx**: 신용카드 **0원 등록(빌키발급) 결제창**을 호출하는 폼입니다. 실제 결제(승인) 없이 0원 인증으로 카드를 등록하고 정기결제용 빌키(`billKey`)를 발급받습니다. JS SDK의 `method="card"` + `methodSub="auth"` 조합으로 빌키발급창(`/card/cardAuth.do`)을 띄우며, `pay_encryptParams.aspx`와 `pay_receiveResult.aspx`를 재사용합니다.
- **billkey_pay_form.aspx**: 발급받은 빌키로 2회차 결제를 요청하는 폼입니다. 결제수단(`method=CA`), 업무구분(`bizType=B0`)은 신용카드 빌키 결제 고정값입니다.
- **billkey_pay_showResult.aspx**: 게이트웨이(`/spay/APICardActionPay.do`)와 Server to Server(JSON) 통신으로 빌키 결제를 요청하고 결과를 출력합니다. 빌키 결제 전용이며 카드번호 직접 입력(수기결제)은 지원하지 않습니다.

> **신용카드 빌키 발급 정책**: 신용카드 빌키는 **UI 결제창(0원 등록)** 방식을 표준으로 제공합니다(`cardReg_form.aspx`). 가맹점이 카드번호를 직접 수집해 API로 빌키를 발급하는 방식(비/구인증 수기)은 표준 제공 범위에 포함되지 않으며, 필요한 경우 영업 담당자 또는 기술지원(pgsupport@hecto.co.kr)을 통해 별도 제공됩니다.

### ❌ 취소 관련 페이지
- **cancel_form.aspx**: 취소 요청 시 사용자로부터 정보를 입력받는 Form 페이지입니다.
- **cancel_showResult.aspx**: 헥토파이낸셜과 Server to Server로 커넥션하여, 취소 요청을 하고 응답을 받아 결과를 출력하는 페이지입니다.

## 🔄 프로세스 처리 순서

- **결제 처리 순서**: pay_form.aspx → pay_encryptParams.aspx → pay_receiveResult.aspx → pay_showResult.aspx
- **휴대폰 자동연장 결제**: pay_form.aspx → pay_autoPayResult.aspx
- **신용카드 0원 등록(빌키발급)**: cardReg_form.aspx → pay_encryptParams.aspx → (빌키발급 결제창) → pay_receiveResult.aspx(응답의 billKey 보관) → pay_showResult.aspx
- **신용카드 빌키 결제(2회차)**: billkey_pay_form.aspx → billkey_pay_showResult.aspx
- **취소 처리 순서**: cancel_form.aspx → cancel_showResult.aspx
- **노티 처리 순서**: receiveNoti.aspx

## ⚙️ SettleUtil.cs 설정 파일 변수 설명

- **PG_MID**: 상점아이디. 테스트환경에서의 상점아이디는 샘플소스에 기재되어 있습니다. 상용테스트 시에는 헥토파이낸셜에서 발급한 MID로 설정하셔야 합니다. 이 값은 외부에 노출되어서는 안됩니다.
- **LICENSE_KEY**: MID당 하나의 라이센스키가 발급됩니다. SHA256 해시체크 용도로 사용됩니다. 이 값은 외부에 노출되어서는 안됩니다.
- **AES256_KEY**: 개인정보/민감정보를 암복호화하는데 사용되는 키로서, 외부에 노출되어서는 안됩니다.
- **PAYMENT_SERVER**: 헥토파이낸셜 결제 처리 서버의 URL입니다. 변경하지 마십시오.
- **CANCEL_SERVER**: 헥토파이낸셜 취소 처리 서버의 URL입니다. 변경하지 마십시오.
- **TIMEOUT**: 헥토파이낸셜 API 통신 연결 타임아웃입니다.
- **LOG_DIR**: 로그파일이 생성되는 디렉터리(없으면 로그파일 생성되지 않습니다.)
- **LOG_FILE**: 일반 거래에 대한 로그 파일명
- **NOTI_LOG_FILE**: 노티 관련 로그 파일명

## 📢 노티 수신 페이지

- **파일명**: receiveNoti.aspx
- 결제 또는 취소 완료 후 헥토파이낸셜 서버에서 콜백으로 호출하게 되는 페이지이며, 헥토파이낸셜에서 가맹점으로 노티를 전송합니다.
- nextUrl(결과페이지)에서는 성공/실패에 대한 결과 화면을 고객에게 리턴하여 주시고,
- notiUrl(노티수신페이지)에서는 가맹점의 실제 내부데이터, DB를 처리하시면 됩니다.

## 문의

- 기술 문의: pgsupport@hecto.co.kr
- 개발 가이드: [헥토파이낸셜 개발자 센터](https://developers.hectofinancial.co.kr)
