
/*
 * GET home page.
 */

exports.index = function(req, res){
    var lines = this.messages.map(function(m) {
        var l = "";
        var first = true;
        for(var p in m) {
            var val = m[p];
            l += (first) ? "" : ",";
            first = false;
            l += ("\"" + (val ? val.toString().replace("\"", "\"\"") : "") + "\"");
        }
        return l + "\r\n";
    });
    res.render('index', { lines: lines });
};
