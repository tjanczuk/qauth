// this is a phone2web middleware that helps relay data from a mobile phone to a web client

var http = require("http")
	, requests = {}
	, messages = {}
	, requestTimeout = 20000
	, messageTimeout = 30000
	, port = 31416;

http.createServer(function(req, res) {
    console.log(req.method + ' ' + req.url);

	var requestid = req.url.substring(1);

	if (isNaN(requestid)) {
		res.writeHead(400);
		res.end();
	} 
	else if (req.method === 'GET') {
		// request for message
		var entry = messages[requestid];
		if (entry) {
			// message is waiting for this requestid
			console.log('Found message waiting for ' + requestid);
			clearTimeout(entry.timer);
			res.writeHead(200, { "Content-type": "application/json", "Cache-Control": "no-cache", "Access-Control-Allow-Origin": "*" });
			res.end(entry.data);
			delete messages[requestid];
		}
		else {
			// no message is waiting - park request
			console.log('Parking request ' + requestid);
			requests[requestid] = {
				res: res,
				timer: setTimeout(function() {
					// request timed out waiting for matching message
					console.log('Timing out request ' + requestid);
					res.writeHead(404, { "Cache-Control": "no-cache", "Access-Control-Allow-Origin": "*" });
					res.end();
					delete requests[requestid];
				}, requestTimeout)
			};
		}
	}
	else if (req.method === 'POST') {
		// message arrived
		var body = "";
		req.on('data', function(chunk) { body += chunk; });
		req.on('end', function() {
			var entry = requests[requestid];
			if (entry) {
				console.log('Found request waiting for ' + requestid);
				// request is waithing for this message
				clearTimeout(entry.timer);
				entry.res.writeHead(200, { "Content-type": "application/json", "Cache-Control": "no-cache", "Access-Control-Allow-Origin": "*" });
				entry.res.end(body);
				delete requests[requestid];
			}
			else {
				// no request is waiting for the message - park the message
				console.log('Parking message ' + requestid)
				messages[requestid] = {
					data: body,
					timer: setTimeout(function() {
						// message timed out waiting for matching request
						console.log('Timing out message ' + requestid)
						delete messages[requestid];
					}, messageTimeout)
				}
			}

			res.writeHead(201);
			res.end();
		});
	}
	else
	{
		res.writeHead(405);
		res.end();
	}
}).listen(process.env.PORT || port);

console.log('Relay listening on ' + (process.env.PORT || port));