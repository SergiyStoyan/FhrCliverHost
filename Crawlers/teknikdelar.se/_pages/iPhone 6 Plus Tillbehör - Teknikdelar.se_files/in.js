var _instore_log_http_referrer = false;


function _instore_qstring2obj(qstring) {
	var values = {};
	if (!qstring) return values;
	var qstringarray = qstring.split('&');
	for(var i=0;i<qstringarray.length;i++) {
		var a = qstringarray[i].split('=');
		var vname = unescape(a[0]);
		var val = unescape(a[1]);
		values[vname]=val;
	}
	return values;
}



function _instore_set_cookie(name,value,days,path,domain)
{
	if (days)
	{
		var date = new Date();
		date.setTime(date.getTime()+(days*24*60*60*1000));
		var expires = "; expires="+date.toGMTString();
	}
	else var expires = "";
	document.cookie = name+"="+escape(value)+expires+(path?'; path='+path:'')+(domain?'; domain='+domain:'');
}


function _instore_get_cookie(name)
{
	var nameEQ = name + "=";
	var ca = document.cookie.split(';');
	for(var i=0;i < ca.length;i++)
	{
		var c = ca[i];
		while (c.charAt(0)==' ') c = c.substring(1,c.length);
		if (c.indexOf(nameEQ) == 0) return unescape(c.substring(nameEQ.length,c.length));
	}
	return null;
}

function _instore_erase_cookie(name)
{
	_instore_set_cookie(name,"",-1);
}

function _instore_insertref(refs, r){
	for(var i=0; i<refs.length; i++){
		var refdata = refs[i].split('|');
		var ref = refdata[0];
		var ts = refdata[1];
		if (ref==r) {
			//finns redan, uppdatera timestamp
			var t = new Date();
			refs[i]=ref+'|'+t.getTime();
			return;
		}
	}
	//finns inte, lagg till ny
	var t = new Date();
	refs.push(r+'|'+t.getTime());
};


function _instore_in()
{
	var qstring = window.location.search;
	qstring = qstring.substr(1);
	var q = _instore_qstring2obj(qstring);
	var ref = q['ref']+'';
	if (qstring.indexOf('prisjakt')>-1) ref='prisjakt';
	var refcookie = _instore_get_cookie('pjref')+'';
	
	var refs = [];
	if (refcookie && refcookie!='null') {
		refs = refcookie.split(',');
	}

	
	if (ref && ref!='undefined') _instore_insertref(refs, ref);
	
	if (_instore_log_http_referrer) {
		if (document.referrer && document.referrer+''!='null') {
			var r = document.referrer;
			var p = r.indexOf('://');
			r = r.substr(p+3);
			if(r.indexOf('www.')==0) r = r.substr(4);
			p = r.indexOf('/');
			r = r.substr(0,p);
			if(ref=='prisjakt' && r.indexOf('prisjakt.')==0) {
				r='';
			}
			if(ref=='pricespy' && r.indexOf('pricespy.co.nz')==0) {
				r='';
			}
			if (r) _instore_insertref(refs, r);
		}
	}
	
	if (refs.length>0) {
		var refstr = refs.join(',');
		
		var dmn=document.domain;
		if (dmn.substring(0,4)=="www.") {
		 dmn=dmn.substring(4,dmn.length);
		}
		_instore_set_cookie('pjref',refstr,30,'/',dmn);
	}
	
}




_instore_in();