///////////////////////////////// Common Functions //////////////////////////////////////////
// (c) 2006 NVMS Corp (J. Scott King)
// 2006 Sept 29th

/* Generic Utilities */

/*
//Disable right mouse click Script
var message="Function Disabled!";

	///////////////////////////////////
	function clickIE4(){
	if (event.button==2){
		alert(message);
		return false;
		}
	}
	function clickNS4(e){
		if (document.layers||document.getElementById&&!document.all){
		if (e.which==2||e.which==3){
		alert(message);
		return false;
		}
	  }
	}

	if (document.layers){
		document.captureEvents(Event.MOUSEDOWN);
		document.onmousedown=clickNS4;
	}
	else if (document.all&&!document.getElementById){
		document.onmousedown=clickIE4;
	}

	document.oncontextmenu=new Function("alert(message);return false");
*/
// -->

// My Client Helper Functions
//Global Variables
var sboxflag = false;

// Get and Set Element Properties (Client-Side) /////////////////////////
    function GetMyElement(ele)
    {
        try
        {
            var e = document.getElementById(ele).value;
            return e;
        }
        catch(e)
        {
            return "";
        }
    }
    
	function GetMyElementNum(ele)
    {
        try
        {
            var e = document.getElementById(ele).value;
            return e;
        }
        catch(e)
        {
            return "0";
        }
    }
    
	function GetMyRadio(ele)
	{
		try {
			var ip=document.getElementsByName(ele);
			//alert(ip.length);
			for (i=0;i<ip.length;i++)
			{
				if(ip[i].checked)
				{
					return ip[i].value;
				}
			}
		}
		 catch(e)
        {
            return "";
        }
	}
	
	function SetMyRadio(ele, val)
	{
		try {
			var ip=document.getElementsByName(ele);
			//alert(ip.length);
			for (i=0;i<ip.length;i++)
			{
				if(ip[i].value == val)
				{
					ip[i].checked = true;
				}
			}
		}
		 catch(e)
        {
            return "";
        }
	}
	
    function SetMyElement(ele, val)
    {
        try
        {
            var e = document.getElementById(ele);
            e.value = val;
        }
        catch(e)
        {
            return "";
        }
    }

	

	function GetMyHTML(ele)
    {
        try
        {
            var e = document.getElementById(ele);
            return e.innerHTML;
        }
        catch(e)
        {
            return "";
        }
    }
	
    function SetMyHTML(ele, val)
    {
        try
        {
            var e = document.getElementById(ele);
            e.innerHTML = val;
        }
        catch(e)
        {
            return "";
        }
    }
    
	function SetMyImage(ele, img)
    {
        try
        {
            var e = document.getElementById(ele);
            e.src = img;
        }
        catch(e)
        {
            return false;
        }
    }
	
	function SetMyClass(ele, cls)
    {
        try
        {
            var e = document.getElementById(ele);
            e.className = cls;
        }
        catch(e)
        {
            return false;
        }
    }
	
    function GetChecked(ele)
    {
        try
        {
            var e = document.getElementById(ele);
            return e.checked;
        }
        catch(e)
        {
            return false;
        }
    }
    
    function SetChecked(ele, val)
    {
        try
        {
			var sclass = $( "#" + ele ).hasClass( "make-switch" );
			
			if (sclass)
			{
				$('#' + ele).bootstrapSwitch('setState', val);
			}
			else
			{
				var e = document.getElementById(ele);
				e.checked = val;
			}
        }
        catch(e)
        {
            return false;
        }
    }
	
	function IsFieldValid(ele)
	{
		try
        {
            var e = GetMyElement(ele);
            if(e == null || e == "")
				return false;
			else
				return true;
        }
        catch(e)
        {
            return false;
        }
	}
	
	function GetResError(ref)
	{
		try {
			if(ref != null)
			{
				var msg = ref.Message;
				if(msg != 'undefined')
					return msg;
				else
					return "";
			}
			else
			{
				return "";
			}
		}
		catch(e) {
			return "";
		}
	}


function ToggleSelects(vis)
{
	// grab all select fields and set input class
	//var ver = Cookies.browse_check;
	
	//alert(ver);
	try
	{
		//var co = myCoType;
		var co = 'None';
		
		if(ver == 'false' && co == 'CLIENT')
		{
			ip=document.getElementsByTagName('select');
			//alert(ip.length);
			for (i=0;i<ip.length;i++)
			{
				ip[i].style.display = vis;
			}
		}
	}
	catch(e)
	{
		//Error processing select fields.
	}
}

function DisplaySelects(vis)
{
	try
	{
		ip=document.getElementsByTagName('select');
		//alert(ip.length);
		for (i=0;i<ip.length;i++)
		{
			ip[i].style.display = vis;
		}
		
	}
	catch(e)
	{
		//Error processing select fields.
	}
}

function ShowListAlert(title, msg)
{
    var qform = document.getElementById("DetailList");
    var atitle = document.getElementById("DetTitle");
    var atext = document.getElementById("DetContent");
    atitle.innerHTML = title;
    atext.innerHTML = msg;
	qform.style.display = "";
}

function HideListAlert()
{
    var qform = document.getElementById("DetailList");
	qform.style.display = "none";
}

function ShowAlert(title, msg, flag)
{
    var opts = {
		"closeButton": true,
		"debug": false,
		"positionClass": "toast-top-right",
		"onclick": null,
		"showDuration": "300",
		"hideDuration": "1000",
		"timeOut": "9000",
		"extendedTimeOut": "6000",
		"showEasing": "swing",
		"hideEasing": "linear",
		"showMethod": "fadeIn",
		"hideMethod": "fadeOut"
	};
	
	switch (flag)
	{
		case 'error':
			toastr.error(msg, title, opts);
			break;
		case 'success':
			toastr.success(msg, title, opts);
			break;
		case 'warning':
			toastr.warning(msg, title, opts);
			break;
		default:
			toastr.info(msg, title, opts);
			break;
	}
}

function HideAlert()
{
    toastr.clear()
}

function ShowAlertManualPos(ele, centeron, left, top)
{
    var qform = document.getElementById(ele);
	qform.style.display = "";
	
	if(centeron)
	{
		var cele = centeron;
		PositionMyElement(qform, cele, left, top);
	}
}

function ShowAlertManual(ele, centeron)
{
    var qform = document.getElementById(ele);
	qform.style.display = "";
	
	if(centeron)
	{
		var cele = document.getElementById(centeron);
		PositionMyElement(qform, cele);
	}
}

function ShowAlertRight(ele, centeron)
{
    var qform = document.getElementById(ele);
	qform.style.display = "";
	
	if(centeron)
	{
		var cele = document.getElementById(centeron);
		PositionMyElement(qform, cele, -460);
	}
}

function HideAlertManual(ele)
{
    var qform = document.getElementById(ele);
	qform.style.display = "none";
	
}


// End Dark Sheet ////////////////////////////////////////////////////////
function PictureWin(theURL,winName) {
  var features = "toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=500,height=500"
  var winHandle = window.open('','',features)
  if(winHandle != null){
    var htmlString = "<html><head><title>" + winName + "</title></head>"
    htmlString = htmlString + "<body topmargin=0 leftmargin=0 marginwidth=0 marginheight=0>"
    htmlString = htmlString + "<a href='javascript:void(0)' onclick='window.close()'><img src='" + theURL + "' title='click image to close...' name=img border=0></a></body></html>"
    winHandle.document.open();
    winHandle.document.write(htmlString);
    winHandle.document.close();
  }
  if (document.layers) {
    winHandle.resizeTo(winHandle.document.images[0].width+50,winHandle.document.images[0].height+80);
	winHandle.focus();
	return winHandle;
  }
  if((winHandle != null) && (winHandle.document.all.img.width > 30)){
    winHandle.resizeTo(winHandle.document.all.img.width + 50,winHandle.document.all.img.height + 80);
	winHandle.focus(); //brings window to top
  }else{
    winHandle.resizeTo(640,480);
	winHandle.focus(); //brings window to top
  }
    return winHandle;
  }
  
 function ContentsWindow(contents,winName) {
  var features = "toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=500,height=500"
  var winHandle = window.open('','',features)
  if(winHandle != null){
    var htmlString = "<html><head>"
	htmlString = htmlString + "<title>" + winName + "</title>"
	htmlString = htmlString + "<link href='../Stylin.css' rel='stylesheet' type='text/css' />"
	htmlString = htmlString + "</head>"
    htmlString = htmlString + "<body topmargin=0 leftmargin=0 marginwidth=0 marginheight=0>"
	htmlString = htmlString + "<div class='ButtonRow'>"
    htmlString = htmlString + "<input type='button' value='Close' onclick='window.close()' class='input_btn'>&nbsp;<input type='button' value='Print' onclick='window.print()' class='input_btn'>"
    htmlString = htmlString + "</div>"
	htmlString = htmlString + contents
	htmlString = htmlString + "</body></html>"
	winHandle.document.open();
    winHandle.document.write(htmlString);
    winHandle.document.close();
  }
  
  if (document.layers) {
	winHandle.focus();
	return winHandle;
  }
  if(winHandle != null){
    winHandle.resizeTo(840,780);
	winHandle.focus(); //brings window to top
  }else{
    winHandle.resizeTo(840,780);
	winHandle.focus(); //brings window to top
  }
    return winHandle;
 }
  
function Error(errorMessage, errorSource) {
    this.message = errorMessage;
    this.source = errorSource;
    this.addSource =
        function (additionalSource) {
            this.source = additionalSource + ":" + this.source;
        };
    this.toString =
        function () {
            return "Error: " + this.message + " (" + this.source + ")";
        };
}

function newID() {
	date_val = new Date().valueOf();
	var range = 99999 - 10000 + 1;
	var random_num = 10000 + Math.floor(Math.random()*range);
	result = date_val + random_num.toString();
	return result;
}

function isUndefined(item) {
    return typeof item == "undefined";
}

function GetBrowserType()
{
	//alert(navigator.userAgent);
	var str = navigator.userAgent;
	
	if(str.indexOf('MSIE', 1) >= 1) //Is IE?
	{
		return 1;
	}
	else if(str.indexOf('Firefox', 1) >= 1) // Is Firefox?
	{
		return 2;
	}
	else if(str.indexOf('Chrome', 1) >= 1) // Is Chome?
	{
		return 3;
	}
	else // Is Something Else...
	{
		return 4;
	}
}

// Get older versions of JavaScript up to speed with a few useful additions.

String.prototype.toBool =
    function () {
        return this.toLowerCase() == "true";
    };

String.prototype.htmlEncode =
    function() {
    	var val = this;
    	val = val.replace(/&/g, "&amp;");
    	val = val.replace(/</g, "&lt;");
    	val = val.replace(/>/g, "&gt;");
    	val = val.replace(/"/g, "&quot;");
    	return val;
    };

String.prototype.htmlDecode =
    function() {
    	var val = this;
    	val = val.replace(/&amp;/g, "&");
    	val = val.replace(/&lt;/g, "<");
    	val = val.replace(/&gt;/g, ">");
    	val = val.replace(/&quot;/g, "\"");
    	return val;
    };

String.prototype.urlEncode =
    function() {
    	var str_val = this, chars = new Array();
        var val_length = str_val.length, i, char_val, char_code;

        for(i = 0; i < val_length; i++) {
            char_val = str_val.charAt(i);
            char_code = char_val.charCodeAt(0);

            if(char_code == 32)
                chars[i] = "+";
            else if((char_code >= 48 && char_code <= 57) || (char_code >= 65 && char_code <= 90) || (char_code >= 97 && char_code <= 122))
                chars[i] = char_val;
            else {
                char_val = char_code.toString(16).toUpperCase();
                chars[i] = "%" + (char_val.length == 1 ? "0" : "") + char_val;
            }
        }
        return chars.join("");
    };

// FORNOW - Make a String.prototype.urlDecode
String.prototype.removeLineFeeds =
    function() {
    	var val = this;
    	val = val.replace(/\r\n|\r|\n/g, " ");
    	return val;
    };

String.prototype.trim = function() {return this.replace(/(^\s+)|(\s+$)/g, "");};

// Rounding to a certain precision.
function precisionRound(val, prec) {
    if (prec < 1) return val;
    var mult = 1;
    for (var i = 1; i <= prec; i++) {
        mult = mult * 10;
    }
    return Math.round(val * mult) / mult;
}

function optionalS(val, singular, plural) {
    return val + " " + (val == 1 ? singular : plural);
}

function ensureInUrl(url, stuffToEnsure) {
    if(stuffToEnsure) {
        if(url.indexOf("?") > -1) {
            if(url.toLowerCase().indexOf(stuffToEnsure.toLowerCase()) == -1) {
                if(url.substring(url.length-1) == "&")
                    url += stuffToEnsure;
                else
                    url += "&" + stuffToEnsure;
            }
        }
        else
            url += "?" + stuffToEnsure;
    }
    return url;
}

//Use sliding effect to toggle display
function ToggleDashEffect(ele)
{
	//var ele = document.getElementById(obj);
	//alert(ele.style.display);
	try {
	    $('#' + ele).slideToggle('medium');
	 }
	catch(e)
	{
	    toggleDisplay(ele);
	}

}

//Use sliding effect to toggle display
function ToggleElement(ele, dir) {
    //alert(ele);
	if(!dir) dir = 'right';
	
    try {
		if(dir == 'up')
		{
			$('#' + ele).slideToggle(500);
		}
		else if(dir == 'right')
		{
			var $marginLefty = $('#' + ele).next();
			$marginLefty.animate({
				marginLeft: parseInt($marginLefty.css('marginLeft'),10) == 0 ? 
				$marginLefty.outerWidth() : 0
			});
		}
		else
		{
			$('#' + ele).slideToggle(500);
		}
    }
    catch (e) {
        toggleDisplay(ele);
    }

}

// Toggle the display of an element, or a list of elements.
function toggleDisplay(item) {
    if (item.style) {
        item.style.display = item.style.display == "" ? "none" : "";
		SetDisplayFlag(item.style.display);
    }
    else if (document.getElementById(item)) {
        toggleDisplay(document.getElementById(item));
    }
    else if (typeof(item) == "object") {
        for (var i = 0; i < item.length; i++) {
            toggleDisplay(item[i]);
        }
    }
    else return;
}

function SetDisplayFlag(disp)
{
	//alert(disp);
	disp == "none" ? sboxflag = false: sboxflag = true;
}

// Set the display of an element, or a list of elements.
function setDisplay(item, shouldBeShown) {
    if (item.style) {
        item.style.display = shouldBeShown ? "" : "none";
    }
    else if (document.getElementById(item)) {
        setDisplay(document.getElementById(item), shouldBeShown);
    }
    else if (typeof(item) == "object") {
        for (var i = 0; i < item.length; i++) {
            setDisplay(item[i], shouldBeShown);
        }
    }
    else return;
}

// Toggle the visibility of an element, or a list of elements.
function toggleVisibility(item) {
    if (item.style) {
        item.style.visibility = item.style.visibility == "" ? "hidden" : "";
    }
    else if (document.getElementById(item)) {
        toggleVisibility(document.getElementById(item));
    }
    else if (typeof(item) == "object") {
        for (var i = 0; i < item.length; i++) {
            toggleVisibility(item[i]);
        }
    }
    else return;
}

// Set the visibility of an element, or a list of elements.
function setVisibility(item, shouldBeShown) {
    if (item.style) {
        item.style.visibility = shouldBeShown ? "" : "hidden";
    }
    else if (document.getElementById(item)) {
        setVisibility(document.getElementById(item), shouldBeShown);
    }
    else if (typeof(item) == "object") {
        for (var i = 0; i < item.length; i++) {
            setVisibility(item[i], shouldBeShown);
        }
    }
    else return;
}


/*
    Format a number.
    Zeros trailing after the decimal point are dropped.
    Set numDigitsAfterDecimal to -1 to leave the numbers after the decimal point unaltered.
*/
function FormatNumber(number, commify, numDigitsAfterDecimal, includeLeadingZero, useParensForNegative, iscurrency) {
    if(isNaN(parseInt(number)))
        return "NaN";

    // Get sign of number.
    var iSign = number < 0 ? -1 : 1;

    // Adjust number so only the specified number of numbers after the decimal point are shown.
    if(numDigitsAfterDecimal >= 0) {
        number *= Math.pow(10, numDigitsAfterDecimal);
        number =  Math.round(Math.abs(number));
        number /= Math.pow(10, numDigitsAfterDecimal);
        number *= iSign;
    }

    // Create a string object on which to do the formatting.
    var number_str = new String(number);

    // Remove the leading zero if necessary.
    if(! includeLeadingZero && number < 1 && number > -1 && number != 0) {
        if(number > 0)
            number_str = number_str.substring(1, number_str.length);
        else
            number_str = "-" + number_str.substring(2, number_str.length);
    }

    // Add commas if necessary.
    if(commify && (number >= 1000 || number <= -1000)) {
        var index = number_str.indexOf(".");
        if(index < 0)
            index = number_str.length;

        index -= 3;
        while(index >= 1) {
            number_str = number_str.substring(0, index) + "," + number_str.substring(index, number_str.length);
            index -= 3;
        }
    }

    // Add parenthesese if necessary.
    if(useParensForNegative && number < 0)
        number_str = "(" + number_str.substring(1, number_str.length) + ")";

    // Return the formatted number.
    if(iscurrency)
    	return '$' + number_str;
    else
    	return number_str;

}

function CommaFormatted(amount)
{
	var delimiter = ","; // replace comma if desired
	var a = amount.split('.',2);
	var d = a[1];
	var i = parseInt(a[0]);
	if(isNaN(i)) { return ''; }
	var minus = '';
	if(i < 0) { minus = '-'; }
	i = Math.abs(i);
	var n = new String(i);
	var a = [];
	while(n.length > 3)
	{
		var nn = n.substr(n.length-3);
		a.unshift(nn);
		n = n.substr(0,n.length-3);
	}
	if(n.length > 0) { a.unshift(n); }
	n = a.join(delimiter);
	if(d.length < 1) { amount = n; }
	else { amount = n + '.' + d; }
	amount = minus + amount;
	return amount;
}
// end of function 

function CurrencyFormatted(amount, nodollar)
{
	var i = parseFloat(amount);
	if(isNaN(i)) { i = 0.00; }
	var minus = '';
	if(i < 0) { minus = '-'; }
		i = Math.abs(i);
		i = parseInt((i + .005) * 100);
		i = i / 100;
		s = new String(i);
	if(s.indexOf('.') < 0) { s += '.00'; }
	if(s.indexOf('.') == (s.length - 2)) { s += '0'; }
	s = minus + s;
	var ret = (nodollar) ? s: '$' + s;
	return ret;
	
}
// end of function 

//Parse number to currency format:
//Remove the $ sign if you wish the parse number to NOT include it
function ParseDecimalFormat(thisone){
var prefix=""
var wd

    if (thisone.value.charAt(0)=="$")
        return
    wd="w"
    var tempnum=thisone.value
    for (i=0;i<tempnum.length;i++){
        if (tempnum.charAt(i)=="."){
        wd="d"
        break
        }
    }
    if (wd=="w")
    thisone.value=prefix+tempnum+".00"
    else{
    if (tempnum.charAt(tempnum.length-2)=="."){
        thisone.value=prefix+tempnum+"0"
    }
    else{
        tempnum=Math.round(tempnum*100)/100
        thisone.value=prefix+tempnum
        }
    }
}

function ParseDate(pdate)
{
	if(pdate)
	{
		var nd = new Date(pdate);
		//alert(nd.toLocaleDateString());
		var dy = nd.toLocaleDateString();
		return dy;
	}
	else
	{
		return "";
	}

}

function IsDate(dt) {
	var d = new Date(dt);
	 if(d && d.getFullYear()>0) 
		return true;
	 else 
		return false;
}

// Focus a form element.
function FocusFormElement(formName, elementName, doSelect) {
    try {
        document.forms[formName].elements[elementName].focus();
        if (doSelect) document.forms[formName].elements[elementName].select();
    }
    catch(e) {
      //No Focus
    }
}

// Ensure cookies are enabled.
function EnsureCookiesEnabled() {
    if(!window.navigator.cookieEnabled)
       return false;
    else
	   return true;
}



function frameDoc(frameName, ignoreNotFoundError) {
    var frame = window.top.document.getElementById(frameName);
    var frame_doc = null;

    try {
        if(frame) {
            if(frame.contentDocument) {
                frame_doc = frame.contentDocument;
            } else if(frame.contentWindow) {
                frame_doc = frame.contentWindow.document;
            } else {
                if(document.frames) {
                    frame_doc = window.top.document.frames[frameName].document;
                    if(frame_doc == window.top.document)
                        frame_doc = null;
                }
            }
        }
    }
    catch (e) {
        frame_doc = null;
    }
    if(!frame_doc && !ignoreNotFoundError)
        throw new Error("Frame document not found: " + frameName, "getFrameDocument");

    return frame_doc;
}

var gMaxFrameContactAttemptCount = 20;


/*
Keep trying to execute a command a certain number of times if it throws an error.
Used for inter frame communication.
Usage: keepTrying(commandStr, [errorOnFail]);
*/
function keepTrying(commandStr, errorOnFail, attemptCount) {
    attemptCount = attemptCount || 0;
    errorOnFail = errorOnFail || false;

    try {
        eval(commandStr);
    }
    catch(e) {
        if(++attemptCount < gMaxFrameContactAttemptCount)
            // Try again in half a second.
            setTimeout("keepTrying(" + commandToString(commandStr) + ", " + errorOnFail + ", " + attemptCount + ")", 500);
        else if(errorOnFail)
            throw new Error("keepTrying failed to run command " + commandToString(commandStr) + ".");
    }
}

function commandToString(commandStr) {
    return "\"" + commandStr.replace(/"/g, "\\\"") + "\"";
}

// Break out of a frameset if currently in one.
function ExitFrameset() {
    // If we cannot access the top frame's href then we know we need to break out.
    var top_href;
    try{
        top_href = top.location.href;
    }
    catch (e) {
        top_href = "";
        e = null;
    }
    if(window.location.href != top_href)
        top.location.replace(window.location.href);
}

function getItemCoords(domNode) {
    var coords = {x: 0, y: 0};
    do {
        coords.x += domNode.offsetLeft;
        coords.y += domNode.offsetTop;
    } while ((domNode = domNode.offsetParent));

    return coords;
}

function scrollItemIntoView(domNode, ignoreHorizontal, ignoreVertical) {
    var coords = getItemCoords(domNode);
    window.scrollTo(ignoreHorizontal ? 0 : coords.x, ignoreVertical ? 0 : coords.y);
}

// Determine how far from the top of the page an item is.
function getOffsetTop(item, top_offset) {
  var returnValue = item.offsetTop + item.offsetHeight;
  while((item = item.offsetParent) != null)returnValue += item.offsetTop;
  return returnValue + top_offset;
}

// Determine how far from the left of the page an item is.
function getOffsetLeft(item, left_offset) {
  var returnValue = item.offsetLeft;
  while((item = item.offsetParent) != null)returnValue += item.offsetLeft;
  return returnValue + left_offset;
}

//Set Item Position relative to passed element
function PositionMyElement(ele, item, offset_left, offset_top)
{
	if(!offset_left) offset_left = 0;
	if(!offset_top) offset_top = 2;
	
	ele.style.left = getOffsetLeft(item, offset_left) + 'px';
	ele.style.top = getOffsetTop(item, offset_top) + 'px';
	
	//alert("Left: " + getOffsetLeft(item,0) + " Top: " + getOffsetTop(item,0));
}

function initScrollInPlace(wrapperName, scrollAnchorID, setFocus, minHeight, matchDocWidth, widthOffset, heightOffset) {
    var wrapper = document.getElementById(wrapperName);
    if(!wrapper) return;

    // Set the DIV to scroll as needed and to clip to its own rectangle.
    wrapper.style.overflow = "auto";

    // Set the onresize event and prevent document scrolling.
    registerOnResizeFunc(function() {stretchToDocBottom(document.getElementById(wrapperName), minHeight, matchDocWidth, widthOffset, heightOffset);});
    document.body.style.overflow = "hidden";

    // Start scrolling immediately, don't wait until page load.
    stretchToDocBottom(wrapper, minHeight, matchDocWidth, widthOffset, heightOffset);

    // Set up browser-compliant scrolling peripheral slavery.
    if(scrollAnchorID) {
        // This causes Mozilla forms to lose their focus on mouse movement.
        /*
        if(wrapper.addEventListener)
            wrapper.addEventListener("mouseover", function () {obeyScrollInput(wrapperName, scrollAnchorID);}, false);
        else if(wrapper.attachEvent)
            wrapper.attachEvent("onmouseover", function () {obeyScrollInput(wrapperName, scrollAnchorID);});
        */

        // NoOp.
        scrollAnchorID = scrollAnchorID;
    }
    else if(setFocus && wrapper.focus) {
        wrapper.focus();
    }

    registerOnLoadFunc(function() {stretchToDocBottom(document.getElementById(wrapperName), minHeight, matchDocWidth, widthOffset, heightOffset);});
}

// Obey the scroll input from a user's peripherals such as mouse-wheel, arrow keys, and page-up-and-down.
var gCurrentFocusName = "";
function obeyScrollInput(wrapperName, scrollAnchorID) {
    var wrapper = document.getElementById(wrapperName);
    var scroll_anchor = document.getElementById(scrollAnchorID);

    if(wrapper && scroll_anchor && wrapperName != gCurrentFocusName) {
        gCurrentFocusName = wrapperName;
        var scroll_top = wrapper.scrollTop;
        scroll_anchor.focus();
        wrapper.scrollTop = scroll_top;
    }
}

// Make an item take up the rest of the height of the page.
function stretchToDocBottom(item, minHeight, matchDocWidth, widthOffset, heightOffset) {
    minHeight = minHeight || 0;
    widthOffset = widthOffset || 0;
    heightOffset = heightOffset || 0;

    // See how far the item is from the top of the screen.
    var item_top = getOffsetTop(item) + heightOffset;

    // Decide on the desired height of the item.  Don't go lower than the minimum.
    var desired_height;
    if(window.innerHeight)
        desired_height = window.innerHeight - item_top;
    else
        desired_height = document.body.clientHeight - item_top;

    if(desired_height < minHeight)
        desired_height = minHeight;

    // Set the desired height.
    item.style.height = Math.max(desired_height, 0);

    if(matchDocWidth) {
        var window_width;
        if(window.innerWidth)
            window_width = window.innerWidth;
        else
            window_width = document.body.clientWidth;

        item.style.width = Math.max(window_width - widthOffset, 0);
    }
}

function getEventTarget(eventObj, getParentIfNotDiv) {
    var target = eventObj.target ? eventObj.target : eventObj.srcElement;
    var tag_name = target.tagName;

    if(getParentIfNotDiv) {
        if(target.tagName != "DIV")
            target = target.parentNode;

        target.originalTagName = tag_name;
    }

    return target;
}

function shadowLinkClick(actualHref, isJavaScript) {
    var propogate_click;
    if(isJavaScript) {
        eval(actualHref);
        propogate_click = false;
    }
    else {
        this.href = actualHref;
        propogate_click = true;
    }

    return propogate_click;
}

function getWindowSize() {
    if(window.innerHeight)
        return {width: window.innerWidth, height: window.innerHeight};
    else
        return {width: document.body.clientWidth, height: document.body.clientHeight};
}

function centerItem(itemID, noHoriz, noVert) {
    noHoriz = noHoriz || false;
    noVert = noVert || false;

    var item = document.getElementById(itemID);
    if(!item) return;

    var window_size = getWindowSize();

    if(!noHoriz)
        item.style.left = window_size.width/2 - item.offsetLeft/2;

    if(!noVert)
        item.style.top = window_size.height/2 - item.offsetHeight/2;
}

function stretchItem(item) {
    var window_size = getWindowSize();
    item.style.width = window_size.width;
    item.style.height = window_size.height;
}

function stretchDarkSheet() {
    stretchItem(document.getElementById("idDarkSheet"));
}

function centerQuickPage() {
    centerItem("QuickPageWrapper", true);
}

var gOpacityType = 0;
function setOpacity(item, percentage) {
    // Determine the supported opacity type.
	var BrowserType = GetBrowserType();
    if(gOpacityType == 0) {
        if(BrowserType == 1)
            gOpacityType = 1;
        else if(BrowserType == 2)
            gOpacityType = 2;
		else if(BrowserType == 3)
            gOpacityType = 2;
		else
			gOpacityType = 2;
    }

    var clear_opacity = isNaN(parseInt(percentage));
	//alert(BrowserType);
    // Set the opacity.
    switch(gOpacityType) {
        case 1:
            item.style.filter = clear_opacity ? "" : "alpha(opacity=" + percentage + ")";
            break;
        case 2:
            item.style.opacity = clear_opacity ? "" : percentage / 100;
            break;
        case 3:
            item.style.MozOpacity = clear_opacity ? "" : percentage / 100;
            break;
    }
}

// Apply a style to an element and all its children.
function styleRecurse(elem, styleToApply) {
    if (elem.childNodes) {
        for (var i = 0; i < elem.childNodes.length; i++) {
            styleRecurse(elem.childNodes[i], styleToApply);
        }
    }
    if (elem.style) {
        elem.id = (elem.id == "" ? "idStyleRecurseTemp" : elem.id);
        while (document.getElementById(elem.id).length) elem.id = "idStyleRecurseTemp" + Math.round(Math.random * 100000);
        eval("document.getElementById('" + elem.id + "').style." + styleToApply);
        elem.id = (elem.id.indexOf("idStyleRecurseTemp") >= 0 ? "" : elem.id);
    }
}

// Get the value of any named form item.
function formValue(formName, itemName) {
    try {
        var item = document.forms[formName][itemName];
        if (item.length) {
            if (item.selectedIndex) return item[item.selectedIndex].value;
            else {
                var to_return = "";
                if (item[0].type.toLowerCase() == "radio") {
                    for (var i = 0; i < item.length; i++) {
                        if (item[i].checked) return item[i].value;
                    }
                }
                else {
                    for (var i = 0; i < item.length; i++) {
                        if (item[i].checked) {
                            if (to_return != "") to_return += ",";
                            to_return += item[i].value;
                        }
                    }
                    return to_return;
                }
            }
        }
        else {
            if (item.type.toLowerCase() == "checkbox" || item.type.toLowerCase() == "radio")
                return (item.checked ? item.value : "");
            else
                return item.value;
        }
    }
    catch (e) {
        return "";
    }
}

// When using an object as a dictionary, use this to remove an item.
// NOTE: passed by value, not reference, so we have to return the new one.
function removeItemFromObject(obj, item) {
    var new_obj = new Object();
    for (var i in obj) {
        if (i != item) new_obj[i] = obj[i];
    }
    return new_obj;
}
 function IterateObject(obj)
 {
	var txt = "";
	try {
		for (var name in obj) {
		  txt += "key:" + name + "\n";
		}
		
	  } catch (err) {
		txt += "Unknown error: " + err.description + "\n";
	  }
	  alert(txt);
 }
 
 function inspect(obj, maxLevels, level)
{
  var str = '', type, msg;

    // Start Input Validations
    // Don't touch, we start iterating at level zero
    if(level == null)  level = 0;

    // At least you want to show the first level
    if(maxLevels == null) maxLevels = 1;
    if(maxLevels < 1)     
        return '<font color="red">Error: Levels number must be > 0</font>';

    // We start with a non null object
    if(obj == null)
    return '<font color="red">Error: Object <b>NULL</b></font>';
    // End Input Validations

    // Each Iteration must be indented
    str += '<ul>';

    // Start iterations for all objects in obj
    for(property in obj)
    {
      try
      {
          // Show "property" and "type property"
          type =  typeof(obj[property]);
          str += '<li>(' + type + ') ' + property + 
                 ( (obj[property]==null)?(': <b>null</b>'):('')) + '</li>';

          // We keep iterating if this property is an Object, non null
          // and we are inside the required number of levels
          if((type == 'object') && (obj[property] != null) && (level+1 < maxLevels))
          str += inspect(obj[property], maxLevels, level+1);
      }
      catch(err)
      {
        // Is there some properties in obj we can't access? Print it red.
        if(typeof(err) == 'string') msg = err;
        else if(err.message)        msg = err.message;
        else if(err.description)    msg = err.description;
        else                        msg = 'Unknown';

        str += '<li><font color="red">(Error) ' + property + ': ' + msg +'</font></li>';
      }
    }

      // Close indent
      str += '</ul>';

    return str;
}

//////////////////////////////////////////////////////////////////////////////////////////
//Date Validate Functions
// Date Vaidate Function (mm/dd/yyyy)**************************
// Declaring valid date character, minimum year and maximum year
var dtCh= "/";
var minYear=1900;
var maxYear=2100;

function isInteger(s){
	var i;
    for (i = 0; i < s.length; i++){
        // Check that current character is number.
        var c = s.charAt(i);
        if (((c < "0") || (c > "9"))) return false;
    }
    // All characters are numbers.
    return true;
}

function stripCharsInBag(s, bag){
	var i;
    var returnString = "";
    // Search through string's characters one by one.
    // If character is not in bag, append to returnString.
    for (i = 0; i < s.length; i++){
        var c = s.charAt(i);
        if (bag.indexOf(c) == -1) returnString += c;
    }
    return returnString;
}

function daysInFebruary (year){
	// February has 29 days in any year evenly divisible by four,
    // EXCEPT for centurial years which are not also divisible by 400.
    return (((year % 4 == 0) && ( (!(year % 100 == 0)) || (year % 400 == 0))) ? 29 : 28 );
}
function DaysArray(n) {
	for (var i = 1; i <= n; i++) {
		this[i] = 31;
		if (i==4 || i==6 || i==9 || i==11) {this[i] = 30};
		if (i==2) {this[i] = 29};
   }
   return this
}

function CheckValidDate(dtStr, dtField){
	
	if(dtStr.length == 0)
	{
		return true;
	}
	
	var daysInMonth = DaysArray(12);
	var pos1=dtStr.indexOf(dtCh);
	var pos2=dtStr.indexOf(dtCh,pos1+1);
	var strMonth=dtStr.substring(0,pos1);
	var strDay=dtStr.substring(pos1+1,pos2);
	var strYear=dtStr.substring(pos2+1);
	strYr=strYear;
	if (strDay.charAt(0)=="0" && strDay.length>1) strDay=strDay.substring(1);
	if (strMonth.charAt(0)=="0" && strMonth.length>1) strMonth=strMonth.substring(1);
	for (var i = 1; i <= 3; i++) {
		if (strYr.charAt(0)=="0" && strYr.length>1) strYr=strYr.substring(1);
	}
	month=parseInt(strMonth);
	day=parseInt(strDay);
	year=parseInt(strYr);
	if (pos1==-1 || pos2==-1){
		alert("The date format should be : mm/dd/yyyy");
		dtField.value = "";
		return false;
	}
	if (strMonth.length<1 || month<1 || month>12){
		alert("Please enter a valid month");
		dtField.value = "";
		return false;
	}
	if (strDay.length<1 || day<1 || day>31 || (month==2 && day>daysInFebruary(year)) || day > daysInMonth[month]){
		alert("Please enter a valid day");
		dtField.value = "";
		return false;
	}
	if (strYear.length != 4 || year==0 || year<minYear || year>maxYear){
		alert("Please enter a valid 4 digit year between "+minYear+" and "+maxYear);
		dtField.value = "";
		return false;
	}
	if (dtStr.indexOf(dtCh,pos2+1)!=-1 || isInteger(stripCharsInBag(dtStr, dtCh))==false){
		alert("Please enter a valid date");
		dtField.value = "";
		return false;
	}
return true;
}

function ConvertDate(dt)
{
	try
	{
		var day = dt.getDate();
		var mnth = dt.getMonth() + 1;
		var year = dt.getFullYear();
		if (mnth <= 9) mnth = '0' + mnth;
		
		return mnth + "/" + day + "/" + year;
	}
	catch(e)
	{
		return dt;
	}
	
}

function ConvertTime(dt)
{
	try
	{
		var currentTime = new Date(dt);
		var hours = currentTime.getHours();
		var minutes = currentTime.getMinutes();
		var yr = currentTime.getFullYear();
		//alert(yr);
		if(yr > 1980)
		{
			var suffix = "AM";
			if (hours >= 12) {
			  suffix = "PM";
			  hours = hours - 12;
			}
			if (hours == 0) {
				hours = 12;
			}

			if (minutes < 10)
				minutes = "0" + minutes;

			return hours + ":" + minutes + " " + suffix;
		}

	}
	catch(e)
	{
		return dt;
	}
	
}

//////////////////////////////////////////////////////////////////////////////////////////
// Proper Case Functions
// This function makes all cases proper for given text.
/////////////////////////////////////////////////////////////////////////////////////////
function PCase(STRING){
	var strReturn_Value = "";
	var iTemp = STRING.length;
	if(iTemp==0){
		return"";
	}
	var UcaseNext = false;
	strReturn_Value += STRING.charAt(0).toUpperCase();
	for(var iCounter=1;iCounter < iTemp;iCounter++){
		if(UcaseNext == true){
			strReturn_Value += STRING.charAt(iCounter).toUpperCase();
		}
		else{
			strReturn_Value += STRING.charAt(iCounter).toLowerCase();
		}
	var iChar = STRING.charCodeAt(iCounter);
	if(iChar == 32 || iChar == 45 || iChar == 46){
		UcaseNext = true;
		}
	else{
		UcaseNext = false
		}
	if(iChar == 99 || iChar == 67){
	if(STRING.charCodeAt(iCounter-1)==77 || STRING.charCodeAt(iCounter-1)==109){
		UcaseNext = true;
		}
	}

} //End For

return strReturn_Value;
} //End Function


/*
Mask Script
Usage:
Use On actions: OnKeyUp and OnBlur in text field.
	replace 'location1,location2' with the locations where you want the delimiter
	replace the 'delimiter' with the separating character you would like
	javascript:return mask(this.value,this,'location1,location2','delimiter')
*/
function mask(str,textbox,loc,delim){
var locs = loc.split(',');

for (var i = 0; i <= locs.length; i++){
	for (var k = 0; k <= str.length; k++){
	 if (k == locs[i]){
	  if (str.substring(k, k+1) != delim){
	    str = str.substring(0,k) + delim + str.substring(k,str.length);
	  }
	 }
	}
 }
  textbox.value = str;
}
//End Function

function checknum_val(ele){
	var x=ele
	var anum=/(^\d+$)|(^\d+\.\d+$)/
	if (x.length > 0){
		if (anum.test(x)){
			return true;
			}
		else{
			return false;
		}
	}	
}

function checknum(ele){
	var x=ele.value
	var anum=/(^\d+$)|(^\d+\.\d+$)/
	if (x.length > 0){
		if (anum.test(x))
		{
			return true;
		}
		else
		{
			alert("This value must not contain any characters other than numbers!")
			ele.value = 0;
			ele.focus();
			return false;
		}
	}	
}

function checknum_order(ele){
	var x = ele.value;
	var anum=/(^\d+$)|(^\d+\.\d+$)/
	if (x.length > 0){
		if (anum.test(x)){
			return true;
			}
		else{
			alert("This Job Number must not contain any characters other than numbers!")
			ele.value = "";
			return false;
			}
		}	
	}

function checknum_alert(id){
	var x = id.value;
	var anum=/(^\d+$)|(^\d+\.\d+$)/
	//alert(id.value + ", " + x.length);
	if (x.length > 0){
		if (anum.test(x)){
			return true;
		}
	else{
			alert("Please input a valid number!");
			id.value = 0;
			return false;
		}
	}	
}

/*
Rad Menu Client
Usage:
For Rad Menu Client Events
*/
function MyRadClick(item)
{
	try
	{
		switch(item.Category)
		{
			case "popWin":
				eval(item.Value);
				break;
			case "script":
				eval(item.Value);
				break;
			case "GetHelp":
				OpenHelpDoc();
				break;
		}
	}
	catch(e)
	{
		alert("This script is not currently available on this page.");
	}
}

function OpenHelpDoc()
{
	try
	{
		var co = myCoType;
		if(co == 'CLIENT')
		{
			window.open('http://clienthelp.nvms.com');
		}
		
		if(co == 'FIELDREP')
		{
			window.open('http://rephelp.nvms.com');
		}
	}
	catch(e)
	{
		alert(e.message + " - This script is not currently available on this page.");
	}
}

//Get Popup Window Script ("myWin Anchor")
function popWin(url, pw, ph){
	var myPopWin = new PopupWindow();
	myPopWin.offsetX=50;
	myPopWin.offsetY=20;
	myPopWin.setSize(pw, ph);
	myPopWin.setUrl(url);
	myPopWin.autoHide();
	myPopWin.showPopup('content');
}

//Get Popup Window Script (Passed Anchor)
function popWinFull(url, pw, ph, anchor){
	var tPopWin = new PopupWindow();

	if(pw == 0) pw = screen.width;
	if(ph == 0) ph = screen.height-80;
	
	tPopWin.offsetX=-900;
	tPopWin.offsetY=-900;
	tPopWin.setSize(pw, ph);
	tPopWin.setUrl(url);
	tPopWin.showPopup(anchor);
	
	 // params  = 'width='+screen.width;
	 // params += ', height='+screen.height;
	 // params += ', top=0, left=0';
	 // params += ', scrollbars=yes';
	 // params += ', fullscreen=yes';

	 // var newwin = window.open(url,'windowname4', params);
	 // if (window.focus) {newwin.focus()}
	 // return false;
}

//Get Popup Window Script (Passed Anchor)
function popWinThis(url, pw, ph, anchor){
	var tPopWin = new PopupWindow();
	var window_size = getWindowSize();

	tPopWin.offsetX=25;
	tPopWin.offsetY=-500;
	
	if(pw == 0) pw = window_size.width;
	if(ph == 0) ph = window_size.height;
	
	tPopWin.setSize(pw, ph);
	tPopWin.setUrl(url);
	tPopWin.autoHide();
	tPopWin.showPopup(anchor);
}

//Get Popup Window Script (Passed Anchor)
function popWinContent(div, content, pw, ph, anchor){
	var tPopWin = new PopupWindow();
	tPopWin.offsetX=25;
	tPopWin.offsetY=-500;
	tPopWin.setSize(pw, ph);
	tPopWin.contents = content;
	tPopWin.autoHide();
	tPopWin.showPopup(div);
}

//Get Popup Window For Div
function popWinDiv(div, pw, ph, anchor){
	var tPopWin = new PopupWindow();
	tPopWin.offsetX=25;
	tPopWin.offsetY=-500;
	tPopWin.setSize(pw, ph);
	//tPopWin.autoHide();
	tPopWin.contents = document.getElementById(div).innerHTML;
	tPopWin.showPopup(div);
}

function popWinNoHide(url, pw, ph, ref){
	
	var myPopWin = new PopupWindow();
	myPopWin.offsetX=50;
	myPopWin.offsetY=20;
	
	if(pw == 0) pw = window_size.width;
	if(ph == 0) ph = window_size.height;
	
	myPopWin.setSize(pw, ph);
	myPopWin.setUrl(url);
	
	if(ref != null)
		myPopWin.showPopup(ref);
	else
		myPopWin.showPopup('content');
	
}

// Set the position of the popup window based on the anchor
function PopupWindow_getXYPosition(anchorname) {
	var coordinates;
	if (this.type == "WINDOW") {
		coordinates = getAnchorWindowPosition(anchorname);
		}
	else {
		coordinates = getAnchorPosition(anchorname);
		}
	this.x = coordinates.x;
	this.y = coordinates.y;
	}
// Set width/height of DIV/popup window
function PopupWindow_setSize(width,height) {
	this.width = width;
	this.height = height;
	}
// Fill the window with contents
function PopupWindow_populate(contents) {
	this.contents = contents;
	this.populated = false;
	}
// Set the URL to go to
function PopupWindow_setUrl(url) {
	this.url = url;
	}
// Set the window popup properties
function PopupWindow_setWindowProperties(props) {
	this.windowProperties = props;
	}
// Refresh the displayed contents of the popup
function PopupWindow_refresh() {
	if (this.divName != null) {
		// refresh the DIV object
		if (this.use_gebi) {
			document.getElementById(this.divName).innerHTML = this.contents;
			}
		else if (this.use_css) {
			document.all[this.divName].innerHTML = this.contents;
			}
		else if (this.use_layers) {
			var d = document.layers[this.divName];
			d.document.open();
			d.document.writeln(this.contents);
			d.document.close();
			}
		}
	else {
		if (this.popupWindow != null && !this.popupWindow.closed) {
			if (this.url!="") {
				this.popupWindow.location.href=this.url;
				}
			else {
				this.popupWindow.document.open();
				this.popupWindow.document.writeln(this.contents);
				this.popupWindow.document.close();
			}
			this.popupWindow.focus();
			}
		}
	}
// Position and show the popup, relative to an anchor object
function PopupWindow_showPopup(anchorname) {
	this.getXYPosition(anchorname);
	
	this.x += this.offsetX;
	this.y += this.offsetY;
	if (!this.populated && (this.contents != "")) {
		this.populated = true;
		this.refresh();
		}
	if (this.divName != null) {
		// Show the DIV object
		if (this.use_gebi) {
			document.getElementById(this.divName).style.left = this.x;
			document.getElementById(this.divName).style.top = this.y;
			document.getElementById(this.divName).style.visibility = "visible";
			}
		else if (this.use_css) {
			document.all[this.divName].style.left = this.x;
			document.all[this.divName].style.top = this.y;
			document.all[this.divName].style.visibility = "visible";
			}
		else if (this.use_layers) {
			document.layers[this.divName].left = this.x;
			document.layers[this.divName].top = this.y;
			document.layers[this.divName].visibility = "visible";
			}
		}
	else {
		if (this.popupWindow == null || this.popupWindow.closed) {
			// If the popup window will go off-screen, move it so it doesn't
			if (this.x<0) { this.x=0; }
			if (this.y<0) { this.y=0; }
			if (screen && screen.availHeight) {
				if ((this.y + this.height) > screen.availHeight) {
					this.y = screen.availHeight - this.height;
					}
				}
			if (screen && screen.availWidth) {
				if ((this.x + this.width) > screen.availWidth) {
					this.x = screen.availWidth - this.width;
					}
				}
			this.popupWindow = window.open("about:blank","win_"+anchorname,this.windowProperties+",width="+this.width+",height="+this.height+",screenX="+this.x+",left="+this.x+",screenY="+this.y+",top="+this.y+"");
			}
		this.refresh();
		}
	}
// Hide the popup
function PopupWindow_hidePopup() {
	if (this.divName != null) {
		if (this.use_gebi) {
			document.getElementById(this.divName).style.visibility = "hidden";
			}
		else if (this.use_css) {
			document.all[this.divName].style.visibility = "hidden";
			}
		else if (this.use_layers) {
			document.layers[this.divName].visibility = "hidden";
			}
		}
	else {
		if (this.popupWindow && !this.popupWindow.closed) {
			this.popupWindow.close();
			this.popupWindow = null;
			}
		}
	}
// Pass an event and return whether or not it was the popup DIV that was clicked
function PopupWindow_isClicked(e) {
	if (this.divName != null) {
		if (this.use_layers) {
			var clickX = e.pageX;
			var clickY = e.pageY;
			var t = document.layers[this.divName];
			if ((clickX > t.left) && (clickX < t.left+t.clip.width) && (clickY > t.top) && (clickY < t.top+t.clip.height)) {
				return true;
				}
			else { return false; }
			}
		else if (document.all) { // Need to hard-code this to trap IE for error-handling
			var t = window.event.srcElement;
			while (t.parentElement != null) {
				if (t.id==this.divName) {
					return true;
					}
				t = t.parentElement;
				}
			return false;
			}
		else if (this.use_gebi) {
			var t = e.originalTarget;
			while (t.parentNode != null) {
				if (t.id==this.divName) {
					return true;
					}
				t = t.parentNode;
				}
			return false;
			}
		return false;
		}
	return false;
	}

// Check an onMouseDown event to see if we should hide
function PopupWindow_hideIfNotClicked(e) {
	if (this.autoHideEnabled && !this.isClicked(e)) {
		this.hidePopup();
		}
	}
// Call this to make the DIV disable automatically when mouse is clicked outside it
function PopupWindow_autoHide() {
	this.autoHideEnabled = true;
	}
// This global function checks all PopupWindow objects onmouseup to see if they should be hidden
function PopupWindow_hidePopupWindows(e) {
	for (var i=0; i<popupWindowObjects.length; i++) {
		if (popupWindowObjects[i] != null) {
			var p = popupWindowObjects[i];
			p.hideIfNotClicked(e);
			}
		}
	}
// Run this immediately to attach the event listener
function PopupWindow_attachListener() {
	if (document.layers) {
		document.captureEvents(Event.MOUSEUP);
		}
	window.popupWindowOldEventListener = document.onmouseup;
	if (window.popupWindowOldEventListener != null) {
		document.onmouseup = new Function("window.popupWindowOldEventListener(); PopupWindow_hidePopupWindows();");
		}
	else {
		document.onmouseup = PopupWindow_hidePopupWindows;
		}
	}
	
// CONSTRUCTOR for the PopupWindow object
// Pass it a DIV name to use a DHTML popup, otherwise will default to window popup
function PopupWindow() {
	if (!window.popupWindowIndex) { window.popupWindowIndex = 0; }
	if (!window.popupWindowObjects) { window.popupWindowObjects = new Array(); }
	if (!window.listenerAttached) {
		window.listenerAttached = true;
		PopupWindow_attachListener();
		}
	this.index = popupWindowIndex++;
	popupWindowObjects[this.index] = this;
	this.divName = null;
	this.popupWindow = null;
	this.width=0;
	this.height=0;
	this.populated = false;
	this.visible = false;
	this.autoHideEnabled = false;

	this.contents = "";
	this.url="";
	this.windowProperties="toolbar=no,location=no,status=yes,menubar=no,scrollbars=yes,resizable,alwaysRaised,dependent,titlebar=no";
	if (arguments.length>0) {
		this.type="DIV";
		this.divName = arguments[0];
		}
	else {
		this.type="WINDOW";
		}
	this.use_gebi = false;
	this.use_css = false;
	this.use_layers = false;
	if (document.getElementById) { this.use_gebi = true; }
	else if (document.all) { this.use_css = true; }
	else if (document.layers) { this.use_layers = true; }
	else { this.type = "WINDOW"; }
	this.offsetX = 0;
	this.offsetY = 0;
	// Method mappings
	this.getXYPosition = PopupWindow_getXYPosition;
	this.populate = PopupWindow_populate;
	this.setUrl = PopupWindow_setUrl;
	this.setWindowProperties = PopupWindow_setWindowProperties;
	this.refresh = PopupWindow_refresh;
	this.showPopup = PopupWindow_showPopup;
	this.hidePopup = PopupWindow_hidePopup;
	this.setSize = PopupWindow_setSize;
	this.isClicked = PopupWindow_isClicked;
	this.autoHide = PopupWindow_autoHide;
	this.hideIfNotClicked = PopupWindow_hideIfNotClicked;
	}

// getAnchorPosition(anchorname)
//   This function returns an object having .x and .y properties which are the coordinates
//   of the named anchor, relative to the page.
function getAnchorPosition(anchorname) {
	// This function will return an Object with x and y properties
	var useWindow=false;
	var coordinates=new Object();
	var x=0,y=0;
	// Browser capability sniffing
	var use_gebi=false, use_css=false, use_layers=false;
	if (document.getElementById) { use_gebi=true; }
	else if (document.all) { use_css=true; }
	else if (document.layers) { use_layers=true; }
	// Logic to find position
 	if (use_gebi && document.all) {
		x=AnchorPosition_getPageOffsetLeft(document.all[anchorname]);
		y=AnchorPosition_getPageOffsetTop(document.all[anchorname]);
		}
	else if (use_gebi) {
		var o=document.getElementById(anchorname);
		x=AnchorPosition_getPageOffsetLeft(o);
		y=AnchorPosition_getPageOffsetTop(o);
		}
 	else if (use_css) {
		x=AnchorPosition_getPageOffsetLeft(document.all[anchorname]);
		y=AnchorPosition_getPageOffsetTop(document.all[anchorname]);
		}
	else if (use_layers) {
		var found=0;
		for (var i=0; i<document.anchors.length; i++) {
			if (document.anchors[i].name==anchorname) { found=1; break; }
			}
		if (found==0) {
			coordinates.x=0; coordinates.y=0; return coordinates;
			}
		x=document.anchors[i].x;
		y=document.anchors[i].y;
		}
	else {
		coordinates.x=0; coordinates.y=0; return coordinates;
		}
	coordinates.x=x;
	coordinates.y=y;
	return coordinates;
	}

// getAnchorWindowPosition(anchorname)
//   This function returns an object having .x and .y properties which are the coordinates
//   of the named anchor, relative to the window
function getAnchorWindowPosition(anchorname) {
	var coordinates=getAnchorPosition(anchorname);
	var x=0;
	var y=0;
	if (document.getElementById) {
		if (isNaN(window.screenX)) {
			x=coordinates.x-document.body.scrollLeft+window.screenLeft;
			y=coordinates.y-document.body.scrollTop+window.screenTop;
			}
		else {
			x=coordinates.x+window.screenX+(window.outerWidth-window.innerWidth)-window.pageXOffset;
			y=coordinates.y+window.screenY+(window.outerHeight-24-window.innerHeight)-window.pageYOffset;
			}
		}
	else if (document.all) {
		x=coordinates.x-document.body.scrollLeft+window.screenLeft;
		y=coordinates.y-document.body.scrollTop+window.screenTop;
		}
	else if (document.layers) {
		x=coordinates.x+window.screenX+(window.outerWidth-window.innerWidth)-window.pageXOffset;
		y=coordinates.y+window.screenY+(window.outerHeight-24-window.innerHeight)-window.pageYOffset;
		}
	coordinates.x=x;
	coordinates.y=y;
	return coordinates;
	}

// Functions for IE to get position of an object
function AnchorPosition_getPageOffsetLeft (el) {
	var ol=el.offsetLeft;
	while ((el=el.offsetParent) != null) { ol += el.offsetLeft; }
	return ol;
	}
function AnchorPosition_getWindowOffsetLeft (el) {
	return AnchorPosition_getPageOffsetLeft(el)-document.body.scrollLeft;
	}
function AnchorPosition_getPageOffsetTop (el) {
	var ot=el.offsetTop;
	while((el=el.offsetParent) != null) { ot += el.offsetTop; }
	return ot;
	}
function AnchorPosition_getWindowOffsetTop (el) {
	return AnchorPosition_getPageOffsetTop(el)-document.body.scrollTop;
	}


function getBusinessDays(BusDays) {
  var now = new Date();
  var dayOfTheWeek = now.getDay();
  var calendarDays = BusDays;
  var deliveryDay = dayOfTheWeek + BusDays;
  if (deliveryDay >= 6) {
    BusDays -= 6 - dayOfTheWeek;  //deduct this-week days
	calendarDays += 2;  //count this coming weekend
	deliveryWeeks = Math.floor(BusDays / 5); //how many whole weeks?
	calendarDays += deliveryWeeks * 2;  //two days per weekend per week
  }
  now.setTime(now.getTime() + calendarDays * 24 * 60 * 60 * 1000);
 
 var fDay = now.getDate();
 var fMonth = now.getMonth() + 1;
 var fYear = now.getFullYear();
 
 return fMonth + "/" + fDay + "/" + fYear
 
}

//remove spaces from str
function nospace(ele, key) {
	ele.value = key.replace(/ /g, "");
} 


//  AJAX (Client-Side) Helper Functions //////////////////////////////////////////////////////////////////////////

function escapeNull(val, ret)
{
	if(val == null)
		return ret;
	else
		return val;
}

function IntYesNo(val)
{
	if(val == 1)
		return true;
	else
		return false;
}

function JYesNo(val)
{
	if(val)
		return 1;
	else
		return 0;
}

function VBYesNo(val)
{
	if(val == "True")
		return 1;
	else
		return 0;
}

function BoolFromStr(val)
{
	if(val == "true")
		return true;
	else
		return false;
}

function VBYesNoToggle(val)
{
	if(val == "True")
		return 0;
	else
		return 1;
}

function ImageCheckImg(val)
{
	if(val)
		return "checkbox_checked";
	else
		return "checkbox_unchecked";

}

function Left(str, n)
{
   if (n <= 0)
         return "";
   else if (n > String(str).length)
         return str;
   else
         return String(str).substring(0,n);
}

function RTrim(VALUE){
	var w_space = String.fromCharCode(32);
	var v_length = VALUE.length;
	var strTemp = "";

	if(v_length < 0){
		return"";
	}

	var iTemp = v_length -1;

	while(iTemp > -1){
		if(VALUE.charAt(iTemp) == w_space){
	}
		else{
		strTemp = VALUE.substring(0,iTemp +1);
		break;
	}
	iTemp = iTemp-1;

	} //End While
	return strTemp;

} //End Function

function LimitText(ele, limitNum) {
	var limitField = ele;
	if (limitField.value.length > limitNum) {
		limitField.value = limitField.value.substring(0, limitNum);
	} else {
		SetMyHTML("LimitField", limitNum - limitField.value.length);
	}
}

//Capture cross-browser HTML data with values (Firefox, Chrome)
// Usage: ('#element').forhtml()

(function ($) {
    var oldHTML = $.fn.html;

    $.fn.formhtml = function () {
        if (arguments.length) return oldHTML.apply(this, arguments);
        $("input", this).each(function () {
            this.setAttribute('value', this.value);
        });
        $("textarea", this).each(function () {
            this.defaultValue = this.value;
        });
        $(":radio,:checkbox", this).each(function () {
            // im not really even sure you need to do this for "checked"
            // but what the heck, better safe than sorry
            if (this.checked) this.setAttribute('checked', 'checked');
            else this.removeAttribute('checked');
        });
        $("option", this).each(function () {
            // also not sure, but, better safe...
            if (this.selected) this.setAttribute('selected', 'selected');
            else this.removeAttribute('selected');
        });
        return oldHTML.apply(this);
    };

    //optional to override real .html() if you want
    // $.fn.html = $.fn.formhtml;
})(jQuery);
// ****************** End Helper Functions ****************************************************//////////////////
