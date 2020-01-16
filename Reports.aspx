<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Reports.aspx.vb" Inherits="Reports" %>

<%@ Register Src="~/TopMenu.ascx" TagPrefix="uc1" TagName="TopMenu" %>
<%@ Register Src="~/BottomNav.ascx" TagPrefix="uc1" TagName="BottomNav" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">

    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="custom homemade pies">
    <meta name="author" content="ProteusNet">
    <link rel="icon" type="image/ico" href="images/favicon.ico" />

    <title>Pie Gourmet :: Dashboard</title>

    <!-- Custom styles for this template -->
    <!-- build:css stylesheets/main.css -->
    <link href="stylesheets/bootstrap.css" rel="stylesheet">
    <link href="stylesheets/main.css" rel="stylesheet">
    <!-- endbuild -->

    <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
      <script src="https://oss.maxcdn.com/libs/respond.js/1.3.0/respond.min.js"></script>
    <![endif]-->

    <!-- Google fonts -->
    <script type="text/javascript">
        WebFontConfig = {
            google: { families: ['Arvo:700:latin', 'Open+Sans:400,600,700:latin'] }
        };
        (function () {
            var wf = document.createElement('script');
            wf.src = ('https:' == document.location.protocol ? 'https' : 'http') +
              '://ajax.googleapis.com/ajax/libs/webfont/1/webfont.js';
            wf.type = 'text/javascript';
            wf.async = 'true';
            var s = document.getElementsByTagName('script')[0];
            s.parentNode.insertBefore(wf, s);
        })();
    </script>
    <script src="bower_components/jquery/jquery.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Top Full Menus -->
        <uc1:TopMenu runat="server" ID="TopMenu" />

        <div class="breadcrumbs">
            <div class="container">
                <div class="row">
                    <div class="col-xs-12">
                        <nav>
                            <ol class="breadcrumb">

                                <li><a href="Default.aspx">Home</a></li>
                                <li><a href="Admin.aspx">Admin</a></li>
                                <li class="active">Reports</li>

                            </ol>
                        </nav>
                    </div>
                </div>
            </div>
        </div>

        <div class="container">
            <!-- Big banner -->
            <div class="row">
                <div class="push-down-30">
                    <div class="col-xs-8">

                       <h2>Coming Soon...</h2>
                    </div>
                   
                </div>
            </div>
        </div>
    </form>

    <uc1:BottomNav runat="server" ID="BottomNav" />
    <script src="scripts/toastr.js"></script>
    <script data-main="scripts/main" src="bower_components/requirejs/require.js"></script>
    <script src="scripts/app/default.js"></script>
    <script src="scripts/app/Admin.js"></script>

</body>
</html>
