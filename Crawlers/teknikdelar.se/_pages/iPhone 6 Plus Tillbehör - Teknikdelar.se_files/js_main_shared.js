jQuery.cookie = function(name, value, options) { if (typeof value != 'undefined') {if (value === null) {value = '';options.expires = -1;}var expires = '';if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {var date;if (typeof options.expires == 'number') {date = new Date();date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));} else {date = options.expires;}expires = '; expires=' + date.toUTCString(); }var path = options.path ? '; path=' + (options.path) : '';var domain = options.domain ? '; domain=' + (options.domain) : '';var secure = options.secure ? '; secure' : '';document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');} else {var cookieValue = null;if (document.cookie && document.cookie != '') {var cookies = document.cookie.split(';');for (var i = 0; i < cookies.length; i++) {var cookie = jQuery.trim(cookies[i]);if (cookie.substring(0, name.length + 1) == (name + '=')) {cookieValue = decodeURIComponent(cookie.substring(name.length + 1));break;}}}return cookieValue;}};

var popCartCloseTimer = null;
var popCartChoosingCountry = false;
var popCartBuffer = {"headerHTML":"", "bodyHTML":"", "loading":false};

var sprakKod = "sv";
var cookieCurrencyMult = 1;
var visaExklMoms = "0";
var priserSparadeExklMoms = "0";
var cookieCurrencyPrintFormat = "";
var cartCountryID = 1;
var VAT_FORCE_INCL = 1;
var VAT_FORCE_EXCL = 2;

var animationSettings = {
	aktivt : 1,
	targetX : 10,
	targetY : 10,
	resultat : [],
	resultatIndex : -1
};

initMainShared();

function initMainShared()
{
	// Disable caching of AJAX responses
	$.ajaxSetup ({cache: false});

	// Load cart parameters
	try {
		sprakKod = cartParams.languageCode;
		cookieCurrencyMult = cartParams.currencyRate;
		cookieCurrencyPrintFormat = cartParams.currencyPrintFormat;	
		visaExklMoms = cartParams.showPricesWithoutVAT;
		priserSparadeExklMoms = cartParams.pricesWithoutVAT;
		animationSettings.aktivt = cartParams.animateProducts;		
		cartCountryID = cartParams.countryID;
	}
	catch(err) {}

	// Load external javascripts after page is fully loaded
	$(window).load(function()
	{
		loadExternalScripts();
	});

	$(".showAllSubSub").click(function(){
		subSubID = $(this).data('subsubid');
		$(this).hide();
		$("#subsubcat-" + subSubID + " .hidden-subsubcategory").slideDown('fast');
		return false;
	});

	// Alla köpknappar i en produktruta:
	$(".buybuttonRuta").click(function(event)
	{
		// Stoppa klick på eventuell omslutande ruta
		event.stopImmediatePropagation();

		// Om animationen är avstängd, använd länkens HREF istället
		if (animationSettings.aktivt == 0)
			return true;

		// Produktens kombinations-ID
		var kID = $(this).data('cid');		

		// Skicka iväg bilden!
		var imageHolder = $('#image-holder-'+kID+' img:first');
		animate(kID, imageHolder);

		// Stäng av HREF för länken
		return false;
	});

	// Alla köpknappar i listläge
	$(".buybuttonLista").click(function(event)
	{
		// Stoppa klick på eventuell omslutande ruta
		event.stopImmediatePropagation();

		// Om animationen är avstängd, använd länkens HREF istället
		if (animationSettings.aktivt == 0)
			return true;

		// Produktens kombinations-ID
		var kID = $(this).data('cid');		

		// Skicka iväg bilden!
		var imageHolder = $(this).parent().parent().find('img.produktbildLista:first');

		if (!imageHolder.length) return true;

		animate(kID, imageHolder);

		// Stäng av HREF för länken
		return false;
	});

	// Om vi är på "visa_produkt"
	if (animationSettings.aktivt == 1) {
		$(".visaprodBuyButton").removeAttr('onclick');
	}

	$(".visaprodBuyButton").click(function(event)
	{
		// Kolla att det är OK att köpa önskat antal (funktion i js_visaprod.js)
		if (finnsAntaletILager(true) == false)
			return false;

		// Stoppa klick på eventuell omslutande ruta
		event.stopImmediatePropagation();

		// Om animationen är avstängd, använd länkens HREF istället
		if (animationSettings.aktivt == 0)
			return true;

		// Produktens kombinations-ID
		var kID = document.addToBasketForm.valdKombination.value;
		var antal = document.addToBasketForm.antal.value;

		// Skicka iväg bilden!
		var imageHolder = $("#currentBild img:first");
		animateBig(kID, imageHolder, antal);

		// Stäng av default action för knappen
		return false;		
	});
	
	popCartInit();
}

function loadExternalScripts()
{
	if (typeof externalScripts != 'undefined') {
		documentWrite = document.write;
		document.write = function(x) {
			$(x).appendTo('body');
		}
		for (var i = 0; i < externalScripts.length; i++) {		
			loadExternalScript(externalScripts[i]);
		}
		document.write = documentWrite;
	}
}

function loadExternalScript(scriptTuple) 
{
	if (scriptTuple.src.length > 1) {
		$.ajax({
			type:'GET',
			dataType:'script',
			cache: true,
			url: scriptTuple.src,
			storedScriptTuple: scriptTuple,
			success: function()
			{
				if (this.storedScriptTuple.inline.length > 1) {
					$.globalEval(this.storedScriptTuple.inline);
				}
				if (typeof this.storedScriptTuple.next != 'undefined') {					
					loadExternalScript(this.storedScriptTuple.next);
				}
			}
		});
	}
	else {
		$.globalEval(scriptTuple.inline);
	}
}

/*
Funktion som tar in pris inkl/exkl moms och returnerar exkl/inkl moms beroende på inställningar.
Inpriset ska vara avrundat till max 2 decimaler.
Resultatet avrundas till max 2 decimaler om det har förändrats från inpriset.
*/
function momsFilter(pris, momssats, flags)
{
	switch(flags)
	{
		case 0:		
			if (priserSparadeExklMoms == "1")
			{
				if (visaExklMoms == "1")
					return pris;	//priset är sparat exkl moms och ska visas så
				else
					return myRound(pris * (1 + 0.01 * momssats), 2);	//priset är sparat exkl moms men ska visas inkl
			}
			else
			{
				if (visaExklMoms == "1")
					return myRound(pris / (1 + 0.01 * momssats), 2);	//priset är sparat inkl moms men ska visas exkl moms
				else
					return pris;	//priset är sparat inkl moms och ska visas så
			}	
			break;
			
		case VAT_FORCE_INCL:
			if (priserSparadeExklMoms == "1")
				return myRound(pris * (1 + 0.01 * momssats), 2);	//priset är sparat exkl moms men ska visas inkl
			else
				return pris;	//priset är sparat inkl moms och ska visas så
			break;
			
		case VAT_FORCE_EXCL:
			if (priserSparadeExklMoms == "1")
				return pris;	//priset är sparat exkl moms och ska visas så
			else
				return myRound(pris / (1 + 0.01 * momssats), 2);	//priset är sparat inkl moms men ska visas exkl moms
			break;
	}
}

// Detta anropas när en varukorgsanimation är färdig. Då ska done vara 2 (eller mer).
function handleAnimationResponse(i)
{
	var thisResult = animationSettings.resultat[i];
	if (thisResult.done > 1)
	{	
		// Visa buffer och dölj sedan automatiskt om det itne är en mobil
		if ($("#flagMobile").length > 0) {
			popCartViewBuffer(false);
			$("html, body").animate({ scrollTop: 0 }, "slow");	
		}
		else {
			popCartViewBuffer(true);
		}
	}
}

function animate(kID, imageHolder)
{
	// Lägg varan i korgen direkt.
	animationSettings.resultat.push({"done":0});
	var thisResultatIndex = ++animationSettings.resultatIndex;
	
	//Både ajax och animationen måste vara färdiga innan vi blinkar korgen.
	$.ajax(
	{
		url: "/ajax/?action=cart-additem&lang=" + sprakKod,
		global: false,
		type: "POST",
		data: (
		{
			'combinationID': kID,
			'quantity': "1"
		}),
		dataType: "html",
		success: function(data)
		{
			popCartRefreshBufferData(data,false);
			animationSettings.resultat[thisResultatIndex].done++;
			handleAnimationResponse(thisResultatIndex);
		}
	});
	
	// Korgens och bildens positioner
	var cartPos  = $('#topcart-area').offset();
	var imagePos = imageHolder.offset();
	
	//Kopiera bilden och lägg direkt på <body> på samma position
	var kopia = imageHolder.clone().attr({"alt":"","id":""+thisResultatIndex+"activeanimation"}).appendTo('body');
	
	kopia.css(
	{
		'z-index' : '1000',
		'position' : 'absolute',
		'margin':0,
		'padding':0,
		'left' : imagePos.left,
		'top' : imagePos.top
	});
	
	//Låt bilden flyga iväg
	kopia.animate(
	{
		left: cartPos.left + animationSettings.targetX ,
		top:  cartPos.top + animationSettings.targetY
	}, 1200);

	kopia.animate(
	{
		height: 0
	}, 400, null, function(foo)
	{
		$(this).remove();
		var i = parseInt($(this).attr("id"));
		
		animationSettings.resultat[i].done++;
		handleAnimationResponse(i);				
	});
}

function animateBig(kID, imageHolder, antal)
{
	// Lägg varan i korgen direkt.
	animationSettings.resultat.push({"done":0});
	var thisResultatIndex = ++animationSettings.resultatIndex;
	
	var dataStr = "combinationID="+kID+"&quantity="+antal;
	
	var fritextfalt = $("input.fritextfalt");
	for (var i = 0; i < fritextfalt.length; i++)
	{
		dataStr += "&" + fritextfalt[i].name + "=" + encodeURIComponent(fritextfalt[i].value)
	}
	
	//Både ajax och animationen måste vara färdiga innan vi blinkar korgen.
	$.ajax(
	{
		url: "/ajax/?action=cart-additem&lang=" + sprakKod,
		global: false,
		type: "POST",
		data: dataStr,
		dataType: "html",
		success: function(data)
		{
			popCartRefreshBufferData(data, false);
			animationSettings.resultat[thisResultatIndex].done++;
			handleAnimationResponse(thisResultatIndex);
		}
	});
	
	
	var cartPos  = $('#topcart-area').offset();
	var imagePos = imageHolder.offset();	
	var kopia = imageHolder.clone().attr({"alt":"","id":""+thisResultatIndex+"activeanimation"}).appendTo('body');
	
	kopia.css(
	{
		'z-index' : '1000',
		'position' : 'absolute',
		'margin':0,
		'padding':0,
		'left' : imagePos.left,
		'top' : imagePos.top
	});	
	
	kopia.animate(
	{
		height: 0,
		left: cartPos.left + animationSettings.targetX ,
		top:  cartPos.top + animationSettings.targetY
	}, 1500, null, function(foo)
	{
		$(this).remove();
		var i = parseInt($(this).attr("id"));
		
		animationSettings.resultat[i].done++;
		handleAnimationResponse(i);
	});

}

var felaVarukorg = function()
{
	$("#varukorgtop_text").css('backgroundColor','#CC0005');
	setTimeout("$('#varukorgtop_text').css('backgroundColor','');",500);
}

function failVarukorgen(callback)
{
	$("#varukorgtopholder").load("/ajax/?action=get-popcart&message=2&lang=" + sprakKod, {'rnd' : Math.random()}, callback);

}

function goToURL(x)
{
	this.location.href=x;
}

function getPhrase(keyname,p)
{
	var result = "";
	var post = fras[keyname];
	if (post != undefined)
	{
		if (post.length > 0)
			result = post;
	}	
	if (p.length > 0)
	{
		var pArr = p.split("|");
		for (var i=0; i<pArr.length;i++)
		{
			eval("result = result.replace(/%%"+(i+1)+"/gi, pArr[i]);")
		}
	}
	if (result == "%%NULL")
		result = "";
	return result;
}

function getFracPart(x)
{
	var xStr = x.toString();
	var punktPos = xStr.indexOf(".");
	if (punktPos == -1)
		return "00";
	else
		return xStr.substr(punktPos + 1);
}

function myRound(x,y)	//rundar x till y decimaler. Vid 0.5, runda alltid uppåt.
{
	var tmp = x * Math.pow(10,y);	
	if (Math.floor(tmp) == tmp)
		return tmp / Math.pow(10,y);
	else if (Math.floor(tmp * 2) == tmp * 2)
		return (tmp + 0.5) / Math.pow(10,y);
	else
		return Math.round(tmp) / Math.pow(10,y);
}

//Omvandlar till rätt valuta och rundar med max 2 decimaler
function myMultCurrency(x)
{
	return myRound(cookieCurrencyMult * parseFloat(x), 2);
}

//Formatterar en redan omvandlad summa (ändrar bara strängen alltså)
function myFormatCurrencyStr(x)
{
	var frac, resultat;
	
	frac = getFracPart(x);
		
	if (frac.length == 1)
		frac = frac + "0";
	
	if (frac == "00")
	{
		resultat = cookieCurrencyPrintFormat.replace(/%1/, Math.floor(x).toString());
		resultat = resultat.replace(/\.%2/, "");
		return resultat.replace(/\,%2/, "");
	}
	else
	{
		resultat = cookieCurrencyPrintFormat.replace(/%1/, Math.floor(x).toString());
		return resultat.replace(/%2/, frac);
	}
}

//Tar in ett värde i SEK och ger en formatterad sträng med rätt valutaomvandling
function myFormatCurrency(x)
{
	return myFormatCurrencyStr(myMultCurrency(x));
}

// Sortering av varugrupp
function sorteraGrupp(x)
{
	$.get("/ajax/?action=set-sorting&sorting="+x+"&lang="+sprakKod, function(data)
	{
		window.location.reload(true);
	});	
}

// Visa inkl/exkl moms
function setVATSetting(x)
{
	$.get("/ajax/?action=set-vatmode&excl="+(x == 'exkl' ? '1' : '0')+"&lang="+sprakKod, function(data)
	{
		window.location.reload(true);
	});	
}

function setCurrency(x)
{
	$.get("/ajax/?action=set-currency&currency="+x+"&lang="+sprakKod, function(data)
	{
		window.location.reload(true);
	});	
}

function checkEmail(x)
{
	var filter = /^([a-zA-Z0-9_.-])+@(([a-zA-Z0-9-])+.)+([a-zA-Z0-9]{2,4})+$/;
	return filter.test(x);
}

function nyttLosen()
{
	$("#nyttlosen").removeClass("is-hidden");
	$("#nyttlosenlank").hide();	
}

function isScrolledIntoView(elem)
{
    var docViewTop = $(window).scrollTop();
    var docViewBottom = docViewTop + $(window).height();

    var elemTop = $(elem).offset().top;
    var elemBottom = elemTop + $(elem).height();

    return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop));
}
 
function flashBG(elem, hexColor)
{
	var element = $(elem);
	var originalBGColor = element.css('background-color');
	
	setTimeout(function(){ element.css('background-color',hexColor); }, 200);
	setTimeout(function(){ element.css('background-color',originalBGColor); }, 400);
	
	setTimeout(function(){ element.css('background-color',hexColor); }, 600);
	setTimeout(function(){ element.css('background-color',originalBGColor); }, 800);
	
	setTimeout(function(){ element.css('background-color',hexColor); }, 1000);
	setTimeout(function(){ element.css('background-color',originalBGColor); }, 1200);
}

function setMobileCookie(stayDesktop)
{
	$.cookie('cmobile', stayDesktop ? 'stayDesktop' : 'stayMobile', { expires: 30, path: '/' });
}

function gotoDesktop()
{
	setMobileCookie(true);
	window.location.reload(true);
	return false;
}

function gotoMobile()
{
	setMobileCookie(false);
	window.location.reload(true);
	return false;
}

function popCartOpen()
{
	if (popCartCloseTimer) {
		clearTimeout(popCartCloseTimer);
	}
	
	var cartHeight = parseInt($("#popcart").stop(true,false).css('height'));
	if (cartHeight == 0) {
		popCartFitWidth();
		cartHeight = $("#popcart").show().css({'height':'auto'}).height();
		$("#popcart").css({'height':0}).animate({'height':cartHeight}, 200);
	}
	else {
		if ($("#popcart").is(":visible")) {
			$("#popcart").css({'height':'auto'});
		}
	}
}

function popCartClose()
{
	if (popCartChoosingCountry) {
		return false;
	}
	
	var cartHeight = parseInt($("#popcart").stop(true,false).css('height'));
	if (cartHeight > 0) {		
		$("#popcart").show().animate({'height':0}, 200, function()
		{
			$(this).hide();
		});
	}
}

function popCartToggle()
{
	if (popCartCloseTimer) {
		clearTimeout(popCartCloseTimer);
	}
	var cartHeight = parseInt($("#popcart").stop(true,false).css('height'));
	if (cartHeight == 0) {
		popCartOpen();
	}
 	else {
	 	popCartClose();
 	}
}

function popCartCloseTimed()
{
	if (popCartCloseTimer) {
		clearTimeout(popCartCloseTimer);
	}
	popCartCloseTimer = setTimeout('popCartClose()', 400);
}

function popCartFitWidth()
{
	$("#popcart").css({'min-width': $("#topcart-area").width()});
}

function popCartInit()
{
	$("#topcart-hoverarea").hover(popCartOpen, popCartCloseTimed).click(popCartToggle);
	$("#popcart").hover(popCartOpen, popCartCloseTimed);
}

function popCartTimeoutHandler(level)
{
	switch(level){
		case 2:
			window.location.reload(true);
			break;
		default:
			$("#popcart").html('<div style="text-align:center;margin:20px;"><img src="/design/ajaxloader.gif" alt="Loading"></div>');
			break;
	}
}

function popCartSetItem(itemNumber, newQuantity)
{
	// Ifall det går segt att ändra antal på varorna, hantera denna väntetid
	var slowTimeOut1 = setTimeout('popCartTimeoutHandler(1)', 1000);
	var slowTimeOut2 = setTimeout('popCartTimeoutHandler(2)', 5000);
	
	$.get('/ajax/?action=cart-setitem&quantity=' + newQuantity + '&item=' + itemNumber + '&lang=' + sprakKod, function(data)
	{
		clearTimeout(slowTimeOut1);
		clearTimeout(slowTimeOut2);
		popCartRefreshBufferData(data, true);
	});
	return false;
}

function popCartViewBuffer(autoHide)
{
	$("#topcart-area").html(popCartBuffer.headerHTML);
	$("#popcart").html(popCartBuffer.bodyHTML);
	
	if (popCartBuffer.bodyHTML.length > 5) {
		popCartInit();
		popCartOpen();	
	}
	else {
		$("#popcart").hide();
	}	
	
	if (popCartCloseTimer) {
		clearTimeout(popCartCloseTimer);
	}
	
	if (autoHide) {
		popCartCloseTimer = setTimeout('popCartClose()', 1500);
	}
	
	if($('#nostoActive').val() == 1){
		nostojs(function(api){
		  api.sendTagging();
		});
	}
}

function popCartRefreshBufferData(data, instaView)
{
	var tmpDOM = $(data);
	popCartBuffer.headerHTML = tmpDOM.find("#tmpPopCartHeader").html();
	popCartBuffer.bodyHTML = tmpDOM.find("#tmpPopCartBody").html();
	popCartBuffer.loading = false;
	
	if (instaView) {
		// Visa buffern men dölj inte automatiskt efteråt
		popCartViewBuffer(false);
	}
}

function popCartRefreshBuffer(instaView, autoHide)
{
	popCartBuffer.loading = true;
	$.get("/ajax/?action=get-popcart&lang=" + sprakKod, function(data)
	{
		popCartRefreshBufferData(data, instaView);
	});
}

function popCartCountry(x)
{
	// Prevent closing of box while picking a country
	popCartChoosingCountry = true;
	if (popCartCloseTimer) {
		clearTimeout(popCartCloseTimer);
	}	
	// Load list of countries
	$.get('/ajax/?action=get-countries&lang=' + sprakKod, function(data)
	{
		$(x).before(data);
		$(x).remove();
	});	
	return false;
}

function popCartCountrySelect()
{
	// Ifall det går segt att byta land, hantera denna väntetid
	var slowTimeOut1 = setTimeout('popCartTimeoutHandler(1)', 1000);
	var slowTimeOut2 = setTimeout('popCartTimeoutHandler(2)', 5000);
	
	var countryID = $("#cartCountrySelect").val();
	
	$.get("/ajax/?action=set-country&id="+countryID+'&lang=' + sprakKod, function(data)
	{
		clearTimeout(slowTimeOut1);
		clearTimeout(slowTimeOut2);
		
		popCartChoosingCountry = false;
		if (popCartCloseTimer) {
			clearTimeout(popCartCloseTimer);
		}
		popCartRefreshBuffer(true);
	});	
}
