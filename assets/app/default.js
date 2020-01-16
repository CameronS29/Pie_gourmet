// J Scott King
// www.jskdesign.net 2014
// Default Page Client Functions

$(document).ready(function () {
   
    ShowMyCart();
    $(".fancybox").fancybox();
});

// Auto Alert Show/Hide ////////////////////////
function ShowAutoAlert(sub, msg, flag, doclose, nofade){
    //window.scrollTo(0,0);
    var opts = {
        "closeButton": false,
        "debug": false,
        "positionClass": "toast-top-right",
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    if (doclose) {
        opts.closeButton = true;
    }

    //alert(flag +','+ nofade);
    if (nofade) {
        opts.timeOut = 0;
    }

    switch (flag) {
        case 'error':
            toastr.error(msg, sub, opts);
            break;
        case 'success':
            toastr.success(msg, sub, opts);
            break;
        case 'warning':
            toastr.warning(msg, sub, opts);
            break;
        default:
            toastr.info(msg, sub, opts);
            break;
    }
}

function TestAlert() {
    ShowAutoAlert('Field Rep Maintenance', 'This is a special test message...', 'success', true);
}

function CheckLocalDeliveryForm() {
    //jQuery.noConflict();
    $('#DeliveryCheck').modal('show');
}

function CheckLocalZipDelivery() {
    var zip = $('#ZipCheck').val();
    var html = GlobalAjax.LocalDeliveryCheck(zip).value;
    var ret = html.split("|");

    var code = ret[0];
    var msg = ret[1];
    if (code >= 1) {
        $('#ZipCheckText').html("<span class='alert alert-danger'>" + msg + "</span>");
    }
    else
    {
        $('#ZipCheckText').html("<span class='alert alert-success'>" + msg + "</span>");
    }
   

}

function ShowMyCart() {
    //alert('Check!');
    var html = GlobalAjax.ShowCart(sessionid).value;
    var ret = html.split("|");

    var table = ret[0];
    var cnt = ret[1];
    var price = ret[2];

    $('#CartList').html(table);
    $('#CartCount').html(cnt);
    $('#CartPrice').html(price);
}

function AddToCartDetail() {
    var cd = GlobalAjax.GetProductAddClass().value;
    cd.CustomerID = customerid;
    cd.SessionID = sessionid;
    cd.ProductID = $('#MyProductID').val();
    cd.Quantity = $('#quantity').val();
    cd.SubProduct = $('#ProductPricingID').val();

    GlobalAjax.AddProduct(cd, AddProductCallback);
}

function AddToCart(p, pg) {
    var cd = GlobalAjax.GetProductAddClass().value;
    cd.CustomerID = customerid;
    cd.SessionID = sessionid;
    cd.ProductID = p;
    cd.Quantity = 1;
    cd.SubProduct = pg;

    GlobalAjax.AddProduct(cd, AddProductCallback);
}

function AddProductCallback(res) {
    window.scrollTo(0, 0);

    //alert(res.value);
    var req = res.value;
    var req_array = req.split("|");
    var err = req_array[0];
    var id = req_array[1];
    var msg = "";

    if (res.value == null || res.error != null) {
        ShowAutoAlert('Product Error', res.error.Message, 'error', true);
    }
    else {
        if (err == 1) {
            ShowAutoAlert('Product Error', id, 'error', true);
        }
        else {
            msg = "Product Added Successfully...";
            ShowAutoAlert('Product Addded', msg, 'success', true);
            ShowMyCart();
        }
    }
}

function RemoveCartItem(cID) {
    GlobalAjax.RemoveProduct(cID, RemoveProductCallback);
}

function ChangeProductPrice(ele) {
    var pg = ele.value;
    var p = $('#MyProductID').val();

    GlobalAjax.GetProductPricing(p, pg, PriceChangeCallback);
}

function PriceChangeCallback(res) {
    if (res.value == null || res.error != null) return false;
    var rows = res.value.Tables[0].Rows.length;
    if (rows >= 1) {
        var dTable = res.value.Tables[0].Rows[0];
        var pName = dTable.ProductName + ' <span class="light">(' + dTable.ProductSize + '" ' + dTable.TypeCategory + ')';
        var pPrice = CurrencyFormatted(dTable.ProductPrice, false)

        $('#ProductName').html(pName);
        $('#ProductPrice').html(pPrice);
    }
}

function RemoveProductCallback(res) {
  
    //alert(res.value);
    var req = res.value;
    var req_array = req.split("|");
    var err = req_array[0];
    var id = req_array[1];
    var msg = "";

    if (res.value == null || res.error != null) {
        ShowAutoAlert('Product Error', res.error.Message, 'error', true);
    }
    else {
        if (err == 1) {
            ShowAutoAlert('Product Error', id, 'error', true);
        }
        else {
            msg = "Product Removed Successfully...";
            ShowAutoAlert('Product Removed', msg, 'success', true);
            ShowMyCart();
        }
    }
}

function Validate_Login() {
    var pass = true;
    var ChkFields = new Array('Username', 'MyPassword');
    for (var i = 0; i < ChkFields.length; i++) {
        //Find Element
        var ele = document.getElementById(ChkFields[i]);
        val = ele.value;
        if (val == '') {
            ele.style.backgroundColor = '#FFFF00';
            pass = false;
        }
        else {
            ele.style.backgroundColor = '';
        }
    }
    if (!pass) {
        ShowAutoAlert('Login Error', 'Please complete hi-lighted fields before continuing...', 'warning', true);
    }
    return pass;
}

function DoLogin() {
    var u = $('#Username').val();
    var p = $('#MyPassword').val();

    if (Validate_Login()) {
        MyAccount.ProcessLogin(u, p, DoLoginCallback);
    }
}

function DoLoginCallback(res) {
    var req = res.value;
    var req_array = req.split("|");
    var code = req_array[0];
    var msg = req_array[1];
  
    if (res.value == null || res.error != null) {
        ShowAutoAlert('Login Error', res.error.Message, 'error', true);
    }
    else {
        if (code == 1) {
            ShowAutoAlert('Login Error', msg, 'error', true);
        }
        else if (code == 0) {
            location.href = msg;
        }
        else {
            ShowAutoAlert('Login Error', msg, 'error', true);
        }
    }
}

function Validate_Comment() {
    var pass = true;
    var ChkFields = new Array('CommentCity', 'CommentState', 'ProductComment');
    for (var i = 0; i < ChkFields.length; i++) {
        //Find Element
        var ele = document.getElementById(ChkFields[i]);
        val = ele.value;
        if (val == '') {
            ele.style.backgroundColor = '#FFFF00';
            pass = false;
        }
        else {
            ele.style.backgroundColor = '';
        }
    }
    if (!pass) {
        ShowAutoAlert('Form Validation', 'Please complete hi-lighted fields before saving...', 'warning', true);
    }
    return pass;
}

function AddComment() {
    //jQuery.noConflict();
    $('#AddCommentForm').modal('show');
}

function SaveComment() {
    if (Validate_Comment()) {
        var cd = GlobalAjax.Getsite_commentsClass().value;

        cd.productID = $('#MyProductID').val();
        cd.Name = $('#CommentName').val();
        cd.City = $('#CommentCity').val();
        cd.State = $('#CommentState').val();
        cd.comment = $('#ProductComment').val();
        cd.rating = $('input[name="rating"]:checked').val();
        cd.show = 1;
        GlobalAjax.ProcessComment(cd, AddCommentCallback);
    }
}

function AddCommentCallback(res) {
    var req = res.value;
    var req_array = req.split("|");
    var code = req_array[0];
    var msg = req_array[1];

    if (res.value == null || res.error != null) {
        ShowAutoAlert('Comment Error', res.error.Message, 'error', true);
    }
    else {
        if (code == 1) {
            ShowAutoAlert('Comment Error', msg, 'error', true);
        }
        else if (code == 0) {
            ShowAutoAlert('Comment Saved', 'Thank you for your comment!', 'success', true);
            var txt = GlobalAjax.ShowComments($('#MyProductID').val(), 0).value;
            $('#ReviewList').html(txt);
            $('#AddCommentForm').modal('hide');
        }
        else {
            ShowAutoAlert('Comment Error', msg, 'error', true);
        }
    }
}

function RemoveComment(cID) {
    if (confirm("Remove this comment?"))
        GlobalAjax.DeleteComment(cID, RemoveCommentCallback);
}

function RemoveCommentCallback(res) {
    var txt = GlobalAjax.ShowCommentsAdmin($('#MyProductID').val()).value;
    $('#ReviewText').html(txt);
}