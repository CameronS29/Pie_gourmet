<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ShopDetail.aspx.vb" Inherits="ShopDetail" %>

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

    <title>Pie Gourmet :: My Account</title>

    <!-- Retina.js -->
    <!-- WARNING: Retina.js doesn't work if you view the page via file:// -->
    <script src="assets/js/plugins/retina.min.js"></script>

    <!-- Bootstrap Core CSS -->
    <link href="assets/bootstrap/css/bootstrap.min.css" rel="stylesheet" />

    <!-- Animate CSS -->
    <link href="assets/css/animate.min.css" rel="stylesheet" />
    <link href="assets/css/main.css" rel="stylesheet" />

    <!-- Fonts -->
    <link href='//fonts.googleapis.com/css?family=National' rel='stylesheet' type='text/css' />

    <!--Font Awesome-->
    <link href="assets/fonts/fontawesome/css/font-awesome.min.css" rel="stylesheet" />

    <!-- Main CSS -->
    <link href="assets/css/foodster.css" rel="stylesheet" />

    <!-- Your custom CSS -->
    <link href="assets/css/custom.css" rel="stylesheet" />

    <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
    <!--[if lt IE 9]>
        <script src="assets/js/plugins/htmlshiv.min.js"></script>
        <script src="assets/js/plugins/respond.min.js"></script>
    <![endif]-->

    
</head>
<body>
    <form id="form1" runat="server">
        <input type="hidden" id="MyProductID" value="0" runat="server" />

        <!-- Top Full Menus -->
        <uc1:TopMenu runat="server" ID="TopMenu" />

        <div class="container">
            <div style="margin-top: 30px; margin-bottom: 30px">
                <div class="row">
                    <div class="col-xs-12 col-sm-4">
                        <div class="product-preview">
                            <div class="push-down-20">
                                <a href="#" class="fancybox" runat="server" id="ProdImgHref">
                                <img id="ProductImage" runat="server" class="js--product-preview" alt="Single product image" src="images/dummy/w360/13.jpg" width="360" height="458" />
                                </a>
                            </div>
                            
                        </div>
                    </div>

                    <div class="col-xs-12 col-sm-8">
                        <div class="products__content">
                            <div class="push-down-30"></div>

                            <span class="products__category" id="ProductCategory" runat="server">Muesli</span>
                            <h1 class="single-product__title" id="ProductName" runat="server"><span class="light">Fresh</span> Super Fruit</h1>
                            <span class="single-product__price" id="ProductPrice" runat="server">$4.77</span>
                            <div class="single-product__rating" id="ProductRating" runat="server">
                               
                            </div>

                            <div class="in-stock--single-product" id="InStock" runat="server">
                                <span class="in-stock">&bull;</span> <span class="in-stock--text">In stock</span>
                            </div>
                            <hr class="bold__divider" />
                            <p class="single-product__text" id="ProductDescription" runat="server">
                                Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus vulputate, leo vel malesuada tincidunt, purus nunc tristique erat, at elementum tellus mi nec mi. Nunc rutrum ullamcorper. In hac habitasse platea tumst. Vestibulum lorem dolor, pharetra sit amet urna nec, hendrerit scelerisque risus. Vestibulum lorem dolor,  pharetra sit amet urna nec, hendrerit scelerisque risus. In hac habitasse platea dictumst.
         
                            </p>
                            <hr class="bold__divider" />
                            <!-- Single button -->

                            <asp:Literal ID="ProductTypeSelect" runat="server"></asp:Literal>

                            <!-- Quantity buttons -->
                            <div class="quantity  js--quantity">
                                <input type="button" value="-" class="quantity__button  js--minus-one  js--clickable" />
                                <input type="text" id="quantity" name="quantity" value="1" class="quantity__input" readonly="readonly" />
                                <input type="button" value="+" class="quantity__button  js--plus-one  js--clickable" />
                            </div>

                            <!-- Add to cart button -->
                            <a id="btnAddToCart" class="btn btn-primary" href="javascript:void(0)" onclick="AddToCartDetail()" runat="server">
                                <span class="glyphicon glyphicon-plus"></span><span class="glyphicon glyphicon-shopping-cart"></span>
                                <span class="single-product__btn-text">Add to shopping cart</span>
                            </a>
                           
                        </div>
                    </div>
                </div>
            </div>

            <!-- Tabs -->
            <div class="push-down-30">
                <div class="row">
                    <div class="col-xs-12">
                        <!-- Nav tabs -->
                        <ul class="nav  nav-tabs">
                            <li class="active"><a href="#tabDesc" data-toggle="tab">Description</a></li>
                            <li><a href="#tabManufacturer" data-toggle="tab">Ingredients</a></li>
                            <li><a href="#tabReviews" data-toggle="tab">Reviews (<span id="ReviewCount" runat="server"></span>)</a></li>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane fade in active" id="tabDesc" style="padding-left: 20px">
                                <div class="row">
                                    <div class="col-xs-12  col-sm-6">
                                        <h5>Description</h5>
                                        <p class="tab-text" id="ProductDescription2" runat="server">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce est purus, fringilla sit amet arcu quis, feugiat ultrices metus. Vestibulum lorem dolor, pharetra sit amet urna nec, hendrerit scelerisque risus. In hac habitasse platea dictumst. Vestibulum lorem dolor, pharetra sit amet urna nec, hendrerit scelerisque risus. Vestibulum lorem dolor, pharetra sit amet urna nec, hendrerit scelerisque risus. In hac habitasse platea dictumst.</p>
                                    </div>
                                   
                                </div>
                            </div>
                            <div class="tab-pane  fade" id="tabManufacturer" style="padding-left: 20px">
                                <h5>Ingredients details</h5>
                                <p class="tab-text" id="Indredients" runat="server">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce est purus, fringilla sit amet arcu quis, feugiat ultrices metus. Vestibulum lorem dolor, pharetra sit amet urna nec, hendrerit scelerisque risus. In hac habitasse platea dictumst.</p>
                            </div>
                            <div class="tab-pane  fade" id="tabReviews" style="padding-left: 20px">
                                 <div class="pull-right">
                                    <a href="javascript:void(0)" class="btn btn-warning--transition" data-toggle="modal" onclick="AddComment()"><i class="glyphicon glyphicon-plus"></i> Add Review</a>
                                    
                                </div>
                                <h5>Reviews</h5>
                                <asp:Literal runat="server" ID="ReviewText"></asp:Literal>
                            </div>
                        </div>


                    </div>
                </div>
            </div>

            <!-- Navigation -->
            <%--<div class="push-down-30">
                <div class="products-navigation">
                    <div class="products-navigation__title">
                        <h3><span class="light">Related</span> Products</h3>
                    </div>
                </div>
            </div>

            <!-- Products -->
            <div class="push-down-30">
                <div class="row">
                    <!-- Related Products -->
                    <asp:Literal ID="RelatedProductList" runat="server"></asp:Literal>
                </div>
            </div>--%>
        </div>
        
         <!-- Order Sub-Status Edit Form -->
        <div class="modal fade" id="AddCommentForm" title="Product Comments">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h3><i class="entypo-tools"></i> Product Comment </h3>
                        <button class="close" aria-hidden="true" type="button" data-dismiss="modal">×</button>
                    </div>
                    <div class="modal-body">
                       
                        <table class="table">
                            <tr>
                                <td class="field_name">Name:</td>
                                <td>
                                    <input type="text" size="20" value="" id="CommentName" class="form-control" /></td>
                            </tr>
                            <tr>
                                <td class="field_name">City:</td>
                                <td>
                                    <input type="text" size="20" value="" id="CommentCity" class="form-control" /></td>
                            </tr>
                            <tr>
                                <td class="field_name">State:</td>
                                <td>
                                    <input type="text" size="10" value="" id="CommentState" class="form-control" maxlength="2" /></td>
                            </tr>
                            
                            <tr>
                                <td class="field_name">Comment:</td>
                                <td><textarea id="ProductComment" rows="3" cols="40" class="form-control"></textarea></td>
                            </tr>
                             <tr>
                                <td class="field_name">Rating:</td>
                                <td>
                                    <div id="CommentRating">
                                       <input type="radio" id="rate1" name="rating" value="1" /> 1
                                        <input type="radio" id="Radio1" name="rating" value="2" /> 2
                                        <input type="radio" id="Radio2" name="rating" value="3" /> 3
                                        <input type="radio" id="Radio3" name="rating" value="4" /> 4
                                        <input type="radio" id="Radio4" name="rating" value="5" /> 5
                                        Star(s)
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <div id="CommentErrText" class="error"></div>
                                </td>
                            </tr>
                        </table>
                        
                    </div>
                    <div class="modal-footer">
                        <button type="button" name="btn_addlsave" id="Button30" class="btn btn-primary--transition" onclick="SaveComment()"><i class="glyphicon glyphicon-check"></i>  Save Comment </button>
                    </div>
                </div>
            </div>
        </div>
    </form>

    <uc1:BottomNav runat="server" ID="BottomNav" />
    <div class="scroll-up">
        <a class="page-scroll" href="#nav"><i class="fa fa-angle-double-up"></i></a>
    </div>

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
    
     <!-- Add fancyBox -->
    <link rel="stylesheet" href="assets/fancybox/jquery.fancybox.css?v=2.1.5" type="text/css" media="screen" />
    <script type="text/javascript" src="assets/fancybox/jquery.fancybox.pack.js?v=2.1.5"></script>

    <!-- Your Custom JavaScript -->
    <script src="assets/js/custom.js"></script>
    <script src="assets/app/toastr.js"></script>
    <script src="assets/app/Common.js"></script>
    <script src="assets/app/hideShowPassword.min.js"></script>
    <script src="assets/app/jquery.storageapi.min.js"></script>
    <script src="assets/app/utils.js"></script>
    <script src="assets/app/default.js"></script>
    
     <!-- AddRoll Section -->
    <script type="text/javascript"> adroll_adv_id = "3POYGIA4GFHOHHQ4JB2MDR"; adroll_pix_id = "KLCQNDEAMNC4LCKOZSGQML"; /* OPTIONAL: provide email to improve user identification */ /* adroll_email = "username@example.com"; */ (function () { var _onload = function(){ if (document.readyState && !/loaded|complete/.test(document.readyState)){setTimeout(_onload, 10);return} if (!window.__adroll_loaded){__adroll_loaded=true;setTimeout(_onload, 50);return} var scr = document.createElement("script"); var host = (("https:" == document.location.protocol) ? "https://s.adroll.com" : "http://a.adroll.com"); scr.setAttribute('async', 'true'); scr.type = "text/javascript"; scr.src = host + "/j/roundtrip.js"; ((document.getElementsByTagName('head') || [null])[0] || document.getElementsByTagName('script')[0].parentNode).appendChild(scr); }; if (window.addEventListener) {window.addEventListener('load', _onload, false);} else {window.attachEvent('onload', _onload)} }()); </script>

</body>
</html>
