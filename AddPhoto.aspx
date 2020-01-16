<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AddPhoto.aspx.vb" Inherits="Account_AddPhoto" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="">
    <meta name="author" content="">

   <link rel="icon" type="image/ico" href="images/favicon.ico" />

    <title>Pie Gourmet :: Product Photos</title>


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
    
      <!-- jQuery Version 1.11.0 -->
    <script src="assets/js/plugins/jquery-1.11.0.js"></script>

    <!-- Bootstrap Core JavaScript -->
    <script src="assets/bootstrap/js/bootstrap.min.js"></script>
    
    <script src="assets/app/toastr.js"></script>
    <script src="assets/app/Common.js"></script>

    <script type="text/javascript">
        function CloseRefresh() {
            window.opener.location.href = "ProductDetail.aspx";
            window.close();
        }

        function ClearPhoto() {
            var p = $('#ProductID').val();
            if (confirm("Are you sure you want to clear this product photo?")) {
                GlobalAjax.ClearProductPhoto(p, ClearPhotoCallback);
            }
        }

        function ClearPhotoCallback(res) {
            if (res.value == null || res.error != null) {
                ShowAutoAlert('Batch Process Error', GetResError(res.error) + ' - ' + res.value, 'error', true);
            }
            else {
                var ret = res.value;
                var arr = ret.split("|");
                var err = arr[0];
                var msg = arr[1];

                if (err == 0) {
                    $('#err_text').html('Photo cleared successfully!');

                }
                else {
                    $('#err_text').html('Error clearing photo: ' + msg);
                }
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <input type="hidden" id="ProductID" runat="server" value="0" />

    <div id="content">
        <div><h3 class="panel-title">Photo Upload :: <asp:Label ID="ClientName" runat="server"></asp:Label></h3></div>
        <div class="ui-content">
            <table class="table table-bordered">
                <tr>
                    <td><b>Get Photo:</b></td>
                    <td><asp:FileUpload ID="GetDoc" runat="server" Width="321px" /></td>
                </tr>
               
                <tr>
                    <td colspan="2">
                        <asp:Button ID="btn_save" runat="server" CssClass="btn btn-primary--transition" Text="Save Photo" />
                        <input type="button" id="btn_clear" value="Clear Photo" class="btn btn-warning--transition" onclick="ClearPhoto()" />
                        <input type="button" id="btn_close" value="Close" class="btn btn-danger--transition" onclick="CloseRefresh()" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="err_text" runat="server" CssClass="error"></asp:Label><hr />
                    </td>
                </tr>
            
            </table>
        
        </div>
    </div>
    </form>
</body>
</html>
