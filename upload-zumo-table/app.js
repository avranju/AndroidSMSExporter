var https = require("https");
var fs = require("fs");

// load messages json
var messages;
fs.readFile("messages.json", {encoding: "utf8"}, function(err, data) {
    messages = JSON.parse(data);
    var done = 0;
    messages.forEach(function(msg) {
        for(var p in msg) {
            if(!msg[p]) {
                msg[p] = "";
            }
        }

        var options = {
            hostname: 'smsbackup.azure-mobile.net',
            port: 443,
            path: '/tables/sms',
            method: 'POST',
            headers: {
                "Content-Type": "application/json",
                "X-ZUMO-APPLICATION": "IaOVBmcqPoSUOVxSKWoWXVIxPlPrvE62"
            }
        };

        var request = https.request(options, function(response) {
            process.stdout.write("Inserted " + (++done) + " of " + messages.length + " rows.\r");
        });

        request.on('error', function(e) {
            console.log('problem with request: ' + e.message);
        });

        request.write(JSON.stringify(msg));
        request.end();
    });
});
