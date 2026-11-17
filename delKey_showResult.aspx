<%@ Page Language="C#" AutoEventWireup="true" CodeFile="delKey_showResult.aspx.cs" Inherits="delKey_showResult" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, user-scalable=no, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0">
<meta http-equiv="X-UA-Compatible" content="ie=edge">
<title>빌키 삭제 응답 결과</title>
<style type="text/css">
    body            {font-family:굴림; font-size:10pt; color:#000000;}
    td              {font-family:굴림; font-size:10pt; padding:3px; border:1px solid #e1e1e1;}
    table           {width:100%; border-collapse:collapse;}
    .left           {padding-left:5px; width:230px;}
    .right          {padding-left:5px;}
    .wrapper        {width:700px; border:1px solid #e1e1e1;}
    .tab            {background-color:#f1f1f1; padding:10px 20px; border:1px solid #e1e1e1; font-weight:bold; font-size:1.1em;}
    .button         {padding:5px 20px; border-radius:20px; border:1px solid #ccc; width:70%; margin:5px 0px; transition:0.3s; cursor:pointer;}
    .button:hover   {background-color:#aaaaaa;}
</style>
</head>
<body>
<h2>빌키 삭제 응답 결과</h2>
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
        <tr><td class="left">trdDt[거래일자]</td><td class="right"><asp:Label ID="Label_trdDt" runat="server" /></td></tr>
        <tr><td class="left">trdTm[거래시간]</td><td class="right"><asp:Label ID="Label_trdTm" runat="server" /></td></tr>
        <tr><td class="left">outStatCd[거래상태코드]</td><td class="right"><asp:Label ID="Label_outStatCd" runat="server" /></td></tr>
        <tr><td class="left">outRsltCd[거래결과코드]</td><td class="right"><asp:Label ID="Label_outRsltCd" runat="server" /></td></tr>
        <tr><td class="left">outRsltMsg[결과메세지]</td><td class="right"><asp:Label ID="Label_outRsltMsg" runat="server" /></td></tr>
    </table>
    <div class="tab">data</div>
    <table>
        <tr><td class="left">pktHash[해쉬값]</td><td class="right"><asp:Label ID="Label_pktHash" runat="server" /></td></tr>
        <tr style="background-color:yellow;"><td class="left">billKey[삭제된 빌키]</td><td class="right"><asp:Label ID="Label_billKey" runat="server" /></td></tr>
        <tr>
            <td colspan="2" style="text-align: center;"><input class="button" type="button" value="돌아가기" onclick="location.href='delKey_form.aspx'"></td>
        </tr>
    </table>
</div>
</body>
</html>
