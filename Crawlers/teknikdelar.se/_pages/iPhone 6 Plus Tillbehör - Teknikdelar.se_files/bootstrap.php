
//create pop-up element
var ceiframe = document.createElement("iframe");

ceiframe.src = "https://www.ehandelscertifiering.se/lv5/pratbubbla_tom.php";
ceiframe.id = "ceiframe";
ceiframe.frameBorder = 0;
ceiframe.allowTransparency = true;
ceiframe.marginWidth = 0;
ceiframe.marginHeight = 0;
ceiframe.scrolling = "no";
ceiframe.onmouseover = function() { clearTimeout(cetimer); }
ceiframe.onmouseout = function() { cehide1(); }
ceiframe.style.position = "absolute";
ceiframe.style.display = "none";
ceiframe.style.width = "310px";
ceiframe.style.height = "310px";
ceiframe.style.zIndex = "99999";


document.body.appendChild(ceiframe);


function cescroll(xy) {

var xScroll, yScroll;
if (self.pageYOffset) {
	yScroll = self.pageYOffset;
	xScroll = self.pageXOffset;
} else if (document.documentElement && document.documentElement.scrollTop) {
	yScroll = document.documentElement.scrollTop;
	xScroll = document.documentElement.scrollLeft;
} else if (document.body) {// all other Explorers
	yScroll = document.body.scrollTop;
	xScroll = document.body.scrollLeft;
}

if(xy == "x") return xScroll;
if(xy == "y") return yScroll;

}


function cepos(obj, xy) {

var curleft = 0;
var curtop = 0;

if(obj.offsetParent) {
	do {
		curleft += obj.offsetLeft;
		curtop += obj.offsetTop;
	} while (obj = obj.offsetParent);
}

//cescroll to get relativ to viewport
if(xy == "x") return curleft - cescroll("x");
if(xy == "y") return curtop - cescroll("y");

}


function ceshow1() {

img_w = document.getElementById('ceimg1').width;
img_h = document.getElementById('ceimg1').height;

img_x = cepos(document.getElementById('ceimg1'), 'x');
img_y = cepos(document.getElementById('ceimg1'), 'y');


var margin = 10;
var pop_w = 310;
var pop_h = 310;
var fit = "";
var just_x = 0;
var just_y = 0;


//x axis

if(img_x > pop_w + margin) {
	fit = "left";
	
} else if(img_x > (pop_w / 2) + margin) {
	fit = "center";

} else {
	fit = "right";
}

//y axis

if(img_y > pop_h + margin) {
	fit = fit + "top";

} else if(img_y > (pop_h / 2) + margin  && fit != "center") {
	fit = fit + "center";

} else {
	fit = fit + "bottom";
}



ceObj = document.getElementById('ceiframe');

var url = "https://www.ehandelscertifiering.se/lv5/pratbubbla.php?url=www.teknikdelar.se&lang=se&pos=" + fit;
if(ceObj.src != url) {
    ceObj.src = url;
}

if(fit.substr(0,4) == "left") just_x = just_x - pop_w;
if(fit.substr(0,6) == "center") just_x = just_x - (pop_w / 2) + (img_w / 2);
if(fit.substr(0,5) == "right") just_x = just_x + img_w;

if(fit.indexOf("top") != -1) just_y = just_y - pop_h;
if(fit.indexOf("center") != -1 && fit.substr(0,6) != "center") just_y = just_y - (pop_h / 2) + (img_h / 2);
if(fit.indexOf("bottom") != -1) just_y = just_y + img_h;

//adjust for that we are not calculating from celogo postion but rather body 0,0
just_x = just_x + img_x + cescroll("x");
just_y = just_y + img_y + cescroll("y");

ceObj.style.left = just_x + 'px';
ceObj.style.top = just_y + 'px';

ceObj.style.display = "block";

}


function cehide1(delay) {

	ceObj = document.getElementById('ceiframe');

	if(delay) {
		cetimer = setTimeout("ceObj.style.display = 'none'", 600);
	} else {
		ceObj.style.display = "none";
	}
}


/*
function cepos(element, xy) {

//The counters for the total offset values.
var left = 0;
var top = 0;
//Loop while this element has a parent.
while (element.offsetParent) { 
//Sum the current offsets with the total.
left += element.offsetLeft;
top += element.offsetTop; 
//Switch position to this element's parent.
element = element.offsetParent;
}
//Do a final increment in case there was no parent or if
//the last parent has an offset.
left += element.offsetLeft;
top += element.offsetTop;

//return {x:left, y:top};
if(xy == "x") return left;
if(xy == "y") return top;

}
*/

//activeate javascript on logo, now that we know the functions being called are loaded (they are in this file)
//check if it exists first. in can be nonexisten in the case of code loaded for taget frames (where only popup code exists) or errors

if(typeof(document.getElementById("celink1")) != "undefined") {
var celink1 = document.getElementById("celink1");
celink1.onmouseover = function() { ceshow1(); }
celink1.onmouseout = function() { cehide1(true); }

//set taget link of whole logo, reuse input data to this script
celink1.href = "https://www.ehandelscertifiering.se/info.php?lang=se&autolang=yes&url=www.teknikdelar.se";
}

if(typeof(document.getElementById("ceimg1")) != "undefined") {
document.getElementById("ceimg1").src = "https://www.ehandelscertifiering.se/lv5/logotyp.php?size=65&bg=&url=www.teknikdelar.se";
}
