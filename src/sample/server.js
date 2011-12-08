var static = require('node-static')
	, port = 31415;

var file = new(static.Server)('./');

require('http').createServer(function (request, response) {
  request.addListener('end', function () {
    file.serve(request, response);
  });
}).listen(process.env.PORT || port);