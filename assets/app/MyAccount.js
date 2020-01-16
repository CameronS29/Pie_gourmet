// J Scott King
// www.jskdesign.net 2014
// Default Page Client Functions

var storage = $.localStorage;

$(document).ready(function () {
    CheckSavedInfo();
    ShowCustomerData();

});

function ShowCustomerData() {
    GlobalAjax.Getcustomers(customerid, FillCustomerCallback);
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

function Validate_NewAccount() {
    var pass = true;
    var ChkFields = new Array('FirstName', 'LastName', 'MyEmail', 'AddPassword', 'SaveKey');
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

function CreateNewAccount() {
    if (Validate_NewAccount()) {
        var cd = GlobalAjax.GetcustomersClass().value;
        cd.customerID = 0;
        cd.company = GetMyElement('CompanyName');
        cd.title = GetMyElement('MyTitle');
        cd.first_name = GetMyElement('FirstName');
        cd.last_name = GetMyElement('LastName');
        cd.address1 = GetMyElement('MyStreet');
        cd.city = GetMyElement('MyCity');
        cd.state = GetMyElement('MyState');
        cd.zip = GetMyElement('MyZip');
        cd.email = GetMyElement('MyEmail');
        cd.home_phone = GetMyElement('HomePhone');
        cd.mobile_phone = GetMyElement('MobilePhone');
        cd.password = GetMyElement('AddPassword');
        cd.TestKey = GetMyElement('SaveKey');
        cd.create_user = myemail;
        //Pass class to server, Save data
        GlobalAjax.ProcessCustomersOnline(cd, CreateCustomersCallback);
    }
}

function CreateCustomersCallback(res) {
    if (res.value == null || res.error != null) {
        ShowAutoAlert('Save Error', GetResError(res.error) + ' - ' + res.value, 'error', true);
    }
    else {
        var ret = res.value;
        var arr = ret.split("|");
        var err = arr[0];
        var msg = arr[1];

        if (err == 0) {
            msg = "Your account was added successfully... You can now login with your saved details"
            ShowAutoAlert('Account Added', msg, 'success', true);

            CopyLogin();
        }
        else {
            ShowAutoAlert('Account Save Error', msg, 'error', true);
        }
    }
}

function CopyLogin() {
    $('#Username').val($('#MyEmail').val());
    $('#MyPassword').val($('#AddPassword').val());
    DoLogin();
}

function SendLogin() {
    var e = $('#Username').val();
    MyAccount.ForgotLogin(e, SendLoginCallback);
}

function SendLoginCallback(res) {
    if (res.value == null || res.error != null) {
        ShowAutoAlert('Send Details Error', GetResError(res.error) + ' - ' + res.value, 'error', true);
    }
    else {
        var ret = res.value;
        var arr = ret.split("|");
        var err = arr[0];
        var msg = arr[1];

        if (err == 0) {
            ShowAutoAlert('Account Login Info', msg, 'success', true);
           
        }
        else {
            ShowAutoAlert('Account Login Error', msg, 'error', true);
        }
    }
}

function SaveLogin() {
    var u = $('#Username').val();
    var p = $('#MyPassword').val();

    storage.set("pgmyusername", u);
    storage.set("pgmypassword", p);

    ShowAutoAlert('Login Info', 'Your login information has been saved successfully...', 'success', true);
}

function CheckSavedInfo() {
    if (storage) {
        $('#Username').val(storage.get("pgmyusername"));
        $('#MyPassword').val(storage.get("pgmypassword"));
    }
}

function New_customers_shipping() {
    var cd = GlobalAjax.Getcustomers_shippingClass().value;

    SetMyElement('shippingID', cd.cust_shipID);
    SetMyElement('ship_name', escapeNull(cd.ship_name, ''));
    SetMyElement('ship_address1', escapeNull(cd.ship_address1, ''));
    SetMyElement('ship_address2', escapeNull(cd.ship_address2, ''));
    SetMyElement('ship_city', escapeNull(cd.ship_city, ''));
    SetMyElement('ship_state', escapeNull(cd.ship_state, ''));
    SetMyElement('ship_zip', escapeNull(cd.ship_zip, ''));
    SetMyHTML('date_row', '');
}


function EditShipping(es) {
    //jQuery.noConflict();
    $('#AccountAddressAddForm').modal('show');
    GlobalAjax.Getcustomers_shipping(es, FillShippingCallback);
}

function FillShippingCallback(res) {
    if (res.value == null || res.error != null) return false;
    var rows = res.value.Tables[0].Rows.length;

    if (rows >= 1) {
        var dTable = res.value.Tables[0].Rows[0];
        SetMyElement('shippingID', escapeNull(dTable.cust_shipID, 0));
        SetMyElement('ship_name', escapeNull(dTable.ship_name, ''));
        SetMyElement('ship_address1', escapeNull(dTable.ship_address1, ''));
        SetMyElement('ship_address2', escapeNull(dTable.ship_address2, ''));
        SetMyElement('ship_city', escapeNull(dTable.ship_city, ''));
        SetMyElement('ship_state', escapeNull(dTable.ship_state, ''));
        SetMyElement('ship_zip', escapeNull(dTable.ship_zip, ''));
        SetMyElement('ship_phone', escapeNull(dTable.ship_phone, ''));

        SetMyHTML('date_row', 'Created: ' + dTable.create_date + ':' + escapeNull(dTable.create_user, '') + ' | Modified: ' + escapeNull(dTable.mod_date, '') + ':' + escapeNull(dTable.mod_user, ''));
    }
}

function RemoveShipping(es) {
    if (confirm("Are you sure you want to remove this shipping address from your saved shipping list?")) {
        GlobalAjax.deactivate_shipping(es, ShippingListCallback);
    }
}

function ShippingListCallback(res) {
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
            var txt = GlobalAjax.ShippingListGet(customerid).value();
            $('#ShipHistoryList').html(txt);
        }
        else {
            ShowAutoAlert('Account Save Error', msg, 'error', true);
        }
    }
}

function Validate_customers_shipping() {
    var pass = true;
    var ChkFields = new Array('ship_name', 'ship_address1', 'ship_city', 'ship_state');
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

function SaveShipping() {
    
    if (Validate_customers_shipping()) {
        var cd = GlobalAjax.Getcustomers_shippingClass().value;
        cd.cust_shipID = GetMyElement('shippingID');
        cd.customerID = customerid;
        cd.ship_name = GetMyElement('ship_name');
        cd.ship_address1 = GetMyElement('ship_address1');
        cd.ship_city = GetMyElement('ship_city');
        cd.ship_state = GetMyElement('ship_state');
        cd.ship_zip = GetMyElement('ship_zip');
        cd.create_user = username;
        //Pass class to server, Save data
        GlobalAjax.Process_customers_shipping(cd, Savecustomers_shippingCallback);
    }

}

function Savecustomers_shippingCallback(res) {
    if (res.value == null || res.error != null) {
        ShowAutoAlert('Save Error', GetResError(res.error) + ' - ' + res.value, 'error', true);
    }
    else {
        var ret = res.value;
        var arr = ret.split("|");
        var err = arr[0];
        var msg = arr[1];

        if (err == 0) {
            ShowAutoAlert('Shippping Saved', msg, 'success', true);
            var txt = MyAccount.ShippingListGet(customerid).value;
            $('#ShipHistoryList').html(txt);
        }
        else {
            ShowAutoAlert('Shippping Save Error', msg, 'error', true);
        }
    }
}

function AddAddress() {
    //jQuery.noConflict();
    New_customers_shipping();
    $('#AccountAddressAddForm').modal('show');
}