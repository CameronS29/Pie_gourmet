// J Scott King
// www.jskdesign.net 2014
// Default Page Client Functions

$(document).ready(function () {
    List_Ingredients();
    OrderHistory2Years();
});

function ProductFilter(fValue) {
    ProductDetail.GetProductList(fValue, FilterProductCallback);
}

function FilterProductsCustom(fValue) {
    ProductDetail.GetProductListQuick(fValue, FilterProductCallback);
}

function FilterProductCallback(res) {
    SetMyHTML('ProductListEdit', res.value);
}

function AddPhoto() {
    popWin("AddPhoto.aspx", 500, 500);
}

function AddNewProduct() {
    NewProduct();
    //jQuery.noConflict();
    $('#ProductForm').modal('show');
}

function GetPricing() {
    var pg = $('#epricing_group').val();
    $('#PricingForm').modal('show');
    GetPricingListByScheme(pg);

}

function GetPricingListByScheme(val) {
    ProductDetail.Listpricing(val, GetPriceListCallback);
    SetMyElement('pSchemeFilter', val);
}

function GetPriceListCallback(res) {
    SetMyHTML('FillPricingList', res.value);
}


function NewProduct() {
    var cd = ProductDetail.GetproductsClass().value;

    SetMyElement('eProductID', cd.productID);
    SetMyElement('epricing_group', escapeNull(cd.pricing_group, ''));
    SetMyElement('eIngredientsID', escapeNull(cd.IngredientsID, ''));
    SetMyElement('eCategory', escapeNull(cd.Category, ''));
    SetMyElement('eProductName', escapeNull(cd.ProductName, ''));
    SetMyElement('eProductDescription', escapeNull(cd.ProductDescription, ''));
    SetChecked('eActive', IntYesNo(cd.active));
    SetChecked('eInStock', IntYesNo(cd.InStock));
    SetChecked('eFresh', IntYesNo(cd.Fresh));
    SetChecked('eFeatured', IntYesNo(cd.Featured));
    SetMyElement('eRating', escapeNull(cd.Rating, '0'));
    SetMyHTML('edate_row', '');
    SetMyHTML('prod_err', '');

}


function EditProduct() {
    //jQuery.noConflict();
    var p = $('#MyProductID').val();
    $('#ProductForm').modal('show');
    ProductDetail.Getproducts(p, FillproductsForm);
}

function FillproductsForm(res) {
    if (res.value == null || res.error != null) return false;
    var rows = res.value.Tables[0].Rows.length;

    if (rows >= 1) {
        var dTable = res.value.Tables[0].Rows[0];
        SetMyElement('eProductID', escapeNull(dTable.productID, 0));
        SetMyElement('epricing_group', escapeNull(dTable.pricing_group, ''));
        SetMyElement('eIngredientsID', escapeNull(dTable.IngredientsID, ''));
        SetMyElement('eCategory', escapeNull(dTable.Category, ''));
        SetMyElement('eProductName', escapeNull(dTable.ProductName, ''));
        SetMyElement('eProductDescription', escapeNull(dTable.ProductDescription, ''));
        
        SetChecked('eActive', escapeNull(dTable.active, false));
        SetChecked('eInStock', escapeNull(dTable.InStock, false));
        SetChecked('eFresh', escapeNull(dTable.Holiday, false));
        SetChecked('eFeatured', escapeNull(dTable.Featured, false));
        SetMyElement('eRating', escapeNull(dTable.Rating, ''));


        SetMyHTML('edate_row', 'Created: ' + dTable.create_date + ':' + escapeNull(dTable.create_user, '') + ' | Modified: ' + escapeNull(dTable.mod_date, '') + ':' + escapeNull(dTable.mod_user, ''));
    }
}

function RemoveShipping(es) {
    if (confirm("Are you sure you want to remove this shipping address from your saved shipping list?")) {
        MyAccount.deactivate_shipping(es, ShippingListCallback);
    }
}

function Validate_Product() {
    var pass = true;
    var ChkFields = new Array('eProductName', 'eCategory', 'epricing_group');
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

function SaveProduct() {

    if (Validate_Product()) {
        var cd = ProductDetail.GetproductsClass().value;
        cd.productID = GetMyElement('eProductID');
        cd.pricing_group = GetMyElement('epricing_group');
        cd.IngredientsID = GetMyElement('eIngredientsID');
        cd.Category = GetMyElement('eCategory');
        cd.ProductName = GetMyElement('eProductName');
        cd.ProductDescription = GetMyElement('eProductDescription');
        cd.active = JYesNo(GetChecked('eActive'));
        cd.InStock = JYesNo(GetChecked('eInStock'));
        cd.Fresh = JYesNo(GetChecked('eFresh'));
        cd.Featured = JYesNo(GetChecked('eFeatured'));
        cd.Rating = GetMyElement('eRating');
        cd.create_user = username;

        //Pass class to server, Save data
        ProductDetail.Process_products(cd, SaveproductsCallback);

    }

}

function SaveproductsCallback(res) {
    if (res.value == null || res.error != null) {
        ShowAutoAlert('Save Error', GetResError(res.error) + ' - ' + res.value, 'error', true);
    }
    else {
        var ret = res.value;
        var arr = ret.split("|");
        var err = arr[0];
        var msg = arr[1];

        if (err == 0) {
            msg = "(" + msg + ") Product saved successfully!";
            ShowAutoAlert('Product Saved', msg, 'success', true);
            window.setTimeout("RefreshPage()", 3000);
        }
        else {
            ShowAutoAlert('Product Save Error', msg, 'error', true);
        }
    }
}

function RefreshPage() {
    location.href = "ProductDetail.aspx";
}

function NewIngredientWin() {
    //jQuery.noConflict();
    $('#IngredientForm').modal('show');

    var cd = GlobalAjax.GetIngredientsClass().value;

    SetMyElement('IngredientID', cd.IngredientID);
    SetMyElement('IngredientName', escapeNull(cd.IngredientName, ''));
    SetMyElement('Description', escapeNull(cd.Description, ''));
    SetMyHTML('idate_row', '');
    SetMyHTML('ierr_text', '');
}

function New_Ingredients() {
    var cd = GlobalAjax.GetIngredientsClass().value;

    SetMyElement('IngredientID', cd.IngredientID);
    SetMyElement('IngredientName', escapeNull(cd.IngredientName, ''));
    SetMyElement('Description', escapeNull(cd.Description, ''));
    SetMyHTML('idate_row', '');
    SetMyHTML('ierr_text', '');
}

function Validate_Ingredients() {
    var pass = true;
    var ChkFields = new Array('IngredientName', 'Description');
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

function SaveIngredients() {
    if (Validate_Ingredients()) {
        var cd = GlobalAjax.GetIngredientsClass().value;
        cd.IngredientID = GetMyElement('IngredientID');
        cd.IngredientName = GetMyElement('IngredientName');
        cd.Description = GetMyElement('Description');
        cd.create_user = myemail;
        //Pass class to server, Save data
        GlobalAjax.Process_Ingredients(cd, SaveIngredientsCallback);
    }
}

function SaveIngredientsCallback(res) {
    if (res.value == null || res.error != null) {
        ShowAutoAlert('Save Error', GetResError(res.error) + ' - ' + res.value, 'error', true);
    }
    else {
        var ret = res.value;
        var arr = ret.split("|");
        var err = arr[0];
        var msg = arr[1];

        if (err == 0) {
            msg = "(" + msg + ") Ingredient saved successfully!";
            ShowAutoAlert('Ingredient Saved', msg, 'success', true);
            List_Ingredients();
        }
        else {
            ShowAutoAlert('Ingredient Save Error', msg, 'error', true);
        }
    }
}

function Get_Ingredients(ele) {
    if (ele >= 1) {
        //jQuery.noConflict();
        $('#IngredientForm').modal('show');
        GlobalAjax.GetIngredients(ele, FillIngredientsForm);
    }
}

function FillIngredientsForm(res) {
    if (res.value == null || res.error != null) return false;
    var rows = res.value.Tables[0].Rows.length;
    if (rows >= 1) {
        var dTable = res.value.Tables[0].Rows[0];
        SetMyElement('IngredientID', escapeNull(dTable.IngredientID, 0));
        SetMyElement('IngredientName', escapeNull(dTable.IngredientName, ''));
        SetMyElement('Description', escapeNull(dTable.Description, ''));
        SetMyHTML('idate_row', 'Created: ' + dTable.create_date + ':' + escapeNull(dTable.create_user, '') + ' | Modified: ' + escapeNull(dTable.mod_date, '') + ':' + escapeNull(dTable.mod_user, ''));
        SetMyHTML('ierr_text', '');
    }
}
function Delete_Ingredients(ele) {
    if (ele >= 1) {
        if (confirm('Are you sure you want to delete this record?'))
            GlobalAjax.DeleteIngredients(ele, Delete_IngredientsCallback);
    }
}
function Delete_IngredientsCallback(res) {
    var d = document.getElementById('IngredientList');
    res.value == null || res.error != null ? d.innerHTML = res.error.Message :
	List_Ingredients();
}

function List_Ingredients() {
    var list = GlobalAjax.ListIngredients().value;
    //Return list to server...
    SetMyHTML('IngredientList', list);
}

function OrderHistory2Years() {
    var p = $('#MyProductID').val();
    var retStr = "";
    var d = new Date();
    var n = d.getFullYear();

    if (p >= 1) {
        //Current Year
        retStr = ProductDetail.GetOrderHistory(p, n).value;

        retStr += ProductDetail.GetOrderHistory(p, n-1).value;

        SetMyHTML('OrderHistory', retStr);
    }
}


function Validate_pricing() {
    var pass = true;
    var ChkFields = new Array('pricing_group', 'ProductPrice', 'Packaging', 'ShipWeight', 'ProductSize', 'ProductSize');
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

function New_pricing() {
    var cd = ProductDetail.GetpricingClass().value;
    SetMyElement('pricingID', cd.pricingID);
    SetMyElement('pricing_group', escapeNull(cd.pricing_group, ''));
    SetMyElement('GroupName', escapeNull(cd.GroupName, ''));
    SetMyElement('pProductPrice', escapeNull(cd.ProductPrice, ''));
    SetMyElement('Packaging', escapeNull(cd.Packaging, ''));
    SetMyElement('ShipWeight', escapeNull(cd.ShipWeight, ''));
    SetMyElement('ProductSize', escapeNull(cd.ProductSize, ''));
    SetMyElement('TypeCategory', escapeNull(cd.TypeCategory, ''));
    SetMyElement('TypeQuantity', escapeNull(cd.TypeQuantity, ''));
}

function Savepricing() {
    if (Validate_pricing()) {
        var cd = ProductDetail.GetpricingClass().value;
        cd.pricingID = GetMyElement('pricingID');
        cd.pricing_group = GetMyElement('pricing_group');
        cd.GroupName = GetMyElement('GroupName');
        cd.ProductPrice = GetMyElement('pProductPrice');
        cd.Packaging = GetMyElement('Packaging');
        cd.ShipWeight = GetMyElement('ShipWeight');
        cd.ProductSize = GetMyElement('ProductSize');
        cd.TypeCategory = GetMyElement('TypeCategory');
        cd.TypeQuantity = GetMyElement('TypeQuantity');
        cd.create_user = myemail;
        //Pass class to server, Save data
        ProductDetail.Process_pricing(cd, SavepricingCallback);
    }
}

function SavepricingCallback(res) {
    if (res.value == null || res.error != null) {
        ShowAutoAlert('Save Error', GetResError(res.error) + ' - ' + res.value, 'error', true);
    }
    else {
        var ret = res.value;
        var arr = ret.split("|");
        var err = arr[0];
        var msg = arr[1];

        if (err == 0) {
            msg = "(" + msg + ") Pricing saved successfully!";
            ShowAutoAlert('Pricing Saved', msg, 'success', true);
            List_pricing();
        }
        else {
            ShowAutoAlert('Pricing Save Error', msg, 'error', true);
        }
    }
}

function Get_pricing(ele) {
    if (ele >= 1) {
        ProductDetail.Getpricing(ele, FillpricingForm);
    }
}

function FillpricingForm(res) {
    if (res.value == null || res.error != null) return false;
    var rows = res.value.Tables[0].Rows.length;
    if (rows >= 1) {
        var dTable = res.value.Tables[0].Rows[0];
        SetMyElement('pricingID', escapeNull(dTable.pricingID, 0));
        SetMyElement('pricing_group', escapeNull(dTable.pricing_group, ''));
        SetMyElement('GroupName', escapeNull(dTable.GroupName, ''));
        SetMyElement('pProductPrice', escapeNull(dTable.ProductPrice, ''));
        SetMyElement('Packaging', escapeNull(dTable.Packaging, ''));
        SetMyElement('ShipWeight', escapeNull(dTable.ShipWeight, ''));
        SetMyElement('ProductSize', escapeNull(dTable.ProductSize, ''));
        SetMyElement('TypeCategory', escapeNull(dTable.TypeCategory, ''));
        SetMyElement('TypeQuantity', escapeNull(dTable.TypeQuantity, ''));
        SetMyHTML('pdate_row', 'Created: ' + dTable.create_date + ':' + escapeNull(dTable.create_user, '') + ' | Modified: ' + escapeNull(dTable.mod_date, '') + ':' + escapeNull(dTable.mod_user, ''));

    }
}

function Delete_pricing(ele) {
    if (ele >= 1) {
        if (confirm('Are you sure you want to delete this record?'))
            ProductDetail.Deletepricing(ele, Delete_pricingCallback);
    }
}
function Delete_pricingCallback(res) {
    var d = document.getElementById('SearchList1');
    res.value == null || res.error != null ? d.innerHTML = res.error.Message :
	List_pricing();
}

function List_pricing() {
    var pg = GetMyElement('pSchemeFilter');
    var list = ProductDetail.Listpricing(pg).value;
    //Return list to server...
    SetMyHTML('FillPricingList', list);
}
