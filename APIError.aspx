<%@ Page Language="VB" %>

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

    <title>Pie Gourmet :: PayPal Error</title>

    <!-- Retina.js -->
    <!-- WARNING: Retina.js doesn't work if you view the page via file:// -->
    <script src="assets/js/plugins/retina.min.js"></script>

    <!-- Bootstrap Core CSS -->
    <link href="assets/bootstrap/css/bootstrap.min.css" rel="stylesheet">

    <!-- Animate CSS -->
    <link href="assets/css/animate.min.css" rel="stylesheet">
    <link href="assets/css/main.css" rel="stylesheet" />

    <!-- Fonts -->
    <link href='http://fonts.googleapis.com/css?family=Economica%7COld+Standard+TT:400,400italic,700' rel='stylesheet' type='text/css'>
    <link href='http://fonts.googleapis.com/css?family=Voltaire' rel='stylesheet' type='text/css'>
   	<link href='http://fonts.googleapis.com/css?family=Lato:300,400,700.900,300italic,400italic' rel='stylesheet' type='text/css'>
    <link href="http://fonts.googleapis.com/css?family=Oleo+Script" rel="stylesheet" type="text/css">

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

		<table>
			<tr>
				<td class="field"></td>
				<td><%=Request.QueryString.Get("ErrorCode")%></td>
			</tr>
			
			<tr>
				<td class="field"></td>
				<td><%=Request.QueryString.Get("Desc")%></td>
			</tr>
			
			<tr>
				<td class="field"></td>
				<td><%=Request.QueryString.Get("Desc2")%></td>
			</tr>

		</table>
    </form>
</body>
</html>
