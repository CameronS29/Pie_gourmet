<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CurrentSpecials.aspx.vb" Inherits="CurrentSpecials" %>

<%@ Register Src="~/TopMenu.ascx" TagPrefix="uc1" TagName="TopMenu" %>
<%@ Register Src="~/BottomNav.ascx" TagPrefix="uc1" TagName="BottomNav" %>

<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="">
    <meta name="author" content="">

    <title>The Pie Gourmet - Specials Menu</title>

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

<body id="page-top" class="index">
    <form id="form1" runat="server">

        <!-- Navigation -->
        <header id="header" class="intro"></header>
        <!-- Top Full Menus -->
        <uc1:TopMenu runat="server" ID="TopMenu" />


        <!-- End Navigation -->
        
         <!-- Choose Specials Section -->
        <section id="Section2">
            <div class="container">
                <div class="row">
                    <div class="col-lg-12 text-center">
                        <h2>Current Specials</h2>
                    </div>
                    
                </div>
                <div class="line"></div>
                <div class="row">
                    <div class="col-lg-12" runat="server" id="SpecialsText">
                    </div>
                </div>
                <!-- /.row -->
            </div>
            <!-- /.container -->
        </section>
        <!-- End Specials Section -->

     
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

    <!-- Google Maps -->
    <script src="//maps.google.com/maps/api/js?sensor=true"></script>

    <!-- Foodster JavaScript -->
    <script src="assets/js/foodster.js"></script>

    <!-- Add fancyBox -->
    <link rel="stylesheet" href="assets/fancybox/jquery.fancybox.css?v=2.1.5" type="text/css" media="screen" />
    <script type="text/javascript" src="assets/fancybox/jquery.fancybox.pack.js?v=2.1.5"></script>

    <!-- Only run this JS on this page -->
    <script>
        $(document).scroll(function () {
            "use strict";
            // Add and remove the navbar-shrink class for fixed navigation on page scroll
            if ($(this).scrollTop() >= $('header').position().top) {
                $('nav').addClass('navbar-shrink');
            }

            if ($(window).scrollTop() < $('header').height() + 1) {
                $('nav').removeClass('navbar-shrink');
            }

            if ($(this).scrollTop() >= $('header').position().top) {
                $('#intro').addClass('intro-shrink');
            }

            if ($(window).scrollTop() < $('header').height() + 1) {
                $('#intro').removeClass('intro-shrink');
            }
        });
    </script>

    <!-- Your Custom JavaScript -->
    <script src="assets/js/custom.js"></script>
    <script src="assets/app/toastr.js"></script>
    <script src="assets/app/Common.js"></script>
    <script src="assets/app/hideShowPassword.min.js"></script>
    <script src="assets/app/jquery.storageapi.min.js"></script>
    <script src="assets/app/default.js"></script>

    <!-- Placeholders.js provides IE 6-9 support of HTML5 placeholder -->
    <!--[if lte IE 9]>
        <script src="assets/js/plugins/placeholders.min.js"></script>
    <![endif]-->

</body>
</html>

