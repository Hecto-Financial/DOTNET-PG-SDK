using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class pay_encryptParams : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        /** 설정 정보 얻기 */
        SettleUtil util = SettleUtil.Instance;
        String LICENSE_KEY = util.LICENSE_KEY;//라이센스키
        String AES256_KEY = util.AES256_KEY;//AES256암호화키
        String LOG_FILE = util.LOG_FILE;//로그파일명

        /** 해쉬 및 aes256암호화 후 리턴 될 json */
        JObject rsp = new JObject();

        /** SHA256 해쉬 파라미터 */
        String mchtId       = String.IsNullOrEmpty(Request.Form["mchtId"]) ? "" :       Request.Form["mchtId"];       //상점아이디
        String method       = String.IsNullOrEmpty(Request.Form["method"]) ? "" :       Request.Form["method"];       //결제수단
        String mchtTrdNo    = String.IsNullOrEmpty(Request.Form["mchtTrdNo"]) ? "" :    Request.Form["mchtTrdNo"];    //상점주문번호
        String trdDt        = String.IsNullOrEmpty(Request.Form["trdDt"]) ? "" :        Request.Form["trdDt"];        //거래날짜
        String trdTm        = String.IsNullOrEmpty(Request.Form["trdTm"]) ? "" :        Request.Form["trdTm"];        //거래시간
        String trdAmt       = String.IsNullOrEmpty(Request.Form["plainTrdAmt"]) ? "" :  Request.Form["plainTrdAmt"];  //거래금액(평문)

        /** AES256 암호화 파라미터 */
        Dictionary<String, String> plain = new Dictionary<String, String>
        {
            { "trdAmt",          trdAmt },                              //거래금액
            { "mchtCustNm",      String.IsNullOrEmpty(Request.Form["plainMchtCustNm"]) ? "" :      Request.Form["plainMchtCustNm"] },     //상점고객명
            { "cphoneNo",        String.IsNullOrEmpty(Request.Form["plainCphoneNo"]) ? "" :        Request.Form["plainCphoneNo"] },       //상점고객휴대폰번호
            { "email",           String.IsNullOrEmpty(Request.Form["plainEmail"]) ? "" :           Request.Form["plainEmail"] },          //상점고객이메일
            { "mchtCustId",      String.IsNullOrEmpty(Request.Form["plainMchtCustId"]) ? "" :      Request.Form["plainMchtCustId"] },     //상점고객아이디
            { "taxAmt",          String.IsNullOrEmpty(Request.Form["plainTaxAmt"]) ? "" :          Request.Form["plainTaxAmt"] },         //과세금액
            { "vatAmt",          String.IsNullOrEmpty(Request.Form["plainVatAmt"]) ? "" :          Request.Form["plainVatAmt"] },         //부가세금액
            { "taxFreeAmt",      String.IsNullOrEmpty(Request.Form["plainTaxFreeAmt"]) ? "" :      Request.Form["plainTaxFreeAmt"] },     //면세금액
            { "svcAmt",          String.IsNullOrEmpty(Request.Form["plainSvcAmt"]) ? "" :          Request.Form["plainSvcAmt"] },         //봉사료
            { "clipCustNm",      String.IsNullOrEmpty(Request.Form["plainClipCustNm"]) ? "" :      Request.Form["plainClipCustNm"] },     //클립포인트고객명
            { "clipCustCi",      String.IsNullOrEmpty(Request.Form["plainClipCustCi"]) ? "" :      Request.Form["plainClipCustCi"] },     //클립포인트고객CI
            { "clipCustPhoneNo", String.IsNullOrEmpty(Request.Form["plainClipCustPhoneNo"]) ? "" : Request.Form["plainClipCustPhoneNo"] } //클립포인트고객휴대폰번호
        };

        /** 암호화 처리 후 */
        Dictionary<String, String> encrypted = new Dictionary<string, string>();



        /*============================================================================================================================================
         *  SHA256 해쉬 처리
         *조합 필드 : 상점아이디 + 결제수단 + 상점주문번호 + 요청일자 + 요청시간 + 거래금액(평문) + 라이센스키
         *============================================================================================================================================*/
        String hashPlain = mchtId + method + mchtTrdNo + trdDt + trdTm + trdAmt + LICENSE_KEY;
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
            rsp["hashCipher"] = hashCipher; // sha256 해쉬 결과 저장
        }

        /*============================================================================================================================================
         *  AES256 암호화 처리(AES-256-ECB encrypt -> Base64 encoding)
         *============================================================================================================================================ */
        try
        {
            foreach (KeyValuePair<String,String> pair in plain)
            {
                String aesPlain = plain[pair.Key];
                if ( "" != aesPlain )
                {
                    String aesCipher = util.Encrypt(aesPlain);

                    encrypted[pair.Key] = aesCipher;//암호화된 데이터로 세팅
                    util.LogMessage(LOG_FILE, "[" + mchtTrdNo + "][AES256 Encrypt] " + pair.Key + "[" + aesPlain + "] ---> [" + aesCipher + "]");
                }
                else
                {
                    encrypted[pair.Key] = "";
                }
            }

        }
        catch (Exception ex)
        {
            util.LogMessage(LOG_FILE,"[" + mchtTrdNo + "][AES256 Encrypt] AES256 Fail! : " + ex.Message);
        }
        finally
        {
            JObject encParams = JObject.FromObject(encrypted); //aes256 암호화 결과 저장
            rsp["encParams"] = encParams;
        }
        /* 결과 리턴 */
        Response.Write(rsp);

    }
}