using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class pay_autoPayResult : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //설정 정보 가져오기
        SettleUtil util = SettleUtil.Instance;
        String AES256_KEY = util.AES256_KEY;       //AES256 암복호화 키
        String LICENSE_KEY = util.LICENSE_KEY;     //라이센스 키
        String apiHost = util.CANCEL_SERVER;       //타겟URL
        String LOG_FILE = util.LOG_FILE;           //로그파일명
        int TIMEOUT = util.TIMEOUT;                //타임아웃


        //요청 파라미터(헤더)
        Dictionary<String, String> REQ_HEADER = new Dictionary<String, String>
        {   
            { "mchtId",     String.IsNullOrEmpty(Request.Form["mchtId"]) ? "" :    Request.Form["mchtId"] },    //상점아이디
            { "ver",        String.IsNullOrEmpty(Request.Form["ver"]) ? "" :       Request.Form["ver"] },       //버전
            { "method",     String.IsNullOrEmpty(Request.Form["method"]) ? "" :    Request.Form["method"] },    //결제수단
            { "bizType",    String.IsNullOrEmpty(Request.Form["bizType"]) ? "" :   Request.Form["bizType"] },   //업무구분
            { "encCd",      String.IsNullOrEmpty(Request.Form["encCd"]) ? "" :     Request.Form["encCd"] },     //암호화구분
            { "mchtTrdNo",  String.IsNullOrEmpty(Request.Form["mchtTrdNo"]) ? "" : Request.Form["mchtTrdNo"] }, //상점주문번호
            { "trdDt",      String.IsNullOrEmpty(Request.Form["trdDt"]) ? "" :     Request.Form["trdDt"] },     //요청일자
            { "trdTm",      String.IsNullOrEmpty(Request.Form["trdTm"]) ? "" :     Request.Form["trdTm"] },     //요청시간
            { "mobileYn",   String.IsNullOrEmpty(Request.Form["mobileYn"]) ? "" :  Request.Form["mobileYn"] },  //모바일여부
            { "osType",     String.IsNullOrEmpty(Request.Form["osType"]) ? "" :    Request.Form["osType"] }     //운영체제구분
        };

        //요청 파라미터(바디)
        Dictionary<String, String> REQ_BODY = new Dictionary<String, String>
        {
            { "telCo",   String.IsNullOrEmpty(Request.Form["telCo"]) ? "" :     Request.Form["telCo"] },    //통신사
            { "email",   String.IsNullOrEmpty(Request.Form["email"]) ? "" :     Request.Form["email"] },    //상점고객이메일
            { "mUserId", String.IsNullOrEmpty(Request.Form["mUserId"]) ? "" :   Request.Form["mUserId"] },  //상점고객아이디
            { "crcCd",   String.IsNullOrEmpty(Request.Form["crcCd"]) ? "" :     Request.Form["crcCd"] },    //통화구분
            { "trdAmt",  String.IsNullOrEmpty(Request.Form["trdAmt"]) ? "" :    Request.Form["trdAmt"] },   //거래금액
            { "prdtNm",  String.IsNullOrEmpty(Request.Form["prdtNm"]) ? "" :    Request.Form["prdtNm"] },   //상품명
            { "sellerNm",String.IsNullOrEmpty(Request.Form["sellerNm"]) ? "" :  Request.Form["sellerNm"] }, //판매자명
            { "ordNm",   String.IsNullOrEmpty(Request.Form["ordNm"]) ? "" :     Request.Form["ordNm"] },    //주문자명
            { "billKey", String.IsNullOrEmpty(Request.Form["billKey"]) ? "" :   Request.Form["billKey"] }   //자동결제키
        };


        //응답 파라미터(헤더)
        Dictionary<String, String> RES_HEADER = new Dictionary<String, String>
        {
            { "mchtId", "" },       //상점아이디
            { "ver", "" },          //버전
            { "method", "" },       //결제수단
            { "bizType", "" },      //업무구분
            { "encCd", "" },        //암호화구분
            { "mchtTrdNo", "" },    //상점주문번호
            { "trdNo", "" },        //헥토파이낸셜거래번호
            { "trdDt", "" },        //요청일자
            { "trdTm", "" },        //요청시간
            { "outStatCd", "" },    //결과코드
            { "outRsltCd", "" },    //거절코드
            { "outRsltMsg", "" }    //결과메세지
        };

        //응답 파라미터(바디)
        Dictionary<String, String> RES_BODY = new Dictionary<String, String>
        {
            { "pktHash", "" },     //해쉬값
            { "telCo", "" },       //통신사
            { "trdAmt", "" },      //거래금액
            { "billKey", "" }      //자동결제키
        };


        //AES256 암호화 필요 파라미터
        String[] ENCRYPT_PARAMS = { "telCo", "trdAmt" };

        //AES256 복호화 필요 파라미터
        String[] DECRYPT_PARAMS = { "telCo", "trdAmt" };

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
            for (int i= 0; i < ENCRYPT_PARAMS.Length; i++)
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



        /** ======================================================================
         *                          타겟 URL 설정
         *  ======================================================================   */
        String requestUrl = apiHost + "/spay/APIService.do";


        //요청파라미터 JSON에 세팅
        //params, data 이름은 헥토파이낸셜로 전달되야 하는 값이니 변경하지 마십시오.
        JObject reqParam = new JObject();
        reqParam.Add("params", JObject.FromObject(REQ_HEADER));
        reqParam.Add("data", JObject.FromObject(REQ_BODY));



        /** ======================================================================
                                    API호출(가맹점->헥토파이낸셜) 및 응답 처리
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
                foreach (string k in RES_HEADER.Keys)
                {
                    respParam[k] = "";
                }
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
                foreach (String k in RES_BODY.Keys)
                {
                    respParam[k] = "";
                }
            }


        }
        catch (Exception ex)
        {
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
            for (int i=0; i< DECRYPT_PARAMS.Length; i++)
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

        // 응답 값 출력
        Label_mchtId.Text       = respParam["mchtId"];
        Label_ver.Text          = respParam["ver"];
        Label_method.Text       = respParam["method"];
        Label_bizType.Text      = respParam["bizType"];
        Label_encCd.Text        = respParam["encCd"];
        Label_mchtTrdNo.Text    = respParam["mchtTrdNo"];
        Label_trdNo.Text        = respParam["trdNo"];
        Label_trdDt.Text        = respParam["trdDt"];
        Label_trdTm.Text        = respParam["trdTm"];
        Label_outStatCd.Text    = respParam["outStatCd"];
        Label_outRsltCd.Text    = respParam["outRsltCd"];
        Label_outRsltMsg.Text   = respParam["outRsltMsg"];
        Label_pktHash.Text      = respParam["pktHash"];
        Label_telCo.Text        = respParam["telCo"];
        Label_trdAmt.Text       = respParam["trdAmt"];
        Label_billKey.Text      = respParam["billKey"];
    }
}