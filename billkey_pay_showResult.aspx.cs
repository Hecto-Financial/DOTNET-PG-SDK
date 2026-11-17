using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// 신용카드 빌키 결제 API 결과 처리 (2회차 결제)
/// - 호출 경로: /spay/APICardActionPay.do (게이트웨이, JSON, UI 없음 / 서버 간 통신)
/// - 빌키 결제 전용 샘플입니다. 카드번호 직접 입력(수기결제)은 지원하지 않습니다.
/// </summary>
public partial class billkey_pay_showResult : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //설정 정보 가져오기
        SettleUtil util = SettleUtil.Instance;
        String AES256_KEY = util.AES256_KEY;       //AES256 암복호화 키
        String LICENSE_KEY = util.LICENSE_KEY;     //라이센스 키
        String CANCEL_SERVER = util.CANCEL_SERVER; //게이트웨이(API) 서버 = 취소/빌키결제 공통 호스트(tbgw/gw)
        String LOG_FILE = util.LOG_FILE;           //로그파일명
        int TIMEOUT = util.TIMEOUT;                //타임아웃

        //요청 파라미터(헤더)
        Dictionary<String, String> REQ_HEADER = new Dictionary<String, String>
        {
            { "mchtId",     String.IsNullOrEmpty(Request.Form["mchtId"]) ? "" :     Request.Form["mchtId"] },       //상점아이디
            { "ver",        String.IsNullOrEmpty(Request.Form["ver"]) ? "" :        Request.Form["ver"] },          //버전
            { "method",     String.IsNullOrEmpty(Request.Form["method"]) ? "" :     Request.Form["method"] },       //결제수단(CA)
            { "bizType",    String.IsNullOrEmpty(Request.Form["bizType"]) ? "" :    Request.Form["bizType"] },      //업무구분(B0)
            { "encCd",      String.IsNullOrEmpty(Request.Form["encCd"]) ? "" :      Request.Form["encCd"] },        //암호화구분
            { "mchtTrdNo",  String.IsNullOrEmpty(Request.Form["mchtTrdNo"]) ? "" :  Request.Form["mchtTrdNo"] },    //상점주문번호
            { "trdDt",      String.IsNullOrEmpty(Request.Form["trdDt"]) ? "" :      Request.Form["trdDt"] },        //요청일자
            { "trdTm",      String.IsNullOrEmpty(Request.Form["trdTm"]) ? "" :      Request.Form["trdTm"] },        //요청시간
            { "mobileYn",   String.IsNullOrEmpty(Request.Form["mobileYn"]) ? "" :   Request.Form["mobileYn"] },     //모바일여부
            { "osType",     String.IsNullOrEmpty(Request.Form["osType"]) ? "" :     Request.Form["osType"] }        //운영체제구분
        };

        //요청 파라미터(바디) - 빌키 결제 전용(카드번호 등 수기 입력 필드 없음)
        Dictionary<String, String> REQ_BODY = new Dictionary<String, String>
        {
            { "pktHash",    "" },                                                                                   //해쉬값
            { "pmtprdNm",   String.IsNullOrEmpty(Request.Form["pmtprdNm"]) ? "" :   Request.Form["pmtprdNm"] },     //상품명
            { "mchtCustNm", String.IsNullOrEmpty(Request.Form["mchtCustNm"]) ? "" : Request.Form["mchtCustNm"] },   //상점고객명
            { "mchtCustId", String.IsNullOrEmpty(Request.Form["mchtCustId"]) ? "" : Request.Form["mchtCustId"] },   //상점고객아이디
            { "email",      String.IsNullOrEmpty(Request.Form["email"]) ? "" :      Request.Form["email"] },        //이메일
            { "billKey",    String.IsNullOrEmpty(Request.Form["billKey"]) ? "" :    Request.Form["billKey"] },      //빌키
            { "instmtMon",  String.IsNullOrEmpty(Request.Form["instmtMon"]) ? "" :  Request.Form["instmtMon"] },    //할부개월수
            { "crcCd",      String.IsNullOrEmpty(Request.Form["crcCd"]) ? "" :      Request.Form["crcCd"] },        //통화구분
            { "taxTypeCd",  String.IsNullOrEmpty(Request.Form["taxTypeCd"]) ? "" :  Request.Form["taxTypeCd"] },    //세금유형
            { "trdAmt",     String.IsNullOrEmpty(Request.Form["trdAmt"]) ? "" :     Request.Form["trdAmt"] },       //거래금액
            { "taxAmt",     String.IsNullOrEmpty(Request.Form["taxAmt"]) ? "" :     Request.Form["taxAmt"] },       //과세금액
            { "vatAmt",     String.IsNullOrEmpty(Request.Form["vatAmt"]) ? "" :     Request.Form["vatAmt"] },       //부가세금액
            { "taxFreeAmt", String.IsNullOrEmpty(Request.Form["taxFreeAmt"]) ? "" : Request.Form["taxFreeAmt"] },   //비과세금액
            { "svcAmt",     String.IsNullOrEmpty(Request.Form["svcAmt"]) ? "" :     Request.Form["svcAmt"] },       //봉사료
            { "notiUrl",    String.IsNullOrEmpty(Request.Form["notiUrl"]) ? "" :    Request.Form["notiUrl"] },      //결과처리URL
            { "mchtParam",  String.IsNullOrEmpty(Request.Form["mchtParam"]) ? "" :  Request.Form["mchtParam"] }     //기타주문정보
        };

        //응답 파라미터(헤더)
        Dictionary<String, String> RES_HEADER = new Dictionary<String, String>
        {
            { "mchtId", "" }, { "ver", "" }, { "method", "" }, { "bizType", "" }, { "encCd", "" },
            { "mchtTrdNo", "" }, { "trdNo", "" }, { "trdDt", "" }, { "trdTm", "" },
            { "outStatCd", "" }, { "outRsltCd", "" }, { "outRsltMsg", "" }
        };

        //응답 파라미터(바디)
        Dictionary<String, String> RES_BODY = new Dictionary<String, String>
        {
            { "pktHash", "" },        //해쉬값
            { "trdAmt", "" },         //거래금액
            { "billKey", "" },        //빌키
            { "cardNo", "" },         //마스킹된 카드번호(사전 협의 가맹점 한정)
            { "vldDtMon", "" },       //유효기간(월)
            { "vldDtYear", "" },      //유효기간(년)
            { "issrId", "" },         //발급사아이디
            { "cardNm", "" },         //카드사명
            { "cardKind", "" },       //카드종류명
            { "ninstmtTypeCd", "" },  //무이자할부타입
            { "instmtMon", "" },      //할부개월수
            { "apprNo", "" }          //승인번호
        };

        //AES256 암호화 필요 파라미터(요청) - 금액 항목만
        String[] ENCRYPT_PARAMS = { "trdAmt", "taxAmt", "vatAmt", "taxFreeAmt", "svcAmt" };

        //AES256 복호화 필요 파라미터(응답)
        String[] DECRYPT_PARAMS = { "trdAmt" };

        /** ========================================================================================================
                                    SHA256 해쉬 처리
                    조합필드 : 요청일자 + 요청시간 + 상점아이디 + 상점주문번호 + 거래금액(평문) + 라이센스키
            ========================================================================================================   */
        String hashPlain = "";
        String hashCipher = "";
        try
        {
            hashPlain = REQ_HEADER["trdDt"] + REQ_HEADER["trdTm"] + REQ_HEADER["mchtId"] + REQ_HEADER["mchtTrdNo"] + REQ_BODY["trdAmt"] + LICENSE_KEY;
            hashCipher = util.Sha256(hashPlain);
        }
        catch (Exception ex)
        {
            util.LogMessage(LOG_FILE, "[" + REQ_HEADER["mchtTrdNo"] + "][SHA256 HASHING] Hashing Fail! : " + ex.Message);
        }
        finally
        {
            util.LogMessage(LOG_FILE, "[" + REQ_HEADER["mchtTrdNo"] + "][SHA256 HASHING] Plain Text[" + hashPlain + "] ---> Cipher Text[" + hashCipher + "]");
            REQ_BODY["pktHash"] = hashCipher; //해쉬 결과 값 세팅
        }

        /** ============================================================================================
                                        AES256 암호화 처리
            ============================================================================================   */
        try
        {
            for (int i = 0; i < ENCRYPT_PARAMS.Length; i++)
            {
                String aesPlain = REQ_BODY[ENCRYPT_PARAMS[i]];
                if ("" != aesPlain)
                {
                    String aesCipher = util.Encrypt(aesPlain);

                    REQ_BODY[ENCRYPT_PARAMS[i]] = aesCipher; //암호화 결과 값 세팅
                    util.LogMessage(LOG_FILE, "[" + REQ_HEADER["mchtTrdNo"] + "][AES256 Encrypt] " + ENCRYPT_PARAMS[i] + "[" + aesPlain + "] ---> [" + aesCipher + "]");
                }
            }
        }
        catch (Exception ex)
        {
            util.LogMessage(LOG_FILE, "[" + REQ_HEADER["mchtTrdNo"] + "][AES256 Encrypt] AES256 Encrypt Fail! : " + ex.Message);
        }

        //URL 설정 : (tb)gw.settlebank.co.kr/spay/APICardActionPay.do
        String requestUrl = CANCEL_SERVER + "/spay/APICardActionPay.do";

        //요청파라미터 JSON에 세팅
        //params, data 이름은 세틀로 전달되야 하는 값이니 변경하지 마십시오.
        JObject reqParam = new JObject();
        reqParam.Add("params", JObject.FromObject(REQ_HEADER));
        reqParam.Add("data", JObject.FromObject(REQ_BODY));

        /** ======================================================================
                                    API호출(가맹점->세틀) 및 응답 처리
            ======================================================================   */
        Dictionary<String, String> respParam = new Dictionary<String, String>();
        try
        {
            //SendApi( API호출 URL, 전송될데이터, 타임아웃 )
            JObject resp = util.SendApi(requestUrl, reqParam.ToString(), TIMEOUT);

            //응답 파라미터 파싱
            JObject respHeader = resp.ContainsKey("params") ? (JObject)resp["params"] : null;
            JObject respBody = resp.ContainsKey("data") ? (JObject)resp["data"] : null;

            //응답 파라미터 세팅(헤더)
            if (respHeader != null)
            {
                foreach (string k in RES_HEADER.Keys)
                {
                    respParam[k] = respHeader.ContainsKey(k) ? respHeader[k].ToString() : "";
                }
            }
            else
            {
                foreach (string k in RES_HEADER.Keys) { respParam[k] = ""; }
            }

            //응답 파라미터 세팅(바디)
            if (respBody != null)
            {
                foreach (String k in RES_BODY.Keys)
                {
                    respParam[k] = respBody.ContainsKey(k) ? respBody[k].ToString() : "";
                }
            }
            else
            {
                foreach (String k in RES_BODY.Keys) { respParam[k] = ""; }
            }
        }
        catch (Exception ex)
        {
            foreach (string k in RES_HEADER.Keys) { if (!respParam.ContainsKey(k)) respParam[k] = ""; }
            foreach (string k in RES_BODY.Keys) { if (!respParam.ContainsKey(k)) respParam[k] = ""; }
            respParam["outStatCd"] = "0098";
            respParam["outRsltCd"] = "0098";
            respParam["outRsltMsg"] = "[Response Parsing Error]" + ex.Message;
            util.LogMessage(LOG_FILE, "[" + REQ_HEADER["mchtTrdNo"] + "][Response Parsing Error]" + ex.Message);
        }

        /** ======================================================================
                                    AES256 복호화 처리
            ======================================================================   */
        try
        {
            for (int i = 0; i < DECRYPT_PARAMS.Length; i++)
            {
                if (respParam.ContainsKey(DECRYPT_PARAMS[i]))
                {
                    String aesCipher = respParam[DECRYPT_PARAMS[i]].Trim();
                    if ("" != aesCipher)
                    {
                        String aesPlain = util.Decrypt(aesCipher);

                        respParam[DECRYPT_PARAMS[i]] = aesPlain;//복호화된 데이터로 세팅
                        util.LogMessage(LOG_FILE, "[" + REQ_HEADER["mchtTrdNo"] + "][AES256 Decrypt] " + DECRYPT_PARAMS[i] + "[" + aesCipher + "] ---> [" + aesPlain + "]");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            util.LogMessage(LOG_FILE, "[" + REQ_HEADER["mchtTrdNo"] + "][AES256 Decrypt] AES256 Decrypt Fail! : " + ex.Message);
        }

        //응답 값 출력
        Label_mchtId.Text        = respParam["mchtId"];
        Label_ver.Text           = respParam["ver"];
        Label_method.Text        = respParam["method"];
        Label_bizType.Text       = respParam["bizType"];
        Label_encCd.Text         = respParam["encCd"];
        Label_mchtTrdNo.Text     = respParam["mchtTrdNo"];
        Label_trdNo.Text         = respParam["trdNo"];
        Label_trdDt.Text         = respParam["trdDt"];
        Label_trdTm.Text         = respParam["trdTm"];
        Label_outStatCd.Text     = respParam["outStatCd"];
        Label_outRsltCd.Text     = respParam["outRsltCd"];
        Label_outRsltMsg.Text    = respParam["outRsltMsg"];
        Label_pktHash.Text       = respParam["pktHash"];
        Label_trdAmt.Text        = respParam["trdAmt"];
        Label_cardNo.Text        = respParam["cardNo"];
        Label_vldDtYear.Text     = respParam["vldDtYear"];
        Label_vldDtMon.Text      = respParam["vldDtMon"];
        Label_issrId.Text        = respParam["issrId"];
        Label_cardNm.Text        = respParam["cardNm"];
        Label_cardKind.Text      = respParam["cardKind"];
        Label_ninstmtTypeCd.Text = respParam["ninstmtTypeCd"];
        Label_instmtMon.Text     = respParam["instmtMon"];
        Label_apprNo.Text        = respParam["apprNo"];
        Label_billKey.Text       = respParam["billKey"];
    }
}
