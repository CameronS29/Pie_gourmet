<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TopMenu.ascx.vb" Inherits="TopMenu" %>
<input type="hidden" id="MyCustomerID" value="" runat="server" />
<input type="hidden" id="MySessionID" value="" runat="server" />

<!-- Navigation -->
<nav id="nav" class="navbar navbar-default">
    <div class="container">
       
        <!-- Brand and toggle are grouped for better mobile display -->
        <div class="navbar-header page-scroll">
            <button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1">
                <span class="sr-only">Toggle navigation</span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
            </button>
        </div>
        <!-- /.navbar-header -->

        <!-- Collect the nav links, forms, and other content for toggling -->
        <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
            <ul class="nav navbar-nav navbar-left">
                <li id="Li1"><a href="Default.aspx">HOME</a></li>
                <li id="lishop"><a href="menu.aspx">SHOP</a></li>
                <asp:Literal runat="server" id="SpecialMenu"></asp:Literal>
            </ul>
            <a class="navbar-brand" href="Default.aspx">
                <img src="assets/images/PG/Logo/Pie Gourmet_White_Small.png" alt="">
            </a>
            
            <ul class="nav navbar-nav navbar-right">
                <li id="MyAccountLink" runat="server"><a href="MyAccount.aspx">My Account</a></li>
                <li id="MyAccountLogout" runat="server"></li>
                <asp:Literal runat="server" ID="TopMenuList"></asp:Literal>
            </ul>
        </div>
        <!-- /.navbar-collapse -->
        <asp:Literal runat="server" ID="Announcements"></asp:Literal>
    </div>
    <!-- /.container-fluid -->
</nav>
<!-- End Navigation -->
