// this is a client side JS library that helps fill out web forms using QR codes

function () {
	
	var self = null;
	
	for (var n in document.scripts) {
		if (document.scripts[n].src.toLowerCase().indexOf("phone2web.js") !== -1) {
			self = document.scripts[n];
			break; 
		}
	}

	if (!self)
		throw "A script tag that includes the phone2web.js script has not been found on the page."

	var options = JSON.parse(self.text);

	// apply defaults

	options.qrid = options.qrid || "qr";
	if (!options.)
}();