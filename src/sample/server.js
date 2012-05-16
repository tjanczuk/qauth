var fs = require('fs')
	, path = require('path')
	, port = 31415;

var routes = {
	'/checkout.html': fs.readFileSync(path.resolve(__dirname, 'checkout.html')),
	'/scripts/phone2web.js': fs.readFileSync(path.resolve(__dirname, 'scripts/phone2web.js'))
};

var contentTypes = {
	'/checkout.html': 'text/html',
	'/scripts/phone2web.js': 'application/javascript'
};

require('http').createServer(function (req, res) {
	if (req.method === 'GET' && routes[req.url]) {
		res.writeHead(200, { 'Content-Type': contentTypes[req.url]});
		res.end(routes[req.url]);
	}
	else {
		res.writeHead(404);
		res.end();
	}
}).listen(process.env.PORT || port);