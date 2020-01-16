<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Checkout.aspx.vb" Inherits="Checkout" %>

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

    <title>Pie Gourmet :: Checkout</title>

    <!-- Retina.js -->
    <!-- WARNING: Retina.js doesn't work if you view the page via file:// -->
    <script src="assets/js/plugins/retina.min.js"></script>

    <!-- Bootstrap Core CSS -->
    <link href="assets/bootstrap/css/bootstrap.css" rel="stylesheet" />
    <link href="assets/css/bootstrap-datepicker3.css" rel="stylesheet" />

    <!-- Animate CSS -->
    <link href="assets/css/animate.min.css" rel="stylesheet">
    <link href="assets/css/main.css" rel="stylesheet" />

    <!-- Fonts -->
    <link href='//fonts.googleapis.com/css?family=National' rel='stylesheet' type='text/css' />

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
    <form id="form1" runat="server" action="expresscheckout.aspx" method="post" target="_blank">
        <input type="hidden" id="MySessionValue" value="" runat="server" />
        <input type="hidden" id="cust_shipID" value="0" />
        <input type="hidden" id="PaymentValid" value="False" />
        <input type="hidden" id="ShipTypeSet" value="" />

        <!-- Top Full Menus -->
        <uc1:TopMenu runat="server" ID="TopMenu" />

        <div class="woocommerce  push-down-30">
            <div class="container">
                <div class="row">
                    <div class="col-xs-12">
                        <h3>Checkout</h3>
                        <hr />

                    </div>
                    <div class="col-xs-12 " id="ProdCheckoutForm" runat="server">
                        <!-- Nav tabs -->
                        <ul class="nav  nav-tabs">
                            <li class="active"><a id="iTab1" href="#Billing" data-toggle="tab">Billing &amp; Shipping Info</a></li>
                            <li><a id="iTab2" href="#Payments" onclick="return SaveTab1()">Payment Info</a></li>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane fade in active" id="Billing">
                                <div class="row">
                                    <div class="col-md-6">
                                        <h3>Billing <span class="light">Information</span></h3>
                                        <div class="col-xs-12  col-md-6 push-down-10">
                                            <p>
                                                <label>
                                                    First Name
                <abbr class="required">
                    *
                </abbr>
                                                </label>
                                                <input class="input-text" id="FirstName" />
                                            </p>
                                        </div>
                                        <div class="col-xs-12  col-md-6 ">
                                            <p>
                                                <label>
                                                    Last Name
                <abbr class="required">
                    *
                </abbr>
                                                </label>
                                                <input class="input-text" id="LastName" />
                                            </p>
                                        </div>
                                        <div class="col-xs-12  col-sm-12  push-down-10">
                                            <p>
                                                <label>
                                                    Company Name
                                                </label>
                                                <input class="input-text" id="CompanyName" />
                                            </p>
                                        </div>
                                        <div class="col-xs-12  col-sm-12  push-down-10">
                                            <p>
                                                <label>
                                                    Address
                <abbr class="required">
                    *
                </abbr>
                                                </label>
                                                <input class="input-text  push-down-10" placeholder="Street address" id="MyStreet" />
                                                <input class="input-text" placeholder="Apartment, suite, unit etc. (optional)" id="MyStreet2" />
                                            </p>
                                        </div>
                                        <div class="col-xs-12  col-sm-12  push-down-10">
                                            <p>
                                                <label>
                                                    City
                <abbr class="required">
                    *
                </abbr>
                                                </label>
                                                <input class="input-text  push-down-10" placeholder="City" id="MyCity" />
                                            </p>
                                        </div>
                                        <div class="col-xs-12  col-sm-12  push-down-10">
                                            <p>
                                                <label>
                                                    State
                <abbr class="required">
                    *
                </abbr>
                                                </label>
                                                <select class="input-text" id="MyState" name="MyState">
                                                    <option value="">Select a State...</option>
                                                    <option value='AK'>Alaska</option>
                                                    <option value='AL'>Alabama</option>
                                                    <option value='AR'>Arkansas</option>
                                                    <option value='AZ'>Arizona</option>
                                                    <option value='CA'>California</option>
                                                    <option value='CO'>Colorado</option>
                                                    <option value='CT'>Connecticut</option>
                                                    <option value='DC'>District of Columbia</option>
                                                    <option value='DE'>Delaware</option>
                                                    <option value='FL'>Florida</option>
                                                    <option value='GA'>Georgia</option>
                                                    <option value='HI'>Hawaii</option>
                                                    <option value='IA'>Iowa</option>
                                                    <option value='ID'>Idaho</option>
                                                    <option value='IL'>Illinois</option>
                                                    <option value='IN'>Indiana</option>
                                                    <option value='KS'>Kansas</option>
                                                    <option value='KY'>Kentucky</option>
                                                    <option value='LA'>Louisiana</option>
                                                    <option value='MA'>Massachusetts</option>
                                                    <option value='MD'>Maryland</option>
                                                    <option value='ME'>Maine</option>
                                                    <option value='MI'>Michigan</option>
                                                    <option value='MN'>Minnesota</option>
                                                    <option value='MO'>Missouri</option>
                                                    <option value='MS'>Mississippi</option>
                                                    <option value='MT'>Montana</option>
                                                    <option value='NC'>North Carolina</option>
                                                    <option value='ND'>North Dakota</option>
                                                    <option value='NE'>Nebraska</option>
                                                    <option value='NH'>New Hampshire</option>
                                                    <option value='NJ'>New Jersey</option>
                                                    <option value='NM'>New Mexico</option>
                                                    <option value='NV'>Nevada</option>
                                                    <option value='NY'>New York</option>
                                                    <option value='OH'>Ohio</option>
                                                    <option value='OK'>Oklahoma</option>
                                                    <option value='OR'>Oregon</option>
                                                    <option value='PA'>Pennsylvania</option>
                                                    <option value='PR'>Puerto Rico</option>
                                                    <option value='RI'>Rhode Island</option>
                                                    <option value='SC'>South Carolina</option>
                                                    <option value='SD'>South Dakota</option>
                                                    <option value='TN'>Tennessee</option>
                                                    <option value='TX'>Texas</option>
                                                    <option value='US'>United States</option>
                                                    <option value='UT'>Utah</option>
                                                    <option value='VA'>Virginia</option>
                                                    <option value='VI'>Virgin Islands</option>
                                                    <option value='VT'>Vermont</option>
                                                    <option value='WA'>Washington</option>
                                                    <option value='WI'>Wisconsin</option>
                                                    <option value='WV'>West Virginia</option>
                                                    <option value='WY'>Wyoming</option>

                                                </select>
                                            </p>
                                        </div>
                                        <div class="col-xs-12  col-sm-12  push-down-10">
                                            <p>
                                                <label>
                                                    Zip
                                                <abbr class="required">
                                                    *
                                                </abbr>
                                                </label>
                                                <input class="input-text  push-down-10" placeholder="Zipcode" id="MyZip" />
                                            </p>
                                        </div>

                                        <div class="col-xs-12  col-sm-6  push-down-10">
                                            <p>
                                                <label>
                                                    Email Address
                                                <abbr class="required">
                                                    *
                                                </abbr>
                                                </label>
                                                <input class="input-text" id="MyEmail" />
                                            </p>
                                        </div>
                                        <div class="col-xs-12  col-sm-6  push-down-10">
                                            <p>
                                                <label>
                                                    Phone
                                                <abbr class="required">
                                                    *
                                                </abbr>
                                                </label>
                                                <input class="input-text" id="MyPhone" />
                                            </p>
                                        </div>

                                    </div>
                                    <div class="col-xs-12 col-sm-6">

                                        <h3><span class="light">Shipping</span> Address
                                         <div class="pull-right">
                                             <a href="javascript:void(0)" class="btn btn-primary--transition" onclick="AddAddress()"><i class="glyphicon glyphicon-plus"></i>Add Address</a>
                                         </div>
                                        </h3>
                                        <hr class="shop__divider" />
                                        <div class="push-down-15">
                                            Select a checkbox below to ship to your billing address, pickup from store, or select a shipping address from your address book below. If you need to add a new shipping address, click the "Add Address" button above.
                                        </div>

                                        <h4 id="ship1">
                                            <input class="input-checkbox" type="checkbox" id="PickupAtStore" value="pickup" onclick="CheckShippingSelect(this)" />
                                            Pickup From Store</h4>

                                        <h4 id="ship2">
                                            <input class="input-checkbox" type="checkbox" id="ShipToBillingSelect" value="shiptobill" onclick="CheckShippingSelect(this)" />
                                            Ship to Billing Address</h4>

                                        <h4 id="ship3">
                                            <input class="input-checkbox" type="checkbox" id="CustomShipping" value="custom" onclick="CheckShippingSelect(this)" />
                                            Custom Shipping Address</h4>
                                        <div id="ShipHistoryList" runat="server"></div>
                                        <p>
                                            <label>Date Requested:</label>
                                            <input type="text" id="date_wanted" value="" class="input-text" readonly="readonly" />
                                        </p>
                                        <p>
                                            <label>Pick-up/Local Delivery Time Requested:</label><br/>
                                            <small>Please pick a time for pick-up or local delivery orders.</small>
                                            <select id="time_wanted" value="" class="input-text">
                                                <option value="">Select for Pickup or Local Delivery...</option>
                                                <option value="Morning">Morning (9am - 12pm)</option>
                                                <option value="Afternoon">Afternoon (12pm - 3pm)</option>
                                                <option value="Evening">Evening (3pm - 6pm)</option>
                                            </select>
                                        </p>
                                        <p>
                                            <label>
                                                Order notes
                                            </label>
                                            <textarea class="input-text" rows="3" id="order_notes"></textarea>
                                            <div><i>Put any special notes here about your order.  For instance, the date you would like to pickup or receive your order.  Or if this is a gift, any note you would like on the card.</i></div>
                                        </p>

                                        <a href="javascript:void(0)" class="btn btn-warning pull-right" onclick="SaveTab1()"><i class="glyphicon glyphicon-credit-card"></i> Save &amp; Continue</a>
                                    </div>
                                </div>
                            </div>
                            <div class="tab-pane  fade" id="Payments">
                                <div class="row">
                                    <div class="col-md-12">
                                        <!-- Your order - table -->
                                        <h3><span class="light">Your</span> Order</h3>

                                        <div id="ProductOrderList" runat="server"></div>


                                        <!-- Payment methods -->
                                        <div class="payment">
                                            <div class="title_text"><h3>Payment Method</h3></div>
                                            <ul class="payment_methods">
                                                <li class="pm_pay">
                                                    <input type="radio" id="Radio1" class="input-radio" name="payment_method" value="cc" onclick="CheckPayType(this)" />
                                                    <label>
                                                        <img src="images/cc_images.png" align="left" style="margin-right: 7px;" />
                                                        <span style="font-size: 11px; font-family: Arial, Verdana;"></span>Credit Card
                                                    </label>
                                                    <div class="payment_box" id="pay_cc">
                                                        <div class="col-xs-12  col-sm-12  push-down-5">
                                                            <p>
                                                                <label>Number</label>
                                                                <input class="input-text" type="text" id="CCNumber" name="CCNumber" placeholder="XXXX-XXXX-XXXX-XXXX (dashes not required)" />
                                                            </p>
                                                        </div>
                                                        <div class="col-xs-12  col-sm-12  push-down-5">
                                                            <p>
                                                                <label>Expiration Date Month:</label>
                                                                <asp:DropDownList runat="server" ID="CCExpireMonth" CssClass="input-text" />
                                                                <br />
                                                                <label>Expiration Date Year:</label>
                                                                <asp:DropDownList runat="server" ID="CCExpireYear" CssClass="input-text" />
                                                            </p>
                                                        </div>
                                                        <div class="col-xs-12  col-sm-12  push-down-5">
                                                            <p>
                                                                <label>CCV Code</label>
                                                                <input class="input-text" type="text" id="CCCode" name="CCCode" placeholder="123" />
                                                            </p>
                                                        </div>
                                                        <div class="row"></div>
                                                    </div>
                                                </li>
                                            <!--
                                                <li class="pm_pay">
                                                    <input type="radio" id="Radio2" class="input-radio" name="payment_method" value="paypal" onclick="CheckPayType(this)" />
                                                    <label>
                                                        <img src="https://www.paypal.com/en_US/i/logo/PayPal_mark_37x23.gif" align="left" style="margin-right: 7px;" /><span style="font-size: 11px; font-family: Arial, Verdana;">The safer, easier way to pay. (Use PayPal to pay with Visa/Mastercard even if you don't have a PayPal account)</span></label>
                                                    <div class="payment_box" id="pay_paypal">
                                                        <form id="form2" action='expresscheckout.aspx' method="post" target="_blank">
                                                            <input type='image' name="submit" src="https://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif" border="0" align="top" alt='Check out with PayPal' />
                                                            <b>Submitting your PayPal payment will complete and submit your order automatically</b>
                                                            <i>(Also pay with Visa/Mastercard using PayPal)</i>
                                                        </form>
                                                    </div>
                                                </li>
                                            -->
                                                <li class="pm_pu">
                                                    <input type="radio" id="Radio3" class="input-radio" name="payment_method" value="pickup" onclick="CheckPayType(this)" />
                                                    <label>Pay In Store At Time of Pickup</label>
                                                </li>
                                            </ul>

                                            <a href="javascript:void(0)" id="btnPlaceOrder" class="btn btn-success pull-right" onclick="CompleteCheckout()" style="display: none"><i class="glyphicon glyphicon-check"></i> Place order</a>&nbsp;&nbsp;
                                            <a href="javascript:void(0)" class="btn btn-default pull-right" onclick="LoadTab1()" style="margin-right: 10px"><i class="glyphicon glyphicon-arrow-left"></i> Go Back</a>&nbsp;
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>


        <!-- Product Edit Form -->
        <div class="modal fade" id="ProductForm" title="Product Edit">
            <input type="hidden" id="temp_cartID" value="0" />
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h3><i class="glyphicon glyphicon-check"></i>Edit Cart Product</h3>
                        <button class="close" aria-hidden="true" type="button" data-dismiss="modal">×</button>
                    </div>
                    <div class="modal-body">
                        <div id="ProductCartEditForm">
                        </div>

                        <!-- Quantity buttons -->
                        <div class="quantity  js--quantity">
                            <input type="button" value="-" class="quantity__button  js--minus-one  js--clickable" />
                            <input type="text" id="quantity" name="quantity" value="1" class="quantity__input" readonly="readonly" />
                            <input type="button" value="+" class="quantity__button  js--plus-one  js--clickable" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" id="Button2" class="btn btn-danger" onclick="CancelUpdate()"><i class="glyphicon glyphicon-remove"></i>Cancel</button>
                        <button type="button" id="Button1" class="btn btn-primary--transition" onclick="UpdateMyCart()"><i class="glyphicon glyphicon-check"></i>Update Cart</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Address Add/Edit Form -->
        <div class="modal fade" id="AccountAddressAddForm" title="Address Form">
            <input type="hidden" id="shippingID" value="0" />
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h3 class="modal-title"><i class="entypo-tools"></i>Customer Address Form</h3>
                    </div>
                    <div class="modal-body woocommerce">
                        <div class="col-xs-12  col-sm-12  push-down-5">
                            <p>
                                <label>Shipping Name</label>
                                <input class="input-text" type="text" id="ship_name" name="ship_name" placeholder="Ship To Name" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-5">
                            <p>
                                <label>Shipping Phone</label>
                                <input class="input-text" type="text" id="ship_phone" name="ship_name" placeholder="Ship To Contact Phone" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-5">
                            <p>
                                <label>Shipping Street</label>
                                <input class="input-text" type="text" id="ship_address1" name="ship_address1" placeholder="" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-5">
                            <p>
                                <label>Shipping City</label>
                                <input class="input-text" type="text" id="ship_city" name="ship_city" placeholder="" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-5">
                            <p>
                                <label>Shipping State</label>
                                <input class="input-text" type="text" id="ship_state" name="ship_state" placeholder="" maxlength="2" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-5">
                            <p>
                                <label>Shipping Zip</label>
                                <input class="input-text" type="text" id="ship_zip" name="ship_zip" placeholder="" />
                            </p>
                        </div>
                        <div id="date_row" style="padding: 4px; font-size: 10px;"></div>
                        <div class="clearfix"></div>
                    </div>
                    <div class="modal-footer">
                        <a href="javascript:void(0)" class="btn btn-success" onclick="SaveShipping()">Save Shipping</a>
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
    <script src="assets/bootstrap/js/bootstrap.js"></script>

    <!-- jQuery Easing -->
    <script src="assets/js/plugins/jquery.easing.1.3.min.js"></script>

    <!-- WOW plugin (used for animated sections) -->
    <script src="assets/js/plugins/wow.min.js"></script>

    <!-- jQuery Bootstrap Validation for Booking Form -->
    <script src="assets/js/plugins/jqBootstrapValidation.js"></script>

    <!-- Add fancyBox -->
    <link rel="stylesheet" href="assets/fancybox/jquery.fancybox.css?v=2.1.5" type="text/css" media="screen" />
    <script type="text/javascript" src="assets/fancybox/jquery.fancybox.pack.js?v=2.1.5"></script>

    <!-- Foodster JavaScript -->
    <script src="assets/js/foodster.js"></script>
    <script src="assets/js/bootstrap-datepicker.js"></script>

    <!-- Your Custom JavaScript -->
    <script src="assets/js/custom.js"></script>
    <script src="assets/app/toastr.min.js"></script>
    <script src="assets/app/Common.min.js"></script>
    <script src="assets/app/hideShowPassword.min.js"></script>
    <script src="assets/app/jquery.storageapi.min.js"></script>
    <script src="assets/app/utils.min.js"></script>
    <script src="assets/app/default.min.js"></script>

    <script src="assets/app/Checkout.js"></script>

</body>
</html>
