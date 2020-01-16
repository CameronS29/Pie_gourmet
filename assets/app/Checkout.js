// J Scott King
// www.jskdesign.net 2014
// Default Page Client Functions

$(document).ready(function () {

    ShowCustomerData();

    $("#date_wanted").datepicker({
        autoclose: true,
        clearBtn: true,
        startDate: new Date(),
        beforeShowDay: BlockNextDay
    });
});

var ShipType = "";

function ShowCustomerData() {
    GlobalAjax.Getcustomers(customerid, FillCustomerCallback);
}

function BlockNextDay(date) {
    var today = new Date();
    var deliveryThreshold = new Date();
    deliveryThreshold.setDate(deliveryThreshold.getDate() + 2);

    if (ShipType != 'pickup') {
        if (date <= today) {
            return false;
        }

        if (date >= today && date <= deliveryThreshold) {
            return false;
        }
    }
}


function FillCustomerCallback(res) {
    if (res.value == null || res.error != null) return false;
    var rows = res.value.Tables[0].Rows.length;

    if (rows >= 1) {
        var dTable = res.value.Tables[0].Rows[0];
        SetMyElement('title', escapeNull(dTable.title, ''));
        SetMyElement('CompanyName', escapeNull(dTable.company, ''));
        SetMyElement('FirstName', escapeNull(dTable.first_name, ''));
        SetMyElement('LastName', escapeNull(dTable.last_name, ''));
        SetMyElement('MyStreet', escapeNull(dTable.address1, ''));
        SetMyElement('MyCity', escapeNull(dTable.city, ''));
        SetMyElement('MyState', escapeNull(dTable.state, ''));
        SetMyElement('MyZip', escapeNull(dTable.zip, ''));
        SetMyElement('MyEmail', escapeNull(dTable.email, ''));
        SetMyElement('MyPhone', escapeNull(dTable.home_phone, ''));
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


function New_customers_shipping() {
    var cd = GlobalAjax.Getcustomers_shippingClass().value;

    SetMyElement('shippingID', cd.cust_shipID);
    SetMyElement('ship_name', escapeNull(cd.ship_name, ''));
    SetMyElement('ship_phone', escapeNull(cd.ship_phone, ''));
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
            var txt = GlobalAjax.ShippingListGetCheckout(customerid).value();
            $('#ShipHistoryList').html(txt);
        }
        else {
            ShowAutoAlert('Account Save Error', msg, 'error', true);
        }
    }
}

function Validate_customers_shipping() {
    var pass = true;
    var ChkFields = new Array('ship_name', 'ship_address1', 'ship_city', 'ship_state', 'ship_phone');
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
        cd.ship_phone = GetMyElement('ship_phone');
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
            msg = "Shipping address item saved successfully!"
            ShowAutoAlert('Shippping Saved', msg, 'success', true);
            var txt = GlobalAjax.ShippingListGetCheckout(customerid).value;
            $('#ShipHistoryList').html(txt);
            $('#AccountAddressAddForm').modal('hide');
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

function CheckShippingSelect(ele) {
    var val = ele.value;
    //console.log(val);
    ShipType = val;

    if (!ele.checked) {
        $('#ShippingTable').show();
        $('#ship1').show();
        $('#ship2').show();
        $('#ship3').show();
        $('#ShipTypeSet').val('');
        $('.CustomShipListClass').prop('checked', false);
        $('#cust_shipID').val(0);

    } else {
        if (val == 'shiptobill') {
            $('#ShippingTable').hide();
            $('#ship1').hide();
            $('#ship3').hide();
            $('#cust_shipID').val(0);
            $('#ShipTypeSet').val('shiptobill');
        } else if (val == 'pickup') {
            $('#ShippingTable').hide();
            $('#cust_shipID').val(0);
            $('#ship2').hide();
            $('#ship3').hide();
            $('#ShipTypeSet').val('pickup');
        }
    }

    $('#date_wanted').datepicker('update');
}

function SetCustomShipping(id) {
    $('#cust_shipID').val(id);
    $('#ShipTypeSet').val('custom');
    $('#ship1').hide();
    $('#ship2').hide();
    SetChecked('CustomShipping', true);
}

function UpdateShippingInCart() {

}

function CheckPayType(ele) {
    var pt = ele.value;
    if (pt == 'cc') {
        $('#pay_cc').show();
        $('#pay_paypal').hide();
        $('#btnPlaceOrder').show();
    } else if (pt == 'paypal') {
        $('#pay_paypal').show();
        $('#pay_cc').hide();
        $('#btnPlaceOrder').hide();
    } else {
        $('#pay_cc').show();
        $('#pay_paypal').hide();
        $('#btnPlaceOrder').show();
    }
}

function RemoveCartCheckItem(cID) {
    if (confirm("Are you sure you want to remove this item from your cart?")) {
        GlobalAjax.RemoveProduct(cID, RemoveCheckoutItemCallback);
    }
}

function RemoveCheckoutItemCallback(res) {
    var html = Checkout.ProductsCheckout(sessionid).value;
    $('#ProductOrderList').html(html);
}

function EditCartCheckItem(id) {
    //jQuery.noConflict();
    $('#ProductForm').modal('show');
    Checkout.GetProduct(id, FillproductsForm);
}

function FillproductsForm(res) {
    if (res.value == null || res.error != null) return false;
    var rows = res.value.Tables[0].Rows.length;

    if (rows >= 1) {
        var dTable = res.value.Tables[0].Rows[0];
        LoadTempProductDetail(dTable.productID);

        SetMyElement('quantity', escapeNull(dTable.qty, 1));
        SetMyElement('ProductPricingID', escapeNull(dTable.pricingID, '0'));
        SetMyElement('temp_cartID', escapeNull(dTable.cart_tempID, '0'));

    }
}

function LoadTempProductDetail(id) {
    var html = Checkout.LoadProduct(id).value;
    $('#ProductCartEditForm').html(html);
}

function CancelUpdate() {
    $('#ProductForm').modal('hide');
}

function UpdateMyCart() {
    var id = $('#temp_cartID').val();
    var qty = $('#quantity').val();
    var price = $('#ProductPricingID').val();
    Checkout.SaveCartUpdate(id, qty, price, SaveProductUpdateCallback);
}

function SaveProductUpdateCallback(res) {
    if (res.value == null || res.error != null) {
        ShowAutoAlert('Save Error', GetResError(res.error) + ' - ' + res.value, 'error', true);
    }
    else {
        var ret = res.value;
        var arr = ret.split("|");
        var err = arr[0];
        var msg = arr[1];

        if (err == 0) {
            msg = "Cart item saved successfully!"
            ShowAutoAlert('Cart Updated', msg, 'success', true);
            UpdateCartDetails();
            $('#ProductForm').modal('hide');
        }
        else {
            ShowAutoAlert('Cart Save Error', msg, 'error', true);
        }
    }
}

function UpdateCartDetails() {
    var txt = Checkout.ProductsCheckout(sessionid).value;
    $('#ProductOrderList').html(txt);
}

function ValidateCheckout() {
    var pass = true;
    var FieldCheck = [];

    //Payment Validation
    var payType = GetMyRadio('payment_method');
    if (payType == 'cc') {
        var ccArray = ['CCNumber', 'CCExpireMonth', 'CCExpireYear', 'CCCode'];
        FieldCheck = FieldCheck.concat(ccArray);
    }
    else if (payType == 'paypal') {

    }
    //else if (payType == 'pickup') {
    //    //Contine to place order with no payment.
    //}
    else {
        pass = false;
        ShowAutoAlert('Payment Validation', 'Please select and process a payment method before saving...', 'warning', true);
    }

    //console.log(FieldCheck);
    for (var i = 0; i < FieldCheck.length; i++) {
        //Find Element
        //alert(FieldCheck[i]);
        var val = $('#' + FieldCheck[i]).val();
        var ele = document.getElementById(FieldCheck[i]);

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

function LoadTab1() {
    $('#iTab1').tab('show');
}

function ValidateTab1() {
    var pass = true;
    var FieldCheck = [];
    var ChkFields1 = ['FirstName', 'LastName', 'MyEmail', 'MyPhone','date_wanted'];
    var ChkFields2 = ['FirstName', 'LastName', 'MyStreet', 'MyCity', 'MyState', 'MyZip', 'MyEmail', 'MyPhone','date_wanted'];
    var CustomShipID = $('#cust_shipID').val();

    if (CustomShipID > 0) {
        FieldCheck = ChkFields1;
    }
    else if (CustomShipID == 0 && $('#ShipTypeSet').val() == "") {
        ShowAutoAlert('Form Validation', 'Please select a shipping location first...', 'warning', true);
        return false;
    } else {
        FieldCheck = ChkFields2;
    }

    //console.log(FieldCheck);

    for (var i = 0; i < FieldCheck.length; i++) {
        //Find Element
        //alert(FieldCheck[i]);
        var val = $('#' + FieldCheck[i]).val();
        var ele = document.getElementById(FieldCheck[i]);

        if (val == '') {
            ele.style.backgroundColor = '#FFFF00';
            pass = false;
        }
        else {
            ele.style.backgroundColor = '';
        }
    }

    if (!pass) {

        ShowAutoAlert('Billing/Shipping Validation', 'Please complete hi-lighted fields before saving...', 'warning', true);
    }
    return pass;

}

function SaveTab1() {

    if (ValidateTab1()) {
        //Set Shipping Page Data
        if (ShipType == 'pickup') {
            $('.pm_pay').show();
            $('.pm_pu').hide();
        } else {
            $('.pm_pay').show();
            $('.pm_pu').hide();
        }

        //Save Data
        var cd = GlobalAjax.GetcustomersClass().value;
        cd.company = GetMyElement('CompanyName');
        cd.customerID = customerid;
        cd.sessionID = sessionid;
        cd.cust_shipID = GetMyElement('cust_shipID');
        cd.title = "";
        cd.first_name = GetMyElement('FirstName');
        cd.last_name = GetMyElement('LastName');
        cd.address1 = GetMyElement('MyStreet');
        cd.address2 = GetMyElement('MyStreet2');
        cd.city = GetMyElement('MyCity');
        cd.state = GetMyElement('MyState');
        cd.zip = GetMyElement('MyZip');
        cd.email = GetMyElement('MyEmail');
        cd.home_phone = GetMyElement('MyPhone');
        cd.notes = GetMyElement('order_notes') + '\n Date Requested: ' + GetMyElement('date_wanted') + '\n Time Requested: ' + GetMyElement('time_wanted');
        cd.DeliveryMethod = GetMyElement('ShipTypeSet');
        cd.create_user = username;

        GlobalAjax.ProcessTab1(cd, ProcessTab1Callback);

    } else {
        return false;
    }
}

function ProcessTab1Callback(res) {
    if (res.value == null || res.error != null) {
        ShowAutoAlert('Save Error', GetResError(res.error) + ' - ' + res.value, 'error', true);
    }
    else {
        var ret = res.value;
        var arr = ret.split("|");
        var err = arr[0];
        var msg = arr[1];

        if (err == 0) {
            //Goto Tab 2
            $('#iTab2').tab('show');
            UpdateCartDetails();
            ShowAutoAlert('Details Saved', 'Billing and shipping details saved successfully!', 'success', true);
        }
        else {
            ShowAutoAlert('Account Save Error', msg, 'error', true);
        }
    }
}

function CompleteCheckout() {
    if (ValidateCheckout()) {
        //What Process Are We Running? ************************
        var pt = GetMyRadio('payment_method');
        //alert(pt);

        switch (pt) {
            case "cc":
                var cd = Checkout.GetEFTClass().value;
                cd.EFTCCNum = GetMyElement('CCNumber');
                cd.EFTCSV = GetMyElement('CCCode');
                cd.myMonth = GetMyElement('CCExpireMonth');
                cd.myYear = GetMyElement('CCExpireYear');
                cd.InvoiceRef = sessionid;
                cd.username = username;
                cd.PayBatch = sessionid;

                //Pass class to server, Save data
                Checkout.ValidateSubmitCreditCardProcess(cd, CompleteCheckoutCallback);
                break;
            case "pickup":
                Checkout.FinalOrderSubmit(sessionid, pt, CompleteCheckoutPickupCallback);

            case "paypal":
                //Processed on another page.
        }

    }
}

function CompleteCheckoutCallback(res) {
   // console.log(res.value);

    if (res.value == null || res.error != null) {
        ShowAutoAlert('Order Process Error', GetResError(res.error) + ' - ' + res.value, 'error', true);
    }
    else {
        var ret = res.value;
        var arr = ret.split("|");
        var err = arr[0];
        var msg = arr[1];
        var id = arr[2];

        if (err == 0) {
            //Goto Tab 2
            showFinalCheckout(id);
            ShowAutoAlert('Order Confirmed', 'Your order has been confirmed and we are busy getting it ready.', 'success', true);
        }
        else {
            ShowAutoAlert('Account Checkout Error', msg, 'error', true);
        }
    }
}

function CompleteCheckoutPickupCallback(res) {
    // console.log(res.value);

    if (res.value == null || res.error != null) {
        ShowAutoAlert('Order Process Error', GetResError(res.error) + ' - ' + res.value, 'error', true);
    }
    else {
        var ret = res.value;
        var arr = ret.split("|");
        var err = arr[0];
        var msg = arr[1];

        if (err == 0) {
            //Goto Tab 2
            showFinalCheckout(msg);
            ShowAutoAlert('Order Confirmed', 'Your order has been confirmed and we are busy getting it ready.', 'success', true);
        }
        else {
            ShowAutoAlert('Account Checkout Error', msg, 'error', true);
        }
    }
}

function showFinalCheckout(id) {
    var txt = Checkout.BuildCartItem(id).value;
    SetMyHTML('ProdCheckoutForm', txt);
}