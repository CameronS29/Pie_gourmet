<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

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
    <link rel="icon" type="image/ico" href="images/favicon.ico" />

    <title>The Pie Gourmet</title>

    <!-- Retina.js -->
    <!-- WARNING: Retina.js doesn't work if you view the page via file:// -->
    <script src="assets/js/plugins/retina.min.js"></script>

    <!-- Bootstrap Core CSS -->
    <link href="assets/bootstrap/css/bootstrap.min.css" rel="stylesheet">

    <!-- Animate CSS -->
    <link href="assets/css/animate.min.css" rel="stylesheet">
    <link href="assets/css/main.css" rel="stylesheet" />

    <!-- Fonts -->
    <link href='//fonts.googleapis.com/css?family=National' rel='stylesheet' type='text/css' />
    <link href="assets/fonts/fontawesome/css/font-awesome.min.css" rel="stylesheet" />

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

        <!-- Top Full Menus -->
        <uc1:TopMenu runat="server" ID="TopMenu" />


        <!-- End Navigation -->

        <!-- Introduction Section -->
        <div class="row" >
            <img src="assets/images/PG/home/TopImage.jpg" class="home-image" />
        </div>
        <section id="intro">
            <div class="container">

                <div class="row">
                    <div class="col-md-6">
                        <h2>Our Story</h2>
                        <p class="large">Since our humble start in 1987. We opened the doors to our current location in 1988 and have been proudly serving our homemade artisan pies and baked goods to thousands of customers we’ve come to know and love in our hometown of Vienna, VA!</p>
                    </div>
                    <div class="col-md-6">
                        <img src="assets/images/PG/home/home-intro1.jpg" class="home-image" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6">
                        <img src="assets/images/PG/home/home-intro2.jpg" class="home-image" />
                    </div>
                    <div class="col-md-6">
                        <h2>Our Pies</h2>
                        <p class="large">All of our pies are handmade from scratch and crafted with love. We start off each day by peeling and coring locally grown apples that have become the staple of many of our delicious recipes. We believe the rich natural flavors propelled by the perfect combination of fresh local produce and the right mix of herbs and spices are unparalleled in taste. Therefore, no artificial flavoring or preservatives are used in our delectable goods.</p>
                        <br>
                        <a href="menu.aspx">
                            <h6>See Our Menu &nbsp; <i class="fa fa-angle-double-right"></i></h6>
                        </a>

                    </div>

                </div>

            </div>
            <!-- /.container -->
        </section>
        <!-- End Introduction Section -->



        <!-- Menus Section -->
        <section id="menus">
            <div class="container">
                
                <!-- /.row -->
            </div>
            <!-- /.container -->
        </section>
        <!-- End Menus Section -->



        <!-- Book Section -->
        <section id="book">
            <div class="container">
                <div class="row">
                    <div class="col-lg-10 col-lg-offset-1 text-center">
                        <h2>Ordering Made Easy</h2>
                    </div>
                </div>
                <br>
                <div class="row">
                    <div class="col-lg-12">

                        <table style="width: 100%">
                            <tr>
                                <td style="width: 50%; vertical-align: top; padding: 10px">
                                    <h6>ORDER PICK-UP BY PHONE</h6>
                                    <p class="large">Have a favorite pie? No problem! Feel free to give us a call and reserve a pie to come pick up later that day. Want to make a special request? There’s a good chance we’ll be able to help with that. Simply give us a call 24-hours in advance and we’ll bake a masterpiece just for you :)</p>
                                    <p><em>Call (703) 281-7437</em></p>
                                </td>
                                <td style="width: 50%; vertical-align: top; padding: 10px">
                                    <h6>ORDER LOCAL AND NATIONAL DELIVERY BY PHONE</h6>
                                    <p class="large">Can’t swing by the store? Give us call and let us know where you are and we’ll hand deliver your delicious pie anywhere in the metropolitan area or ship nationally across the country! </p>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 50%; vertical-align: top; padding: 10px">
                                    <h6>ORDER PICK-UP ONLINE</h6>
                                    <p class="large">In a rush and want to get in and out. Order your pie and pay online! When you arrive, simply walk in and give us your name and we’ll get you on your way!</p>

                                </td>
                                <td style="width: 50%; vertical-align: top; padding: 10px">
                                    <h6>ORDER LOCAL AND NATIONAL DELIVERY ONLINE</h6>
                                    <p class="large">
                                        Don’t feel like picking up the phone? No worries! Stay seated and order your pies online. We’ll have a pie waiting for you at your door before you know it!
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </div>

                </div>
                <!-- /.row -->
            </div>
            <!-- /.container -->
        </section>
        <!-- End Book Section -->



        <!-- Unique Chain Section -->
        <section id="unique">
            <div class="container">
                <div class="row">
                    <div class="col-lg-12 text-center">
                        <h2>Our Reviews</h2>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-lg-4">
                        <p class="large">"Stopped in here because it was next to a sandwich shop and was glad I did. The slice of pecan pie was fabulous with a very short crust that I know was homemade because I saw the owner in back making it. The filling was loaded with pecans and was not too sweet. They also have savory pies (I wish I would have known that before I bought the sandwich). Highly recommended!" -Mike B.</p>
                        <img src="images/reviews/tripadvisor.png">
                    </div>
                    <div class="col-lg-4">
                        <p class="large">“Best Pies Ever!!!! Can’t get enough of them pies, especially the apple pie. It’s so delicious!!! Their quiches are great too and their beef lasagna is so good every time I serve it, people think it is homemade! YUM! Yum! YUM!!!" -Lulu B.</p>
                        <img src="images/reviews/tripadvisor.png">
                    </div>
                    <div class="col-lg-4">
                        <p class="large">“The Pie Gourmet is a unique little bakery that lives up to its name. I have tried a variety of their pies, and both savory and sweet are excellent. Their apple pie is excellent! I would also recommend them for holiday pies if you are not inclined to bake. Call to order to avoid being disappointed. They are exceptionally busy…" -Gadget357</p>
                        <img src="images/reviews/tripadvisor.png">
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-2">
                        <p class="large">“The crust was dedicate and flavorful. The apple pie had just the right amount of cinnamon and spice. The pie was perfectly baked. Super delicious!" -<em><a style="target-new: tab;" target="_blank" href="http://www.yelp.com/biz/big-buns-arlington">Mark E.</a></em></p>
                        <a href="https://www.yelp.com/biz/the-pie-gourmet-vienna" target="_blank">
                            <img src="assets/images/reviews/yelp.png"></a>
                    </div>
                    <div class="col-lg-2 col-lg-offset-8">
                        <p class="large">"Their Marble Brownie is the best in the region.  A must have at every visit! -<em><a style="target-new: tab;" target="_blank" href="http://www.yelp.com/biz/big-buns-arlington">Drider S.</a></em></p>
                        <a href="https://www.yelp.com/biz/the-pie-gourmet-vienna" target="_blank">
                            <img src="assets/images/reviews/yelp.png"></a>
                    </div>
                </div>
            </div>
            <!-- /.container -->
        </section>
        <!-- End Unique Chain Section -->

        <!-- Choose Restaurant Section -->
        <section id="Section1">
            <div class="container">
                <div class="row">
                    <div class="col-lg-12 text-center">
                        <h2>Give us a shout.</h2>
                    </div>

                </div>
                <div class="row">
                    <div class="col-lg-10 col-lg-offset-1">
                        <div class="hr1"></div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12 text-center">
                        <h2>Join our email list for special offers and promotions.</h2>
                    </div>
                </div>
                <br>
                <div class="row">
                    <div class="col-md-6 col-md-offset-3">

                        <!-- BEGIN MAILCHIMP SIGNUP  -->
                        <div id="mc_embed_signup">
                            <form id="newsletter" class="form-" role="form" action="http://piegourmet.us10.list-manage.com/subscribe/post?u=f2205e5fba49006f0785ea658&amp;id=0800518382" method="post" id="mc-embedded-subscribe-form" name="mc-embedded-subscribe-form" class="validate" target="_blank">
                                <div id="mc_embed_signup_scroll">
                                    <input type="email" value="" name="EMAIL" class="required email form-control input-large" id="mce-EMAIL" placeholder="Enter your email here..." required>
                                    <div id="mce-responses" class="clear">
                                        <div class="response hide" id="mce-error-response"></div>
                                        <div class="response hide" id="mce-success-response"></div>
                                    </div>
                                    <!-- real people should not fill this in and expect good things - do not remove this or risk form bot signups-->
                                    <br>
                                    <center>
                  	                    <div class="bots"><input type="hidden" name="b_f2205e5fba49006f0785ea658_0800518382" tabindex="-1" value=""></div>
                  	                    <div class="clear"><input type="submit" value="Subscribe" name="Subscribe" id="mc-embedded-subscribe" class="btn btn-xl btn-primary"></div>
                	                </center>
                                </div>
                            </form>
                        </div>
                        <!-- END MAILCHIMP SIGNUP  -->

                    </div>
                </div>
                <!-- /.row -->
            </div>
            <!-- /.container -->
        </section>
        <!-- End Restaurants Section -->



        <uc1:BottomNav runat="server" ID="BottomNav" />

        <!-- End Footer Section -->
    </form>

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

    <script src="assets/app/toastr.js"></script>
    <script src="assets/app/Common.js"></script>
    <script src="assets/app/default.js"></script>


    <!-- Placeholders.js provides IE 6-9 support of HTML5 placeholder -->
    <!--[if lte IE 9]>
        <script src="assets/js/plugins/placeholders.min.js"></script>
    <![endif]-->

    <!-- AddRoll Section -->
    <script type="text/javascript"> adroll_adv_id = "3POYGIA4GFHOHHQ4JB2MDR"; adroll_pix_id = "KLCQNDEAMNC4LCKOZSGQML"; /* OPTIONAL: provide email to improve user identification */ /* adroll_email = "username@example.com"; */ (function () { var _onload = function () { if (document.readyState && !/loaded|complete/.test(document.readyState)) { setTimeout(_onload, 10); return } if (!window.__adroll_loaded) { __adroll_loaded = true; setTimeout(_onload, 50); return } var scr = document.createElement("script"); var host = (("https:" == document.location.protocol) ? "https://s.adroll.com" : "http://a.adroll.com"); scr.setAttribute('async', 'true'); scr.type = "text/javascript"; scr.src = host + "/j/roundtrip.js"; ((document.getElementsByTagName('head') || [null])[0] || document.getElementsByTagName('script')[0].parentNode).appendChild(scr); }; if (window.addEventListener) { window.addEventListener('load', _onload, false); } else { window.attachEvent('onload', _onload) } }()); </script>

</body>
</html>
