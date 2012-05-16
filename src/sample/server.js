var fs = require('fs')
	, path = require('path')
	, port = 31415;

var routes = {
	'/checkout.html': {
		body: fs.readFileSync(path.resolve(__dirname, 'checkout.html')),
		contentType: 'text/html'
	},
	'/scripts/phone2web.js': {
		body: fs.readFileSync(path.resolve(__dirname, 'scripts/phone2web.js')),
		contentType: 'application/javascript'
	}
};

require('http').createServer(function (req, res) {
	if (req.method === 'GET' && routes[req.url]) {
		res.writeHead(200, { 'Content-Type': routes[req.url].contentType});
		res.end(routes[req.url].body);
	}
	else {
		res.writeHead(404);
		res.end();
	}
}).listen(process.env.PORT || port);