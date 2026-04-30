<%@ Page Language="C#" %>
<%
    /** 설정 정보 저장 */
    SettleUtil util = SettleUtil.Instance;
    String AES256_KEY = util.AES256_KEY;
    String LOG_FILE = util.LOG_FILE;


    /** 응답 파라미터 세팅 */
    Dictionary<String, String> RES_PARAMS = new Dictionary<String, String>
    {
        { "mchtId",     String.IsNullOrEmpty(Request.Form["mchtId"]) ? "" :     Request.Form["mchtId"] },       //상점아이디
        { "outStatCd",  String.IsNullOrEmpty(Request.Form["outStatCd"]) ? "" :  Request.Form["outStatCd"] },    //결과코드
        { "outRsltCd",  String.IsNullOrEmpty(Request.Form["outRsltCd"]) ? "" :  Request.Form["outRsltCd"] },    //거절코드
        { "outRsltMsg", String.IsNullOrEmpty(Request.Form["outRsltMsg"]) ? "" : Request.Form["outRsltMsg"] },   //결과메세지
        { "method",     String.IsNullOrEmpty(Request.Form["method"]) ? "" :     Request.Form["method"] },       //결제수단
        { "mchtTrdNo",  String.IsNullOrEmpty(Request.Form["mchtTrdNo"]) ? "" :  Request.Form["mchtTrdNo"] },    //상점주문번호
        { "mchtCustId", String.IsNullOrEmpty(Request.Form["mchtCustId"]) ? "" : Request.Form["mchtCustId"] },   //상점고객아이디
        { "trdNo",      String.IsNullOrEmpty(Request.Form["trdNo"]) ? "" :      Request.Form["trdNo"] },        //세틀뱅크 거래번호
        { "trdAmt",     String.IsNullOrEmpty(Request.Form["trdAmt"]) ? "" :     Request.Form["trdAmt"] },       //거래금액
        { "mchtParam",  String.IsNullOrEmpty(Request.Form["mchtParam"]) ? "" :  Request.Form["mchtParam"] },    //상점 예약필드
        { "authDt",     String.IsNullOrEmpty(Request.Form["authDt"]) ? "" :     Request.Form["authDt"] },       //승인일시
        { "authNo",     String.IsNullOrEmpty(Request.Form["authNo"]) ? "" :     Request.Form["authNo"] },       //승인번호
        { "reqIssueDt", String.IsNullOrEmpty(Request.Form["reqIssueDt"]) ? "" : Request.Form["reqIssueDt"] },   //채번요청일시
        { "intMon",     String.IsNullOrEmpty(Request.Form["intMon"]) ? "" :     Request.Form["intMon"] },       //할부개월수
        { "fnNm",       String.IsNullOrEmpty(Request.Form["fnNm"]) ? "" :       Request.Form["fnNm"] },         //카드사명
        { "fnCd",       String.IsNullOrEmpty(Request.Form["fnCd"]) ? "" :       Request.Form["fnCd"] },         //카드사코드
        { "pointTrdNo", String.IsNullOrEmpty(Request.Form["pointTrdNo"]) ? "" : Request.Form["pointTrdNo"] },   //포인트거래번호
        { "pointTrdAmt",String.IsNullOrEmpty(Request.Form["pointTrdAmt"]) ? "" :Request.Form["pointTrdAmt"] },  //포인트거래금액
        { "cardTrdAmt", String.IsNullOrEmpty(Request.Form["cardTrdAmt"]) ? "" : Request.Form["cardTrdAmt"] },   //신용카드결제금액
        { "vtlAcntNo",  String.IsNullOrEmpty(Request.Form["vtlAcntNo"]) ? "" :  Request.Form["vtlAcntNo"] },    //가상계좌번호
        { "expireDt",   String.IsNullOrEmpty(Request.Form["expireDt"]) ? "" :   Request.Form["expireDt"] },     //입금기한
        { "cphoneNo",   String.IsNullOrEmpty(Request.Form["cphoneNo"]) ? "" :   Request.Form["cphoneNo"] },     //휴대폰번호
        { "billKey",    String.IsNullOrEmpty(Request.Form["billKey"]) ? "" :    Request.Form["billKey"] },      //자동결제키
        { "csrcAmt",    String.IsNullOrEmpty(Request.Form["csrcAmt"]) ? "" :    Request.Form["csrcAmt"] }       //현금영수증 발급 금액(네이버페이)
    };


    //AES256 복호화 필요 파라미터
    String[] DECRYPT_PARAMS = { "mchtCustId", "trdAmt", "pointTrdAmt", "cardTrdAmt", "vtlAcntNo", "cphoneNo", "csrcAmt" };




    /** ======================================================================
            AES256 복호화 처리(Base64 decoding -> AES-256-ECB decrypt )
        ======================================================================   */
    try
    {
        for ( int i=0; i< DECRYPT_PARAMS.Length; i++)
        {
            if (RES_PARAMS.ContainsKey(DECRYPT_PARAMS[i]))
            {
                String aesCipher = RES_PARAMS[DECRYPT_PARAMS[i]].Trim();
                if ("" != aesCipher )
                {
                    String aesPlain = util.Decrypt(aesCipher);

                    RES_PARAMS[DECRYPT_PARAMS[i]] = aesPlain;//복호화된 데이터로 세팅
                    util.LogMessage(LOG_FILE, "[" + RES_PARAMS["mchtTrdNo"] + "][AES256 Decrypt] " + DECRYPT_PARAMS[i] + "[" + aesCipher + "] ---> [" + aesPlain + "]");
                }
            }
        }
    }
    catch (Exception ex)
    {
        util.LogMessage(LOG_FILE, "[" + RES_PARAMS["mchtTrdNo"] + "][AES256 Decrypt] AES256 Decrypt Fail! : " + ex.Message);
    }

    //응답 파라미터 로깅
    String logStr = "[" + RES_PARAMS["mchtTrdNo"] + "][Response Data] ";
    foreach (KeyValuePair<String,String> pair in RES_PARAMS)
    {
        logStr += pair.Key + "(" + pair.Value + ") ";
    }
    util.LogMessage(LOG_FILE, logStr);
%>
<html>
<head><title>헥토파이낸셜 PG 결제 샘플</title>
<meta name="viewport" content="width=device-width, user-scalable=no, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0">
<meta http-equiv="X-UA-Compatible" content="ie=edge">
<style type="text/css">
    body            {font-family:굴림; font-size:10pt; color:#000000; text-decoration:none;}
    font            {font-family:굴림; font-size:10pt; color:#000000; text-decoration:none;}
    td              {font-family:굴림; font-size:10pt; color:#000000; text-decoration:none; padding:3px; border:1px solid #e1e1e1;}
    .left           {padding-left:5px; width:100px;}
    .right          {padding-left:5px;}
    .wrapper        {max-width:700px;border:1px solid #e1e1e1;}
    .tab            {background-color:#f1f1f1;padding:10px 20px;border:1px solid #e1e1e1; font-weight: bold; font-size:1.1em;}
    table           {width:100%; border-collapse:collapse;}
    .button         {padding:5px 20px; border-radius:20px; border:1px solid #ccc; width:70%; margin:5px 0px; transition:0.3s; cursor:pointer;}
    .button:hover   {background-color:#aaaaaa;}
</style>
<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
<script>
//결제 결과 세팅
var _PAY_RESULT = {
    mchtId :        "<%= RES_PARAMS["mchtId"] %>",
    outStatCd :     "<%= RES_PARAMS["outStatCd"] %>",
    outRsltCd :     "<%= RES_PARAMS["outRsltCd"] %>",
    outRsltMsg :    "<%= RES_PARAMS["outRsltMsg"] %>",
    method :        "<%= RES_PARAMS["method"] %>",
    mchtTrdNo :     "<%= RES_PARAMS["mchtTrdNo"] %>",
    mchtCustId :    "<%= RES_PARAMS["mchtCustId"] %>",
    trdNo :         "<%= RES_PARAMS["trdNo"] %>",
    trdAmt :        "<%= RES_PARAMS["trdAmt"] %>",
    mchtParam :     "<%= RES_PARAMS["mchtParam"] %>",
    authDt :        "<%= RES_PARAMS["authDt"] %>",
    authNo :        "<%= RES_PARAMS["authNo"] %>",
    reqIssueDt :    "<%= RES_PARAMS["reqIssueDt"] %>",
    intMon :        "<%= RES_PARAMS["intMon"] %>",
    fnNm :          "<%= RES_PARAMS["fnNm"] %>",
    fnCd :          "<%= RES_PARAMS["fnCd"] %>",
    pointTrdNo :    "<%= RES_PARAMS["pointTrdNo"] %>",
    pointTrdAmt :   "<%= RES_PARAMS["pointTrdAmt"] %>",
    cardTrdAmt :    "<%= RES_PARAMS["cardTrdAmt"] %>",
    vtlAcntNo :     "<%= RES_PARAMS["vtlAcntNo"] %>",
    expireDt :      "<%= RES_PARAMS["expireDt"] %>",
    cphoneNo :      "<%= RES_PARAMS["cphoneNo"] %>",
    billKey :       "<%= RES_PARAMS["billKey"] %>",
    csrcAmt :       "<%= RES_PARAMS["csrcAmt"] %>"
};
//main으로 결과 전달
function sendResult()
{
    if(top.opener){
        //팝업창
        top.opener.rstparamSet(_PAY_RESULT);
        top.opener.goResult();
        self.close();
    }
    else{//iframe
        parent.postMessage(JSON.stringify({action:"HECTO_IFRAME_CLOSE", params: _PAY_RESULT}), "*");
    }
}
</script>
</head>
<body>
<h2>승인 요청 결과</h2>
<div class="wrapper">
    <div class="tab">응답 파라미터</div>
    <table>
        <tr>
            <td class="left">mchtId</td>
            <td class="right"><%= RES_PARAMS["mchtId"] %></td>
        </tr>
        <tr>
            <td class="left">outStatCd</td>
            <td class="right"><%= RES_PARAMS["outStatCd"] %></td>
        </tr>
        <tr>
            <td class="left">outRsltCd</td>
            <td class="right"><%= RES_PARAMS["outRsltCd"] %></td>
        </tr>
        <tr>
            <td class="left">outRsltMsg</td>
            <td class="right"><%= RES_PARAMS["outRsltMsg"] %></td>
        </tr>
        <tr>
            <td class="left">method</td>
            <td class="right"><%= RES_PARAMS["method"] %></td>
        </tr>
        <tr>
            <td class="left">mchtTrdNo</td>
            <td class="right"><%= RES_PARAMS["mchtTrdNo"] %></td>
        </tr>
        <tr>
            <td class="left">mchtCustId</td>
            <td class="right"><%= RES_PARAMS["mchtCustId"] %></td>
        </tr>
        <tr>
            <td class="left">trdNo</td>
            <td class="right"><%= RES_PARAMS["trdNo"] %></td>
        </tr>
        <tr>
            <td class="left">trdAmt</td>
            <td class="right"><%= RES_PARAMS["trdAmt"] %></td>
        </tr>
        <tr>
            <td class="left">mchtParam</td>
            <td class="right"><%= RES_PARAMS["mchtParam"] %></td>
        </tr>
        <tr>
            <td class="left">authDt</td>
            <td class="right"><%= RES_PARAMS["authDt"] %></td>
        </tr>
        <tr>
            <td class="left">authNo</td>
            <td class="right"><%= RES_PARAMS["authNo"] %></td>
        </tr>
        <tr>
            <td class="left">reqIssueDt</td>
            <td class="right"><%= RES_PARAMS["reqIssueDt"] %></td>
        </tr>
        <tr>
            <td class="left">intMon</td>
            <td class="right"><%= RES_PARAMS["intMon"] %></td>
        </tr>
        <tr>
            <td class="left">fnNm</td>
            <td class="right"><%= RES_PARAMS["fnNm"] %></td>
        </tr>
        <tr>
            <td class="left">fnCd</td>
            <td class="right"><%= RES_PARAMS["fnCd"] %></td>
        </tr>
        <tr>
            <td class="left">pointTrdNo</td>
            <td class="right"><%= RES_PARAMS["pointTrdNo"] %></td>
        </tr>
        <tr>
            <td class="left">pointTrdAmt</td>
            <td class="right"><%= RES_PARAMS["pointTrdAmt"] %></td>
        </tr>
        <tr>
            <td class="left">cardTrdAmt</td>
            <td class="right"><%= RES_PARAMS["cardTrdAmt"] %></td>
        </tr>
        <tr>
            <td class="left">vtlAcntNo</td>
            <td class="right"><%= RES_PARAMS["vtlAcntNo"] %></td>
        </tr>
        <tr>
            <td class="left">expireDt</td>
            <td class="right"><%= RES_PARAMS["expireDt"] %></td>
        </tr>
        <tr>
            <td class="left">cphoneNo</td>
            <td class="right"><%= RES_PARAMS["cphoneNo"] %></td>
        </tr>
        <tr>
            <td class="left">billKey</td>
            <td class="right"><%= RES_PARAMS["billKey"] %></td>
        </tr>
        <tr>
            <td class="left">csrcAmt</td>
            <td class="right"><%= RES_PARAMS["csrcAmt"] %></td>
        </tr>

        <tr>
            <td colspan="2" style="text-align: center;">
                <input class="button" type="button" value="확인" onclick="sendResult()" /> 
            </td>
        </tr>
    </table>
</div>
</body>
</html>

