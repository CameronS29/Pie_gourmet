<%@ Page Language="VB" AutoEventWireup="false" CodeFile="menu.aspx.vb" Inherits="menu" %>

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

    <title>The Pie Gourmet - Menu</title>

    <!-- Retina.js -->
    <!-- WARNING: Retina.js doesn't work if you view the page via file:// -->
    <script src="assets/js/plugins/retina.min.js"></script>

    <!-- Bootstrap Core CSS -->
    <link href="assets/bootstrap/css/bootstrap.min.css" rel="stylesheet">

    <!-- Animate CSS -->
    <link href="assets/css/animate.min.css" rel="stylesheet">
    <link href="assets/css/main.css" rel="stylesheet" />

    <!-- Fonts -->
    <link href='//fonts.googleapis.com/css?family=National' rel='stylesheet' type='text/css'>

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
        
        <!-- Callout section start -->
        <section id="menu-one" class="callout">
        </section>
        <!-- Callout section end -->

        <!-- Menu start -->
        <section class="module">
            <div class="container">

                <div class="row">
                    <center>
        	            <!--<h6>build whatever you're craving in three easy steps</h6>-->
                      <h2>Our Fruit Pies</h2>
                    </center>
                </div>

                <div class="line"></div>

                <div class="row" id="FruitList" runat="server">
                    [Fruit Product List]
        
                </div>
                <!-- .row -->
            </div>
            <!-- .container -->
        </section>

        <!-- Menu end -->

        <!-- Callout section start -->

        <section id="menu-two" class="callout">
        </section>

        <!-- Callout section end -->

        <!-- Menu start -->

        <section class="module">
            <div class="container">

                <div class="row">
                    <center>
        	            <!--<h6>special combinations from craft beer dinners and burger competitions</h6>-->
                      <h2>Our Sweet Pies</h2>
                    </center>
                </div>

                <div class="line"></div>

                <div class="row" id="SweetList" runat="server">
                    [Sweet Pie Product List]

                </div>
                <!-- .row -->

            </div>
            <!-- .container -->
        </section>


        <!-- Callout section start -->

        <section id="menu-three" class="callout">
        </section>

        <!-- Callout section end -->

        <!-- Menu start -->

        <section class="module wow fadeIn">
            <div class="container">

                <div class="row">
                    <center>
        	            <!--<h6>the perfect burger isn't complete without the perfect accompaniment</h6>-->
                      <h2>Our Savory Pies & Quiches</h2>
                    </center>
                </div>

                <div class="line"></div>

                <div class="row" id="DinnerList" runat="server">
                    [Dinner Pie Product List]

                </div>
                <!-- .row -->

            </div>
            <!-- .container -->
        </section>

        <!-- Menu end -->

        <!-- Callout section start -->

        <section id="menu-four" class="callout">
            <div class="container">
            </div>
            <!-- .container -->
        </section>

        <!-- Callout section end -->

        <!-- Menu start -->

        <section class="module wow fadeIn">
            <div class="container">

                <div class="row">
                    <center>
                        <h2>Cheesecakes</h2>
                    </center>
                </div>

                <div class="line"></div>

                <div class="row" id="CheesecakeList" runat="server">
                    [Cheesecake Product List]
                </div>
                <!-- .row -->

            </div>
            <!-- .container -->
        </section>

        <!-- Menu end -->

        <!-- Callout section start -->

        <section id="menu-five" class="callout">
            <div class="container">
            </div>
            <!-- .container -->
        </section>

        <!-- Callout section end -->

        <!-- Menu start -->

        <section class="module wow fadeIn">
            <div class="container">

                <div class="row">
                    <center>
                        <h2>Seasonal Specials</h2>
                    </center>
                </div>

                <div class="line"></div>

                <div class="row" id="SeasonalList" runat="server">
                    [Seasonal Product List]

                </div>
                <!-- .row -->

            </div>
            <!-- .container -->
        </section>

        <!-- Menu end -->

        <!-- Callout section start -->

        <section id="menu-six" class="callout">
            <div class="container">
            </div>
            <!-- .container -->
        </section>

        <!-- Callout section end -->

        <!-- Menu start -->

        <section class="module wow fadeIn">
            <div class="container">
                <div class="row">
                    <center>
      		            <!--<h6>specially designed to wash it all down</h6>-->
        	            <h2>Other Specialty Items</h2>
       	            </center>
                </div>

                <div class="line"></div>

                <div class="row">

                    <div class="col-sm-4">
                        <div class="row">
                            <div class="col-sm-12">
                                <h4 class="menu-title">Seasonal + Special Order Pies</h4>
                                <p><strong>Seasonal Pies</strong></p>
                                <ul>
                                    <li>Apple Strawberry (<em>Spring-Summer</em>)</li>
                                    <li>Blueberry (<em>Summer</em>)</li>
                                    <li>Mince Meat (<em>Winter</em>)</li>
                                    <li>Plum Walnut (<em>Summer-Fall</em>)</li>
                                    <li>Pumpkin (<em>Fall</em>)</li>
                                    <li>Rhubarb (<em>Spring</em>)</li>
                                    <li>Strawberry (<em>Spring-Summer</em>)</li>
                                    <li>Strawberry Rhubarb (<em>Spring</em>)</li>
                                    <li>Sweet Potato (<em>Fall</em>)</li>
                                </ul>
                                <br>
                                <p><strong>Special Order</strong></p>
                                <ul>
                                    <li>Apple-Blackberry</li>
                                    <li>Apple-Cranberry</li>
                                    <li>Chocolate Mousse</li>
                                    <li>Lemon Meringue</li>
                                    <li>Peach Melba</li>
                            </div>
                        </div>
                    </div>
                    <!-- .col-sm-4 -->


                    <div class="col-sm-4">
                        <div class="row">
                            <div class="col-sm-12">
                                <h4 class="menu-title">Desserts</h4>
                                <ul>
                                    <li>Banana Loaf</li>
                                    <li>Carrot Cake Muffin</li>
                                    <li>Coffee Cakes (Variety)</li>
                                    <li>Marble Cheesecake Brownies</li>
                                    <li>Mini Muffins</li>
                                    <li>Muffins</li>
                                    <li>Pie Slices</li>
                                    <li>Pumpkin Bars</li>
                                    <li>Scones</li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    <!-- .col-sm-2 -->

                    <div class="col-sm-4">
                        <div class="row">
                            <div class="col-sm-12">
                                <h4 class="menu-title">Breads</h4>
                                <ul>
                                    <li>Challah</li>
                                    <li>Dinner Rolls</li>
                                    <li>Irish Soda Bread</li>
                                    <li>Honey Whole Wheat</li>
                                    <li>Orange Date & Nut Loaf</li>
                                    <li>Whole Wheat & Date Nut</li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    <!-- .col-sm-2 -->

                </div>
                <!-- .row -->
            </div>
            <!-- .container -->
        </section>

        <!-- Menu end -->
        
       
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
    
     <!-- AddRoll Section -->
    <script type="text/javascript"> adroll_adv_id = "3POYGIA4GFHOHHQ4JB2MDR"; adroll_pix_id = "KLCQNDEAMNC4LCKOZSGQML"; /* OPTIONAL: provide email to improve user identification */ /* adroll_email = "username@example.com"; */ (function () { var _onload = function(){ if (document.readyState && !/loaded|complete/.test(document.readyState)){setTimeout(_onload, 10);return} if (!window.__adroll_loaded){__adroll_loaded=true;setTimeout(_onload, 50);return} var scr = document.createElement("script"); var host = (("https:" == document.location.protocol) ? "https://s.adroll.com" : "http://a.adroll.com"); scr.setAttribute('async', 'true'); scr.type = "text/javascript"; scr.src = host + "/j/roundtrip.js"; ((document.getElementsByTagName('head') || [null])[0] || document.getElementsByTagName('script')[0].parentNode).appendChild(scr); }; if (window.addEventListener) {window.addEventListener('load', _onload, false);} else {window.attachEvent('onload', _onload)} }()); </script>

</body>
</html>

