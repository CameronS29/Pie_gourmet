// J Scott King
// www.jskdesign.net 2014
// Default Page Client Functions

var storage = $.localStorage;

$(document).ready(function () {
    CheckSavedInfo();

    $('#sDate1').datepicker();
    $('#sDate2').datepicker();
});

var iArray = new Array();

function RemoveArrayItems(array, item) {
    var i = 0;
    while (i < array.length) {
        if (array[i] == item) {
            array.splice(i, 1);
        }
        else {
            i++;
        }
    }
    return array;
}

function SelectItem(ele) {
    var val = ele.value;
    if (ele.checked) {
        iArray.push(val);
    }
    else {
        iArray = RemoveArrayItems(iArray, val);
    }
    $("#gCartArray").val(iArray);
    $('#ProdSelectCnt').html('[' + iArray.length + ']');

    if (iArray.length >= 1) {
        $('#processBtn').removeClass("btn-dark").addClass("btn-success");
    } else {
        $('#processBtn').removeClass("btn-success").addClass("btn-dark");
    }
}

function checkOpen(ele) {
    $("input[name='remchecked']").each(function () {
        this.click();
    });
}

function ProcessChecked(stat) {
    if (iArray.length >= 1) {
        var list = $("#gCartArray").val();

        if (stat == 1) {
            if (confirm("Set selected orders to DELIVERED status?")) {
                Dashboard.BatchProcess(list, myname, BatchProcessCallback);
            }
        }
       
        if (stat == 0) {
            if (confirm("Set selected orders to CANCEL status?")) {
                Dashboard.BatchProcessCancel(list, myname, BatchProcessCallback);
            }
        }
    }
}

function BatchProcessCallback(res) {

    if (res.value == null || res.error != null) {
        ShowAutoAlert('Batch Process Error', GetResError(res.error) + ' - ' + res.value, 'error', true);
    }
    else {
        var ret = res.value;
        var arr = ret.split("|");
        var err = arr[0];
        var msg = arr[1];

        if (err == 0) {
            ShowAutoAlert('Batch Processed', msg, 'success', true);
            RefreshDashboardList();
            $('#processBtn').removeClass("btn-success").addClass("btn-dark");
            iArray = new Array();
        }
        else {
            ShowAutoAlert('Batch Process Error', msg, 'error', true);
        }
    }
}

function RefreshDashboardList() {
    var text = Dashboard.PendingOrdersList().value;
    $('#OrderTable').html(text);
}


function AdminLogin() {
    var u = $('#Username').val();
    var p = $('#MyPassword').val();

    Admin.ProcessLogin(u, p, DoLoginCallback);
}

function AdminLoginCallback(res) {
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

function SaveLogin() {
    var u = $('#Username').val();
    var p = $('#MyPassword').val();

    storage.set("pgamyusername", u);
    storage.set("pgamypassword", p);

    ShowAutoAlert('Login Info', 'Your login information has been saved successfully...', 'success', true);
}

function CheckSavedInfo() {
    if (storage) {
        $('#Username').val(storage.get("pgamyusername"));
        $('#MyPassword').val(storage.get("pgamypassword"));
    }
}

//Content Help Functions
function GetHTMLText() {
    var iEdit = oUtil.obj;
    return iEdit.getXHTMLBody();
}

function SetHTMLText(txt) {
    var iEdit = oUtil.obj;
    iEdit.focus();
    iEdit.insertHTML(txt);
    //iEdit.loadHTML(txt);
}

function SavePage() {
    var c = GetHTMLText();
    var p = GetMyElement('PageLoadType');

    Specials.SaveFileToServer(c, p, SavePageCallback);
}

function SavePageCallback(res) {
    var req = res.value;
    var req_array = req.split("|");
    var code = req_array[0];
    var msg = req_array[1];

    if (res.value == null || res.error != null) {
        ShowAutoAlert('Page Save Error', res.error.Message, 'error', true);
    }
    else {
        if (code == 1) {
            ShowAutoAlert('Page Save Error', msg, 'error', true);
        }
        else if (code == 0) {
            ShowAutoAlert('Page Saved', 'Page saved successfully!', 'success', true);
        }
        else {
            ShowAutoAlert('Page Save Error', msg, 'error', true);
        }
    }
}

function EditCustomer(cID) {
    //jQuery.noConflict();
    $('#CustomerForm').modal('show');
    SetMyElement('CustomerID', cID);
    GlobalAjax.Getcustomers(cID, FillCustomerCallback);
}

function FillCustomerCallback(res) {
    if (res.value == null || res.error != null) return false;
    var rows = res.value.Tables[0].Rows.length;

    if (rows >= 1) {
        var dTable = res.value.Tables[0].Rows[0];
        SetMyElement('title', escapeNull(dTable.title, ''));
        SetMyElement('company', escapeNull(dTable.company, ''));
        SetMyElement('first_name', escapeNull(dTable.first_name, ''));
        SetMyElement('last_name', escapeNull(dTable.last_name, ''));
        SetMyElement('address1', escapeNull(dTable.address1, ''));
        SetMyElement('city', escapeNull(dTable.city, ''));
        SetMyElement('state', escapeNull(dTable.state, ''));
        SetMyElement('zip', escapeNull(dTable.zip, ''));
        SetMyElement('email', escapeNull(dTable.email, ''));
        SetMyElement('password', escapeNull(dTable.password, ''));
        SetMyElement('home_phone', escapeNull(dTable.home_phone, ''));
        SetMyElement('mobile_phone', escapeNull(dTable.mobile_phone, ''));
        SetMyHTML('date_row', 'Created: ' + dTable.create_date + ':' + escapeNull(dTable.create_user, '') + ' | Modified: ' + escapeNull(dTable.mod_date, '') + ':' + escapeNull(dTable.mod_user, ''));
    }
}

function Validate_customers() {
    var pass = true;
    var ChkFields = new Array('first_name', 'last_name', 'email', 'password');
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

function SaveMyAccount() {
    if (Validate_customers()) {
        var cd = GlobalAjax.GetcustomersClass().value;
        cd.customerID = customerid;
        cd.company = GetMyElement('company');
        cd.title = GetMyElement('title');
        cd.first_name = GetMyElement('first_name');
        cd.last_name = GetMyElement('last_name');
        cd.address1 = GetMyElement('address1');
        cd.address2 = GetMyElement('address2');
        cd.city = GetMyElement('city');
        cd.state = GetMyElement('state');
        cd.zip = GetMyElement('zip');
        cd.email = GetMyElement('email');
        cd.home_phone = GetMyElement('home_phone');
        cd.mobile_phone = GetMyElement('mobile_phone');
        cd.username = GetMyElement('username');
        cd.password = GetMyElement('password');
        cd.create_user = myemail;
        //Pass class to server, Save data
        GlobalAjax.ProcessCustomersOnline(cd, SavecustomersCallback);
    }
}

function SaveAdminAccount() {
    if (Validate_customers()) {
        var cd = GlobalAjax.GetcustomersClass().value;
        cd.customerID = GetMyElement('CustomerID');
        cd.company = GetMyElement('company');
        cd.title = GetMyElement('title');
        cd.first_name = GetMyElement('first_name');
        cd.last_name = GetMyElement('last_name');
        cd.address1 = GetMyElement('address1');
        cd.address2 = GetMyElement('address2');
        cd.city = GetMyElement('city');
        cd.state = GetMyElement('state');
        cd.zip = GetMyElement('zip');
        cd.email = GetMyElement('email');
        cd.home_phone = GetMyElement('home_phone');
        cd.mobile_phone = GetMyElement('mobile_phone');
        cd.username = GetMyElement('username');
        cd.password = GetMyElement('password');
        cd.create_user = myemail;
        //Pass class to server, Save data
        GlobalAjax.ProcessCustomersOnline(cd, SavecustomersCallback);
    }
}

function SavecustomersCallback(res) {

    if (res.value == null || res.error != null) {
        ShowAutoAlert('Save Error', GetResError(res.error) + ' - ' + res.value, 'error', true);
    }
    else {
        var ret = res.value;
        var arr = ret.split("|");
        var err = arr[0];
        var msg = arr[1];

        if (err == 0) {
            ShowAutoAlert('Account Saved', msg, 'success', true);
        }
        else {
            ShowAutoAlert('Account Save Error', msg, 'error', true);
        }
    }
}

function PrintCart() {
    $.print("#CartDetailForm");
}

function PrintCartItem(c) {
    //jQuery.noConflict();
    $('#CartDetailForm').modal('show');
    var text = Dashboard.BuildCartItem(c).value;
    $('#CartPrintForm').html(text);
}

function PrintCartItemSearch(c) {
    //jQuery.noConflict();
    $('#CartDetailForm').modal('show');
    var text = SearchOrders.BuildCartItem(c).value;
    $('#CartPrintForm').html(text);
}

function GoToSearchOrders() {
    location.href = 'SearchOrders.aspx';
}

function DoSearchOrders() {
    var btn = document.getElementById("processBtn3");
    btn.disabled = true;

    var cd = SearchOrders.GetSearchClass().value;

    cd.LastName = $("#sCustomerLastName").val();
    cd.SearchCity = $("#sCity").val();
    cd.SearchState = $("#sState").val();
    cd.SearchStatus = $("#sStatus").val();
    cd.SearchDate1 = $("#sDate1").val();
    cd.SearchDate2 = $("#sDate2").val();

    SearchOrders.SearchOrdersList(cd, SearchListCallback);
}

function SearchListCallback(res) {
    //alert(res.value);
    var btn1 = document.getElementById("processBtn3");

    if (res.value == null || res.error != null) {
        $('#OrderSearchTable').html("<div class='error'>" + res.error + "</div>");
    }
    else {
        //parse results
        var presults = res.value;
        var splitOut = presults.split("|");
        var err = splitOut[0];
        var html = splitOut[1];

        if (err == 0) {
            $('#OrderSearchTable').html(html);
        } else {
            $('#OrderSearchTable').html("<div class='error'>" + html + "</div>");
        }
       
    }

    btn1.disabled = false;
}
