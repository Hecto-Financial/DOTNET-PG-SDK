using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class receiveNoti : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        /** 설정 정보 저장 */
        SettleUtil util = SettleUtil.Instance;
        String LICENSE_KEY = util.LICENSE_KEY; //라이센스키
        String LOG_FILE = util.NOTI_LOG_FILE;  //로그파일명

        /** 노티 처리 결과 */
        bool resp = false;

        /** 노티 수신 파라미터 */
        String outStatCd        = String.IsNullOrEmpty(Request.Form["outStatCd"]) ? "" :        Request.Form["outStatCd"];
        String trdNo            = String.IsNullOrEmpty(Request.Form["trdNo"]) ? "" :            Request.Form["trdNo"];
        String method           = String.IsNullOrEmpty(Request.Form["method"]) ? "" :           Request.Form["method"];
        String bizType          = String.IsNullOrEmpty(Request.Form["bizType"]) ? "" :          Request.Form["bizType"];
        String mchtId           = String.IsNullOrEmpty(Request.Form["mchtId"]) ? "" :           Request.Form["mchtId"];
        String mchtTrdNo        = String.IsNullOrEmpty(Request.Form["mchtTrdNo"]) ? "" :        Request.Form["mchtTrdNo"];
        String mchtCustNm       = String.IsNullOrEmpty(Request.Form["mchtCustNm"]) ? "" :       Request.Form["mchtCustNm"];
        String mchtName         = String.IsNullOrEmpty(Request.Form["mchtName"]) ? "" :         Request.Form["mchtName"];
        String pmtprdNm         = String.IsNullOrEmpty(Request.Form["pmtprdNm"]) ? "" :         Request.Form["pmtprdNm"];
        String trdDtm           = String.IsNullOrEmpty(Request.Form["trdDtm"]) ? "" :           Request.Form["trdDtm"];
        String trdAmt           = String.IsNullOrEmpty(Request.Form["trdAmt"]) ? "" :           Request.Form["trdAmt"];
        String billKey          = String.IsNullOrEmpty(Request.Form["billKey"]) ? "" :          Request.Form["billKey"];
        String billKeyExpireDt  = String.IsNullOrEmpty(Request.Form["billKeyExpireDt"]) ? "" :  Request.Form["billKeyExpireDt"];
        String bankCd           = String.IsNullOrEmpty(Request.Form["bankCd"]) ? "" :           Request.Form["bankCd"];
        String bankNm           = String.IsNullOrEmpty(Request.Form["bankNm"]) ? "" :           Request.Form["bankNm"];
        String cardCd           = String.IsNullOrEmpty(Request.Form["cardCd"]) ? "" :           Request.Form["cardCd"];
        String cardNm           = String.IsNullOrEmpty(Request.Form["cardNm"]) ? "" :           Request.Form["cardNm"];
        String telecomCd        = String.IsNullOrEmpty(Request.Form["telecomCd"]) ? "" :        Request.Form["telecomCd"];
        String telecomNm        = String.IsNullOrEmpty(Request.Form["telecomNm"]) ? "" :        Request.Form["telecomNm"];
        String vAcntNo          = String.IsNullOrEmpty(Request.Form["vAcntNo"]) ? "" :          Request.Form["vAcntNo"];
        String expireDt         = String.IsNullOrEmpty(Request.Form["expireDt"]) ? "" :         Request.Form["expireDt"];
        String AcntPrintNm      = String.IsNullOrEmpty(Request.Form["AcntPrintNm"]) ? "" :      Request.Form["AcntPrintNm"];
        String dpstrNm          = String.IsNullOrEmpty(Request.Form["dpstrNm"]) ? "" :          Request.Form["dpstrNm"];
        String email            = String.IsNullOrEmpty(Request.Form["email"]) ? "" :            Request.Form["email"];
        String mchtCustId       = String.IsNullOrEmpty(Request.Form["mchtCustId"]) ? "" :       Request.Form["mchtCustId"];
        String cardNo           = String.IsNullOrEmpty(Request.Form["cardNo"]) ? "" :           Request.Form["cardNo"];
        String cardApprNo       = String.IsNullOrEmpty(Request.Form["cardApprNo"]) ? "" :       Request.Form["cardApprNo"];
        String instmtMon        = String.IsNullOrEmpty(Request.Form["instmtMon"]) ? "" :        Request.Form["instmtMon"];
        String instmtType       = String.IsNullOrEmpty(Request.Form["instmtType"]) ? "" :       Request.Form["instmtType"];
        String phoneNoEnc       = String.IsNullOrEmpty(Request.Form["phoneNoEnc"]) ? "" :       Request.Form["phoneNoEnc"];
        String orgTrdNo         = String.IsNullOrEmpty(Request.Form["orgTrdNo"]) ? "" :         Request.Form["orgTrdNo"];
        String orgTrdDt         = String.IsNullOrEmpty(Request.Form["orgTrdDt"]) ? "" :         Request.Form["orgTrdDt"];
        String mixTrdNo         = String.IsNullOrEmpty(Request.Form["mixTrdNo"]) ? "" :         Request.Form["mixTrdNo"];
        String mixTrdAmt        = String.IsNullOrEmpty(Request.Form["mixTrdAmt"]) ? "" :        Request.Form["mixTrdAmt"];
        String payAmt           = String.IsNullOrEmpty(Request.Form["payAmt"]) ? "" :           Request.Form["payAmt"];
        String csrcIssNo        = String.IsNullOrEmpty(Request.Form["csrcIssNo"]) ? "" :        Request.Form["csrcIssNo"];
        String cnclType         = String.IsNullOrEmpty(Request.Form["cnclType"]) ? "" :         Request.Form["cnclType"];
        String mchtParam        = String.IsNullOrEmpty(Request.Form["mchtParam"]) ? "" :        Request.Form["mchtParam"];
        String acntType          = String.IsNullOrEmpty(Request.Form["acntType"]) ? "" :          Request.Form["acntType"];
        String kkmAmt         = String.IsNullOrEmpty(Request.Form["kkmAmt"]) ? "" :          Request.Form["kkmAmt"];
        String coupAmt          = String.IsNullOrEmpty(Request.Form["coupAmt"]) ? "" :          Request.Form["coupAmt"];
        String pktHash          = String.IsNullOrEmpty(Request.Form["pktHash"]) ? "" :          Request.Form["pktHash"];

        /* 응답 파라미터 List에 저장 */
        Dictionary<String,String> noti = new Dictionary<String,String>
        {
            { "거래상태", outStatCd },
            { "거래번호", trdNo },
            { "결제수단", method },
            { "업무구분", bizType },
            { "상점아이디", mchtId },
            { "상점거래번호", mchtTrdNo },
            { "주문자명", mchtCustNm },
            { "상점한글명", mchtName },
            { "상품명", pmtprdNm },
            { "거래일시", trdDtm },
            { "거래금액", trdAmt },
            { "자동결제키", billKey },
            { "자동결제키 유효기간", billKeyExpireDt },
            { "은행코드", bankCd },
            { "은행명", bankNm },
            { "카드사코드", cardCd },
            { "카드명", cardNm },
            { "이통사코드", telecomCd },
            { "이통사명", telecomNm },
            { "가상계좌번호", vAcntNo },
            { "가상계좌 입금만료일시", expireDt },
            { "통장인자명", AcntPrintNm },
            { "입금자명", dpstrNm },
            { "고객이메일", email },
            { "상점고객아이디", mchtCustId },
            { "카드번호", cardNo },
            { "카드승인번호", cardApprNo },
            { "할부개월수", instmtMon },
            { "할부타입", instmtType },
            { "휴대폰번호(암호화)", phoneNoEnc },
            { "원거래번호", orgTrdNo },
            { "원거래일자", orgTrdDt },
            { "복합결제 거래번호", mixTrdNo },
            { "복합결제 금액", mixTrdAmt },
            { "실결제금액", payAmt },
            { "현금영수증 승인번호", csrcIssNo },
            { "취소거래타입", cnclType },
            { "기타주문정보", mchtParam },
            { "계좌구분", acntType },
            { "카카오머니 금액", kkmAmt },
            { "쿠폰 금액", coupAmt },
            { "해쉬값", pktHash } //서버에서 전달된 해쉬 값
        };

        /** 해쉬 조합 필드 
         *  결과코드 + 거래일시 + 상점아이디 + 가맹점거래번호 + 거래금액 + 라이센스키 */
        String hashPlain = outStatCd + trdDtm + mchtId + mchtTrdNo + trdAmt + LICENSE_KEY;
        String hashCipher = "";

        /** SHA256 해쉬 처리 */
        try
        {
            hashCipher = util.Sha256(hashPlain);//해쉬 값
        }
        catch (Exception ex)
        {
            util.LogMessage(LOG_FILE, "[" + mchtTrdNo + "][SHA256 HASHING] Hashing Fail! : " + ex.Message);
        }
        finally
        {
            util.LogMessage(LOG_FILE, "[" + mchtTrdNo + "][SHA256 HASHING] Plain Text[" + hashPlain + "] ---> Cipher Text[" + hashCipher + "]");
        }

        /**
            hash데이타값이 맞는 지 확인 하는 루틴은 헥토파이낸셜에서 받은 데이타가 맞는지 확인하는 것이므로 꼭 사용하셔야 합니다
            정상적인 결제 건임에도 불구하고 노티 페이지의 오류나 네트웍 문제 등으로 인한 hash 값의 오류가 발생할 수도 있습니다.
            그러므로 hash 오류건에 대해서는 오류 발생시 원인을 파악하여 즉시 수정 및 대처해 주셔야 합니다. 
            그리고 정상적으로 데이터를 처리한 경우에도 헥토파이낸셜에서 응답을 받지 못한 경우는 결제결과가 중복해서 나갈 수 있으므로 관련한 처리도 고려되어야 합니다
        */
        if (hashCipher == pktHash)//해쉬값 일치
        {
            util.LogMessage(LOG_FILE, "[" + mchtTrdNo + "][SHA256 Hash Check] hashCipher[" + hashCipher + "] pktHash[" + pktHash + "] equals?[TRUE]");
            if ("0021" == outStatCd )//결제 성공
            {
                util.LogMessage(LOG_FILE, "[" + mchtTrdNo + "][Success] params:" + String.Join("", noti));
                resp = util.NotiSuccess(noti);
            }
            else if ("0051"==outStatCd )//가상계좌 채번 성공
            {
                util.LogMessage(LOG_FILE, "[" + mchtTrdNo + "][Wait For Deposit] params:" + String.Join("", noti));
                resp = util.NotiWaitingPay(noti);
            }
            else
            {
                util.LogMessage(LOG_FILE, "[" + mchtTrdNo + "][Undefined Code] outStatCd:" + outStatCd);
                resp = false;
            }
        }
        else //해쉬값 불일치
        {
            util.LogMessage(LOG_FILE, "[" + mchtTrdNo + "][SHA256 Hash Check] hashCipher[" + hashCipher + "] pktHash[" + pktHash + "] equals?[FALSE]");
            resp = util.NotiHashError(noti);
        }

        // OK, FAIL문자열은 헥토파이낸셜로 전송되어야 하는 값이므로 변경하거나 삭제하지마십시오.
        if (resp)
        {
            Response.Write("OK");
            util.LogMessage(LOG_FILE, "[" + mchtTrdNo + "][Result] OK");
        }
        else
        {
            Response.Write("FAIL");
            util.LogMessage(LOG_FILE, "[" + mchtTrdNo + "][Result] FAIL");
        }

    }
}