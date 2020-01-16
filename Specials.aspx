<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Specials.aspx.vb" Inherits="Specials" %>

<%@ Register Src="~/TopMenu.ascx" TagPrefix="uc1" TagName="TopMenu" %>
<%@ Register Src="~/BottomNav.ascx" TagPrefix="uc1" TagName="BottomNav" %>
<%@ Register TagPrefix="editor" Assembly="WYSIWYGEditor" Namespace="InnovaStudio" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="">
    <meta name="author" content="">

    <link rel="icon" type="image/ico" href="images/favicon.ico" />

    <title>Pie Gourmet :: Announcements</title>

    <!-- Retina.js -->
    <!-- WARNING: Retina.js doesn't work if you view the page via file:// -->
    <script src="assets/js/plugins/retina.min.js"></script>

    <!-- Bootstrap Core CSS -->
    <link href="assets/bootstrap/css/bootstrap.min.css" rel="stylesheet">

    <!-- Animate CSS -->
    <link href="assets/css/animate.min.css" rel="stylesheet">
    <link href="assets/css/main.css" rel="stylesheet" />

    <!-- Fonts -->
    <link href='//fonts.googleapis.com/css?family=Economica%7COld+Standard+TT:400,400italic,700' rel='stylesheet' type='text/css'>
    <link href='//fonts.googleapis.com/css?family=Voltaire' rel='stylesheet' type='text/css'>
    <link href='//fonts.googleapis.com/css?family=Lato:300,400,700.900,300italic,400italic' rel='stylesheet' type='text/css'>
    <link href="//fonts.googleapis.com/css?family=Oleo+Script" rel="stylesheet" type="text/css">

    <!--Font Awesome-->
    <link href="assets/fonts/fontawesome/css/font-awesome.min.css" rel="stylesheet">

    <!-- Main CSS -->
    <link href="assets/css/foodster.css" rel="stylesheet">

    <!-- Your custom CSS -->
    <link href="assets/css/custom.css" rel="stylesheet">

    <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
    <!--[if lt IE 9]>
        <script src="assets/js/plugins/htmlshiv.min.js"></script>
        <script src="assets/js/plugins/respond.min.js"></script>
    <![endif]-->


</head>
<body>
    <form id="form1" runat="server">
        <input type="hidden" id="PageLoadType" runat="server" />

        <!-- Top Full Menus -->
        <uc1:TopMenu runat="server" ID="TopMenu" />

        <div class="container">
            <!-- Big banner -->
            
            <div class="row">
                <div class="col-xs-12  col-sm-12 push-down-30" id="MyButtonRow" runat="server">
                    [Button Row]
                </div>
                <div class="col-xs-12  col-sm-12 push-down-30" id="SpecialPage" runat="server">
                    <editor:WYSIWYGEditor ID="oEdit1" runat="server" Height="800px" Width="100%"></editor:WYSIWYGEditor>
                </div>
            </div>
        </div>
    </form>

    <uc1:BottomNav runat="server" ID="BottomNav" />
    <!-- jQuery Version 1.11.0 -->
    <script src="assets/js/plugins/jquery-1.11.0.js"></script>

    <!-- Bootstrap Core JavaScript -->
    <script src="assets/bootstrap/js/bootstrap.min.js"></script>

    <!-- jQuery Easing -->
    <script src="assets/js/plugins/jquery.easing.1.3.min.js"></script>

    <!-- WOW plugin (used for animated sections) -->
    <script src="assets/js/plugins/wow.min.js"></script>

    <!-- jQuery Bootstrap Validation for Booking Form -->
    <script src="assets/js/plugins/jqBootstrapValidation.js"></script>

    <!-- Foodster JavaScript -->
    <script src="assets/js/foodster.js"></script>

    <!-- Your Custom JavaScript -->
    <script src="assets/js/custom.js"></script>


    <!-- Placeholders.js provides IE 6-9 support of HTML5 placeholder -->
    <!--[if lte IE 9]>
        <script src="assets/js/plugins/placeholders.min.js"></script>
    <![endif]-->
    <script src="assets/app/toastr.js"></script>
    <script src="assets/app/Common.js"></script>
    <script src="assets/app/hideShowPassword.min.js"></script>
    <script src="assets/app/jquery.storageapi.min.js"></script>
    <script src="assets/app/default.js"></script>
    <script src="assets/app/Admin.js"></script>

</body>
</html>