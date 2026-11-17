<%@ Page Language="C#" AutoEventWireup="true" CodeFile="billkey_pay_showResult.aspx.cs" Inherits="billkey_pay_showResult" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, user-scalable=no, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0">
<meta http-equiv="X-UA-Compatible" content="ie=edge">
<title>빌키 결제 응답 결과</title>
<style type="text/css">
    body            {font-family:굴림; font-size:10pt; color:#000000;}
    td              {font-family:굴림; font-size:10pt; padding:3px; border:1px solid #e1e1e1;}
    table           {width:100%; border-collapse:collapse;}
    .left           {padding-left:5px; width:230px;}
    .right          {padding-left:5px;}
    .wrapper        {width:700px; border:1px solid #e1e1e1;}
    .tab            {background-color:#f1f1f1; padding:10px 20px; border:1px solid #e1e1e1; font-weight:bold; font-size:1.1em;}
    .button         {padding:5px 20px; border-radius:20px; border:1px solid #ccc; width:70%; margin:5px 0; transition:0.3s; cursor:pointer;}
    .button:hover   {background-color:#aaaaaa;}
</style>
</head>
<body>
<h2>빌키 결제 응답 결과</h2>
<div class="wrapper">
    <div class="tab">params</div>
    <table>
        <tr><td class="left">mchtId[상점아이디]</td><td class="right"><asp:Label ID="Label_mchtId" runat="server" /></td></tr>
        <tr><td class="left">ver[버전]</td><td class="right"><asp:Label ID="Label_ver" runat="server" /></td></tr>
        <tr><td class="left">method[결제수단]</td><td class="right"><asp:Label ID="Label_method" runat="server" /></td></tr>
        <tr><td class="left">bizType[업무구분]</td><td class="right"><asp:Label ID="Label_bizType" runat="server" /></td></tr>
        <tr><td class="left">encCd[암호화구분]</td><td class="right"><asp:Label ID="Label_encCd" runat="server" /></td></tr>
        <tr><td class="left">mchtTrdNo[상점주문번호]</td><td class="right"><asp:Label ID="Label_mchtTrdNo" runat="server" /></td></tr>
        <tr><td class="left">trdNo[헥토파이낸셜 거래번호]</td><td class="right"><asp:Label ID="Label_trdNo" runat="server" /></td></tr>
        <tr><td class="left">trdDt[요청일자]</td><td class="right"><asp:Label ID="Label_trdDt" runat="server" /></td></tr>
        <tr><td class="left">trdTm[요청시간]</td><td class="right"><asp:Label ID="Label_trdTm" runat="server" /></td></tr>
        <tr><td class="left">outStatCd[거래상태코드]</td><td class="right"><asp:Label ID="Label_outStatCd" runat="server" /></td></tr>
        <tr><td class="left">outRsltCd[거래결과코드]</td><td class="right"><asp:Label ID="Label_outRsltCd" runat="server" /></td></tr>
        <tr><td class="left">outRsltMsg[결과메세지]</td><td class="right"><asp:Label ID="Label_outRsltMsg" runat="server" /></td></tr>
    </table>
    <div class="tab">data</div>
    <table>
        <tr><td class="left">pktHash[해쉬값]</td><td class="right"><asp:Label ID="Label_pktHash" runat="server" /></td></tr>
        <tr><td class="left">trdAmt[거래금액]</td><td class="right"><asp:Label ID="Label_trdAmt" runat="server" /></td></tr>
        <tr><td class="left">cardNo[카드번호(마스킹)]</td><td class="right"><asp:Label ID="Label_cardNo" runat="server" /></td></tr>
        <tr><td class="left">vldDtYear[유효기간(년)]</td><td class="right"><asp:Label ID="Label_vldDtYear" runat="server" /></td></tr>
        <tr><td class="left">vldDtMon[유효기간(월)]</td><td class="right"><asp:Label ID="Label_vldDtMon" runat="server" /></td></tr>
        <tr><td class="left">issrId[발급사아이디]</td><td class="right"><asp:Label ID="Label_issrId" runat="server" /></td></tr>
        <tr><td class="left">cardNm[카드사명]</td><td class="right"><asp:Label ID="Label_cardNm" runat="server" /></td></tr>
        <tr><td class="left">cardKind[카드종류명]</td><td class="right"><asp:Label ID="Label_cardKind" runat="server" /></td></tr>
        <tr><td class="left">ninstmtTypeCd[무이자할부타입]</td><td class="right"><asp:Label ID="Label_ninstmtTypeCd" runat="server" /></td></tr>
        <tr><td class="left">instmtMon[할부개월수]</td><td class="right"><asp:Label ID="Label_instmtMon" runat="server" /></td></tr>
        <tr><td class="left">apprNo[승인번호]</td><td class="right"><asp:Label ID="Label_apprNo" runat="server" /></td></tr>
        <tr style="background-color:yellow;"><td class="left">billKey[빌키]</td><td class="right"><asp:Label ID="Label_billKey" runat="server" /></td></tr>
        <tr>
            <td colspan="2" style="text-align:center;"><input class="button" type="button" value="돌아가기" onclick="location.href='billkey_pay_form.aspx'"></td>
        </tr>
    </table>
</div>
</body>
</html>
