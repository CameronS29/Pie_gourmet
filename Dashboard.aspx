<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Dashboard.aspx.vb" Inherits="Dashboard" %>

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

    <title>Pie Gourmet :: Admin Dashboard</title>


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
        <input type="hidden" id="gCartArray" value="" />

        <!-- Top Full Menus -->
        <uc1:TopMenu runat="server" ID="TopMenu" />

        <div class="container" style="margin-top: 30px">
            <!-- Big banner -->
            <div class="row">
                <button type="button" class="btn btn-dark" onclick="ProcessChecked(1)" id="processBtn">Process Checked</button>&nbsp;
                <button type="button" class="btn btn-danger" onclick="ProcessChecked(0)" id="processBtn2">Cancel Checked</button>&nbsp;
                <button type="button" class="btn btn-info" onclick="GoToSearchOrders()" id="processBtn3">Search Orders</button>
            </div>
            <div class="row">
                <div class="push-down-10">
                    <div class="col-md-8">
                        <div id="OrderTable" runat="server"></div>
                    </div>
                    <div class="col-md-4">
                        <div id="OrderHistory" runat="server"></div>
                    </div>
                </div>
            </div>
        </div>
    </form>

    <uc1:BottomNav runat="server" ID="BottomNav" />
    
    
    <!-- Customer Edit Form -->
        <div class="modal fade" id="CustomerForm" title="Customer Edit">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h3><i class="entypo-tools"></i>Customer Maintenance </h3>
                        <button class="close" aria-hidden="true" type="button" data-dismiss="modal">×</button>
                    </div>
                    <div class="modal-body">
                        <table class="table">
                            <tr>
                                <td>
                                    Customer ID:
                                </td>
                                <td><input class="form-control" type="text" id="CustomerID" name="CustomerID" readonly="readonly" size="10" /></td>
                            </tr>
                        
                            <tr>
                                <td>
                                    Title:
                                </td>
                                <td><input class="form-control" type="text" id="title" name="title" placeholder="Mr., Mrs., Dr., etc..."/></td>
                            </tr>
                       
                            <tr>
                                <td>
                                    Company Name:
                                </td>
                                <td><input class="form-control" type="text" id="company" name="company" size="30" /></td>
                            </tr>
                        
                            <tr>
                                <td>
                                    First Name:
                           
                                <abbr class="required">* </abbr>
                                </td>
                                <td><input class="form-control" type="text" id="first_name" name="first_name" size="30" /></td>
                            </tr>
                       
                            <tr>
                                <td>
                                    Last Name:
                           
                                <abbr class="required">* </abbr>
                                </td>
                                <td><input class="form-control" type="text" id="last_name" name="last_name" size="30" /></td>
                            </tr>
                      
                            <tr>
                                <td>
                                    Email:
                           
                                <abbr class="required">
                                    *
                           
                                </abbr>
                                </td>
                                <td><input class="form-control" type="email" id="email" name="email" size="40" /></td>
                            </tr>
                       
                            <tr>
                                <td>
                                    Password:
                           
                                <abbr class="required">
                                    *
                                </abbr>
                                </td>
                                <td><input class="form-control" type="text" id="password" name="password" /></td>
                            </tr>
                       
                            <tr>
                                <td>
                                    Address:
                                </td>
                                <td><input class="form-control" type="text" id="address1" name="address1" placeholder="Street address" /></td>
                            </tr>
                            <tr>
                                <td>City</td>
                                <td>
                                <input class="form-control" type="text" id="city" name="city" placeholder="City" />
                                </td>
                            </tr>
                            <tr>
                                <td>State</td>
                                <td>
                                <select class="form-control" id="state" name="state">
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

                                </select></td>
                            </tr>
                            <tr>
                                <td>Zip</td>
                                <td>
                                <input class="form-control" type="text" id="zip" name="zip" placeholder="Zip" />
                                </td>
                            </tr>
                       
                            <tr>
                                <td>
                                    Phone 1:
                           
                                </td>
                                <td><input class="form-control" type="text" id="home_phone" name="home_phone" placeholder="Home/Work Phone" /></td>
                            </tr>
                      
                            <tr>
                                <td>
                                    Mobile Phone:
                           
                                </td>
                               <td> <input class="form-control" type="text" id="mobile_phone" name="mobile_phone" placeholder="Mobile Phone" /></td>
                            </tr>
                        
                    </table>
                    </div>
                    <div class="modal-footer">
                        
                        <button type="button" id="Button1" class="btn btn-primary--transition" onclick="SaveAdminAccount()"><i class="glyphicon glyphicon-check"></i> Save Customer </button>
                    </div>
                </div>
            </div>
        </div>
    
     <!-- Cart Details -->
    <div class="modal fade" id="CartDetailForm" role="dialog" aria-hidden="true" title="Cart Form">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h3 class="modal-title"><i class="entypo-tools"></i>Order Details Form</h3>
                </div>
                <div class="modal-body woocommerce">
                    <div class="col-xs-12  col-sm-12  push-down-5" id="CartPrintForm">
                            
                    </div>
                        
                    <div class="clearfix"></div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0)" class="btn btn-success" onclick="PrintCart()">Print Cart</a>
                </div>
            </div>
        </div>
    </div>

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
    <script src="assets/js/jQuery.print.js"></script>

    <script src="assets/app/default.js"></script>
    <script src="assets/app/Admin.js"></script>

</body>
</html>
