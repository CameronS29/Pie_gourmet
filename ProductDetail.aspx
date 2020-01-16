<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ProductDetail.aspx.vb" Inherits="ProductDetail" %>

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
        <input type="hidden" id="MyProductID" value="0" runat="server" />

        <!-- Top Full Menus -->
        <uc1:TopMenu runat="server" ID="TopMenu" />

        <div class="container" id="content" style="margin-top: 30px">
            <div class="push-down-30">
                <div class="row">
                    <div class="col-xs-12  col-sm-8">

                        <div class="col-xs-12 col-sm-4">
                            <div class="product-preview">
                                <div class="push-down-20">
                                    <img id="ProductImage" runat="server" class="js--product-preview" alt="Single product image" src="images/products/no_photo.png" width="360" />
                                </div>
                                <div>
                                    <a class="btn btn-primary--transition" href="javascript:void(0)" onclick="AddPhoto()">
                                        <span class="glyphicon glyphicon-plus"></span>
                                        <span class="single-product__btn-text">Add Photo</span>
                                    </a>
                                </div>
                            </div>
                        </div>

                        <div class="col-xs-12 col-sm-8" style="margin-bottom: 20px">
                            <div class="products__content">
                                <div class="push-down-30"></div>

                                <span class="products__category" id="ProductCategory" runat="server">GROUP</span>
                                <h1 class="single-product__title" id="ProductName" runat="server"><span class="light">Select</span> Product</h1>

                                <span class="single-product__price" id="ProductPrice" runat="server">$0.00</span>

                                <div class="single-product__rating" id="ProductRating" runat="server">
                                </div>

                                <div class="in-stock--single-product" id="Active" runat="server">
                                    <span class="in-stock">&bull;</span> <span class="in-stock--text">In stock</span>
                                </div>

                                <div class="in-stock--single-product" id="InStock" runat="server">
                                    <span class="in-stock">&bull;</span> <span class="in-stock--text">In stock</span>
                                </div>

                                <div class="in-stock--single-product" id="Holiday" runat="server">
                                    <span class="in-stock">&bull;</span> <span class="in-stock--text">Seasonal</span>
                                </div>

                                <div class="in-stock--single-product" id="Featured" runat="server">
                                    <span class="in-stock">&bull;</span> <span class="in-stock--text">Featured</span>
                                </div>

                                <hr class="bold__divider" />
                                <p class="single-product__text" id="ProductDescription" runat="server"></p>



                                <hr class="bold__divider" />
                                <!-- Single button -->

                                <asp:Literal ID="ProductTypeSelect" runat="server"></asp:Literal>


                                <!-- Add to cart button -->
                                <a class="btn btn-info--transition" href="javascript:void(0)" onclick="AddNewProduct()">
                                    <span class="glyphicon glyphicon-plus"></span>
                                    <span class="single-product__btn-text">New </span>
                                </a>

                                <a class="btn btn-primary--transition" href="javascript:void(0)" onclick="EditProduct()">
                                    <span class="glyphicon glyphicon-check"></span>
                                    <span class="single-product__btn-text">Edit </span>
                                </a>

                            </div>
                        </div>

                        <!-- Tabs -->
                        <div class="push-down-30" style="margin-top: 20px">
                            <div class="row">
                                <div class="col-xs-12">
                                    <!-- Nav tabs -->
                                    <ul class="nav  nav-tabs">
                                        <li class="active"><a href="#tabDesc" data-toggle="tab">Description</a></li>
                                        <li><a href="#tabManufacturer" data-toggle="tab">Ingredients</a></li>
                                        <li><a href="#tabReviews" data-toggle="tab">Reviews (<span id="ReviewCount" runat="server"></span>)</a></li>
                                    </ul>
                                    <div class="tab-content">
                                        <div class="tab-pane  fade  in  active" id="tabDesc">
                                            <div class="row">
                                                <div class="col-xs-12  col-sm-6">
                                                    <h5>Description</h5>
                                                    <p class="tab-text" id="ProductDescription2" runat="server">Select Product...</p>
                                                </div>

                                            </div>
                                        </div>
                                        <div class="tab-pane  fade" id="tabManufacturer">
                                            <h5>Ingredients details</h5>
                                            <p class="tab-text" id="Indredients" runat="server">Select Product...</p>
                                        </div>
                                        <div class="tab-pane  fade" id="tabReviews">

                                            <div runat="server" id="ReviewText"></div>
                                        </div>
                                    </div>


                                </div>
                            </div>
                        </div>

                        <!-- Navigation -->
                        <div class="push-down-30">
                            <div class="">

                                <div>
                                    <h3>
                                        <span class="light">Ingredients</span> Maintenance
                                    </h3>
                                    <hr class="divider" />
                                    <button type="button" id="Button5" class="btn btn-primary--transition" onclick="NewIngredientWin()"><i class="glyphicon glyphicon-plus"></i> New </button>


                                </div>
                                <div id="IngredientList" runat="server"></div>
                            </div>
                        </div>

                        <!-- Products -->
                        <div class="push-down-30">
                            <div class="">
                                <div>
                                    <h3><span class="light">Order</span> History</h3>
                                    <hr class="divider" />
                                </div>
                                <div id="OrderHistory" runat="server"></div>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-12  col-sm-4">
                        <aside class="sidebar  sidebar--blog">

                            <div class="sidebar-container">
                                <h3><span class="light">Product</span> List</h3>
                                <hr>
                                <div class="input-group">
                                    <span class="input-group-addon">
                                        <i class="glyphicon  glyphicon-search"></i>
                                    </span>
                                    <input type="text" id="FilterProduct" class="form-control" placeholder="Filter Products..." onkeyup="ProductFilter(this.value)" />
                                </div>
                                <div class="small">
                                    <button type="button" id="fbtn1" class="btn btn-info btn-sm" onclick="FilterProductsCustom('Seasonal')"><i class="glyphicon glyphicon-filter"></i> Seasonal </button>
                                    <button type="button" id="fbtn2" class="btn btn-info btn-sm" onclick="FilterProductsCustom('Active')"><i class="glyphicon glyphicon-filter"></i> Active </button>
                                </div>
                                <div runat="server" id="ProductListEdit"></div>
                            </div>
                        </aside>
                    </div>
                </div>
            </div>
        </div>

        <!-- Order Sub-Status Edit Form -->
        <div class="modal fade" id="AddCommentForm" title="Product Comments">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h3><i class="entypo-tools"></i>Product Comment </h3>
                        <button class="close" aria-hidden="true" type="button" data-dismiss="modal">×</button>
                    </div>
                    <div class="modal-body">

                        <table class="table">
                            <tr>
                                <td class="field_name">City:</td>
                                <td>
                                    <input type="text" size="20" value="" id="CommentCity" class="form-control" /></td>
                            </tr>
                            <tr>
                                <td class="field_name">State:</td>
                                <td>
                                    <input type="text" size="10" value="" id="CommentState" class="form-control" /></td>
                            </tr>

                            <tr>
                                <td class="field_name">Comment:</td>
                                <td>
                                    <textarea id="ProductComment" rows="3" cols="40" class="form-control"></textarea></td>
                            </tr>

                            <tr>
                                <td colspan="2">
                                    <div id="CommentErrText" class="error"></div>
                                </td>
                            </tr>
                        </table>

                    </div>
                    <div class="modal-footer">
                        <button type="button" name="btn_addlsave" id="Button30" class="btn btn-primary--transition" onclick="SaveComment()"><i class="glyphicon glyphicon-check"></i>Save Comment </button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Product Edit Form -->
        <div class="modal fade" id="ProductForm" title="Product Edit">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #f2f2f2">
                        <h3><i class="entypo-tools"></i>Product Maintenance </h3>
                        <button class="close" aria-hidden="true" type="button" data-dismiss="modal">×</button>
                    </div>
                    <div class="modal-body">
                        <table class="table">
                            <tr>
                                <td class="field_name">Product ID:</td>
                                <td>
                                    <input type="text" size="20" value="" id="eProductID" class="form-control" readonly="readonly" /></td>
                            </tr>
                            <tr>
                                <td class="field_name">Name:</td>
                                <td>
                                    <input type="text" size="20" value="" id="eProductName" class="form-control bold" /></td>
                            </tr>
                            <tr>
                                <td class="field_name">Price Group:</td>
                                <td>
                                    <div class="input-group">
                                        <asp:DropDownList ID="epricing_group" class="form-control" runat="server" />
                                        <span class="input-group-addon">
                                            <a href="javascript:void(0)" id="btn21" onclick="GetPricing()" class="btn btn-info"><i class="glyphicon glyphicon-check"></i></a>
                                        </span>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="field_name">Ingredients:</td>
                                <td>
                                    <asp:DropDownList ID="eIngredientsID" class="form-control" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="field_name">Category:</td>
                                <td>
                                    <asp:DropDownList ID="eCategory" class="form-control" runat="server" /></td>
                            </tr>

                            <tr>
                                <td class="field_name">Flags:</td>
                                <td>
                                    <div>
                                        <b>Active:</b>
                                        <input type="checkbox" value="1" id="eActive" />
                                    </div>
                                    <div>
                                        <b>In Stock:</b>
                                        <input type="checkbox" value="1" id="eInStock" />
                                    </div>
                                    <div>
                                        <b>Seasonal:</b>
                                        <input type="checkbox" value="1" id="eFresh" />
                                    </div>
                                    <div>
                                        <b>Featured:</b>
                                        <input type="checkbox" value="1" id="eFeatured" />
                                    </div>
                                </td>
                            </tr>

                            <tr>
                                <td class="field_name">Rating:</td>
                                <td>
                                    <asp:DropDownList ID="eRating" class="form-control" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="field_name">Description:</td>
                                <td>
                                    <textarea rows="3" cols="40" id="eProductDescription" class="form-control"></textarea></td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <div id="edate_row" style="font-size: 9px"></div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <div id="prod_err" class="error"></div>
                                </td>
                            </tr>
                        </table>

                    </div>
                    <div class="modal-footer">
                        <button type="button" id="Button2" class="btn btn-info--transition" onclick="NewProduct()"><i class="glyphicon glyphicon-plus"></i> New Product </button>
                        <button type="button" id="Button1" class="btn btn-success" onclick="SaveProduct()"><i class="glyphicon glyphicon-check"></i> Save Product </button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Pricing Edit Form -->
        <div class="modal fade" id="PricingForm" title="Pricing Edit" role="dialog" aria-hidden="true">
            <input type="hidden" id="pricing_group" value="0" />
            <div class="modal-dialog" style="width: 80%">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #f2f2f2">
                        <h3><i class="entypo-tools"></i>Pricing Maintenance </h3>
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                        <div class="col-md-6">
                            <table class="table">
                                <tr>
                                    <td class="field_name">ID:</td>
                                    <td>
                                        <input type="text" size="20" value="" id="pricingID" class="form-control" readonly="readonly" /></td>
                                </tr>
                                <tr>
                                    <td class="field_name">Category:</td>
                                    <td>
                                        <input type="text" size="20" value="" id="TypeCategory" class="form-control" readonly="readonly" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="field_name">Quantity:</td>
                                    <td>
                                        <input type="text" size="20" value="" id="TypeQuantity" class="form-control" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="field_name">Price:</td>
                                    <td>
                                        <input type="text" size="20" value="" id="pProductPrice" class="form-control" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="field_name">Packaging:</td>
                                    <td>
                                        <input type="text" size="20" value="" id="Packaging" class="form-control" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="field_name">Weight:</td>
                                    <td>
                                        <input type="text" size="20" value="" id="ShipWeight" class="form-control" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="field_name">Size:</td>
                                    <td>
                                        <input type="text" size="20" value="" id="ProductSize" class="form-control" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <div id="pdate_row" style="font-size: 9px"></div>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <div id="price_err" class="error"></div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="col-md-6">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <i class="glyphicon  glyphicon-search"></i>
                                </span>
                               <asp:DropDownList ID="pSchemeFilter" class="form-control" runat="server" />
                            </div>
                            <div id="FillPricingList">[Pricing List]</div>
                        </div>
                        </div>
                    </div>
                
                <div class="modal-footer" style="background-color: #f2f2f2">
                    
                    <button type="button" id="Button11" class="btn btn-success" onclick="Savepricing()"><i class="glyphicon glyphicon-check"></i> Save Pricing </button>
                </div>
                </div>
            </div>
        </div>

        <!-- Ingredient Edit Form -->
        <div class="modal fade" id="IngredientForm" title="Ingredient Edit">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h3><i class="entypo-tools"></i>Ingredient Maintenance </h3>
                        <button class="close" aria-hidden="true" type="button" data-dismiss="modal">×</button>
                    </div>
                    <div class="modal-body">

                        <table class="table">
                            <tr>
                                <td class="field_name">Ingredient ID:</td>
                                <td>
                                    <input type="text" size="20" value="" id="IngredientID" class="form-control" readonly="readonly" /></td>
                            </tr>
                            <tr>
                                <td class="field_name">Name:</td>
                                <td>
                                    <input type="text" size="20" value="" id="IngredientName" class="form-control bold" /></td>
                            </tr>

                            <tr>
                                <td class="field_name">Ingredients:</td>
                                <td>
                                    <textarea rows="3" cols="40" id="Description" class="form-control"></textarea></td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <div id="idate_row" style="font-size: 9px"></div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <div id="ierr_text" class="error"></div>
                                </td>
                            </tr>
                        </table>

                    </div>
                    <div class="modal-footer">
                        <button type="button" id="Button3" class="btn btn-info--transition" onclick="New_Ingredients()"><i class="glyphicon glyphicon-plus"></i> New Ingredient </button>
                        <button type="button" id="Button4" class="btn btn-primary--transition" onclick="SaveIngredients()"><i class="glyphicon glyphicon-check"></i> Save Ingredient </button>
                    </div>
                </div>
            </div>
        </div>
    </form>

    <uc1:BottomNav runat="server" ID="BottomNav" />

    <!-- Placeholders.js provides IE 6-9 support of HTML5 placeholder -->
    <!--[if lte IE 9]>
        <script src="assets/js/plugins/placeholders.min.js"></script>
    <![endif]-->

    <!-- jQuery Version 1.11.0 -->
    <script src="assets/js/plugins/jquery-1.11.0.js"></script>

    <!-- Bootstrap Core JavaScript -->
    <script src="assets/bootstrap/js/bootstrap.min.js"></script>

    <script src="assets/app/toastr.js"></script>
    <script src="assets/app/Common.js"></script>
    <script src="assets/app/jquery.storageapi.min.js"></script>

    <script src="assets/app/default.js"></script>
    <script src="assets/app/ProductAdmin.js?v=1.2"></script>

</body>
</html>

