<!--
/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */
-->

<%@ Page 
    CodeBehind="Login.aspx.cs" 
    Inherits="Agrishare.CMS.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">

    <head runat="server">
        <base href="/" />
        <title>AgriShare CMS - Login</title>
        <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700,400italic" />
        <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet" />
        <meta content="IE=edge, chrome=1" http-equiv="X-UA-Compatible" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <meta name="SKYPE_TOOLBAR" content="SKYPE_TOOLBAR_PARSER_COMPATIBLE" />    

        <style type="text/css">

            html, body {
                height: 100%;
                min-height: 100%;
            }

            body, input {
                font-size: 1em;
                font-family: Roboto, sans-serif;
            }

            body { 
                color: #222;
                line-height: 1.4;
                height: 100%;
                background: #339933;
                display: flex;
                flex-direction: column;
                align-items: center;
                justify-content: center;
            }

            form {
                width: 100%;
                max-width: 280px;
                overflow: hidden;
                text-align: center;
            }

            h1 {
                margin: 0;
                color: #fff;
                font-size: 24px;
            }

            p {
                color: rgba(255, 255, 255, .5);
            }

            .form > div {
                display: flex;
                align-items: center;
                background: white;
                border-radius: 3px;
                margin: 1em 0;
                padding: 0 10px;
                box-shadow: rgba(0, 0, 0,.2) 0 1px 2px;
            }

            .form > div > input {
                width: 100%;
                border: 0;
                margin: 0;
                padding: 15px;
                outline: none;
            }

            input[type=submit] {
                display: block;
                width: 100%;
                padding: 15px;
                border: 0;
                margin: 1em 0;
                color: white;
                text-transform: uppercase;
                font-size: 14px;
                font-weight: bold;
                border-radius: 3px;
                background-color: #cc3d78;
                cursor: pointer;
                transition: all .4s ease;
            }

            input[type=submit]:hover {
                background: black;
            }

            #ForgotForm { display: none; }

            a {
                display: block;
                margin: 1em 0;
                text-transform: uppercase;
                font-size: 12px;
                cursor: pointer;
            }

            .feedback {
                border: solid 1px #fff;
                border-radius: 3px;
                padding: 10px;
                color: white;
            }

        </style>

        <script type="text/javascript">

            function showLogin() {
                document.getElementById('LoginForm').style.display = 'block';
                document.getElementById('ForgotForm').style.display = 'none';
            }

            function showForgot() {
                document.getElementById('LoginForm').style.display = 'none';
                document.getElementById('ForgotForm').style.display = 'block';
            }

        </script>

    </head>

    <body>

        <form runat="server">

            <h1>AgriShare CMS</h1>

            <p class="feedback" runat="server" id="Feedback" visible="false" />

            <asp:Panel runat="server" class="form" id="LoginForm">

                <p>Please enter your mobile number and PIN to proceed.</p>
                                
                <div>
                    <span class="material-icons">phone_android</span>
                    <asp:TextBox runat="server" ID="LoginMobileNumber" maxlength="1024" placeholder="Mobile number" />
                </div>

                <div>
                    <span class="material-icons">vpn_key</span>
                    <asp:TextBox runat="server" ID="LoginPin" TextMode="Password" type="password" maxlength="32" placeholder="PIN" />
                </div>

                <asp:Button runat="server" class="button" onclick="Authenticate" Text="Login" />
                <a onclick="showForgot()">Forgot your PIN</a>

            </asp:Panel>

            <asp:Panel runat="server" class="form" id="ForgotForm">
                
                <p>Enter your mobile number and we will send you a verification code via SMS.</p>

                <div>
                    <span class="material-icons">phone_android</span>
                    <asp:TextBox runat="server" ID="ForgotMobileNumber" maxlength="1024" placeholder="Mobile number" />
                </div>

                <asp:Button runat="server" class="button" onclick="SendCode" Text="Send" />
                <a onclick="showLogin()">Back to login</a>

            </asp:Panel>

            <asp:Panel runat="server" class="form" id="ResetForm" Visible="false">
                
                <p>Please enter your verfication code and a <strong>new</strong> PIN.</p>

                <div>
                    <span class="material-icons">phone_android</span>
                    <asp:TextBox runat="server" ID="ResetMobileNumber" maxlength="16" placeholder="Mobile number" />
                </div>

                <div>
                    <span class="material-icons">vpn_key</span>
                    <asp:TextBox runat="server" ID="ResetSmsCode" maxlength="4" placeholder="SMS Code" />
                </div>

                <div>
                    <span class="material-icons">vpn_key</span>
                    <asp:TextBox runat="server" ID="ResetNewPin" maxlength="4" placeholder="New PIN" />
                </div>

                <asp:Button runat="server" class="button" onclick="ResetPin" Text="Reset" />

            </asp:Panel>

        </form>
    
    </body>

</html>