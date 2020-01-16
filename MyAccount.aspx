<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MyAccount.aspx.vb" Inherits="MyAccount" %>

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
    <link href="assets/bootstrap/css/bootstrap.min.css" rel="stylesheet">

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
    <form id="form1" runat="server">
        <!-- Top Full Menus -->
        <uc1:TopMenu runat="server" ID="TopMenu" />

       
        <div class="woocommerce" style="margin-top: 20px">
            <div class="container" id="NewForm" runat="server">
                <!-- Big banner -->
                
                <div class="row">
                    <div class="col-xs-12  col-sm-6">

                        <h3 class="sidebar__title"><span class="light">Pie Gourmet</span> Login</h3>
                        <hr class="shop__divider">

                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Email
                           
                                <abbr class="required">
                                    *
                           
                                </abbr>
                                </label>
                                <input class="input-text" type="email" id="Username" name="Username" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Password
                           
                                <abbr class="required">
                                    *
                                </abbr>
                                </label>
                                <input class="input-text" type="password" id="MyPassword" name="MyPassword" />
                            </p>
                        </div>
                        <div class="pull-right">
                            <a href="javascript:void(0)" class="btn  btn-default" onclick="SendLogin()">I Forget</a>
                            <a href="javascript:void(0)" class="btn btn-warning" onclick="SaveLogin()">Save</a>
                            <a href="javascript:void(0)" class="btn  btn-success" onclick="DoLogin()">Login</a>

                        </div>

                    </div>
                    <div class="col-xs-12  col-sm-6">

                        <h3 class="sidebar__title">Add <span class="light">New Account</span> </h3>
                        <hr class="shop__divider">
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Company Name
                                </label>
                                <input class="input-text" type="text" id="CompanyName" name="CompanyName" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Title
                                </label>
                                <input class="input-text" type="text" id="MyTitle" name="MyTitle" placeholder="Mr., Mrs., Dr., etc..."/>
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    First Name
                           
                                <abbr class="required">* </abbr>
                                </label>
                                <input class="input-text" type="text" id="FirstName" name="FirstName" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Last Name
                           
                                <abbr class="required">* </abbr>
                                </label>
                                <input class="input-text" type="text" id="LastName" name="LastName" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Email
                           
                                <abbr class="required">
                                    *
                           
                                </abbr>
                                </label>
                                <input class="input-text" type="email" id="MyEmail" name="MyEmail" />
                            </p>
                        </div>
                         <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Password
                           
                                <abbr class="required">
                                    *
                           
                                </abbr>
                                </label>
                                <input class="input-text" type="password" id="AddPassword" name="AddPassword" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Address
                           
                                </label>
                                <input class="input-text" type="text" id="MyStreet" name="MyStreet" placeholder="Street address" />
                                <input class="input-text" type="text" id="MyCity" name="MyCity" placeholder="City" />
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
                                <input class="input-text" type="text" id="MyZip" name="MyZip" placeholder="Zip" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Phone 1
                           
                                </label>
                                <input class="input-text" type="text" id="HomePhone" name="HomePhone" placeholder="Home/Work Phone" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Mobile Phone
                           
                                </label>
                                <input class="input-text" type="text" id="MobilePhone" name="MobilePhone" placeholder="Mobile Phone" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Human Security Key
                                </label>
                                <input class="input-text" type="text" id="SaveKey" name="SaveKey" placeholder="Enter key below" />
                                <asp:Literal runat="server" ID="MyPageKey"></asp:Literal>
                            </p>
                        </div>
                        <div class="pull-right">
                            <a href="javascript:void(0)" class="btn btn-success" onclick="CreateNewAccount()">Create Account</a>

                        </div>
                    </div>
                </div>
            </div>
            <div class="container" id="AccountUpdate" runat="server">
                <div class="col-xs-12  col-sm-6" style="margin-bottom: 20px">

                   <h3 class="sidebar__title">Edit <span class="light">My Account</span> </h3>
                    <hr class="shop__divider" />
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Title
                                </label>
                                <input class="input-text" type="text" id="title" name="title" placeholder="Mr., Mrs., Dr., etc..."/>
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Company Name
                                </label>
                                <input class="input-text" type="text" id="company" name="company" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    First Name
                           
                                <abbr class="required">* </abbr>
                                </label>
                                <input class="input-text" type="text" id="first_name" name="first_name" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Last Name
                           
                                <abbr class="required">* </abbr>
                                </label>
                                <input class="input-text" type="text" id="last_name" name="last_name" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Email
                           
                                <abbr class="required">
                                    *
                           
                                </abbr>
                                </label>
                                <input class="input-text" type="email" id="email" name="email" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Password
                           
                                <abbr class="required">
                                    *
                                </abbr>
                                </label>
                                &nbsp;&nbsp;<input type="checkbox" id="show-password" /> <label for="show-password">Show password</label>
                                <input class="input-text" type="password" id="password" name="password" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Address
                           
                                </label>
                                <input class="input-text" type="text" id="address1" name="address1" placeholder="Street address" />
                                <input class="input-text" type="text" id="city" name="city" placeholder="City" />
                                <select class="input-text" id="state" name="state">
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
                                <input class="input-text" type="text" id="zip" name="zip" placeholder="Zip" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Phone 1
                           
                                </label>
                                <input class="input-text" type="text" id="home_phone" name="home_phone" placeholder="Home/Work Phone" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-10">
                            <p>
                                <label>
                                    Mobile Phone
                           
                                </label>
                                <input class="input-text" type="text" id="mobile_phone" name="mobile_phone" placeholder="Mobile Phone" />
                            </p>
                        </div>
                        
                        <div class="pull-right">
                            <a href="javascript:void(0)" class="btn btn-success" onclick="SaveMyAccount()">Save Account</a>
                            <div class="push-down-35"></div>
                        </div>
                        
                        
                    </div>
                    
                <div class="col-xs-12  col-sm-6">

                   <h3 class="sidebar__title">My Address <span class="light">Book</span> 
                        <div class="pull-right">
                            <a href="javascript:void(0)" class="btn btn-primary--transition" onclick="AddAddress()">Add Address</a>
                        </div>
                   </h3>
                    <hr class="shop__divider" />

                    <div id="ShipHistoryList" runat="server"></div>
                </div>
                
                 <div class="col-xs-12  col-sm-6">
                     <h3 class="sidebar__title">My Order <span class="light">History</span> </h3>
                     <hr class="shop__divider" />
                     <div id="OrderHistoryList" runat="server"></div>
                 </div>
            </div>
        </div>
        
        <!-- Address Add/Edit Form -->
        <div class="modal fade" id="AccountAddressAddForm" role="dialog" aria-hidden="true" title="Address Form">
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
                                <label> Name</label>
                                <input class="input-text" type="text" id="ship_name" name="ship_name" placeholder="Ship To Name" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-5">
                            <p>
                                <label>Address</label>
                                <input class="input-text" type="text" id="ship_address1" name="ship_address1" placeholder="" />
                            </p>
                        </div>
                         <div class="col-xs-12  col-sm-12  push-down-5">
                            <p>
                                <label> City</label>
                                <input class="input-text" type="text" id="ship_city" name="ship_city" placeholder="" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-5">
                            <p>
                                <label> State</label>
                                <input class="input-text" type="text" id="ship_state" name="ship_state" placeholder="" maxlength="2" />
                            </p>
                        </div>
                        <div class="col-xs-12  col-sm-12  push-down-5">
                            <p>
                                <label> Zip</label>
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
    
    <!-- Add fancyBox -->
    <link rel="stylesheet" href="assets/fancybox/jquery.fancybox.css?v=2.1.5" type="text/css" media="screen" />
    <script type="text/javascript" src="assets/fancybox/jquery.fancybox.pack.js?v=2.1.5"></script>

    <!-- Placeholders.js provides IE 6-9 support of HTML5 placeholder -->
    <!--[if lte IE 9]>
        <script src="assets/js/plugins/placeholders.min.js"></script>
    <![endif]-->
    <script src="assets/app/toastr.js"></script>
    <script src="assets/app/Common.js"></script>
    <script src="assets/app/hideShowPassword.min.js"></script>
    <script src="assets/app/jquery.storageapi.min.js"></script>
    <script src="assets/app/default.js"></script>
    <script src="assets/app/MyAccount.js"></script>
    
    <script type="text/javascript">
        $('#show-password').change(function () {
            $('#password').hideShowPassword($(this).prop('checked'));
        });
    </script>
</body>
</html>
