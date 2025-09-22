using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

/// <summary>
/// 세틀뱅크 PG 유틸리티 클래스
/// </summary>
public class SettleUtil
{
    //private 생성자 
    private SettleUtil() { }
    //private static 인스턴스 객체
    private static readonly Lazy<SettleUtil> _instance = new Lazy<SettleUtil>(() => new SettleUtil());
    //public static 의 객체반환 함수
    public static SettleUtil Instance { get { return _instance.Value; } }


    /*===========================================================================================================
     *   MID(상점아이디)
     *상점아이디는 세틀뱅크에서 상점으로 발급하는 상점의 고유한 식별자입니다.
     *테스트환경에서의 MID는 다음과 같습니다.
     *  nx_mid_il : 문화/도서/해피/스마트문상/틴캐시/계좌이체/가상계좌/티머니
     *  nxca_jt_il : 신용카드 인증 결제
     *  nxca_jt_bi : 신용카드 비인증 결제
     *  nxca_jt_gu : 신용카드 구인증 결제
     *  nxca_payco : 페이코 간편결제
     *  nxca_kakao : 카카오 간편결제
     *  nxhp_pl_il : 휴대폰 일반 결제
     *  nxhp_pl_ma : 휴대폰 월 자동 결제
     *  nxhp_pl_hd : 휴대폰 인증/승인 분리형
     *  nxpt_kt_il : 포인트 결제
     *상용서비스시에는 세틀뱅크에서 발급한 상점 고유 MID를 설정하십시오.
     *===========================================================================================================*/
    private const string _PG_MID = "nx_mid_il";



    /*======================================================================================================================================
     *   라이센스키
     *회원사 mid당 하나의 라이센스키가 발급되며 SHA256 해시체크 용도로 사용됩니다. 이 값은 외부에 노출되어서는 안 됩니다.
     *테스트환경에서는 ST1009281328226982205 값을 사용하시면 되며,
     *상용서비스시에는 세틀뱅크에서 발급한 상점 고유 라이센스키를 설정하십시오.
     *======================================================================================================================================*/
    private const string _LICENSE_KEY = "ST1009281328226982205";



    /*====================================================================================================
     *      AES256 암호화 키
     * 파라미터 AES256암/복호화에 사용되는 키 입니다. 이 값은 외부에 노출되어서는 안 됩니다.
     * 테스트환경에서는 pgSettle30y739r82jtd709yOfZ2yK5K를 사용하시면 됩니다.
     * 상용서비스시에는 세틀뱅크에서 발급한 상점 고유 암호화키를 설정하십시오.
     *====================================================================================================*/
    private const string _AES256_KEY = "pgSettle30y739r82jtd709yOfZ2yK5K";



    /*==================================================================================
     *      결제 서버 URL
     * 세틀뱅크 결제 서버 URL입니다. 이 값은 변경하지 마십시오. 
     * 필요에 따라 주석 on/off 하여 사용하십시오.
     * 
     *==================================================================================*/
    private const string _PAYMENT_SERVER = "https://tbnpg.settlebank.co.kr"; //테스트서버 URL
    //private const string _PAYMENT_SERVER = "https://npg.settlebank.co.kr"; //운영서버 URL




    /*===================================================================
     *  취소 서버 URL
     *세틀뱅크 취소 서버 URL입니다. 이 값은 변경하지 마십시오.
     *필요에 따라 주석 on/off 하여 사용하십시오.    
     *===================================================================*/
    private const string _CANCEL_SERVER = "https://tbgw.settlebank.co.kr"; //테스트서버 URL
    //private const string _CANCEL_SERVER = "https://gw.settlebank.co.kr"; //운영서버 URL




    /*===========================================
    *   세틀뱅크 API통신 Connect Timeout 설정(ms)
    *===========================================*/
    private const int _TIMEOUT = 5000;


    /*=========================================================================================================
     *  로그 파일명 및 경로 수정
     *=========================================================================================================*/
    private const string _LOG_DIR = "C:/npg/dotnet";            //로그 디렉터리(디렉터리가 존재하지 않으면, 로그 파일 생성하지 않습니다.)
    private const string _LOG_FILE = "trans.log";               //일반 거래 로그 파일명
    private const string _NOTI_LOG_FILE = "noti_trans.log";     //노티 처리 로그 파일명

    // 프로퍼티 세팅
    public string PG_MID { get { return _PG_MID; } }
    public string LICENSE_KEY { get { return _LICENSE_KEY; } }
    public string AES256_KEY { get { return _AES256_KEY; } }
    public string PAYMENT_SERVER { get { return _PAYMENT_SERVER; } }
    public string CANCEL_SERVER { get { return _CANCEL_SERVER; } }
    public int TIMEOUT { get { return _TIMEOUT; } }
    public string LOG_DIR { get { return _LOG_DIR; } }
    public string LOG_FILE { get { return _LOG_FILE; } }
    public string NOTI_LOG_FILE { get { return _NOTI_LOG_FILE; } }


    //파라미터 AES256 암호화
    public string Encrypt(String val)
    {
        RijndaelManaged aes = new RijndaelManaged
        {
            KeySize = 256,
            BlockSize = 128,
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7,
            Key = Encoding.UTF8.GetBytes(this.AES256_KEY),
            IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);

        byte[] xBuff = null;
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
            {
                byte[] xXml = Encoding.UTF8.GetBytes(val);
                cs.Write(xXml, 0, xXml.Length);
            }

            xBuff = ms.ToArray();
        }

        String Output = Convert.ToBase64String(xBuff);

        return Output;
    }

    //파라미터 AES256복호화
    public string Decrypt(String val)
    {
        RijndaelManaged aes = new RijndaelManaged
        {
            KeySize = 256,
            BlockSize = 128,
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7,
            Key = Encoding.UTF8.GetBytes(this.AES256_KEY),
            IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        var decrypt = aes.CreateDecryptor();
        byte[] xBuff = null;
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
            {
                byte[] xXml = Convert.FromBase64String(val);
                cs.Write(xXml, 0, xXml.Length);
            }

            xBuff = ms.ToArray();
        }

        String Output = Encoding.UTF8.GetString(xBuff);
        return Output;
    }

    /* Sha256 암호화 */
    public string Sha256(string payload)
    {
        SHA256 sha = new SHA256Managed();
        byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(payload));
        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte b in hash)
        {
            stringBuilder.AppendFormat("{0:x2}", b);
        }
        return stringBuilder.ToString();
    }


    /* 로그 출력 메소드 */
    public void LogMessage(string log_fname, string msg)
    {
        // 로그 파일명 패턴 : 파일명.yyyy-MM-dd
        string filename = log_fname + "." + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

        // 로그 디렉터리 설정
        string logDir = this.LOG_DIR;

        // 디렉터리가 존재할 경우에만 파일생성
        if (Directory.Exists(logDir))
        {
            // 파일 없으면 생성, 있으면 Append
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(logDir, filename), true))
            {
                outputFile.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " : " + msg);
            }
        }
    }

    //API 호출 메소드
    public dynamic SendApi(string target_url, string postData, int timeout)
    {
        LogMessage(LOG_FILE, "[SendAPI Start] URL(" + target_url + "), Timeout(" + timeout + ")");
        LogMessage(LOG_FILE, "[SendAPI Request] Parameters : " + HttpUtility.UrlDecode(postData));

        JObject responseFromServer = new JObject();
        String responseStr = "";
        try
        {
            WebRequest webRequest = WebRequest.Create(target_url);
            webRequest.Method = "POST";                                    // POST로 설정
            webRequest.Timeout = timeout;                                  // Timeout 설정
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);           // byte[]로 변환
            webRequest.ContentType = "Application/json";                   // ContentType 설정
            webRequest.ContentLength = byteArray.Length;                   // Content 길이 설정

            // Get the request stream. request 스트림을 얻는다.
            Stream dataStream = webRequest.GetRequestStream();
            // request스트림에 데이터 출력
            dataStream.Write(byteArray, 0, byteArray.Length);
            // request스트림 닫기
            dataStream.Close();

            // response객체를 얻는다.
            WebResponse webResponse = webRequest.GetResponse();

            LogMessage(LOG_FILE, "[SendAPI Result] HTTP POST request for URL(" + target_url + ") resulted in HTTP status code " + (int)((HttpWebResponse)webResponse).StatusCode + "(" + ((HttpWebResponse)webResponse).StatusDescription + ")");

            // 서버로부터 리턴된 내용을 스트림에서 읽어온다.
            using (dataStream = webResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                responseStr = reader.ReadToEnd();
                responseFromServer = JObject.Parse(responseStr);
            }

            webResponse.Close();

            LogMessage(LOG_FILE, "[SendAPI End] Response : " + responseFromServer.ToString() + "\n");
        }
        catch (Exception ex)
        {
            LogMessage(LOG_FILE, "[SendAPI Exception] "+ex.Message);
            Dictionary<String, String> head = new Dictionary<String, String>
            {
                { "outStatCd", "0099" },
                { "outRsltCd", "0099" },
                { "outRsltMsg", "[SendAPI Exception] "+ex.Message }
            };
            Dictionary<String, String> body = new Dictionary<String, String>();
            Dictionary<String, Object> resData = new Dictionary<String, Object>
            {
                { "params", head },
                { "data", body }
            };

            responseFromServer = JObject.FromObject(resData);
        }
        return responseFromServer;
    }


    //노티를 성공적으로 수신한 경우 처리할 로직을 작성하여 주세요.

    public bool NotiSuccess(Dictionary<String,String> noti)
    {
        //TODO : 관련 로직 추가
        return true;
    }



    //입금대기시 처리할 로직을 작성하여 주세요.
    public bool NotiWaitingPay(Dictionary<String, String> noti)
    {
        //TODO : 관련 로직 추가
        return true;
    }



    //노티 수신중 해시 체크 에러가 생긴 경우 처리할 로직을 작성하여 주세요.
    public bool NotiHashError(Dictionary<String, String> noti)
    {
        //TODO : 관련 로직 추가
        return false;
    }
}